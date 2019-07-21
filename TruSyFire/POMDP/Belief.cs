using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;
using TruSyFire.TrustSystems.Environment;

namespace TruSyFire.POMDP
{
    public class Belief<State> : IState
        where State : IState
    {
        Distribution<State>[] BeliefVector { get; set; }
        public Belief(int epoch, Distribution<State>[] Beliefvector)
        {
            Epoch = epoch;
            BeliefVector = Beliefvector;
        }

        public State[] GetAllStatesInBeliefVector()
        {
            return
                BeliefVector.Select(d => d.Object).ToArray();
        }
        public double GetStateProbability(State s)
        {
            var b = BeliefVector.FirstOrDefault(d => d.Object == s);
            if (b == null)
                return 0;
            return b.Prob;
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
            foreach (var s in BeliefVector)
                _Index += "{" + s.Object.Index + ":" + s.Prob + "}";
        }
        public override int GetHashCode()
        {
            ComputeIndex();
            return _Index.GetHashCode();
        }
        public Belief<State> CreateNextBelief(Distribution<State>[] hs)
        {
            return new Belief<State>(Epoch + 1, hs);
        }

    }
}
