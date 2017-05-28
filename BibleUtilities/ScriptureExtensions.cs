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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DHaven.BibleUtilities
{
    /// <summary>
    ///     Extension methods to consolidate a list of scripture references to the
    ///     smallest set that covers everything in the list.
    /// </summary>
    public static class ScriptureExtensions
    {
        /// <summary>
        ///     Reduce a list of strings of scriptures to a smaller list of strings.
        ///     Uses the current culture.
        /// </summary>
        /// <param name="source">the list of scripture references</param>
        /// <returns>a reduced set of scripture references</returns>
        public static HashSet<string> ReduceScriptures(this IEnumerable<string> source)
        {
            return ReduceScriptures(source, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Reduce a list of strings of scriptures to a smaller list of strings.
        /// </summary>
        /// <param name="source">the list of scripture references</param>
        /// <param name="culture">the culture to use for parsing scripture references</param>
        /// <returns>a reduced set of scripture references</returns>
        public static HashSet<string> ReduceScriptures(this IEnumerable<string> source, CultureInfo culture)
        {
            var references = source.Select(r => Reference.Parse(r, culture)).Reduce();
            return new HashSet<string>(references.Select(r => r.ToString()));
        }

        /// <summary>
        ///     Reduce a list of References to the smallest set of references that
        ///     covers the same set as the original.
        /// </summary>
        /// <param name="source">the list of references</param>
        /// <returns>a reduced set of references</returns>
        public static ICollection<Reference> Reduce(this IEnumerable<Reference> source)
        {
            Reference lastReference = null;
            var destination = new List<Reference>();

            foreach (var check in source.OrderBy(r => r))
            {
                if (lastReference?.Book == check.Book && lastReference?.Chapter == check.Chapter)
                {
                    var uniqueVerses = new HashSet<int>(lastReference.Verses.Union(check.Verses));
                    lastReference.Verses = uniqueVerses.OrderBy(v => v).ToArray();
                }
                else
                {
                    if (lastReference != null)
                    {
                        destination.Add(lastReference);
                    }

                    lastReference = check;
                }
            }

            if (lastReference != null)
            {
                destination.Add(lastReference);
            }

            return destination;
        }
    }
}