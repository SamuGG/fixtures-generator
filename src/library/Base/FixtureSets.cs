using System;
using System.Collections.Generic;

namespace FixturesGenerator.Base
{
    public class FixtureSets<T> : List<FixtureSet<T>>
    {
        public FixtureSets() : base()
        {
        }

        public FixtureSets(int count) : base(count)
        {
        }

        public FixtureSets(IEnumerable<FixtureSet<T>> items): base(items)
        {
        }

        public static FixtureSet<T> NewItem()
        {
            return new FixtureSet<T>();
        }

        public static FixtureSet<T> NewItem(int initialCount)
        {
            return new FixtureSet<T>(initialCount);
        }

        public static FixtureSet<T> NewItem(IEnumerable<Pair<T>> items)
        {
            return new FixtureSet<T>(items);
        }

        // <summary>
        // Returns each set with its items' components inverted.
        // </summary>
        public IEnumerable<FixtureSet<T>> GetInvertedSets()
        {
            foreach (var fixtureSet in this)
            {
                yield return NewItem(fixtureSet.GetInvertedPairs());
            }
        }

        public void SwapPositions(int position1, int position2)
        {
            if (position1 < 0 || position1 >= this.Count) throw new ArgumentException(nameof(position1));
            if (position2 < 0 || position2 >= this.Count) throw new ArgumentException(nameof(position2));

            FixtureSet<T> temp = this[position2];
            this[position2] = this[position1];
            this[position1] = temp;
        }
    }
}
