// Copyright 2016 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.Speech.V1Beta1;
using Google.Protobuf;
using Grpc.Core;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpeechGrpc = Google.Cloud.Speech.V1Beta1.Speech;

namespace Google.Cloud.Speech.NAudioDemo
{
    public class Recognizer
    {
        private readonly Task _handleResponsesTask;
        private readonly IWaveIn _naudioDevice;
        private readonly AsyncDuplexStreamingCall<StreamingRecognizeRequest, StreamingRecognizeResponse> _grpcCall;

        public Recognizer(SpeechGrpc.SpeechClient grpcClient, IWaveIn naudioDevice)
        {
            _naudioDevice = naudioDevice;
            naudioDevice.DataAvailable += ConsumeData;
            naudioDevice.RecordingStopped += EndCall;
            _grpcCall = grpcClient.StreamingRecognize();
            _handleResponsesTask = HandleResponses();
        }

        private async Task HandleResponses()
        {
            while (await _grpcCall.ResponseStream.MoveNext())
            {
                var response = _grpcCall.ResponseStream.Current;
                if (response.Error != null)
                {
                    var error = response.Error;
                    Console.WriteLine($"Error: {error.Message} ({error.Code})");
                }
                foreach (var text in response.Results.SelectMany(r => r.Alternatives.Select(x => x.Transcript)))
                {
                    Console.WriteLine(text);
                }
            }
            Console.WriteLine("Done!");
        }

        public void Start()
        {
            var configRequest = new StreamingRecognizeRequest
            {
                StreamingConfig = new StreamingRecognitionConfig
                {
                    Config = new RecognitionConfig
                    {
                        Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                        SampleRate = 16000
                    }
                }
            };
            _grpcCall.RequestStream.WriteAsync(configRequest).Wait();
            _naudioDevice.StartRecording();
        }

        public void Stop()
        {
            _naudioDevice.StopRecording();
            _handleResponsesTask.Wait();
        }

        void ConsumeData(object sender, WaveInEventArgs e)
        {
            var request = new StreamingRecognizeRequest
            {
                AudioContent = ByteString.CopyFrom(e.Buffer, 0, e.BytesRecorded)
            };
            // We block until completion here as gRPC streams can only handle one pending
            // request at a time. We could potentially form our own queue...
            _grpcCall.RequestStream.WriteAsync(request).Wait();
        }

        void EndCall(object sender, StoppedEventArgs e)
        {
            _grpcCall.RequestStream.CompleteAsync().Wait();
        }
    }
}
