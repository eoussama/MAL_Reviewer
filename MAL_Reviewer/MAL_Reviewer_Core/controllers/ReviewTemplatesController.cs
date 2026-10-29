using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using MAL_Reviewer_Core.exceptions;
using MAL_Reviewer_Core.interfaces;
using MAL_Reviewer_Core.models.ReviewTemplateModels;

namespace MAL_Reviewer_Core.controllers
{
    /// <summary>
    /// The collection of review templates
    /// /// </summary>
    public class ReviewTemplatesController : ISettings
    {
        #region Fields

        /// <summary>
        /// The maximum review templates allowed to be created.
        /// </summary>
        public static readonly short MaxReviewTemplates = 10;

        #endregion

        #region Properties

        /// <summary>
        /// Contains all the template reviews loaded in the memory.
        /// </summary>
        public List<ReviewTemplateModel> ReviewTemplates { get; set; } = new List<ReviewTemplateModel>();

        /// <summary>
        /// Gets the number of review templates loaded into the memory.
        /// </summary>
        [JsonIgnore]
        public short Count
        {
            get => (short)this.ReviewTemplates.Count;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ReviewTemplatesController() { }

        #endregion

        #region Private methods

        /// <summary>
        /// Whether or not there is still place for more review templates taking in
        /// consideration the maximum allowed number.
        /// </summary>
        /// <returns></returns>
        private bool IsAllowedToAddReviewTemplate() => ReviewTemplates.Count < MaxReviewTemplates;

        #endregion

        #region Public methods

        /// <summary>
        /// Gets a review template by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ReviewTemplateModel Get(short index)
        {
            try
            {
                return ReviewTemplates[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidReviewTemplateException(index);
            }
        }

        /// <summary>
        /// Adds a new review template to the collection.
        /// </summary>
        /// <param name="reviewTemplateModel"></param>
        public void Add(ReviewTemplateModel reviewTemplateModel)
        {
            if (IsAllowedToAddReviewTemplate())
            {
                ReviewTemplates.Add(reviewTemplateModel);
            }
            else
            {
                throw new MaximumReviewTemplatesException();
            }
        }

        /// <summary>
        /// Deletes a review template.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string Delete(short index)
        {
            // Creating a variable for the name of the review template that's about to be deleted.
            string deletedReviewTemplateName = string.Empty;

            try
            {
                // Getting the name of the review template that's about to be deleted.
                deletedReviewTemplateName = ReviewTemplates[index].TemplateName;

                // Removing the review template from the collection.
                ReviewTemplates.RemoveAt(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidReviewTemplateException(index);
            }

            // Returning the name of the deleted review template.
            return deletedReviewTemplateName;
        }

        /// <summary>
        /// Deletes all review templates.
        /// </summary>
        public void Clear()
        {
            ReviewTemplates.Clear();
        }

        /// <summary>
        /// Updates a review template.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="reviewTemplateModel"></param>
        public void Update(short index, ReviewTemplateModel reviewTemplateModel)
        {
            try
            {
                // Replacing the review template of the passed index with an updated one.
                ReviewTemplates[index] = reviewTemplateModel;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidReviewTemplateException(index);
            }
        }

        /// <summary>
        /// Appends the proper number in a string to avoid duplicating the review template names.
        /// </summary>
        /// <returns></returns>
        public string GetDuplicateName()
        {
            // Constructing the first free review template name, (ex: defaultReviewTemplateName, defaultReviewTemplateName 1, defaultReviewTemplateName 2...).
            string reviewTemplateName = ReviewTemplateModel.DefaultReviewTemplateName;

            for (int number = 1; ReviewTemplates.Any(reviewTemp => reviewTemp.TemplateName == reviewTemplateName); number++)
            {
                reviewTemplateName = $"{ ReviewTemplateModel.DefaultReviewTemplateName } { number }";
            }

            return reviewTemplateName;
        }

        /// <summary>
        /// Generate default template settings when the application first runs or
        /// when a template setting is requested.
        /// </summary>
        public void SeedSettings()
        {
            // Adding the default “Classic MAL review”.
            Add(new ReviewTemplateModel("Classic MAL review", true, false, DateTime.Now, DateTime.Now, new List<ReviewAspectModel>() {
                        new ReviewAspectModel("Story", "", 0),
                        new ReviewAspectModel("Art", "", 0),
                        new ReviewAspectModel("Sound", "", 0),
                        new ReviewAspectModel("Character", "", 0),
                        new ReviewAspectModel("Enjoyment", "", 0)
                    }));

            // Adding the default “Lazy MAL review”.
            Add(new ReviewTemplateModel("Lazy MAL review", false, true, DateTime.Now, DateTime.Now, new List<ReviewAspectModel>()));
        }

        /// <summary>
        /// Resets the review template's settings.
        /// </summary>
        public void ResetSettings()
        {
            Clear();
            SeedSettings();
        }

        #endregion
    }
}
