using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;

namespace TruSyFire.TrustSystems.Environment.HonestEntities
{
    public class GreedyClient:IClient
    {
        public GreedyClient(string Name):base(Name){}
        public override Distribution<IServiceProvider>[]            
            SelectServiceProvider<History, Trust,Rec>
            (IServiceProvider[] Entities, 
                ITCM<History, Trust,Rec> TCM, 
                State<History> State)

        {

            var Reps = Entities.Select(e => 
                new KeyValuePair<IServiceProvider, double>(e, TCM.Quantitize(
                    TCM.GetTrust(e,this, State))))
                .ToArray();
            var maxRep = Reps.Max(kv => kv.Value);
            var maxies = Reps.Where(kv => kv.Value == maxRep);
            return maxies.Select(kv =>
                new Distribution<IServiceProvider>() 
                { Object = kv.Key, Prob = 1.0 / maxies.Count() })
                .ToArray();
        }


        public override QoS Rate(QoS qos, IServiceProvider sp)
        {
            return qos;
        }

        public override IClient Clone(string NewName)
        {
            return new GreedyClient(NewName);
        }

        public override string EntityType
        {
            get { return "Greedy"; }
        }
    }

       public class ProbabilisticClient:IClient
    {
        public ProbabilisticClient(string Name):base(Name){}
                   public const double Eps = 0.01;
        public double IgnoranceProb = .0;

        public override Distribution<IServiceProvider>[]            
            SelectServiceProvider<History, Trust,Rec>
            (IServiceProvider[] Entities, 
                ITCM<History, Trust,Rec> TCM, 
                State<History> State)

        {

            var trustvector = Entities.Select(sp => 
                TCM.Quantitize(
                TCM.GetTrust(sp,this, State)))
                .ToArray();
            //var min = allvalues.Min();
            var SumRep = trustvector.Sum(r => r + Eps);

            var SL = Entities.Select(e => new Distribution<IServiceProvider>()
            {
                Object = e,
                Prob = (TCM.Quantitize(TCM.GetTrust(e, this, State)) + Eps) / SumRep
            });
            if(SL.Count(v=>v.Prob<0)>0)
                return SL.ToArray();


            return SL.ToArray();
            var OSL = SL.Where(sl => sl.Prob >= IgnoranceProb).ToArray();

            if (OSL.Count() != SL.Count())
                return OSL;
            return OSL;

        }

        public override QoS Rate(QoS qos, IServiceProvider sp)
        {
            return qos;
        }

        public override IClient Clone(string NewName)
        {
            return new ProbabilisticClient(NewName);
        }

        public override string EntityType
        {
            get { return "ProbabilisticClient"; }
        }
    }


       public class RiskTakingClient : IClient
       {
                   public double RiskFactor { get;private set; }

                   public RiskTakingClient(string Name, double riskFactor) : base(Name) {
                       RiskFactor = riskFactor;
                   }

                   public override Distribution<IServiceProvider>[]
               SelectServiceProvider<History, Trust, Rec>(IServiceProvider[] Entities,
                ITCM<History, Trust, Rec> TCM,
                State<History> State)
           {

               var allrep = Entities.Select(sp => TCM.GetTrust(sp, this, State)).ToArray();
               var allw = allrep.Select(r => TCM.Quantitize(r)).ToArray();
               var min = 0;// allw.Min();
               var max = allw.Max();
               var interval = max == min ? 1 : (max - min);
               var norms = allw.Select(w => ((w - min) / (interval))).ToArray();
               var sum = norms.Sum(n => Math.Exp(n / RiskFactor));



               return Entities.Select(e => new Distribution<IServiceProvider>()
               {
                   Object = e,
                   Prob =
                       Math.Exp(
                           (TCM.Quantitize(TCM.GetTrust(e, this, State)) - min)
                           / (interval * RiskFactor)
                           )
                       / sum
               }).ToArray();




           }

           public override QoS Rate(QoS qos, IServiceProvider sp)
           {
               return qos;
           }

           public override IClient Clone(string NewName)
           {
               return new RiskTakingClient(NewName, RiskFactor);
           }

           public override string EntityType
           {
               get { return "RiskTaking("+RiskFactor+")"; }
           }
       }

    /// <summary>
    /// This client decrease its risk taking during the time
    /// </summary>
       public class ProbToGreedyClient : IClient
       {

           public ProbToGreedyClient(string Name)
               : base(Name)
           {
           }

           public override Distribution<IServiceProvider>[]
       SelectServiceProvider<History, Trust, Rec>(IServiceProvider[] Entities,
        ITCM<History, Trust, Rec> TCM,
        State<History> State)
           {
               var RiskFactor = 1.0 / (State.Epoch+1);
               var allrep = Entities.Select(sp =>
                    new Distribution<IServiceProvider>()
               {
                   Object = sp,
                   Prob = TCM.Quantitize(TCM.GetTrust(sp, this, State))
                }).ToArray();
               if (allrep[1].Prob != 0.5)
                   allrep = allrep;
               var min = 0;// allw.Min();
               var max = allrep.Max(sp=>sp.Prob);
               var interval = max == min ? 1 : (max - min);
               //var norms = allrep.Select(w => 
               //    {w.Prob = Math.Exp(((w.Prob - min) / (interval))/RiskFactor);return w;}).ToArray();
            var norms = allrep.Select(w =>
            { w.Prob = Math.Exp(w.Prob * State.Epoch); return w; }).ToArray();

            //var probs = norms.Select(n => Math.Exp(n / RiskFactor))
            //    .Where(p=>p>=0.01);
            var sum = norms.Sum(n => n.Prob);
               var probs = norms.Select(sp => { sp.Prob = sp.Prob / sum; return sp; }).ToArray();
               return probs.ToArray();
               probs = probs.Where(sp => sp.Prob >= 0.1).ToArray();
               var newsum = probs.Sum(sp=>sp.Prob);
               var newprobs = probs.Select(sp=>{sp.Prob=sp.Prob/newsum;return sp;});

               if (norms.Count() != newprobs.Count())
                   probs  = probs;



               return newprobs.ToArray();




           }

           public override QoS Rate(QoS qos, IServiceProvider sp)
           {
               return qos;
           }

           public override IClient Clone(string NewName)
           {
               return new ProbToGreedyClient(NewName);
           }

           public override string EntityType
           {
               get { return "ProbToGreedyClient"; }
           }
       }
   }



