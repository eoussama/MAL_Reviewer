using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MAL_Reviewer_API.models.ListEntryModels;
using MAL_Reviewer_Core.enumerations;
using MAL_Reviewer_Core.helpers;

namespace MAL_Reviewer_Core.services
{
    /// <summary>
    /// The formats Anime and Manga lists are exported in.
    /// </summary>
    public enum ListFileFormat
    {
        /// <summary>
        /// The XML of MyAnimeList's own export, which MyAnimeList, AniList and Kitsu can import.
        /// </summary>
        MalXml,

        /// <summary>
        /// Comma-separated values, for spreadsheets.
        /// </summary>
        Csv
    }

    /// <summary>
    /// Writes Anime and Manga lists to files.
    /// </summary>
    public static class ListExport
    {
        #region Fields

        /// <summary>
        /// The status names of MyAnimeList's export, in the order of <see cref="ListStatus.Codes"/>.
        /// </summary>
        private static readonly string[] MalAnimeStatuses = { "Watching", "Completed", "On-Hold", "Dropped", "Plan to Watch" };
        private static readonly string[] MalMangaStatuses = { "Reading", "Completed", "On-Hold", "Dropped", "Plan to Read" };

        private const string NoDate = "0000-00-00";

        #endregion

        #region Public methods

        /// <summary>
        /// The name suggested for an exported list, (ex: Xinil_animelist.xml).
        /// </summary>
        /// <param name="username"> The owner of the list. </param>
        /// <param name="type"> Whether it is the Anime or the Manga list. </param>
        /// <param name="format"> The format of the file. </param>
        public static string FileName(string username, TargetType type, ListFileFormat format)
        {
            return $"{ username }_{ (type == TargetType.Anime ? "animelist" : "mangalist") }.{ (format == ListFileFormat.Csv ? "csv" : "xml") }";
        }

        /// <summary>
        /// The format of a file to write: the one its extension names, or else the chosen one.
        /// </summary>
        /// <param name="path"> The file. </param>
        /// <param name="chosen"> The format chosen for it. </param>
        public static ListFileFormat FormatOf(string path, ListFileFormat chosen)
        {
            switch (Path.GetExtension(path ?? string.Empty).ToLowerInvariant())
            {
                case ".csv":
                    return ListFileFormat.Csv;
                case ".xml":
                    return ListFileFormat.MalXml;
                default:
                    return chosen;
            }
        }

        /// <summary>
        /// Writes an Anime list.
        /// </summary>
        /// <param name="username"> The owner of the list. </param>
        /// <param name="entries"> The entries of the list. </param>
        /// <param name="format"> The format of the file. </param>
        /// <returns> The content of the file. </returns>
        public static byte[] AnimeList(string username, IEnumerable<AnimelistEntryModel> entries, ListFileFormat format)
        {
            List<AnimelistEntryModel> list = (entries ?? Enumerable.Empty<AnimelistEntryModel>()).Where(entry => entry != null).ToList();

            if (format == ListFileFormat.Csv)
            {
                IReadOnlyList<string> labels = ListStatus.Labels(TargetType.Anime);

                return Csv(new[] { "MAL id", "Title", "Type", "Status", "Score", "Episodes watched", "Episodes", "Tags", "Link" },
                    list.Select(entry => new object[] { entry.Mal_id, entry.Title, entry.Type, Label(labels, entry.Watching_status), Score(entry.Score), entry.Watched_episodes, entry.Total_episodes, entry.Tags, entry.Url }));
            }

            return MalXml(1, username, "anime", MalAnimeStatuses, list.Select(entry => entry.Watching_status).ToList(), list.Select(entry => (Action<XmlWriter, string>)((writer, status) =>
            {
                writer.WriteElementString("series_animedb_id", Number(entry.Mal_id));
                writer.WriteElementString("series_title", entry.Title ?? string.Empty);
                writer.WriteElementString("series_type", entry.Type ?? string.Empty);
                writer.WriteElementString("series_episodes", Number(entry.Total_episodes));
                writer.WriteElementString("my_id", "0");
                writer.WriteElementString("my_watched_episodes", Number(entry.Watched_episodes));
                writer.WriteElementString("my_start_date", NoDate);
                writer.WriteElementString("my_finish_date", NoDate);
                writer.WriteElementString("my_score", Number(entry.Score));
                writer.WriteElementString("my_status", status);
                writer.WriteElementString("my_times_watched", "0");
                writer.WriteElementString("my_tags", entry.Tags ?? string.Empty);
                writer.WriteElementString("update_on_import", "1");
            })).ToList());
        }

