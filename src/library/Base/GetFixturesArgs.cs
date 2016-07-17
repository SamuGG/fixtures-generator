using System.Collections.Generic;

namespace FixturesGenerator.Base
{
    internal struct GetFixturesArgs<T>
    {
        public IEnumerable<Pair<T>> PairCollection;
        public int PairCount;
        public int SetsCount;
    }
}
