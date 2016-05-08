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

using DHaven.BibleUtilities;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;

namespace BibleUtilities.Test
{
    [TestFixture]
    public class BookTest
    {
        private Bible englishBible = new Bible(new CultureInfo("en"));
        private Bible norwegianBible = new Bible(new CultureInfo("nb"));

        [Test]
        public void CanParseToString()
        {
            foreach (Book book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
            {
                // Ensure we can do the default ToString() processing
                string toString = book.ToString();
                Assert.That(toString, Is.Not.Null.Or.Empty);

                // And ensure that it comes back to the same book
                Book newBook = Book.Parse(toString);
                Assert.That(newBook, Is.EqualTo(book));
            }
        }

        [Test]
        public void InvalidFormatTypeThrowsException()
        {
            Book book = englishBible.OldTestament.First();

            Assert.Throws<FormatException>(() => book.ToString("?"));
        }

        [Test]
        public void ToStringThompson()
        {
            foreach (Book book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
            {
                Assert.That(book.ToString("T"), Is.EqualTo(book.ThompsonAbreviation));
            }
        }

        [Test]
        public void ToStringStandard()
        {
            foreach (Book book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
            {
                Assert.That(book.ToString("S"), Is.EqualTo(book.StandardAbreviation));
            }
        }

        [Test]
        public void ToStringFullName()
        {
            foreach (Book book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
            {
                Assert.That(book.ToString("N"), Is.EqualTo(book.Name));
            }
        }

        [Test]
        public void BadBookNameCannotBeParsed()
        {
            Assert.Throws<FormatException>(() => Book.Parse("This is not a book in the Bible"));
        }

        [Test]
        public void CanParseAllToStringVariants()
        {
            foreach (Book book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
            {
                Book parsedThompson = Book.Parse(book.ToString("T"));
                Book parsedStandard = Book.Parse(book.ToString("S"));
                Book parsedStandardRoman = Book.Parse(book.ToString("s"));
                Book parsedName = Book.Parse(book.ToString("N"));
                Book parsedNameRoman = Book.Parse(book.ToString("n"));

                foreach (Book parsed in new[] { parsedName, parsedNameRoman, parsedStandard, parsedStandardRoman, parsedThompson })
                {
                    Assert.That(parsed, Is.EqualTo(book));
                }
            }
        }
    }
}
