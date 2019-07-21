using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Console = TruSyFire.Auxiliary.Console;

namespace TruSyFire.MDP
{
    class MDP<State, Action>
        where State : IState
        where Action : IAction
    {
        public ISimulator<State, Action> Simul { get; private set; }
        public int MaxEpoch { get; private set; }
        public State StartState { get; private set; }

        public MDP(ISimulator<State, Action> Simulator, IEnumerable<Action> Actions, State StartState, int MaxEpoch)
        {

            Simul = Simulator;
            this.MaxEpoch = MaxEpoch;
            this.StartState = StartState;
            Values = new SortedList<State, double>
            [MaxEpoch + 1];
            NextStates =
            new SortedList<string, SortedList<State, SortedList<State, double>>>[MaxEpoch + 1];
            StatesImmediateReward = new SortedList<string, SortedList<State, SortedList<State, double>>>[MaxEpoch];
            //StatesReward = new SortedList<State, double>[MaxEpoch + 1];
            this.Actions = Actions;
        }

        SortedList<State, double>[] Values;
        public double GetStateValue(State st)
        {
            if (Values[st.Epoch].ContainsKey(st))
                return Values[st.Epoch][st];
            return 0;
        }
        public void SetStateValue(State st, double val)
        {
            Values[st.Epoch][st] = val;

        }

        public IEnumerable<State> GetAllStates(int epoch)
        {
            return Values[epoch].Keys;
        }
        SortedList<string, SortedList<State, SortedList<State, double>>>[] NextStates;
        public SortedList<State, double> GetNextStates(State State, Action Action, bool GenerateStatesIfNotAvalailable = true)
        {
            if (NextStates[State.Epoch] == null)
                NextStates[State.Epoch] = new SortedList<string, SortedList<State, SortedList<State, double>>>();
            string actIndex = Action.Index;
            if (!NextStates[State.Epoch].ContainsKey(actIndex))
            {
                NextStates[State.Epoch][actIndex] = new SortedList<State, SortedList<State, double>>();
            }
            if ((!NextStates[State.Epoch][actIndex].ContainsKey(State)))
                if (GenerateStatesIfNotAvalailable)
                {
                    var NextState =
                    Simul.GenerateStates(State, Action);
                    foreach (var st in NextState)
                        SetImmediateReward(State, st.Key, Action);
                    return NextStates[State.Epoch][actIndex][State] = NextState;
                }
                else
                    return null;


            return NextStates[State.Epoch][actIndex][State];
        }


        //private string GetIndex(Action Action)
        public void AddState(State State)
        {

            if (Values[State.Epoch] == null)
                Values[State.Epoch] = new SortedList<State, double>();
            if (!Values[State.Epoch].ContainsKey(State))
                Values[State.Epoch][State] = 0;
        }
        public IEnumerable<Action> Actions { get; private set; }
        public void GenerateAllStates()
        {
            var start = DateTime.Now;
            var last = start;
            var initial = StartState;
            Queue<State> Q = new Queue<State>();
            Q.Enqueue(initial);
            var cnt = 0;
            while (Q.Count > 0)
            {
                var State = Q.Dequeue();
                if (Values[State.Epoch] != null && Values[State.Epoch].ContainsKey(State))
                    continue;
                cnt++;
                if ((cnt % 100 == 0) || DateTime.Now.Subtract(last).TotalSeconds > 1)
                {
                    Console.InplaceWrite("States are being generated: Epoch {1} #{0} [{2}]...", cnt, State.Epoch,
                        DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss"));
                    last = DateTime.Now;
                }
                AddState(State);
                if (Simul.IsIsolated(State, Actions.First()))
                    continue;

                var AllAct = Actions;
                foreach (var act in AllAct)
                {

                    var NS = GetNextStates(State, act);
                    foreach (var ns in NS.Keys)
                        Q.Enqueue(ns);
                }
            }
            Console.WriteLine(@"States generation is completed in {1}. [States count: {0}#]", cnt,
               DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss"));


        }
        public void GenerateAllStates(IPlan<State, Action> Plan)
        {
            var start = DateTime.Now;
            var initial = StartState;
            Queue<State> Q = new Queue<State>();
            Q.Enqueue(initial);
            var cnt = 0;
            while (Q.Count > 0)
            {
                var State = Q.Dequeue();
                if (Values[State.Epoch] != null && Values[State.Epoch].ContainsKey(State))
                    continue;
                cnt++;
                if (cnt % 50 == 0)
                {
                    Console.InplaceWrite("States are generating: Epoch {1} #{0} [{2}]...", cnt, State.Epoch,
                        DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss"));
                }
                AddState(State);

                var NS = GetNextStates(State, Plan.GetAction(State));
                foreach (var ns in NS.Keys)
                    Q.Enqueue(ns);

            }
            Console.WriteLine(@"States generation is completed in {1}. [States count: {0}#]", cnt,
                DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss"));


        }
        SortedList<string, SortedList<State, SortedList<State, double>>>[] StatesImmediateReward;
        public double ImmediateReward(State Source, State Destin, Action act)
        {
            string actIndex = act.Index;
            SetImmediateReward(Source, Destin, act);
            return StatesImmediateReward[Source.Epoch][actIndex][Source][Destin];

        }

        private void SetImmediateReward(State Source, State Destin, Action act)
        {
            if (StatesImmediateReward[Source.Epoch] == null)
                StatesImmediateReward[Source.Epoch] = new SortedList<string, SortedList<State, SortedList<State, double>>>();
            string actIndex = act.Index;
            if (!StatesImmediateReward[Source.Epoch].ContainsKey(actIndex))
            {
                StatesImmediateReward[Source.Epoch][actIndex] = new SortedList<State, SortedList<State, double>>();
            }
            if (!StatesImmediateReward[Source.Epoch][actIndex].ContainsKey(Source))
            {
                StatesImmediateReward[Source.Epoch][actIndex][Source] = new SortedList<State, double>();
            }
            if (!StatesImmediateReward[Source.Epoch][actIndex][Source].ContainsKey(Destin))
            {
                var r = Simul.GetImmediateReward(Source, Destin, act);
                StatesImmediateReward[Source.Epoch][actIndex][Source][Destin] = r;
            }
        }
        //SortedList<State, double>[] StatesReward;

        //public double StateReward(State state)
        //{
        //    if (StatesReward[state.Epoch] == null)
        //        StatesReward[state.Epoch] = new SortedList<State, double>();
        //    if (!StatesReward[state.Epoch].ContainsKey(state))
        //    {
        //        var r = Simul.GetStateReward(state);
        //        StatesReward[state.Epoch][state] = r;
        //    }
        //    return StatesReward[state.Epoch][state];
        //}
    }
}
