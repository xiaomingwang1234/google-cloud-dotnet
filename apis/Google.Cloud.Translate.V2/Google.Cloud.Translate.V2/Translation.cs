// Copyright 2017 Google Inc. All Rights Reserved.
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

namespace Google.Cloud.Translate.V2
{
    /// <summary>
    /// The result of a translation operation.
    /// </summary>
    public sealed class Translation
    {
        /// <summary>
        /// The original text (or HTML) that was translated.
        /// </summary>

        public string OriginalText { get; }
        /// <summary>
        /// The translated text.
        /// </summary>

        public string TranslatedText { get; }

        /// <summary>
        /// The source language code, either as provided by the caller
        /// or detected by the server.
        /// </summary>
        public string SourceLanguage { get; }

        /// <summary>
        /// The target language code that the text was translated into.
        /// </summary>
        public string TargetLanguage { get; }

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="originalText">The original text.</param>
        /// <param name="translatedText">The translated text.</param>
        /// <param name="sourceLanguage">The source language code.</param>
        /// <param name="targetLanguage">The target language code.</param>
        public Translation(string originalText, string translatedText, string sourceLanguage, string targetLanguage)
        {
            OriginalText = originalText;
            TranslatedText = translatedText;
            SourceLanguage = sourceLanguage;
            TargetLanguage = targetLanguage;
        }
    }
}
