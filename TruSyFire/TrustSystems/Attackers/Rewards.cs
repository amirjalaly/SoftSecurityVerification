using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.Attackers
{
    public class Rewards
    {
        public Rewards(double R_req, double alpha_slnd, double c_req, double c_id,
            double c_srv_good, double c_srv_bad,
            double r_srv_good, double r_srv_bad)
        {
            R_Req = R_req;
            Alpha_Slnd = alpha_slnd;
            C_Req = c_req;
            C_ID = c_id;
            C_Srv_Good = c_srv_good;
            C_Srv_Bad = c_srv_bad;
            R_Srv_Good = r_srv_good;
            R_Srv_Bad = r_srv_bad;
        }
        double R_Req { get;  set; }
        internal double Alpha_Slnd { get;  set; }

         double C_Req { get;  set; }
         double C_ID { get;  set; }
         double C_Srv_Good { get;  set; }
         double C_Srv_Bad { get;  set; }
         double R_Srv_Good { get;  set; }
         double R_Srv_Bad { get;  set; }
         public double GetNewIdentityCost()
         {
             return C_ID;
         }

         public double GetServiceProviderReward(QoS QualityOfExecutedService)
         {
             return R_Req - GetServiceCost(QualityOfExecutedService);
         }
         public double GetClientReward(QoS QualityOfRecievedService)
         {
             return GetServiceReward(QualityOfRecievedService) - 
                 C_Req;
         }
        double GetServiceReward(QoS q)
        {
            switch (q)
            {
                case QoS.Bad:
                    return R_Srv_Bad;
                case QoS.Good:
                    return R_Srv_Good;
                default:
                    return C_Req;
            }
        }

        double GetServiceCost(QoS q)
        {
            switch (q)
            {
                case QoS.Bad:
                    return C_Srv_Bad;
                case QoS.Good:
                    return C_Srv_Good;
                default:
                    return R_Req;
            }
        }

        public double GetSlanderingReward<History, TCM, Trust, TRec>
            (State<History> State,TrustSystem<History, TCM, Trust, TRec> System)
            where History : IHistory
            where TCM : ITCM<History, Trust, TRec>, new()
            where Trust : IComparable

        {
            var sumR = 0.0;
            foreach (var sp in System.Environment.HonestSP)
            {

                var meanm_sp = System.Environment.HonestClients
                    .Where(c=>c!=sp)
                    .Average(c => c.SelectServiceProvider(System.Environment.SP.Where(s=>s!=c).ToArray(),
                        System.TCM,
                        State)
                        .Where(dsc => dsc.Object == sp)
                        .Sum(dsc => dsc.Prob));
                sumR += (1 - meanm_sp) * Alpha_Slnd;
            }
            return sumR;
        }
    }
}
