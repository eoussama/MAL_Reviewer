using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using MAL_Reviewer_UI.user_controls;
using MAL_Reviewer_UI.extensions;
using MAL_Reviewer_Core;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.models.ReviewTemplateModels;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models.SearchModels;
using MAL_Reviewer_API.models.TargetModels;
using MAL_Reviewer_API.sources;

namespace MAL_Reviewer_UI.forms
{
    /// <summary>
    /// The form that debuts a review.
    /// </summary>
    public partial class NewReviewForm : Form
    {
        #region Fields

        private bool
            // Whether or not the data has been fetched.
            dataFetched = false,

            // Whether or not a target has been previewed successfully.
            hasPreview = false;

        /// <summary>
        /// The kind of the last previewed target, (the Anime/Manga radio buttons' tag).
        /// </summary>
        private int previewKind = 0;

        /// <summary>
        /// The MAL id of the last previewed target.
        /// </summary>
        private int targetId = 0;

        /// <summary>
        /// Cancels everything when the form closes.
        /// </summary>
        private readonly CancellationTokenSource cts;

        /// <summary>
        /// Whether the target is looked for by title or by link.
        /// </summary>
        private SearchMode mode;

        /// <summary>
        /// Set while the Anime/Manga option is changed by the form itself, so it doesn't start a search.
        /// </summary>
        private bool changingKind = false;

        /// <summary>
        /// Cancels the running search when another one starts.
        /// </summary>
        private CancellationTokenSource searchCts;

        /// <summary>
        /// Cancels the running preview when another one starts.
        /// </summary>
        private CancellationTokenSource previewCts;

        /// <summary>
        /// The loader control.
        /// </summary>
        private readonly LoaderControl previewLoader;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public NewReviewForm()
        {
            InitializeComponent();

            // Wiring up eventhandlers to the radio buttons.
            rbAnime.CheckedChanged += RbAnime_CheckedChanged;
            rbScaleOther.CheckedChanged += RbScaleOther_CheckedChanged;

            // Disabeling horizontal scroll on pSearchCards.
            pSearchCards.HorizontalScroll.Maximum = 0;
            pSearchCards.AutoScroll = true;

            // Populating the search panel with search cards.
            PopulateSearchCards();

            // Create loaders.
            previewLoader = new LoaderControl()
            {
                Color = System.Drawing.SystemColors.Control
            };

            // Wiring the text input events.
            searchControl.Tag = CurrentKind();
            this.searchControl.TextboxSubmitEvent += SearchControl_TextboxSubmitEvent;
            this.searchControl.Controls["inputTextBox"].MouseClick += SearchControl_MouseClick;
            this.searchControl.Controls["inputTextBox"].TextChanged += SearchControl_TextChanged;
            this.searchControl.Controls["iconPictureBox"].MouseClick += PbShow_MouseClick;
            this.searchControl.Controls["iconPictureBox"].MouseEnter += PbShow_MouseEnter;
            this.searchControl.Controls["iconPictureBox"].MouseLeave += PbShow_MouseLeave;

            // Loading the review templates.
            LoadReviewTemplates();

            cts = new CancellationTokenSource();

            // Applying the search settings; typing stays possible while a search runs, and a newer search replaces it.
            SearchController settings = Core.Settings.SearchSettings;

            this.searchControl.AutoSubmit = settings.SearchWhileTyping;
            this.searchControl.SearchInterval = settings.Delay;
            this.searchControl.SubmitOnEnter = true;
            this.searchControl.LockWhileLoading = false;
            this.mode = settings.DefaultMode;

            this.ActiveControl = this.searchControl.Controls["inputTextBox"];
            ApplyMode();
        }

        #endregion

        #region UI Updates

        /// <summary>
        /// Handles the enable toggle of the free scaling picker.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void RbScaleOther_CheckedChanged(object sender, EventArgs e) => nupScaleOther.Enabled = rbScaleOther.Checked;

        /// <summary>
        /// Handles the check state change of the Anime radio button.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void RbAnime_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTexts();
            this.searchControl.Tag = CurrentKind();

