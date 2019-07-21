using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.TRAVOS
{
    public class TCM : ITCM<History, double, Recommendation>
    {
        RecommendationFunction RcF = new RecommendationFunction();
        public IRecommendationFunction<History, Recommendation> RecommendationFunction
        {
            get { return RcF; }
        }

        public string Name
        {
            get { return "TRAVOS"; }
        }
        const double BinSize = 0.2;
        public double GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, TruSyFire.TrustSystems.Environment.IClient c, TruSyFire.Verification.State<History> CurrentState)
        {
            int i = 0;
            if (CurrentState.Index == "||Good{2,0  }Good{2,0  }|||Good{0,0  }Good{0,0  }||||"
                && c.Name == "H_0")
                i = i;

            var h = CurrentState.GetHistory(sp,c);
            double m = RcF.GetRecommendation(h).Sat;
            double n = RcF.GetRecommendation(h).Unsat;
            foreach (var other in CurrentState.Environment.Clients)
            {

                if ( other == c || other == sp)
                    continue;
                var r = other.GetRecommendation
                    <RecommendationFunction, History, Recommendation>
                    (sp, CurrentState, RcF);
                var bin = FindBin(r,BinSize);
                double rho;
                rho = ComputeAccuracy(h, i, bin,r);
                if (rho == 0)
                    continue;
                var E_ = .5 + rho*(r.Mean - 0.5);
                var std_ = .29 + rho*(r.Std - 0.29);
                var a = ((E_ * E_) - (E_ * E_ * E_)) / (std_ * std_) - E_;
                E_ = 1 - E_;
                var b = ((E_ * E_) - (E_ * E_ * E_)) / (std_ * std_) - E_;
                m += (a - 1);
                n += (b - 1);
                i++;
            }
            if (m < 0 || n < 0)
                m = m;
            return (m + 1) / (m + n + 2);

        }

        private double ComputeAccuracy(History h, int clientindex, double bin,Recommendation E_r)
        {
            if (E_r.Sat == 0 && E_r.Unsat == 0)
                return 0;
            double rho;
            byte a_o = 0, b_o = 0;
            for (int j = 0; j < h.Outcomes.Count; j++)
            {
                if (h.Recs[j][clientindex].Mean >= bin &&
                    h.Recs[j][clientindex].Mean < (bin + .2))
                {
                    if (h.Outcomes[j] == QoS.Good)
                        a_o++;
                    else
                        b_o++;
                }
            }
            var E_o = (a_o + 1.0) / (b_o + a_o + 2);
            if (E_o >= bin &&
                E_o < (bin + BinSize))
            {
                return 1-Math.Abs(E_o-E_r.Mean)/BinSize;
            }
            return 0;
        }
        double EstimateBeta(double b, double m)
        {
            return (m - b) * (1 + b / m) / m;
        }

        private static double FindBin(Recommendation r,double binsize)
        {
            var b = (int)(r.Mean /binsize) *binsize;
            return b;
        }

        public double Quantitize(double t)
        {
            return t ;
        }
    }
}
