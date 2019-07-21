using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Environment.DecisionMaking;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = RepSyFire.ReputationSystems.Attackers.Action;

namespace RSVerifier.Attacks
{
    class Oscillation<History,Rep,RepModel>: IPlan<History>
        where History : IHistory
        where RepModel : IReputationModel<History, Rep>, new()
        where Rep : IComparable
    {
        IDecision<RepModel, Rep, History> Decision;
        public Attacker[] Attackers { get; private set; }
        public Oscillation(Attacker[] Attackers,
            IDecision<RepModel, Rep, History> Decision)
        {
            this.Attackers = Attackers;
            this.Decision = Decision;
        }

        public override string Title
        {
            get { return "Oscillation"; }
        }
        const double eps = .2;
        public override Action GetAction(State<History> State)
        {
            var acts = new AtomicAction[Attackers.Count()];
            var SL = Decision.SelectServiceProvider(State.GetAllHistories().Select(e=>e.Owner).ToArray(), State);
            double sum1 = 0.0,sum2=0 ;
            for (int i = 0; i < acts.Length; i++)
            {
                var p = SL.Where(e => e.Object == Attackers[i]).Sum(e => e.Prob);
                if (i < acts.Length / 2)
                    sum1 += p;
                else
                    sum2 += p;
            }
            for (int i = 0; i < acts.Length; i++)
            {
                if (i < acts.Length / 2)
                    acts[i] = sum1>eps && sum1>sum2 ?
                        (AtomicAction)new ServiceAction() { Quality = QoS.Bad } :
                        //new PromotingAction() { Object = Attackers[i+acts.Length/2]};
                        new ServiceAction() { Quality = QoS.Good };
                else
                    acts[i] = sum2> eps && sum1 < sum2 ?
                        (AtomicAction)new ServiceAction() { Quality = QoS.Bad } :
                        //new PromotingAction() { Object = Attackers[i - acts.Length / 2] };
                        new ServiceAction() { Quality = QoS.Good };
            }    
            return new Action(acts);
        }
    }
}
