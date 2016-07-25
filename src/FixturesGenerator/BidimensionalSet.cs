namespace FixturesGenerator
{
    public struct BidimensionalSet
    {
        public int[,] Array;
        public int LowerBound;
        public int UpperBound;

        public BidimensionalSet(int[,] array)
        {
            Array = array;
            LowerBound = -1;
            UpperBound = Array.GetLength(0) - 1;
        }
    }
}