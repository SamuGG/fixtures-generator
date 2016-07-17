using System.Collections.Generic;
using System.Linq;
using FixturesGenerator.Base;

namespace FixturesGenerator.Tests
{
    public static class FixtureHelpers
    {
        public static IEnumerable<char> GetCharCollection(int count) => 
            Enumerable.Range('A', count).Select(x => (char)x);

        public static bool PairsAreInverted<T>(Pair<T> pair1, Pair<T> pair2)
        {
            return pair1.Invert().Equals(pair2);
        }

        public static bool SetsAreInverted<T>(IEnumerable<Pair<T>> collection1, IEnumerable<Pair<T>> collection2)
        {
            if (collection1.Count() != collection2.Count()) return false;
            return collection1.All(p1 => collection2.Any(p2 => PairsAreInverted(p1, p2)));
        }
    }
}
