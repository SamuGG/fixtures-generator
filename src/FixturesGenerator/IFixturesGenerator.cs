namespace FixturesGenerator.Generators
{
    public interface IFixturesGenerator
    {
        int[,,] GenerateFixtures(byte numElements);
    }
}