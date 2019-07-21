using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;
using Action = TruSyFire.TrustSystems.Attackers.Action;

namespace TruSyFire.Verification.RobustnessMeasure
{
    class HonestPlan<History> : IPlan<State<History>,Action>
       where History : IHistory
    {
        public TrustSystems.Environment.Environment Environment { get; private set; }
        public HonestPlan(TrustSystems.Environment.Environment Env )
        {
            Environment = Env;
        }

        public Action GetAction(State<History> State)
        {
            var acts = new IAtomicAction[Environment.AttackersCount];
            for (int i = 0; i < Environment.MaliciousSP.Length; i++)
                acts[i] = new SPHonestAction(
                    Environment.HonestSP[0].Clone(Environment.MaliciousSP[i].Name)
                    );
            for (int i = 0; i < Environment.MaliciousClients.Length; i++)
                acts[i + Environment.MaliciousSP.Length] = 
                    new ClientHonestAction(
                    Environment.HonestClients[0].Clone(Environment.MaliciousClients[i].Name)
                        );
            return new Action(acts);
        }

        public string Title
        {
            get { return "Honest"; }
        }
    }
}
