using Console = TruSyFire.Auxiliary.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;
using TruSyFire.TrustSystems.Environment;

namespace TruSyFire.POMDP
{
    class POMDPEstimator<State, Action>
        where State : IState
        where Action : IAction
    {
        public IObserveFunction<State> ObserveFunction { get; private set; }
        MDP<State, Action> UnderlyingMDP;
        public Belief<State> StartState { get; private set; }
        POMDPSimulator<State, Action> Simulator;
        public POMDPEstimator(MDP<State, Action> MDP,
            IObserveFunction<State> ObserveFunction)
        {
            this.ObserveFunction = ObserveFunction;
            UnderlyingMDP = MDP;
            this.StartState = new Belief<State>(MDP.StartState.Epoch,
                new Distribution<State>[] { new Distribution<State>() 
                { Prob = 1, Object = MDP.StartState } });
            Simulator = new POMDPSimulator<State, Action>
                (MDP.Simul, MDP.Actions, ObserveFunction, MDP.StartState, MDP.MaxEpoch);
            BeliefValues = new SortedList<Belief<State>, double>
                    [MDP.MaxEpoch + 1];
        }


        public MDPPlan<Belief<State>, Action> EstimatePOMDP(
            out double Value)
        {
            plan = new MDPPlan<Belief<State>, Action>();
            Console.InplaceWrite("Start POMDP Estimating <QMDP>...");
            var start = DateTime.Now;
            Value = GetBeliefValue(StartState);
            Console.WriteLine(@"POMDP Estimating <QMDP> is done in {1}.
                Best Value is           {0:00.00}.",
                Value,
                DateTime.Now.Subtract(start));
            return plan;

        }


        SortedList<Belief<State>, double>[] BeliefValues;
        public double GetBeliefValue(Belief<State> b)
        {
            if (b.Epoch >= UnderlyingMDP.MaxEpoch)
                return 0;
            if (BeliefValues[b.Epoch] == null)
                BeliefValues[b.Epoch] = new SortedList<Belief<State>, double>();
            if (!BeliefValues[b.Epoch].ContainsKey(b))
            {
                var act = BestAction(b);
                var nexts = Simulator.GenerateStates(b, act);
                var value = 0.0;
                foreach (var nb in nexts.Keys)
                    value +=
                        nexts[nb] * (
                            Simulator.GetImmediateReward(b, nb, act) +
                            GetBeliefValue(nb));
                BeliefValues[b.Epoch][b] = value;
            }
            return BeliefValues[b.Epoch][b];
        }

        MDPPlan<Belief<State>, Action> plan = new MDPPlan<Belief<State>, Action>();

        private Action BestAction(Belief<State> B)
        {
            if (plan.GetAction(B) != null)
                return plan.GetAction(B);
            Action BEst = null;
            var BestVal = double.NegativeInfinity;

            #region Greedy Policy
            foreach (var act in UnderlyingMDP.Actions)
            {
                var cumrew = 0.0;
                foreach (var st in B.GetAllStatesInBeliefVector())
                {
                    var nextst = UnderlyingMDP.GetNextStates(st, act);
                    foreach (var nst in nextst.Keys)
                    {
                        cumrew += B.GetStateProbability(st) *
                            nextst[nst] * (
                                UnderlyingMDP.ImmediateReward(st, nst, act)
                                +
                                UnderlyingMDP.GetStateValue(nst));
                    }
                    if (cumrew > BestVal)
                    {
                        BestVal = cumrew;
                        BEst = act;
                    }

                }

            }
            #endregion

            plan.SetAction(B, BEst);
            return BEst;
        }
    }
}
