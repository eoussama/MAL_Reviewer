using System;

namespace MAL_Reviewer_Core.models.ReviewTemplateModels
{
    /// <summary>
    /// The review aspect model.
    /// </summary>
    public class ReviewAspectModel
    {
        #region Properties

        /// <summary>
        /// The review aspect's name (ex; Story, Art...).
        /// </summary>
        public string AspectName { get; set; } = "";

        /// <summary>
        /// The review aspect's content.
        /// </summary>
        public string AspectReview { get; set; } = "";

        /// <summary>
        /// The review aspect's rating.
        /// </summary>
        public double AspectRating { get; set; } = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ReviewAspectModel() { }

        /// <summary>
        /// Constructor with label parameter.
        /// </summary>
        /// <param name="aspectName"></param>
        public ReviewAspectModel(string aspectName)
        {
            this.AspectName = aspectName;
        }

        /// <summary>
        /// Constructor with all parameters.
        /// </summary>
        /// <param name="aspectName"></param>
        /// <param name="aspectReview"></param>
        /// <param name="aspectRating"></param>
        public ReviewAspectModel(string aspectName, string aspectReview, double aspectRating)
        {
            this.AspectName = aspectName;
            this.AspectReview = aspectReview;
            this.AspectRating = aspectRating;
        }         

        #endregion
    }
}
