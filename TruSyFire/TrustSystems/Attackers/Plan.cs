using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;
using TruSyFire.POMDP;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.Attackers
{
    public abstract class IPlan<History>:IPlan<Belief<State<History>>,Action>
        where History:IHistory
    {
        public Environment.Environment Environment { get; private set; }
        public IPlan(Environment.Environment Env)
        {
            Environment = Env;
        }
        public abstract string Title{get;}

        public abstract ServiceProviderAtomicAction GetAction(Belief<State<History>> Belief, MaliciousServiceProvider attacker);
        public abstract ClientAtomicAction GetAction(Belief<State<History>> Belief, MaliciousClient attacker);



        public Action GetAction(Belief<State<History>> Belief)
        {
            var acts = new IAtomicAction[Environment.AttackersCount];
            for (int i = 0; i < Environment.MaliciousSP.Length; i++)
                acts[i] = GetAction(Belief, Environment.MaliciousSP[i]);
            for (int i = 0; i < Environment.MaliciousClients.Length; i++)
                acts[i + Environment.MaliciousSP.Length] =
                    GetAction(Belief,Environment.MaliciousClients[i]);
            return new Action(acts);

        }
    }
}
