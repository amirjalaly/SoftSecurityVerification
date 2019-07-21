using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TSsBenchmarking.TrustSystems.TRAVOS
{
    public class History : TruSyFire.TrustSystems.TrustComputationModel.IHistory
    {
        public List<QoS> Outcomes = new List<QoS>();
        public List<Recommendation[]> Recs = new List<Recommendation[]>();
        public override void Update<History>(TruSyFire.TrustSystems.TrustComputationModel.QoS q1,
            TruSyFire.Verification.State<History> State)
        {
            //if (q == QoS.NoService)
            //    return;
            Outcomes.Add(q1== QoS.Good?QoS.Good:QoS.Bad);
            Recommendation[] recs = new Recommendation[State.Environment.Clients.Length - 1];
            int i=0;
            foreach (var c in State.Environment.Clients)
            {
                if (c == Client || c == ServiceProvider)
                    continue;

                recs[i++] = c.GetRecommendation
                    <RecommendationFunction, 
                    TSsBenchmarking.TrustSystems.TRAVOS.History, 
                    Recommendation>
                    (ServiceProvider,
                    (State<TSsBenchmarking.TrustSystems.TRAVOS.History>)(object)State,
                    new RecommendationFunction());
            }
            Recs.Add(recs);
        }

        public override TruSyFire.TrustSystems.TrustComputationModel.IHistory 
            Clone()
        {
            return new History()
            {
                Client = Client,
                ServiceProvider = ServiceProvider,
                Outcomes = new List<QoS>(Outcomes),
                Recs = new List<Recommendation[]>(Recs)
            };
        }

        public override string Index
        {
            get
            {
                var str = "";
                for (int i = 0; i < Outcomes.Count; i++)
                {
                    str += Outcomes[i] + "{";
                    foreach (var r in Recs[i])
                        str += r + " ";
                    str += "}";
                }
                return str;
            }
        }

        public override void Reset()
        {
            Outcomes = new List<QoS>();
            Recs = new List<Recommendation[]>();
        }
    }


}
