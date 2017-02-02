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
using Google.Api.Gax.Rest;
using Google.Apis.Translate.v2;
using Google.Apis.Util;
using System.Collections.Generic;
using System.Linq;

namespace Google.Cloud.Translate.V2
{
    /// <summary>
    /// Wrapper around <see cref="TranslateService"/> to provide simpler operations.
    /// </summary>
    /// <remarks>
    /// This is the "default" implementation of <see cref="TranslateClient"/>. Most client code
    /// should refer to <see cref="TranslateClient"/>, creating instances with
    /// <see cref="TranslateClient.Create(Apis.Auth.OAuth2.GoogleCredential)"/> and
    /// <see cref="TranslateClient.CreateAsync(Apis.Auth.OAuth2.GoogleCredential)"/>. The constructor
    /// of this class is public for the sake of constructor-based dependency injection.
    /// </remarks>
    public sealed class TranslateClientImpl : TranslateClient
    {
        private static readonly object _applicationNameLock = new object();
        private static string _applicationName = UserAgentHelper.GetDefaultUserAgent(typeof(TranslateClient));

        /// <summary>
        /// The default application name used when creating a <see cref="TranslateService"/>.
        /// Defaults to "google-cloud-dotnet"; must not be null.
        /// </summary>
        /// <remarks>
        /// Most applications will never want to set this, which is why it's in this class rather than
        /// <see cref="TranslateClient"/>.
        /// </remarks>
        public static string ApplicationName
        {
            get
            {
                lock (_applicationNameLock)
                {
                    return _applicationName;
                }
            }
            set
            {
                GaxPreconditions.CheckNotNull(value, nameof(value));
                lock (_applicationNameLock)
                {
                    _applicationName = value;
                }
            }
        }

        /// <inheritdoc />
        public override TranslateService Service { get; }

        /// <inheritdoc />
        public override Translation TranslateText(string text, string targetLanguage, string sourceLanguage = null)
        {
            var request = Service.Translations.List(new Repeatable<string>(new[] { text }), targetLanguage);
            request.Source = sourceLanguage;
            request.Format = TranslationsResource.ListRequest.FormatEnum.Text;
            var result = request.Execute().Translations[0];
            return new Translation(text, result.TranslatedText, sourceLanguage ?? result.DetectedSourceLanguage, targetLanguage);
        }

        /// <inheritdoc />
        public override Translation TranslateHtml(string text, string targetLanguage, string sourceLanguage = null)
        {
            var request = Service.Translations.List(new Repeatable<string>(new[] { text }), targetLanguage);
            request.Source = sourceLanguage;
            request.Format = TranslationsResource.ListRequest.FormatEnum.Html;
            var result = request.Execute().Translations[0];
            return new Translation(text, result.TranslatedText, sourceLanguage ?? result.DetectedSourceLanguage, targetLanguage);
        }

        /// <inheritdoc />
        public override IList<Translation> TranslateText(IEnumerable<string> textItems, string targetLanguage, string sourceLanguage = null)
        {
            // Evaluate once so we can zip suitably.
            List<string> items = textItems.ToList();
            var request = Service.Translations.List(new Repeatable<string>(items), targetLanguage);
            request.Source = sourceLanguage;
            request.Format = TranslationsResource.ListRequest.FormatEnum.Text;
            return request.Execute().Translations
                .Zip(items, (result, item) =>
                    new Translation(item, result.TranslatedText, sourceLanguage ?? result.DetectedSourceLanguage, targetLanguage))
                .ToList();
        }

        /// <inheritdoc />
        public override IList<Translation> TranslateHtml(IEnumerable<string> htmlItems, string targetLanguage, string sourceLanguage = null)
        {
            // Evaluate once so we can zip suitably.
            List<string> items = htmlItems.ToList();
            var request = Service.Translations.List(new Repeatable<string>(items), targetLanguage);
            request.Source = sourceLanguage;
            request.Format = TranslationsResource.ListRequest.FormatEnum.Html;
            return request.Execute().Translations
                .Zip(items, (result, item) =>
                    new Translation(item, result.TranslatedText, sourceLanguage ?? result.DetectedSourceLanguage, targetLanguage))
                .ToList();
        }

        /// <inheritdoc />
        public override IList<Detection> DetectLanguage(string text)
        {
            var request = Service.Detections.List(new Repeatable<string>(new[] { text }));
            var result = request.Execute().Detections[0];
            return result.Select(Detection.FromResource).ToList();
        }

        /// <inheritdoc />
        public override IList<Language> ListLanguages(string target = null)
        {
            var request = Service.Languages.List();
            request.Target = target;
            return request.Execute().Languages.Select(Language.FromResource).ToList();
        }

        /// <summary>
        /// Constructs a new client wrapping the given <see cref="TranslateService"/>.
        /// </summary>
        /// <param name="service">The service to wrap. Must not be null.</param>
        public TranslateClientImpl(TranslateService service)
        {
            Service = GaxPreconditions.CheckNotNull(service, nameof(service));
        }
    }
}
