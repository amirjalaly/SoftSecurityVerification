using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TruSyFire.TrustSystems.Attackers;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.Environment.HonestEntities;
using TruSyFire.TrustSystems.TrustComputationModel;
using TruSyFire.Verification;
using TSsBenchmarking.AttackPlan;

namespace TSsBenchmarkingUI
{
    public partial class Benchmarking : Form
    {
        public Benchmarking()
        {
            InitializeComponent();
            AllocConsole();
            ShowWindow(GetConsoleWindow(), 0);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("user32")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }
        private void btn_Verify_Click(object sender, EventArgs e)
        {
            Rewards R = GetRewards();
            TruSyFire.TrustSystems.Environment.Environment env = GetEnvironment();

            if (rdb_Blind.Checked)
            {
                Verify<TSsBenchmarking.TrustSystems.Blind.History,
                    TSsBenchmarking.TrustSystems.Blind.TCM,
                    double,
                    double,
                    TSsBenchmarking.TrustSystems.Blind.RecommendationFunction>
                    (R, env);
            }
            if (rdb_SL.Checked)
            {
                Verify<
                    TSsBenchmarking.TrustSystems.SubjectiveLogic.History,
                    TSsBenchmarking.TrustSystems.SubjectiveLogic.TCM,
                    TSsBenchmarking.TrustSystems.SubjectiveLogic.Trust,
                    TSsBenchmarking.TrustSystems.SubjectiveLogic.Trust,
                    TSsBenchmarking.TrustSystems.SubjectiveLogic.RecommendationFunction>
                    (R, env);
            }

            if (rdb_ARH.Checked)
            {
                Verify<
                    TSsBenchmarking.TrustSystems.Abdul_Rahman.History,
                    TSsBenchmarking.TrustSystems.Abdul_Rahman.TCM,
                    TSsBenchmarking.TrustSystems.Abdul_Rahman.Trust,
                    TSsBenchmarking.TrustSystems.Abdul_Rahman.Trust,
                    TSsBenchmarking.TrustSystems.Abdul_Rahman.RecommendationFunction>
                    (R, env);
            }

            if (rdb_REGRET.Checked)
            {
                Verify<
                    TSsBenchmarking.TrustSystems.Regret.History,
                    TSsBenchmarking.TrustSystems.Regret.TCM,
                    double,
                    double,
                    TSsBenchmarking.TrustSystems.Regret.RecommendationFunction>
                    (R, env);
            }
            if (rdb_PT.Checked)
            {
                Verify<
                    TSsBenchmarking.TrustSystems.PeerTrust.History,
                    TSsBenchmarking.TrustSystems.PeerTrust.TCM,
                    double,
                    double,
                    TSsBenchmarking.TrustSystems.PeerTrust.RecommendationFunction>
                    (R, env);
            }
            if (rdb_TRAVOS.Checked)
            {
                Verify<
                    TSsBenchmarking.TrustSystems.TRAVOS.History,
                    TSsBenchmarking.TrustSystems.TRAVOS.TCM,
                    double,
                    TSsBenchmarking.TrustSystems.TRAVOS.Recommendation,
                    TSsBenchmarking.TrustSystems.TRAVOS.RecommendationFunction>
                    (R, env);
            }
        }

        private void Verify<History, TTCM, Trust, TRec, TRecFunc>(Rewards R, TruSyFire.TrustSystems.Environment.Environment env)
        where History : IHistory, new()
        where TTCM : ITCM<History, Trust, TRec>, new()
        where Trust : IComparable
        where TRecFunc : IRecommendationFunction<History, TRec>, new()

        {
            ShowWindow(GetConsoleWindow(), 4);
            BringConsoleToFront();
            var system = new TrustSystem<History, TTCM, Trust, TRec>
    (env, new TTCM(), R, (int)num_maxepochs.Value);

            var verifier = new TSVerifier<History, TTCM, Trust, TRec>
                  (system);

            double measure = 0;
            if (rdb_worstattack.Checked)
                measure = verifier.VerifyAgainstWorstCase(GetKnwoledge());
            else
                measure = verifier.VerifyAgainstPlan(
                    GetAttackPlan<History, TTCM, Trust, TRec, TRecFunc>(env)
                    , GetKnwoledge());

            MessageBox.Show("Robustenss: " + measure);
            ShowWindow(GetConsoleWindow(), 0);
        }
        public static void CreateConsole()
        {
            AllocConsole();
            // reopen stdout
            TextWriter writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(writer);
        }



        private IPlan<History>
            GetAttackPlan<History, TTCM, Trust, TRec, TRecFunc>
            (TruSyFire.TrustSystems.Environment.Environment env)
        where History : IHistory, new()
        where TTCM : ITCM<History, Trust, TRec>, new()
        where Trust : IComparable
        where TRecFunc : IRecommendationFunction<History, TRec>, new()
        {
            if (rdb_promote.Checked)
                return new WhiteWashing<History, TRec, TRecFunc>(env);
            if (rdb_Selfishness.Checked)
                return new Selfishness<History, TRec, TRecFunc, Trust, TTCM>(env);
            if (rdb_Slandering.Checked)
                return new Slandering<History, TRec, TRecFunc>(env);


            return new OnOff<History, TRec, TRecFunc>(env, 5);
        }

        private KnowledgeLevel GetKnwoledge()
        {
            return (KnowledgeLevel)cmb_KNLevel.SelectedIndex;
        }

        private TruSyFire.TrustSystems.Environment.Environment GetEnvironment()
        {
            return TruSyFire.TrustSystems.Environment.Environment.CreateEnvironment
                (GetHonestServiceProvider(),
                GetHonestClient(),
                (int)num_HSP_Count.Value,
                (int)num_HC_Count.Value,
                (int)num_ASP_Count.Value,
                (int)num_AC_Count.Value,
                chk_PlayBothRoles.Checked);
        }

        private IClient GetHonestClient()
        {
            if (rdb_Prob.Checked)
                return new ProbabilisticClient("");
            if (rdb_Proposed.Checked)
                return new ProbToGreedyClient("");
            return new GreedyClient("");
        }

        private TruSyFire.TrustSystems.Environment.IServiceProvider GetHonestServiceProvider()
        {
            if (rdb_Sp_Perfect.Checked)
                return new HonestSP("");
            if (rdb_SP_Faulty.Checked)
                return new FaultyServiceProvider("", ((int)num_faultRatio.Value) / 100.0);
            return new HonestSP("");

        }

        private Rewards GetRewards()
        {
            return new Rewards(
                (double)num_R_Req.Value,
                (double)num_R_Slandering.Value,
                (double)num_C_Req.Value,
                (double)num_C_ID.Value,
                (double)num_C_good.Value,
                (double)num_C_Bad.Value,
                (double)num_R_Good.Value,
                (double)num_R_Bad.Value);
        }

        private void Benchmarking_Load(object sender, EventArgs e)
        {
            cmb_KNLevel.SelectedIndex = 3;
        }
    }
}
