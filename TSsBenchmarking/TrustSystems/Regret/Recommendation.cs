using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Regret
{
    public class RecommendationFunction:
            IRecommendationFunction<History,double>
    {
        public double GetRecommendation(History History)
        {
            var sq = History.SequenceOfServices;
            var r = 0;
            for (int i = 0; i < sq.Length; i++)
                r += (sq[i] == '+' ? 1 : -1) * (i + 1);

            if (r == 0)
                return 0;
            return r / (sq.Length * (sq.Length + 1) / 2.0);
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
