using System;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.Environment.HonestEntities
{
    public class HonestSP : IServiceProvider
    {
        public HonestSP(string name) : base(name) { }

        public override Distribution<QoS>[] Behave<History, Trust, Rec>(IClient Client,
                ITCM<History, Trust, Rec> TCM,
            State<History> CurrentState)
        {
            return new Distribution<QoS>[] 
            { new Distribution<QoS>() { Object = QoS.Good, Prob = 1 }
            };
        }

        public override IServiceProvider Clone(string NewName)
        {
            return new HonestSP(NewName);
        }

        public override string EntityType
        {
            get { return "HonestSP"; }
        }
    }
    public class IsolatingSP<Trust> : IServiceProvider
        where Trust : IComparable
    {
        public Trust TresholdToIsolate { get; private set; }
        public IsolatingSP(string name, Trust Threshold)
            : base(name)
        { TresholdToIsolate = Threshold; }

        public override Distribution<QoS>[] Behave<History, Trust, Rec>
            (IClient Client, ITCM<History, Trust, Rec> TCM, State<History> CurrentState)
        {
            if (
                TCM.GetTrust(new HonestSP(Client.Name),
                new GreedyClient(Name),
                CurrentState).CompareTo(TresholdToIsolate) < 0)
                return new Distribution<QoS>[] 
            { new Distribution<QoS>() { Object = QoS.NoService, Prob = 1 }
            };
            return new Distribution<QoS>[] 
            { new Distribution<QoS>() { Object = QoS.Good, Prob = 1 }
            };
        }

        public override IServiceProvider Clone(string NewName)
        {
            return new IsolatingSP<Trust>(NewName, TresholdToIsolate);
        }

        public override string EntityType
        {
            get { return "IsolatingSP[˂" + TresholdToIsolate + "]"; }
        }
    }
    public class FaultyServiceProvider : IServiceProvider
    {
        public double FaultRatio { get; set; }
        public FaultyServiceProvider(string name, double faultratio)
            : base(name)
        {
            FaultRatio = faultratio;
        }

        public override Distribution<QoS>[] Behave<History, Trust, Rec>(IClient Client, ITCM<History, Trust, Rec> TCM, State<History> CurrentState)
        {
            if (FaultRatio == 0)
                return new Distribution<QoS>[] 
            { new Distribution<QoS>() { Object = QoS.Good, Prob = 1 }
            };
            if (FaultRatio == 1)
                return new Distribution<QoS>[] 
            { new Distribution<QoS>() { Object = QoS.Bad, Prob = 1 }
            };
            return new Distribution<QoS>[] 
            { new Distribution<QoS>() { Object = QoS.Good, Prob = 1-FaultRatio },
            new Distribution<QoS>() { Object = QoS.Bad, Prob = FaultRatio }
            };
        }

        public override IServiceProvider Clone(string NewName)
        {
            return new FaultyServiceProvider(NewName, FaultRatio);
        }

        public override string EntityType
        {
            get { return "FaultySP(" + FaultRatio + ")"; }
        }
    }
}
