using System.Collections.Generic;
using System.Windows.Forms;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_UI.extensions;

namespace MAL_Reviewer_UI.user_controls
{
    public partial class EntryListControl : UserControl
    {
        private int listHeight = 0;        

        /// <summary>
        /// Gets the list's height.
        /// </summary>
        public int ListHeight { get => listHeight; }

        public EntryListControl(string label, TargetType type)
        {
            InitializeComponent();
          
            // Affecting values for the lables.
            lType.Text = label;
            lCount.Text = "0";

            if (type == TargetType.Manga)
            {
                dgvList.Columns["cProgress"].HeaderText = "Chapters";
                dgvList.Columns["cVolumes"].Visible = true;
            }
        }

        /// <summary>
        /// Populates the list with anime data.
        /// </summary>
        /// <param name="animeList"></param>
        public void UpdateList(List<AnimelistEntryModel> animeList)
        {
            int _index = 0;

            dgvList.Rows.Clear();
            lCount.Text = animeList.Count.ToString();

            // Populating the DataDridView.
            animeList.ForEach(a => {
                dgvList.Rows.Add(_index + 1, a.Title, DisplayFormat.Progress(a.Watched_episodes, a.Total_episodes), "", a.Score, a.Type);
                dgvList.Rows[_index++].Tag = a.Url;
            });

            ResizeList();
        }

        /// <summary>
        /// Populates the list with manga data.
        /// </summary>
        /// <param name="animeList"></param>
        public void UpdateList(List<MangalistEntryModel> mangaList)
        {
            int _index = 0;

            dgvList.Rows.Clear();
            lCount.Text = mangaList.Count.ToString();

            // Populating the DataDridView.
            mangaList.ForEach(a => {
                dgvList.Rows.Add(_index + 1, a.Title, DisplayFormat.Progress(a.Read_chapters, a.Total_chapters), DisplayFormat.Progress(a.Read_volumes, a.Total_volumes), a.Score, a.Type);
                dgvList.Rows[_index++].Tag = a.Url;
            });

            ResizeList();
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void ClearList()
        {
            lCount.Text = "0";
            dgvList.Rows.Clear();
            ResizeList();
        }

        /// <summary>
        /// Resizes the control to fit the displayed data.
        /// </summary>
        private void ResizeList()
        {
            Height = 198;

            listHeight = LayoutMath.EntryListHeight(dgvList.Rows.Count, dgvList.Top, lType.Top, dgvList.ColumnHeadersHeight, dgvList.Rows.GetRowsHeight(DataGridViewElementStates.None));

            Height = ListHeight;
        }

        private void DgvList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dgvList.Rows[e.RowIndex].Selected = true;
                Shell.Open(dgvList.Rows[e.RowIndex].Tag);
            }
        }
    }
}
