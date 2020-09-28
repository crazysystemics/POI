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

    class FgBg
    {
        public ConsoleColor Fg;
        public ConsoleColor Bg;

        public FgBg(ConsoleColor f, ConsoleColor b)
        {
            Fg = f;
            Bg = b;
        }
    }

    public static class globals
    {
        public static Random wheel = new Random();
        public static bool debug=false, info=false, error=false;

        public static bool doublediff(double d1, double d2, double tolerance)
        {
            if (Math.Abs(d1 - d2) < tolerance)
                return true;
            else
                return false;
        }
        public static string dbg = String.Empty;
    }

    class SharedCanvas
    {
        public Force Alliance;
        public string[,] Sky;
        public string[,] Canvas;
        public FgBg[,] CanvasColors;
        public int SkyOrder;
        public List<Cluster> Clusters;

        public SharedCanvas(Force side, int order, string[,] b)
        {
            Alliance = side;
            Sky = b;
            Canvas = new string[order, order];
            CanvasColors = new FgBg[order, order];
            Clusters = new List<Cluster>();
            SkyOrder = order;
        }

        public void initCanvas(string[,] initial_canvas)
        {
            CanvasColors = new FgBg[SkyOrder, SkyOrder];
            for (int i = 0; i < SkyOrder; i++)
            {
                for (int j = 0; j < SkyOrder; j++)
                {
                    CanvasColors[i, j] = new FgBg(ConsoleColor.Black, ConsoleColor.Black);
                }
            }

            for (int i = 0; i < SkyOrder; i++)
            {
                for (int j = 0; j < SkyOrder; j++)
                {
                    if (!String.IsNullOrEmpty(initial_canvas[i, j]))
                    {
                        int colbg, colfg;
                        //TODO Make Dictionary based insertions                     
                        //Dirty
                        colbg = (i * SkyOrder + j + 1) % 13;
                        colfg = (colbg < 10 ? 0 : 14);
                        //Cluster c = new Cluster(initial_canvas[i,j], 
                        //                        (ConsoleColor)colbg, (ConsoleColor)colfg);
                        Cluster c = new Cluster(i, j, i, j, initial_canvas[i, j],
                                                ConsoleColor.Black, ConsoleColor.Yellow);
                        Clusters.Add(c);

                        if (globals.info)
                        {
                            Console.WriteLine("INFO: " + c);
                        }
                    }
                }
            }

        }

        public void Clear()
        {
            Clear(0, SkyOrder - 1, 0, SkyOrder - 1);
        }

        public void Clear(Cluster c)
        {
            Clear(c.left, c.top, c.bottom, c.right);
        }

        public void Clear(int top, int left, int bottom, int right)
        {
            for (int i = left; i <= right; i++)
            {
                for (int j = top; j <= bottom; j++)
                {
                    CanvasColors[i, j] = new FgBg(ConsoleColor.Black, ConsoleColor.Black);
                    Canvas[i, j] = " ";
                }
            }

            Console.WriteLine();
        }


        public void Compose()
        {
            foreach (Cluster c in Clusters)
            {
                for (int i = c.top; i <= c.bottom; i++)
                {
                    for (int j = c.left; j <= c.right; j++)
                    {

                        if (globals.debug) { Console.WriteLine("DEBUG: " + i + " " + j); }
                        Canvas[i, j] = Sky[i, j];
                        CanvasColors[i, j] = new FgBg(c.fc, c.bc);
                    }
                }
            }
        }

        public void Paint()
        {
            for (int i = 0; i < SkyOrder; i++)
            {
                for (int j = 0; j < SkyOrder; j++)
                {
                    Console.ForegroundColor = CanvasColors[i, j].Fg;
                    Console.BackgroundColor = CanvasColors[i, j].Bg;
                    Console.Write(Canvas[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void next()
        {
            //foreach (Cluster c in Clusters)
            //{
            //    //c.ComputeSmallPhi();
            //    Compose()
            //}           
            Cluster top = new Cluster();
            top.SplitAndCombine(ref Clusters);
        }
    }

    //=============================================
    class Cluster
    {
        public static int? sid;
        public int id;
        double phi_tolerance = 0.001;

        //public int locn;
        //public int numClusters;
        public ConsoleColor bc, fc;
        public string pivot;
        public int px, py, pvel;
        public int top, left, right, bottom;

        //Cluster is mechanism of IIT 3.0
        //Cluster is cell which experinces toppling or avalanche of SOC
        //Cluster is set of UAS tracked together by a Radar Network
        public double phi;

        public override string ToString()
        {
            string s = " id " + id + " pivot " + pivot + " phi " + phi +
                       " top " + top + " left " + left + " bottom " + bottom + " right " + right + "px, py, pvel " +
                       " background " + bc.ToString() + " foreground " + fc.ToString();

            return s;
        }

        //no concept of sub-clusters now as it is only top level
        //List<Cluster> subClusters = new List<Cluster>();

        public Cluster()
        {



            if (sid == null)
            {
                sid = 0;
            }
            else
            {
                sid++;
            }

            id = sid.Value;

            bc = (ConsoleColor)(id % 16);
            if (bc == ConsoleColor.Gray || bc == ConsoleColor.White || bc == ConsoleColor.Yellow)
            {
                fc = ConsoleColor.Black;
            }
            else
            {
                fc = ConsoleColor.White;
            }

            pivot = ">";//">>" for output
            px = py = pvel = 0;
            phi = 0.0;

            top = left = right = bottom = 0;
        }

        public Cluster(string pivot, ConsoleColor cfg, ConsoleColor cbg) :
               this()
        {
            this.pivot = pivot;
            fc = cfg;
            bc = cbg;
        }

        public Cluster(int top, int left, int bottom, int right, string pivot, ConsoleColor fg, ConsoleColor bg) :
               this()
        {
            this.pivot = pivot;
            this.top = top; this.left = left; this.bottom = bottom; this.right = right;
            bc = bg;
            fc = fg;
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

            return 100.0;
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

        public bool canBeCombined(Cluster c1, Cluster c2)
        {
            Cluster c = new Cluster();

            //Adjacency check
            if (c1.top == c2.top || c1.bottom == c2.bottom)
            {
                if (c1.right == c2.left - 1)
                {
                    //aligned side-by-side c1-left, c2-right
                    return true;
                }
                else if (c1.left == c2.right + 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (c1.left == c2.left || c1.right == c2.right)
            {

                if (c1.bottom == c2.top - 1)
                {
                    return true;
                }
                else if (c1.left == c2.right + 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return false;
        }

        //Containment Check
        //TODO:Check whether Combine can be made separate routine...
        //TODO:Check whether to preserve splitcount == combinecount
        //TODO:  => i.e.. preserve population count;

        public List<Cluster> SplitAndCombine(ref List<Cluster> top_level_clusters)
        {
            List<int> visited = new List<int>();

            //except_list.AddRange(near_max_sc);          

            for (int iter1 = 0; iter1 < top_level_clusters.Count; iter1++)
            {
                Cluster c1 = top_level_clusters[iter1];
                visited.Add(c1.id);
                //check whether c1 is of low fitness or high
                if (globals.wheel.NextDouble() < phi)
                {
                    //c1 fitness is low. Split it
                    SplitMinPhi(ref top_level_clusters);
                }
                else
                {
                    //c1 fitness is high. Combine it with another high phi top-level cluster                    {
                    //check whether it is already visited
                    //var rslt = visited.Find(x => x == c2.id);

                    for (int iter2 = 0; iter2 < top_level_clusters.Count; iter2++)
                    {
                        Cluster c2 = top_level_clusters[iter2];
                        if (c1.id != c2.id && canBeCombined(c1, c2) && Math.Abs(c1.phi - c2.phi) < phi_tolerance)
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

            return top_level_clusters;
        }

        public Cluster CombineTwoClusters(Cluster c1, Cluster c2, List<Cluster> parent_clusters)
        {
            //...similarly select nearly_max_c
            Cluster c = new Cluster();

            if (c1.top == c2.top || c1.bottom == c2.bottom)
            {
                if (c1.right == c2.left - 1)
                {
                    //aligned side-by-side c1-left, c2-right
                    c.top = c1.top; c.left = c1.left; c.bottom = c2.bottom; c.right = c2.right;
                }
                else if (c1.left == c2.right + 1)
                {
                    //aligned side-by-side c2-left, c1-right
                    c.top = c2.top; c.left = c2.left; c.bottom = c1.bottom; c.right = c1.right;
                }

            }
            else if (c1.left == c2.left || c1.right == c2.right)
            {

                if (c1.bottom == c2.top - 1)
                {
                    //aligned one below another c1-top, c2-below
                    c.top = c1.top; c.left = c1.left; c.bottom = c2.bottom; c.right = c2.right;
                }
                else if (c1.left == c2.right + 1)
                {
                    //aligned one below another c1-top, c2-below
                    c.top = c2.top; c.left = c2.left; c.bottom = c1.bottom; c.right = c1.right;
                }
            }

            //parent_clusters.Add(c);


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
            globals.info  = true;
            globals.debug = false;

            InitTestCases();

            Console.WriteLine("Initial...");

            //SharedCanvas skyscope = new SharedCanvas(Force.BLUE, 2, tc.tc);

            //foreach (TestCase tc in TestCases)

            TestCase tc = TestCases.ElementAt(0);
            SharedCanvas skyscope = new SharedCanvas(Force.BLUE, 2, tc.tc);

            string title = "TC:" + tc.id.ToString() + "  " + tc.desc + "Input Sky";
            Console.WriteLine(title);

            skyscope.initCanvas(skyscope.Sky);
            skyscope.Compose();
            skyscope.Paint();
            

            string tick = "y";
            int tick_count = 0;

            while (tick == "y")
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;                

                title = "TC:" + tc.id.ToString() + "  " + tc.desc +
                              "Input Sky: after next" + tick.Count();

                Console.WriteLine(title);

                skyscope.next();
                skyscope.Compose();
                skyscope.Paint();

                //string title = "TC:" + tc.id.ToString() + "  " + tc.desc + "Input Sky";              
                //cuim.PrintMatrix(title, skyscope.Canvas, skyscope.CanvasOrder, skyscope.CanvasOrder,
                //ConsoleColor.Blue, ConsoleColor.Yellow,
                //ConsoleColor.White, ConsoleColor.Black);
                ConsoleKeyInfo key = Console.ReadKey();
                tick = key.KeyChar.ToString().ToLower();
            }
            
        }
    }
}



