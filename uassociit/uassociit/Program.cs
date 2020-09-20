using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//NOTES
//size of partitions (width of partitions no bits flowing into/out-of it in a single tick)
//#partitions vs size of partitions create a distribution (exhibits power-law)
//if we want partition beyond a size, it is below height of one partition only
//(based on maximum value (total in/out flow, #participants and decaying exponent), size of partition may vary)
// phi -> mix of flows, 

//Cluster Coloring Algorithm
//when a cluster is combined or split, assign color based on adjacency

//--------------------------------------------------------------------------------
//public void ConstructObsnMatrix() { }
//public void ConstructBB_Clusters() { }
//public void FillMesgEdge(BBCluster bb) { } //"^", "<", ">","v"
//bb.cique_coeff = 1 Cv from small_world paper
//                                           //if (a block is < phi_c) block will split
//                                           //if (adjacent blocks are of phi > phi_c) they will combine
//                                           //by splitting phi will increase, so they raise above triggering mechanism
//                                           //combine "phi's" of equal value ; hill climbing; max phi are already at top of hill
//                                           //phi is computed at every cluster regardless of previous fitness. So any degradation is detected.

////max-phi refers to phi with maximum integration; all clusters near it neither combine nor split; they are at top of hill
////min-phi and near split
////min-phi < phi < max-phi combine; they climb hill;

////public double   ComputeSmallPhi(BBCluster bb, int[] red_sensor_ins, int[] red_control_outs) { return 0.0; }
////split and combine together of all mechanism will complete one avalanche
////least-fit mechanism in new bbclustermat will trigger new avalanche.
////single-bb-cluster or clusters floating beyond broadcast zones will define stationary critical state
////introduction of intermediate router is a mutation.
////ultimate goal is when whole swarm is connected
//public double ComputeBigPHI(Cluster[] system) { return 0.0; }

//----------------------------------------------------------------------------------------------------
//public void UasSocIit()
//{
//    //compute fitness landscape using phi
//    for (int i = 0; i < RedFormations.Length; i++)
//    {
//        RedFormations[i].phi = RedFormations[i].ComputeSmallPhi();
//    }
//}

//public void InitBlueSharedCanvas()
//{
//}

// 1. Each cell has a color;
// 2. Align to direction of max neighbors
// 3. Fuse (collide as much as possible)
// 4. Edge Sharing (Complete or Partial) [Complete Boundary Sharing] Tiling - Clusters Move Together(Majority Direction)
// 5. Swarm Rules => Avoid Collision
// 5. Previous Cluster State is Input   [next() is mechanism, computation] Next Cluste State is output
// 6. In a given cluster, given state, cells[0..--n^2] (^v) inputs to cells[0..--n^2] (^v) 
// 7. Now, each direction is a partition (how many input direction current vs how many output direction current)
// 8. foreach current, sum (H(input current|output current)) - H(input mechanism current | output mechanism current)
// 9. sum(H(input list cluster layout particular direction (^))| H(output list cluster layout same (^) direction))
// 10. Swarm Rules
// 11. Rule 1. Follow same direction of neighbors
// 12.      max(directions) of neighbors
// 11. Rule 1. Stay Close to everyone 
// 12.      //row = (nearest_top_cluster.bottom + nearest_bottom_cluster.top)/2
// 13.      //col = (nearest_left_cluster.right + nearest_right_cluster.left)/2
// 14.      //surrounded in all directions, move in random directions
// 15.         //otherwise [move where top, bottom, left, right where majority]
// 16.
// 17.      move in steps of step-size
// 18. Rule 2. Avoid Collision
// 18.      reduce (to avoid hit forward)/increase (to avoid hit following) step size
// 17.      if change of speed not possible, change direction
//            for every plane (mark where he is planning to land next)
//            if it is marked it is no-no for others. Whoever marked first, will move there. Another will avoid collision.
// 18.     
// 19.       

// CLUSTERING RULES
// 1. Low phi will  split, High Phi will combine with equal size => high integrity clusters
// 2. Phi will increase with each avalanche
// 3. Intra cluster-wiring (small-worlds) 
// 4. Between different-direction-velocity clusters  through routernode

//CODE
namespace uassociit
{

    enum Force { BLUE, RED }

    public static class glboals
    {
        public static Random wheel = new Random();
        public static bool doublediff(double d1, double d2, double tolerance)
        {
            if (Math.Abs(d1 - d2) < tolerance)
                return true;
            else
                return false;
        }
    }

    class SharedCanvas
    {

        public Force Alliance;
        public string[,] Canvas;
        public int CanvasOrder;
        public Cluster[] Clusters;



