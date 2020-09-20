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
    //
    //2020 JULY 26
    //DONE
        //1.BuildBlueSeesRedMatrix with Radius 
    //TODO
        //1.Build Blue to Blue Cluster
            //1a.assert Number of Clusters and Distribution        
        //2.Calculate Fitness of each UAS
        //3.DoAttrition
            //3a. Calculate attrition rate with and without rewiring
            //3b. ASSERT attrition with rewiring is slower (an UAS will be eliminated based on its inverse fitness value, probabilistically)
        //4.Rewire
        //5.Calculate Average Path Length (Absolute??, Monten Carlo??)
            //5a. ASSERT Average Path length with cross cluster edges is better than WITHOUT cross cluster edges (regular graph)
            //5b. ASSERT that rewiring with  SMALL_WORLD_CRITICAL_THRESHOLD (Probability) 
                //AND picking UAS with fitness less than SMALL_WORLD_CRITICAL_THRESHOLD will result in same Average Path Length but lesser Attrition than
                //picking UAS randomly with SMALL_WORLD_CRITICAL_THRESHOLD
    

    class Cluster
    {
        public int cluster_id;
        public int seed_row, seed_col;
        public int cluster_width, cluster_height;

        public int blue_uas_agents;
        public double strength;
        public double norm_strength;
        public double conn_prob;
        public double cliqueness;

        public List<Tuple<int, int>> edges = new List<Tuple<int, int>>();
    }

    class Small_World_Swarm_Bf
    {
        public double redConnPrblty = 0.5;
        public double blueConnPrblty = 0.5;
        public double red_birth_prblty;
        public Random gr = new Random();
        //int[,] bf = new int[8,8];
        public int bfOrder;
        public string[,] bf_global_view;                     //God's eye view of blue and red forces
        public string[,] bf_red_blue;                        //red's view of blue
        public Dictionary<string, List<string>> bf_blue_red;  //blue's view of red
        public string[,] bf_blue_cluster;                    //cluster of blue, blue_phi_cap
        public string[,] bf_blue_estim_red;                  //blue's estimation of red cluster red_phi_cap
        public string[,] blue_red_adj_mat;
        public int[,] bf_adjacency_red_blue;
        public List<string> BlueRadarIds, RedRadarIds;


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
            bf_global_view        = new string[bfo, bfo];
            bf_adjacency_red_blue = new int[bfo, bfo];
            bfOrder = bfo;
            BlueRadarIds = new List<string>();
            RedRadarIds = new List<string>();
            gr = new Random(18);
        }
                     
        //b=blue, gr=red, f=field
        //flat structures evolve? better seen in netlogo
        //first do in c#
        //0 to full
        public void init_bf_view_global()
        {
            Random r = new Random(18);
            int bcount = 0, rcount = 0;
            string bid = String.Empty;
            string rid = String.Empty;

            for (int i = 0; i < bfOrder; i++)
                for (int j = 0; j < bfOrder; j++)
                {
                    bid = String.Empty;
                    rid = String.Empty;

                    bid += bcount < 100 ? "0" : String.Empty;
                    bid += bcount < 10  ? "0" : String.Empty;
                    bid += bcount.ToString();

                    rid += rcount < 100 ? "0" : String.Empty;
                    rid += rcount < 10  ? "0" : String.Empty;
                    rid += rcount.ToString();

                    int roll = r.Next(3);

                    if (roll == 0)
                    {
                        bf_global_view[i, j] = "B" + bid;
                        BlueRadarIds.Add(bid);
                        bcount++;
                    }
                    else if (roll == 1)
                    {
                        bf_global_view[i, j] = "R" + rid;
                        RedRadarIds.Add(rid);
                        rcount++;
                    }
                    else
                    {
                        bf_global_view[i, j] = "    ";
                    }


                }

        }

        //red to blue bipartite
        public void SrcFindsTgt(string[,] Matrix, int rows, int cols,
                               int srci, int srcj, string target_type, int radius
                               )
        {
            //results = new List<Tuple<int, int>>();
            if (bf_blue_red == null)
            {
                bf_blue_red = new Dictionary<string, List<string>>();
            }
            string src = Matrix[srci, srcj];            
 
            int sr = srci - radius, sc = srcj - radius, er = srci + radius, ec = srcj + radius;
            for (int ri = sr; ri < er; ri++)
            {
                for (int rj = sc; rj < ec; rj++)
                {
                    if (ri > 0 && ri < rows && rj > 0 && rj < cols)
                    {
                        if (Matrix[ri, rj].First().ToString() == target_type)
                        {
                            if (bf_blue_red.ContainsKey(src))
                            {
                                bf_blue_red[src].Add(Matrix[ri, rj]);
                            }
                            else
                            {
                                bf_blue_red[src] = new List<string>();
                                bf_blue_red[src].Add(Matrix[ri, rj]);
                            }
                        }
                    }
                }
            }                

                
        }

        public void BuildBlueSeesRedMatrix(int radius)
        {
            
            List<Tuple<int, int>>temp_results = new List<Tuple<int, int>>();

            for (int i = 0; i < bfOrder; i++)
            {
                for (int j = 0; j < bfOrder; j++)
                {
                    if (bf_global_view[i, j].First().ToString() == "B")
                    {
                        
                        SrcFindsTgt(bf_global_view, bfOrder, bfOrder, i, j,
                                    "R", radius);
                        
                    }
                }
            }
        }


        public void BuildBlueRedAdjacencyMatrix()
        {

           bf_adjacency_red_blue = new int[bfOrder, bfOrder];
           if (blue_red_adj_mat == null)
            {
                blue_red_adj_mat = new string[bfOrder, bfOrder];
            }


            for (int i = 0; i < bfOrder; i++)
            {
                for (int j = 0; j < bfOrder; j++)
                {
                    blue_red_adj_mat[i, j] = "0";
                }
            }

            foreach (string src in bf_blue_red.Keys)
            {
                int bid = BlueRadarIds.FindIndex(x =>"B" + x == src);
                foreach (string dest in bf_blue_red[src])
                {
                    int rid = RedRadarIds.FindIndex(x => "R" + x == dest);
                    //TODO: remove the if condition so that field is assigned every time
                    if (bid < bfOrder && rid < bfOrder)
                        bf_adjacency_red_blue[bid, rid] = 1;
                    
                }
            }              
        }




        public int[,] SetBlueMatrix()
        {
            return new int[2, 2];
        }

        //set up initial condition - blue force number of  elements
        //cluster seeds sprinkled over battlefield. 
        //             each centre has attraction strength - nodes will be drawn from cluster in percentage
        //             they will be distributed in 2-d distribution
        //             entire cluster will move in unison.
        //
        //single-hop neighbor protocol -> discovering inter-cluster links(bfs for any-of-multiple nodes)
        public void SetBlueClusters(List<Cluster> clustlist, int blue_strength)
        {
            double total_strength = 0.0;

            foreach (Cluster cl in clustlist)
                total_strength += cl.strength;

            for (int i = 0; i < clustlist.ToArray().Length; i++)
            {
                Cluster cl = clustlist.ToArray()[i];
                cl.norm_strength = cl.strength / total_strength;
                cl.blue_uas_agents = Convert.ToInt32(blue_strength * cl.norm_strength);
                for (int row = cl.seed_row - cl.cluster_height / 2; row < cl.seed_row + cl.cluster_height + 2; row++)
                {
                    for (int col = cl.seed_row - cl.cluster_width / 2; row < cl.seed_row + cl.cluster_width + 2; row++)
                    {
                        bf_blue_cluster[row, col] = "b";
                    }
                }
            }
        }


        //Rules for Swarm formations evolutions
        //Based on Game Of Life, 
        //if difference between RED and BLUE 

        public void PrintBf()
        {
            ConsoleColor defbg;


            //Console.Write("Enter 0 for light mode 1 for Dark Mode: ");
            //string mode = Console.ReadLine();

            Console.WriteLine("Simulation Begins: ");
            string mode = "0";
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
                             new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Blue }
                            );
        }
    }

    //set up initial condition - blue force number of  elements
    //cluster seeds sprinkled over battlefield. 
    //             each centre has attraction strength - nodes will be drawn from cluster in percentage
    //             they will be distributed in 2-d distribution
    //             entire cluster will move in unison.
    //
    //single-hop neighbor protocol -> discovering inter-cluster links(bfs for any-of-multiple nodes)



    class Program
    {

        static void Main(string[] args)
        {
            //ConsoleUiManager cm = new ConsoleUiManager();
            //cm.RunTests();
            Small_World_Swarm_Bf small_world = new Small_World_Swarm_Bf(10);
            
            

            small_world.init_bf_view_global();
            small_world.BuildBlueSeesRedMatrix(2);
            small_world.BuildBlueRedAdjacencyMatrix();
            small_world.PrintBf();

            Console.ReadKey();
        }
    }
}
