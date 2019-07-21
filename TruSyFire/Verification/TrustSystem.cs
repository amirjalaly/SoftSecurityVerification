using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TruSyFire.Verification
{
    public class TrustSystem<History, TTCM, Trust, TRec>
        where History : IHistory
        where TTCM : ITCM<History, Trust, TRec>, new()
        where Trust : IComparable
    {
        public TrustSystem(TruSyFire.TrustSystems.Environment.Environment env,
            TTCM tcm,
            Rewards r,
            int Maxepochs)
        {
            TCM = tcm;
            Environment = env;
            Rewards = r;
            MaxEpochs = Maxepochs;
        }
        public TruSyFire.TrustSystems.Environment.Environment Environment { get; protected set; }
        public TTCM TCM { get; protected set; }

        public Rewards Rewards { get; protected set; }

        public int MaxEpochs { get; protected set; }

        #region Actions
        TruSyFire.TrustSystems.Attackers.Action[] actions = null;

        public virtual TruSyFire.TrustSystems.Attackers.Action[] AllActions
        {
            get
            {
                if (actions == null)
                    actions = InitilizeActions();
                return actions;
            }
        }

        protected virtual TruSyFire.TrustSystems.Attackers.Action[] InitilizeActions()
        {
            IEnumerable<ServiceProviderAtomicAction> SPactions = AvailableAtomicActionsForSP;
            IEnumerable<ClientAtomicAction> Cactions = AvailableAtomicActionsForClients;

            var _all = new IAtomicAction[Environment.MaliciousSP.Length + Environment.MaliciousClients.Length];
            var All = new List<TruSyFire.TrustSystems.Attackers.Action>();
            FillActions(0, SPactions, Cactions, All, _all);
            return All.ToArray();

        }

        protected virtual IEnumerable<ServiceProviderAtomicAction> AvailableAtomicActionsForSP
        {
            get
            {
                List<ServiceProviderAtomicAction> actions = new List<ServiceProviderAtomicAction>();
                // actions.Add(new ReEntryAction());
                //actions.Add
                //foreach (IEntity a in Environment.Attackers)
                //    actions.Add(new PromotingAction() { Object = a });
                //foreach (IEntity a in Environment.HonestEntities)
                //    actions.Add(new UnfairRatingAction() { Object = a });
                actions.Add(new ServiceAction() { Quality = QoS.Bad });
                actions.Add(new ServiceAction() { Quality = QoS.Good });
                return actions;
            }
        }
        protected virtual IEnumerable<ClientAtomicAction> AvailableAtomicActionsForClients
        {
            get
            {

                var AllAvailbleRecommendation = new List<SortedList<TrustSystems.Environment.IServiceProvider, object>>();

                FillRecommends(0, new SortedList<TrustSystems.Environment.IServiceProvider, object>(), AllAvailbleRecommendation);
                List<ClientAtomicAction> actions = new List<ClientAtomicAction>();
                foreach (var a in Environment.HonestSP)
                {
                    actions.AddRange(
                        AllAvailbleRecommendation.Select(r =>
                            new RequestAction()
                            {
                                Object = a,
                                Recommendation = new SortedList<TrustSystems.Environment.IServiceProvider, object>(r)
                            }));
                }
                //return actions; 
                //foreach (var a in Environment.HonestSP)
                //{
                //    var act = new UnfairRatingAction() { Object = a };
                //    actions.AddRange(
                //        AllAvailbleRecommendation.Select(r =>
                //            new UnfairRatingAction()
                //            {
                //                Object = a,
                //                Recommendation = new SortedList<TrustSystems.Environment.IServiceProvider, object>(r)
                //            }));
                //}

                    //actions.AddRange(
                    //    AllAvailbleRecommendation.Select(r =>
                    //        new ClientNopAction()
                    //        {
                    //            Recommendation = new SortedList<TrustSystems.Environment.IServiceProvider, object>(r)
                    //        }));
                
                return actions;
            }
        }
        void FillRecommends(int index, SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object> reclist,
            List<SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object>> AllKindsOfRecommendation)
        {
            if (index >= Environment.SP.Length)
            {
                var rec = new SortedList<TrustSystems.Environment.IServiceProvider, object>(reclist);
                foreach (var k in reclist.Keys)
                    rec[k] = reclist[k];
                AllKindsOfRecommendation.Add(rec);
                return;
            }
            var recs = new TRec[]
                    {
                        TCM.RecommendationFunction.WorstRecommend,
                            TCM.RecommendationFunction.BestRecommend

                    };
            if (Environment.SP[index] is MaliciousServiceProvider)
                recs = new TRec[]
                    {
                            TCM.RecommendationFunction.BestRecommend
                    };
            foreach (var rec in recs)
            {

                reclist[Environment.SP[index]] = rec;
                FillRecommends(index + 1, reclist, AllKindsOfRecommendation);
            }

        }
        void FillActions(int index,
            IEnumerable<ServiceProviderAtomicAction> allSPAtomicactions,
            IEnumerable<ClientAtomicAction> allClientAtomicactions,
            List<TruSyFire.TrustSystems.Attackers.Action> All,
            IAtomicAction[] currentaction)
        {
            if (index >= Environment.MaliciousSP.Length + Environment.MaliciousClients.Length)
            {
                All.Add(new TruSyFire.TrustSystems.Attackers.Action
                    ((IAtomicAction[])currentaction.Clone()));
                return;
            }
            if (index >= Environment.MaliciousSP.Length)
                foreach (var a in allClientAtomicactions)
                {
                    var c = Environment.MaliciousClients[index - Environment.MaliciousSP.Length];
                    if (a.Type == ClientsActionType.Request &&
                        (a as RequestAction).Object == c)
                        continue;
                    if (a is RequestAction)
                    {
                        var i = 0;
                        for (; i < Environment.MaliciousSP.Length; i++)
                            if (Environment.MaliciousSP[i] == (a as RequestAction).Object)
                                break;
                        if (i < Environment.MaliciousSP.Length && (currentaction[i] as ServiceProviderAtomicAction).Type != ServiceProvidersActionType.Service)
                            continue;
                    }
                    var ap = a.Clone();
                    var sp = Environment.MaliciousSP.FirstOrDefault(e => e.Name == c.Name);
                    if (sp != null && ap.Recommendation.ContainsKey(sp))
                        ap.Recommendation.Remove(sp);
                    currentaction[index] = ap;
                    FillActions(index + 1, allSPAtomicactions, allClientAtomicactions, All, currentaction);
                }
            if (index < Environment.MaliciousSP.Length)
                foreach (var a in allSPAtomicactions)
                {
                    currentaction[index] = a;
                    FillActions(index + 1, allSPAtomicactions, allClientAtomicactions, All, currentaction);
                }
        }
        #endregion
    }
}
