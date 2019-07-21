using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Abdul_Rahman
{
    public class RecommendationFunction
        : IRecommendationFunction<History, Trust>
    {
        public Trust GetRecommendation(History History)
        {
            if (History.S[0] > History.S[3])
                return Trust.vb;
            if (History.S[0] < History.S[3])
                return Trust.vg;
            return Trust.uncertaintly;
        }

        public Trust BestRecommend
        {
            get { return Trust.vg; }
        }

        public Trust WorstRecommend
        {
            get { return Trust.vb; }
        }
    }
}
