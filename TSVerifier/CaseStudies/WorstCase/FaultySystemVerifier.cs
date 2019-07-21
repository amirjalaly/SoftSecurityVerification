using RepSyFire.ReputationSystems.Environment.DecisionMaking.Instances;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using RSVerifier.CaseStudies.System;
using RSVerifier.CaseStudies.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.CaseStudies.WorstCase
{
    class FaultySystemVerifier<History,Reputation,ReputationModel> : RSVerifier<
        History, Reputation, ReputationModel>
        where History : IHistory,new()
        where ReputationModel : IReputationModel<History, Reputation>, new()
        where Reputation : IComparable
    {
        public override string Title
        {
            get
            {
                return "Faulty-System-Study["+FaultRatio+"]";
            }
        }
        public double FaultRatio { get; private set; }

        public FaultySystemVerifier(int H, int A, int Cs, int Max, double faultratio)
            : base(new FinancialSystem<History, Reputation, ReputationModel>
                (new FaultyEnvironment(H, A, Cs,faultratio),
                new MaximalDecision<History, Reputation, ReputationModel>(),
                new ReputationModel(),
                Max))
        {
            FaultRatio = faultratio;
        }

    }
}
