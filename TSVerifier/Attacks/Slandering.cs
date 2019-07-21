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
    class Slandering<History>: IPlan<History>
       where History : IHistory
    {
        public int AttackersSlanderingNo { get; private set; }
        public int AttackersProvidingSrvNo { get; private set; }
        public QoS QualityOfService { get; private set; }
        IEntity[] SlanderingGoals { get; set; }

        public Slandering(int AttackersSlanderingNo,int AttackersProvidingSrvNo,
            QoS QualityOfService, IEntity[] SlanderingGoals)
        {
            this.AttackersSlanderingNo = AttackersSlanderingNo;
            this.AttackersProvidingSrvNo = AttackersProvidingSrvNo;
            this.QualityOfService = QualityOfService;
            this.SlanderingGoals = SlanderingGoals;
        }

        public override string Title
        {
            get { return "Slandering"; }
        }

        public override Action GetAction(State<History> State)
        {
            var acts = new AtomicAction[AttackersProvidingSrvNo+AttackersSlanderingNo];
            for (int i = 0; i < AttackersProvidingSrvNo; i++)
                acts[i] = new ServiceAction() { Quality = QualityOfService };
            for (int i = 0; i < AttackersSlanderingNo; i++)
                    acts[AttackersProvidingSrvNo+ i] 
                        = new UnfairRatingAction() 
                        { 
                            Object = 
                                SlanderingGoals
                                    .ElementAt(i*SlanderingGoals.Count()/AttackersSlanderingNo) };
            return new Action(acts);
        }
    }

}
