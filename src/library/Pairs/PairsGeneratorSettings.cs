namespace FixturesGenerator.Pairs
{
    public struct PairsGeneratorSettings
    {
        public bool ShuffleItems;
        public bool IncludeInvertedPairs;

        public PairsGeneratorSettings(bool shuffleItems, bool includeInvertedPairs)
        {
            ShuffleItems = shuffleItems;
            IncludeInvertedPairs = includeInvertedPairs;
        }
    }
}
