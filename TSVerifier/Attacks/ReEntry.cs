using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = RepSyFire.ReputationSystems.Attackers.Action;

namespace RSVerifier.Attacks
{
    class ReEntry<History, Reputation, ReputationModel> : IPlan<History>
        where History : IHistory, new()
        where ReputationModel : IReputationModel<History, Reputation>, new()
        where Reputation : IComparable
    {
        RepSystem<History, Reputation, ReputationModel> System;
        public ReEntry(RepSystem<History, Reputation, ReputationModel> Sys,double MinRepToRenEnter)
        {
            this.System = Sys;
            MinRep = MinRepToRenEnter;
        }
        public override string Title
        {
            get { return "ReEnter Attack"; }
        }
        double MinRep = .1;
        public override Action GetAction(State<History> State)
        {
            List<AtomicAction> acts = new List<AtomicAction>();
            var SL = System.Decision.SelectServiceProvider(System.Environment.Entities, State);
            foreach (var a in System.Environment.Attackers)
            {
                var hs = State.GetHistory(a);
                hs.Reset();
                if (State.GetHistory(a) == hs)
                {
                    acts.Add(new ServiceAction() { Quality = QoS.Bad });
                    continue;
                }

                var asl = SL.FirstOrDefault(e=>e.Object == a);
                if (asl == null || asl.Prob < MinRep)
                    acts.Add(new ReEntryAction());
                else
                    acts.Add(new ServiceAction() { Quality = QoS.Bad });
            }
            return new Action(acts.ToArray());
                
        }
    }
}
