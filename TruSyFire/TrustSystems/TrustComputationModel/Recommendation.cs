using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruSyFire.TrustSystems.TrustComputationModel
{
    public interface IRecommendationFunction<History, Recommendation>
        where History : IHistory
    {
        Recommendation GetRecommendation(History History);
        Recommendation BestRecommend { get; }
        Recommendation WorstRecommend { get; }
    }
}
