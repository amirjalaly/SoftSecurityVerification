using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.Sporas
{
    class History : IHistory
    {
        public double R { get; private set; }
        public const int D = 10;
        public const int Teta = 2;
        public const int sigma = 1;
        public override void Update<History>(IEntity Client, QoS q, int epoch, State<History> State)
        {
            var w = q == QoS.Good ? 1 : 0.1;
            var E_W = R / D;
            var rep = new ReputationModel();
            var R_other = rep.GetReputation(Client, State as State<Sporas.History>);
            var phi = 1 - (1.0 / (1 + Math.Exp((D - R) / sigma)));
            R += (phi / Teta) * R_other * (w - E_W);
        }

        public override IHistory Clone()
        {
            return new History()
            {
                Owner = Owner,
                R = R
            };
        }

        public override string Index
        {
            get { return R.ToString(); }
        }

        public override void Reset()
        {
            R = 0.1;
        }
    }
}
