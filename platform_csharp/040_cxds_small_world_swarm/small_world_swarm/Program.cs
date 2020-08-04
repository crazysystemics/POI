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
        public string[,] bf_global_view;                     //God's eye view of blue and red forces
        public string[,] bf_red_blue;                        //red's view of blue
        public Dictionary<string,List<string>> bf_blue_red;  //blue's view of red
        public string[,] bf_blue_cluster;                    //cluster of blue, blue_phi_cap
        public string[,] bf_blue_estim_red;                  //blue's estimation of red cluster red_phi_cap

        //bl = (blue_avg_path_len/comm_cost) * (blue_lethality) 
        //rl = ((red_avg_path_len)/comm_cost) (red_lethality_cap)
        //if (bl*rl < x) send reinforcement.

        public List<Cluster> clusters = new List<Cluster>();
        public List<Tuple<int, int>> interClusterConns = new List<Tuple<int, int>>();

        public ConsoleUiManager cuim = new ConsoleUiManager();

        public Small_World_Swarm_Bf()
        {
        }

        public Small_World_Swarm_Bf(int bfo)
        {
            bf_global_view = new string[bfo, bfo];
            bfOrder = bfo;
        }
        //b=blue, r=red, f=field
        //flat structures evolve? better seen in netlogo
        //first do in c#
        //0 to full
        public void init_bf_view_global ()
        {
            for (int i = 0; i < bfOrder; i++)
                for (int j = 0; j < bfOrder; j++)
                {
                    if (i  == 1)
                        bf_global_view[i, j] = "B";
                    else if (j % 2 == 0)
                        bf_global_view[i, j] = "R";
                    else
                        bf_global_view[i, j] = " ";
                }

        }

        //red to blue bipartite
        public void SrcFindsTgt(string[,] Matrix, int rows, int cols,
                               string src, string dest, int radius,
                               List<Tuple<int,int>> results)
        {
            results = new List<Tuple<int, int>>();
            for (int i = 0; i < rows;  i++)
                for (int j = 0; j < cols; j++ )
                {
                    if (Matrix[i,j]==src)
                    {
                        int sr = i-radius, sc = j-radius, er=i+radius, ec=j+radius;
                        for (int ri = sr; ri < er; ri++)
                        {
                            for (int rj = sc; rj < ec; rj++)
                            {
                                if (ri > 0 && ri < rows && rj > 0 && rj < cols)
                                {
                                    if (Matrix[i,j] == src && Matrix[ri,rj] == dest)
                                    {
                                        if (bf_blue_red.ContainsKey(src))
                                        {
                                            bf_blue_red[src].Add(dest);
                                        }
                                        else
                                        {
                                            bf_blue_red[src] = new List<string>();
                                        }
                                    }
                                }
                            }
                        }                        
                        
                    }

                }
        }



        public int[,] SetBlueMatrix()
        {
            

            return new int[2,2];
        }




        //Rules for Swarm formations evolutions
        //Based on Game Of Life, 
        //if difference between RED and BLUE 

        public void PrintBf()
        {
            ConsoleColor defbg;
            

            Console.Write("Enter 0 for light mode 1 for Dark Mode: ");
            string mode = Console.ReadLine();

            if (mode == "0")
            {
                defbg = ConsoleColor.White;
            }
            else
            {
                defbg = ConsoleColor.Black;
                
            }

            cuim.PrintMatrix(
                             "Battle Field", bf_global_view, bfOrder, bfOrder, new string[] { "R", "B" },
                             ConsoleColor.Yellow, ConsoleColor.Black,
                             defbg, ConsoleColor.White,
                             new ConsoleColor[] { defbg, defbg },
                             new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Blue}
                            );
        }
    }



class Program
    {

        static void Main(string[] args)
        {
            //ConsoleUiManager cm = new ConsoleUiManager();
            //cm.RunTests();
            Small_World_Swarm_Bf small_world = new Small_World_Swarm_Bf(5);

            small_world.init_bf();
            small_world.PrintBf();

            Console.ReadKey();
        }
    }
}
