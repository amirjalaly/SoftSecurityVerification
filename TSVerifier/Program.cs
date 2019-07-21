using RSVerifier.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSVerifier.CaseStudies.System;
using RSVerifier.ReputationModels.Sporas;
using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Reputation;

namespace RSVerifier
{
    class Program
    {
        static void Main(string[] args)
        {
            for(double d=.9;d<=1;d+=.1)
            {
                var RS = new RSVerifier.CaseStudies.WorstCase.FaultySystemVerifier
         <History, double, ReputationModel>(2, 2, 100,5,d);
                RS.VerifyAgainstWorstCase(false);
                Console.ReadLine();
            }


        }

        private static void Risk_Taking()
        {
            var risks = new double[] { .1, .3, .7, .9, 1, 1.5, 3, 10, 100, 1000 };
            foreach (var r in risks)
            {

                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
         <History, double, ReputationModel>(2, 2, 100, 6, true, r);
                RS.VerifyAgainstWorstCase(false);
                Console.ReadLine();
            }
        }

        private static void CollPromote()
        {
            for (int i = 9; i < 10; i += 1)
            {
                int no = 3;
                int c = 75;
                int a = (int)Math.Pow(10, i / 4.0) / c;
                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
                    <History, double, ReputationModel>(no, no + a, 100 / c, 10,
                    new Rewards(0, 1, 0, 0, 0, 0), false);
                for (int j = 0; j < a; j++)
                    RS.System.Environment.Attackers.ElementAt(
                        RS.System.Environment.Attackers.Count() - j - 1)
                        .IgnoreInProviderSelection = true;
                //.VerifyAgainstPlan(new On_Off<History>(1, 10));
                RS.VerifyAgainstPlan(
                    //new Oscillation<History,double,ReputationModel>
                    //    (RS.System.Environment.Attackers,RS.System.Decision)
                    new CollusionPromoting<History, double, ReputationModel>
                        (a, no, QoS.Good, RS.System)
                        );
                //.VerifyAgainstWorstCase();
                Console.ReadLine();
            }
        }

        private static void CollSlander()
        {
            for (int i = 8; i < 10; i += 1)
            {
                int no = 3;
                int c = 30;
                int a = (int)Math.Pow(10, i / 4.0) / c;
                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
                    <History, double, ReputationModel>(no, no + a, 100 / c, 10,
                    new Rewards(0, 1, 0, 0, 0, 0), false);
                for (int j = 0; j < a; j++)
                    RS.System.Environment.Attackers.ElementAt(
                        RS.System.Environment.Attackers.Count() - j - 1)
                        .IgnoreInProviderSelection = true;
                //.VerifyAgainstPlan(new On_Off<History>(1, 10));
                RS.VerifyAgainstPlan(
                    //new Oscillation<History,double,ReputationModel>
                    //    (RS.System.Environment.Attackers,RS.System.Decision)
                    new CollusionSlandering<History, double, ReputationModel>
                        (a, no, QoS.Good, RS.System)
                        );
                //.VerifyAgainstWorstCase();
                Console.ReadLine();
            }
        }

        private static void SimpleMal()
        {
            for (int i = 5; i <= 40; i = (int)(i * 1.25))
            {
                int no = 1;

                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
                    <History, double, ReputationModel>(no, no, 100, i,
                    new FinancialReward(), false);

                RS.VerifyAgainstPlan(
                    new SimpleMalic<History>(no)
                        );
                //.VerifyAgainstWorstCase();
                Console.ReadLine();
            }
        }

        private static void Promoting()
        {
            for (int i = 2; i < 10; i += 1)
            {
                int no = 3;
                int c = 1;
                int a = (int)Math.Pow(10, i / 4.0) / c;
                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
                    <History, double, ReputationModel>(no, no + a, 100 / c, 10,
                    new Rewards(0, 1, 0, 0, 0, 0), true);
                for (int j = 0; j < a; j++)
                    RS.System.Environment.Attackers.ElementAt(
                        RS.System.Environment.Attackers.Count() - j - 1)
                        .IgnoreInProviderSelection = true;
                //.VerifyAgainstPlan(new On_Off<History>(1, 10));
                RS.VerifyAgainstPlan(
                    //new Oscillation<History,double,ReputationModel>
                    //    (RS.System.Environment.Attackers,RS.System.Decision)
                    new Promoting<History>
                        (a, no, QoS.Good, RS.System.Environment.Attackers.Take(no).ToArray())
                        );
                //.VerifyAgainstWorstCase();
                Console.ReadLine();
            }
        }

        private static void Slandering()
        {
            for (int i = 2; i < 10; i += 1)
            {
                int no = 3;
                int c = 1;
                int a = (int)Math.Pow(10, i / 4.0) / c;
                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
                    <History, double, ReputationModel>(no, no + a, 100 / c, 10,
                    new Rewards(0, 1, 0, 0, 0, 0), true);
                for (int j = 0; j < a; j++)
                    RS.System.Environment.Attackers.ElementAt(
                        RS.System.Environment.Attackers.Count() - j - 1)
                        .IgnoreInProviderSelection = true;
                //.VerifyAgainstPlan(new On_Off<History>(1, 10));
                RS.VerifyAgainstPlan(
                    //new Oscillation<History,double,ReputationModel>
                    //    (RS.System.Environment.Attackers,RS.System.Decision)
                    new Slandering<History>
                        (a, no, QoS.Good, RS.System.Environment.HonestEntities)
                        );
                //.VerifyAgainstWorstCase();
                Console.ReadLine();
            }
        }

        private static void ReEnter()
        {
            for (int i = 5; i <= 30; i += 5)
            {
                int no = 1;
                int c = 1;
                //int a = i;//(int)Math.Pow(10, i / 3.0) ;/// c;
                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
                    <History, double, ReputationModel>(no, no, 100, i,
                    new Rewards(1, 0, 1, 0, .9, .3), true);
                //for (int j = 0; j < a; j++)
                //    RS.System.Environment.Attackers.ElementAt(
                //        RS.System.Environment.Attackers.Count() - j - 1)
                //        .IgnoreInProviderSelection = true;
                //.VerifyAgainstPlan(new On_Off<History>(1, 10));
                RS.VerifyAgainstPlan(
                    //new Oscillation<History,double,ReputationModel>
                    //    (RS.System.Environment.Attackers,RS.System.Decision)
                    new ReEntry<History, double, ReputationModel>
                        (RS.System, .3)
                        );
                //.VerifyAgainstWorstCase();
                Console.ReadLine();
            }
        }

        private static void On_Off()
        {
            for (int i = 10; i <= 50; i += 5)
            {
                int no = 1;
                int c = 1;
                int a = i;//(int)Math.Pow(10, i / 3.0) ;/// c;
                var RS = new RSVerifier.CaseStudies.WorstCase.FinancialVerifier
                    <History, double, ReputationModel>(no, no, 100, a,
                    new FinancialReward(), true);
                //for (int j = 0; j < a; j++)
                //    RS.System.Environment.Attackers.ElementAt(
                //        RS.System.Environment.Attackers.Count() - j - 1)
                //        .IgnoreInProviderSelection = true;
                //.VerifyAgainstPlan(new On_Off<History>(1, 10));
                RS.VerifyAgainstPlan(
                    //new Oscillation<History,double,ReputationModel>
                    //    (RS.System.Environment.Attackers,RS.System.Decision)
                    new On_Off<History>(no, 5)
                        );
                //.VerifyAgainstWorstCase();
                Console.ReadLine();
            }
        }
    }
}
