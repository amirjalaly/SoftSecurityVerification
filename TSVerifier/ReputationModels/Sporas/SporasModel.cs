using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Environment.HonestEntities;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.Sporas
{
    class ReputationModel : IReputationModel<History, double>
    {
        public string Name
        {
            get { return "Sporas" ; }
        }

        public double GetReputation(IEntity entity, State<History> CurrentState)
        {
            if (entity == Clients.Client)
                return 0.1;
            var h = CurrentState.GetHistory(entity);
            return h.R;
        }

        public double GetWeight(double rep, double[] allReps)
        {
            return rep ;
        }
    }
}
