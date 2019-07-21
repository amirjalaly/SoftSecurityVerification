using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.Regret
{
    public class History : TruSyFire.TrustSystems.TrustComputationModel.IHistory
    {
        public string SequenceOfServices = "";
        public override void Update<History>(TruSyFire.TrustSystems.TrustComputationModel.QoS q, TruSyFire.Verification.State<History> State)
        {
            SequenceOfServices += (q == QoS.Good ? "+" : "-");
        }

        public override TruSyFire.TrustSystems.TrustComputationModel.IHistory Clone()
        {
            return new History()
            {
                Client = Client,
                ServiceProvider = ServiceProvider,
                SequenceOfServices = SequenceOfServices
            };
        }

        public override string Index
        {
            get { return SequenceOfServices; }
        }

        public override void Reset()
        {
            SequenceOfServices = "";
        }
    }
}
