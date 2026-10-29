using System.Collections.Generic;
using System.Linq;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.models.ReviewTemplateModels;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// The texts that describe review templates in the interface.
    /// </summary>
    public static class TemplateSummary
    {
        #region Fields

        /// <summary>
        /// The longest template name shown on a preview card.
        /// </summary>
        public const int CardNameLength = 20;

        #endregion

        #region Public methods

        /// <summary>
        /// The counter of templates, (ex: Review templates [2 / 10]).
        /// </summary>
        /// <param name="count"> The number of existing templates. </param>
        public static string Counter(int count)
        {
            return $"Review templates [{ count } / { ReviewTemplatesController.MaxReviewTemplates }]";
        }

        /// <summary>
        /// The template's name, shortened to fit a preview card.
        /// </summary>
        /// <param name="template"> The review template. </param>
        public static string CardName(ReviewTemplateModel template)
        {
            return DisplayFormat.Truncate(template?.TemplateName, CardNameLength);
        }

        /// <summary>
        /// The number of aspects, (ex: Aspects: 5).
        /// </summary>
        /// <param name="template"> The review template. </param>
        public static string Aspects(ReviewTemplateModel template)
        {
            return $"Aspects: { GetAspects(template).Count }";
        }

        /// <summary>
        /// Whether the template has an introduction, (ex: intro: true).
        /// </summary>
        /// <param name="template"> The review template. </param>
        public static string Intro(ReviewTemplateModel template)
        {
            return $"intro: { (template?.UseIntro == true ? "true" : "false") }";
        }

        /// <summary>
        /// Whether the template has a summary, (ex: tl;dr: false).
        /// </summary>
        /// <param name="template"> The review template. </param>
        public static string Tldr(ReviewTemplateModel template)
        {
            return $"tl;dr: { (template?.UserTLDR == true ? "true" : "false") }";
        }

        /// <summary>
        /// The aspect names, one per line; a single space when there are none,
        /// since an empty tooltip is not shown at all.
        /// </summary>
        /// <param name="template"> The review template. </param>
        public static string AspectsTooltip(ReviewTemplateModel template)
        {
            List<ReviewAspectModel> aspects = GetAspects(template);

            return aspects.Count == 0 ? " " : string.Concat(aspects.Select(aspect => $"{ aspect.AspectName }\n"));
        }

        #endregion

        #region Private methods

        private static List<ReviewAspectModel> GetAspects(ReviewTemplateModel template)
        {
            return template?.TemplateAspects?.Where(aspect => aspect != null).ToList() ?? new List<ReviewAspectModel>();
        }

        #endregion
    }
}
