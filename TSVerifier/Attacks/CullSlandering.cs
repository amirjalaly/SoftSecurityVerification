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
    class CollusionSlandering<History, Rep, RepM> : IPlan<History>
       where History : IHistory
        where RepM : IReputationModel<History, Rep>, new()
        where Rep : IComparable
    {
        public int AttackersSlanderingNo { get; private set; }
        public int AttackersProvidingSrvNo { get; private set; }
        public QoS QualityOfService { get; private set; }
        RepSystem<History,Rep,RepM> System { get; set; }

        public CollusionSlandering(int AttackersSlanderingNo, int AttackersProvidingSrvNo,
            QoS QualityOfService,
            RepSystem<History, Rep, RepM> System)
        {
            this.AttackersSlanderingNo = AttackersSlanderingNo;
            this.AttackersProvidingSrvNo = AttackersProvidingSrvNo;
            this.QualityOfService = QualityOfService;
            this.System = System;
        }

        public override string Title
        {
            get { return "Collusion-Slandering"; }
        }

        public override Action GetAction(State<History> State)
        {
            var acts = new AtomicAction[AttackersProvidingSrvNo+AttackersSlanderingNo];
            for (int i = 0; i < AttackersProvidingSrvNo; i++)
                acts[i] = new ServiceAction() { Quality = QualityOfService };
            var Sl = System.Decision.SelectServiceProvider(System.Environment.HonestEntities,State);
            var max = Sl.Max(e => e.Prob);
            var maxarg = Sl.FirstOrDefault(e=>e.Prob == max);
            for (int i = 0; i < AttackersSlanderingNo; i++)
                    acts[AttackersProvidingSrvNo+ i] 
                        = new UnfairRatingAction() 
                        { 
                            Object = 
                                maxarg.Object };
            return new Action(acts);
        }
    }

}
