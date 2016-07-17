namespace FixturesGenerator.Base
{
    internal struct FixturesGeneratorValidationContext<T> : IIFixturesGeneratorValidationContext<T>
    {
        public FixtureSets<T> PartialSolution { get; private set; }
        public FixtureSet<T> CurrentSet { get; private set; }

        public FixturesGeneratorValidationContext(
            FixtureSets<T> partialSolution,
            FixtureSet<T> currentSet)
        {
            PartialSolution = partialSolution;
            CurrentSet = currentSet;
        }
    }
}