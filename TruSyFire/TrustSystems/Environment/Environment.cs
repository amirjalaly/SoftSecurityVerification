using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TruSyFire.TrustSystems.Attackers;

namespace TruSyFire.TrustSystems.Environment
{
    public class Environment
    {
        protected Environment() { }
        //public Environment(int SP_H, int SP_A, int C_H, int C_A)
        //{
        //    List<TC> c_h = new List<TC>();
        //    for (int i = 0; i < C_H; i++)
        //        c_h.Add(
        //            (TC)typeof(TC).GetConstructor(new Type[]{typeof(string)})
        //            .Invoke(new object[]{"ClientH" + i})
        //            );
        //    HonestClients = c_h.ToArray();

        //    List<TSP> sp_h = new List<TSP>();
        //    for (int i = 0; i < SP_H; i++)
        //        sp_h.Add(
        //            (TC)typeof(TC).GetConstructor(new Type[] { typeof(string) })
        //            .Invoke(new object[] { "H" + i })
        //            );
        //    HonestClients = c_h.ToArray();
        //    List<Attacker> attacker = new List<Attacker>();
        //    for (int i = 0; i < Ano; i++)
        //        attacker.Add(new Attacker("A" + i));
        //    Attackers = attacker.ToArray();

            
        //    Entities = HonestEntities.Union(Attackers).ToArray();
        //    this.ClientsCount = ClientCounts;
        //}
        Environment(IServiceProvider[] SP_H,
            IClient[] C_H,
            MaliciousServiceProvider[] SP_A,
            MaliciousClient[] C_A)
        {
            HonestSP = SP_H;
            HonestClients = C_H;
            MaliciousSP = SP_A;
            MaliciousClients = C_A;
            SP = ((IServiceProvider[])SP_H).Union(SP_A).ToArray();
            Clients = ((IClient[])C_H).Union(C_A).ToArray();
        }
        public IServiceProvider[] SP { get; protected set; }
        public IServiceProvider[] HonestSP { get; protected set; }
        public MaliciousServiceProvider[] MaliciousSP { get; protected set; }
        public IClient[] Clients { get; protected set; }
        public IClient[] HonestClients { get; protected set; }
        public MaliciousClient[] MaliciousClients { get; protected set; }

        public int AttackersCount { get { return MaliciousSP.Length + MaliciousClients.Length; } }
        public static Environment
            CreateEnvironment(IServiceProvider HonestSP,
            IClient HonestClient,
            int HonestSPCount, int HonestClientCount, int MalSpCount, int MalClientCount,
            bool ClientsAreServiceProvider)
        {
            IServiceProvider[] HSP = new IServiceProvider[HonestSPCount];
            IClient[] HC = new IClient[HonestClientCount];
            MaliciousServiceProvider[] ASP = new MaliciousServiceProvider[MalSpCount];
            MaliciousClient[] AC = new MaliciousClient[MalClientCount];
            for (int i = 0; i < HonestSPCount; i++)
                HSP[i] = HonestSP.Clone((ClientsAreServiceProvider?"H_":"SP_H_") + i);
            for (int i = 0; i < HonestClientCount; i++)
                HC[i] = HonestClient.Clone((ClientsAreServiceProvider?"H_":"C_H_") + i);
            for (int i = 0; i < MalSpCount; i++)
                ASP[i] = new MaliciousServiceProvider((ClientsAreServiceProvider?"A_":"SP_A_") + i);
            for (int i = 0; i < MalClientCount; i++)
                AC[i] = new MaliciousClient((ClientsAreServiceProvider ? "A_" : "C_A_") + i);
            return new Environment(HSP, HC, ASP, AC);

        }

    }
}
