using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.AttackPlan
{
    public class OnOff<H, Rec, Rcf> : IPlan<H>
        where H : IHistory, new()
        where Rcf : IRecommendationFunction<H, Rec>, new()
    {
        public int Cycle {get;private set;}
               public OnOff(TruSyFire.TrustSystems.Environment.Environment e,int cycle)
            : base(e) { Cycle= cycle;}
        public override string Title
        {
            get { return "On-Off Attack"; }
        }

        public override ServiceProviderAtomicAction GetAction(
            TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief, 
            MaliciousServiceProvider attacker)
        {
            return new ServiceAction()
                {
                    Quality = Belief.Epoch % (2*Cycle)<Cycle?QoS.Good: QoS.Bad
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
            return new RequestAction() { Recommendation = Recs,
            Object = Environment.SP[0]};
        }
    }

    class RecommendOnOff<H, Rec, Rcf,T,TCM> : IPlan<H>
        where H : IHistory, new()
        where Rcf : IRecommendationFunction<H, Rec>, new()
        where T : IComparable
        where Rec : IComparable
        where TCM:ITCM<H,T,Rec>,new()
    {
        public Rec Threshold { get; private set; }
        public RecommendOnOff(TruSyFire.TrustSystems.Environment.Environment e, Rec threshold)
            : base(e) { Threshold = threshold; }
        public override string Title
        {
            get { return "On-Off Attack"; }
        }

        public override ServiceProviderAtomicAction GetAction(
            TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief,
            MaliciousServiceProvider attacker)
        {
            var state = Belief.GetAllStatesInBeliefVector().First();

            int cnt=0;
            //var Recs = new SortedList<TruSyFire.TrustSystems.Environment.IClient, object>();
            foreach (var c in Environment.Clients)
                if (c.Name != attacker.Name)
                    if(Threshold.CompareTo( rcf.GetRecommendation(state.GetHistory(attacker,c)))>0)
                        cnt++;



            return new ServiceAction()
            {
                Quality = cnt>Environment.Clients.Length/2 ? QoS.Good : QoS.Bad
            }
                    ;
        }
 
        Rcf rcf = new Rcf();
        SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object>
            Recs =
            null;
        public override ClientAtomicAction GetAction(TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief, MaliciousClient attacker)
        {
             var state = Belief.GetAllStatesInBeliefVector().First();
            var Recs = new SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object>();
            foreach (var sp in Environment.SP)
                if (sp != attacker)
                    Recs[sp] = rcf.GetRecommendation(state.GetHistory(sp, attacker));
            var bestsp = state.Environment.SP[0];
            var tcm = new TCM();
            T max = tcm.GetTrust(bestsp, attacker, state);
            for (int i = 1; i < state.Environment.SP.Length; i++)
            {
                var sp2 = state.Environment.SP[i];
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


    class TrustOnOff<H, Rec, Rcf, T, TCM> : IPlan<H>
        where H : IHistory, new()
        where Rcf : IRecommendationFunction<H, Rec>, new()
        where T : IComparable
        where Rec : IComparable
        where TCM : ITCM<H, T, Rec>, new()
    {
        public T Threshold { get; private set; }
        public TrustOnOff(TruSyFire.TrustSystems.Environment.Environment e, T threshold)
            : base(e) { Threshold = threshold; }
        public override string Title
        {
            get { return "On-Off Attack"; }
        }

        public override ServiceProviderAtomicAction GetAction(
            TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief,
            MaliciousServiceProvider attacker)
        {
            var state = Belief.GetAllStatesInBeliefVector().First();

            int cnt = 0;
            var tcm = new TCM();
            //var Recs = new SortedList<TruSyFire.TrustSystems.Environment.IClient, object>();
            foreach (var c in Environment.Clients)
                if (c.Name != attacker.Name)
                    if (Threshold.CompareTo(tcm.GetTrust(attacker, c,state)) > 0)
                        cnt++;



            return new ServiceAction()
            {
                Quality = cnt > Environment.Clients.Length / 2 ? QoS.Good : QoS.Bad
            }
                    ;
        }

        Rcf rcf = new Rcf();
        SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object>
            Recs =
            null;
        public override ClientAtomicAction GetAction(TruSyFire.POMDP.Belief<TruSyFire.Verification.State<H>> Belief, MaliciousClient attacker)
        {
            var state = Belief.GetAllStatesInBeliefVector().First();
            var Recs = new SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object>();
            foreach (var sp in Environment.SP)
                if (sp != attacker)
                    Recs[sp] = rcf.GetRecommendation(state.GetHistory(sp, attacker));
            var bestsp = state.Environment.SP[0];
            var tcm = new TCM();
            T max = tcm.GetTrust(bestsp, attacker, state);
            for (int i = 1; i < state.Environment.SP.Length; i++)
            {
                var sp2 = state.Environment.SP[i];
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
