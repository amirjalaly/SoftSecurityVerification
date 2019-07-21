using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;
using TruSyFire.TrustSystems.Environment;

namespace TruSyFire.POMDP
{
    class POMDP<State,Action>:MDP<Belief<State>, Action>
        where State : IState
        where Action : IAction
    {
        public IObserveFunction<State> ObserveFunction { get; private set; }
        public POMDP(ISimulator<State, Action> Simulator, 
            IEnumerable<Action> Actions,
            IObserveFunction<State> ObserveFunction,
            State StartState, int MaxEpoch)
            :base(
            new POMDPSimulator<State,Action>(Simulator,Actions,ObserveFunction,StartState,MaxEpoch),
            Actions,new Belief<State>(StartState.Epoch,new Distribution<State>[]{new Distribution<State>(){Prob=1,Object=StartState}}),
            MaxEpoch)
        {
            this.ObserveFunction = ObserveFunction;
        }
 
    }
}
