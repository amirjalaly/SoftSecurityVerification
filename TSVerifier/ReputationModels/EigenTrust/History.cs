using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.EigenTrust
{
    class History : IHistory
    {
        public SortedList<IEntity, int> S = new SortedList<IEntity, int>();
        public override void Update<History>(IEntity Client, QoS q, int epoch, State<History> State)
        {
            if (!S.ContainsKey(Client))
                S[Client] = 0;
            S[Client] += (int)q;
        }

        public override IHistory Clone()
        {
            return new History()
            {
                S = new SortedList<IEntity, int>(S),
                Owner = Owner
            };
        }

        public override string Index
        {
            get
            {
                var s = "";
                foreach (var key in S)
                {
                    s += key.Key + ":" + key.Value + ",";
                }
                return s;
            }
        }

        public override void Reset()
        {
            S = new SortedList<IEntity, int>();
        }
    }
}
