using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.PeerTrust
{
    class ReputationModel : IReputationModel<History, double>
    {
        public string Name
        {
            get { return "PeerTrust"; }
        }
        static SortedList<State<History>, SortedList<IEntity,double>> CachedR = new SortedList<State<History>,SortedList<IEntity,double>>();

        public double GetReputation(IEntity entity, State<History> CurrentState)
        {
            ///the Peerturst formula has tow parts
            ///S(.,.) x Cr(.,.) x TF
            ///beta x CF - this part is ignored because there is diffrence between entities
            ///TF is also ignore becuase there is just one context
            ///Cr for reputation model is:
            ///Cr(u) = Sim(u,S)/ Sigma Sim(v,S) v in S
            ///Sim(u,S) = 1 - sqrt(sigma(s(x,u)/|s(x,u) - s(x,.)/|s(x,.)|^2)/
            ///
            if (!CachedR.ContainsKey(CurrentState))
                CachedR[CurrentState] = new SortedList<IEntity,double>();
            if(!CachedR[CurrentState].ContainsKey(entity))
            {
                var h = CurrentState.GetHistory(entity);
                var SumSim = h.S.Keys.Sum(p => Similarity(p, CurrentState));
                var T = h.S.Keys.Sum(p => S(p, h) * Similarity(p, CurrentState) / SumSim);
                CachedR[CurrentState][entity] = T;
                return T;

            }
            return CachedR[CurrentState][entity];


 

        }

        double Similarity(IEntity v, State<History> State)
        {
            var sum = 0.0;
            var N = 0;
            var hs = State.GetAllHistories();
            foreach (var h in hs)
            {
                var S_all = 0.0;
                var N_all = 0.0;
                var sum_v =-1.0;
                foreach (var c in h.S.Keys)
                {
                    if (c == v)
                    {
                        //x is found
                        sum_v = S(v, h);
                        N++;
                    }
                    S_all += h.S[c][0];
                    N_all += h.S[c][1];
                }
                if (sum_v != -1)
                {
                    sum += Math.Pow(sum_v - S_all / N_all,2);
                    
                }
            }
            if (N == 0)
                return 1;
            sum = Math.Sqrt(sum / N);
            return 1 - sum;
        }

        private static double S(IEntity v, History h)
        {
            return h.S[v][0]*1.0 / h.S[v][1];
        }
        public double GetWeight(double rep, double[] allReps)
        {
            return rep;
        }
    }
}
