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
    class On_Off<History> : IPlan<History>
       where History : IHistory
    {
        public int AttackersNo { get; private set; }
        public int Step { get; private set; }
        public On_Off(int AttackersNo)
            : this(AttackersNo, 5) { }

        public On_Off(int AttackersNo, int Step)
        {
            this.AttackersNo = AttackersNo;
            this.Step = Step;
        }

        public override string Title
        {
            get { return "On-Off"; }
        }

        public override Action GetAction(State<History> State)
        {
            var acts = new AtomicAction[AttackersNo];
            if ((State.Epoch % (2 * Step)) < Step)
                for (int i = 0; i < acts.Length; i++)
                    acts[i] = new ServiceAction() { Quality = QoS.Good };
            else
                for (int i = 0; i < acts.Length; i++)
                    acts[i] = new ServiceAction() { Quality = QoS.Bad };
            return new Action(acts);
        }
    }
}
