using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TSsBenchmarking.TrustSystems.Abdul_Rahman
{
    public class TCM : ITCM<History, Trust, Trust>
    {
        RecommendationFunction RcF = new RecommendationFunction();
        public IRecommendationFunction<History, Trust> RecommendationFunction
        {
            get { return RcF; }
        }

        public string Name
        {
            get { return "Abdul-Rahman"; }
        }

        Trust td(History h)
        {
            return RcF.GetRecommendation(h);
        }
        /// <summary>
        /// rtd : 0..3 or -1 for unkown
        /// </summary>
        /// <param name="h"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        int rtd(State<History> s, IClient c, IClient recommender)
        {
            byte[] x = new byte[4];
            foreach (var sp in s.Environment.SP)
            {
                if (sp != c)
                {
                    var h = s.GetHistory(sp, c);
                    for (int j = 0; j < 4; j++)
                    {
                        if (h.T[j] != null)
                        {
                            if (h.T[j].ContainsKey(recommender.Name))
                                for (int i = 0; i < 7; i++)
                                    x[Math.Abs(i - 3)] += h.T[j][recommender.Name][i];
                        }
                    }
                }
            }
            return ArgMax(x);
        }
        int sd(History h, IClient c, Trust e)
        {
            if (h.T[index(e)] == null || !h.T[index(e)].ContainsKey(c.Name))
                return 0;
            var arg = ArgMax(h.T[index(e)][c.Name]);
            if (arg == -1)
                return 0;
            return arg - 3;
        }


        byte weight(int rtd)
        {
            switch (rtd)
            {
                case -1:
                    return 0;
                case 0:
                    return 9;
                case 1:
                    return 5;
                case 2:
                    return 3;
                case 3:
                    return 1;

            }
            return 0;

        }
        int ArgMax(byte[] x)
        {
            int max = -1;
            int arg = -1;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] == max)
                {
                    arg = -1;
                }
                if (x[i] > max)
                {
                    max = x[i];
                    arg = i;
                }
            }
            return arg;
        }
        static SortedList<string, SortedList<string, SortedList<string, SortedList<string, Trust>>>>
     TV_Cache
     = new SortedList<string, SortedList<string, SortedList<string, SortedList<string, Trust>>>>();
        public Trust getTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, IClient c, State<History> CurrentState)
        {
            var i = CurrentState.Index;
            if (!TV_Cache.ContainsKey(i))
                TV_Cache[i] = new SortedList<string, SortedList<string, SortedList<string, Trust>>>();
            var j = "";
            foreach (var a in CurrentState.Environment.MaliciousClients)
                j += (a.SelectedAction == null ? "null" : a.SelectedAction.ToString()) + "~";
            if (!TV_Cache[i].ContainsKey(j))
                TV_Cache[i][j] = new SortedList<string, SortedList<string, Trust>>();
            var k = c.Name;
            if (!TV_Cache[i][j].ContainsKey(k))
                TV_Cache[i][j][k] = new SortedList<string, Trust>();
            var l = sp.Name;
            if (!TV_Cache[i][j][k].ContainsKey(l))
                TV_Cache[i][j][k][l] = getTrust(sp, c, CurrentState);
            return TV_Cache[i][j][k][l];
        }


        public Trust GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, TruSyFire.TrustSystems.Environment.IClient c, TruSyFire.Verification.State<History> CurrentState)
        {

            byte[] x = new byte[4];
            var h = CurrentState.GetHistory(sp, c);
            var t = td(h);

            if (t != Trust.uncertaintly)
                x[index(t)] = weight(0);
            foreach (var other in CurrentState.Environment.Clients)
            {
                if (other != c && other != sp)
                {
                    var rd = other.GetRecommendation
                        <RecommendationFunction, History, Trust>
                        (sp, CurrentState, RcF);
                    if (rd != Trust.uncertaintly)
                    {
                        rd = invertindex(index(rd) + sd(h, other, rd));
                        x[index(rd)] += weight(rtd(CurrentState, c, other));
                    }
                }
            }
            var max = ArgMax(x);
            return invertindex(max);
        }

        public double Quantitize(Trust t)
        {
            return (int)t;
        }
        public static int index(Trust t)
        {
            switch (t)
            {
                case Trust.vb:
                    return 0;
                case Trust.b:
                    return 1;
                case Trust.g:
                    return 2;
                case Trust.vg:
                    return 3;
                default:
                    return -1;
            }
        }
        public static Trust invertindex(int t)
        {
            switch (t)
            {
                case 0:
                    return Trust.vb;
                case 1:
                    return Trust.b;
                case 2:
                    return Trust.g;
                case 3:
                    return Trust.vg;
                default:
                    return Trust.uncertaintly;
            }
        }
    }
    public enum Trust
    {
        vb = 0,
        b = 10,
        uncertaintly = 20,
        g = 30,
        vg = 40
    }
}
