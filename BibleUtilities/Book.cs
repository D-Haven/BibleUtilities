#region Copyright 2016 D-Haven.org

//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Globalization;
using System.Linq;

namespace DHaven.BibleUtilities
{
    public class Book : IFormattable, IComparable<Book>, IEquatable<Book>
    {
        private static int numCreated;
        private readonly string bookResourceName;
        private readonly int order;

        /// <summary>
        ///     Create the book internally.
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
        ///     The current culture for this book.  Controls how it retrieves the resources.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        ///     The unabbreviated name of the book.
        /// </summary>
        public string Name => Resources.Books.GetString(bookResourceName, Culture);

        /// <summary>
        ///     Standard abbreviations as defined in "The Christian Writer's
        ///     Manual of Style", 2004 edition (ISBN: 9780310487715).
        /// </summary>
        public string StandardAbreviation => Resources.StandardAbbreviations.GetString(bookResourceName, Culture);

        /// <summary>
        ///     Thompson Chain references, pulled from the 5th edition.
        /// </summary>
        public string ThompsonAbreviation => Resources.ThompsonAbbreviations.GetString(bookResourceName, Culture);

        public string CommonMistake => Resources.CommonMistakes.GetString(bookResourceName, Culture);

        /// <summary>
        ///     The number of chapters in the book.
        /// </summary>
        public int ChapterCount { get; }

        /// <summary>
        ///     Provides a mechanism to sort books by the order in the Bible.
        ///     Relies on the fact that all the books used in the system are
        ///     defined statically.
        /// </summary>
        /// <param name="other">the other book to compare</param>
        /// <returns>0 if equal or greater or less than depending on order</returns>
        public int CompareTo(Book other)
        {
            return order.CompareTo(other.order);
        }

        public bool Equals(Book other)
        {
            if (other != null)
                return bookResourceName == other.bookResourceName && ChapterCount == other.ChapterCount;

            return false;
        }

        /// <summary>
        ///     Format the book part of a reference with one of the formats.  The default format is "N".
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Format</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>T</term>
        ///             <description>
        ///                 Use the Thompson Chain Reference format.
        ///                 <example>1 Chr</example>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>S</term>
        ///             <description>
        ///                 Use the Standard Abbreviation format as defined in "The Christian Writer's Manual of Style"
        ///                 (2004).
        ///                 <example>1 Chron.</example>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>s</term>
        ///             <description>
        ///                 Use the Standard Abbreviation format as defined in "The Christian Writer's Manual of Style"
        ///                 (2004), but with Roman numerals.
        ///                 <example>I Chron.</example>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <terms>N</terms>
        ///             <description>
        ///                 Use the full book name.
        ///                 <example>2 Chronicles</example>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <terms>n</terms>
        ///             <description>
        ///                 Use the full book name, but with Roman numerals.
        ///                 <example>II Chronicles</example>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="format">the format spec</param>
        /// <param name="formatProvider">the culture specific formatter (unused)</param>
        /// <returns>the formatted string</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // Default to Thompson references if none provided
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

        public override bool Equals(object obj)
        {
            return Equals(obj as Book);
        }

        /// <summary>
        ///     Test for equality
        /// </summary>
        /// <param name="book1">the left hand book</param>
        /// <param name="book2">the right hand book</param>
        /// <returns>true if they are equal</returns>
        public static bool operator ==(Book book1, Book book2)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(book1, book2))
                return true;

            // If one is null, but not both, return false.
            if ((object) book1 == null || (object) book2 == null)
                return false;

