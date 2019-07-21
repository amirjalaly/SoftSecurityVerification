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
    class Promoting
        <History>: IPlan<History>
       where History : IHistory
    {
        public int AttackersPromotingNo { get; private set; }
        public int AttackersProvidingSrvNo { get; private set; }
        public QoS QualityOfService { get; private set; }
        IEntity[] PromotingGoals { get; set; }

        public Promoting(int AttackersPromotingNo, int AttackersProvidingSrvNo,
            QoS QualityOfService, IEntity[] PromotingGoals)
        {
            this.AttackersPromotingNo = AttackersPromotingNo;
            this.AttackersProvidingSrvNo = AttackersProvidingSrvNo;
            this.QualityOfService = QualityOfService;
            this.PromotingGoals = PromotingGoals;
        }

        public override string Title
        {
            get { return "Promoting"; }
        }

        public override Action GetAction(State<History> State)
        {
            var acts = new AtomicAction[AttackersProvidingSrvNo + AttackersPromotingNo];
            for (int i = 0; i < AttackersProvidingSrvNo; i++)
                acts[i] = new ServiceAction() { Quality = QualityOfService };
            for (int i = 0; i < AttackersPromotingNo; i++)
                    acts[AttackersProvidingSrvNo+ i] 
                        = new PromotingAction() 
                        { 
                            Object =
                                PromotingGoals
                                    .ElementAt(i * PromotingGoals.Count() / AttackersPromotingNo)
                        };
            return new Action(acts);
        }
    }

}