            if (!this.changingKind)
            {
                this.searchControl.Submit();
            }
        }

        /// <summary>
        /// Switches between searching by title and by link.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void ModeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.mode = this.mode == SearchMode.Title ? SearchMode.Link : SearchMode.Title;

            // A title isn't a link and vice versa, so the box starts over.
            searchCts?.Cancel();
            this.searchControl.InnerText = string.Empty;
            this.searchControl.ResetSubmit();
            this.dataFetched = false;
            ToggleSearchLoading(false);

            ApplyMode();
            this.ActiveControl = this.searchControl.Controls["inputTextBox"];
        }

        /// <summary>
        /// Shows the controls of the current search mode.
        /// </summary>
        private void ApplyMode()
        {
            bool byLink = this.mode == SearchMode.Link;

            // A link tells by itself whether it is of an Anime or a Manga.
            rbAnime.Enabled = !byLink;
            rbManga.Enabled = !byLink;
            ModeLinkLabel.Text = byLink ? "Search by title instead" : "Search by link instead";

            UpdateTexts();
        }

        /// <summary>
        /// Updates the texts that depend on the search mode and the Anime/Manga option.
        /// </summary>
        private void UpdateTexts()
        {
            string target = CurrentKindName();
            bool byLink = this.mode == SearchMode.Link;

            this.lTitle.Text = byLink ? "MAL link" : $"{ target } title";
            this.lPreview.Text = $"{ target } preview";
            this.TargetNotFoundLabel.Text = HintText(target);

            this.searchControl.Placeholder = byLink ? "MyAnimeList link..." : $"{ target } title...";
            this.searchControl.Icon = CurrentIcon();
        }

        /// <summary>
        /// Unchecking all other review template cards.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void ReviewTemplateCard_ControlCheckedEventHandler(object sender, EventArgs e)
        {
            reviewTemplatesFlowPanel.Controls
                                    .OfType<ReviewTemplatePreviewCardControl>()
                                    .Where(control => control != (ReviewTemplatePreviewCardControl)sender)
                                    .ToList().ForEach(control => control.Check(false));
        }

        /// <summary>
        /// Loads the review templates and displays them in the preview panel.
        /// </summary>
        private void LoadReviewTemplates()
        {
            foreach (ReviewTemplateModel reviewTemplateModel in Core.Settings.ReviewTemplatesSettings.ReviewTemplates)
            {
                ReviewTemplatePreviewCardControl control = new ReviewTemplatePreviewCardControl(reviewTemplateModel);

                control.ControlCheckedEventHandler += ReviewTemplateCard_ControlCheckedEventHandler;
                reviewTemplatesFlowPanel.Controls.Add(control);
            }

            // Update the template count on the group box title.
            reviewTemplatesGroupBox.Text = TemplateSummary.Counter(reviewTemplatesFlowPanel.Controls.Count);
        }

        /// <summary>
        /// Populating the pSearchCards panel with ucTargetSearchCard user controls.
        /// </summary>
        private void PopulateSearchCards()
        {
            foreach (int i in Enumerable.Range(1, 10))
            {
                TargetSearchCardControl searchCard = new TargetSearchCardControl();
                int searchCardCount = pSearchCards.Controls.Count;

                if (searchCardCount < 5)
                    pSearchCards.Height = searchCard.Height * searchCardCount;

                searchCard.CardMouseClickEvent += SearchCard_CardMouseClickEvent;
                searchCard.Top = searchCard.Height * searchCardCount;
                searchCard.Visible = false;
                searchCard.Tag = false;
                pSearchCards.Controls.Add(searchCard);
            }
        }

        #endregion

        #region Target Search

        /// <summary>
        /// The submission of the search query.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The searched text. </param>
        private async void SearchControl_TextboxSubmitEvent(object sender, string e)
        {
            // A newer search replaces the running one.
            searchCts?.Cancel();
            searchCts?.Dispose();

            CancellationTokenSource current = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
            searchCts = current;

            // A link opens its entry directly, whichever the mode.
            if (MalLink.TryParse(e, out MediaKind linkKind, out int linkId))
            {
                OpenLink(linkKind, linkId);
                return;
            }

            if (this.mode == SearchMode.Link)
            {
                TargetNotFoundLabel.Text = $"That isn't the link of an Anime or a Manga on MyAnimeList, (ex: { MalLink.Example }).";
                dataFetched = false;
                searchControl.CompleteSubmit(true);
                ToggleSearchLoading(false);
                return;
            }

            // Reading the search kind now, so changing it during the search doesn't mix the results.
            MediaKind kind = rbAnime.Checked ? MediaKind.Anime : MediaKind.Manga;
            string kindName = CurrentKindName();
            bool succeeded = false;

            ToggleSearchLoading(true);

            try
            {
                SearchResultsModel[] results = await MALHelper.Search(kind.ToPathSegment(), e, Core.Settings.SearchSettings.ResultsLimit, current.Token);

                if (this.IsDisposed || current.IsCancellationRequested)
                {
                    return;
                }

                for (int i = 0; i < pSearchCards.Controls.Count; i++)
                {
                    TargetSearchCardControl searchCard = (TargetSearchCardControl)pSearchCards.Controls[i];

                    // Check if the current user control is withing the range of number of results,
                    // if yes, make it visible and update it.
                    if (i < results.Length)
                    {
                        SearchResultsModel result = results[i];

                        searchCard.UpdateUI(result.Mal_id, result.Title, result.Type, result.Image_url, kindName);
                        searchCard.Visible = true;
                        searchCard.Tag = true;
                    }
                    else
                    {
                        searchCard.Visible = false;
                        searchCard.Tag = false;
                    }
                }

                if (results.Length > 0)
                {
                    pSearchCards.Height = LayoutMath.SearchPanelHeight(results.Length, pSearchCards.Controls[0].Height);
                }

                dataFetched = results.Length > 0;
                succeeded = true;
                TargetNotFoundLabel.Text = dataFetched ? HintText(kindName) : $"No { kindName } with that title was found on MAL.";
            }
            catch (OperationCanceledException)
            {
                // A newer search started, or the form is closing.
                return;
            }
            catch (DataSourceUnavailableException ex) when (!current.IsCancellationRequested)
            {
                // Hiding the results of the previous search, which no longer match the text.
                dataFetched = false;
                TargetNotFoundLabel.Text = HintText(kindName);

                if (!this.IsDisposed)
                {
                    MessageBox.Show($"The search is unavailable right now, please try again later.\n\n{ ex.Message }", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            if (this.IsDisposed || current.IsCancellationRequested)
            {
                return;
            }

            searchControl.CompleteSubmit(succeeded);
            ToggleSearchLoading(false);
            this.ActiveControl = this.searchControl.Controls["inputTextBox"];
        }

        /// <summary>
        /// The click on a search entry that allows previewing a target.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="targetId"> The MAL id of the clicked target. </param>
        private void SearchCard_CardMouseClickEvent(object sender, int targetId)
        {
            // Checking if the targetId isn't equal to the current previewed Anime/Manga's mal_id.
            if (this.targetId == targetId && this.previewKind == CurrentKind() && hasPreview && pPreview.Visible)
                return;

            _ = PreviewAsync(targetId, rbAnime.Checked ? MediaKind.Anime : MediaKind.Manga);
        }

        /// <summary>
        /// Previews the entry of a MyAnimeList link, without searching.
        /// </summary>
        /// <param name="kind"> Whether the link is of an Anime or a Manga. </param>
        /// <param name="malId"> The MAL id of the entry. </param>
        private void OpenLink(MediaKind kind, int malId)
        {
            // Following the link's kind without starting a title search for it.
            this.changingKind = true;
            (kind == MediaKind.Anime ? rbAnime : rbManga).Checked = true;
            this.changingKind = false;
            this.searchControl.Tag = CurrentKind();

            dataFetched = false;
            searchControl.CompleteSubmit(true);
            ToggleSearchLoading(false);
            UpdateTexts();

            if (!(this.targetId == malId && this.previewKind == CurrentKind() && hasPreview && pPreview.Visible))
            {
                _ = PreviewAsync(malId, kind);
            }
        }

        /// <summary>
        /// Toggles the loading animation for the search UI.
        /// </summary>
        /// <param name="state"> The state of the loading; (ON = true, OFF = false). </param>
        private void ToggleSearchLoading(bool state)
        {
            searchControl.ToggleLoading(state);
            pSearchCards.Visible = !state && dataFetched && this.mode == SearchMode.Title && (bool)pSearchCards.Controls[0].Tag == true;

            // Reseting the vertical scrolling.
            pSearchCards.VerticalScroll.Value = 0;
        }

        #region Search panel icon toggler

        /// <summary>
        /// Handles closing the search panel when the input length is lower than 3 characters.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void SearchControl_TextChanged(object sender, EventArgs e)
        {
            if (searchControl.InnerText.Trim().Length < searchControl.SubmitMin)
            {
                pSearchCards.Visible = false;
            }
        }

        /// <summary>
        /// Handles showing the search panel when the textbox is clicked.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void SearchControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && searchControl.Controls["inputTextBox"].Focused && searchControl.InnerText.Trim().Length >= searchControl.SubmitMin && this.mode == SearchMode.Title && dataFetched && (bool)pSearchCards.Controls[0].Tag == true && pSearchCards.Visible == false)
            {
                pSearchCards.Visible = true;
            }
        }

        /// <summary>
        /// Handles closing the search panel.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void PbShow_MouseClick(object sender, MouseEventArgs e)
        {
            pSearchCards.Visible = false;
            searchControl.Controls["iconPictureBox"].Cursor = Cursors.Default;
            searchControl.Icon = CurrentIcon();
        }

        /// <summary>
        /// Handles changing the icon of the textbox depending on the mouse's state (On the icon).
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void PbShow_MouseEnter(object sender, EventArgs e)
        {
            if (searchControl.InnerText.Trim().Length >= searchControl.SubmitMin && pSearchCards.Controls.Count > 0 && pSearchCards.Controls[0]?.Visible == true && pSearchCards.Visible == true)
            {
                searchControl.Controls["iconPictureBox"].Cursor = Cursors.Hand;
                searchControl.Icon = Icons.Close;
            }
            else
            {
                searchControl.Controls["iconPictureBox"].Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles chaning the icon of the textbox depending on the mouse's state (Not on the icon).
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void PbShow_MouseLeave(object sender, EventArgs e) => searchControl.Icon = CurrentIcon();

        #endregion

        #endregion

        #region Preview section update

        /// <summary>
        /// Previews the clicked Anime or Manga. Starting a preview cancels the previous one,
        /// so the last clicked target is the one displayed.
        /// </summary>
        /// <param name="malId"> The MAL id of the target. </param>
        /// <param name="kind"> Whether the target is an Anime or a Manga. </param>
        private async Task PreviewAsync(int malId, MediaKind kind)
        {
            previewCts?.Cancel();
            previewCts?.Dispose();

            CancellationTokenSource current = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
            previewCts = current;

            pPreview.ToggleLoad(true, previewLoader);
            pSearchCards.Visible = false;

            TargetModel target = null;
            string error = null;

            try
            {
                target = kind == MediaKind.Anime
                    ? await MALHelper.GetAnime(malId, current.Token)
                    : (TargetModel)await MALHelper.GetManga(malId, current.Token);

                if (target == null)
                {
                    error = $"This { kind } could not be found on MAL anymore.";
                }
            }
            catch (OperationCanceledException)
            {
                // A newer preview started, or the form is closing.
                return;
            }
            catch (DataSourceUnavailableException ex)
            {
                error = $"The preview is unavailable right now, please try again later.\n\n{ ex.Message }";
            }

            if (this.IsDisposed || current.IsCancellationRequested)
            {
                return;
            }

            pPreview.ToggleLoad(false, previewLoader);

            if (error != null)
            {
                // Keeping the previous preview, or hiding the empty panel if there was none.
                pPreview.Visible = hasPreview;
                ShowPreviewKind(this.previewKind == int.Parse(rbManga.Tag.ToString()) ? MediaKind.Manga : MediaKind.Anime);
                MessageBox.Show(error, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ShowPreview(target, kind);
        }

        /// <summary>
        /// Displays a target's information in the preview panel.
        /// </summary>
        /// <param name="target"> The Anime or Manga. </param>
        /// <param name="kind"> Whether the target is an Anime or a Manga. </param>
        private void ShowPreview(TargetModel target, MediaKind kind)
        {
            ShowPreviewKind(kind);

            lTargetScore.Text = target.Score.HasValue ? DisplayFormat.Decimal(target.Score) : DisplayFormat.Unknown;
            lTargetRank.Text = DisplayFormat.Count(target.Rank);
            lTargetType.Text = DisplayFormat.OrUnknown(target.Type);
            lTargetTitle.Text = target.Title;
            lTargetSynopsis.Text = target.Synopsis;
            _ = pbTargetImage.LoadImageAsync(target.Image_url);
            MALPageButton.Tag = target.Url;

            if (target is AnimeModel anime)
            {
                lTargetStatus.Text = DisplayFormat.AnimeStatus(anime.Airing);
                lTargetVolumesEpisodes.Text = DisplayFormat.Count(anime.Episodes);
            }
            else if (target is MangaModel manga)
            {
                lTargetStatus.Text = DisplayFormat.MangaStatus(manga.Publishing);
                lTargetVolumesEpisodes.Text = DisplayFormat.Count(manga.Volumes);
                lTargetChapters.Text = DisplayFormat.Count(manga.Chapters);
            }

            InfoTooltip.ToolTipTitle = $"{ target.Type } title";
            InfoTooltip.SetToolTip(lTargetTitle, target.Title);

            this.targetId = target.Mal_id;
            this.previewKind = int.Parse((kind == MediaKind.Anime ? rbAnime : rbManga).Tag.ToString());
            this.hasPreview = true;
        }

        /// <summary>
        /// Shows the fields that apply to the passed kind; only Manga have chapters.
        /// </summary>
        /// <param name="kind"> Whether the preview is of an Anime or a Manga. </param>
        private void ShowPreviewKind(MediaKind kind)
        {
            bool manga = kind == MediaKind.Manga;

            lChapters.Visible = manga;
            lTargetChapters.Visible = manga;
            lVolumesEpisodes.Text = manga ? "Volumes" : "Episodes";
        }

        /// <summary>
        /// Redirects to the target's MAL page.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void BMAL_Click(object sender, EventArgs e) => Shell.Open(((Button)sender).Tag);

        #endregion

        #region Form submition

        /// <summary>
        /// Opens the review's composition form.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void ReviewButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Writing reviews is coming in a future version.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// The tag of the selected Anime/Manga radio button.
        /// </summary>
        private int CurrentKind() => int.Parse((rbAnime.Checked ? rbAnime : rbManga).Tag.ToString());

        /// <summary>
        /// The text of the selected Anime/Manga radio button.
        /// </summary>
        private string CurrentKindName() => (rbAnime.Checked ? rbAnime : rbManga).Text;

        /// <summary>
        /// The icon of the selected Anime/Manga radio button.
        /// </summary>
        private System.Drawing.Image CurrentIcon() => this.mode == SearchMode.Link ? Icons.Link : rbAnime.Checked ? Icons.Anime : Icons.Manga;

        private string HintText(string target) => this.mode == SearchMode.Link
            ? $"Paste the MyAnimeList link of the Anime or Manga you want to review, (ex: { MalLink.Example }), and its information will be fetched from MAL."
            : $"Input the { target } title you want review on the text box above, meanwhile we will fetch any related data from MAL to help provide more information on the review target. A MyAnimeList link works too.";

        #endregion

        /// <summary>
        /// Handles the closing of the form.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event's arguments. </param>
        private void NewReviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stopping any search or preview that is still running.
            cts.Cancel();
        }

        /// <summary>
        /// Releases the resources that are not part of the form's controls.
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            searchCts?.Dispose();
            previewCts?.Dispose();
            cts.Dispose();

            if (!previewLoader.IsDisposed && previewLoader.Parent == null)
            {
                previewLoader.Dispose();
            }
        }
    }
}
