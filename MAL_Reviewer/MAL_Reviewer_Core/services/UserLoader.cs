using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core.helpers;

namespace MAL_Reviewer_Core.services
{
    /// <summary>
    /// Loads everything about a MAL user at once: profile, statistics, favorites and lists.
    /// </summary>
    public class UserLoader
    {
        #region Fields

        private readonly IMalApi api;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="api"> The API to read the user's data from. </param>
        public UserLoader(IMalApi api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads a user's profile and lists in parallel.
        /// A list is null when it is private.
        /// </summary>
        /// <param name="username"> The user's name. </param>
        /// <param name="ct"> The cancellation token that permits the task's cancellation. </param>
        /// <returns> The user with their lists, or null if no such user exists. </returns>
        /// <exception cref="ArgumentException"> When the username is not a valid MAL username. </exception>
        /// <exception cref="DataSourceUnavailableException"> When the data could not be fetched. </exception>
        public async Task<MALUserModel> LoadAsync(string username, CancellationToken ct)
        {
            string name = UsernameRule.Normalize(username);

            if (!UsernameRule.IsValid(name))
            {
                throw new ArgumentException(UsernameRule.InvalidMessage);
            }

            Task<MALUserModel> profile = this.api.GetUserAsync(name, ct);
            Task<IEnumerable<AnimelistEntryModel>> animeList = this.api.GetAnimeListAsync(name, ct);
            Task<IEnumerable<MangalistEntryModel>> mangaList = this.api.GetMangaListAsync(name, ct);

            await Task.WhenAll(profile, animeList, mangaList);

            MALUserModel user = profile.Result;

            if (user == null)
            {
                return null;
            }

            user.AnimeList = animeList.Result;
            user.MangaList = mangaList.Result;

            return user;
        }

        #endregion
    }
}
