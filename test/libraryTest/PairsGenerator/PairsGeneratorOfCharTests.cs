using FixturesGenerator.Tests;
using System;
using System.Threading;
using Xunit;

namespace FixturesGenerator.Pairs.Tests
{
    public class PairsGeneratorOfCharTests : IClassFixture<PairsGeneratorOfCharFixture>
    {
        private PairsGeneratorOfCharFixture _pairsGenerator;
        
        private PairsGenerator<char> Generator
        {
            get { return _pairsGenerator.Generator; }
        }

        public PairsGeneratorOfCharTests(PairsGeneratorOfCharFixture pairsGenerator)
        {
            _pairsGenerator = pairsGenerator;
        }

        [Theory]
        [InlineData(2, false)]
        [InlineData(3, true)]
        private void ThrowsExceptionWithOddNumberOfElements(int numElements, bool exceptionExpected)
        {
            // Arrange
            var elements = FixtureHelpers.GetCharCollection(numElements);

            // Act
            var exception = Record.Exception(() =>
                Generator.GetFixtures(elements, CancellationToken.None));
            
            // Assert
            if (exceptionExpected)
            {
                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
            }
            else
            {
                Assert.Null(exception);
            }
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 1)]
        private void ReturnsEmptyWithLessThan2Elements(int numElements, int expectedCount)
        {
            // Arrange
            var elements = FixtureHelpers.GetCharCollection(numElements);
            
            // Act
            var output = Generator.GetFixtures(elements, CancellationToken.None);

            // Assert
            Assert.Equal(expectedCount, output.Count);
        }

        [Fact]
        public void ReturnsEmptyWhenCancelled()
        {
            // Arrange
            var elements = FixtureHelpers.GetCharCollection(2);
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            var output = Generator.GetFixtures(elements, cancellationTokenSource.Token);

            // Assert
            Assert.Empty(output);
        }

        public void Dispose()
        {
            _pairsGenerator.Dispose();
        }
    }
}
