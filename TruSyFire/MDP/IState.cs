using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruSyFire.MDP
{
    public abstract class IState : IComparable ,IEquatable<IState>
    {
        public int Epoch { get; protected set; }
        public abstract string Index { get; }
        public int CompareTo(object obj)
        {
            return Index.CompareTo(((IState)obj).Index);
        }
        public override string ToString()
        {
            return Index;
        }


        public bool Equals(IState other)
        {
            return other.Index == Index;
        }
    }

    class StateComparator : IEqualityComparer<IState>
    {
        public bool Equals(IState x, IState y)
        {
            return x.Index == y.Index;
        }

        public int GetHashCode(IState obj)
        {
            return obj.GetHashCode();
        }
    }
}
