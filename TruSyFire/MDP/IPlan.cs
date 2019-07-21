using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruSyFire.MDP
{
    interface IPlan<State,Action>
    {
        string Title { get; }
        Action GetAction(State State);
    }

    class MDPPlan<State, Action> : IPlan<State, Action>
    {
        public string Title { get { return "Worst Plan"; } }
        SortedList<State, Action> Actions = new SortedList<State, Action>();
        public IEnumerable<State> GetListOfStates()
        {
            return Actions.Keys;
        }
        public void SetAction(State st, Action act)
        {
            Actions[st] = act;
        }


        public Action GetAction(State State)
        {
            if (Actions.ContainsKey(State))
                return Actions[State];
            return default(Action);
        }
    }
}
