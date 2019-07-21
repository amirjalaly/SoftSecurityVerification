using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TruSyFire.TrustSystems.Environment
{

    public abstract class IEntity : IComparable
    {
        public IEntity(string Name)
        {
            this.Name = Name;
        }
        public string Name { get; protected set; }

        public static bool operator ==(IEntity p1, IEntity p2)
        {
            if ((object)p1 == null && (object)p2 == null)
                return true;
            return (object)p1 != null && (object)p2 != null && (p1.Name == p2.Name);
        }
        public static bool operator !=(IEntity p1, IEntity p2)
        {
            return !(p1 == p2);
        }
        public override string ToString()
        {
            return Name;
        }


        public int CompareTo(object obj)
        {
            return Name.CompareTo((obj as IEntity).Name);
        }
        public abstract string EntityType
        { get; }
    }
    public class Distribution<T>
    {
        public T Object { get; set; }
        public double Prob { get; set; }
    }

}
