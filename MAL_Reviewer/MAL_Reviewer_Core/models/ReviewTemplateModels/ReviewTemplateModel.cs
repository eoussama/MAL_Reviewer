using System;
using System.Collections.Generic;

namespace MAL_Reviewer_Core.models.ReviewTemplateModels
{
    /// <summary>
    /// Review template model.
    /// </summary>
    public class ReviewTemplateModel
    {
        #region Fields

        /// <summary>
        /// The default name of all review templates.
        /// </summary>
        public static readonly string DefaultReviewTemplateName = "New review template";

        #endregion

        #region Properties

        /// <summary>
        /// The name of the review template.
        /// </summary>
        public string TemplateName { get; set; } = DefaultReviewTemplateName;

        /// <summary>
        /// Whether or not to use an intro for the review.
        /// </summary>
        public bool UseIntro { get; set; } = false;

        /// <summary>
        /// Whether or not to add a tl;dr section.
        /// </summary>
        public bool UserTLDR { get; set; } = true;

        /// <summary>
        /// The date of when the review template was created.
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The date of when the review template was last modified.
        /// </summary>
        public DateTime ModificationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The list of the aspects of the review template (ex; Art, Story, Sound...).
        /// </summary>
        public List<ReviewAspectModel> TemplateAspects { get; set; } = new List<ReviewAspectModel>();

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ReviewTemplateModel() { }

        /// <summary>
        /// Constructor with all parameters.
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="useIntro"></param>
        /// <param name="userTLDR"></param>
        /// <param name="creationDate"></param>
        /// <param name="lastModified"></param>
        /// <param name="templateAspects"></param>
        public ReviewTemplateModel(string templateName, bool useIntro, bool userTLDR, DateTime creationDate, DateTime lastModified, List<ReviewAspectModel> templateAspects)
        {
            TemplateName = templateName;
            UseIntro = useIntro;
            UserTLDR = userTLDR;
            CreationDate = creationDate;
            ModificationDate = lastModified;
            TemplateAspects = templateAspects;
        }

        #endregion
    }
}
