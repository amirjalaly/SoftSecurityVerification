using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TSsBenchmarking.TrustSystems.Abdul_Rahman
{
    public class History : TruSyFire.TrustSystems.TrustComputationModel.IHistory
    {
        public byte[] S = new byte[4];
        public SortedList<string, byte[]>[] T = new SortedList<string, byte[]>[4];

        public override void Update<History>(TruSyFire.TrustSystems.TrustComputationModel.QoS q, TruSyFire.Verification.State<History> State)
        {
            //if (q == QoS.NoService)
            //    return;
            Trust t = q == QoS.Good ? Trust.vg : Trust.vb;
            S[TCM.index(t)]++;

            foreach (var c in State.Environment.Clients)
            {
                if (c != Client && c != ServiceProvider)
                {
                    var r = c.GetRecommendation
                    <RecommendationFunction,
                    TSsBenchmarking.TrustSystems.Abdul_Rahman.History,
                    Trust>
                    (ServiceProvider,
                    (State<TSsBenchmarking.TrustSystems.Abdul_Rahman.History>)(object)State,
                    new RecommendationFunction());
                    if (r == Trust.uncertaintly)
                        continue;
                    if (T[TCM.index(r)] == null)
                        T[TCM.index(r)] = new SortedList<string, byte[]>();
                    if (!T[TCM.index(r)].ContainsKey(c.Name))
                        T[TCM.index(r)][c.Name] = new byte[7];
                    T[TCM.index(r)][c.Name][TCM.index(t) - TCM.index(r) + 3]++;
                }
            }

        }

        public override TruSyFire.TrustSystems.TrustComputationModel.IHistory Clone()
        {
            var s = new byte[4];
            var t = new SortedList<string, byte[]>[4];
            for (int i = 0; i < 4; i++)
            {
                s[i] = S[i];
                if (T[i] != null)
                {
                    t[i] = new SortedList<string, byte[]>();
                    foreach (var tt in T[i])
                        t[i][tt.Key] = (byte[])tt.Value.Clone();
                }
            }

            return new History()
            {
                Client = Client,
                ServiceProvider = ServiceProvider,
                S = s,
                T = t
            };
        }

        public override string Index
        {
            get { 
                var str="(";
                foreach (var s in S)
                    str += s + ",";
                str += ")";
                for (int i = 0; i < 4; i++)
                {
                    str+="(";
                    if (T[i] != null)
                    {
                        foreach (var t in T[i])
                        {
                            str += t.Key + ":[";
                            foreach (var v in t.Value)
                                str += v + ",";
                            str += "]";
                        }
                    }
                    str += ")";
                }
                return str;
            }
        }

        public override void Reset()
        {
            S = new byte[4];
            T = new SortedList<string, byte[]>[4];
        }
    }
}
