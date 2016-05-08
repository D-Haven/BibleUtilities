using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DHaven.BibleUtilities
{
    /// <summary>
    /// Extension methods to consolidate a list of scripture references to the
    /// smallest set that covers everything in the list.
    /// </summary>
    public static class ScriptureExtensions
    {
        /// <summary>
        /// Reduce a list of strings of scriptures to a smaller list of strings.
        /// Uses the current culture.
        /// </summary>
        /// <param name="source">the list of scripture references</param>
        /// <returns>a reduced set of scripture references</returns>
        public static HashSet<string> ReduceScriptures(this IEnumerable<string> sourcee)
        {
            return ReduceScriptures(sourcee, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Reduce a list of strings of scriptures to a smaller list of strings.
        /// </summary>
        /// <param name="source">the list of scripture references</param>
        /// <param name="culture">the culture to use for parsing scripture references</param>
        /// <returns>a reduced set of scripture references</returns>
        public static HashSet<string> ReduceScriptures(this IEnumerable<string> source, CultureInfo culture)
        {
            ICollection<Reference> references = source.Select(r => Reference.Parse(r, culture)).Reduce();
            return new HashSet<string>(references.Select(r => r.ToString()));
        }

        /// <summary>
        /// Reduce a list of References to the smallest set of references that
        /// covers the same set as the original.
        /// </summary>
        /// <param name="source">the list of references</param>
        /// <returns>a reduced set of references</returns>
        public static ICollection<Reference> Reduce(this IEnumerable<Reference> source)
        {
            Reference lastReference = null;
            List<Reference> destination = new List<Reference>();

            foreach (Reference check in source.OrderBy(r => r))
            {
                if (lastReference != null)
                {
                    if (lastReference.Book.Equals(check.Book) && lastReference.Chapter == check.Chapter)
                    {
                        HashSet<int> uniqueVerses = new HashSet<int>(lastReference.Verses.Union(check.Verses));
                        lastReference.Verses = uniqueVerses.OrderBy(v => v).ToArray();
                    }
                    else
                    {
                        destination.Add(lastReference);
                        lastReference = null;
                    }
                }

                lastReference = check;
            }

            if (lastReference != null)
            {
                destination.Add(lastReference);
            }

            return destination;
        }
    }
}
