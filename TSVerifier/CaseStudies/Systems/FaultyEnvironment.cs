using RepSyFire.ReputationSystems.Attackers;
using RepSyFire.ReputationSystems.Environment;
using RepSyFire.ReputationSystems.Environment.HonestEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSVerifier.CaseStudies.Systems
{
    class FaultyEnvironment:RepSyFire.ReputationSystems.Environment.Environment
           {
        public double FaultRatio { get; private set; }
        public FaultyEnvironment(int Hno, int Ano, int ClientCounts,double faultratio)
        {
            FaultRatio = faultratio;
            List<IEntity> honests = new List<IEntity>();
            for (int i = 0; i < Hno; i++)
                honests.Add(new FaultyEntity("H" + i, faultratio));
            HonestEntities = honests.ToArray();

            List<Attacker> attacker = new List<Attacker>();
            for (int i = 0; i < Ano; i++)
                attacker.Add(new Attacker("A" + i));
            Attackers = attacker.ToArray();

            
            Entities = HonestEntities.Union(Attackers).ToArray();
            this.ClientsCount = ClientCounts;
        }

    }
}
