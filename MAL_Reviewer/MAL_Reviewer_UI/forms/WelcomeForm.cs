using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_UI.interfaces;
using MAL_Reviewer_UI.user_controls;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_API.models.UserModels;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_UI.theming;

namespace MAL_Reviewer_UI.forms
{
    /// <summary>
    /// The application's initial form.
    /// The home of the dashboard and the bridge to other parts of the application.
    /// </summary>
    public partial class WelcomeForm : Form, IThemedControl
    {
        #region Fields

        /// <summary>
        /// The size the window opens at, when the screen is large enough.
        /// </summary>
        private static readonly System.Drawing.Size OpeningSize = new System.Drawing.Size(1280, 800);

        /// <summary>
        /// The text of the user button when no MAL account is loaded.
        /// </summary>
        private readonly string loadUserText;

        /// <summary>
        /// The dashboard's loading animation, made for the light theme.
        /// </summary>
        private readonly System.Drawing.Image dashboardLoading;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public WelcomeForm()
        {
            InitializeComponent();

            // Opening large enough for the dashboard, without covering more than the screen.
            System.Drawing.Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Size = new System.Drawing.Size(Math.Min(OpeningSize.Width, screen.Width), Math.Min(OpeningSize.Height, screen.Height));

            // Fetching application info.
            lTitle.Text = AppInfo.Title;
            lVersion.Text = AppInfo.Version;
            loadUserText = bUser.Text;

            // Temporarely removing Animelist and Mangalist tab pages.
            tcDashboard.TabPages.Remove(tpAnimelist);
            tcDashboard.TabPages.Remove(tpMangalist);

            // Horizontal scroll disable
            pChildAnime.HorizontalScroll.Maximum = 0;
            pChildManga.HorizontalScroll.Maximum = 0;
            pChildCharacters.HorizontalScroll.Maximum = 0;
            pChildPeople.HorizontalScroll.Maximum = 0;

            pChildAnime.AutoScroll = true;
            pChildManga.AutoScroll = true;
            pChildCharacters.AutoScroll = true;
            pChildPeople.AutoScroll = true;

            // Creating the card UI.
            CreateCards();
            CreateLists(TargetType.Anime);
            CreateLists(TargetType.Manga);

            // Showing the form in the chosen theme.
            this.dashboardLoading = pbDashBoardLoad.Image;
            Theme.Attach(this);
        }

        #endregion

        #region User Data Load

