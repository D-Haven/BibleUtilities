using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DHaven.BibleUtilities.Test
{
    public class GivenSetOfReferences
    {
        [Fact]
        public void IdenticalReferencesShouldBecomeOne()
        {
            var source = new [] { "1 John 3:2", "1 Jn 3:2" };
            var reduced = source.ReduceScriptures();

            Assert.Equal(1, reduced.Count);
            Assert.Contains("1 John 3:2", reduced);
        }

        [Fact]
        public void CompletelySeparateReferencesShouldStaySeparate()
        {
            var source = new[] {"3 John 2", "Deut. 14:5"};
            var reduced = source.ReduceScriptures();

            Assert.Equal(2, reduced.Count);
            Assert.Contains("3 John 2", reduced);
            Assert.Contains("Deuteronomy 14:5", reduced);
        }

        [Fact]
        public void OverlappingReferencesShouldBeConsolidated()
        {
            var source = new[] {"1 Cor 13:3,6", "1 Cor 13:5", "1 Cor 13:4,8"};
            var reduced = source.ReduceScriptures();

            Assert.Equal(1, reduced.Count);
            Assert.Contains("1 Corinthians 13:3-6,8", reduced);
        }
    }
}