            return book1.Equals(book2);
        }

        /// <summary>
        ///     Test for inequality.
        /// </summary>
        /// <param name="book1">the left hand book</param>
        /// <param name="book2">the right hand book</param>
        /// <returns>true if they are not equal</returns>
        public static bool operator !=(Book book1, Book book2)
        {
            // If both are null, or both are same instance, return false.
            if (ReferenceEquals(book1, book2))
                return false;

            // If one is null, but not both, return true.
            if ((object) book1 == null || (object) book2 == null)
                return true;

            return !book1.Equals(book2);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;

                if (bookResourceName != null)
                    hash = hash * 23 + bookResourceName.GetHashCode();

                hash = hash * 23 + ChapterCount.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///     Display this book as a string, uses the full name format.
        /// </summary>
        /// <returns>the formatted book</returns>
        public override string ToString()
        {
            return ToString("N", Culture);
        }

        /// <summary>
        ///     Format the string with the current culture.
        ///     <see cref="ToString(string,IFormatProvider)" />
        /// </summary>
        /// <param name="format">the format spec</param>
        /// <returns>the formatted book</returns>
        public string ToString(string format)
        {
            return ToString(format, Culture);
        }

        /// <summary>
        ///     Format the number part of the books that have more than one part as
        ///     a Roman numeral rather than the Arabic numeral
        /// </summary>
        /// <param name="book">the book to reformat</param>
        /// <returns></returns>
        private string ToRomanNumeral(string book)
        {
            var parts = book.Split(' ');

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
        ///     Parses the string and returns a book instance if it matches.
        /// </summary>
        /// <exception cref="FormatException">If the book format could not be recognized</exception>
        /// <param name="inString">the string to parse</param>
        /// <returns>a book</returns>
        public static Book Parse(string inString)
        {
            return Parse(inString, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Parses the string and returns a book instance if it matches.
        /// </summary>
        /// <exception cref="FormatException">If the book format could not be recognized</exception>
        /// <param name="inString">the string to parse</param>
        /// <param name="culture">the language/culture to use for parsing</param>
        /// <returns>a book</returns>
        public static Book Parse(string inString, CultureInfo culture)
        {
            Book book;
            if (TryParse(inString, culture, out book))
                return book;

            throw new FormatException(string.Format("Could not recognize the book {0}", inString));
        }

        /// <summary>
        ///     Tries to parse the string into a Book.  If it can't it will return false instead
        ///     of throwing an exception.
        /// </summary>
        /// <param name="inString">The string to parse</param>
        /// <param name="book">the book that was found (or null)</param>
        /// <returns>true if found, false if not</returns>
        public static bool TryParse(string inString, out Book book)
        {
            return TryParse(inString, CultureInfo.CurrentCulture, out book);
        }

        /// <summary>
        ///     Tries to parse the string into a Book.  If it can't it will return false instead
        ///     of throwing an exception.
        /// </summary>
        /// <param name="inString">The string to parse</param>
        /// <param name="culture">The culture to return results for</param>
        /// <param name="book">the book that was found (or null)</param>
        /// <returns>true if found, false if not</returns>
        public static bool TryParse(string inString, CultureInfo culture, out Book book)
        {
            var potentialBook = StandardizeBookOrdinals(inString, culture);

            var bible = new Bible(culture);

            // Find the first book where the input string now matches one of the recognized formats.
            book = bible.AllBooks.FirstOrDefault(
                b => b.ThompsonAbreviation.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase)
                     || b.StandardAbreviation.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase)
                     || b.Name.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase)
                     || b.CommonMistake.Equals(potentialBook, StringComparison.CurrentCultureIgnoreCase));

            if (book != null)
                return true;

            // If we didn't find it, check to see if we just missed it because the abbreviation
            // didn't have a period
            book = bible.AllBooks.FirstOrDefault(b =>
            {
                var stdAbbrev = b.StandardAbreviation;
                if (stdAbbrev.EndsWith("."))
                    stdAbbrev = stdAbbrev.Substring(0, stdAbbrev.Length - 1);

                return potentialBook == stdAbbrev;
            });

            if (book != null)
                return true;

            return book != null;
        }

        private static string StandardizeBookOrdinals(string str, CultureInfo culture)
        {
            // Break up on all remaining white space
            var parts = (str ?? "").Trim().Split(' ', '\r', '\n', '\t');

            var first = Resources.Books.GetString("First", culture).ToLower();
            var second = Resources.Books.GetString("Second", culture).ToLower();
            var third = Resources.Books.GetString("Third", culture).ToLower();
            var fourth = Resources.Books.GetString("Fourth", culture).ToLower();
            var fifth = Resources.Books.GetString("Fifth", culture).ToLower();

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
                    if (number == first)
                        parts[0] = "1";
                    else if (number == second)
                        parts[0] = "2";
                    else if (number == third)
                        parts[0] = "3";
                    else if (number == fourth)
                        parts[0] = "4";
                    else if (number == fifth)
                        parts[0] = "5";
                    break;
            }

            // Recompile the parts into one string that only has a single space separating elements
            return string.Join(" ", parts);
        }
    }
}