using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.TRAVOS
{
    public class RecommendationFunction :
        IRecommendationFunction<History, Recommendation>
    {
        public Recommendation GetRecommendation(History History)
        {
            byte s=0,r=0;
            foreach (var q in History.Outcomes)
                if (q == QoS.Good)
                    s++;
                else
                    r++;
            return new Recommendation()
            {
                Sat = s,
                Unsat = r
            };
        }

        public Recommendation BestRecommend
        {
            get { return new Recommendation() { Sat = 100, Unsat = 0 }; }
        }

        public Recommendation WorstRecommend
        {
            get { return new Recommendation() { Sat = 0, Unsat = 100 }; }
        }
    }

    public class Recommendation
    {
        public byte Sat { get; set; }
        public byte Unsat { get; set; }
        public double Mean { get { return (Sat + 1.0) / (Sat + Unsat + 2.0); } }
        public double Std
        {
            get
            {
                return Math.Sqrt((Sat + 1) * (Unsat + 1) /
                    (Math.Pow(Sat + Unsat + 2, 2) * (Sat + Unsat + 3)));
            }
        }
        public override string ToString()
        {
            return Sat.ToString()+","+Unsat;
        }
    }
}
