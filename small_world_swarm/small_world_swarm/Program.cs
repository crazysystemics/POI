using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace small_world_swarm
{
    //TODO BEGIN:
    //1. Page Display
    //2. Print Header in different color
    //3. Make Title a parameter
    //4. Write multi histogram chart multi plot chart
    //TODO END:
    class Cluster
    {
        int cluster_id;
        List<Tuple<int, int>> edges = new List<Tuple<int, int>>();
        double cliqueness;
    }

    class Small_World_Swarm_Bf
    {
        public double redConnPrblty = 0.5;
        public double blueConnPrblty = 0.5;
        public double red_birth_prblty;
        public Random r = new Random();
        //int[,] bf = new int[8,8];
        int bfOrder;
        public string[,] bf; //blue to red bipartite
        public List<Cluster> clusters = new List<Cluster>();
        public List<Tuple<int, int>> interClusterConns = new List<Tuple<int, int>>();

        public Small_World_Swarm_Bf()
        {
        }



        public Small_World_Swarm_Bf(int bfo)
        {
            bf = new string[bfo, bfo];
            bfOrder = bfo;
        }
        //b=blue, r=red, f=field
        //flat structures evolve? better seen in netlogo
        //first do in c#
        //0 to full
        public void init_bf()
        {
            for (int i = 0; i < bfOrder; i++)
                for (int j = 0; j < bfOrder; j++)
                {
                    if (r.NextDouble() < blueConnPrblty)
                        bf[i, j] = "B";
                    else if (r.NextDouble() < redConnPrblty)
                        bf[i, j] = "R";
                    else
                        bf[i, j] = " ";
                }

        }
        //Rules for Swarm formations evolutions
        //Based on Game Of Life, 
        //if difference between RED and BLUE 
    }



class Program
    {

        static void Main(string[] args)
        {
            ConsoleUiManager cm = new ConsoleUiManager();
            cm.RunTests();

            Console.ReadKey();
        }
    }
}
