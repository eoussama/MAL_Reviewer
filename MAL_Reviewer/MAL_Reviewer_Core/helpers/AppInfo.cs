using System;
using System.Reflection;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// The application's information.
    /// </summary>
    public static class AppInfo
    {
        #region Fields

        public const string Title = "MAL Reviewer";

        public const string Author = "Eoussama";

        public const string AuthorEmail = "oussama_gtacj@live.fr";

        public const string AuthorLink = "https://github.com/EOussama";

        public const string License = "MIT license";

        public const string LicenseLink = "https://github.com/EOussama/MAL_Reviewer/blob/master/LICENSE";

        public const string RepoLink = "https://github.com/EOussama/MAL_Reviewer";

        public const string BugReportLink = "https://github.com/EOussama/MAL_Reviewer/issues/new";

        public const string JikanLink = "https://github.com/jikan-me";

        public const string AniListLink = "https://anilist.co";

        public const string FreepikLink = "https://www.freepik.com/";

        public const string FontAwesomeLink = "https://www.flaticon.com/packs/font-awesome";

        #endregion

        #region Properties

        /// <summary>
        /// The application's version, (ex: 0.2.0).
        /// </summary>
        public static string Version => ReadVersion(typeof(AppInfo).Assembly);

        /// <summary>
        /// The application's description.
        /// </summary>
        public static string Description =>
            $"{ Title } is an unofficial productivity booster application that organizes and simplifies writing Anime and Manga reviews for MyAnimeList. " +
            $"For you out there who like to keep records of their reviews on a local drive, you can finally ditch the lengthy manual process of managing your reviews in separate text files and take advantage of { Title } and its wide range of tools.";

        /// <summary>
        /// The link that opens a new email to the author.
        /// </summary>
        public static string ContactLink => $"mailto:{ AuthorEmail }?subject={ Uri.EscapeDataString($"{ Title } message") }";

        #endregion

        #region Public methods

        /// <summary>
        /// Reads the version of an assembly without the build metadata, (ex: 0.2.0+abc123 gives 0.2.0).
        /// </summary>
        /// <param name="assembly"> The assembly. </param>
        public static string ReadVersion(Assembly assembly)
        {
            string version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString(3)
                ?? "0.0.0";

            int metadata = version.IndexOf('+');

            return metadata >= 0 ? version.Substring(0, metadata) : version;
        }

        #endregion
    }
}
