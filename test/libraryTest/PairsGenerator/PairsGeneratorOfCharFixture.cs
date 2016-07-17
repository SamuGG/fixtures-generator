using System;

namespace FixturesGenerator.Pairs.Tests
{
    public class PairsGeneratorOfCharFixture : IDisposable
    {
        public PairsGenerator<char> Generator { get; private set; }

        public PairsGeneratorOfCharFixture()
        {
            Generator = new PairsGenerator<char>();
        }

        public void Dispose()
        {
        }
    }
}
