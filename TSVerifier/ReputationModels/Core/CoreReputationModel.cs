using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.Core
{
    class ReputationModel : IReputationModel<History, double>
    {
        public string Name
        {
            get { return "Core"; }
        }

        public double GetReputation(IEntity entity, State<History> CurrentState)
        {
            var hs = CurrentState.GetHistory(entity);
            var sq = hs.SequenceOfServices;
            var r = 0;
            for (int i = 0; i < sq.Length; i++)
                r += (sq[i] == '+' ? 1 : 0) * (sq.Length-i);

            if (r == 0)
                return 0;
            return r / (sq.Length * (sq.Length + 1) / 2.0);
        }

        public double GetWeight(double rep, double[] allReps)
        {
            return rep;
        }
    }

}
