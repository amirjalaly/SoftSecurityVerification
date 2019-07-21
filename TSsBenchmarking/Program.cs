#define SL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.Environment.HonestEntities;
using TruSyFire.Verification;
using TSsBenchmarking.AttackPlan;
using SP = TruSyFire.TrustSystems.Environment.HonestEntities.HonestSP;
using Client = TruSyFire.TrustSystems.Environment.HonestEntities.ProbToGreedyClient;
//using Client = TruSyFire.TrustSystems.Environment.HonestEntities.ProbabilisticClient;
#region TCM NSs
#if AHR
using TSsBenchmarking.TrustSystems.Abdul_Rahman;
using Trust = TSsBenchmarking.TrustSystems.Abdul_Rahman.Trust;
using Rec = TSsBenchmarking.TrustSystems.Abdul_Rahman.Trust;
#endif
#if BL
using TSsBenchmarking.TrustSystems.Blind;
using Trust = System.Double;
using Rec = System.Double;
#endif
#if RG
using TSsBenchmarking.TrustSystems.Regret;
using Trust = System.Double;
using Rec = System.Double;
#endif
#if N
using TSsBenchmarking.TrustSystems.Naive;
using Trust = System.Double;
using Rec = System.Int32;
#endif
#if PT
using TSsBenchmarking.TrustSystems.PeerTrust;
using Trust = System.Double;
using Rec = System.Double;
using TruSyFire.TrustSystems.Environment;
#endif
#if SL
using TSsBenchmarking.TrustSystems.SubjectiveLogic;
using Trust = TSsBenchmarking.TrustSystems.SubjectiveLogic.Trust;
using Rec = TSsBenchmarking.TrustSystems.SubjectiveLogic.Trust;
#endif
#if TR
using TSsBenchmarking.TrustSystems.TRAVOS;
using Rec = TSsBenchmarking.TrustSystems.TRAVOS.Recommendation;
using Trust = System.Double;
#endif
#endregion

