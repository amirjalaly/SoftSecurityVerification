using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.SubjectiveLogic
{
    public class RecommendationFunction:
        IRecommendationFunction<History,Trust>
    {
        public Trust GetRecommendation(History History)
        {
            if (History == null)
                return new Trust(0, 0, 1);
            return new Trust(History.Sat, History.Unsat);
        }

        public Trust BestRecommend
        {
            get { return new Trust(1,0,0) ; }
        }

        public Trust WorstRecommend
        {
            get { return new Trust(0,1,0); }
        }
    }

}
