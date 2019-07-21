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
    class CollusionPromoting<History, Rep, RepM> : IPlan<History>
        where History : IHistory
        where RepM : IReputationModel<History, Rep>, new()
        where Rep : IComparable
    {
        public int AttackersPromotingNo { get; private set; }
        public int AttackersProvidingSrvNo { get; private set; }
        public QoS QualityOfService { get; private set; }
        RepSystem<History, Rep, RepM> System { get; set; }

        public CollusionPromoting(int AttackersPromotingNo, int AttackersProvidingSrvNo,
            QoS QualityOfService,
            RepSystem<History, Rep, RepM> System)
        {
            this.AttackersPromotingNo = AttackersPromotingNo;
            this.AttackersProvidingSrvNo = AttackersProvidingSrvNo;
            this.QualityOfService = QualityOfService;
            this.System = System;
        }

        public override string Title
        {
            get { return "Collusion-Promoting"; }
        }

        public override Action GetAction(State<History> State)
        {
            var acts = new AtomicAction[AttackersProvidingSrvNo + AttackersPromotingNo];
            for (int i = 0; i < AttackersProvidingSrvNo; i++)
                acts[i] = new ServiceAction() { Quality = QualityOfService };
            var Sl = System.Decision.SelectServiceProvider(System.Environment.Attackers, State);

            var max = Sl.Min(e => e.Prob);
            var maxarg = Sl.FirstOrDefault(e => e.Prob == max); 

            for (int i = 0; i < AttackersPromotingNo; i++)
                    acts[AttackersProvidingSrvNo+ i] 
                        = new PromotingAction() 
                        { 
                            Object =maxarg.Object
                        };
            return new Action(acts);
        }
    }

}
