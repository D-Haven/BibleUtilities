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
    ///     Provide a convenience class to get the already translated books
    ///     of the Bible.  Organizes the books in order, and categorizes them
    ///     as New and Old Testaments.
    /// </summary>
    public class Bible
    {
        private readonly List<Book> newTestament;
        private readonly List<Book> oldTestament;
        private CultureInfo bibleCulture;

        public Bible() : this(CultureInfo.CurrentCulture)
        {
        }

        public Bible(CultureInfo culture)
        {
            bibleCulture = culture;

            oldTestament = new List<Book>
            {
                new Book("Genesis", 50, Culture), // Gen
                new Book("Exodus", 40, Culture), // Exod
                new Book("Leviticus", 27, Culture), // Lev
                new Book("Numbers", 36, Culture), // Num
                new Book("Deuteronomy", 34, Culture), // Deut
                new Book("Joshua", 24, Culture), // Josh
                new Book("Judges", 21, Culture), // Judg
                new Book("Ruth", 4, Culture), // Ruth
                new Book("OneSamuel", 31, Culture), // 1Sam
                new Book("TwoSamuel", 24, Culture), // 2Sam
                new Book("OneKings", 22, Culture), // 1Kgs
                new Book("TwoKings", 25, Culture), // 2Kgs
                new Book("OneChronicles", 29, Culture), // 1Chr
                new Book("TwoChronicles", 36, Culture), // 2Chr
                new Book("Ezra", 10, Culture), // Ezra
                new Book("Nehemiah", 13, Culture), // Neh
                new Book("Esther", 10, Culture), // Esth
                new Book("Job", 42, Culture), // Job
                new Book("Psalms", 150, Culture), // Ps
                new Book("Proverbs", 31, Culture), // Prov
                new Book("Ecclesiastes", 12, Culture), // Eccl
                new Book("SongOfSolomon", 8, Culture), // Song
                new Book("Isaiah", 66, Culture), // Isa
                new Book("Jeremiah", 52, Culture), // Jer
                new Book("Lamentations", 5, Culture), // Lam
                new Book("Ezekiel", 48, Culture), // Ezek
                new Book("Daniel", 12, Culture), // Dan
                new Book("Hosea", 14, Culture), // Hos
                new Book("Joel", 3, Culture), // Joel
                new Book("Amos", 9, Culture), // Amos
                new Book("Obadaiah", 1, Culture), // Obad
                new Book("Jonah", 4, Culture), // Jonah
                new Book("Micah", 7, Culture), // Mic
                new Book("Nahum", 3, Culture), // Nah
                new Book("Habakkuk", 3, Culture), // Hab
                new Book("Zephaniah", 3, Culture), // Zeph
                new Book("Haggai", 2, Culture), // Hag
                new Book("Zechariah", 14, Culture), // Zech
                new Book("Malachai", 4, Culture) // Mal
            };

            newTestament = new List<Book>
            {
                new Book("Matthew", 28, Culture), // Matt
                new Book("Mark", 16, Culture), // Mark
                new Book("Luke", 24, Culture), // Luke
                new Book("John", 21, Culture), // John
                new Book("Acts", 28, Culture), // Acts
                new Book("Romans", 16, Culture), // Rom
                new Book("OneCorinthians", 16, Culture), // 1Cor
                new Book("TwoCorinthians", 13, Culture), // 2Cor
                new Book("Galatians", 6, Culture), // Gal
                new Book("Ephesians", 6, Culture), // Eph
                new Book("Philippians", 4, Culture), // Phil
                new Book("Colossians", 4, Culture), // Col
                new Book("OneThessalonians", 5, Culture), // 1Thess
                new Book("TwoThessalonians", 3, Culture), // 2Thess
                new Book("OneTimothy", 6, Culture), // 1Tim
                new Book("TwoTimothy", 4, Culture), // 2Tim
                new Book("Titus", 3, Culture), // Titus
                new Book("Philemon", 1, Culture), // Phlm
                new Book("Hebrews", 13, Culture), // Heb
                new Book("James", 5, Culture), // Jas
                new Book("OnePeter", 5, Culture), // 1Pet
                new Book("TwoPeter", 3, Culture), // 2Pet
                new Book("OneJohn", 5, Culture), // 1John
                new Book("TwoJohn", 1, Culture), // 2John
                new Book("ThreeJohn", 1, Culture), // 3John
                new Book("Jude", 1, Culture), // Jude
                new Book("Revelation", 22, Culture) // Rev
            };
        }

        public IList<Book> OldTestament => oldTestament;

        public IList<Book> NewTestament => newTestament;

        public IEnumerable<Book> AllBooks => oldTestament.Union(newTestament);

        public CultureInfo Culture
        {
            get => bibleCulture;
            set
            {
                if (!Equals(bibleCulture, value))
                {
                    bibleCulture = value;

                    foreach (var book in AllBooks)
                        book.Culture = value;
                }
            }
        }
    }
}