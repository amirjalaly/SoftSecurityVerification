using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.POMDP;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TruSyFire.Verification
{
    class AttackerObservation<History, Trust, Recommendation> : IObserveFunction<State<History>>
        where History : IHistory
        where Trust : IComparable
    {
        public KnowledgeLevel KnowledgeLevel { get; private set; }
        public ITCM<History, Trust, Recommendation> TCM { get; private set; }
        public AttackerObservation(KnowledgeLevel KnowledgeLevel,
            ITCM<History, Trust, Recommendation> TCM)
        {
            this.KnowledgeLevel = KnowledgeLevel;
            this.TCM = TCM;
        }
        public string GetObservation(State<History> st)
        {
            switch (KnowledgeLevel)
            {
                case KnowledgeLevel.RecommendationValues:
                    var Rmatrix = "";
                    foreach (var h in st.GetAllHistories())
                        if (h.ServiceProvider != h.Client)
                            Rmatrix += TCM.RecommendationFunction.GetRecommendation(
                                h).ToString() + "&";
                    return Rmatrix;
                case KnowledgeLevel.TrustValues:
                    var Tmatrix = "";
                    foreach (var h in st.GetAllHistories())
                        if (h.ServiceProvider != h.Client)
                            Tmatrix += TCM.GetTrust(
                            h.ServiceProvider,
                            h.Client,
                            st).ToString() + "&";
                    return Tmatrix;
                case KnowledgeLevel.FullObservation:
                    return st.Index;
                case KnowledgeLevel.ZeroKnowledge:
                default:
                    return st.Epoch.ToString();
            }
        }
    }
    public enum KnowledgeLevel
    {
        ZeroKnowledge =0,
        RecommendationValues,
        TrustValues,
        FullObservation
    }
}