        /// <summary>
        /// Writes a Manga list.
        /// </summary>
        /// <param name="username"> The owner of the list. </param>
        /// <param name="entries"> The entries of the list. </param>
        /// <param name="format"> The format of the file. </param>
        /// <returns> The content of the file. </returns>
        public static byte[] MangaList(string username, IEnumerable<MangalistEntryModel> entries, ListFileFormat format)
        {
            List<MangalistEntryModel> list = (entries ?? Enumerable.Empty<MangalistEntryModel>()).Where(entry => entry != null).ToList();

            if (format == ListFileFormat.Csv)
            {
                IReadOnlyList<string> labels = ListStatus.Labels(TargetType.Manga);

                return Csv(new[] { "MAL id", "Title", "Type", "Status", "Score", "Chapters read", "Chapters", "Volumes read", "Volumes", "Tags", "Link" },
                    list.Select(entry => new object[] { entry.Mal_id, entry.Title, entry.Type, Label(labels, entry.Reading_status), Score(entry.Score), entry.Read_chapters, entry.Total_chapters, entry.Read_volumes, entry.Total_volumes, entry.Tags, entry.Url }));
            }

            return MalXml(2, username, "manga", MalMangaStatuses, list.Select(entry => entry.Reading_status).ToList(), list.Select(entry => (Action<XmlWriter, string>)((writer, status) =>
            {
                writer.WriteElementString("manga_mangadb_id", Number(entry.Mal_id));
                writer.WriteElementString("manga_title", entry.Title ?? string.Empty);
                writer.WriteElementString("manga_volumes", Number(entry.Total_volumes));
                writer.WriteElementString("manga_chapters", Number(entry.Total_chapters));
                writer.WriteElementString("my_id", "0");
                writer.WriteElementString("my_read_volumes", Number(entry.Read_volumes));
                writer.WriteElementString("my_read_chapters", Number(entry.Read_chapters));
                writer.WriteElementString("my_start_date", NoDate);
                writer.WriteElementString("my_finish_date", NoDate);
                writer.WriteElementString("my_score", Number(entry.Score));
                writer.WriteElementString("my_status", status);
                writer.WriteElementString("my_times_read", "0");
                writer.WriteElementString("my_tags", entry.Tags ?? string.Empty);
                writer.WriteElementString("update_on_import", "1");
            })).ToList());
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Writes the XML of MyAnimeList's export. Entries without a known status are left out, since MyAnimeList can't import them.
        /// </summary>
        /// <param name="exportType"> 1 for Anime lists, 2 for Manga lists. </param>
        /// <param name="username"> The owner of the list. </param>
        /// <param name="element"> The element of each entry. </param>
        /// <param name="statusNames"> The status names, in the order of <see cref="ListStatus.Codes"/>. </param>
        /// <param name="statuses"> The status code of each entry. </param>
        /// <param name="writeEntries"> Writes the content of each entry, given its status name. </param>
        private static byte[] MalXml(int exportType, string username, string element, string[] statusNames, List<short?> statuses, List<Action<XmlWriter, string>> writeEntries)
        {
            int[] indexes = statuses.Select(status => status.HasValue ? ListStatus.Codes.ToList().IndexOf(status.Value) : -1).ToArray();
            int[] totals = Enumerable.Range(0, statusNames.Length).Select(i => indexes.Count(index => index == i)).ToArray();
            string[] totalNames = exportType == 1
                ? new[] { "user_total_watching", "user_total_completed", "user_total_onhold", "user_total_dropped", "user_total_plantowatch" }
                : new[] { "user_total_reading", "user_total_completed", "user_total_onhold", "user_total_dropped", "user_total_plantoread" };

            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = true }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("myanimelist");

                    writer.WriteStartElement("myinfo");
                    writer.WriteElementString("user_name", username ?? string.Empty);
                    writer.WriteElementString("user_export_type", exportType.ToString(CultureInfo.InvariantCulture));
                    writer.WriteElementString($"user_total_{ element }", totals.Sum().ToString(CultureInfo.InvariantCulture));

                    for (int i = 0; i < totals.Length; i++)
                    {
                        writer.WriteElementString(totalNames[i], totals[i].ToString(CultureInfo.InvariantCulture));
                    }

                    writer.WriteEndElement();

                    for (int i = 0; i < writeEntries.Count; i++)
                    {
                        if (indexes[i] >= 0)
                        {
                            writer.WriteStartElement(element);
                            writeEntries[i](writer, statusNames[indexes[i]]);
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Writes comma-separated values, in UTF-8 with a byte order mark so spreadsheets read the accents right.
        /// </summary>
        private static byte[] Csv(string[] header, IEnumerable<object[]> rows)
        {
            StringBuilder csv = new StringBuilder();

            foreach (IEnumerable<object> row in new[] { header }.Concat(rows))
            {
                csv.Append(string.Join(",", row.Select(Field))).Append("\r\n");
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        }

        /// <summary>
        /// Writes a value of a CSV row, quoted when it holds a separator, a quote or a line break.
        /// </summary>
        private static string Field(object value)
        {
            string text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;

            return text.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0 ? $"\"{ text.Replace("\"", "\"\"") }\"" : text;
        }

        private static string Label(IReadOnlyList<string> labels, short? status)
        {
            int index = status.HasValue ? ListStatus.Codes.ToList().IndexOf(status.Value) : -1;

            return index >= 0 ? labels[index] : string.Empty;
        }

        /// <summary>
        /// A score, where 0 means "not scored".
        /// </summary>
        private static byte? Score(byte? score) => score > 0 ? score : null;

        /// <summary>
        /// A number of MyAnimeList's export, where 0 stands for unknown.
        /// </summary>
        private static string Number(int? value) => (value ?? 0).ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