        public SharedCanvas(Force side, int order, string[,] b)
        {
            Alliance = side;
            Canvas = b;
            CanvasOrder = order;
            Clusters = new Cluster[order * order];
        }

        public void next()
        {
            foreach (Cluster c in Clusters)
            {
                c.ComputeSmallPhi();
            }

            foreach (Cluster c in Clusters)
            {
                c.SplitMinPhi();
            }

            foreach (Cluster c in Clusters)
            {
                List<CombinedCluster> cclist = c.CombineSimialrPhi();
            }
        }

    }

    class CombinedCluster
    {
        public int id1, id2;
        public Cluster comb_cluster;
    }

    class Cluster
    {
        public int id;
        public int locn;
        public int numClusters;
        public string pivot;
        public int px, py, pvel;
        double phi_tolerance = 0.001;

        int top, left, right, bottom;

        //Cluster is mechanism of IIT 3.0
        //Cluster is cell which experinces toppling or avalanche of SOC
        //Cluster is set of UAS tracked together by a Radar Network
        public double phi;

        //no concept of sub-clusters now as it is only top level
        //List<Cluster> subClusters = new List<Cluster>();

        public Cluster()
        {
            numClusters = 0;
            pivot = ">";//">>" for output
            px = py = pvel = 0;
            phi = 0.0;
        }

        public double ComputeSmallPhi()
        {
            //Recursively Traverse Through Sub Clusters and compute phi
            //for leaf nodes take it from IIT 3.0
            //treat each sub cluster as a  partition.
            //homogeneity will be treated as measure of integration
            //all in same direction, integrity is high
            //so phi is ratio of "cells with same direction"/"total number of cells"
            Dictionary<string, int> phi_hist = new Dictionary<string, int>();

            return 0.0;
        }



        public List<Cluster> SplitMinPhi(ref List<Cluster> parent_cluster)
        {
            //split among all subclusters
            //split based on intracluster integreties - phis

            //if there are no edges between clusters delete them.
            //if there are edges check strenth {if both are mixes} and then split

            //Split Currrent Cluster into 4-Sub Clusters
            Cluster c0, c1, c2, c3;
            c0 = new Cluster(); c1 = new Cluster(); c2 = new Cluster(); c3 = new Cluster();

            int lr = (left + right) / 2;
            int tb = (top + bottom) / 2;

            c0.top = top; c0.left = left; c0.bottom = tb; c0.right = lr;
            c1.top = top; c1.left = lr; c1.bottom = tb; c1.right = right;
            c2.top = tb; c2.left = left; c2.bottom = bottom; c2.right = lr;
            c3.top = tb; c3.left = lr; c3.bottom = bottom; c3.right = right;

            Cluster tempc = new Cluster();

            //inherit characteristics from current cluster
            tempc = this;
            tempc.top = c0.top; tempc.left = c0.left; tempc.bottom = c0.bottom; tempc.right = c0.right;
            parent_cluster.Add(tempc);

            tempc = this;
            tempc.top = c1.top; tempc.left = c1.left; tempc.bottom = c1.bottom; tempc.right = c1.right;
            parent_cluster.Add(tempc);

            tempc = this;
            tempc.top = c2.top; tempc.left = c2.left; tempc.bottom = c2.bottom; tempc.right = c2.right;
            parent_cluster.Add(tempc);


            tempc = this;
            tempc.top = c3.top; tempc.left = c3.left; tempc.bottom = c3.bottom; tempc.right = c3.right;
            parent_cluster.Add(tempc);  

            return parent_cluster;
        }

