using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Console = TruSyFire.Auxiliary.Console;

namespace TruSyFire.MDP
{
    class MDPSolver<State, Action>
        where State : IState
        where Action : IAction
    {
        protected MDP<State, Action> MDP { get; private set; }
        public MDPSolver(MDP<State, Action> mdp)
        {
            MDP = mdp;
        }

        public MDPPlan<State, Action> SolveMDP(int MaxEpoch, out double Value)
        {
            MDPPlan<State, Action> plan = new MDPPlan<State, Action>();
            Console.InplaceWrite("Start MDP Solving <Value Itteration> for MaxEpoch {0}...", MaxEpoch);
            var start = DateTime.Now;
            var states = MDP.GetAllStates(MaxEpoch).ToArray();
            foreach (var st in states)
            {
                MDP.SetStateValue(st, 0);
            }
            Console.InplaceWrite("MDP Solving <Value Itteration>: Epoch {0} done. [#{1} States]...", MaxEpoch, states.Length);
            for (int i = MaxEpoch - 1; i >= 0; i--)
            {
                states = MDP.GetAllStates(i).ToArray();
                foreach (var st in states)
                {
                    double BestVal;
                    plan.SetAction(st, BestAction(st, out BestVal));
                    MDP.SetStateValue(st, BestVal);
                }
                Console.InplaceWrite("MDP Solving <Value Itteration>: Epoch {0} done. [#{1} States]...", i, states.Length);
            }
            Value = MDP.GetStateValue(MDP.StartState);
            Console.WriteLine(@"MDP Solving <Value Itteration> is done in {1}.
                Best Value is           {0:00.00}.",
    Value,
    DateTime.Now.Subtract(start), MaxEpoch);
            return plan;

        }

        public double SolveMDP(IPlan<State, Action> Plan, int MaxEpoch)
        {
            Console.InplaceWrite("Start MDP Solving for Plan<{1}> for MaxEpoch {0}...", MaxEpoch, Plan.Title);
            var start = DateTime.Now;
            var states = MDP.GetAllStates(MaxEpoch).ToArray();
            foreach (var st in states)
            {
                MDP.SetStateValue(st, 0);
            }
            Console.InplaceWrite("MDP Solving for Plan<{2}>: Epoch {0} done. [#{1} States]...", MaxEpoch, states.Length, Plan.Title);
            for (int i = MaxEpoch - 1; i >= 0; i--)
            {
                states = MDP.GetAllStates(i).ToArray();
                foreach (var st in states)
                {
                    double BestVal =
                    GetStateValue(st, Plan);
                    MDP.SetStateValue(st, BestVal);
                }
                Console.InplaceWrite("MDP Solving for Plan<{2}>: Epoch {0} done. [#{1} States]...", i, states.Length, Plan.Title);
            }
            Console.WriteLine(@"MDP Solving for Plan<{3}> is done in {1}. 
                The Value is            {0:00.00}.",
MDP.GetStateValue(MDP.StartState),
DateTime.Now.Subtract(start), MaxEpoch, Plan.Title);
            return MDP.GetStateValue(MDP.StartState);


        }


        double GetStateValue(State st, IPlan<State, Action> Plan)
        {


            var act = Plan.GetAction(st);
            var cumrew = 0.0;
            var nextst = MDP.GetNextStates(st, act,false);
            if(nextst!=null)
            foreach (var nst in nextst.Keys)
            {
                cumrew += nextst[nst] * (
                    MDP.ImmediateReward(st, nst, act) + MDP.GetStateValue(nst));
            }
            return cumrew;


        }

        private Action BestAction(State st, out double BestVal)
        {
            Action BEst = null;
            BestVal = double.NegativeInfinity;

            #region Greedy Policy
            foreach (var act in MDP.Actions)
            {
                var cumrew = 0.0;
                var nextst = MDP.GetNextStates(st, act, false);
                if (nextst != null)
                    foreach (var nst in nextst.Keys)
                    {
                        cumrew += nextst[nst] * (
                            MDP.ImmediateReward(st, nst, act) + MDP.GetStateValue(nst));
                    }
                if (cumrew > BestVal)
                {
                    BestVal = cumrew;
                    BEst = act;
                }
            }
            #endregion
            return BEst;
        }

        //public void SolveMDP_ItterateOnMaxEpoch()
        //{
        //    for (int i = 1; i <= MDP.MaxEpoch; i++)
        //        SolveMDP(i);
        //}


    }
}
