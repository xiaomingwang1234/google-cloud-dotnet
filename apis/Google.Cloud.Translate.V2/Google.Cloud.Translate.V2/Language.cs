using Google.Api.Gax;
using Google.Apis.Translate.v2.Data;

namespace Google.Cloud.Translate.V2
{
    /// <summary>
    /// A language supported by the Google Cloud Translate API.
    /// </summary>
    public sealed class Language
    {
        /// <summary>
        /// The descriptive name of the language, e.g. "English". May be null if no target language was
        /// provided when requesting the supported languages.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The code to use for the language when providing this to the API
        /// as a source or target language.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="name">The name of the language. May be null.</param>
        /// <param name="code">The code of the language. Must not be null.</param>
        public Language(string name, string code)
        {
            Name = name;
            Code = GaxPreconditions.CheckNotNull(code, nameof(code));
        }

        internal static Language FromResource(LanguagesResource resource) =>
            new Language(resource.Name, resource.Language);
    }
}
