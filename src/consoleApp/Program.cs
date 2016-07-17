using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FixturesGenerator.Base;
using FixturesGenerator.Pairs;
using FixturesGenerator.League;

namespace ConsoleApplication
{
    public class Program
    {
        private const int DefaultNumElements = 4;

        public static void Main(string[] args)
        {
            int numElements = DefaultNumElements;
            if (args.Length > 0) int.TryParse(args[0], out numElements);

            try
            {
                Console.WriteLine();
                Console.WriteLine("Generate pairs for {0} elements", numElements);
                PrintResults(GeneratePairsAsync(numElements)
                    .GetAwaiter()
                    .GetResult());
                
                Console.WriteLine();
                Console.WriteLine("Generate league fixtures for {0} elements", numElements);
                PrintResults(GenerateLeagueFixturesAsync(numElements)
                    .GetAwaiter()
                    .GetResult());
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("The operation was cancelled.");
            }
            catch
            {
                throw;
            }
        }

        private static void PrintResults(FixtureSets<char> sets)
        {
            Console.WriteLine();
            if (sets.Count > 0)
            {
                Console.WriteLine("Solution found is:");
                foreach (var fixtureSet in sets)
                {
                    Console.WriteLine("[ {0} ]", string.Join(", ", fixtureSet));
                }
            }
            else
            {
                Console.WriteLine("Could not find a valid solution");
            }
        }

        private static async Task<FixtureSets<char>> GeneratePairsAsync(int numElements)
        {
            // define elements
            IEnumerable<char> elements = Enumerable.Range('A', numElements).Select(x => (char)x);

            // create logger
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Debug);
            var logger = loggerFactory.CreateLogger<PairsGenerator<char>>();

            // create generator
            var pairsGenerator = new PairsGenerator<char>(logger);
            pairsGenerator.Configure(new PairsGeneratorSettings(shuffleItems: false, includeInvertedPairs: true));
            
            // get combinations
            return await Task.Run(() => 
                pairsGenerator.GetFixtures(elements, CancellationToken.None));
        }

        private static async Task<FixtureSets<char>> GenerateLeagueFixturesAsync(int numElements)
        {
            // define elements
            IEnumerable<char> elements = Enumerable.Range('A', numElements).Select(x => (char)x);

            // create logger
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Debug);
            var logger = loggerFactory.CreateLogger<LeagueFixturesGenerator<char>>();

            // create generator
            var fixturesGenerator = new LeagueFixturesGenerator<char>(logger);
            
            // get combinations
            return await Task.Run(() => 
                fixturesGenerator.GetFixtures(elements, CancellationToken.None));
        }
    }
}
