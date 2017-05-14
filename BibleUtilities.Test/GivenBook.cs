using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace DHaven.BibleUtilities.Test
{
    public class GivenBook
    {
        private readonly Bible englishBible = new Bible(new CultureInfo("en"));
        private readonly Bible norwegianBible = new Bible(new CultureInfo("nb"));

        [Theory]
        [InlineData("T")]
        [InlineData("S")]
        [InlineData("s")]
        [InlineData("N")]
        [InlineData("n")]
        public void ShouldParseAllToStringVariants(string format)
        {
            foreach (var book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
                Assert.Equal(book, Book.Parse(book.ToString(format)));
        }

        [Fact]
        public void ShouldFormatAsFullName()
        {
            foreach (var book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
                Assert.Equal(book.Name, book.ToString("N"));
        }

        [Fact]
        public void ShouldFormatAsStandardAbbreviations()
        {
            foreach (var book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
                Assert.Equal(book.StandardAbreviation, book.ToString("S"));
        }

        [Fact]
        public void ShouldFormatAsThompsonChainAbbreviation()
        {
            foreach (var book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
                Assert.Equal(book.ThompsonAbreviation, book.ToString("T"));
        }

        [Fact]
        public void ShouldParseResultsOfToString()
        {
            foreach (var book in englishBible.AllBooks.Union(norwegianBible.AllBooks))
            {
                var stringForm = book.ToString();

                Assert.Equal(book, Book.Parse(stringForm));
            }
        }

        [Fact]
        public void ShouldThrowExceptionParsingBadBookName()
        {
            Assert.Throws<FormatException>(() => Book.Parse("This is not a book in the Bible"));
        }

        [Fact]
        public void ShouldThrowExceptionWithInvalidFormatType()
        {
            var book = englishBible.OldTestament.First();
            Assert.Throws<FormatException>(() => book.ToString("?"));
        }
    }
}