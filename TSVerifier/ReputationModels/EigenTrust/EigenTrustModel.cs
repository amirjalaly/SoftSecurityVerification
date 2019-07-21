using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Environment.HonestEntities;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.EigenTrust
{
    class ReputationModel : IReputationModel<History, double>
    {
        public string Name
        {
            get { return "EigenTrust"; }
        }
        static SortedList<State<History>, double[]> CachedR = new SortedList<State<History>, double[]>();

        public double GetReputation(IEntity entity, State<History> CurrentState)
        {
            var hs = CurrentState.GetAllHistories();
            var cnt = hs.Length + 1;
            if (!CachedR.ContainsKey(CurrentState))
            {

                var C = new double[cnt, cnt];
                for (int i = 0; i <= hs.Length; i++)
                    for (int j = 0; j <= hs.Length; j++)
                    {
                        if (j == 0 )
                        {
                            C[i, j] = 1.0 / cnt;
                            continue;
                        }
                        if (hs[j-1].Owner is Attacker && (hs[j-1].Owner as Attacker).IgnoreInProviderSelection)
                        {
                            C[i, j] = 1.0 / cnt;
                            continue;
                        }
                        if (i == j)
                        {
                            C[i, j] = 0;
                            continue;
                        }
                        if (i == 0)
                        {

                            C[i, j] = hs[j - 1].S.ContainsKey(Clients.Client) ?
                                Math.Max((int)hs[j - 1].S[Clients.Client], 0) :
                                0;
                            continue;
                        }
                        C[i, j] = hs[j - 1].S.ContainsKey(hs[i - 1].Owner) ?
                            Math.Max((int)hs[j - 1].S[hs[i - 1].Owner], 0) :
                            0;
                    }
                for (int i = 0; i <= hs.Length; i++)
                {
                    var sum = 0.0;
                    for (int j = 1; j <= hs.Length; j++)
                    {
                        sum += C[i, j];
                    }
                    for (int j = 1; j <= hs.Length; j++)
                        C[i, j] = sum == 0 ? 1.0 / cnt : (C[i, j] / sum);
                }
                double[] R = new double[cnt];
                for (int i = 0; i < cnt; i++)
                    R[i] = 1.0 / cnt;
                for (int n = 0; n < 10; n++)
                {
                    var diff = 0.0;
                    for (int i = 0; i < cnt; i++)
                    {
                        double r = 0;
                        for (int j = 0; j < cnt; j++)
                            r += R[j] * C[j, i];
                        diff += Math.Abs(R[i] - r);
                        R[i] = r;
                    }
                    if (diff / R.Length < 0.01)
                        break;
                }
                CachedR[CurrentState] = R;
            }
            for (int i = 1; i < cnt; i++)
                if (hs[i - 1].Owner == entity)
                    return CachedR[CurrentState][i];
            return 0;

        }

        public double GetWeight(double rep, double[] allReps)
        {
            return rep;
        }
    }
}