        public bool isAdjacent(Cluster c1, Cluster sc)
        {

            if ((c1.top <= sc.top && c1.left >= sc.left && c1.top >= sc.bottom && c1.left <= sc.right) || //c1.topleft is within  sc
                (c1.top <= sc.top && c1.right >= sc.left && c1.top >= sc.bottom && c1.right <= sc.right) || //c1.topright within   sc
                (c1.bottom <= sc.top && c1.right >= sc.left && c1.bottom >= sc.bottom && c1.right <= sc.right) || //c1.bottomright within sc
                (c1.bottom <= sc.top && c1.left >= sc.left && c1.bottom >= sc.bottom && c1.left <= sc.right))    //c1.bottomright within sc
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Cluster> SplitAndCombine(List<Cluster> top_level_clusters)
        {
            List<int> visited = new List<int>();

            //except_list.AddRange(near_max_sc);          

            foreach (Cluster c1 in top_level_clusters)
            {
                visited.Add(c1.id);
                //check whether c1 is of low fitness or high
                if (glboals.wheel.NextDouble() < phi)
                {
                    //c1 fitness is low. Split it
                    SplitMinPhi(ref top_level_clusters);
                }
                else
                {
                    //c1 fitness is high. Combine it with another high phi top-level cluster
                    foreach (Cluster c2 in top_level_clusters)
                    {
                        var rslt = top_level_clusters.Find(x => x.id == c2.id);
                        if (rslt != null && isAdjacent(c1, c2) && Math.Abs(c1.phi - c2.phi) < phi_tolerance)
                        {
                            visited.Add(c2.id);
                            //spinning random roulette, for selection
                            //wheel will generate a random number between 0 and 0.1; So if it is less than phi,
                            //it will be combined. Very high phis will not be combined where as intermediate will
                            Cluster c = CombineTwoClusters(c1, c2, top_level_clusters);
                            top_level_clusters.Add(c);

                            c = top_level_clusters.Find(x => x.id == c1.id);
                            top_level_clusters.Remove(c);

                            c = top_level_clusters.Find(x => x.id == c2.id);
                            top_level_clusters.Remove(c);
                        }
                    }
                }
            }

            //}
            return top_level_clusters;
        }






        public Cluster CombineTwoClusters(Cluster c1, Cluster c2, List<Cluster> parent_clusters)
        {
            //...similarly select nearly_max_c


            Cluster c = new Cluster();

            c.id = c1.id;
            parent_clusters.Add(c1);
            parent_clusters.Add(c2);

            int cindex = parent_clusters.Find(x => x.id == c.id).locn;
            parent_clusters[cindex] = c;

            int c2index = parent_clusters.Find(x => x.id == c2.id).locn;
            parent_clusters[c2index] = null;

            return c;
        }
    }


    class TestCase
    {
        public int id;
        public string desc;
        public string[,] tc;

        public TestCase()
        {
            id = 0;
        }

        public TestCase(int id, string[,] tc, string desc = "")
        {
            this.id = id;
            this.tc = tc;
            this.desc = desc;
        }


    }

    class Program
    {
        static public List<TestCase> TestCases = new List<TestCase>();

        static public void InitTestCases()
        {
            int id = 0;

            //TC-00: 
            TestCases.Add(new TestCase(id++, new string[,] { { ">", ">" }, { ">", ">" } }, "all same - 1 cluster"));
            //TC-01: 
            TestCases.Add(new TestCase(id++, new string[,] { { ">", "<" }, { "^", "v" } }, "all different - 4 clusters"));
            //TC-02:
            TestCases.Add(new TestCase(id++, new string[,] { { ">", ">" }, { "<", "<" } }, "two horizontal clusters"));
            //TC-03:
            TestCases.Add(new TestCase(id++, new string[,] { { "v", "<" }, { "v", "<" } }, "two vertical clusters"));
            //TC-04: 
            TestCases.Add(new TestCase(id++, new string[,] { { "v", "<" }, { "<", "v" } }, "diagonal - hyper clusters"));
        }


        static void Main(string[] args)
        {

            ConsoleUiManager cuim = new ConsoleUiManager();

            InitTestCases();

            foreach (TestCase tc in TestCases)
            {
                SharedCanvas skyscope = new SharedCanvas(Force.BLUE, 2, tc.tc);
                string title = "TC:" + tc.id.ToString() + "  " + tc.desc + "Input Sky";
                cuim.PrintMatrix(title, skyscope.Canvas, skyscope.CanvasOrder, skyscope.CanvasOrder,
                                        ConsoleColor.Blue, ConsoleColor.Yellow,
                                        ConsoleColor.White, ConsoleColor.Black);
                foreach (Cluster C in skyscope.Clusters)
                {

                }
            }
        }
    }
}


//List<Cluster> hillClimbers = (subClusters.ToList().Except(except_list)).ToList();
//   foreach (Cluster hc1 in hillClimbers)
//   {
//       //combine two adjacent (neighborhood clustes)
//       //     1. hillclimbers are clusters who did not get dropped because of low fitness
//       //     2. high fitness => high integrity clusters will also be combined.
//       //     2a.      should not cause drop in fitness as [even after recombining integrity may be same]
//       //     3. hill climbers will try to join [among sub clusters..] with similar integrity and improve integrity
//       //                 3a. Subclusters will geometrically in same cluster; will check whether they can form a bigger cluster 
//       //                 3b.  
//       //       group all of same arrow type in hc1 and hc2 together
//       //       after placing together-redraw boundaries

//       //combining is processing tiling two clusters together.
//       //after adding phi may increase (due to information)
//       //or may drop due to integration
//       //all clusters connect
