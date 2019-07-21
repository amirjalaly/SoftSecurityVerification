using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Environment;
using TruSyFire.TrustSystems.TrustComputationModel;

namespace TSsBenchmarking.TrustSystems.SubjectiveLogic
{
    public class TCM:ITCM<History,Trust,Trust>
    {
        RecommendationFunction RcF = new RecommendationFunction();
        public IRecommendationFunction<History, Trust> RecommendationFunction
        {
            get { return RcF; }
        }

        public string Name
        {
            get { return "Subjective Logic" ; }
        }

        public Trust GetTrust(TruSyFire.TrustSystems.Environment.IServiceProvider sp, 
            TruSyFire.TrustSystems.Environment.IClient c, 
            TruSyFire.Verification.State<History> CurrentState)
        {
            List<Trust> trs = new List<Trust>();
            foreach (var h in CurrentState.GetAllHistories())
            {
                if (h.ServiceProvider == h.Client)
                    continue;
                if (h.ServiceProvider == sp)
                {
                    if (h.Client == c)
                        ///Direct trust of c to sp
                        trs.Add(RecommendationFunction.GetRecommendation(h));
                    else
                    {
                        ///Witness recommendation
                        var c2 = h.Client;
                        var rec = c2.GetRecommendation<RecommendationFunction, History, Trust>
                            (sp, CurrentState, RcF);
                        ///Trust to witness
                        ///
                        var c2tr = RecommendationFunction.GetRecommendation(
                            CurrentState.GetHistory(c2.Name,
                            c.Name));
                        trs.Add(TransivityOperator(c2tr, rec));
                    }
                }
            }
            var tr = trs[0];
            for (int i = 1; i < trs.Count; i++)
                tr = CumulativeOperator(tr, trs[i]);
            return tr;
        }
        public Trust TransivityOperator(Trust op1, Trust op2)
        {
            return new Trust(op1.Belief * op2.Belief,
                op1.DisBelief * op2.DisBelief,
                op1.DisBelief + op1.Uncertaintly + op1.Belief * op2.Uncertaintly);
        }
        public Trust CumulativeOperator(Trust op1, Trust op2)
        {
            var l = op1.Uncertaintly + op2.Uncertaintly - op1.Uncertaintly * op2.Uncertaintly;
            var b = (op1.Belief * op2.Uncertaintly + op1.Uncertaintly * op2.Belief) / l;
            var d = (op1.DisBelief * op2.Uncertaintly + op1.Uncertaintly * op2.DisBelief) / l;
            var u = (op1.Uncertaintly * op2.Uncertaintly) / l;
            return new Trust(b, d, u);
        }

        public double Quantitize(Trust t)
        {

            return t.Belief+t.Uncertaintly*.5;
        }
    }
    public class Trust:IComparable
    {

        public double Belief
        {
            get;private set;
        }
        public double DisBelief
        {
            get;private set;
        }
        public double Uncertaintly
        {
            get;private set;
        }
        public Trust(int R, int S)
        {
            Belief = R / (R + S + 2.0); 
            DisBelief = S / (R + S + 2.0);
            Uncertaintly = 2.0 / (R + S + 2.0);
        }
        public Trust(double b, double d, double u)
        {
            Belief = b;
            DisBelief = d;
            Uncertaintly = u;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", Belief, DisBelief, Uncertaintly);
        }
        public int CompareTo(object obj)
        {
            return (Belief+Uncertaintly*.5).CompareTo((obj as Trust).Belief+(obj as Trust).Uncertaintly*.5);
        }
    }
}
