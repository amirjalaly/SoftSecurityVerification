using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Blind
{
    public class TCM:ITCM<History,double,double>
    {
        RecommendationFunction RcF = new RecommendationFunction();
        public IRecommendationFunction<History, double> RecommendationFunction
        {
            get { return RcF; }
        }

        public string Name
        {
            get { return "Blind" ; }
        }

        public double GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, 
            TruSyFire.TrustSystems.Environment.IClient c, 
            TruSyFire.Verification.State<History> CurrentState)
        {
            return 1;
        }
        public double Quantitize(double t)
        {

            return t;
        }
    }
 }
