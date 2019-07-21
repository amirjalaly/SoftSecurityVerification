using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.Environment
{

    public abstract class IServiceProvider : IEntity
    {
        public IServiceProvider(string Name):base(Name){}
        public abstract Distribution<QoS>[] Behave<History, Trust, Rec>(IClient Client,
                ITCM<History, Trust, Rec> TCM, 
            State<History> CurrentState)
            where History : IHistory
            where Trust:IComparable;
        public abstract IServiceProvider Clone(string NewName);


    }

}
