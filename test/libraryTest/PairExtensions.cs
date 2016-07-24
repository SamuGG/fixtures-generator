using System.Collections.Generic;
using System.Linq;
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
        public void UnrelatedItemsReturnsItemsNotRelated()
        {
            // Arrange
            var item1 = new Pair<char>('A', 'B');
            var item2 = new Pair<char>('C', 'D');
            var items = new List<Pair<char>>(new []{item1, item2});

            // Act
            var unrelatedItems = items.UnrelatedItems(item1);

            // Assert
            Assert.NotEmpty(unrelatedItems);
            Assert.Equal(item2, unrelatedItems.First());
            Assert.False(item1.IsRelatedTo(item2));
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