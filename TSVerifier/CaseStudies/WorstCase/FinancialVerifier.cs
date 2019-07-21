using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Environment.DecisionMaking;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using RSVerifier.CaseStudies.System;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = RepSyFire.ReputationSystems.Attackers.Action;
using Environment = RepSyFire.ReputationSystems.Environment.Environment;
using System.Text;
using RepSyFire.ReputationSystems.Environment.DecisionMaking.Instances;

namespace RSVerifier.CaseStudies.WorstCase
{
    class FinancialVerifier<History,Reputation,ReputationModel> : RSVerifier<
        History, Reputation, ReputationModel>
        where History : IHistory,new()
        where ReputationModel : IReputationModel<History, Reputation>, new()
        where Reputation : IComparable
    {
        public override string Title
        {
            get
            {
                return "Financial System [BadRW "+System.Rewards.C_Srv_Bad+"]";
            }
        }
        public FinancialVerifier(int H, int A, int Cs, int Max,bool UseRiskyDecisionMaking,double Risk)
            : base(new FinancialSystem<History, Reputation, ReputationModel>
                (new Environment(H, A, Cs),
                UseRiskyDecisionMaking?
                    (IDecision<ReputationModel,Reputation,History>)
                        new RiskyDecisionMaking<History, Reputation, ReputationModel>(Risk) :
                    (IDecision<ReputationModel, Reputation, History>)
                        new MaximalDecision<History, Reputation, ReputationModel>(),
                new ReputationModel(),
                Max)) { }
        public FinancialVerifier(int H, int A, int Cs, int Max, Rewards Rewards,bool UseGreedyDecisioning)
            : base(new FinancialSystem<History, Reputation, ReputationModel>
                (new Environment(H, A, Cs),
                        UseGreedyDecisioning?
                    (IDecision<ReputationModel,Reputation,History>)
                        new MaximalDecision<History, Reputation, ReputationModel>():
                    (IDecision<ReputationModel, Reputation, History>)
                        new ProbabilisticDecision<History, Reputation, ReputationModel>(),
                new ReputationModel(),
                Max, Rewards)) { }
        public FinancialVerifier(int H, int A, int Cs, int Max)
            : this(H, A, Cs, Max, false, 0) { }

    }

}
