using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHaven.BibleUtilities
{
    public class Book : IFormattable, IComparable<Book>, IEquatable<Book>
    {
        private static int numCreated = 0;
        private int order;
        private string bookResourceName;

        /// <summary>
        /// Create the book internally.
        /// </summary>
        /// <param name="book">the resource name for the book</param>
        /// <param name="chapters">the number of chapters in that book</param>
        /// <param name="culture">the current culture used for the books</param>
        internal Book(string book, int chapters, CultureInfo culture)
        {
            order = numCreated;
            bookResourceName = book;
            ChapterCount = chapters;
            Culture = culture;
            numCreated++;
        }

        /// <summary>
        /// The current culture for this book.  Controls how it retrieves the resources.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// The unabreviated name of the book.
        /// </summary>
        public string Name { get { return Resources.Books.ResourceManager.GetString(bookResourceName, Culture); } }

        /// <summary>
        /// Standard abbreviations as defined in "The Christian Writer's
        /// Manual of Style", 2004 edition (ISBN: 9780310487715).
        /// </summary>
        public string StandardAbreviation { get { return Resources.StandardAbbreviations.ResourceManager.GetString(bookResourceName, Culture); } }

        /// <summary>
        /// Thompson Chain references, pulled from the 5th edition.
        /// </summary>
        public string ThompsonAbreviation { get { return Resources.ThompsonAbbreviations.ResourceManager.GetString(bookResourceName, Culture); } }

        public string CommonMistake { get { return Resources.CommonMistakes.ResourceManager.GetString(bookResourceName, Culture); } }

        /// <summary>
        /// The number of chapters in the book.
        /// </summary>
        public int ChapterCount { get; private set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Book);
        }

        public bool Equals(Book other)
        {
            if (other != null)
            {
                return bookResourceName == other.bookResourceName && ChapterCount == other.ChapterCount;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;

                if (bookResourceName != null)
                {
                    hash = hash * 23 + bookResourceName.GetHashCode();
                }

                hash = hash * 23 + ChapterCount.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Display this book as a string, uses the full name format.
        /// </summary>
        /// <returns>the formatted book</returns>
        public override string ToString()
        {
            return ToString("N", Culture);
        }

        /// <summary>
        /// Format the string with the current culture.
        /// <see cref="ToString(string,IFormatProvider)"/>
        /// </summary>
        /// <param name="format">the format spec</param>
        /// <returns>the formatted book</returns>
        public string ToString(string format)
        {
            return ToString(format, Culture);
        }

        /// <summary>
        /// Format the book part of a reference with one of the formats.  The default format is "N".
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
            // Default to Thompsan references if none provided
            format = format ?? "T";

            switch (format)
            {
                case "T":
                    return ThompsonAbreviation;

                case "S":
                    return StandardAbreviation;

                case "s":
                    return ToRomanNumeral(StandardAbreviation);

                case "N":
                    return Name;

                case "n":
                    return ToRomanNumeral(Name);
            }

            throw new FormatException(string.Format("The {0} format string is not supported.", format));
        }

        /// <summary>
        /// Format the number part of the books that have more than one part as
        /// a Roman numeral rather than the Arabic numeral
        /// </summary>
        /// <param name="book">the book to reformat</param>
        /// <returns></returns>
        private string ToRomanNumeral(string book)
        {
            string[] parts = book.Split(' ');

            // We only have to convert the first part of the book to a roman numeral
            // And the highest we go is 3 (3 Jn).
            switch (parts[0])
            {
                case "1":
                    parts[0] = "I";
                    break;

                case "2":
                    parts[0] = "II";
                    break;

                case "3":
                    parts[0] = "III";
                    break;

                case "4":
                    parts[0] = "IV";
                    break;

                case "5":
                    parts[0] = "V";
                    break;
            }

            return string.Join(" ", parts);
        }

        /// <summary>
        /// Provides a mechanism to sort books by the order in the Bible.
        /// Relies on the fact that all the books used in the system are
        /// defined statically.
        /// </summary>
        /// <param name="other">the other book to compare</param>
        /// <returns>0 if equal or greater or less than depending on order</returns>
        public int CompareTo(Book other)
        {
            return order.CompareTo(other.order);
        }

        /// <summary>
        /// Parses the string and returns a book instance if it matches.
        /// </summary>
        /// <exception cref="FormatException">If the book format could not be recognized</exception>
        /// <param name="inString">the string to parse</param>
        /// <returns>a book</returns>
        public static Book Parse(string inString)
        {
            return Parse(inString, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Parses the string and returns a book instance if it matches.
        /// </summary>
        /// <exception cref="FormatException">If the book format could not be recognized</exception>
        /// <param name="inString">the string to parse</param>
        /// <param name="culture">the language/culture to use for parsing</param>
        /// <returns>a book</returns>
        public static Book Parse(string inString, CultureInfo culture)
        {
            Book book;
            if (TryParse(inString, culture, out book))
            {
                return book;
            }

            throw new FormatException(string.Format("Could not recognize the book {0}", inString));
        }

        /// <summary>
        /// Tries to parse the string into a Book.  If it can't it will return false instead
        /// of throwing an exception.
        /// </summary>
        /// <param name="inString">The string to parse</param>
        /// <param name="book">the book that was found (or null)</param>
        /// <returns>true if found, false if not</returns>
        public static bool TryParse(string inString, out Book book)
        {
            return TryParse(inString, CultureInfo.CurrentCulture, out book);
        }

        /// <summary>
        /// Tries to parse the string into a Book.  If it can't it will return false instead
        /// of throwing an exception.
        /// </summary>
        /// <param name="inString">The string to parse</param>
        /// <param name="book">the book that was found (or null)</param>
        /// <returns>true if found, false if not</returns>
        public static bool TryParse(string inString, CultureInfo culture, out Book book)
        {
            string potentialBook = StandardizeBookOrdinals(inString, culture);

            Bible bible = new Bible(culture);

            // Find the first book where the input string now matches one of the recognized formats.
            book = bible.AllBooks.FirstOrDefault(
                b => b.ThompsonAbreviation.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase)
                    || b.StandardAbreviation.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase)
                    || b.Name.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase)
                    || b.CommonMistake.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase));

            if (book != null)
            {
                return true;
            }

            // If we didn't find it, check to see if we just missed it because the abreviation
            // didn't have a period
            book = bible.AllBooks.FirstOrDefault((b) =>
            {
                string stdAbrev = b.StandardAbreviation;
                if (stdAbrev.EndsWith("."))
                {
                    stdAbrev = stdAbrev.Substring(0, stdAbrev.Length - 1);
                }

                return potentialBook == stdAbrev;
            });

            if (book != null)
            {
                return true;
            }

            return book != null;
        }

        private static string StandardizeBookOrdinals(string str, CultureInfo culture)
        {
            // Break up on all remaining white space
            string[] parts = (str ?? "").Trim().Split(' ', '\r', '\n', '\t');

            string first = Resources.Books.ResourceManager.GetString("First", culture).ToLower();
            string second = Resources.Books.ResourceManager.GetString("Second", culture).ToLower();
            string third = Resources.Books.ResourceManager.GetString("Third", culture).ToLower();
            string fourth = Resources.Books.ResourceManager.GetString("Fourth", culture).ToLower();
            string fifth = Resources.Books.ResourceManager.GetString("Fifth", culture).ToLower();

            // If the first part is a roman numeral, or spelled ordinal, convert it to arabic
            var number = parts[0].ToLower();
            switch (number)
            {
                case "i":
                    parts[0] = "1";
                    break;

                case "ii":
                    parts[0] = "2";
                    break;

                case "iii":
                    parts[0] = "3";
                    break;

                case "iv":
                    parts[0] = "4";
                    break;

                case "v":
                    parts[0] = "5";
                    break;

                default:
                    if(number == first)
                    {
                        parts[0] = "1";
                    }
                    else if(number == second)
                    {
                        parts[0] = "2";
                    }
                    else if (number == third)
                    {
                        parts[0] = "3";
                    }
                    else if (number == fourth)
                    {
                        parts[0] = "4";
                    }
                    else if (number == fifth)
                    {
                        parts[0] = "5";
                    }
                    break;
            }

            // Recompile the parts into one string that only has a single space separating elements
            return string.Join(" ", parts);
        }
    }
}
