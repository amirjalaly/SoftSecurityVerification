using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Blind
{
    public class RecommendationFunction:
        IRecommendationFunction<History, double>
    {
        public double GetRecommendation(History History)
        {
            return (History.Sat+1.0)/(History.Sat+History.Unsat+2.0);
        }

        public double BestRecommend
        {
            get { return 1; }
        }

        public double WorstRecommend
        {
            get { return -1; }
        }
    }

}
