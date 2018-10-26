﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MAL_Reviwer_UI.user_controls;
using MAL_Reviewer_API;
using MAL_Reviewer_API.models;
using System.Threading;

namespace MAL_Reviwer_UI.forms
{
    public partial class fNewReview : Form
    {
        private bool _ready = true;
        public fNewReview()
        {
            InitializeComponent();

            pSearchCards.HorizontalScroll.Maximum = 0;
            pSearchCards.AutoScroll = true;

            rbAnime.CheckedChanged += RbAnime_CheckedChanged;
            rbScaleOther.CheckedChanged += RbScaleOther_CheckedChanged;
        }

        #region Manga/Anime labeling

        private void RbScaleOther_CheckedChanged(object sender, EventArgs e)
        {
            nupScaleOther.Enabled = rbScaleOther.Checked;
        }

        private void RbAnime_CheckedChanged(object sender, EventArgs e)
        {
            lTitle.Text = $"{ (rbAnime.Checked ? rbAnime.Text : rbManga.Text) } title";
            ttSearchCard.ToolTipTitle = lTitle.Text;
            pbShow.Image = (rbAnime.Checked ? Properties.Resources.icon_anime : Properties.Resources.icon_manga);
            tbSearch_TextChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Target Search

        private async void tbSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this._ready)
                    return;

                string
                    searchTitle = tbSearch.Text.Trim(),
                    searchType = rbAnime.Checked ? rbAnime.Text.ToLower() : rbManga.Text.ToLower();

                if (searchTitle.Length > 2)
                {
                    pbLoading.Visible = true;
                    pSearchCards.Visible = false;
                    this._ready = false;

                    SearchModel searchModel = await MALHelper.Search(searchType, searchTitle);
                    pSearchCards.Controls.Clear();

                    if (searchModel != null)
                    {
                        foreach (SearchResultsModel resultsModel in searchModel.results)
                        {
                            ucTargetSearchCard searchCard = new ucTargetSearchCard(resultsModel.mal_id, resultsModel.title, resultsModel.type, resultsModel.image_url);
                            int searchCardCount = pSearchCards.Controls.Count;

                            if (searchCardCount < 5)
                                pSearchCards.Height = searchCard.Height * searchCardCount;

                            searchCard.CardMouseClickEvent += SearchCard_CardMouseClickEvent;
                            searchCard.Top = searchCard.Height * searchCardCount;
                            ttSearchCard.SetToolTip(searchCard, resultsModel.title);
                            pSearchCards.Controls.Add(searchCard);
                        }

                        pSearchCards.Visible = true;
                    }

                    pbLoading.Visible = false;
                    this._ready = true;
                }
                else
                {
                    pSearchCards.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SearchCard_CardMouseClickEvent(object sender, int targetId)
        {
            if (rbAnime.Checked)
                PreviewAnime(targetId);
            else
                PreviewManga(targetId);
        }

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            if (tbSearch.Text.Trim().Length > 2 && pSearchCards.Controls.Count > 0 && !pSearchCards.Visible)
                pSearchCards.Visible = true;
        }

        #endregion

        #region Preview section update

        private async void PreviewAnime(int animeId)
        {
            try
            {
                AnimeModel animeModel = await MALHelper.GetAnime(animeId);

                lChapters.Visible = false;
                lTargetChapters.Visible = false;
                lVolumesEpisodes.Text = "Episodes";

                lTargetScore.Text = animeModel.score.ToString();
                lTargetRank.Text = animeModel.rank.ToString();
                lTargetType.Text = animeModel.type;
                lTargetStatus.Text = animeModel.airing ? "Airing" : "Finished";
                lTargetVolumesEpisodes.Text = animeModel.episodes != null ? animeModel.episodes.ToString() : "?";
                lTargetTitle.Text = animeModel.title.Length > 55 ? animeModel.title.Substring(0, 55) + "..." : animeModel.title;
                lTargetSynopsis.Text = animeModel.synopsis.Length > 215 ? animeModel.synopsis.Substring(0, 215) + "..." : animeModel.synopsis;
                pbTargetImage.Load(animeModel.image_url);
                bMAL.Tag = animeModel.url;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                pSearchCards.Visible = false;
            }
        }

        private async void PreviewManga(int mangaId)
        {
            try
            {
                MangaModel mangaModel = await MALHelper.GetManga(mangaId);

                lChapters.Visible = true;
                lTargetChapters.Visible = true;
                lVolumesEpisodes.Text = "Volumes";

                lTargetScore.Text = mangaModel.score.ToString();
                lTargetRank.Text = mangaModel.rank.ToString();
                lTargetType.Text = mangaModel.type;
                lTargetStatus.Text = mangaModel.publishing ? "Publishing" : "Finished";
                lTargetVolumesEpisodes.Text = mangaModel.volumes != null ? mangaModel.volumes.ToString() : "?";
                lTargetChapters.Text = mangaModel.chapters != null ? mangaModel.chapters.ToString() : "?";
                lTargetTitle.Text = mangaModel.title.Length > 55 ? mangaModel.title.Substring(0, 55) + "..." : mangaModel.title;
                lTargetSynopsis.Text = mangaModel.synopsis.Length > 215 ? mangaModel.synopsis.Substring(0, 215) + "..." : mangaModel.synopsis;
                pbTargetImage.Load(mangaModel.image_url);
                bMAL.Tag = mangaModel.url;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                pSearchCards.Visible = false;
            }
        }

        private void bMAL_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(((Button)sender).Tag.ToString());
        }

        #endregion
    }
}