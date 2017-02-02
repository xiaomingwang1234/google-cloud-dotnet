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

using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Google.Cloud.Translate.V2
{
    /// <summary>
    /// Abstract class providing operations for Google Cloud Translate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class is provided to enable testability while permitting
    /// additional operations to be added in the future. See <see cref="Create(GoogleCredential)"/>
    /// and <see cref="CreateAsync(GoogleCredential)"/> to construct instances; alternatively, you can
    /// construct a <see cref="TranslateClientImpl"/> directly from a <see cref="TranslateService"/>.
    /// </para>
    /// <para>
    /// All instance methods declared in this class are virtual, with an implementation which simply
    /// throws <see cref="NotImplementedException"/>. All these methods are overridden in <see cref="TranslateClientImpl"/>.
    /// </para>
    /// </remarks>
    public abstract class TranslateClient
    {
        private static readonly ScopedCredentialProvider _credentialProvider = new ScopedCredentialProvider(
            new[] { "https://www.googleapis.com/auth/cloud-platform" });

        /// <summary>
        /// The underlying translation service object used by this client.
        /// </summary>
        /// <remarks>
        /// The <see cref="TranslateClient"/> class only provides convenience operations built on top of
        /// an existing service object. Any more complex operations which are not supported by this wrapper may wish
        /// to use the same service object as the wrapper, in order to take advantage of its configuration (for authentication,
        /// application naming etc).
        /// </remarks>
        public virtual TranslateService Service { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Translates a single item of text.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="targetLanguage">The code for the language to translate into. Must not be null.</param>
        /// <param name="sourceLanguage">The code for the language to translate from. May be null, in which case the server
        /// will detect the source language.</param>
        /// <returns>The translation of <paramref name="text"/>.</returns>
        public virtual Translation TranslateText(string text, string targetLanguage, string sourceLanguage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translates a single item of HTML.
        /// </summary>
        /// <param name="html">The HTML to translate.</param>
        /// <param name="targetLanguage">The code for the language to translate into. Must not be null.</param>
        /// <param name="sourceLanguage">The code for the language to translate from. May be null, in which case the server
        /// will detect the source language.</param>
        /// <returns>The translation of <paramref name="html"/>.</returns>
        public virtual Translation TranslateHtml(string html, string targetLanguage, string sourceLanguage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translates multiple items of text.
        /// </summary>
        /// <param name="textItems">The text strings to translate. Must not be null or contain null elements.</param>
        /// <param name="targetLanguage">The code for the language to translate into. Must not be null.</param>
        /// <param name="sourceLanguage">The code for the language to translate from. May be null, in which case the server
        /// will detect the source language.</param>
        /// <returns>A list of translations. This will be the same size as <paramref name="textItems"/>, in the same order.</returns>
        public virtual IList<Translation> TranslateText(IEnumerable<string> textItems, string targetLanguage, string sourceLanguage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translates multiple items of HTML.
        /// </summary>
        /// <param name="htmlItems">The HTML strings to translate. Must not be null or contain null elements.</param>
        /// <param name="targetLanguage">The code for the language to translate into. Must not be null.</param>
        /// <param name="sourceLanguage">The code for the language to translate from. May be null, in which case the server
        /// will detect the source language.</param>
        /// <returns>A list of translations. This will be the same size as <paramref name="htmlItems"/>, in the same order.</returns>
        public virtual IList<Translation> TranslateHtml(IEnumerable<string> htmlItems, string targetLanguage, string sourceLanguage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Detects the language of the specified text.
        /// </summary>
        /// <param name="text">The text to detect the language of. Must not be null.</param>
        /// <returns>A list of detected language possibilities.</returns>
        public virtual IList<Detection> DetectLanguage(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lists the language supported by the Translate API.
        /// </summary>
        /// <param name="target">The target language for the </param>
        /// <returns>A list of supported languages.</returns>
        public virtual IList<Language> ListLanguages(string target = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously creates a <see cref="TranslateClient"/>, using application default credentials if
        /// no credentials are specified.
        /// </summary>
        /// <remarks>
        /// The credentials are scoped as necessary.
        /// </remarks>
        /// <param name="credential">Optional <see cref="GoogleCredential"/>.</param>
        /// <returns>The task representing the created <see cref="TranslateClient"/>.</returns>
        public static async Task<TranslateClient> CreateAsync(GoogleCredential credential = null)
        {
            var scopedCredentials = await _credentialProvider.GetCredentialsAsync(credential);
            return CreateImpl(scopedCredentials);
        }

        /// <summary>
        /// Synchronously creates a <see cref="TranslateClient"/>, using application default credentials if
        /// no credentials are specified.
        /// </summary>
        /// <remarks>
        /// The credentials are scoped as necessary.
        /// </remarks>
        /// <param name="credential">Optional <see cref="GoogleCredential"/>.</param>
        /// <returns>The created <see cref="TranslateClient"/>.</returns>
        public static TranslateClient Create(GoogleCredential credential = null)
        {
            var scopedCredentials = _credentialProvider.GetCredentials(credential);
            return CreateImpl(scopedCredentials);
        }

        private static TranslateClient CreateImpl(GoogleCredential scopedCredentials)
        {
            var service = new ModifiedUriTranslateService(new BaseClientService.Initializer
            {
                HttpClientInitializer = scopedCredentials,
                ApplicationName = TranslateClientImpl.ApplicationName,
            });

            return new TranslateClientImpl(service);
        }

        private sealed class ModifiedUriTranslateService : TranslateService
        {
            public ModifiedUriTranslateService() : base() { }
            public ModifiedUriTranslateService(Initializer initializer) : base(initializer) { }

            public override string BaseUri => "https://translation.googleapis.com/language/translate/";
        }
    }
}
