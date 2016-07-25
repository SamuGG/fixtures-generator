using System;
using System.Diagnostics;
using FixturesGenerator.Generators;

namespace ConsoleApplication
{
    public class Program
    {
        private const byte DefaultNumElements = 2;

        public static void Main(string[] args)
        {
            byte numElements = DefaultNumElements;
            if (args.Length > 0) byte.TryParse(args[0], out numElements);
            Console.WriteLine($"Generating fixtures for {numElements} elements...");
            
            var stopWatch = new Stopwatch();
            var generator = new LeagueFixturesGenerator();
            stopWatch.Start();
            int[,,] fixtures = generator.GenerateFixtures(numElements);
            stopWatch.Stop();
            
            if (fixtures != null)
            {
                int rowLength = fixtures.GetLength(0);
                int colLength = fixtures.GetLength(1);

                for (int row = 0; row < rowLength; row++)
                {
                    for (int col = 0; col < colLength; col++)
                    {
                        int printableElementIndex1 = fixtures[row, col, 0] + 1;
                        int printableElementIndex2 = fixtures[row, col, 1] + 1;
                        Console.Write($"{printableElementIndex1},{printableElementIndex2} ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine($"Solution generated in {stopWatch.Elapsed}");
            }
            else
            {
                Console.WriteLine("Could not find a solution.");
            }
        }
    }
}
