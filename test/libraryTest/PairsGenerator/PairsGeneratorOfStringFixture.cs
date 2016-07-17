using System;

namespace FixturesGenerator.Pairs.Tests
{
    public class PairsGeneratorOfStringFixture : IDisposable
    {
        public PairsGenerator<string> Generator { get; private set; }

        public PairsGeneratorOfStringFixture()
        {
            Generator = new PairsGenerator<string>();
        }

        public void Dispose()
        {
        }
    }
}
