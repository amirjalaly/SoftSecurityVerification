using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepSyFire.ReputationSystems.Environment.DecisionMaking.Instances
{
    public class RiskyDecisionMaking<History, Reputation, ReputationModel> : IDecision<ReputationModel, Reputation, History>
        where History : IHistory
        where ReputationModel : IReputationModel<History, Reputation>, new()
        where Reputation : IComparable
    {
        public double RiskFactor { get; private set; }
        public RiskyDecisionMaking(double riskfactor)
        {
            RiskFactor = riskfactor;
        }

        ReputationModel RepModel = new ReputationModel();
        //public const double Eps = 0.1;
        public Distribution<IEntity>[] SelectServiceProvider(IEntity[] Entities, State<History> State)
        {
            Entities = Entities.Where(e => !(e is Attacker) || !(e as Attacker).IgnoreInProviderSelection).ToArray();

            var allrep = Entities.Select(e => RepModel.GetReputation(e, State)).ToArray();
            var allw = allrep.Select(r => RepModel.GetWeight(r, allrep)).ToArray();
            var min  = allw.Min();
            var max = allw.Max();
            var interval = max==min?1:(max - min);
            var norms = allw.Select(w => ((w - min) / (interval))).ToArray();
            var sum = norms.Sum(n => Math.Exp(n / RiskFactor));
            


            return Entities.Select(e => new Distribution<IEntity>()
            {
                Object = e,
                Prob = 
                    Math.Exp(
                        (RepModel.GetWeight(RepModel.GetReputation(e, State), allrep)-min)/(interval*RiskFactor)
                        )
                    / sum
            }).ToArray();

        }

        public string Name
        {
            get { return "Risky("+RiskFactor+")"; }
        }
    }
}
