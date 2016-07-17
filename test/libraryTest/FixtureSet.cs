using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FixturesGenerator.Base.Tests
{
    public class FixtureSetTests
    {
        [Fact]
        public void GetInvertedPairsReturnsMirroredPairs()
        {
            // Arrange
            var pair = new Pair<char>('A', 'B');
            var pairSet = new FixtureSet<char>(new []{pair});

            // Act
            var invertedPairSet = pairSet.GetInvertedPairs();

            // Assert
            Assert.Equal(pair.Invert(), invertedPairSet.First());
        }
    }
}