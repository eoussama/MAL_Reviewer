﻿namespace MAL_Reviewer_Core.models
{
    /// <summary>
    /// Review method model.
    /// </summary>
    public class ReviewMethodModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reviewScale"></param>
        /// <param name="decimalUse"></param>
        public ReviewMethodModel(byte reviewScale, bool decimalUse)
        {
            this.ReviewScale = reviewScale;
            this.DecimalUse = decimalUse;
        }

        /// <summary>
        /// The scale of the review's rating (5, 10, 50...).
        /// </summary>
        public byte ReviewScale { get; set; }

        /// <summary>
        /// Whether or not to use decimal numbers in the rating (ex; 8.63, 6.13...).
        /// </summary>
        public bool DecimalUse { get; set; }
    }
}
