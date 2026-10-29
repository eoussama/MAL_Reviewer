using System;
using MAL_Reviewer_Core.enumerations;

namespace MAL_Reviewer_Core.models.ReviewTemplateModels
{
    /// <summary>
    /// The review model.
    /// </summary>
    public class ReviewModel
    {
        #region Properties

        /// <summary>
        /// The review's title.
        /// </summary>
        public string TargetTitle { get; set; } = "";

        /// <summary>
        /// The MAL ID of the Anime or Manga reviewed.
        /// </summary>
        public int TargetId { get; set; } = -1;

        /// <summary>
        /// The type of the review's target (ex; Anime or Manga).
        /// </summary>
        public TargetType TargetType { get; set; } = TargetType.Anime;

        /// <summary>
        /// The review's rating.
        /// </summary>
        public double ReviewRating { get; set; } = 0;

        /// <summary>
        /// The review's intro.
        /// </summary>
        public string TemplateReviewIntro { get; set; } = "";

        /// <summary>
        /// The date of when the review was created.
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The date of when the review was last modified.
        /// </summary>
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The review's template.
        /// </summary>
        public ReviewTemplateModel ReviewTemplate { get; set; } = new ReviewTemplateModel();

        /// <summary>
        /// The review method of the review template.
        /// </summary>
        public ReviewMethodModel ReviewMethod { get; set; } = new ReviewMethodModel();

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ReviewModel() { }

        /// <summary>
        /// Constructor with all parameters.
        /// </summary>
        /// <param name="targetTitle"></param>
        /// <param name="targetId"></param>
        /// <param name="targetType"></param>
        /// <param name="reviewRating"></param>
        /// <param name="templateReviewIntro"></param>
        /// <param name="creationDate"></param>
        /// <param name="modificationDate"></param>
        /// <param name="reviewTemplate"></param>
        /// <param name="reviewMethod"></param>
        public ReviewModel(string targetTitle, int targetId, TargetType targetType, double reviewRating, string templateReviewIntro, DateTime creationDate, DateTime modificationDate, ReviewTemplateModel reviewTemplate, ReviewMethodModel reviewMethod)
        {
            TargetTitle = targetTitle;
            TargetId = targetId;
            TargetType = targetType;
            ReviewRating = reviewRating;
            TemplateReviewIntro = templateReviewIntro;
            CreationDate = creationDate;
            ModificationDate = modificationDate;
            ReviewTemplate = reviewTemplate;
            ReviewMethod = reviewMethod;
        }

        #endregion
    }
}
