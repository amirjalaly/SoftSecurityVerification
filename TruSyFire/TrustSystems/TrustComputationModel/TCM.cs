using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.TrustComputationModel
{
    public interface ITCM<History,Trust,Recommendation>
        where History:IHistory
        where Trust : IComparable
    {
        IRecommendationFunction<History, Recommendation> RecommendationFunction
        { get; }
        string Name { get; }
        Trust GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp,
            IClient c, 
            State<History> CurrentState);
        double Quantitize(Trust t);
    }

}
