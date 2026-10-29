using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;
using MAL_Reviewer_Core.models.ReviewTemplateModels;
using Xunit;

namespace MAL_Reviewer_Core.Tests
{
    public class UsernameRuleTests
    {
        [Theory]
        [InlineData("ab", true)]
        [InlineData("Xinil", true)]
        [InlineData("user_name-01", true)]
        [InlineData("sixteencharacter", true)]
        [InlineData("seventeencharacte", false)]
        [InlineData("a", false)]
        [InlineData("", false)]
        [InlineData("with space", false)]
        [InlineData("dot.name", false)]
        [InlineData(null, false)]
        public void IsValid(string username, bool expected)
        {
            Assert.Equal(expected, UsernameRule.IsValid(username));
        }

        [Theory]
        [InlineData("  ab ", "ab")]
        [InlineData(null, "")]
        public void Normalize_Trims(string username, string expected)
        {
            Assert.Equal(expected, UsernameRule.Normalize(username));
        }
    }

    public class MalUrlsTests
    {
        [Fact]
        public void BuildsEncodedLinks()
        {
            Assert.Equal("https://myanimelist.net/profile/Xinil", MalUrls.Profile(" Xinil "));
            Assert.Equal("https://myanimelist.net/profile/Xinil/friends", MalUrls.Friends("Xinil"));
            Assert.Equal("https://myanimelist.net/animelist/a%20b", MalUrls.AnimeList("a b"));
            Assert.Equal("https://myanimelist.net/mangalist/x%2Fy", MalUrls.MangaList("x/y"));
            Assert.Equal("https://myanimelist.net/profile/", MalUrls.Profile(null));
            Assert.Equal("https://myanimelist.net/register.php", MalUrls.Register);
        }
    }

    public class ListStatusTests
    {
        [Fact]
        public void Split_GroupsByStatus_InDisplayOrder()
        {
            short?[] statuses = { 1, 2, 6, 2, 4, 3, 5, null, 9 };

            List<short?>[] buckets = ListStatus.Split(statuses, status => status);

            Assert.Equal(5, buckets.Length);
            Assert.Single(buckets[0]);
            Assert.Equal(2, buckets[1].Count);
            Assert.Single(buckets[2]);
            Assert.Single(buckets[3]);
            Assert.Single(buckets[4]);
            Assert.Equal((short)6, buckets[4][0]);
        }

        [Fact]
        public void Split_TreatsNullAsEmpty()
        {
            Assert.All(ListStatus.Split<short?>(null, status => status), Assert.Empty);
        }

        [Fact]
        public void Labels_MatchTheTargetType()
        {
            Assert.Equal("Watching", ListStatus.Labels(TargetType.Anime)[0]);
            Assert.Equal("Plan to Watch", ListStatus.Labels(TargetType.Anime)[4]);
            Assert.Equal("Reading", ListStatus.Labels(TargetType.Manga)[0]);
            Assert.Equal("Plan to Read", ListStatus.Labels(TargetType.Manga)[4]);
            Assert.Equal(ListStatus.Codes.Count, ListStatus.Labels(TargetType.Manga).Count);
        }
    }

    public class DisplayFormatTests
    {
        public DisplayFormatTests()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [Theory]
        [InlineData("Cowboy Bebop", 17, "Cowboy Bebop")]
        [InlineData("Neon Genesis Evangelion", 17, "Neon Genesis Evan...")]
        [InlineData("exactly", 7, "exactly")]
        [InlineData(null, 5, "")]
        [InlineData("", 5, "")]
        public void Truncate(string text, int length, string expected)
        {
            Assert.Equal(expected, DisplayFormat.Truncate(text, length));
        }

