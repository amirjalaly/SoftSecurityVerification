using System;
using System.Collections.Generic;
using System.Linq;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.Verification;

namespace RepSyFire.ReputationSystems.Environment.DecisionMaking.Instances
{
    public class MaximalDecision<History, Reputation, ReputationModel> : IDecision<ReputationModel, Reputation, History>
        where History : IHistory
        where ReputationModel : IReputationModel<History, Reputation>, new()
        where Reputation : IComparable
    {

        ReputationModel RepModel = new ReputationModel();
        public Distribution<IEntity>[] SelectServiceProvider(IEntity[] Entities, State<History> State)
        {
            Entities = Entities.Where(e => !(e is Attacker) || !(e as Attacker).IgnoreInProviderSelection).ToArray();
            var Reps = Entities.Select(e => new KeyValuePair<IEntity, Reputation>(e, RepModel.GetReputation(e, State))).ToArray();
            var maxRep = Reps.Max(kv => kv.Value);
            var maxies = Reps.Where(kv => kv.Value.CompareTo(maxRep) == 0);
            return maxies.Select(kv => new Distribution<IEntity>() { Object = kv.Key, Prob = 1.0 / maxies.Count() }).ToArray();

        }

        public string Name
        {
            get { return "Maximal"; }
        }
    }
}
