using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.Beta
{
    class ReputationModel : IReputationModel<History, double>
    {
        public string Name
        {
            get { return "Beta"; }
        }

        public double GetReputation(IEntity entity, State<History> CurrentState)
        {
            var hs = CurrentState.GetHistory(entity);
            return (hs.Sat + 1.0) / (hs.UnSat + hs.Sat + 2.0);
        }

        public double GetWeight(double rep, double[] allReps)
        {
            return rep;
        }
    }
}
