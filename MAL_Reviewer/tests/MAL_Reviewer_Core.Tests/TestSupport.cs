using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_API.sources;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    /// <summary>
    /// A storage location of its own for each test, deleted afterwards.
    /// </summary>
    internal sealed class TempStorage : IDisposable
    {
        public TempStorage()
        {
            Root = Path.Combine(Path.GetTempPath(), "mal-reviewer-tests", Guid.NewGuid().ToString("N"));
            Config = new Config(Root);
        }

        public string Root { get; }

        public Config Config { get; }

        public string File(string name) => Config.GetStorageFilePath(name);

        public void Dispose()
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
        }
    }

    /// <summary>
    /// An API answering with prepared data, recording the usernames it was asked about.
    /// </summary>
    internal sealed class FakeMalApi : IMalApi
    {
        public MALUserModel User { get; set; }

        public IEnumerable<AnimelistEntryModel> AnimeList { get; set; }

        public IEnumerable<MangalistEntryModel> MangaList { get; set; }

        public Exception ListError { get; set; }

        public List<string> RequestedUsers { get; } = new List<string>();

        public Task<SearchResultsModel[]> SearchAsync(MediaKind kind, string title, int limit, CancellationToken ct) => Task.FromResult(Array.Empty<SearchResultsModel>());

        public Task<AnimeModel> GetAnimeAsync(int animeId, CancellationToken ct) => Task.FromResult<AnimeModel>(null);

        public Task<MangaModel> GetMangaAsync(int mangaId, CancellationToken ct) => Task.FromResult<MangaModel>(null);

        public Task<MALUserModel> GetUserAsync(string username, CancellationToken ct)
        {
            RequestedUsers.Add(username);
            return Task.FromResult(User);
        }

        public async Task<IEnumerable<AnimelistEntryModel>> GetAnimeListAsync(string username, CancellationToken ct)
        {
            await Task.Yield();

            if (ListError != null)
            {
                throw ListError;
            }

            return AnimeList;
        }

        public Task<IEnumerable<MangalistEntryModel>> GetMangaListAsync(string username, CancellationToken ct) => Task.FromResult(MangaList);
    }

    /// <summary>
    /// Tests that change the static application core run one at a time.
    /// </summary>
    [CollectionDefinition(nameof(CoreState), DisableParallelization = true)]
    public class CoreState { }
}
