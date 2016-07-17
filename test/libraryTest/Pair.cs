using Xunit;

namespace FixturesGenerator.Base.Tests
{
    public class PairTests
    {
        public static Pair<char> BasicCharPair = new Pair<char>('A', 'B');

        [Fact]
        public void EqualsTrueWhenComponentsMatchExactly()
        {
            // Arrange
            Pair<char> pairToCompare = new Pair<char>('A', 'B');

            // Assert
            Assert.True(BasicCharPair.Equals(pairToCompare));
            Assert.True(pairToCompare.Equals(BasicCharPair));
        }

        [Fact]
        public void EqualsFalseWhenComponentsDontMatchExactly()
        {
            // Arrange
            Pair<char> pairToCompare = new Pair<char>('B', 'A');

            // Assert
            Assert.False(BasicCharPair.Equals(pairToCompare));
            Assert.False(pairToCompare.Equals(BasicCharPair));
        }
    }
}