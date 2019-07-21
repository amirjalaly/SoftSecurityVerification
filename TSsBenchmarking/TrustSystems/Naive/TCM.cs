using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Naive
{
    class TCM : ITCM<History,  double,int>
    {
        RecommendationFunction RcF = new RecommendationFunction();
        public IRecommendationFunction<History, int> RecommendationFunction
        {
            get { return RcF; }
        }

        public string Name
        {
            get { return "Naive" ; }
        }

        public double GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, 
            TruSyFire.TrustSystems.Environment.IClient c, 
            TruSyFire.Verification.State<History> CurrentState)
        {
            double t = 0;
            int cnt=0;
            foreach (var h in CurrentState.GetAllHistories())
            {
                if (h.ServiceProvider == h.Client)
                    continue;
                t += RecommendationFunction.GetRecommendation(h);
                cnt++;
                
            }

            return t/cnt;
        }

        public double Quantitize(double t)
        {

            return t;
        }
    }
 }
