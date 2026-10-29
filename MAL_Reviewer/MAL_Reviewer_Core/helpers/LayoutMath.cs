using System;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// The size calculations of the interface's dynamic panels.
    /// </summary>
    public static class LayoutMath
    {
        #region Fields

        /// <summary>
        /// Lists with this many rows or more stop growing and scroll instead.
        /// </summary>
        public const int ListScrollThreshold = 50;

        /// <summary>
        /// The height of the rows area of a list that scrolls.
        /// </summary>
        public const int ListFixedRowsHeight = 1500;

        /// <summary>
        /// The space kept under a list.
        /// </summary>
        public const int ListBottomSpace = 23;

        /// <summary>
        /// The number of search results visible without scrolling.
        /// </summary>
        public const int VisibleSearchResults = 4;

        #endregion

        #region Public methods

        /// <summary>
        /// The height of an entry list control.
        /// </summary>
        /// <param name="rowCount"> The number of rows in the list. </param>
        /// <param name="gridTop"> The top of the grid inside the control. </param>
        /// <param name="labelTop"> The top of the title label inside the control. </param>
        /// <param name="headerHeight"> The height of the grid's header. </param>
        /// <param name="rowsHeight"> The total height of the rows. </param>
        public static int EntryListHeight(int rowCount, int gridTop, int labelTop, int headerHeight, int rowsHeight)
        {
            if (rowCount <= 0)
            {
                return gridTop + labelTop;
            }

            int contentHeight = rowCount < ListScrollThreshold ? rowsHeight : ListFixedRowsHeight;

            return gridTop + headerHeight * 2 + contentHeight + ListBottomSpace;
        }

        /// <summary>
        /// The height of the search results panel: one card per result, up to <see cref="VisibleSearchResults"/>.
        /// </summary>
        /// <param name="resultCount"> The number of results. </param>
        /// <param name="cardHeight"> The height of one result card. </param>
        public static int SearchPanelHeight(int resultCount, int cardHeight)
        {
            return cardHeight * Math.Clamp(resultCount, 0, VisibleSearchResults);
        }

        #endregion
    }
}