        [Fact]
        public void Truncate_RejectsNegativeLengths()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DisplayFormat.Truncate("x", -1));
        }

        [Theory]
        [InlineData(5, 12, "5/12")]
        [InlineData(623, null, "623/?")]
        [InlineData(3, 0, "3/?")]
        [InlineData(null, 24, "0/24")]
        public void Progress(int? done, int? total, string expected)
        {
            Assert.Equal(expected, DisplayFormat.Progress(done, total));
        }

        [Fact]
        public void Values_ShowUnknownsAsQuestionMarks_ButKeepZeros()
        {
            Assert.Equal("0", DisplayFormat.Count(0));
            Assert.Equal("?", DisplayFormat.Count(null));
            Assert.Equal("7.40", DisplayFormat.Decimal(7.4));
            Assert.Equal("0.00", DisplayFormat.Decimal(0));
            Assert.Equal("?", DisplayFormat.Decimal(null));
            Assert.Equal("?", DisplayFormat.OrUnknown(" "));
            Assert.Equal("Male", DisplayFormat.OrUnknown("Male"));
            Assert.Equal("?", DisplayFormat.LongDate(null));
            Assert.Equal("Sunday, 05 January 2020", DisplayFormat.LongDate(new DateTime(2020, 1, 5)));
        }

        [Fact]
        public void TextsForTheInterface()
        {
            Assert.Equal("Such empty!", DisplayFormat.About(null));
            Assert.Equal("Such empty!", DisplayFormat.About("  "));
            Assert.Equal("Hi", DisplayFormat.About("Hi"));
            Assert.Equal("Airing", DisplayFormat.AnimeStatus(true));
            Assert.Equal("Finished", DisplayFormat.AnimeStatus(false));
            Assert.Equal("Publishing", DisplayFormat.MangaStatus(true));
            Assert.Equal("Finished", DisplayFormat.MangaStatus(false));
            Assert.Equal("Xinil's Anime list is public.", DisplayFormat.ListStateTooltip("Xinil", "Anime Stats", "Public"));
            Assert.Equal("x's  list is .", DisplayFormat.ListStateTooltip("x", null, null));
        }
    }

    public class TemplateSummaryTests
    {
        [Fact]
        public void DescribesATemplate()
        {
            ReviewTemplateModel template = new ReviewTemplateModel("A very long template name here", true, false, DateTime.Now, DateTime.Now,
                new List<ReviewAspectModel> { new ReviewAspectModel("Story"), null, new ReviewAspectModel("Art") });

            Assert.Equal("A very long template...", TemplateSummary.CardName(template));
            Assert.Equal("Aspects: 2", TemplateSummary.Aspects(template));
            Assert.Equal("intro: true", TemplateSummary.Intro(template));
            Assert.Equal("tl;dr: false", TemplateSummary.Tldr(template));
            Assert.Equal("Story\nArt\n", TemplateSummary.AspectsTooltip(template));
            Assert.Equal("Review templates [3 / 10]", TemplateSummary.Counter(3));
        }

        [Fact]
        public void ToleratesMissingAspects()
        {
            ReviewTemplateModel template = new ReviewTemplateModel { TemplateAspects = null };

            Assert.Equal("Aspects: 0", TemplateSummary.Aspects(template));
            Assert.Equal(" ", TemplateSummary.AspectsTooltip(template));
            Assert.Equal("intro: false", TemplateSummary.Intro(null));
            Assert.Equal("", TemplateSummary.CardName(null));
        }
    }

    public class TemplateEditorTests
    {
        private static readonly DateTime Created = new DateTime(2019, 1, 1);
        private static readonly DateTime Now = new DateTime(2026, 10, 29);

        [Fact]
        public void BuildUpdated_KeepsTheCreationDate_AndKnownAspectReviews()
        {
            ReviewTemplateModel original = new ReviewTemplateModel("Old", false, true, Created, Created, new List<ReviewAspectModel>
            {
                new ReviewAspectModel("Story", "Kept", 8),
                new ReviewAspectModel("Sound", "Dropped", 5)
            });

            ReviewTemplateModel updated = TemplateEditor.BuildUpdated(original, "  New  ", new[] { "Story", " ", "Art " }, true, false, Now);

            Assert.Equal("New", updated.TemplateName);
            Assert.Equal(Created, updated.CreationDate);
            Assert.Equal(Now, updated.ModificationDate);
            Assert.True(updated.UseIntro);
            Assert.False(updated.UserTLDR);
            Assert.Equal(new[] { "Story", "Art" }, updated.TemplateAspects.Select(aspect => aspect.AspectName));
            Assert.Equal("Kept", updated.TemplateAspects[0].AspectReview);
            Assert.Equal(8, updated.TemplateAspects[0].AspectRating);
            Assert.Equal("", updated.TemplateAspects[1].AspectReview);
            Assert.NotSame(original.TemplateAspects[0], updated.TemplateAspects[0]);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void BuildUpdated_RequiresAName(string name)
        {
            ArgumentException error = Assert.Throws<ArgumentException>(() => TemplateEditor.BuildUpdated(new ReviewTemplateModel(), name, null, false, false, Now));

            Assert.Equal("name", error.ParamName);
        }

        [Fact]
        public void BuildUpdated_ToleratesMissingParts()
        {
            ReviewTemplateModel updated = TemplateEditor.BuildUpdated(new ReviewTemplateModel { TemplateAspects = null }, "T", null, false, false, Now);

            Assert.Empty(updated.TemplateAspects);
            Assert.Throws<ArgumentNullException>(() => TemplateEditor.BuildUpdated(null, "T", null, false, false, Now));
        }
    }

    public class SearchGateTests
    {
        [Fact]
        public void RequiresTheMinimumLength()
        {
            SearchGate gate = new SearchGate(3);

            Assert.False(gate.ShouldSubmit("ab", 1));
            Assert.False(gate.ShouldSubmit("  ab  ", 1));
            Assert.False(gate.ShouldSubmit(null, 1));
            Assert.True(gate.ShouldSubmit("abc", 1));
        }

        [Fact]
        public void SkipsTheSameSuccessfulSearch_ButNotAnotherKind()
        {
            SearchGate gate = new SearchGate();
            gate.MarkSucceeded(" naruto ", 1);

            Assert.False(gate.ShouldSubmit("naruto", 1));
            Assert.True(gate.ShouldSubmit("naruto", 2));
            Assert.True(gate.ShouldSubmit("bleach", 1));
        }

        [Fact]
        public void GoingBackDuringASearch_SearchesAgain()
        {
            // Issue #13: Anime "naruto" done, Manga "naruto" running, back to Anime: the Manga results must not stay.
            SearchGate gate = new SearchGate();
            gate.MarkSucceeded("naruto", 1);

            Assert.True(gate.ShouldSubmit("naruto", 2));
            gate.MarkSent("naruto", 2);

            Assert.True(gate.ShouldSubmit("naruto", 1));
            Assert.False(gate.ShouldSubmit(" naruto ", 2));
        }

        [Fact]
        public void RunningSearches_AreRememberedOnlyWhenTheySucceed()
        {
            SearchGate gate = new SearchGate();

            gate.MarkSent("naruto", 1);
            gate.Complete(false);
            Assert.True(gate.ShouldSubmit("naruto", 1));

            gate.MarkSent("naruto", 1);
            gate.Complete(true);
            Assert.False(gate.ShouldSubmit("naruto", 1));

            // Completing without a running search changes nothing.
            gate.Complete(true);
            Assert.False(gate.ShouldSubmit("naruto", 1));

            gate.MarkSent("bleach", 1);
            gate.Reset();
            Assert.True(gate.ShouldSubmit("bleach", 1));
            Assert.True(gate.ShouldSubmit("naruto", 1));
        }

        [Fact]
        public void SwitchingKindsBackAndForth_SearchesEachTime()
        {
            // The dashboard bug: the kind must be the selected one, not the last previewed one.
            SearchGate gate = new SearchGate();

            gate.MarkSucceeded("naruto", 1);
            Assert.True(gate.ShouldSubmit("naruto", 2));
            gate.MarkSucceeded("naruto", 2);
            Assert.True(gate.ShouldSubmit("naruto", 1));
        }

        [Fact]
        public void FailedSearches_CanBeRetried()
        {
            SearchGate gate = new SearchGate();

            // Nothing is marked when the search fails.
            Assert.True(gate.ShouldSubmit("naruto", 1));
            Assert.True(gate.ShouldSubmit("naruto", 1));

            gate.MarkSucceeded("naruto", 1);
            gate.Reset();
            Assert.True(gate.ShouldSubmit("naruto", 1));
        }
    }

    public class LayoutMathTests
    {
        [Fact]
        public void EntryListHeight()
        {
            Assert.Equal(30, LayoutMath.EntryListHeight(0, 20, 10, 25, 0));
            Assert.Equal(20 + 50 + 220 + 23, LayoutMath.EntryListHeight(10, 20, 10, 25, 220));
            Assert.Equal(20 + 50 + 1500 + 23, LayoutMath.EntryListHeight(50, 20, 10, 25, 9999));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 120)]
        [InlineData(4, 240)]
        [InlineData(10, 240)]
        [InlineData(-1, 0)]
        public void SearchPanelHeight(int results, int expected)
        {
            Assert.Equal(expected, LayoutMath.SearchPanelHeight(results, 60));
        }
    }

    public class AppInfoTests
    {
        [Fact]
        public void Version_IsTheReleaseVersion()
        {
            Assert.Equal("0.2.0", AppInfo.Version);
        }

        [Fact]
        public void ContactLink_EncodesTheSubject()
        {
            Assert.Equal("mailto:oussama_gtacj@live.fr?subject=MAL%20Reviewer%20message", AppInfo.ContactLink);
        }

        [Fact]
        public void Description_NamesTheApplication()
        {
            Assert.StartsWith("MAL Reviewer is an unofficial", AppInfo.Description);
            Assert.DoesNotContain("{0}", AppInfo.Description);
        }

        [Fact]
        public void ReadVersion_DropsBuildMetadata()
        {
            Assert.DoesNotContain("+", AppInfo.ReadVersion(typeof(AppInfoTests).Assembly));
        }
    }
}
