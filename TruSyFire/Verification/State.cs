using TruSyFire.MDP;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.TrustSystems.Environment;

namespace TruSyFire.Verification
{
    public class State<THistory> : IState
        where THistory : IHistory
    {
        public TruSyFire.TrustSystems.Environment.Environment Environment { get; private set; }
        public State(TruSyFire.TrustSystems.Environment.Environment env, int epoch, params THistory[] hs)
        {
            Environment = env;
            Epoch = epoch;
            historyDB = hs.Select(h=>(THistory)h.Clone()).ToArray();
        }
        THistory[] historyDB;

        public THistory GetHistory(string sp, string c)
        {
            var hs = historyDB.FirstOrDefault(h =>
                    h.ServiceProvider.Name == sp && h.Client.Name == c);
            if (hs == null)
                return hs;
            return (THistory)hs.Clone();
        }
        public THistory GetHistory(TruSyFire.TrustSystems.Environment.IServiceProvider sp, IClient c)
        {
            var hs = historyDB.FirstOrDefault(h => 
                h.ServiceProvider == sp && h.Client == c);
            if(hs==null)
                return hs;
            return (THistory)hs.Clone();
        }
        public THistory[] GetAllHistories()
        {
            return historyDB.Select(h=>(THistory)h.Clone()).ToArray();
        }
        string _Index = string.Empty;
        public override string Index
        {
            get
            {
                if (string.IsNullOrEmpty(_Index))
                {
                    ComputeIndex();
                }
                return _Index;
            }
        }

        private void ComputeIndex()
        {
            _Index = "";
            foreach (var h in historyDB)
                _Index += h.Index + "|";
        }

        public override int GetHashCode()
        {
            ComputeIndex();
            return _Index.GetHashCode();
        }

        public  State<THistory> CreateNextState(THistory[] hs)
        {
            return new State<THistory>(Environment, Epoch  + 1, hs);
        }

    }
}
