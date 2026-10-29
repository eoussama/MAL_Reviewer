using System;
using System.Collections.Generic;
using System.Formats.Nrbf;
using System.IO;
using System.Linq;
using MAL_Reviewer_Core.models.ReviewTemplateModels;

namespace MAL_Reviewer_Core.migration
{
    /// <summary>
    /// The data recovered from the settings file of versions before 0.2.0.
    /// </summary>
    internal class LegacySettings
    {
        public string DefaultUser { get; set; } = string.Empty;

        public List<ReviewTemplateModel> ReviewTemplates { get; set; } = new List<ReviewTemplateModel>();
    }

    /// <summary>
    /// Reads the binary settings file (settings.dat) written with BinaryFormatter by versions before 0.2.0.
    /// </summary>
    /// <remarks>
    /// The file is decoded with <see cref="NrbfDecoder"/>, which only reads the payload's records and never
    /// instantiates the types it names, so it carries none of BinaryFormatter's security issues.
    /// Records are matched by type and member names; assembly versions are ignored.
    /// </remarks>
    internal static class LegacySettingsMigrator
    {
        #region Fields

        internal const string SettingsTypeName = "MAL_Reviewer_Core.Settings";

        #endregion

        #region Public methods

        /// <summary>
        /// Reads a legacy settings file.
        /// </summary>
        /// <param name="path"> The path of settings.dat. </param>
        /// <param name="settings"> The recovered settings, or null when the file could not be read. </param>
        /// <returns> Whether the file was read. </returns>
        public static bool TryRead(string path, out LegacySettings settings)
        {
            try
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    settings = Read(stream);
                }
            }
            catch (Exception)
            {
                // A damaged or unexpected file is not worth crashing over; the defaults are used instead.
                settings = null;
            }

            return settings != null;
        }

        /// <summary>
        /// Reads a legacy settings payload.
        /// </summary>
        /// <param name="stream"> The payload. </param>
        /// <returns> The recovered settings, or null when the payload is not a settings object. </returns>
        internal static LegacySettings Read(Stream stream)
        {
            if (!(NrbfDecoder.Decode(stream, leaveOpen: true) is ClassRecord root) || root.TypeName.FullName != SettingsTypeName)
            {
                return null;
            }

            ClassRecord user = GetClass(root, "UserSettings");
            ClassRecord templates = GetClass(root, "ReviewTemplatesSettings");

            return new LegacySettings
            {
                DefaultUser = GetString(user, "DefaultUser") ?? string.Empty,
                ReviewTemplates = ReadList(GetClass(templates, "ReviewTemplates")).Select(ReadTemplate).ToList()
            };
        }

        #endregion

        #region Private methods

        private static ReviewTemplateModel ReadTemplate(ClassRecord record)
        {
            ReviewTemplateModel template = new ReviewTemplateModel
            {
                TemplateName = GetString(record, "TemplateName") ?? ReviewTemplateModel.DefaultReviewTemplateName,
                UseIntro = Get(record, "UseIntro", record.GetBoolean, false),
                UserTLDR = Get(record, "UserTLDR", record.GetBoolean, true),
                TemplateAspects = ReadList(GetClass(record, "TemplateAspects")).Select(ReadAspect).ToList()
            };

            template.CreationDate = Get(record, "CreationDate", record.GetDateTime, template.CreationDate);
            template.ModificationDate = Get(record, "ModificationDate", record.GetDateTime, template.ModificationDate);

            return template;
        }

        private static ReviewAspectModel ReadAspect(ClassRecord record)
        {
            return new ReviewAspectModel(
                GetString(record, "AspectName") ?? string.Empty,
                GetString(record, "AspectReview") ?? string.Empty,
                Get(record, "AspectRating", record.GetDouble, 0d));
        }

        /// <summary>
        /// Reads the items of a serialized List&lt;T&gt;, which stores them in "_items" and their count in "_size".
        /// </summary>
        private static IEnumerable<ClassRecord> ReadList(ClassRecord list)
        {
            if (list == null || !list.HasMember("_items") || !list.HasMember("_size"))
            {
                return Enumerable.Empty<ClassRecord>();
            }

            if (!(list.GetArrayRecord("_items") is SZArrayRecord<SerializationRecord> items))
            {
                return Enumerable.Empty<ClassRecord>();
            }

            int size = list.GetInt32("_size");

            return items.GetArray().Take(size).OfType<ClassRecord>().ToList();
        }

        /// <summary>
        /// The name the compiler gives the field behind an auto-property.
        /// </summary>
        private static string BackingField(string property) => $"<{ property }>k__BackingField";

        private static ClassRecord GetClass(ClassRecord record, string property)
        {
            string member = BackingField(property);

            return record != null && record.HasMember(member) ? record.GetClassRecord(member) : null;
        }

        private static string GetString(ClassRecord record, string property)
        {
            string member = BackingField(property);

            return record != null && record.HasMember(member) ? record.GetString(member) : null;
        }

        private static T Get<T>(ClassRecord record, string property, Func<string, T> read, T fallback)
        {
            string member = BackingField(property);

            return record.HasMember(member) ? read(member) : fallback;
        }

        #endregion
    }
}
