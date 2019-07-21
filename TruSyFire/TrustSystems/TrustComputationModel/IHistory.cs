using TruSyFire.TrustSystems.Environment;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.TrustComputationModel
{
    public abstract class IHistory
    {
        public TruSyFire.TrustSystems.Environment.IServiceProvider ServiceProvider { get; set; }
        public IClient Client { get; set; }
        public abstract void Update<History>(QoS q, State<History> State)
            where History : IHistory;
        public abstract IHistory Clone();
        public abstract string Index { get; }
        public abstract void Reset();

        public static bool operator ==(IHistory p1, IHistory p2)
        {
            if ((object)p1 == null && (object)p2 == null)
                return true;
            return (object)p1 != null && (object)p2 != null && (p1.Index == p2.Index);
        }
        public static bool operator !=(IHistory p1, IHistory p2)
        {
            return !(p1 == p2);
        }


    }
}