        /// <summary>
        /// Handles the click that loads a MAL account, or unloads the current one.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BUser_Click(object sender, EventArgs e)
        {
            // Check if the current user is null, (Meaning no user is connected),
            // if yes, opening the LoadUserForm.
            if (Core.User == null)
            {
                ShowLoadUserForm(new LoadUserForm());
            }
            // If not, unloading the connected MAL account.
            else if (DialogResult.Yes == MessageBox.Show($"Do you want to unload the MAL account “{ Core.User.Username }”?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                UnloadUser();
            }
        }

        /// <summary>
        /// Shows a user loading form and disposes it once closed.
        /// </summary>
        /// <param name="loadUserForm"> The form to show. </param>
        private void ShowLoadUserForm(LoadUserForm loadUserForm)
        {
            using (loadUserForm)
            {
                loadUserForm.UserLoadedEvent += FLoadUser_UserLoadedEvent;
                loadUserForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Handles the event of when the user's data is loaded.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="user"> The loaded user, with their lists. </param>
        private void FLoadUser_UserLoadedEvent(object sender, MALUserModel user)
        {
            // Whether the user is shown from their saved data, since MAL couldn't be reached.
            DateTimeOffset? savedAt = (sender as LoadUserForm)?.SavedAt;

            // Toggling the loading UI ON.
            ShowLoading();

            try
            {
                ProfileUpdateUI(user);
                AnimeStatsUpdateUI(user.Anime_stats ?? new AnimeStatsModel());
                MangaStatsUpdateUI(user.Manga_stats ?? new MangaStatsModel());
                FavoritesUpdateUI(user.Favorites ?? new FavoritesModel());
                AnimelistUpdateUI(user.AnimeList, user.Username);
                MangalistUpdateUI(user.MangaList, user.Username);

                Core.SetUser(user, savedAt);
                ShowDashboard(user.AnimeList != null, user.MangaList != null);
            }
            catch (Exception ex)
            {
                UnloadUser();
                MessageBox.Show($"The MAL account could not be displayed.\n\n{ ex.Message }", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveUserData();

            if (savedAt.HasValue)
            {
                MessageBox.Show($"MAL can't be reached right now, so the data of “{ user.Username }” saved on { savedAt.Value.LocalDateTime:f} is shown.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Saves the default user's data, so it can be shown when MAL can't be reached.
        /// Failing to save it doesn't stop the user from being shown; it's tried again when the application closes.
        /// </summary>
        private static void SaveUserData()
        {
            try
            {
                Core.SaveUserData();
            }
            catch (Exception ex) when (ex is System.IO.IOException || ex is UnauthorizedAccessException)
            {
                // Nothing more to do here.
            }
        }

        /// <summary>
        /// Triggers when the form is loaded.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void WelcomeForm_Load(object sender, EventArgs e)
        {
            // Loading nothing when turned off in the user settings.
            if (!Core.Settings.UserSettings.LoadOnStart)
            {
                return;
            }

            // Getting the default user's name.
            string defaultUsername = Core.Settings.UserSettings.DefaultUser;

            // Checking if the username is valid.
            // If yes, automatically loading their data.
            if (!string.IsNullOrWhiteSpace(defaultUsername))
            {
                ShowLoadUserForm(new LoadUserForm(defaultUsername));
            }
            // If not valid.
            else
            {
                string message = "No default MAL username was found, would you like to input yours now?\n\nProviding your MAL username now will save you the trouble of loading it manually every time you open this application.\n\nThe default username can be changed any time in the user's settings, where this question can also be turned off.";

                if (DialogResult.Yes == MessageBox.Show(message, $"Welcome to { lTitle.Text }", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    // Passing true to set the inputted username as the default on.
                    ShowLoadUserForm(new LoadUserForm(true));
                }
            }
        }

        #endregion

        #region Dashboard management

        /// <summary>
        /// Redirects to the user's MAL profile page.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BMALProfile_Click(object sender, EventArgs e) => Shell.Open(((Button)sender).Tag);

        /// <summary>
        /// Redirects to the user's MAL friends page.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BMALFriends_Click(object sender, EventArgs e) => Shell.Open(((Button)sender).Tag);

        /// <summary>
        /// Redirects to the user's Anime list on MAL.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BMALAnimelist_Click(object sender, EventArgs e) => Shell.Open(((Button)sender).Tag);

        /// <summary>
        /// Redirects to the user's Manga list on MAL.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BMALMangalist_Click(object sender, EventArgs e) => Shell.Open(((Button)sender).Tag);

        #endregion

        #region Visual updates

        /// <summary>
        /// Updates the UI with the user's information.
        /// </summary>
        /// <param name="user"> The user's information object. </param>
        private void ProfileUpdateUI(MALUserModel user)
        {
            lUserUsername.Text = user.Username;
            lUserGender.Text = DisplayFormat.OrUnknown(user.Gender);
            lUserJoinDate.Text = DisplayFormat.LongDate(user.Joined);
            lUserBirthday.Text = DisplayFormat.LongDate(user.Birthday);
            lUserLocation.Text = DisplayFormat.OrUnknown(user.Location);
            rtbAbout.Text = DisplayFormat.About(user.About);

            // The avatar is downloaded in the background.
            _ = pbUserImage.LoadImageAsync(user.Image_url, Icons.User);

            ttExtendedInfo.SetToolTip(lUserUsername, lUserUsername.Text);
            ttExtendedInfo.SetToolTip(lUserGender, lUserGender.Text);
            ttExtendedInfo.SetToolTip(lUserJoinDate, lUserJoinDate.Text);
            ttExtendedInfo.SetToolTip(lUserBirthday, lUserBirthday.Text);
            ttExtendedInfo.SetToolTip(lUserLocation, lUserLocation.Text);

            bMALProfile.Tag = string.IsNullOrWhiteSpace(user.Url) ? MalUrls.Profile(user.Username) : user.Url;
            bMALFriends.Tag = MalUrls.Friends(user.Username);
            bMALAnimelist.Tag = MalUrls.AnimeList(user.Username);
            bMALMangalist.Tag = MalUrls.MangaList(user.Username);
        }

        /// <summary>
        /// Updates the UI of Anime stats.
        /// </summary>
        /// <param name="stats"> The user's Anime statistics. </param>
        private void AnimeStatsUpdateUI(AnimeStatsModel stats)
        {
            lvDashAnimeWatching.Text = DisplayFormat.Count(stats.Watching);
            lvDashAnimeCompleted.Text = DisplayFormat.Count(stats.Completed);
            lvDashAnimeOnHold.Text = DisplayFormat.Count(stats.On_hold);
            lvDashAnimeDropped.Text = DisplayFormat.Count(stats.Dropped);
            lvDashAnimePTW.Text = DisplayFormat.Count(stats.Plan_to_watch);
            lvDashAnimeEpisodes.Text = DisplayFormat.Count(stats.Episodes_watched);
            lvDashAnimeRewatches.Text = DisplayFormat.Count(stats.Rewatched);

            lvDashAnimeDaysWatched.Text = DisplayFormat.Decimal(stats.Days_watched);
            lvDashAnimeMeanScore.Text = DisplayFormat.Decimal(stats.Mean_score);
        }

        /// <summary>
        /// Updates the UI of Manga stats.
        /// </summary>
        /// <param name="stats"> The user's Manga statistics. </param>
        private void MangaStatsUpdateUI(MangaStatsModel stats)
        {
            lvDashMangaReading.Text = DisplayFormat.Count(stats.Reading);
            lvDashMangaCompleted.Text = DisplayFormat.Count(stats.Completed);
            lvDashMangaOnHold.Text = DisplayFormat.Count(stats.On_hold);
            lvDashMangaDropped.Text = DisplayFormat.Count(stats.Dropped);
            lvDashMangaPTR.Text = DisplayFormat.Count(stats.Plan_to_read);
            lvDashMangaVolumes.Text = DisplayFormat.Count(stats.Volumes_read);
            lvDashMangaChapters.Text = DisplayFormat.Count(stats.Chapters_read);
            lvDashMangaReread.Text = DisplayFormat.Count(stats.Reread);

            lvDashMangaDaysRead.Text = DisplayFormat.Decimal(stats.Days_read);
            lvDashMangaMeanScore.Text = DisplayFormat.Decimal(stats.Mean_score);
        }

        /// <summary>
        /// Updates the UI of favorite lists.
        /// </summary>
        /// <param name="favorites"> The user's favorites. </param>
        private void FavoritesUpdateUI(FavoritesModel favorites)
        {
            FillFavorites(pChildAnime, lFavAnimeCount, favorites.Anime, "Anime");
            FillFavorites(pChildManga, lFavMangaCount, favorites.Manga, "Manga");
            FillFavorites(pChildCharacters, lFavCharactersCount, favorites.Characters, "Character");
            FillFavorites(pChildPeople, lFavPeopleCount, favorites.People, "Person");
        }

        /// <summary>
        /// Replaces the thumbnails of a favorites panel.
        /// </summary>
        /// <param name="panel"> The panel holding the thumbnails. </param>
        /// <param name="countLabel"> The label showing the number of favorites. </param>
        /// <param name="favorites"> The favorites, in the user's order. </param>
        /// <param name="type"> The kind of favorites, (ex: Anime, Person). </param>
        private void FillFavorites(Panel panel, Label countLabel, IEnumerable<FavTargetModel> favorites, string type)
        {
            List<FavTargetModel> items = (favorites ?? Enumerable.Empty<FavTargetModel>()).ToList();

            panel.SuspendLayout();
            panel.Controls.ClearAndDispose();

            // Controls docked to the top stack upwards, so adding them backwards keeps the user's order.
            for (int i = items.Count - 1; i >= 0; i--)
            {
                panel.Controls.Add(new FavoriteThumbControl(items[i].Name, items[i].Image_url, type)
                {
                    Tag = items[i].Url,
                    Dock = DockStyle.Top
                });
            }

            panel.ResumeLayout();
            countLabel.Text = items.Count.ToString();
        }

        /// <summary>
        /// Updates the UI of Anime list.
        /// </summary>
        /// <param name="animeList"> The user's Anime list, null when it is private. </param>
        /// <param name="username"> The user's name. </param>
        private void AnimelistUpdateUI(IEnumerable<AnimelistEntryModel> animeList, string username)
        {
            UpdateListState((CardControl)tlpAnimeMangaCards.Controls[0], bMALAnimelist, animeList != null, username);

            if (animeList == null)
            {
                return;
            }

            RefreshLists(TargetType.Anime);

            List<AnimelistEntryModel>[] buckets = ListStatus.Split(animeList, anime => anime.Watching_status);

            for (int i = 0; i < buckets.Length; i++)
            {
                ((EntryListControl)tlpAnimelistMain.Controls[i]).UpdateList(buckets[i]);
            }

            ResizeTable(TargetType.Anime);
        }

        /// <summary>
        /// Updates the UI of Manga list.
        /// </summary>
        /// <param name="mangaList"> The user's Manga list, null when it is private. </param>
        /// <param name="username"> The user's name. </param>
        private void MangalistUpdateUI(IEnumerable<MangalistEntryModel> mangaList, string username)
        {
            UpdateListState((CardControl)tlpAnimeMangaCards.Controls[1], bMALMangalist, mangaList != null, username);

            if (mangaList == null)
            {
                return;
            }

            RefreshLists(TargetType.Manga);

            List<MangalistEntryModel>[] buckets = ListStatus.Split(mangaList, manga => manga.Reading_status);

            for (int i = 0; i < buckets.Length; i++)
            {
                ((EntryListControl)tlpMangalistMain.Controls[i]).UpdateList(buckets[i]);
            }

            ResizeTable(TargetType.Manga);
        }

        /// <summary>
        /// Shows whether a list is public on its stats card.
        /// </summary>
        /// <param name="card"> The stats card. </param>
        /// <param name="listButton"> The button that opens the list on MAL. </param>
        /// <param name="isPublic"> Whether the list is public. </param>
        /// <param name="username"> The user's name. </param>
        private static void UpdateListState(CardControl card, Button listButton, bool isPublic, string username)
        {
            card.LabelText = isPublic ? "Public" : "Private";
            card.UpdateTooltip(username);
            listButton.Enabled = isPublic;
        }

        /// <summary>
        /// Creates all the Manga and Anime lists.
        /// </summary>
        /// <param name="type"> The type of entry (Anime or Manga). </param>
        private void CreateLists(TargetType type)
        {
            TableLayoutPanel table = type == TargetType.Anime ? tlpAnimelistMain : tlpMangalistMain;

            table.Controls.AddRange(ListStatus.Labels(type)
                .Select(label => (Control)new EntryListControl(label, type) { Dock = DockStyle.Fill })
                .ToArray());
        }

        /// <summary>
        /// Refreshes the lists' UI.
        /// </summary>
        /// <param name="type"> The type of entry (Anime or Manga). </param>
        private void RefreshLists(TargetType type)
        {
            TableLayoutPanel table = type == TargetType.Anime ? tlpAnimelistMain : tlpMangalistMain;

            table.AutoScroll = false;

            table.Controls.ClearAndDispose();
            table.RowStyles.Clear();

            table.RowCount = 6;

            for (int i = 0; i < table.RowCount; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
            }

            table.AutoScroll = true;

            CreateLists(type);
        }

        /// <summary>
        /// Resizes the table to fit the content.
        /// </summary>
        /// <param name="type"> The type of entry (Anime or Manga). </param>
        private void ResizeTable(TargetType type)
        {
            TableLayoutPanel table = type == TargetType.Anime ? tlpAnimelistMain : tlpMangalistMain;

            for (int i = 0; i < table.RowCount; i++)
            {
                if (i < table.Controls.Count)
                {
                    EntryListControl list = (EntryListControl)table.Controls[i];
                    table.RowStyles[i] = new RowStyle(SizeType.Absolute, list.ListHeight + list.Margin.Bottom);
                }
                else
                {
                    table.RowStyles[i] = new RowStyle(SizeType.Absolute, 0F);
                }
            }
        }

        /// <summary>
        /// Shows the loading animation in place of the dashboard.
        /// </summary>
        private void ShowLoading()
        {
            ResetScroll();

            tcDashboard.SelectedIndex = 0;
            tcDashboard.TabPages.Remove(tpAnimelist);
            tcDashboard.TabPages.Remove(tpMangalist);

            tlpDashboardMain.Visible = false;
            lMALAccPreview.Visible = false;
            pbDashBoardLoad.Visible = true;
            bUser.Enabled = false;
        }

        /// <summary>
        /// Shows the dashboard of the loaded user.
        /// </summary>
        /// <param name="animePublic"> Whether the Anime list is public. </param>
        /// <param name="mangaPublic"> Whether the Manga list is public. </param>
        private void ShowDashboard(bool animePublic, bool mangaPublic)
        {
            if (animePublic)
            {
                tcDashboard.TabPages.Add(tpAnimelist);
            }

            if (mangaPublic)
            {
                tcDashboard.TabPages.Add(tpMangalist);
            }

            bUser.Text = "Unload this MAL account";
            pbDashBoardLoad.Visible = false;
            lMALAccPreview.Visible = false;
            tlpDashboardMain.Visible = true;
            bUser.Enabled = true;
            ResetScroll();
        }

        /// <summary>
        /// Disconnects the current user and goes back to the empty dashboard.
        /// </summary>
        private void UnloadUser()
        {
            Core.UnloadUser();

            ResetScroll();

            tcDashboard.SelectedIndex = 0;
            tcDashboard.TabPages.Remove(tpAnimelist);
            tcDashboard.TabPages.Remove(tpMangalist);

            pChildAnime.Controls.ClearAndDispose();
            pChildManga.Controls.ClearAndDispose();
            pChildCharacters.Controls.ClearAndDispose();
            pChildPeople.Controls.ClearAndDispose();
            RefreshLists(TargetType.Anime);
            RefreshLists(TargetType.Manga);
            pbUserImage.ClearImage();

            bUser.Text = loadUserText;
            pbDashBoardLoad.Visible = false;
            tlpDashboardMain.Visible = false;
            lMALAccPreview.Visible = true;
            bUser.Enabled = true;
        }

        private void ResetScroll()
        {
            tlpDashboardMain.VerticalScroll.Value = 0;
            tlpAnimelistMain.VerticalScroll.Value = 0;
            tlpMangalistMain.VerticalScroll.Value = 0;
        }

        /// <summary>
        /// Creates the UI cards.
        /// </summary>
        private void CreateCards()
        {
            #region Profile card

            CardControl profileInfoCard = new CardControl()
            {
                Dock = DockStyle.Fill,
                Icon = Properties.Resources.icon_user,
                Margin = new Padding(0, 0, 19, 0),
                Title = "User information"
            };

            profileInfoCard.Content.Controls.Add(pCardProfileInfo);
            tlpUserInfo.Controls.Add(profileInfoCard);

            #endregion

            #region Redirect card

            CardControl redirectCard = new CardControl()
            {
                Dock = DockStyle.Fill,
                Icon = Properties.Resources.icon_link,
                Margin = new Padding(19, 0, 0, 0),
                Title = "Redirects"
            };

            redirectCard.Content.Controls.Add(pRedirectCard);
            tlpUserInfo.Controls.Add(redirectCard);

            #endregion

            #region Anime stats card

            CardControl animeStatsCard = new CardControl()
            {
                Dock = DockStyle.Fill,
                Icon = Properties.Resources.icon_anime,
                LabelVisibility = true,
                Margin = new Padding(0, 0, 19, 0),
                Title = "Anime Stats"
            };

            animeStatsCard.Content.Controls.Add(pDashAnimeCard);
            tlpAnimeMangaCards.Controls.Add(animeStatsCard);

            #endregion

            #region Manga stats card

            CardControl mangaStatsCard = new CardControl()
            {
                Dock = DockStyle.Fill,
                Icon = Properties.Resources.icon_manga,
                LabelVisibility = true,
                Margin = new Padding(19, 0, 0, 0),
                Title = "Manga Stats"
            };

            mangaStatsCard.Content.Controls.Add(pDashMangaCard);
            tlpAnimeMangaCards.Controls.Add(mangaStatsCard);

            #endregion

            #region Favorites card

            CardControl favoritesCard = new CardControl()
            {
                Dock = DockStyle.Fill,
                Icon = Properties.Resources.icon_favorite,
                Margin = new Padding(0, 0, 0, 19),
                Title = "Favorites"
            };

            favoritesCard.Content.Controls.Add(tlpFavorites);
            pDashFavorites.Controls.Add(favoritesCard);

            #endregion

            #region About card

            CardControl aboutCard = new CardControl()
            {
                Dock = DockStyle.Fill,
                Icon = Properties.Resources.icon_info,
                Margin = new Padding(0, 19, 0, 0),
                Title = "About"
            };

            aboutCard.Content.Controls.Add(pDashAbout);
            tlpDashboardMain.Controls.Add(aboutCard);

            #endregion
        }

        #endregion

        /// <summary>
        /// Handles the click on the settings button, which opens the settings' panel.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog(this);
            }

            // Persisting whatever was changed.
            Core.Save();
        }

        /// <summary>
        /// Handles the click on the new button, which opens a form for creating a new review.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BNew_Click(object sender, EventArgs e)
        {
            using (NewReviewForm newReviewForm = new NewReviewForm())
            {
                newReviewForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Recolors the dashboard's loading animation for the theme.
        /// </summary>
        public void ApplyTheme()
        {
            pbDashBoardLoad.Image = Theme.Palette == null ? this.dashboardLoading : Icons.Loading(Theme.Palette[ThemeRole.Background]);
        }
    }
}
