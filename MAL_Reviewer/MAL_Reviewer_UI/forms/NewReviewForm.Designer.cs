namespace MAL_Reviewer_UI.forms
{
    partial class NewReviewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewReviewForm));
            this.pSectionSeparator = new System.Windows.Forms.Panel();
            this.pPreview = new System.Windows.Forms.Panel();
            this.lTargetSynopsis = new System.Windows.Forms.RichTextBox();
            this.lChapters = new System.Windows.Forms.Label();
            this.lTargetTitle = new System.Windows.Forms.Label();
            this.lTargetChapters = new System.Windows.Forms.Label();
            this.pbTargetImage = new System.Windows.Forms.PictureBox();
            this.lScore = new System.Windows.Forms.Label();
            this.MALPageButton = new System.Windows.Forms.Button();
            this.lVolumesEpisodes = new System.Windows.Forms.Label();
            this.lRank = new System.Windows.Forms.Label();
            this.lTargetType = new System.Windows.Forms.Label();
            this.lTargetVolumesEpisodes = new System.Windows.Forms.Label();
            this.lType = new System.Windows.Forms.Label();
            this.lTargetRank = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.lTargetScore = new System.Windows.Forms.Label();
            this.lTargetStatus = new System.Windows.Forms.Label();
            this.pSetup = new System.Windows.Forms.Panel();
            this.searchControl = new MAL_Reviewer_UI.user_controls.TextboxControl();
            this.pSearchCards = new System.Windows.Forms.Panel();
            this.gbRating = new System.Windows.Forms.GroupBox();
            this.rbDecimalNo = new System.Windows.Forms.RadioButton();
            this.rbDecimalYes = new System.Windows.Forms.RadioButton();
            this.lRatingDecimal = new System.Windows.Forms.Label();
            this.gbRatingScale = new System.Windows.Forms.GroupBox();
            this.nupScaleOther = new System.Windows.Forms.NumericUpDown();
            this.rbScaleOther = new System.Windows.Forms.RadioButton();
            this.rbScale100 = new System.Windows.Forms.RadioButton();
            this.rbScale10 = new System.Windows.Forms.RadioButton();
            this.rbScale5 = new System.Windows.Forms.RadioButton();
            this.rbAnime = new System.Windows.Forms.RadioButton();
            this.rbManga = new System.Windows.Forms.RadioButton();
            this.lTitle = new System.Windows.Forms.Label();
            this.TargetNotFoundLabel = new System.Windows.Forms.Label();
            this.ModeLinkLabel = new System.Windows.Forms.LinkLabel();
            this.lPreview = new System.Windows.Forms.Label();
            this.ReviewButton = new System.Windows.Forms.Button();
            this.InfoTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.reviewTemplatesGroupBox = new System.Windows.Forms.GroupBox();
            this.reviewTemplatesFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTargetImage)).BeginInit();
            this.pSetup.SuspendLayout();
            this.gbRating.SuspendLayout();
            this.gbRatingScale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupScaleOther)).BeginInit();
            this.reviewTemplatesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pSectionSeparator
            // 
            this.pSectionSeparator.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pSectionSeparator.Location = new System.Drawing.Point(310, 28);
            this.pSectionSeparator.Name = "pSectionSeparator";
            this.pSectionSeparator.Size = new System.Drawing.Size(6, 310);
            this.pSectionSeparator.TabIndex = 0;
            // 
            // pPreview
            // 
            this.pPreview.Controls.Add(this.lTargetSynopsis);
            this.pPreview.Controls.Add(this.lChapters);
            this.pPreview.Controls.Add(this.lTargetTitle);
            this.pPreview.Controls.Add(this.lTargetChapters);
            this.pPreview.Controls.Add(this.pbTargetImage);
            this.pPreview.Controls.Add(this.lScore);
            this.pPreview.Controls.Add(this.MALPageButton);
            this.pPreview.Controls.Add(this.lVolumesEpisodes);
            this.pPreview.Controls.Add(this.lRank);
            this.pPreview.Controls.Add(this.lTargetType);
            this.pPreview.Controls.Add(this.lTargetVolumesEpisodes);
            this.pPreview.Controls.Add(this.lType);
            this.pPreview.Controls.Add(this.lTargetRank);
            this.pPreview.Controls.Add(this.lStatus);
            this.pPreview.Controls.Add(this.lTargetScore);
            this.pPreview.Controls.Add(this.lTargetStatus);
            this.pPreview.Location = new System.Drawing.Point(343, 12);
            this.pPreview.Name = "pPreview";
            this.pPreview.Size = new System.Drawing.Size(255, 339);
            this.pPreview.TabIndex = 4;
            this.pPreview.Visible = false;
            // 
            // lTargetSynopsis
            // 
            this.lTargetSynopsis.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lTargetSynopsis.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetSynopsis.Location = new System.Drawing.Point(10, 249);
            this.lTargetSynopsis.Name = "lTargetSynopsis";
            this.lTargetSynopsis.ReadOnly = true;
            this.lTargetSynopsis.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.lTargetSynopsis.Size = new System.Drawing.Size(235, 77);
            this.lTargetSynopsis.TabIndex = 21;
            this.lTargetSynopsis.Text = "Such empty";
            // 
            // lChapters
            // 
            this.lChapters.AutoSize = true;
            this.lChapters.Location = new System.Drawing.Point(144, 156);
            this.lChapters.Name = "lChapters";
            this.lChapters.Size = new System.Drawing.Size(49, 13);
            this.lChapters.TabIndex = 21;
            this.lChapters.Text = "Chapters";
            this.lChapters.Visible = false;
            // 
            // lTargetTitle
            // 
            this.lTargetTitle.AutoEllipsis = true;
            this.lTargetTitle.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetTitle.Location = new System.Drawing.Point(10, 19);
            this.lTargetTitle.Name = "lTargetTitle";
            this.lTargetTitle.Size = new System.Drawing.Size(236, 38);
            this.lTargetTitle.TabIndex = 5;
            this.lTargetTitle.Text = "Title";
            this.InfoTooltip.SetToolTip(this.lTargetTitle, "Title");
            // 
            // lTargetChapters
            // 
            this.lTargetChapters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lTargetChapters.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetChapters.Location = new System.Drawing.Point(189, 156);
            this.lTargetChapters.Name = "lTargetChapters";
            this.lTargetChapters.Size = new System.Drawing.Size(58, 13);
            this.lTargetChapters.TabIndex = 20;
            this.lTargetChapters.Text = "23";
            this.lTargetChapters.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lTargetChapters.Visible = false;
            // 
            // pbTargetImage
            // 
            this.pbTargetImage.Location = new System.Drawing.Point(13, 66);
            this.pbTargetImage.Name = "pbTargetImage";
            this.pbTargetImage.Size = new System.Drawing.Size(124, 163);
            this.pbTargetImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbTargetImage.TabIndex = 0;
            this.pbTargetImage.TabStop = false;
            // 
            // lScore
            // 
            this.lScore.AutoSize = true;
            this.lScore.Location = new System.Drawing.Point(144, 66);
            this.lScore.Name = "lScore";
            this.lScore.Size = new System.Drawing.Size(35, 13);
            this.lScore.TabIndex = 10;
            this.lScore.Text = "Score";
            // 
            // MALPageButton
            // 
            this.MALPageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MALPageButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MALPageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MALPageButton.Location = new System.Drawing.Point(146, 202);
            this.MALPageButton.Name = "MALPageButton";
            this.MALPageButton.Size = new System.Drawing.Size(99, 27);
            this.MALPageButton.TabIndex = 13;
            this.MALPageButton.Text = "Visit MAL page";
            this.MALPageButton.UseVisualStyleBackColor = true;
            this.MALPageButton.Click += new System.EventHandler(this.BMAL_Click);
            // 
            // lVolumesEpisodes
            // 
            this.lVolumesEpisodes.AutoSize = true;
            this.lVolumesEpisodes.Location = new System.Drawing.Point(144, 138);
            this.lVolumesEpisodes.Name = "lVolumesEpisodes";
            this.lVolumesEpisodes.Size = new System.Drawing.Size(50, 13);
            this.lVolumesEpisodes.TabIndex = 19;
            this.lVolumesEpisodes.Text = "Episodes";
            // 
            // lRank
            // 
            this.lRank.AutoSize = true;
            this.lRank.Location = new System.Drawing.Point(144, 84);
            this.lRank.Name = "lRank";
            this.lRank.Size = new System.Drawing.Size(33, 13);
            this.lRank.TabIndex = 11;
            this.lRank.Text = "Rank";
            // 
            // lTargetType
            // 
            this.lTargetType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lTargetType.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetType.Location = new System.Drawing.Point(189, 102);
            this.lTargetType.Name = "lTargetType";
            this.lTargetType.Size = new System.Drawing.Size(58, 13);
            this.lTargetType.TabIndex = 9;
            this.lTargetType.Text = "movie";
            this.lTargetType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lTargetVolumesEpisodes
            // 
            this.lTargetVolumesEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lTargetVolumesEpisodes.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetVolumesEpisodes.Location = new System.Drawing.Point(189, 138);
            this.lTargetVolumesEpisodes.Name = "lTargetVolumesEpisodes";
            this.lTargetVolumesEpisodes.Size = new System.Drawing.Size(58, 13);
            this.lTargetVolumesEpisodes.TabIndex = 18;
            this.lTargetVolumesEpisodes.Text = "23";
            this.lTargetVolumesEpisodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lType
            // 
            this.lType.AutoSize = true;
            this.lType.Location = new System.Drawing.Point(144, 102);
            this.lType.Name = "lType";
            this.lType.Size = new System.Drawing.Size(31, 13);
            this.lType.TabIndex = 12;
            this.lType.Text = "Type";
            // 
            // lTargetRank
            // 
            this.lTargetRank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lTargetRank.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetRank.Location = new System.Drawing.Point(191, 82);
            this.lTargetRank.Name = "lTargetRank";
            this.lTargetRank.Size = new System.Drawing.Size(56, 16);
            this.lTargetRank.TabIndex = 8;
            this.lTargetRank.Text = "10000";
            this.lTargetRank.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(144, 120);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(37, 13);
            this.lStatus.TabIndex = 17;
            this.lStatus.Text = "Status";
            // 
            // lTargetScore
            // 
            this.lTargetScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lTargetScore.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetScore.Location = new System.Drawing.Point(196, 64);
            this.lTargetScore.Name = "lTargetScore";
            this.lTargetScore.Size = new System.Drawing.Size(51, 17);
            this.lTargetScore.TabIndex = 7;
            this.lTargetScore.Text = "7.63";
            this.lTargetScore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lTargetStatus
            // 
            this.lTargetStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lTargetStatus.Font = new System.Drawing.Font("Bahnschrift", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTargetStatus.Location = new System.Drawing.Point(175, 120);
            this.lTargetStatus.Name = "lTargetStatus";
            this.lTargetStatus.Size = new System.Drawing.Size(72, 13);
            this.lTargetStatus.TabIndex = 16;
            this.lTargetStatus.Text = "airing";
            this.lTargetStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pSetup
            // 
            this.pSetup.Controls.Add(this.searchControl);
            this.pSetup.Controls.Add(this.pSearchCards);
            this.pSetup.Controls.Add(this.gbRating);
            this.pSetup.Controls.Add(this.rbAnime);
            this.pSetup.Controls.Add(this.rbManga);
            this.pSetup.Controls.Add(this.lTitle);
            this.pSetup.Controls.Add(this.TargetNotFoundLabel);
            this.pSetup.Controls.Add(this.ModeLinkLabel);
            this.pSetup.Location = new System.Drawing.Point(27, 12);
            this.pSetup.Name = "pSetup";
            this.pSetup.Size = new System.Drawing.Size(255, 339);
            this.pSetup.TabIndex = 5;
            // 
            // searchControl
            // 
            this.searchControl.AllowLoad = true;
            this.searchControl.AutoSubmit = true;
            this.searchControl.BackColor = System.Drawing.SystemColors.ControlLight;
            this.searchControl.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.searchControl.Icon = global::MAL_Reviewer_UI.Properties.Resources.icon_anime;
            this.searchControl.InnerText = "";
            this.searchControl.Location = new System.Drawing.Point(12, 40);
            this.searchControl.Name = "searchControl";
            this.searchControl.Placeholder = "Target\'s title...";
            this.searchControl.PlaceholderColor = System.Drawing.SystemColors.ControlDark;
            this.searchControl.Size = new System.Drawing.Size(230, 35);
            this.searchControl.SubmitMin = 3;
            this.searchControl.TabIndex = 0;
            this.searchControl.Tag = "0";
            this.searchControl.ToggleIcon = true;
            // 
            // pSearchCards
            // 
            this.pSearchCards.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pSearchCards.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSearchCards.Location = new System.Drawing.Point(12, 75);
            this.pSearchCards.Name = "pSearchCards";
            this.pSearchCards.Size = new System.Drawing.Size(230, 10);
            this.pSearchCards.TabIndex = 0;
            this.pSearchCards.Visible = false;
            // 
            // gbRating
            // 
            this.gbRating.Controls.Add(this.rbDecimalNo);
            this.gbRating.Controls.Add(this.rbDecimalYes);
            this.gbRating.Controls.Add(this.lRatingDecimal);
            this.gbRating.Controls.Add(this.gbRatingScale);
            this.gbRating.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbRating.Location = new System.Drawing.Point(12, 167);
            this.gbRating.Name = "gbRating";
            this.gbRating.Size = new System.Drawing.Size(231, 159);
            this.gbRating.TabIndex = 0;
            this.gbRating.TabStop = false;
            this.gbRating.Text = "Rating settings";
            // 
            // rbDecimalNo
            // 
            this.rbDecimalNo.AutoSize = true;
            this.rbDecimalNo.Checked = true;
            this.rbDecimalNo.Font = new System.Drawing.Font("Bahnschrift Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDecimalNo.Location = new System.Drawing.Point(179, 129);
            this.rbDecimalNo.Name = "rbDecimalNo";
            this.rbDecimalNo.Size = new System.Drawing.Size(42, 20);
            this.rbDecimalNo.TabIndex = 7;
            this.rbDecimalNo.TabStop = true;
            this.rbDecimalNo.Text = "No";
            this.rbDecimalNo.UseVisualStyleBackColor = true;
            // 
            // rbDecimalYes
            // 
            this.rbDecimalYes.AutoSize = true;
            this.rbDecimalYes.Font = new System.Drawing.Font("Bahnschrift Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDecimalYes.Location = new System.Drawing.Point(127, 129);
            this.rbDecimalYes.Name = "rbDecimalYes";
            this.rbDecimalYes.Size = new System.Drawing.Size(46, 20);
            this.rbDecimalYes.TabIndex = 6;
            this.rbDecimalYes.Text = "Yes";
            this.rbDecimalYes.UseVisualStyleBackColor = true;
            // 
            // lRatingDecimal
            // 
            this.lRatingDecimal.AutoSize = true;
            this.lRatingDecimal.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lRatingDecimal.Location = new System.Drawing.Point(6, 128);
            this.lRatingDecimal.Name = "lRatingDecimal";
            this.lRatingDecimal.Size = new System.Drawing.Size(115, 19);
            this.lRatingDecimal.TabIndex = 5;
            this.lRatingDecimal.Text = "Decimal rating";
            // 
            // gbRatingScale
            // 
            this.gbRatingScale.Controls.Add(this.nupScaleOther);
            this.gbRatingScale.Controls.Add(this.rbScaleOther);
            this.gbRatingScale.Controls.Add(this.rbScale100);
            this.gbRatingScale.Controls.Add(this.rbScale10);
            this.gbRatingScale.Controls.Add(this.rbScale5);
            this.gbRatingScale.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbRatingScale.Location = new System.Drawing.Point(9, 26);
            this.gbRatingScale.Name = "gbRatingScale";
            this.gbRatingScale.Size = new System.Drawing.Size(212, 93);
            this.gbRatingScale.TabIndex = 1;
            this.gbRatingScale.TabStop = false;
            this.gbRatingScale.Text = "Rating scale";
            // 
            // nupScaleOther
            // 
            this.nupScaleOther.BackColor = System.Drawing.SystemColors.ControlLight;
            this.nupScaleOther.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nupScaleOther.Enabled = false;
            this.nupScaleOther.Font = new System.Drawing.Font("Bahnschrift Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nupScaleOther.Location = new System.Drawing.Point(83, 57);
            this.nupScaleOther.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nupScaleOther.Name = "nupScaleOther";
            this.nupScaleOther.Size = new System.Drawing.Size(44, 23);
            this.nupScaleOther.TabIndex = 0;
            this.nupScaleOther.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // rbScaleOther
            // 
            this.rbScaleOther.AutoSize = true;
            this.rbScaleOther.Font = new System.Drawing.Font("Bahnschrift Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbScaleOther.Location = new System.Drawing.Point(15, 58);
            this.rbScaleOther.Name = "rbScaleOther";
            this.rbScaleOther.Size = new System.Drawing.Size(58, 20);
            this.rbScaleOther.TabIndex = 8;
            this.rbScaleOther.Text = "Other";
            this.rbScaleOther.UseVisualStyleBackColor = true;
            // 
            // rbScale100
            // 
            this.rbScale100.AutoSize = true;
            this.rbScale100.Font = new System.Drawing.Font("Bahnschrift Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbScale100.Location = new System.Drawing.Point(155, 26);
            this.rbScale100.Name = "rbScale100";
            this.rbScale100.Size = new System.Drawing.Size(44, 20);
            this.rbScale100.TabIndex = 7;
            this.rbScale100.Text = "100";
            this.rbScale100.UseVisualStyleBackColor = true;
            // 
            // rbScale10
            // 
            this.rbScale10.AutoSize = true;
            this.rbScale10.Checked = true;
            this.rbScale10.Font = new System.Drawing.Font("Bahnschrift Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbScale10.Location = new System.Drawing.Point(83, 26);
            this.rbScale10.Name = "rbScale10";
            this.rbScale10.Size = new System.Drawing.Size(37, 20);
            this.rbScale10.TabIndex = 6;
            this.rbScale10.TabStop = true;
            this.rbScale10.Text = "10";
            this.rbScale10.UseVisualStyleBackColor = true;
            // 
            // rbScale5
            // 
            this.rbScale5.AutoSize = true;
            this.rbScale5.Font = new System.Drawing.Font("Bahnschrift Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbScale5.Location = new System.Drawing.Point(15, 26);
            this.rbScale5.Name = "rbScale5";
            this.rbScale5.Size = new System.Drawing.Size(33, 20);
            this.rbScale5.TabIndex = 5;
            this.rbScale5.Text = "5";
            this.rbScale5.UseVisualStyleBackColor = true;
            // 
            // rbAnime
            // 
            this.rbAnime.AutoSize = true;
            this.rbAnime.Checked = true;
            this.rbAnime.Location = new System.Drawing.Point(124, 19);
            this.rbAnime.Name = "rbAnime";
            this.rbAnime.Size = new System.Drawing.Size(54, 17);
            this.rbAnime.TabIndex = 4;
            this.rbAnime.TabStop = true;
            this.rbAnime.Tag = "1";
            this.rbAnime.Text = "Anime";
            this.rbAnime.UseVisualStyleBackColor = true;
            // 
            // rbManga
            // 
            this.rbManga.AutoSize = true;
            this.rbManga.Location = new System.Drawing.Point(184, 19);
            this.rbManga.Name = "rbManga";
            this.rbManga.Size = new System.Drawing.Size(58, 17);
            this.rbManga.TabIndex = 0;
            this.rbManga.Tag = "2";
            this.rbManga.Text = "Manga";
            this.rbManga.UseVisualStyleBackColor = true;
            // 
            // lTitle
            // 
            this.lTitle.AutoSize = true;
            this.lTitle.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTitle.Location = new System.Drawing.Point(8, 16);
            this.lTitle.Name = "lTitle";
            this.lTitle.Size = new System.Drawing.Size(86, 19);
            this.lTitle.TabIndex = 0;
            this.lTitle.Text = "Anime title";
            // 
            // ModeLinkLabel
            // 
            this.ModeLinkLabel.ActiveLinkColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ModeLinkLabel.AutoSize = true;
            this.ModeLinkLabel.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModeLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.ModeLinkLabel.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.ModeLinkLabel.Location = new System.Drawing.Point(10, 79);
            this.ModeLinkLabel.Name = "ModeLinkLabel";
            this.ModeLinkLabel.Size = new System.Drawing.Size(113, 13);
            this.ModeLinkLabel.TabIndex = 8;
            this.ModeLinkLabel.TabStop = true;
            this.ModeLinkLabel.Text = "Search by link instead";
            this.ModeLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ModeLinkLabel_LinkClicked);
            // 
            // TargetNotFoundLabel
            // 
            this.TargetNotFoundLabel.Font = new System.Drawing.Font("Bahnschrift Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TargetNotFoundLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.TargetNotFoundLabel.Location = new System.Drawing.Point(10, 98);
            this.TargetNotFoundLabel.Name = "TargetNotFoundLabel";
            this.TargetNotFoundLabel.Size = new System.Drawing.Size(227, 52);
            this.TargetNotFoundLabel.TabIndex = 7;
            this.TargetNotFoundLabel.Text = "Input the [target] title you want review on the text box above, meanwhile we\'ll f" +
    "etch any related information from MAL to help provide more data on the review ta" +
    "rget.";
            // 
            // lPreview
            // 
            this.lPreview.Font = new System.Drawing.Font("Bahnschrift", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lPreview.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lPreview.Location = new System.Drawing.Point(343, 12);
            this.lPreview.Name = "lPreview";
            this.lPreview.Size = new System.Drawing.Size(255, 339);
            this.lPreview.TabIndex = 6;
            this.lPreview.Text = "Anime preview";
            this.lPreview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ReviewButton
            // 
            this.ReviewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReviewButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ReviewButton.Enabled = false;
            this.ReviewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReviewButton.Location = new System.Drawing.Point(499, 588);
            this.ReviewButton.Name = "ReviewButton";
            this.ReviewButton.Size = new System.Drawing.Size(99, 27);
            this.ReviewButton.TabIndex = 15;
            this.ReviewButton.Text = "Review";
            this.ReviewButton.UseVisualStyleBackColor = true;
            this.ReviewButton.Click += new System.EventHandler(this.ReviewButton_Click);
            // 
            // InfoTooltip
            // 
            this.InfoTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.InfoTooltip.ToolTipTitle = "Target title";
            // 
            // reviewTemplatesGroupBox
            // 
            this.reviewTemplatesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reviewTemplatesGroupBox.Controls.Add(this.reviewTemplatesFlowPanel);
            this.reviewTemplatesGroupBox.Font = new System.Drawing.Font("Bahnschrift SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reviewTemplatesGroupBox.Location = new System.Drawing.Point(27, 377);
            this.reviewTemplatesGroupBox.Name = "reviewTemplatesGroupBox";
            this.reviewTemplatesGroupBox.Size = new System.Drawing.Size(571, 196);
            this.reviewTemplatesGroupBox.TabIndex = 16;
            this.reviewTemplatesGroupBox.TabStop = false;
            this.reviewTemplatesGroupBox.Text = "Review templates [x]";
            // 
            // reviewTemplatesFlowPanel
            // 
            this.reviewTemplatesFlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reviewTemplatesFlowPanel.AutoScroll = true;
            this.reviewTemplatesFlowPanel.Location = new System.Drawing.Point(6, 26);
            this.reviewTemplatesFlowPanel.Name = "reviewTemplatesFlowPanel";
            this.reviewTemplatesFlowPanel.Size = new System.Drawing.Size(559, 163);
            this.reviewTemplatesFlowPanel.TabIndex = 22;
            this.reviewTemplatesFlowPanel.WrapContents = false;
            // 
            // NewReviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 632);
            this.Controls.Add(this.reviewTemplatesGroupBox);
            this.Controls.Add(this.ReviewButton);
            this.Controls.Add(this.pSetup);
            this.Controls.Add(this.pPreview);
            this.Controls.Add(this.pSectionSeparator);
            this.Controls.Add(this.lPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NewReviewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Review";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewReviewForm_FormClosing);
            this.pPreview.ResumeLayout(false);
            this.pPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTargetImage)).EndInit();
            this.pSetup.ResumeLayout(false);
            this.pSetup.PerformLayout();
            this.gbRating.ResumeLayout(false);
            this.gbRating.PerformLayout();
            this.gbRatingScale.ResumeLayout(false);
            this.gbRatingScale.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupScaleOther)).EndInit();
            this.reviewTemplatesGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pSectionSeparator;
        private System.Windows.Forms.Panel pPreview;
        private System.Windows.Forms.Panel pSetup;
        private System.Windows.Forms.Label lTitle;
        private System.Windows.Forms.RadioButton rbAnime;
        private System.Windows.Forms.RadioButton rbManga;
        private System.Windows.Forms.GroupBox gbRating;
        private System.Windows.Forms.GroupBox gbRatingScale;
        private System.Windows.Forms.NumericUpDown nupScaleOther;
        private System.Windows.Forms.RadioButton rbScaleOther;
        private System.Windows.Forms.RadioButton rbScale100;
        private System.Windows.Forms.RadioButton rbScale10;
        private System.Windows.Forms.RadioButton rbScale5;
        private System.Windows.Forms.RadioButton rbDecimalNo;
        private System.Windows.Forms.RadioButton rbDecimalYes;
        private System.Windows.Forms.Label lRatingDecimal;
        private System.Windows.Forms.Panel pSearchCards;
        private System.Windows.Forms.Label lTargetTitle;
        private System.Windows.Forms.PictureBox pbTargetImage;
        private System.Windows.Forms.Label lType;
        private System.Windows.Forms.Label lRank;
        private System.Windows.Forms.Label lScore;
        private System.Windows.Forms.Label lTargetType;
        private System.Windows.Forms.Label lTargetRank;
        private System.Windows.Forms.Label lTargetScore;
        private System.Windows.Forms.Button MALPageButton;
        private System.Windows.Forms.Label lChapters;
        private System.Windows.Forms.Label lTargetChapters;
        private System.Windows.Forms.Label lVolumesEpisodes;
        private System.Windows.Forms.Label lTargetVolumesEpisodes;
        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.Label lTargetStatus;
        private System.Windows.Forms.Label lPreview;
        private System.Windows.Forms.Label TargetNotFoundLabel;
        private System.Windows.Forms.LinkLabel ModeLinkLabel;
        private System.Windows.Forms.Button ReviewButton;
        private System.Windows.Forms.ToolTip InfoTooltip;
        private System.Windows.Forms.RichTextBox lTargetSynopsis;
        private user_controls.TextboxControl searchControl;
        private System.Windows.Forms.GroupBox reviewTemplatesGroupBox;
        private System.Windows.Forms.FlowLayoutPanel reviewTemplatesFlowPanel;
    }
}