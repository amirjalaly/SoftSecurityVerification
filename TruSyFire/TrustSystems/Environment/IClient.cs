using System;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.Environment
{

    public abstract class IClient : IEntity
    {
        public IClient(string Name) : base(Name) { }
        public abstract Distribution<IServiceProvider>[]
            SelectServiceProvider<History, Trust,Rec>(IServiceProvider[] Entities, 
                ITCM<History, Trust,Rec> TCM, 
                State<History> State)
            where History : IHistory
            where Trust : IComparable;
        public virtual Recommendation GetRecommendation<RecommendationFunction, History, Recommendation>
            (IServiceProvider sp, State<History> s,RecommendationFunction RecFunc)
            where History : IHistory
            where RecommendationFunction : IRecommendationFunction<History, Recommendation>
        {
            return RecFunc.GetRecommendation(s.GetHistory(sp, this));
        }


        public abstract QoS Rate(QoS qos, IServiceProvider sp);
        public abstract IClient Clone(string NewName);

    }


}
