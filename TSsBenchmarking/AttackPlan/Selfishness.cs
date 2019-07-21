using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TSsBenchmarking.AttackPlan
{
    public class Selfishness<H, Rec, Rcf, Trust, TCM> : IPlan<H>
        where H : IHistory, new()
        where Rcf : IRecommendationFunction<H, Rec>, new()
        where TCM : ITCM<H, Trust, Rec>, new()
        where Trust : IComparable
    {
        public Selfishness(TruSyFire.TrustSystems.Environment.Environment e)
            : base(e) { }
        public override string Title
        {
            get { return "Selfishness"; }
        }

        public override ServiceProviderAtomicAction GetAction(TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief, MaliciousServiceProvider attacker)
        {
            return new ServiceAction()
            {
                Quality = QoS.NoService
            }
                    ;
        }
        Rcf rcf = new Rcf();


        public override ClientAtomicAction GetAction(TruSyFire.POMDP.Belief<State<H>> Belief
            , MaliciousClient attacker)
        {
            var state = Belief.GetAllStatesInBeliefVector().First();
            var Recs = new SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object>();
            foreach (var sp in Environment.SP)
                if (sp != attacker)
                    Recs[sp] = rcf.GetRecommendation(state.GetHistory(sp, attacker));
            var bestsp = state.Environment.HonestSP[0];
            var tcm = new TCM();
            Trust max = tcm.GetTrust(bestsp, attacker, state);
            for (int i = 1; i < state.Environment.HonestSP.Length; i++)
            {
                var sp2 = state.Environment.HonestSP[i];
                var t = tcm.GetTrust(sp2, attacker, state);
                if (max.CompareTo(t) < 0)
                {
                    bestsp = sp2;
                    max = t;
                }
            }
            return new RequestAction()
            {
                Object = bestsp,
                Recommendation = Recs
            };
        }
    }
}
