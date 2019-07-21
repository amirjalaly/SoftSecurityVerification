
using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = RepSyFire.ReputationSystems.Attackers.Action;

namespace RSVerifier.Attacks
{
    class SimpleMalic<History> : IPlan<History>
       where History : IHistory
    {
        public int AttackersNo { get; private set; }

        public SimpleMalic(int AttackersNo)
        {
            this.AttackersNo = AttackersNo;
        }

        public override string Title
        {
            get { return "Simple-Malic"; }
        }

        public override Action GetAction(State<History> State)
        {
            var acts = new AtomicAction[AttackersNo];

                for (int i = 0; i < acts.Length; i++)
                    acts[i] = new ServiceAction() { Quality = QoS.Bad };
            return new Action(acts);
        }
    }
}
