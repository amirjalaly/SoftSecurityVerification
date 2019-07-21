using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Reputation;
using RepSyFire.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.ReputationModels.eBay
{
    class History:IHistory
    {
        public int SatisfactoryCounts { get; private set; }
        public override void Update<History>(IEntity Client, QoS q, int epoch, State<History> State)
        {
            SatisfactoryCounts += (int)q; 
        }

        public override IHistory Clone()
        {
            return new History() { SatisfactoryCounts = SatisfactoryCounts,Owner=Owner };
        }

        public override string Index
        {
            get { return SatisfactoryCounts.ToString(); }
        }

        public override void Reset()
        {
            SatisfactoryCounts = 0;
        }
    }
}
