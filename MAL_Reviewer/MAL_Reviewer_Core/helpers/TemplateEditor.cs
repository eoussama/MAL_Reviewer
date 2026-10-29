using System;
using System.Collections.Generic;
using System.Linq;
using MAL_Reviewer_Core.models.ReviewTemplateModels;

namespace MAL_Reviewer_Core.helpers
{
    /// <summary>
    /// Builds review templates from what the user typed.
    /// </summary>
    public static class TemplateEditor
    {
        #region Fields

        /// <summary>
        /// The message shown when a template has no name.
        /// </summary>
        public const string MissingNameMessage = "Input a valid name for the review template.";

        #endregion

        #region Public methods

        /// <summary>
        /// Returns an updated copy of a review template. The creation date is kept, and so are
        /// the reviews and ratings of aspects that keep their name.
        /// </summary>
        /// <param name="original"> The template being edited. </param>
        /// <param name="name"> The new name; surrounding spaces are ignored. </param>
        /// <param name="aspectNames"> The aspect names, in order. </param>
        /// <param name="useIntro"> Whether the template has an introduction. </param>
        /// <param name="useTldr"> Whether the template has a summary. </param>
        /// <param name="now"> The modification date. </param>
        /// <exception cref="ArgumentException"> When the name is empty. </exception>
        public static ReviewTemplateModel BuildUpdated(ReviewTemplateModel original, string name, IEnumerable<string> aspectNames, bool useIntro, bool useTldr, DateTime now)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            string templateName = name?.Trim();

            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException(MissingNameMessage, nameof(name));
            }

            List<ReviewAspectModel> previous = original.TemplateAspects ?? new List<ReviewAspectModel>();

            List<ReviewAspectModel> aspects = (aspectNames ?? Enumerable.Empty<string>())
                .Select(aspectName => aspectName?.Trim())
                .Where(aspectName => !string.IsNullOrEmpty(aspectName))
                .Select(aspectName =>
                {
                    ReviewAspectModel kept = previous.FirstOrDefault(aspect => aspect?.AspectName == aspectName);

                    return kept == null
                        ? new ReviewAspectModel(aspectName)
                        : new ReviewAspectModel(aspectName, kept.AspectReview, kept.AspectRating);
                })
                .ToList();

            return new ReviewTemplateModel(templateName, useIntro, useTldr, original.CreationDate, now, aspects);
        }

        #endregion
    }
}
