using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DHaven.BibleUtilities
{
    /// <summary>
    /// Represents one scripture reference.  Assumes all verses are in the same chapter.
    /// </summary>
    public class Reference : IFormattable, IComparable<Reference>, IEquatable<Reference>
    {
        private static readonly Regex RemoveHtml = new Regex("<[^>]*>");

        public Book Book { get; set; }

        public int Chapter { get; set; }

        public int[] Verses { get; set; }

        public int CompareTo(Reference other)
        {
            int compare = Book.CompareTo(other.Book);

            if (compare == 0)
            {
                compare = Chapter.CompareTo(other.Chapter);
            }

            if (compare == 0)
            {
                Verses = Verses.OrderBy(v => v).ToArray();
                other.Verses = other.Verses.OrderBy(v => v).ToArray();

                for (int i = 0; compare == 0 && i < Math.Max(Verses.Length, other.Verses.Length); i++)
                {
                    if (i < Verses.Length && i < other.Verses.Length)
                    {
                        compare = Verses[i].CompareTo(other.Verses[i]);
                    }
                    else
                    {
                        compare = Verses.Length - other.Verses.Length;
                    }
                }
            }

            return compare;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Reference);
        }


        public bool Equals(Reference other)
        {
            if (other != null)
            {
                bool versesEqual = Verses.Length == other.Verses.Length;
                for (int i = 0; versesEqual && i < Verses.Length; i++)
                {
                    versesEqual = versesEqual && Verses[i] == other.Verses[i];
                }

                return Book == other.Book && Chapter == other.Chapter && versesEqual;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;

                if (Book != null)
                {
                    hash = hash * 23 + Book.GetHashCode();
                }

                hash = hash * 23 + Chapter.GetHashCode();

                if (Verses != null)
                {
                    foreach (int verse in Verses)
                    {
                        hash = hash * 23 + verse.GetHashCode();
                    }
                }

                return hash;
            }
        }

        public static ICollection<Reference> Scan(string text)
        {
            return Scan(text, CultureInfo.CurrentCulture);
        }

        public static ICollection<Reference> Scan(string text, CultureInfo culture)
        {
            List<Reference> references = new List<Reference>();

            if (text == null)
            {
                return references;
            }

            string[] words = RemoveHtml.Replace(text, "").Split(' ', '(', ')', ';', '\r', '\n', '\t');

            for (int i = 0; i < words.Length; i++)
            {
                string one = words[i];

                // If we are starting with a blank entry, just skip this cycle
                if (string.IsNullOrWhiteSpace(one))
                {
                    continue;
                }

                string two = i + 1 < words.Length ? string.Join(" ", one, words[i + 1]) : one;
                string three = i + 2 < words.Length ? string.Join(" ", two, words[i + 2]) : two;

                Book book;
                bool match = Book.TryParse(one, culture, out book);
                match = match || Book.TryParse(two, culture, out book);
                match = match || Book.TryParse(three, culture, out book);

                if (match)
                {
                    string four = i + 3 < words.Length ? string.Join(" ", three, words[i + 3]) : three;
                    string five = i + 4 < words.Length ? string.Join(" ", four, words[i + 4]) : four;

                    // Keep the most inclusive version of the reference
                    Reference found = null;
                    foreach (string test in new[] { two, three, four, five })
                    {
                        Reference check;
                        if (TryParse(test, culture, out check))
                        {
                            found = check;
                        }
                    }

                    if (found != null && !references.Contains(found))
                    {
                        references.Add(found);
                    }
                }
            }

            return references;
        }

        public static bool TryParse(string text, out Reference reference)
        {
            return TryParse(text, CultureInfo.CurrentCulture, out reference);
        }

        public static bool TryParse(string text, CultureInfo culture, out Reference reference)
        {
            string errorString;
            reference = InternalParse(text, culture, out errorString);

            if (errorString != null)
            {
                reference = null;
                return false;
            }

            return true;
        }

        public static Reference Parse(string text)
        {
            return Parse(text, CultureInfo.CurrentCulture);
        }

        public static Reference Parse(string text, CultureInfo culture)
        {
            string errorString;
            Reference reference = InternalParse(text, culture, out errorString);

            if (errorString != null)
            {
                throw new FormatException(errorString);
            }

            return reference;
        }

        private static Reference InternalParse(string text, CultureInfo culture, out string errorString)
        {
            errorString = null;
            int colon = text.LastIndexOf(':');
            int chapter = -1;
            string chapterSection = "1";
            string verseSection = "";

            if (colon > 0)
            {
                verseSection = text.Substring(colon + 1);
                chapter = colon - 3;

                chapterSection = text.Substring(chapter, colon - chapter);
                while (!string.IsNullOrEmpty(chapterSection) && !Char.IsDigit(chapterSection[0]))
                {
                    chapter++;
                    chapterSection = text.Substring(chapter, colon - chapter);
                }
            }
            else
            {
                chapter = 2;  // skip initial numbers for books
                while (chapter < text.Length && !Char.IsDigit(text[chapter]))
                {
                    chapter++;
                }

                if (chapter == text.Length)
                {
                    errorString = "There are no chapter or verses, can't be a reference.";
                    return null;
                }

                verseSection = text.Substring(chapter);
            }

            Book book;
            if (!Book.TryParse(text.Substring(0, chapter), culture, out book))
            {
                errorString = "There is no book, can't be a reference.";
                return null;
            }

            if (!int.TryParse(chapterSection, out chapter))
            {
                errorString = "Bad chapter format";
                return null;
            }

            Reference reference = new Reference
            {
                Book = book,
                Chapter = chapter
            };

            if (colon < 0 && reference.Book.ChapterCount > 1)
            {
                if (!int.TryParse(verseSection, out chapter))
                {
                    errorString = "Bad chapter format.";
                    return null;
                }

                reference.Chapter = chapter;
                reference.Verses = new int[0];
                return reference;
            }

            if (reference.Chapter > reference.Book.ChapterCount)
            {
                errorString = "Chapter found was too high";
                return null;
            }

            reference.Verses = ParseRanges(verseSection, out errorString);

            return reference;
        }

        private static int[] ParseRanges(string section, out string errorString)
        {
            errorString = null;
            List<int> numbers = new List<int>();
            string[] items = section.Split(',');

            foreach (string verse in items)
            {
                string[] ranges = verse.Split('-');

                if (ranges.Length > 2 || ranges.Length == 0)
                {
                    errorString = "Invalid range specification";
                    return new int[0];
                }

                int start;
                if (!int.TryParse(ranges[0], out start))
                {
                    errorString = "Invalid range specification";
                    return new int[0];
                }

                int end = start;
                if (ranges.Length > 1 && !int.TryParse(ranges[1], out end))
                {
                    errorString = "Invalid range specification";
                    return new int[0];
                }

                if (end < start)
                {
                    errorString = "invalid range specification";
                    return new int[0];
                }

                for (int i = start; i <= end; i++)
                {
                    numbers.Add(i);
                }
            }

            return numbers.ToArray();
        }

        /// <summary>
        /// Display this reference as a string, uses the full name format.
        /// </summary>
        /// <returns>the formatted book</returns>
        public override string ToString()
        {
            return ToString("N", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Format the string with the current culture.
        /// <see cref="ToString(string,IFormatProvider)"/>
        /// </summary>
        /// <param name="format">the format spec</param>
        /// <returns>the formatted book</returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Format the reference with one of the formats.  The default format is "T".
        /// <list type="table">
        /// <listheader>
        ///   <term>Format</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term>T</term>
        ///   <description>Use the Thompson Chain Reference format.  <example>1 Chr</example></description>
        /// </item>
        /// <item>
        ///   <term>S</term>
        ///   <description>Use the Standard Abbreviation format as defined in "The Christian Writer's Manual of Style" (2004).  <example>1 Chron.</example></description>
        /// </item>
        /// <item>
        ///   <term>s</term>
        ///   <description>Use the Standard Abbreviation format as defined in "The Christian Writer's Manual of Style" (2004), but with Roman numerals.  <example>I Chron.</example></description>
        /// </item>
        /// <item>
        ///   <terms>N</terms>
        ///   <description>Use the full book name.  <example>2 Chronicles</example></description>
        /// </item>
        /// <item>
        ///   <terms>n</terms>
        ///   <description>use the full book name, but with Roman numerals.  <example>II Chronicles</example></description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="format">the format spec</param>
        /// <param name="formatProvider">the culture specific formatter (unused)</param>
        /// <returns>the formatted string</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder builder = new StringBuilder(Book.ToString(format, formatProvider));

            builder.Append(" ");

            if (Book.ChapterCount > 1)
            {
                builder.Append(Chapter);

                if (Verses != null && Verses.Length > 0)
                {
                    builder.Append(":");
                }
            }

            if (Verses != null && Verses.Length > 0)
            {
                bool firstTime = true;

                for (int i = 0; i < Verses.Length; i++)
                {
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    else
                    {
                        builder.Append(",");
                    }

                    builder.Append(Verses[i]);
                    bool doRange = false;

                    while (i < Verses.Length - 1 && Verses[i + 1] == Verses[i] + 1)
                    {
                        i++;
                        doRange = true;
                    }

                    if (doRange)
                    {
                        builder.Append("-").Append(Verses[i]);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
