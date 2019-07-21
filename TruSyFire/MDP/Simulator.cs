using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruSyFire.MDP
{
    abstract class ISimulator<State,Action>
    {
        public abstract SortedList<State, double> GenerateStates(State Current, Action SelectedAction);
        public abstract double GetImmediateReward(State Source, State Dest, Action SelectedAction);
        //public abstract double GetStateReward(State State, Action SelectedAction);
        public abstract bool IsIsolated(
State current,
Action SelectedAction);

    }

}
