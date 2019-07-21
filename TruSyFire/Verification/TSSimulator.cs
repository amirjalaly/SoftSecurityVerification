using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TruSyFire.Verification
{
    class Simulator<History, TCM, Trust, TRec>
        : ISimulator<State<History>, TruSyFire.TrustSystems.Attackers.Action>
        where History : IHistory
        where TCM : ITCM<History, Trust, TRec>, new()
        where Trust : IComparable
    {
        public Simulator(TrustSystem<History, TCM, Trust, TRec> system)
        {
            System = system;
        }
        public TrustSystem<History, TCM, Trust, TRec>
            System { get; private set; }
        public override SortedList<State<History>, double> GenerateStates(
            State<History> current,
            TruSyFire.TrustSystems.Attackers.Action SelectedAction)
        {
            SortedList<State<History>, double> Nexts
                = new SortedList<State<History>, double>();
            if (current.Epoch == System.MaxEpochs)
            {
                return Nexts;
            }

            History[] hs = current.GetAllHistories();
            List<TrustSystems.Environment.IServiceProvider> ParticipatingsSP;
            List<IClient> ParticipatingsClients;
            List<TrustSystems.Environment.IServiceProvider> ReEntryAttackers;
            ClassifyEntities(SelectedAction, out ParticipatingsSP, out ParticipatingsClients, out ReEntryAttackers);

            foreach (var e in ReEntryAttackers)
                hs.FirstOrDefault(h => h.ServiceProvider == e).Reset();
            int CsNo = ParticipatingsClients.Count();

            foreach (var c in ParticipatingsClients)
            {
                var sp = c.SelectServiceProvider
                    (
                    ParticipatingsSP.Where(p => p != c).ToArray(),
                    System.TCM,
                    current);
                foreach (var s in sp)
                {
                    var qs = s.Object.Behave(c, System.TCM, current);
                    foreach (var q in qs)
                    {
                        var nhs = hs.Select(h => (History)h.Clone()).ToArray();
                        nhs.FirstOrDefault(h => h.ServiceProvider == s.Object && h.Client == c)
                            .Update(c.Rate(q.Object, s.Object), current);
                        AddNextState(Nexts, current, nhs,
                            s.Prob * q.Prob / CsNo);
                    }
                }
            }

            return Nexts;


        }

        public override bool IsIsolated(
    State<History> current,
    TruSyFire.TrustSystems.Attackers.Action SelectedAction)
        {
            History[] hs = current.GetAllHistories();
            List<TrustSystems.Environment.IServiceProvider> ParticipatingsSP;
            List<IClient> ParticipatingsClients;
            List<TrustSystems.Environment.IServiceProvider> ReEntryAttackers;
            ClassifyEntities(SelectedAction, out ParticipatingsSP, out ParticipatingsClients, out ReEntryAttackers);

            int CsNo = ParticipatingsClients.Count();
            double Sum = 0;
            foreach (var c in ParticipatingsClients)
            {
                if (c.EntityType == "Attacker")
                    continue;
                var sp = c.SelectServiceProvider
                    (
                    ParticipatingsSP.Where(p => p != c).ToArray(),
                    System.TCM,
                    current);
                foreach (var s in sp)
                {
                    if (s.Object.EntityType == "Attacker")
                        Sum += s.Prob;
                }
            }

            return  Sum<.3;


        }

        private void ClassifyEntities(TruSyFire.TrustSystems.Attackers.Action SelectedAction, out List<TrustSystems.Environment.IServiceProvider> ParticipatingsSP, out List<IClient> ParticipatingsClients, out List<TrustSystems.Environment.IServiceProvider> ReEntryAttackers)
        {
            ParticipatingsSP = new List<TruSyFire.TrustSystems.Environment.IServiceProvider>();
            ParticipatingsClients = new List<TruSyFire.TrustSystems.Environment.IClient>();
            ReEntryAttackers = new List<TruSyFire.TrustSystems.Environment.IServiceProvider>();
            for (int i = 0; i < System.Environment.MaliciousSP.Length; i++)
            {
                System.Environment.MaliciousSP[i].SelectedAction = (ServiceProviderAtomicAction)SelectedAction[i];
                switch (System.Environment.MaliciousSP[i].SelectedAction.Type)
                {
                    case ServiceProvidersActionType.Honest:
                    case ServiceProvidersActionType.Service:
                        ParticipatingsSP.Add(System.Environment.MaliciousSP[i]);
                        break;

                    case ServiceProvidersActionType.ReEntry:
                        ReEntryAttackers.Add(System.Environment.MaliciousSP[i]);
                        break;
                }
            }
            ParticipatingsSP.AddRange(System.Environment.HonestSP);

            for (int i = 0;
                i < System.Environment.MaliciousClients.Length;
                i++)
            {
                int j = System.Environment.MaliciousSP.Length + i;
                System.Environment.MaliciousClients[i].SelectedAction =
                    (ClientAtomicAction)SelectedAction[j];
                if (((ClientAtomicAction)SelectedAction[j]).Type != ClientsActionType.Nop)
                    ParticipatingsClients.Add(System.Environment.MaliciousClients[i]);

            }
            ParticipatingsClients.AddRange(System.Environment.HonestClients);

        }

        void AddNextState(SortedList<State<History>, double> Nexts, State<History> current, History[] hs, double p)
        {
            var State = current.CreateNextState(hs);
            if (Nexts.ContainsKey(State))
                Nexts[State] += p;
            else
                Nexts.Add(State, p);
        }

        public override double GetImmediateReward(State<History> Source,
            State<History> Dest,
            TruSyFire.TrustSystems.Attackers.Action SelectedAction)
        {
            History[] hs = Source.GetAllHistories();

            #region Finding Particapting/Client/Renentry attackers
            List<TrustSystems.Environment.IServiceProvider> ParticipatingsSP;
            List<IClient> ParticipatingsClients;
            List<TrustSystems.Environment.IServiceProvider> ReEntryAttackers;
            ClassifyEntities(SelectedAction, out ParticipatingsSP, out ParticipatingsClients, out ReEntryAttackers);
            //ParticipatingsClients.AddRange(System.Environment.HonestClients);
            int CsNo = ParticipatingsClients.Count();
            #endregion

            #region C_ID
            var sumR = 0.0;
            var sumP = 0.0;
            var ReEnterCnt = ReEntryAttackers.Count;
            var CID = ReEnterCnt != 0 ? ReEnterCnt * System.Rewards.GetNewIdentityCost() : 0;
            #endregion

            #region C_Srv / R_Req
            // foreach(var sp in ParticipatingsSP)
            foreach (var c in ParticipatingsClients)
            {
                var sp = c.SelectServiceProvider
                    (
                    ParticipatingsSP.Where(p => p != c).ToArray(),
                    System.TCM,
                    Source);
                foreach (var s in sp)
                {
                    var qs = s.Object.Behave(c, System.TCM, Source);
                    foreach (var q in qs)
                    {
                        //var nhs = hs.Select(h => (History)h.Clone()).ToArray();
                        var nh = hs.FirstOrDefault(h => h.ServiceProvider == s.Object && h.Client == c).Clone();
                        nh.Update(c.Rate(q.Object, s.Object), Source);

                        if (Dest.GetHistory(s.Object, c) == nh)
                        {
                            var p = s.Prob * q.Prob / CsNo;
                            if (s.Object is MaliciousServiceProvider)
                                sumR +=
                                  p * System.Rewards.GetServiceProviderReward(q.Object);
                            if (c is MaliciousClient)
                                sumR += p * System.Rewards.GetClientReward(q.Object);
                            if (c is MaliciousClient || s.Object is MaliciousServiceProvider)
                                sumP += p;
                        }
                    }

                }

            }
            #endregion

            //#region C_Req/R_Srv
            //foreach (var c in ParticipatingsClients)
            //{
            //    var ds = c.SelectServiceProvider(allSP, System.TCM, Source);
            //    foreach (var d in ds)
            //    {
            //        var sp = d.Object;
            //        var qs = sp.Behave(c, System.TCM, Source);
            //        foreach (var q in qs)
            //        {
            //            var rate = c.Rate(q.Object, sp);/// the QoS does not matter in rating
            //            var hs = Source.GetHistory(sp, c);
            //            hs.Update(rate, Source);
            //            if (Dest.GetHistory(sp, c) == hs)
            //            {
            //                var p = d.Prob * q.Prob / CsNo;
            //                sumP += p;
            //            }
            //        }
            //    }
            //}
            //#endregion
            if (sumR == 0)
                return -CID + GetStateReward(Dest);
            sumR = sumR / sumP;
            return sumR - CID + GetStateReward(Dest);


        }

        double GetStateReward(State<History> State)
        {
            if (State.Epoch != System.MaxEpochs)
                return 0;

            return System.Rewards.GetSlanderingReward(State, System);
        }
    }
}
