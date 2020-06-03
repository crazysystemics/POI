 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace soc_battle
{
    enum info_attrib_t { RADAR=0, RWR, EA, EP};


    class info_attrib {
        public bool[]    attrList    = new bool[4];
        

        public info_attrib (bool[] al, double[] pl)
        {            
            attrList = al;          
        }

        public info_attrib()
        {
            attrList = new bool[]    { false, false, false, false };
            //probList = new double[4] { 0.5, 0.5, 0.5, 0.5 };
        }
    }      
    
   class info_node { public info_attrib blueinfo, redinfo; };
      
   class Program
   {
        //public char[,] bf = new char[4, 4];
        public bool[,] radarScope = new bool[4, 4];
        public bool[,] rwrScope = new bool[4, 4];

        public int[,] blueCluster = new int[4, 4];

        public info_node[,] info_bf = new info_node[4, 4];
        public double[]     probList    = new double[4] { 0.5, 0.5, 0.5, 0.5 };


        //bbc => bipartite_blue_cliques
        //bbc[i] indicates the clique to which ith aircraft belongs to
        //if ith aircraft does not belong to any cluster(clique), then it is set -1

        //a set of aircrafts will be in a clique, if each one of them has at least
        //one another aircraft which observes same radar as this aircraft does
        //let us say there is a clique c
        //let bbc[] = {-1,0,0,0}
        //this means b1, b2, b3 is clique
        //=> b1, b2, b3 are connected
        //=> by transitive closure, it will become clique
        public int[] bbc = new int[4];

        
        int radard = 0, rwrd = 1, ea = 2, ep = 3;

        void connect_cluster(int[,] blueCluster, int red, int blue, int old_clusterId)
        {
            //connect_by_dfs
            return;
        }

        public void updateBlueRedSwarms()
        {
            //red-radars-blue mat; blue-on-row, red-on-col
            //+1 refers to red  watching blue via radar
            //+2 refers to blue watcing red   via rwr
            //if this captures
            Random r = new Random(5);

            //RED-RADAR-SCOPE-CONNECT
            for (int red = 0; red < 4; red++)
            {
                for (int blue = 0; blue < 4; blue++)
                {
                    //Bipartite Graph is formed
                    //red on rows, blue on cols
                    //edges from red to blue
                    if (r.NextDouble() < probList[radard])
                        radarScope[red, blue] = true;
                    else
                        radarScope[red, blue] = false;

                }
            }


            for (int red = 0; red < 4; red++)
            {
                for (int blue = 0; blue < 4; blue++)//
                {
                    blueCluster[blue, red] = -2;
                }
            }



            //BLUE-RWR-SCOPE-CONNECT
            for (int red = 0; red < 4; red++)
            {
                for (int blue = 0; blue < 4; blue++)
                {
                    //red on rows, blue on cols
                    //arrows from blue to red

                    if (radarScope[red, blue] == true &&
                        r.NextDouble() < probList[rwrd])
                        blueCluster[red, blue] = -1;
                    else
                        blueCluster[red, blue] = -1;
                }
            }

            //BFS Root Search
            //DFS Connect
            //BLUE-RWR-SCOPE-CONNECT
            int clusterId = -1;
            for (int red = 0; red < 4; red++)
            {
                for (int blue = 0; blue < 4; blue++)
                {
                    //red on rows, blue on cols
                    //arrows from blue to red
                    if (blueCluster[red, blue] <= clusterId)
                        connect_cluster(blueCluster, red, blue, clusterId - 1);
                    clusterId++;
                }
            }
        }

                             
        public void buildBlueCluster()
        {
            //BLUE-CLUSTER PHASE
            for (int blue = 0; blue < 4; blue++)
            {
                for (int red = 0; red < 4; red++)
                {
                    //red on rows, blue on cols
                    //arrows from blue to red
                    if (radarScope[red, blue] == true &&
                        r.NextDouble() < probList[rwrd])
                        rwrScope[red, blue] = true;
                    else
                        rwrScope[red, blue] = true;
                }
            }

            //Label each cluster by assigning cluster number to first 
            //connected but unclustered. do a dfs from there.
        }

        public void MergeClusters()
        {
            //mergong of two clusters is avalanche
            //or toppling of sand-grains
            //avalanches will continue till no more merging
            //of clusters are done
            int avalanches = 0;
            int n;
            do
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        cur_cluster = getCluster(i, j);

                        if (cur_cluster != null)
                        {
                            new_neighbour = getNextNeighbourCluster(cur_cluster);
                            rowmin = min(cur_row, new_neibhbour_row);
                            rowmax = max(cur_row, new_neibhbour_row);

                            colmin = min(cur_col, new_neibhbour_col);
                            colmax = max(cur_row, new_neibhbour_col);

                            new_neighbor[*, *] = cur_cluster.id;
                            for (i = rowmin; i < rowmax; i++)
                                for (j = colmin; j < colmax; j++)
                                {
                                    cur_c; ister[i, j] = cur_cluster.id


                                }
                        }
                        avalanches++;
                    }

            } while (avalanches > 0);
        }

        public void ComputeClusterFreq()
        {
            foreach cluster in battlefiled
            {
                cluster_hist[Cluster.size]++;
            }

            foreach ch in cluster_hist
            {
                Console.WriteLine(ch);
            }
        }


        static void Main(string[] args)
        {
            string nature = "complex";

            Console.WriteLine("complex");
        }
    }
}
