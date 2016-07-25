using System.Collections.Generic;
using FixturesGenerator.Tests.Helpers;

namespace FixturesGenerator.Tests.Rules
{
    internal static class BasicFixturesGeneratorRules
    {
        public static bool ThereAreNoDuplicatedIndexes(int[,,] resultMatrix)
        {
            int rowCount = resultMatrix.GetLength(0);
            int colCount = resultMatrix.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if (resultMatrix[i, j, 0] == resultMatrix[i, j, 1]) return false;
                }
            }

            return true;
        }

        public static bool ThereAreNoDuplicatedItems(int[,,] resultMatrix)
        {
            int rowCount = resultMatrix.GetLength(0);
            int colCount = resultMatrix.GetLength(1);
            int expectedNumItems = rowCount * colCount;
            HashSet<int> hashes = new HashSet<int>();

            for (int i = 0; i < rowCount; i++)
            {
                for (int j= 0; j < colCount; j++)
                {
                    int hashCode = HashHelpers.Combine(resultMatrix[i, j, 0], resultMatrix[i, j, 1]);
                    hashes.Add(hashCode);
                    hashes.Add(hashCode);
                }
            }

            return hashes.Count == expectedNumItems;
        }
    }
}