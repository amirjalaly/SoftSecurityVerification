using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.Core
{
    class History : IHistory
    {
        public string SequenceOfServices = "";
        public override void Update<History>(IEntity Client, QoS q, int epoch, State<History> State)
        {
            SequenceOfServices += (q == QoS.Good ? '+' : '-');
        }

        public override IHistory Clone()
        {
            return new History()
            {
                Owner = Owner,
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
