using FixturesGenerator.Base;
using System.Collections.Generic;

namespace FixturesGenerator.Tests
{
    public struct CSVFileStructure<T>
    {
        public IEnumerable<T> Elements;
        public FixtureSets<T> FixturesList;
    }
}
