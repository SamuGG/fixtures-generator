using System.Collections.Generic;

namespace FixturesGenerator.Base
{
    public class FixtureSet<T> : List<Pair<T>>
    {
        public FixtureSet()
        {
        }

        public FixtureSet(int count) : base(count)
        {
        }

        public FixtureSet(IEnumerable<Pair<T>> elements) : base(elements)
        {
        }

        public IEnumerable<Pair<T>> GetInvertedPairs()
        {
            foreach(var pair in this)
            {
                yield return pair.Invert();
            }
        }
    }
}