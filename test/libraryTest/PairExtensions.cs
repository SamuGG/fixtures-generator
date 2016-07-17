using System.Collections.Generic;
using Xunit;

namespace FixturesGenerator.Base.Tests
{
    public class PairExtensionsTests
    {
        public static IEnumerable<object[]> TestDataForIsRelatedTo
        {
            get
            {
                yield return new object[] { new Pair<char>('A', 'B'), new Pair<char>('A', 'B'), true };
                yield return new object[] { new Pair<char>('A', 'B'), new Pair<char>('B', 'A'), true };
                yield return new object[] { new Pair<char>('A', 'B'), new Pair<char>('A', 'C'), true };
                yield return new object[] { new Pair<char>('A', 'B'), new Pair<char>('C', 'B'), true };
                yield return new object[] { new Pair<char>('A', 'B'), new Pair<char>('C', 'D'), false };
            }
        }
        
        [Theory]
        [MemberData(nameof(TestDataForIsRelatedTo))]
        public void IsRelatedWhenTheyShareAnyComponent(Pair<char> pair1, Pair<char> pair2, bool expectedResult)
        {
            Assert.Equal(expectedResult, pair1.IsRelatedTo(pair2));
        }

        [Fact]
        public void TestInvert()
        {
            // Act
            Pair<char> inverted = PairTests.BasicCharPair.Invert();

            // Assert
            Assert.Equal(PairTests.BasicCharPair.Item2, inverted.Item1);
            Assert.Equal(PairTests.BasicCharPair.Item1, inverted.Item2);
        }
    }
}