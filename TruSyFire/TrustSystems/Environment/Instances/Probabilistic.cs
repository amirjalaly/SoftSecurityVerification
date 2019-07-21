using System;

namespace RepSyFire.ReputationSystems.Environment.DecisionMaking.Instances
{
    public class ProbabilisticDecision<History, Reputation, ReputationModel> : IDecision<ReputationModel, Reputation, History>
        where History : IHistory
        where ReputationModel : IReputationModel<History, Reputation>, new()
        where Reputation : IComparable
    {

        ReputationModel RepModel = new ReputationModel();
        public Distribution<IEntity>[] SelectServiceProvider(IEntity[] Entities, State<History> State)
        {
            Entities = Entities.Where(e => !(e is Attacker) || !(e as Attacker).IgnoreInProviderSelection).ToArray();

        }

        public string Name
        {
            get { return "Probablistic"; }
        }
    }
}
