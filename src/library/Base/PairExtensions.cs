namespace FixturesGenerator.Base
{
    public static class PairExtensions
    {
        public static bool IsRelatedTo<T>(this Pair<T> pair1, Pair<T> pair2) => 
            pair1.Item1.Equals(pair2.Item1) ||
            pair1.Item1.Equals(pair2.Item2) ||
            pair1.Item2.Equals(pair2.Item1) ||
            pair1.Item2.Equals(pair2.Item2);

        public static Pair<T> Invert<T>(this Pair<T> item) =>
            new Pair<T>(item.Item2, item.Item1);
    }
}
