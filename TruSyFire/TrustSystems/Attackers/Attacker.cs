using System;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;
using IServiceProvider = TruSyFire.TrustSystems.Environment.IServiceProvider;

namespace TruSyFire.TrustSystems.Attackers
{
    public class MaliciousServiceProvider : IServiceProvider
    {
        public MaliciousServiceProvider(string name) :
            base(name) { }


        public ServiceProviderAtomicAction SelectedAction { get; set; }
        public override Distribution<QoS>[] Behave<History, Trust, Rec>(IClient Client,
            ITCM<History, Trust, Rec> TCM,
            State<History> CurrentState)
        {
            if (SelectedAction.Type == ServiceProvidersActionType.Honest)
                return (SelectedAction as SPHonestAction).HonestEntity.Behave(Client, TCM, CurrentState);
            return new Distribution<QoS>[]{
                new Distribution<QoS>()
                    {
                        Prob=1,
                        Object=(SelectedAction as ServiceAction).Quality
                    }};
        }

        public override IServiceProvider Clone(string NewName)
        {
            return new MaliciousServiceProvider(NewName) { SelectedAction = SelectedAction };
            ;
        }

        public override string EntityType
        {
            get { return "Attacker"; }
        }
    }
    public class MaliciousClient : IClient
    {
        public MaliciousClient(string name) : base(name) { }
        public ClientAtomicAction SelectedAction { get; set; }


        public override Distribution<IServiceProvider>[] SelectServiceProvider<History, Trust, Rec>
            (IServiceProvider[] Entities,
                ITCM<History, Trust, Rec> TCM,
                State<History> State)
        {
            if (SelectedAction.Type == ClientsActionType.Honest)
                return (SelectedAction as ClientHonestAction).HonestEntity.SelectServiceProvider(Entities, TCM, State);
            return new Distribution<IServiceProvider>[]
            {
                new Distribution<IServiceProvider>()
                {
                    Object = (SelectedAction as RequestAction).Object,
                    Prob = 1
                }
            };
        }

        public override QoS Rate(QoS qos, IServiceProvider sp)
        {
            return qos;
            //if (SelectedAction.Type == ClientsActionType.Honest)
            //    return (SelectedAction as ClientHonestAction).HonestEntity.Rate(qos, sp);
            //return SelectedAction is UnfairRatingAction ? QoS.Bad : QoS.Good;
        }

        public override Recommendation GetRecommendation<RecommendationFunction, History, Recommendation>
            (IServiceProvider sp, State<History> s, RecommendationFunction RecFunc)
        {
            if (SelectedAction == null)
                return RecFunc.GetRecommendation(s.GetHistory(sp,this));
            if(SelectedAction.Type == ClientsActionType.Honest)
                return (SelectedAction as ClientHonestAction).HonestEntity
                    .GetRecommendation<RecommendationFunction, History, Recommendation>(sp, s, RecFunc);
            return (Recommendation)SelectedAction.Recommendation[sp];
        }
        public override IClient Clone(string NewName)
        {
            return new MaliciousClient(NewName) { SelectedAction = SelectedAction };
        }
        public override string EntityType
        {
            get { return "Attacker"; }
        }

    }
}
