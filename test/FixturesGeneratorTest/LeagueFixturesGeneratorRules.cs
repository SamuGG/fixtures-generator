using System.Collections.Generic;
using FixturesGenerator.Tests.Helpers;

namespace FixturesGenerator.Tests.Rules
{
    internal static class LeagueFixturesGeneratorRules
    {
        public static bool ThereAreNoInvertedItemsInSameHalf(int[,,] resultMatrix)
        {
            int rowCount = resultMatrix.GetLength(0);
            int rowCountHalf = rowCount >> 1;
            int colCount = resultMatrix.GetLength(1);
            int expectedNumPairs = rowCountHalf * colCount;
            HashSet<int> hashes = new HashSet<int>();

            // For the 1st half: gather the hash of each inverted pair of elements
            for (int i = 0; i < rowCountHalf; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    int hashCode = HashHelpers.Combine(resultMatrix[i, j, 1], resultMatrix[i, j, 0]);
                    hashes.Add(hashCode);
                }
            }
            if (hashes.Count < expectedNumPairs) return false;

            // For the 2nd half: the hash of each pair should be found
            for (int i = rowCountHalf; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    int hashCode = HashHelpers.Combine(resultMatrix[i, j, 0], resultMatrix[i, j, 1]);
                    hashes.Remove(hashCode);
                }
            }
            return hashes.Count == 0;
        }

        public static bool ThereIsNoElementRepeatingPositionMoreThanTwiceInConsecutiveRows(int[,,] resultMatrix)
        {
            int colCount = resultMatrix.GetLength(1);
            int lastRowIndex = resultMatrix.GetLength(0) - 1;

            for (int i = 0; i < colCount; i++)
            {
                for (int j = 1; j < lastRowIndex; j++)
                {
                    // Check if the element at the current column is the same
                    // in the row above and the row below.
                    if (resultMatrix[j, i, 0] == resultMatrix[j - 1, i, 0] &&
                        resultMatrix[j, i, 0] == resultMatrix[j + 1, i, 0])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}