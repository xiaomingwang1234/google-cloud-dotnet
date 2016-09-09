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
using NAudio.Wave;
using System;

namespace Google.Cloud.Speech.NAudioDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Available input devices:");
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                Console.WriteLine($"{i + 1}: {WaveInEvent.GetCapabilities(i).ProductName}");
            }

            int device = 0;
            while (device < 1 || device > WaveInEvent.DeviceCount)
            {
                Console.Write("Input device to use? ");
                int.TryParse(Console.ReadLine(), out device);
            }

            var input = new WaveInEvent
            {
                DeviceNumber = device - 1,
                WaveFormat = new WaveFormat(16000, 1), // 16KHz, mono, 16 bit.
                BufferMilliseconds = 500,
                NumberOfBuffers = 3
            };

            var client = SpeechClient.Create();
            var grpc = client.GrpcClient;
            var recognizer = new Recognizer(grpc, input);
            recognizer.Start();
            Console.WriteLine("Hit return to finish...");
            Console.ReadLine();
            recognizer.Stop();
        }
    }
}
