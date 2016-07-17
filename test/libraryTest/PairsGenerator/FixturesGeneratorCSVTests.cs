using FixturesGenerator.Base;
using FixturesGenerator.Tests;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace FixturesGenerator.Pairs.Tests
{
    public class FixturesGeneratorCSVTests : IClassFixture<PairsGeneratorOfStringFixture>
    {
        private const string CSVFilesPath = "csv";

        private PairsGeneratorOfStringFixture _pairsGenerator;

        private PairsGenerator<string> Generator
        {
            get { return _pairsGenerator.Generator; }
        }

        public FixturesGeneratorCSVTests(PairsGeneratorOfStringFixture pairsGenerator)
        {
            _pairsGenerator = pairsGenerator;
        }

        [Theory]
        [InlineData("PairsGenerator2.csv")]
        [InlineData("PairsGenerator4.csv")]
        [InlineData("PairsGenerator6.csv")]
        [InlineData("PairsGenerator8.csv")]
        [InlineData("PairsGenerator10.csv")]
        [InlineData("PairsGenerator12.csv")]
        [InlineData("PairsGenerator14.csv")]
        [InlineData("PairsGenerator16.csv")]
        [InlineData("PairsGenerator18.csv")]
        [InlineData("PairsGenerator20.csv")]
        public void RunFixture(string csvFilename)
        {
            // Arrange
            CSVFileStructure<string> parsedFixture = ParseFixtureFile<string>(csvFilename);

            // Act
            var output = Generator.GetFixtures(parsedFixture.Elements, CancellationToken.None);

            // Assert
            Assert.Equal(parsedFixture.FixturesList.Count, output.Count);
            AssertSetsMatch(output, parsedFixture.FixturesList);
        }

        [Theory]
        [InlineData("PairsGenerator4Inverted.csv", true)]
        public void RunFixtureWithOptions(string csvFilename, bool includeInvertedPairs)
        {
            // Arrange
            CSVFileStructure<string> parsedFixture = ParseFixtureFile<string>(csvFilename);
            Generator.Configure(new PairsGeneratorSettings(false, true));

            // Act
            var output = Generator.GetFixtures(parsedFixture.Elements, CancellationToken.None);

            // Assert
            Assert.Equal(parsedFixture.FixturesList.Count, output.Count);
            AssertSetsMatch(output, parsedFixture.FixturesList);
        }

        private static CSVFileStructure<T> ParseFixtureFile<T>(string filename)
        {
            CSVFileStructure<T> fileStructure = new CSVFileStructure<T>();

            FixtureSets<T> fixturesList = new FixtureSets<T>();
            using (StreamReader reader = new StreamReader(File.OpenRead(Path.Combine(CSVFilesPath, filename))))
            {
                // read elements (comma-separated)
                fileStructure.Elements = reader.ReadLine().Split(',').Cast<T>();
                
                // read expected sets of pairs
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    // read collection of pairs (separated with semi-colon)
                    string[] pairs = line.Split(';');
                    var pairSet = FixtureSets<T>.NewItem(pairs.Length);
                    foreach(string p in pairs)
                    {
                        // read pair components (comma-separated)
                        T[] pairElements = p.Split(',').Cast<T>().ToArray();
                        pairSet.Add(new Pair<T>(pairElements[0], pairElements[1]));
                    }
                    fixturesList.Add(pairSet);
                }
            }
            fileStructure.FixturesList = fixturesList;

            return fileStructure;
        }

        private static void AssertSetsMatch<T>(FixtureSets<T> output, FixtureSets<T> expectedOutput)
        {
            // Check that all pair sets match, in a way where order of sets or
            // their pairs doesn't matter, just assures that all sets exist & match.
            foreach (FixtureSet<T> expectedSet in expectedOutput)
            {
                // Explanation: Assert...
                // there's 1 set in output (and only 1) which...
                // all of its pairs...
                // can be found in the current expected set
                Assert.NotNull(
                    output.Single(outputSet => 
                        outputSet.All(outputPair => 
                            expectedSet.Any(expectedPair => 
                                expectedPair.Equals(outputPair))
                        )
                    )
                );
                // If a set in output is found with all the pairs 
                // in the current expected set then, both sets match.
            }
        }

        public void Dispose()
        {
            _pairsGenerator.Dispose();
        }
    }
}
