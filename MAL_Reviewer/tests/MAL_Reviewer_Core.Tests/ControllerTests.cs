using System;
using System.Collections.Generic;
using System.Linq;
using MAL_Reviewer_Core.controllers;
using MAL_Reviewer_Core.exceptions;
using MAL_Reviewer_Core.models.ReviewTemplateModels;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class ReviewTemplatesControllerTests
    {
        private static ReviewTemplateModel Template(string name) => new ReviewTemplateModel { TemplateName = name };

        [Fact]
        public void Seed_AddsTheTwoDefaultTemplates()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();

            controller.SeedSettings();

            Assert.Equal(2, controller.Count);
            Assert.Equal(new[] { "Story", "Art", "Sound", "Character", "Enjoyment" }, controller.Get(0).TemplateAspects.Select(aspect => aspect.AspectName));
            Assert.Empty(controller.Get(1).TemplateAspects);
        }

        [Fact]
        public void Add_AllowsExactlyTheMaximum()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();

            for (int i = 0; i < ReviewTemplatesController.MaxReviewTemplates; i++)
            {
                controller.Add(Template($"T{ i }"));
            }

            MaximumReviewTemplatesException error = Assert.Throws<MaximumReviewTemplatesException>(() => controller.Add(Template("one too many")));

            Assert.Equal(ReviewTemplatesController.MaxReviewTemplates, controller.Count);
            Assert.Contains("(10)", error.Message);
            Assert.Equal(error.Message, error.ToString());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void InvalidIndexes_Throw(short index)
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();
            controller.SeedSettings();

            Assert.Throws<InvalidReviewTemplateException>(() => controller.Get(index));
            Assert.Throws<InvalidReviewTemplateException>(() => controller.Update(index, Template("x")));
            InvalidReviewTemplateException error = Assert.Throws<InvalidReviewTemplateException>(() => controller.Delete(index));

            Assert.Contains(index.ToString(), error.Message);
            Assert.Equal(error.Message, error.ToString());
            Assert.Equal(2, controller.Count);
        }

        [Fact]
        public void Update_ReplacesTheTemplate()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();
            controller.Add(Template("Old"));

            controller.Update(0, Template("New"));

            Assert.Equal("New", controller.Get(0).TemplateName);
        }

        [Fact]
        public void Delete_ReturnsTheDeletedName()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();
            controller.Add(Template("A"));
            controller.Add(Template("B"));

            Assert.Equal("A", controller.Delete(0));
            Assert.Equal("B", controller.Get(0).TemplateName);
        }

        [Fact]
        public void GetDuplicateName_NumbersNewTemplates()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();

            Assert.Equal("New review template", controller.GetDuplicateName());

            controller.Add(Template(controller.GetDuplicateName()));
            Assert.Equal("New review template 1", controller.GetDuplicateName());

            controller.Add(Template(controller.GetDuplicateName()));
            Assert.Equal("New review template 2", controller.GetDuplicateName());
        }

        [Fact]
        public void GetDuplicateName_NeverRepeatsAName_AfterDeletions()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();
            controller.Add(Template("New review template"));
            controller.Add(Template("New review template 1"));
            controller.Add(Template("New review template 2"));
            controller.Delete(0);

            // Two templates left: counting them would give "… 2", which already exists.
            string name = controller.GetDuplicateName();

            Assert.Equal("New review template", name);
            Assert.DoesNotContain(controller.ReviewTemplates, template => template.TemplateName == name);
        }

        [Fact]
        public void GetDuplicateName_IgnoresSimilarButDifferentNames()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();
            controller.Add(Template("New review template for Bebop"));

            Assert.Equal("New review template", controller.GetDuplicateName());
        }

        [Fact]
        public void ClearAndReset()
        {
            ReviewTemplatesController controller = new ReviewTemplatesController();
            controller.Add(Template("A"));

            controller.Clear();
            Assert.Equal(0, controller.Count);

            controller.Add(Template("B"));
            controller.ResetSettings();
            Assert.Equal(new[] { "Classic MAL review", "Lazy MAL review" }, controller.ReviewTemplates.Select(template => template.TemplateName));
        }
    }

    public class UserControllerTests
    {
        [Fact]
        public void SetDefaultUser_StoresTheTrimmedName()
        {
            UserController controller = new UserController();

            controller.SetDefaultUser("  Xinil ");

            Assert.Equal("Xinil", controller.DefaultUser);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("has space")]
        [InlineData("waytoolongusername")]
        [InlineData(null)]
        public void SetDefaultUser_RejectsInvalidNames(string username)
        {
            UserController controller = new UserController();

            ArgumentException error = Assert.Throws<ArgumentException>(() => controller.SetDefaultUser(username));

            Assert.Equal(MAL_Reviewer_Core.helpers.UsernameRule.InvalidMessage, error.Message);
            Assert.Equal(string.Empty, controller.DefaultUser);
        }

        [Fact]
        public void SetDefaultUser_RejectsTheCurrentDefault_InAnyCase()
        {
            UserController controller = new UserController { DefaultUser = "Xinil" };

            ArgumentException error = Assert.Throws<ArgumentException>(() => controller.SetDefaultUser("xinil"));

            Assert.Equal("The default user is already “Xinil”.", error.Message);
        }

        [Fact]
        public void ClearSeedAndReset_EmptyTheDefaultUser()
        {
            UserController controller = new UserController { DefaultUser = "a1" };
            controller.ClearDefaultUser();
            Assert.Equal(string.Empty, controller.DefaultUser);

            controller.DefaultUser = "b2";
            controller.SeedSettings();
            Assert.Equal(string.Empty, controller.DefaultUser);

            controller.DefaultUser = "c3";
            controller.ResetSettings();
            Assert.Equal(string.Empty, controller.DefaultUser);
        }
    }

    public class ModelTests
    {
        [Fact]
        public void Models_HaveUsableDefaults()
        {
            ReviewAspectModel aspect = new ReviewAspectModel();
            ReviewMethodModel method = new ReviewMethodModel();
            ReviewModel review = new ReviewModel();
            ReviewTemplateModel template = new ReviewTemplateModel();

            Assert.Equal(string.Empty, aspect.AspectName);
            Assert.Equal(10, method.ReviewScaling);
            Assert.False(method.UseDecimal);
            Assert.Equal(-1, review.TargetId);
            Assert.NotNull(review.ReviewTemplate);
            Assert.NotNull(review.ReviewMethod);
            Assert.Equal(ReviewTemplateModel.DefaultReviewTemplateName, template.TemplateName);
            Assert.Empty(template.TemplateAspects);
        }

        [Fact]
        public void Models_KeepTheirConstructorValues()
        {
            DateTime date = new DateTime(2020, 5, 5);
            ReviewTemplateModel template = new ReviewTemplateModel("T", true, false, date, date, new List<ReviewAspectModel> { new ReviewAspectModel("Art") });
            ReviewMethodModel method = new ReviewMethodModel(5, true);
            ReviewModel review = new ReviewModel("Bebop", 1, enumerations.TargetType.Anime, 9, "Intro", date, date, template, method);

            Assert.Equal("Art", template.TemplateAspects[0].AspectName);
            Assert.Equal(5, review.ReviewMethod.ReviewScaling);
            Assert.True(review.ReviewMethod.UseDecimal);
            Assert.Equal("Bebop", review.TargetTitle);
            Assert.Equal(9, review.ReviewRating);
            Assert.Equal("Intro", review.TemplateReviewIntro);
            Assert.Same(template, review.ReviewTemplate);
        }
    }
}
