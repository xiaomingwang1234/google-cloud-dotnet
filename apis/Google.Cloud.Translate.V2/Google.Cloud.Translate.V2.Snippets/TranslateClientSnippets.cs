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

using System;
using Xunit;

namespace Google.Cloud.Translate.V2.Snippets
{
    public class TranslateClientSnippets
    {
        [Fact]
        public void TranslateText()
        {
            // Sample: TranslateText
            TranslateClient client = TranslateClient.Create();
            Translation result = client.TranslateText("It is raining.", LanguageCodes.French);
            Console.WriteLine($"Result: {result.TranslatedText}; detected language {result.SourceLanguage}");
            // End sample

            Assert.Equal("Il pleut.", result.TranslatedText);
            Assert.Equal("en", result.SourceLanguage);
        }

        [Fact]
        public void TranslateHtml()
        {
            // Sample: TranslateHtml
            TranslateClient client = TranslateClient.Create();
            Translation result = client.TranslateHtml("<p><strong>It is raining.</strong></p>", LanguageCodes.French);
            Console.WriteLine($"Result: {result.TranslatedText}; detected language {result.SourceLanguage}");
            // End sample

            Assert.Equal("<p> <strong>Il pleut.</strong> </p>", result.TranslatedText);
            Assert.Equal("en", result.SourceLanguage);
        }

    }
}
