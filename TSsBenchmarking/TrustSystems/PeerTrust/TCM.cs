using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TSsBenchmarking.TrustSystems.PeerTrust
{
    public class TCM : ITCM<History, double, double>
    {
        double Sim(IClient v, IClient w, State<History> State)
        {
            if (v == w)
                return 1;
            var ijs = IJS(v, w, State);
            if (ijs.Count() == 0)
                return 1;
            double sum = 0;
            foreach (var sp in ijs)
                sum +=
                    Math.Pow(
                    v.GetRecommendation<RecommendationFunction, History, double>
                    (sp, State, new RecommendationFunction())
                    -
                    w.GetRecommendation<RecommendationFunction, History, double>
                    (sp, State, new RecommendationFunction())
                    , 2);
            return 1 - Math.Sqrt(sum / ijs.Count());
        }
        IEnumerable<TruSyFire.TrustSystems.Environment.IServiceProvider>
            IJS(IClient v, IClient w, State<History> State)
        {
            foreach (var sp in State.Environment.SP)
                if (I(v, sp, State) > 0 && I(w, sp, State) > 0)
                    yield return sp;

        }
        int I(IClient c, TruSyFire.TrustSystems.Environment.IServiceProvider sp, State<History> State)
        {
            return State.GetHistory(sp, c).TotalTransaction;
        }
        RecommendationFunction RcF = new RecommendationFunction();
        public IRecommendationFunction<History, double> RecommendationFunction
        {
            get { return RcF; }
        }

        public string Name
        {
            get { return "PeerTrust"; }
        }
        static SortedList<string, SortedList<string, SortedList<string, SortedList<string, double>>>> 
            TV_Cache
            = new SortedList<string, SortedList<string, SortedList<string, SortedList<string, double>>>>();
        public double GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, IClient c, State<History> CurrentState)
        {
            var i = CurrentState.Index;
            if (!TV_Cache.ContainsKey(i))
                TV_Cache[i] = new SortedList<string, SortedList<string, SortedList<string, double>>>();
            var j = "";
            foreach (var a in CurrentState.Environment.MaliciousClients)
                j += (a.SelectedAction==null?"null":a.SelectedAction.ToString()) + "~";
            if (!TV_Cache[i].ContainsKey(j))
                TV_Cache[i][j] = new SortedList<string, SortedList<string, double>>();
            var k = c.Name;
            if (!TV_Cache[i][j].ContainsKey(k))
                TV_Cache[i][j][k] = new SortedList<string, double>();
            var l = sp.Name;
            if (!TV_Cache[i][j][k].ContainsKey(l))
                TV_Cache[i][j][k][l] = getTrust(sp, c, CurrentState);
            return TV_Cache[i][j][k][l];
        }


        public double getTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, IClient c, State<History> CurrentState)
        {
            double sum = 0, sumsim = 0;
            //if (CurrentState.Index == "(0/0)|(0/0)|(0/1)|(0/1)|(0/0)|(0/0)|(0/1)|(0/1)|(0/0)|(0/0)|(0/0)|(0/0)|(0/0)|(0/0)|(0/0)|(0/0)|" 
            //    && c.Name == "H_0" && sp.Name == "H_1")
            //    sum = 0;
            foreach (var client in CurrentState.Environment.Clients)
            {
                if (client == sp)
                    continue;
                var sim = Sim(c, client, CurrentState);
                double s;
                if(client == c)
                s = RcF.GetRecommendation(CurrentState.GetHistory(sp,c));
                else
                s = client.GetRecommendation<RecommendationFunction, History, double>
                    (sp, CurrentState,RcF);
                sum += sim * s;
                sumsim += sim;
            }
            if (sumsim == 0)
                return 0;
            
            return sum / sumsim;

        }

        public double Quantitize(double t)
        {
            return t;
        }
    }
}
