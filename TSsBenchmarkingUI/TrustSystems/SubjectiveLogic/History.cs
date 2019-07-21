using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSsBenchmarking.TrustSystems.SubjectiveLogic
{
    public class History:TruSyFire.TrustSystems.TrustComputationModel.IHistory
    {
        public int Sat { get; set; }
        public int Unsat { get; set; }
        public override void Update<History>(TruSyFire.TrustSystems.TrustComputationModel.QoS q, TruSyFire.Verification.State<History> State)
        {
            switch (q)
            {
                case TruSyFire.TrustSystems.TrustComputationModel.QoS.Good:
                    Sat++;
                    break;
                default:
                    Unsat++;
                    break;
            }
        }

        public override TruSyFire.TrustSystems.TrustComputationModel.IHistory Clone()
        {
            return new History()
            {
                Client = Client,
                ServiceProvider = ServiceProvider,
                Sat = Sat,
                Unsat = Unsat
            };
        }

        public override string Index
        {
            get { return string.Format("({0},{1})",Sat,Unsat); }
        }

        public override void Reset()
        {
            Sat = Unsat = 0 ;
        }
    }
}
