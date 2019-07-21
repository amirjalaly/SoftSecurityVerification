using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.Beta
{
    class History:IHistory
    {
        public double Sat { get; set; }
        public double UnSat { get; set; }
        public const double ForgetingFactor = 0.9;
        public override void Update<History>(IEntity Client, QoS q, int epoch, State<History> State)

        {
            Sat *= 0.9;
            UnSat *= 0.9;
            if (q == QoS.Good)
                Sat++;
            else
                UnSat++;
        }

        public override IHistory Clone()
        {
            return new History()
            {
                Owner = Owner,
                Sat = Sat,
                UnSat = UnSat
            };
        }

        public override string Index
        {
            get { return string.Format("{0},{1}",Sat,UnSat); }
        }

        public override void Reset()
        {
            Sat = UnSat = 0;
        }
    }
}
