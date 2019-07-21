using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.AttackPlan
{
    class SimMaliciousness<H,Rec,Rcf>:IPlan<H>
        where H:IHistory,new()
        where Rcf:IRecommendationFunction<H,Rec>,new()
    {
        public SimMaliciousness(TruSyFire.TrustSystems.Environment.Environment e)
            : base(e) { }
        public override string Title
        {
            get { return "Simple Maliciousness"; }
        }

        public override ServiceProviderAtomicAction GetAction(
            TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief, 
            MaliciousServiceProvider attacker)
        {
            return new ServiceAction()
                {
                    Quality = QoS.Bad
                }
                    ;
        }
        Rcf rcf = new Rcf();
        SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider,object>
            Recs = 
            null;
        public override ClientAtomicAction GetAction(TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief, MaliciousClient attacker)
        {
            if(Recs == null)
            {
                Recs = new SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider,object>();
                foreach(var sp in Environment.SP)
                    Recs[sp] = rcf.GetRecommendation(new H());
            }
            return new ClientNopAction() { Recommendation = Recs };
        }
    }
}
