using System;
using System.Threading;
using FixturesGenerator.Generators;
using FixturesGenerator.Tests.Rules;
using Xunit;

namespace FixturesGenerator.Tests
{
    public class BasicFixturesGeneratorTests
    {
        private BasicFixturesGenerator m_Generator;

        public BasicFixturesGeneratorTests()
        {
            m_Generator = new BasicFixturesGenerator();
        }

        ~BasicFixturesGeneratorTests()
        {
            m_Generator = null;
        }

        [Fact]
        public void GenerateFixturesFailsWithNumElementsLessThanMinimum()
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => m_Generator.GenerateFixtures(1));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void GenerateFixturesFailsWithNumElementsGreaterThanMaximum()
        {
            // Arrange
            byte numElements = (byte)(BasicFixturesGenerator.MaxNumElements + 1);

            // Act
            var exception = Assert.Throws<ArgumentException>(() => m_Generator.GenerateFixtures(numElements));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void GenerateFixturesFailsWithOddNumElements()
        {
            // Act
            var exception = Assert.Throws<ArgumentException>(() => m_Generator.GenerateFixtures(3));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void ReturnsNullWhenCancellationRequested()
        {
            // Arrange
            var cancellationToken = new CancellationToken(true);
            var generator = new BasicFixturesGenerator(cancellationToken);

            // Act
            var output = generator.GenerateFixtures(2);

            // Assert
            Assert.Null(output);
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
            Assert.True(BasicFixturesGeneratorRules.ThereAreNoDuplicatedItems(fixtures), $"Rule {nameof(BasicFixturesGeneratorRules.ThereAreNoDuplicatedIndexes)} failed.");
        }
    }
}
