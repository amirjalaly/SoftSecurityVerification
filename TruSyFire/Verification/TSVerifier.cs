using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Console = TruSyFire.Auxiliary.Console;
using Action = TruSyFire.TrustSystems.Attackers.Action;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification.RobustnessMeasure;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.POMDP;
using TruSyFire.MDP;

namespace TruSyFire.Verification
{
    public class TSVerifier<History, TCM, Trust, TRec>
        where History : IHistory, new()
        where TCM : ITCM<History, Trust, TRec>, new()
        where Trust : IComparable
    {
        public virtual string Title { get { return "TS Verification Case Study"; } }
        public TrustSystem<History, TCM, Trust, TRec> System { get; private set; }
        Simulator<History, TCM, Trust, TRec> Simulator { get; set; }


        public TSVerifier(TrustSystem<History, TCM, Trust, TRec> System)
        {
            this.System = System;
            Simulator = new Simulator<History, TCM, Trust, TRec>(System);
            var initialHistory = new History[System.Environment.Clients.Length * System.Environment.SP.Length];
            for (int j = 0; j < System.Environment.Clients.Length; j++)
                for (int i = 0; i < System.Environment.SP.Length; i++)
                {
                    var k = j * System.Environment.SP.Length + i;
                    initialHistory[k] = new History();
                    initialHistory[k].ServiceProvider = System.Environment.SP[i];
                    initialHistory[k].Client = System.Environment.Clients[j];
                    initialHistory[k].Reset();
                }
            StartState = new State<History>(System.Environment, 0, initialHistory);
        }

        public State<History> StartState { get; protected set; }
        public double VerifyAgainstWorstCase(KnowledgeLevel Level)
        {
            return VerifyAgainstWorstCase(Level, false);
        }
        public void EstimateWorstCase()
        {
            EstimateWorstCase(true);
        }
        public double VerifyAgainstWorstCase(KnowledgeLevel Level, bool PrintPlan)
        {

            Console.FilePath = Title + "[Worst]-" + System.TCM.Name + "-" +
                 System.Environment.HonestSP[0].EntityType + "(" +
                System.Environment.HonestSP.Length + ")-" +
                System.Environment.HonestClients[0].EntityType + "(" +
                System.Environment.HonestClients.Length + ")-" +
                System.Environment.MaliciousSP.Length + "-" +
                System.Environment.MaliciousClients.Length +
                "[" + Level + "@" + System.MaxEpochs + "].out";
            Console.Clear();
            Console.Clear();
            Console.WriteLine(@"Begin TS Verification Against Given Plan:
Verification Title: {0}
Trust Model: {1}
Knowledge Level: {8}
Environment: {2} {9}, {3} {10}, {4} Malicious SP, {5} Malicious Clients
Maximum Epoch: {6}",
                   Title,
                   System.TCM.Name,
                   System.Environment.HonestSP.Length, System.Environment.HonestClients.Length,
                   System.Environment.MaliciousSP.Length, System.Environment.MaliciousClients.Length,
                   System.MaxEpochs,
                   "",
                   Level,
                   System.Environment.HonestSP[0].EntityType,
                   System.Environment.HonestClients[0].EntityType
                   );
            Console.WriteFullSeparator();


            MDP<State<History>, Action> mdp = new MDP<State<History>, Action>
                (Simulator, System.AllActions,
                /*new AttackerObservation<History, Trust, TRec>(Level, System.TCM),*/
                StartState, System.MaxEpochs);
            if(Level == KnowledgeLevel.FullObservation)
            mdp.GenerateAllStates();
            Console.WriteMiniSeparator();
            //var mdpsolver = new MDPSolver<Belief<State<History>>, Action>(mdp);
            var mdpsolver = new MDPSolver<State<History>, Action>(mdp);
            double value, rvalue;


            var worstplan = mdpsolver.SolveMDP(System.MaxEpochs, out value);
            double atteckers, hreward;
            Console.WriteMiniSeparator();
            rvalue = GetRobustnessMeasure(value, out hreward, out atteckers);
            Console.WriteFullSeparator();
            if (PrintPlan)
            {
                var ActionCount = new Dictionary<string, int>[System.MaxEpochs];

                foreach (var st in worstplan.GetListOfStates())
                    CountAction(ActionCount, st.Epoch, worstplan.GetAction(st));

                PrintActionCount(ActionCount);
            }
            Console.WriteFullSeparator();
            Console.WriteLine(
@"
                    Honest Reward       {0:00.00}
                    Worst Plan Reward   {1:00.00}
                    Attackers Ideal     {2:00.00}", hreward, value, atteckers);
            Console.WriteMiniSeparator();
            Console.WriteLine(
@"                    Robustness Measure  {0:00.00}.",
    rvalue);
            Console.WriteFullSeparator();
            Console.Beep();
            return rvalue;
        }
        public void EstimateWorstCase(bool PrintPlan)
        {

            Console.FilePath = Title + "[Worst]-" + System.TCM.Name + "-" +
                System.Environment.HonestSP[0].EntityType+"("+
                System.Environment.HonestSP.Length + ")-" +
                System.Environment.HonestClients[0].EntityType + "(" +
                System.Environment.HonestClients.Length + ")-" +
                System.Environment.MaliciousSP.Length + "-" +
                System.Environment.MaliciousClients.Length +
                "[All@" + System.MaxEpochs + "].out";
            Console.Clear();
            Console.Clear();
            Console.WriteLine(@"Begin TS Verification Against Given Plan:
Verification Title: {0}
Trust Model: {1}
Knowledge Level: {8}
Environment: {2} {9}, {3} {10}, {4} Malicious SP, {5} Malicious Clients
Maximum Epoch: {6}",
                   Title,
                   System.TCM.Name,
                   System.Environment.HonestSP.Length, System.Environment.HonestClients.Length,
                   System.Environment.MaliciousSP.Length, System.Environment.MaliciousClients.Length,
                   System.MaxEpochs,
                   "",
                   "For all levels",
                   System.Environment.HonestSP[0].EntityType,
                   System.Environment.HonestClients[0].EntityType
                   );
            Console.WriteFullSeparator();


            var mdp = new MDP<State<History>, Action>
                (Simulator,
                System.AllActions,
                StartState, System.MaxEpochs);
            mdp.GenerateAllStates();
            Console.WriteMiniSeparator();
            var mdpsolver = new MDPSolver<State<History>, Action>(mdp);
            double value;

            mdpsolver.SolveMDP(System.MaxEpochs, out value);
            var HReward = GetHonestReward();

            var AttackersIdealReward = MaximumReward();
            foreach (KnowledgeLevel kl in Enum.GetValues(typeof(KnowledgeLevel)))
                EstimateWorstcase(kl, PrintPlan, mdp, HReward, AttackersIdealReward);
            Console.Beep();
        }

        private double EstimateWorstcase(KnowledgeLevel Level, 
            bool PrintPlan, 
            MDP<State<History>, Action> mdp,
            double HReward,
            double AttackersIdealReward
            )
        {
            double value;
            double rvalue;
            var estimator = new POMDPEstimator<State<History>, Action>
                (mdp,
                 new AttackerObservation<History, Trust, TRec>(Level, System.TCM));
            var worstplan = estimator.EstimatePOMDP(out value);


            rvalue = (AttackersIdealReward - value) / (AttackersIdealReward - HReward);

            if (PrintPlan)
            {
                var ActionCount = new Dictionary<string, int>[System.MaxEpochs];

                foreach (var st in worstplan.GetListOfStates())
                    CountAction(ActionCount, st.Epoch, worstplan.GetAction(st));

                PrintActionCount(ActionCount);
            }
            Console.WriteMiniSeparator();
            Console.WriteLine(
@"Worst Case Estimating for Knwoledge Level: {3}
                    Honest Reward       {0:00.00}
                    Worst Plan Reward   {1:00.00}
                    Attackers Ideal     {2:00.00}", 
                                                  HReward, value, AttackersIdealReward,Level);
            Console.WriteLine(
@"                    Robustness Measure  {0:00.00}.",
    rvalue);
            Console.WriteFullSeparator();
            return rvalue;
        }

        //private double GetRobustnessMeasure(MDPSolver<State<History>, Action> mdpsolver, double worstplanvalue, out double HReward, out double AttackersIdealReward)
        //{
        //    double hvalue;
        //    Console.WriteLine("Starting Honest Plan Solving...");
        //    HReward = mdpsolver.SolveMDP(new HonestPlan<History>(System.Environment.Attackers.Length),
        //        System.MaxEpochs);
        //    Console.WriteLine("Honest Plan Solving is Done...");
        //    Console.WriteMiniSeparator();
        //    Console.WriteLine("Starting Attackers Ideal Solving...");
        //    var mdp = new MDP<State<History>, Action>
        //        (AttackersIdealSimulator, System.AllActions//.Where(act=>act[
        //        , StartState, AttackersIdealSystem.MaxEpochs);
        //    mdp.GenerateAllStates();
        //    Console.WriteLine("");
        //    var mdpsolver2 = new MDPSolver<State<History>, Action>(mdp);
        //    mdpsolver2.SolveMDP
        //        (AttackersIdealSystem.MaxEpochs, out AttackersIdealReward);
        //    Console.WriteLine("Attackers Ideal Reward is done...");
        //    return (AttackersIdealReward - worstplanvalue) / (AttackersIdealReward - HReward);

        //}
        public double GetHonestReward()
        {
            //return System.MaxEpochs/10.0;
            Console.WriteLine("Starting Honest Reward Solving...");
            var HPlan =
                new HonestPlan<History>(System.Environment);
            var RSidealmdp = new MDP<State<History>, Action>
                (Simulator, new Action[0]//.Where(act=>act[
                , StartState, System.MaxEpochs);
            RSidealmdp.GenerateAllStates(HPlan);
            var RSidealmdpsolver = new MDPSolver<State<History>, Action>(RSidealmdp);
            var HReward = RSidealmdpsolver.SolveMDP
                (HPlan,
                System.MaxEpochs);
            Console.WriteMiniSeparator();
            return HReward;


        }
        private double GetRobustnessMeasure(
            double planvalue,
            out double HReward,
            out double AttackersIdealReward)
        {
            HReward = GetHonestReward();

            AttackersIdealReward = MaximumReward();

            return (AttackersIdealReward - planvalue) / (AttackersIdealReward - HReward);
        }

        private double MaximumReward()
        {
            double AttackersIdealReward;
            var maxsrv =
                Math.Max(System.Rewards.GetServiceProviderReward(QoS.Good),
                    System.Rewards.GetServiceProviderReward(QoS.Bad));
            var maxcl =
                Math.Max(System.Rewards.GetClientReward(QoS.Good),
                    System.Rewards.GetClientReward(QoS.Bad));

            var max_sp_cl =
                Math.Max(System.Rewards.GetClientReward(QoS.Good) + System.Rewards.GetServiceProviderReward(QoS.Good),
                    System.Rewards.GetClientReward(QoS.Bad) + System.Rewards.GetServiceProviderReward(QoS.Bad));
            var max = Math.Max(max_sp_cl, Math.Max(maxsrv, maxcl));

            AttackersIdealReward = max * System.MaxEpochs +
                System.Environment.HonestSP.Length * System.Rewards.Alpha_Slnd;
            return AttackersIdealReward;
        }

        public double VerifyAgainstPlan(IPlan<History> Plan, KnowledgeLevel Level)
        {
            Console.FilePath = Title + "[" + Plan.Title + "]-" +
                System.TCM.Name + "-" +
                (System.Environment.HonestSP.Length>0?System.Environment.HonestSP[0].EntityType:"") + "(" +
                System.Environment.HonestSP.Length + ")-" +
                (System.Environment.HonestClients.Length>0?System.Environment.HonestClients[0].EntityType:"") + "(" +
                System.Environment.HonestClients.Length + ")-" +
                System.Environment.MaliciousSP.Length + "-" +
                System.Environment.MaliciousClients.Length +
                "[" + System.MaxEpochs + "].out";

            Console.Clear();
            Console.WriteLine(@"Begin TS Verification Against Given Plan:
Verification Title: {0}
Trust Model: {1}
Attack Plan: {7}
Knowledge Level: {8}
Environment: {2} {9}, {3} {10}, {4} Malicious SP, {5} Malicious Clients
Maximum Epoch: {6}",
                   Title,
                   System.TCM.Name,
                   System.Environment.HonestSP.Length, 
                   System.Environment.HonestClients.Length,
                   System.Environment.MaliciousSP.Length, 
                   System.Environment.MaliciousClients.Length,
                   System.MaxEpochs,
                   Plan.Title,
                   Level,
                   (System.Environment.HonestSP.Length>0?System.Environment.HonestSP[0].EntityType:""),
                   (System.Environment.HonestClients.Length > 0 ? System.Environment.HonestClients[0].EntityType : "")
                   );
            Console.WriteFullSeparator();
            var mdp = new POMDP<State<History>, Action>
                (Simulator, new Action[0],
                new AttackerObservation<History, Trust, TRec>(Level, System.TCM),
                StartState, System.MaxEpochs);
            var mdpsolver = new MDPSolver<Belief<State<History>>, Action>(mdp);
            double value, robustness;
            double atteckersvalue, hvalue;

            /// Start to solve
            mdp.GenerateAllStates(Plan);
            Console.WriteMiniSeparator();

            value = mdpsolver.SolveMDP(Plan, System.MaxEpochs);
            Console.WriteMiniSeparator();

            robustness = GetRobustnessMeasure(value, out hvalue, out atteckersvalue);

            Console.WriteFullSeparator();
            Console.WriteLine(
@"
                    Honest Reward           {0:00.00}
                    Plan Expected Reward    {1:00.00}
                    Attackers Ideal Reward  {2:00.00}", hvalue, value, atteckersvalue);
            Console.WriteMiniSeparator();
            Console.WriteLine(
@"                    Robustness Measure  {0:00.00}.",
    robustness);

            Console.Beep();
            return robustness;



        }

        private void CountActions(POMDP<State<History>, Action> mdp,
            Belief<State<History>> state,
            IPlan<Belief<State<History>>, Action> Plan,
            Dictionary<string, int>[] Counts)
        {

            var act = Plan.GetAction(state);
            if (act == null)
                return;
            CountAction(Counts, state.Epoch, act);
            var st = mdp.GetNextStates(state, act);
            foreach (var ns in st)
            {
                CountActions(mdp, ns.Key, Plan, Counts);
            }


        }
        protected void PrintActionCount(Dictionary<string, int>[] ActionCounts)
        {
            for (int j = 0; j < ActionCounts.Length; j++)
            {
                var ActionCount = ActionCounts[j];
                Console.WriteLine("Epoch {0}", j);
                var actOrdered = ActionCount.OrderByDescending(a => a.Value);
                var sum = ActionCount.Sum(a => a.Value);
                Console.WriteLine(@"Max Selected Actions:");
                for (int i = 0; i < 10 && i < actOrdered.Count(); i++)
                    Console.WriteLine("#{3}>{1} #{2} out of #{0}", sum,
                            actOrdered.ElementAt(i).Key, actOrdered.ElementAt(i).Value, i + 1);
            }
        }

        protected void CountAction(Dictionary<string, int>[] ActionCount, int epoch, Action act)
        {
            if (ActionCount[epoch] == null)
                ActionCount[epoch] = new Dictionary<string, int>();
            if (!ActionCount[epoch].ContainsKey(act.ToString()))
                ActionCount[epoch][act.Index.ToString()] = 0;
            ActionCount[epoch][act.Index.ToString()]++;
        }

    }
}
