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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BibleUtilities.Test
{
    [TestFixture]
    public class ReferenceTest
    {
        [Test]
        public void ParseSimpleReference()
        {
            string text = "1 John 1:9";

            Reference reference = Reference.Parse(text, new CultureInfo("en"));

            Assert.That(reference.Book.ToString(), Is.EqualTo("1 John"));
            Assert.That(reference.Chapter, Is.EqualTo(1));
            Assert.That(reference.Verses, Has.Member(9));
            Assert.That(reference.Verses.Length, Is.EqualTo(1));
        }

        [Test]
        public void ParseCommaDelimitedReference()
        {
            string text = "Ge 2:4,6,8";

            Reference reference = Reference.Parse(text, new CultureInfo("en"));

            Assert.That(reference.Book.ToString(), Is.EqualTo("Genesis"));
            Assert.That(reference.Chapter, Is.EqualTo(2));
            Assert.That(reference.Verses.Length, Is.EqualTo(3));
            Assert.That(reference.Verses, Has.Member(4));
            Assert.That(reference.Verses, Has.Member(6));
            Assert.That(reference.Verses, Has.Member(8));
        }

        [Test]
        public void ParseRangeReference()
        {
            string text = "Heb. 12:1-4";

            Reference reference = Reference.Parse(text, new CultureInfo("en"));

            Assert.That(reference.Book.ToString(), Is.EqualTo("Hebrews"));
            Assert.That(reference.Chapter, Is.EqualTo(12));
            Assert.That(reference.Verses.Length, Is.EqualTo(4));

            foreach (int i in Enumerable.Range(1, 4))
            {
                Assert.That(reference.Verses, Has.Member(i));
            }
        }

        [Test]
        public void ParseCombinedCommaAndRange()
        {
            string text = "Galatians 6:2,4-5,8,16-18";

            Reference reference = Reference.Parse(text, new CultureInfo("en"));

            Assert.That(reference.Book.ToString(), Is.EqualTo("Galatians"));
            Assert.That(reference.Chapter, Is.EqualTo(6));
            Assert.That(reference.Verses.Length, Is.EqualTo(7));

            Assert.That(reference.Verses, Has.Member(2));

            foreach (int i in Enumerable.Range(4, 2))
            {
                Assert.That(reference.Verses, Has.Member(i));
            }

            Assert.That(reference.Verses, Has.Member(8));

            foreach (int i in Enumerable.Range(16, 3))
            {
                Assert.That(reference.Verses, Has.Member(i));
            }
        }

        [Test]
        public void ParseOnlyVerseReferences()
        {
            string text = "II John 10";

            Reference reference = Reference.Parse(text, new CultureInfo("en"));

            Assert.That(reference.Book.ToString(), Is.EqualTo("2 John"));
            Assert.That(reference.Book.ChapterCount, Is.EqualTo(1));
            Assert.That(reference.Chapter, Is.EqualTo(1));
            Assert.That(reference.Verses.Length, Is.EqualTo(1));
            Assert.That(reference.Verses, Has.Member(10));
        }

        [Test]
        public void ParseOnlyChapterReference()
        {
            string text = "Gal. 3";

            Reference reference = Reference.Parse(text, new CultureInfo("en"));

            Assert.That(reference.Book.ToString(), Is.EqualTo("Galatians"));
            Assert.That(reference.Chapter, Is.EqualTo(3));
            Assert.That(reference.Verses.Length, Is.EqualTo(0));
        }

        [Test]
        public void ParseOnlyVerseWithRange()
        {
            string text = "3 Jn 5-7";

            Reference reference = Reference.Parse(text, new CultureInfo("en"));

            Assert.That(reference.Book.ToString(), Is.EqualTo("3 John"));
            Assert.That(reference.Book.ChapterCount, Is.EqualTo(1));
            Assert.That(reference.Chapter, Is.EqualTo(1));
            Assert.That(reference.Verses.Length, Is.EqualTo(3));

            foreach (int i in Enumerable.Range(5, 3))
            {
                Assert.That(reference.Verses, Has.Member(i));
            }
        }

        [Test]
        public void ParseChapterRangeNotSupported()
        {
            string text = "2 Cor. 5-7";

            Assert.Throws<FormatException>(() => Reference.Parse(text, new CultureInfo("en")));
        }

        [Test]
        public void ParseInvalidRange()
        {
            Assert.Throws<FormatException>(() => Reference.Parse("Gen. 3:4-5-9", new CultureInfo("en")));
        }

        [Test]
        public void ParseRangeInReverse()
        {
            Assert.Throws<FormatException>(() => Reference.Parse("Lev. 6:12-8", new CultureInfo("en")));
        }

        [Test]
        public void ParseChapterTooHigh()
        {
            Assert.Throws<FormatException>(() => Reference.Parse("Ps 256:128", new CultureInfo("en")));
        }

        [Test]
        public void ScanForVersesFindsThemInsideParantheses()
        {
            ICollection<Reference> references = Reference.Scan("This is random text with a reference (1 Co 13:3)");

            Assert.That(references.Count, Is.EqualTo(1));
            Assert.That(references.ElementAt(0), Is.EqualTo(Reference.Parse("1 Cor. 13:3", new CultureInfo("en"))));
        }

        [Test]
        public void ScanCanFindSemicolonSeparatedScriptures()
        {
            ICollection<Reference> references = Reference.Scan("Lorem ipsum dolor -- 2 Kings 5:23;Prov. 3:13", new CultureInfo("en"));

            Assert.That(references.Count, Is.EqualTo(2));

            Assert.That(references.ElementAt(0), Is.EqualTo(Reference.Parse("2 K 5:23", new CultureInfo("en"))));
            Assert.That(references.ElementAt(1), Is.EqualTo(Reference.Parse("Pr 3:13", new CultureInfo("en"))));
        }
    }
}
