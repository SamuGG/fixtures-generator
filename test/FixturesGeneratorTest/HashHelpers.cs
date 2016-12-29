namespace FixturesGenerator.Tests.Helpers
{
    // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Numerics/Hashing/HashHelpers.cs
    internal static class HashHelpers
    {
        public static int Combine(int h1, int h2)
        {
            uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }
    }
}