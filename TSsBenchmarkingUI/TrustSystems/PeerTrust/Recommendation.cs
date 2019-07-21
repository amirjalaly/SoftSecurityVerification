using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.PeerTrust
{
    public class RecommendationFunction :
            IRecommendationFunction<History, double>
    {
        public double GetRecommendation(History History)
        {
            if (History.TotalTransaction == 0)
                return 0.5;
            return History.Sat / ((double)(History.TotalTransaction));
        }

        public double BestRecommend
        {
            get { return 1; }
        }

        public double WorstRecommend
        {
            get { return 0; }
        }
    }
}
