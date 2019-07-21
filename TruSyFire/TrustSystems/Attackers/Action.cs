using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.MDP;

namespace TruSyFire.TrustSystems.Attackers
{
    public class Action:IAction
    {
        public Action(IAtomicAction[] Actions)
        {
            this.Actions = Actions;
        }
        IAtomicAction[] Actions { get; set; }
        public IAtomicAction this[int i]
        {
            get
            {
                return Actions[i];
            }
        }
        public IAtomicAction[] GetActionVector()
        {
            return (IAtomicAction[])Actions.Clone();
        }
        string index = string.Empty;

        public override string Index
        {
            get
            {
                if (string.IsNullOrEmpty(index))
                {
                    foreach (var a in Actions)
                        index += a.ToString() + ",";
                }
                return index;
            }

        }
        public override string ToString()
        {
            return Index;
        }
    }
}
