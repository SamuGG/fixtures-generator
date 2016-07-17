namespace FixturesGenerator.Base
{
    public interface IIFixturesGeneratorValidationContext<T>
    {
        FixtureSets<T> PartialSolution { get; }
        FixtureSet<T> CurrentSet { get; }
    }
}