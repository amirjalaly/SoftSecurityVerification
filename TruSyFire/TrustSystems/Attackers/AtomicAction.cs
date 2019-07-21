using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TruSyFire.TrustSystems.Attackers
{
    public interface IAtomicAction
    {
    }
    public class ServiceProviderAtomicAction : IAtomicAction
    {
        public virtual ServiceProvidersActionType Type { get { return ServiceProvidersActionType.Nop; } }
        public override string ToString()
        {
            return "N";
        }
    }
    public class SPHonestAction : ServiceProviderAtomicAction
    {
        public SPHonestAction(TruSyFire.TrustSystems.Environment.IServiceProvider honest)
        {
            HonestEntity = honest;
        }
        public TruSyFire.TrustSystems.Environment.IServiceProvider HonestEntity { get; private set; }
        public override ServiceProvidersActionType Type
        {
            get
            {
                return ServiceProvidersActionType.Honest;
            }
        }
        public override string ToString()
        {
            return "H";
        }

    }

    public class ServiceAction : ServiceProviderAtomicAction
    {
        public override ServiceProvidersActionType Type
        {
            get
            {
                return ServiceProvidersActionType.Service;
            }
        }
        public QoS Quality { get; set; }
        public override string ToString()
        {
            return "S["+Quality+"]";
        }
    }
    public class ReEntryAction : ServiceProviderAtomicAction
    {
        public override ServiceProvidersActionType Type
        {
            get
            {
                return ServiceProvidersActionType.ReEntry;
            }
        }
        public override string ToString()
        {
            return "E";
        }

    }
    public class SPNopAction : ServiceProviderAtomicAction
    {
        public override ServiceProvidersActionType Type
        {
            get
            {
                return ServiceProvidersActionType.Nop;
            }
        }
        public override string ToString()
        {
            return "N";
        }

    }
    public abstract class ClientAtomicAction : IAtomicAction
    {
        public abstract ClientsActionType Type { get; }
        public SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object> Recommendation =
            new SortedList<TruSyFire.TrustSystems.Environment.IServiceProvider, object>();
        public override string ToString()
        {
            var str = "";
            foreach (var r in Recommendation)
                str += r.Key + ":" + r.Value + ",";
            return str;
        }
        public abstract ClientAtomicAction Clone();
    }
    public class ClientHonestAction : ClientAtomicAction
    {
        public ClientHonestAction(TruSyFire.TrustSystems.Environment.IClient honest)
        {
            HonestEntity = honest;
        }
        public TruSyFire.TrustSystems.Environment.IClient HonestEntity { get; private set; }

        public override ClientsActionType Type
        {
            get
            {
                return ClientsActionType.Honest;
            }
        }
        public override string ToString()
        {
            return "H";
        }
        public override ClientAtomicAction Clone()
        {
            return new ClientHonestAction(HonestEntity);
        }

    }
    public class ClientNopAction : ClientAtomicAction
    {
        public override ClientsActionType Type
        {
            get
            {
                return ClientsActionType.Nop;
            }
        }
        public override string ToString()
        {
            return "N "+base.ToString();
        }
        public override ClientAtomicAction Clone()
        {
            return new ClientNopAction()
                {
                    Recommendation = new SortedList<Environment.IServiceProvider, object>(Recommendation)
                };
        }

    }
    public class RequestAction : ClientAtomicAction
    {

        public TruSyFire.TrustSystems.Environment.IServiceProvider Object { get; set; }

        public override ClientsActionType Type
        {
            get
            {
                return ClientsActionType.Request;
            }
        }
        public override string ToString()
        {

            return "R["+Object.Name+"] "+base.ToString();
        }
        public override ClientAtomicAction Clone()
        {
            return new RequestAction()
                {
                    Object = Object,
                    Recommendation = new SortedList<Environment.IServiceProvider, object>(Recommendation)
                } ;
        }
    }
    //public class PromotingAction : DishonestRatingAction
    //{
    //    public override ClientsActionType Type
    //    {
    //        get
    //        {
    //            return ClientsActionType.Promoting;
    //        }
    //    }
    //    public override string ToString()
    //    {
    //        return "P[" + Object.Name + "] " + base.ToString();
    //    }
    //    public override ClientAtomicAction Clone()
    //    {
    //        return new UnfairRatingAction()
    //        {
    //            Object = Object,
    //            Recommendation = new SortedList<Environment.IServiceProvider, object>(Recommendation)
    //        };
    //    }
    //}
    public enum ServiceProvidersActionType
    {
        Service,
        ReEntry,
        Honest,
        Nop
    }
    public enum ClientsActionType
    {
        Request,
        Honest,
        Nop
    }
}
