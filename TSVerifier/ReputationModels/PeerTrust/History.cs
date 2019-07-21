using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.PeerTrust
{
    class History : IHistory
    {
        public SortedList<IEntity, int[]> S = new SortedList<IEntity, int[]>();
        public override void Update<History>(IEntity Client, QoS q, int epoch, State<History> State)
        {
            if (!S.ContainsKey(Client))
                S[Client] = new int[2];
            S[Client][0] += q == QoS.Good?1:0;
            S[Client][1]++;
        }

        public override IHistory Clone()
        {
            var h = new History()
            {
                Owner = Owner
            };
            foreach (var s in S)
            {
                h.S[s.Key] = new int[2];
                h.S[s.Key][0] = s.Value[0];
                h.S[s.Key][1] = s.Value[1];
            }
            return h;
        }

        public override string Index
        {
            get
            {
                var s = "";
                foreach (var key in S)
                {
                    s += key.Key + ":" + key.Value[0] + "/" + key.Value[1] + ",";
                }
                return s;
            }
        }

        public override void Reset()
        {
            S = new SortedList<IEntity, int[]>();
        }

        ////public SortedList<IEntity, int> I = new SortedList<IEntity, int>();
        //public double S { get; set; }
        //public override void Update<History>(IEntity Client, QoS q, int epoch, State<History> State)
        //{
        //    if(q== QoS.Good)
        //    {
        //        var state = State as State<PeerTrust.History>;
        //        var trust = new PeerTrustModel ();
        //        var sum=0.0;
        //        var hs = State.GetAllHistories();
        //        foreach(var h in hs)
        //            sum += trust.GetReputation(h.Owner, state);
        //        if (sum != 0)
        //            S += trust.GetReputation(Client, state) / sum;
        //        else
        //            S += 1.0 / (hs.Length+1);
        //    }
        //}

        //public override IHistory Clone()
        //{
        //    return new History()
        //    {
        //        S = S,
        //        Owner = Owner
        //    };
        //}

        //public override string Index
        //{
        //    get
        //    {
        //        return S.ToString();
        //    }
        //}

        //public override void Reset()
        //{
        //    S=0;
        //}
    }
}
