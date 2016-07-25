using FixturesGenerator.Generators;
using FixturesGenerator.Tests.Rules;
using Xunit;

namespace FixturesGenerator.Tests
{
    public class LeagueFixturesGeneratorTests
    {
        private LeagueFixturesGenerator m_Generator;

        public LeagueFixturesGeneratorTests()
        {
            m_Generator = new LeagueFixturesGenerator();
        }

        ~LeagueFixturesGeneratorTests()
        {
            m_Generator = null;
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(8)]
        // [InlineData(BasicFixturesGenerator.MaxNumElements)]
        public void ResultMatrixSatisfiesRules(byte numElements)
        {
            // Act
            var fixtures = m_Generator.GenerateFixtures(numElements);

            // Assert
            Assert.NotNull(fixtures);
            Assert.True(BasicFixturesGeneratorRules.ThereAreNoDuplicatedIndexes(fixtures), $"Rule {nameof(BasicFixturesGeneratorRules.ThereAreNoDuplicatedIndexes)} failed.");
            Assert.True(BasicFixturesGeneratorRules.ThereAreNoDuplicatedItems(fixtures), $"Rule {nameof(BasicFixturesGeneratorRules.ThereAreNoDuplicatedItems)} failed.");
            Assert.True(LeagueFixturesGeneratorRules.ThereAreNoInvertedItemsInSameHalf(fixtures), $"Rule {nameof(LeagueFixturesGeneratorRules.ThereAreNoInvertedItemsInSameHalf)} failed.");
            Assert.True(LeagueFixturesGeneratorRules.ThereIsNoElementRepeatingPositionMoreThanTwiceInConsecutiveRows(fixtures), $"Rule {nameof(LeagueFixturesGeneratorRules.ThereIsNoElementRepeatingPositionMoreThanTwiceInConsecutiveRows)} failed.");
        }
    }
}