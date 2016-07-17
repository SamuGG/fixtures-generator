using System.Collections.Generic;
using System.Threading;

namespace FixturesGenerator.Base
{
    public interface IFixturesGenerator<T>
    {
        FixtureSets<T> GetFixtures(IEnumerable<T> elements, CancellationToken cancellationToken);
    }
}