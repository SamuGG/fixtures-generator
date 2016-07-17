using System;
using System.Collections.Generic;
using System.Linq;
using FixturesGenerator.Tests;
using Xunit;

namespace FixturesGenerator.Base.Tests
{
    public class FixtureSetsTests
    {
        [Fact]
        public void GetInvertedSetsReturnsMirroredPairs()
        {
            // Arrange
            var set1 = FixtureSets<char>.NewItem(new Pair<char>[] { new Pair<char>('A', 'B') });
            var set2 = FixtureSets<char>.NewItem(new Pair<char>[] { new Pair<char>('C', 'D') });

            var sets = new FixtureSets<char>();
            sets.Add(set1);
            sets.Add(set2);

            // Act
            var invertedSets = sets.GetInvertedSets();

            // Assert
            Assert.Equal(sets.Count, invertedSets.Count());
            foreach (var invertedSet in invertedSets)
            {
                var originalPairSet = sets.Single(s => s[0].IsRelatedTo(invertedSet[0]));
                Assert.True(FixtureHelpers.SetsAreInverted(originalPairSet, invertedSet));
            }
        }

        [Fact]
        public void SwapPositionsTest()
        {
            // Arrange
            var sets = new FixtureSets<char>(2);
            var set1 = FixtureSets<char>.NewItem(new []{new Pair<char>('A', 'B')});
            var set2 = FixtureSets<char>.NewItem(new []{new Pair<char>('C', 'D')});

            sets.Add(set1);
            sets.Add(set2);

            // Act
            sets.SwapPositions(0, 1);

            // Assert
            Assert.Equal(set2[0], sets[0][0]);
            Assert.Equal(set1[0], sets[1][0]);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 2)]
        public void SwapPositionsThrowWithInvalidArguments(int position1, int position2)
        {
            // Arrange
            var sets = new FixtureSets<char>(new []{FixtureSets<char>.NewItem(new []{new Pair<char>('A', 'B')})});
            
            // Act
            var exception = Record.Exception(() => sets.SwapPositions(position1, position2));
            
            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }
    }
}