namespace TSsBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            WorstSelfishTest();
        }

        private static void FaultyHonestProvider()
        {
#if PT
            const int Maxepoch = 8;
#else
#if SL || RG
            const int Maxepoch = 7;
#else
            const int Maxepoch = 6;
#endif
#endif
            for (double faultratio = 0; faultratio <= 1; faultratio += .1)
            {
                var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
               (new FaultyServiceProvider("", faultratio),
                    new Client(""),
                    3, 3, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, Maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);

                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

        private static void WorstSlandererTest()
        {
            var R = new Rewards(0, 1, 0, 0, 0, 0, 0, 0);
            MotivationTest(R, new SP(""));
        }
        private static void WorstCompatitorTest()
        {
            var R = new Rewards(1, 0, 0, 0, 0, 0, 0, 0);
            MotivationTest(R, new SP(""));
        }
        private static void WorstMaliciousTest()
        {
            var R = new Rewards(1, 0, 0, 0, 1, 0, 0, 0);
            MotivationTest(R, new SP(""));
        }
        private static void WorstSelfishTest()
        {
            var R = new Rewards(0, 0, 0, 0, 1, 0, 1, 0);
            var sp = new IsolatingSP<Trust>("",
#if AHR
                       Rec.uncertaintly
#endif
#if PT
 .5
#endif
#if RG
   0
#endif
#if N
   0
#endif
#if SL
                     new Trust(0,0,1)
#endif
#if TR
                        .5
#endif
#if BL
 .5
#endif
);
            MotivationTest(R, sp);

        }

        private static void MotivationTest(Rewards R, TruSyFire.TrustSystems.Environment.IServiceProvider HonestSP)
        {

#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
#if BL
            const int Maxepoch = 5;
#else
            const int Maxepoch = 10;
#endif
#endif
#endif
            for (int i = 5; i <= Maxepoch; i++)
            {
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
               (HonestSP,

                    new Client(""),
                    2, 2, 1, 1, true);
                //var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, i);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }
        private static void BehaviorFunctiontest()
        {
#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
            const int Maxepoch = 7;
#endif
#endif
            for (double i = 0; i <= 10; i++)
            {
                var R = new Rewards(1, 0, 0, 0, 1, 0, 0, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
               (new IsolatingSP<Trust>("",
#if AHR
                       (Rec)((int)(i*.4)*10) //AHR
#endif
#if PT
                        i*.1
#endif
#if BL
 i * .1
#endif
#if RG
                            -1+i*.2 //REGRET
#endif
#if SL
                     new Trust(i*.1,1-i*.1,0)
#endif
#if TR
                        i*0.1
#endif
#if N
   0
#endif
   ),

                    new Client(""),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, Maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }
        private static void SelfishPropToGreedyTest()
        {
#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
            const int Maxepoch = 7;
#endif
#endif
            var R = new Rewards(0, 0, 0, 0, 1, 0, 1, 0);

            {
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new IsolatingSP<Trust>("",
#if AHR
                       Rec.uncertaintly
#endif
#if PT
 .5
#endif
#if RG
   0
#endif
#if N
   0
#endif
#if SL
                     new Trust(0,0,1)
#endif
#if TR
                        .5
#endif
#if BL
 .5
#endif
),
                    new ProbToGreedyClient(""),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, Maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }
        private static void SelfishRiskTakingtest()
        {
#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
            const int Maxepoch = 7;
#endif
#endif
            var R = new Rewards(0, 0, 0, 0, 1, 0, 1, 0);

            for (double i = -2; i <= 2; i += .2)
            {
                var risk = Math.Pow(10, i);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new IsolatingSP<Trust>("",
#if AHR
                       Rec.uncertaintly
#endif
#if PT
 .5
#endif
#if RG
   0
#endif
#if SL
                     new Trust(0,0,1)
#endif
#if TR
                        .5
#endif
#if BL
 .5
#endif
#if N
 0
#endif
),
                    new RiskTakingClient("", risk),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, Maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

        private static void DSCaseStudies()
        {
#if PT
            const int Maxepoch = 10;
#else
#if SL || RG
            const int Maxepoch = 10;
#else
            const int Maxepoch = 10;
#endif
#endif
            for (int m = 1; m <= Maxepoch; m++)
            {
                //var risk = Math.Pow(10, i);
                var R = new Rewards(1, 1, 0, 0, 0, 0, 0, 1);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new ProbabilisticClient(""),
                    3, 3, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, m);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstPlan(new Slandering<History, Rec, RecommendationFunction>(env) ,KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

        private static void MaliciousPropToGreedyTest()
        {
#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
            const int Maxepoch = 7;
#endif
#endif
            //for (double i = 0.2; i <= 2; i += .2)
            {
                //var risk = Math.Pow(10, i);
                var R = new Rewards(1, 0, 0, 0, 1, 0, 0, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new ProbToGreedyClient(""),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, Maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

        private static void MaliciousRiskTakingtest()
        {
#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
            const int Maxepoch = 7;
#endif
#endif
            for (double i = 0.2; i <= 2; i += .2)
            {
                var risk = Math.Pow(10, i);
                var R = new Rewards(1, 0, 0, 0, 1, 0, 0, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new RiskTakingClient("", risk),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, Maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

        private static void RiskTakingtest()
        {
#if RG || PT || SL
            const int Maxepoch = 9;
#else
            const int Maxepoch = 7;
#endif
            for (double i = -0.2; i <= 2; i += .2)
            {
                var risk = Math.Pow(10, i);
                var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new RiskTakingClient("", risk),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, Maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstWorstCase(KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }
        private static void MaliciousKnowledgeLeveltest()
        {
#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
            const int Maxepoch = 7;
#endif
#endif
            for (int maxepoch = Maxepoch; maxepoch <= Maxepoch; maxepoch++)
            {
                var R = new Rewards(1, 0, 0, 0, 1, 0, 0, 0);
                //var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new Client(""),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.EstimateWorstCase();

                Console.ReadLine();
            }
        }

        private static void SelfishKnowledgeLeveltest()
        {
#if PT
            const int Maxepoch = 9;
#else
#if SL || RG
            const int Maxepoch = 8;
#else
            const int Maxepoch = 7;
#endif
#endif
            for (int maxepoch = Maxepoch; maxepoch <= Maxepoch; maxepoch++)
            {
                var R = new Rewards(0, 0, 0, 0, 1, 0, 1, 0);
                //var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new IsolatingSP<Trust>("",
#if AHR
                       Rec.uncertaintly
#endif
#if PT
 .5
#endif
#if RG
   0
#endif
#if SL
                     new Trust(0,0,1)
#endif
#if TR
                        .5
#endif
#if BL
 .5
#endif
#if N
 0
#endif
),
                    new Client(""),
                    2, 2, 1, 1, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.EstimateWorstCase();

                Console.ReadLine();
            }
        }
        private static void WhiteWashing()
        {
            const int Maxepoch = 35;
            for (int maxepoch = 1; maxepoch <= Maxepoch; maxepoch += (int)Math.Max(maxepoch / 10 * 2.5, 1))
            {
                //var R = new Rewards(0, 1, 0, 0, 0, 0, 0, 0);
                                var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new ProbToGreedyClient(""),
                    2, 2, 2, 2, true);
                var plan = new WhiteWashing<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstPlan(plan, KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

        private static void Slander()
        {
            const int Maxepoch = 35;
            for (int maxepoch = 1; maxepoch <= Maxepoch; maxepoch += (int)Math.Max(maxepoch / 10 * 2.5, 1))
            {
                //var R = new Rewards(0, 1, 0, 0, 0, 0, 0, 0);
                var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new ProbToGreedyClient(""),
                    2, 2, 1, 1, true);
                var plan = new Slandering<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstPlan(plan, KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

//        private static void OnOff2()
//        {
//            const int Maxepoch = 20;
//            for (int maxepoch = 1; maxepoch <= Maxepoch; maxepoch++)
//            {
//                var R = new Rewards(0, 0, 0, 0,0, -1, 0, 0);
//                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
//                    (new SP(""),
//                    new Client(""),
//                    0, 0, 3, 3, true);
//                var plan = new TrustOnOff<History, Rec, RecommendationFunction,Trust,TCM>(env,
//#if SL
//                    new Rec(0,0,1)
//#endif
//#if RG
//                    0
//#endif
//#if N
// 0
//#endif
// );
//                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
//                    new TCM(), R, maxepoch);

//                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
//                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
//                verifier.VerifyAgainstPlan(plan, KnowledgeLevel.FullObservation);

//                Console.ReadLine();
//            }
//        }

        private static void OnOff()
        {
            const int Maxepoch = 35;
            for (int maxepoch = 10; maxepoch <= Maxepoch; maxepoch++)
            {
                var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new ProbToGreedyClient(""),
                    2, 2, 1, 0, true);
                var plan = new OnOff<History, Rec, RecommendationFunction>(env, 3);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstPlan(plan, KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

        private static void Selfish()
        {
            const int Maxepoch = 20;
            for (int maxepoch = 1; maxepoch <= Maxepoch; maxepoch += (int)Math.Max(maxepoch / 10 * 2.5, 1))
            {
                var R = new Rewards(0, 0, 0, 0, 1, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new IsolatingSP<Trust>("",
#if AHR
                       Rec.b //AHR
#endif
#if PT
                        0.5
#endif
#if BL
 0.5
#endif
#if RG
                            0 //REGRET
#endif
#if SL
                     new Trust(0,0,1)
#endif
#if TR
                        0.5
#endif
#if N
   0
#endif
   ),
                    new ProbToGreedyClient(""),
                    2, 2, 1, 1, true);
                var plan = new Selfishness<History, Rec, RecommendationFunction, Trust, TCM>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstPlan(plan, KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }
        private static void SimMal()
        {
            const int Maxepoch = 35;
            for (int maxepoch = 1; maxepoch <= Maxepoch; maxepoch += (int)Math.Max(maxepoch / 10 * 2.5, 1))
            {
                var R = new Rewards(1, 0, 1, 0, 0.8, 0, 1, 0);
                var env = TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                    (new SP(""),
                    new ProbToGreedyClient(""),
                    2, 2, 1, 0, true);
                var plan = new SimMaliciousness<History, Rec, RecommendationFunction>(env);
                var sys = new TrustSystem<History, TCM, Trust, Rec>(env,
                    new TCM(), R, maxepoch);

                var verifier = new TSVerifier<History, TCM, Trust, Rec>(sys);
                //verifier.VerifyAgainstWorstCase( KnowledgeLevel.FullObservation);
                verifier.VerifyAgainstPlan(plan, KnowledgeLevel.FullObservation);

                Console.ReadLine();
            }
        }

    }

}
