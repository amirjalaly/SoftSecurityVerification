using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.eBay
{
    class ReputationModel : IReputationModel<History, double>
    {
        public string Name
        {
            get { return "eBay"; }
        }

        public double GetReputation(IEntity entity, State<History> CurrentState)
        {
            return CurrentState.GetHistory(entity).SatisfactoryCounts;
        }

        public double GetWeight(double rep, double[] allReps)
        {
            var min = allReps.Min();
            return rep - min;
        }
    }
}
