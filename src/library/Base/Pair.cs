using System;
using System.Diagnostics;

namespace FixturesGenerator.Base
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Pair<T> : Tuple<T, T>
    {
        public Pair(T item1, T item2) : base(item1, item2)
        {
        }

        public bool Equals(Pair<T> obj)
        {
            if (obj == null) return false;
            return Item1.Equals(obj.Item1) && Item2.Equals(obj.Item2);
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<T> ? 
                Equals(obj as Pair<T>) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"({Item1}, {Item2})";
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return this.ToString(); }
        }
    }
}
