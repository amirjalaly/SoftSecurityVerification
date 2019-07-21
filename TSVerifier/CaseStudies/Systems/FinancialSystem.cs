using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Environment.DecisionMaking;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = RepSyFire.ReputationSystems.Attackers.Action;
using Environment = RepSyFire.ReputationSystems.Environment.Environment;

namespace RSVerifier.CaseStudies.System
{
    class FinancialSystem<History, Reputation, ReputationModel> : RepSystem<History, Reputation, ReputationModel>
        where History : IHistory
        where ReputationModel : IReputationModel<History, Reputation>, new()
        where Reputation : IComparable
    {
        public FinancialSystem
            (Environment env, IDecision<ReputationModel, Reputation, History> dec, ReputationModel rep, int maxepoch)
            : base(env, dec, rep, new FinancialReward(), maxepoch) { }
        public FinancialSystem
            (Environment env, IDecision<ReputationModel, Reputation, History> dec, ReputationModel rep, int maxepoch, Rewards Rewards)
            : base(env, dec, rep, Rewards, maxepoch) { }
        protected override IEnumerable<AtomicAction> AvailableAtomicActions
        {
            get
            {
                return base.AvailableAtomicActions.Where(a => a.Type != ActionType.ReEntry)
                    .Where(a => a.Type != ActionType.UnfairRating ||
                        (a as UnfairRatingAction).Object.Name.CompareTo("H" + (Environment.HonestEntities.Length / 2)) < 0)
                        //.Where(a => a.Type == ActionType.Service);

                      ;
            }
        }
    }

    class FinancialReward : Rewards
    {
        public FinancialReward()
            : this(0.3) { }
        public FinancialReward(double BadSrvCost)
            : base(1, 0, 1, double.PositiveInfinity, 0.9, BadSrvCost) { }
            //: base(1, 0, 1, double.PositiveInfinity, 1, 0) { }
    }
}
