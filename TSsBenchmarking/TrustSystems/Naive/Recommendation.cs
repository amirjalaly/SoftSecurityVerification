using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Naive
{
    class RecommendationFunction:
        IRecommendationFunction<History,int>
    {
        public int GetRecommendation(History History)
        {
            if (History == null)
                return 0;
            return History.Sat- History.Unsat;
        }

        public int BestRecommend
        {
            get { return 100 ; }
        }

        public int WorstRecommend
        {
            get { return -100; }
        }
    }

}
