using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;

namespace TruSyFire.POMDP
{
    public interface IObserveFunction<State>
        where State:IState
    {
        string GetObservation(State st);
    }
}
