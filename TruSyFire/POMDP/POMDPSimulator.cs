using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;
using TruSyFire.TrustSystems.Environment;

namespace TruSyFire.POMDP
{
    class POMDPSimulator<State, Action> : ISimulator<Belief<State>, Action>
        where State : IState
        where Action : IAction
   {
        IObserveFunction<State> Observation { get; set; }
        MDP<State, Action> UnderlyingMDP { get; set; }
        public POMDPSimulator(ISimulator<State, Action> Simulator, IEnumerable<Action> Actions,IObserveFunction<State> observation, State StartState, int MaxEpoch)
        {
            UnderlyingMDP = new MDP<State, Action>(Simulator, Actions, StartState, MaxEpoch);
            Observation = observation;
            //UnderlyingMDP.GenerateAllStates();
        }

        public override SortedList<Belief<State>, double> GenerateStates(
            Belief<State> Current, Action SelectedAction)
        {
            if (Current.Epoch == UnderlyingMDP.MaxEpoch)
                return new SortedList<Belief<State>, double>();
            var NB = new SortedList<string, SortedList<State, double>>();
            var St = Current.GetAllStatesInBeliefVector();
            foreach (var s in St)
            {
                var NS = UnderlyingMDP.GetNextStates(s, SelectedAction);
                foreach (var ns in NS)
                {
                    var obs = Observation.GetObservation(ns.Key);
                    if (!NB.ContainsKey(obs))
                        NB[obs] = new SortedList<State, double>();
                    if (!NB[obs].ContainsKey(ns.Key))
                        NB[obs][ns.Key] = 0;
                    NB[obs][ns.Key] += Current.GetStateProbability(s) *
                        ns.Value;
                }
            }
            var Nexts = new SortedList<Belief<State>, double>();
            foreach (var obs in NB.Keys)
            {
                var sumP = NB[obs].Sum(i => i.Value);
                var b = Current.CreateNextBelief(
                   NB[obs].Select(d => new Distribution<State>()
                {
                    Object = d.Key,
                    Prob = d.Value / sumP
                }).ToArray());
                Nexts[b] = sumP;
            }
            return Nexts;

        }

        public override double GetImmediateReward(Belief<State> Source, Belief<State> Dest, Action SelectedAction)
        {
            var S1 = Source.GetAllStatesInBeliefVector();
            var S2 = Dest.GetAllStatesInBeliefVector();
            var R = 0.0;
            var sumP = S1.Sum(s1 =>
                Source.GetStateProbability(s1) *
                S2.Sum(s2 => UnderlyingMDP.GetNextStates(s1, SelectedAction).ContainsKey(s2)?
                                UnderlyingMDP.GetNextStates(s1, SelectedAction)[s2]:
                                0));
            foreach (var s1 in S1)
                foreach (var s2 in S2)
                    if(UnderlyingMDP.GetNextStates(s1, SelectedAction).ContainsKey(s2))
                    R += Source.GetStateProbability(s1) *
                        UnderlyingMDP.GetNextStates(s1, SelectedAction)[s2] / sumP *
                        UnderlyingMDP.ImmediateReward(s1, s2, SelectedAction);
            return R;
         }

        public override bool IsIsolated(Belief<State> current, Action SelectedAction)
        {
            return false;
        }

        //public double GetStateReward(Belief<State> State)
        //{
        //    var S1 = State.GetAllStatesInBeliefVector();
        //    var R = 0.0;
        //    foreach (var s1 in S1)
        //            R += State.GetStateProbability(s1) *
        //                UnderlyingMDP.StateReward(s1);
        //    return R;
        //}
    }
}
