using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Regret
{
    public class TCM : ITCM<History, double, double>
    {
        const double zeta_a_b = 0.5;
        const double zeta_A_b = 0.5;


        public IRecommendationFunction<History, double> RecommendationFunction
        {
            get { return new RecommendationFunction(); }
        }

        public string Name
        {
            get { return "REGRET"; }
        }

        public double GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, TruSyFire.TrustSystems.Environment.IClient c,
            TruSyFire.Verification.State<History> CurrentState)
        {
            var a_b = RecommendationFunction.GetRecommendation(
                CurrentState.GetHistory(sp,c));

            int cnt = 0;
            double SumRec = 0;
            foreach (var other in CurrentState.Environment.Clients)
            {
                if (other != c && other != sp)
                {
                    var rec = other.GetRecommendation<RecommendationFunction, History, double>
                            (sp, CurrentState, new RecommendationFunction());
                    cnt++;
                    SumRec = rec;
                }
            }
            var A_b = SumRec==0?0:(SumRec / cnt);
            return a_b * zeta_a_b + A_b * zeta_A_b;
        }

        public double Quantitize(double t)
        {
            return t+1;
        }
    }
}
