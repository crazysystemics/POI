using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

//NOTES:ONE NOTE - SEARCH FOR UAS-SOC-IIT
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

    class Log
    {
        public int level;
        public StreamWriter logp;
        public string log_file_name;
        public List<string> slist = new List<string>();
        public bool entry_exit = false;
        public int call_level = 0;


        public Log()
        { }

        public Log(string filename)
        {
            level = 0;
        }

        public void Open(string filename, bool append)
        {
            log_file_name = filename;
            logp = new StreamWriter(filename, append);
        }

        public void WriteHead()
        {
            //Write In HTML
            HtmlWriter htw = new HtmlWriter();
            htw.GetHtmlHead(this);
        }

        public void WriteLine(string text, ConsoleColor fg = ConsoleColor.Black, ConsoleColor bg = ConsoleColor.White)
        {
            //WriteOnConsole
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.WriteLine(text);
            Console.ResetColor();

            //Write In HTML
            HtmlWriter htw = new HtmlWriter();
            htw.GetHtmlString(this, bg, fg, text);
        }

        public void WriteMat(string title, string[] rh, string[] ch, string[,] Mat, int rows, int cols, FgBg[,] MatColors,
                                    int top = -1, int left = -1, int bottom = -1, int right = -1)
        {
            //WriteOnConsole
            //Write In HTML
            top = (top == -1 ? 0 : top);
            left = (left == -1 ? 0 : left);
            bottom = (bottom == -1 ? sglobal.SkyRow - 1 : bottom);
            right = (right == -1 ? sglobal.SkyCol - 1 : right);
            Debug.Assert(top <= bottom && left <= right);

            HtmlWriter htw = new HtmlWriter();
            if (!String.IsNullOrEmpty(title))
            {
                WriteLine(title);
            }

            for (int i = top; i <= bottom; i++)
            {
                for (int j = left; j <= right; j++)
                {
                    Console.BackgroundColor = MatColors[i, j].Bg;
                    Console.ForegroundColor = MatColors[i, j].Fg;
                    Console.WriteLine(Mat[i, j]);
                    Console.ResetColor();
                }
            }

            htw.GetHtmlforMat(this, rh, ch, Mat, rows, cols, MatColors, top, left, bottom, right);
        }

        public void WriteToFile(bool addfooter)
        {
            if (addfooter)
            {
                slist.Add("</style>");
                slist.Add("</body >");
                slist.Add("</html>");
            }

            foreach (string s in slist)
            {
                logp.WriteLine(s);
            }
        }

        public void Close(bool store, bool addfooter)
        {
            if (store)
            {
                WriteToFile(addfooter);
            }

            logp.Close();
        }

        public void Save(bool addfooter)
        {
            //store to file, but dont add footer
            Close(true, addfooter);
            Open(log_file_name, true);
        }
    }
    enum Force { BLUE, RED }

    public static class Helpers
    {
        public static string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }
    }

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

    //renamed globals to sglobal to avoid conflict with global in intellisense
    static class sglobal
    {
        public static Random wheel = new Random();
        public static bool debug = false, info = false, error = false;
        public static bool close_stream = false;
        public static int tick_count = 0;
        public static string[] single_star = new string[] { "*" };
        public static Log logger = new Log();
        


        public static bool doublediff(double d1, double d2, double tolerance)
        {
            if (Math.Abs(d1 - d2) < tolerance)
                return true;
            else
                return false;
        }
        public static string dbg = String.Empty;
        public static string[,] Sky;
        public static int SkyRow, SkyCol;

        public static Dictionary<ConsoleColor, string> chmap = new Dictionary<ConsoleColor, string>();

        public static void init_chmap()
        {
            chmap.Add(ConsoleColor.Black,       "black");
            chmap.Add(ConsoleColor.Blue,         "blue");
            chmap.Add(ConsoleColor.Cyan,         "cyan");
            chmap.Add(ConsoleColor.DarkBlue, "darkblue");

            chmap.Add(ConsoleColor.DarkCyan,      "darkcyan");
            chmap.Add(ConsoleColor.DarkGray,      "darkgray");
            chmap.Add(ConsoleColor.DarkGreen,    "darkgreen");
            chmap.Add(ConsoleColor.DarkMagenta, "darkmagenta");

            chmap.Add(ConsoleColor.DarkRed,       "darkred");
            chmap.Add(ConsoleColor.DarkYellow, "darkyellow");
            chmap.Add(ConsoleColor.Gray,             "gray");
            chmap.Add(ConsoleColor.Green,            "green");

            chmap.Add(ConsoleColor.Magenta, "magenta");
            chmap.Add(ConsoleColor.Red, "red");
            chmap.Add(ConsoleColor.White, "white");
            chmap.Add(ConsoleColor.Yellow, "yellow");
        }
     }

    class SharedCanvas
    {
        public Force Alliance;

        public string[,] Canvas;
        public FgBg[,] CanvasColors;
        public FgBg[,] DefaultColors;
        public int SkyOrder;
        public string[] CanvasIndices;
        public List<Cluster> Clusters;
        public double PHI = 0.0;
        public string state = String.Empty;

        public SharedCanvas(Force side, int order, string[,] b)
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD"   + " METHOD: SharedCanvas ";
            if (sglobal.logger.entry_exit)
            {
                //sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }

            Alliance = side;
            sglobal.Sky = b;

            Canvas = new string[order, order];
            CanvasIndices = new string[order];
            for (int i = 0; i < order; i++)
            {
                CanvasIndices[i] = i.ToString();
            }

            CanvasColors = new FgBg[order, order];
            DefaultColors = new FgBg[order, order];

            for (int i = 0; i < order; i++)
            { 
                for (int j = 0; j < order; j++)
                {
                    DefaultColors[i, j] = new FgBg(ConsoleColor.Black, ConsoleColor.White);
                }
            }

            Clusters = new List<Cluster>();
            SkyOrder = order;
            sglobal.SkyRow = order;
            sglobal.SkyCol = order;
        }

        public void initCanvas(string[,] initial_canvas)
        {

            //state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: initCanvas ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }


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

                        //TODO :Make Dictionary based insertions                     
                        //DIRTY:
                        colbg = (i * SkyOrder + j + 1) % 13;
                        colfg = (colbg < 10 ? 0 : 14);
                        //Cluster c = new Cluster(initial_canvas[i,j], 
                        //(ConsoleColor)colbg, (ConsoleColor)colfg);
                        Cluster c = new Cluster(i, j, i, j, initial_canvas[i, j]);
                        Clusters.Add(c);         
                    }
                }
            }
        }



        public void Clear()
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: Clear ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }

            Clear(0, SkyOrder - 1, 0, SkyOrder - 1);

            state = "@Exit: Clear ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }
        }

        public void Clear(Cluster c)
        {
            Clear(c.left, c.top, c.bottom, c.right);
        }

        public void Clear(int top, int left, int bottom, int right)
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: Clear ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }


            for (int i = top; i <= bottom; i++)
            {
                for (int j = left; j <= right; j++)
                {
                    CanvasColors[i, j] = new FgBg(ConsoleColor.Black, ConsoleColor.Black);
                    Canvas[i, j] = " ";
                }
            }

            state = "@Exit: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: Clear ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }

            sglobal.logger.WriteMat("Clearing Canvas...",sglobal.single_star, sglobal.single_star, 
                                    Canvas,  this.SkyOrder, this.SkyOrder,  CanvasColors, top, left);

            Console.WriteLine();
        }


        public void Compose(List<Cluster>clusters_to_compose, bool print_cluster)
        {
            
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: Compose ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }

            foreach (Cluster c in clusters_to_compose)
            {
                
                for (int i = c.top; i <= c.bottom; i++)
                {
                    for (int j = c.left; j <= c.right; j++)
                    {

                        //sglobal.logger.WriteLine("DEBUG: " + i + " " + j);

                        Canvas[i, j] = sglobal.Sky[i, j];
                        CanvasColors[i, j] = new FgBg(c.fc, c.bc);
                    }
                }

                if (print_cluster)
                {

                    sglobal.logger.WriteLine("Cluster id = " + c + " ", ConsoleColor.White, ConsoleColor.Black);
                    sglobal.logger.WriteMat("Composed Canvas", sglobal.single_star, sglobal.single_star,
                                            Canvas, this.SkyOrder, this.SkyOrder, CanvasColors, c.top, c.left, c.bottom, c.right);
                }
            }
            state = "@Exit: Compose ";

            if (sglobal.logger.entry_exit)
            {

                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Black);
                //sglobal.logger.WriteMat("Composed Canvas", sglobal.single_star, sglobal.single_star,
                //                        Canvas, this.SkyOrder, this.SkyOrder, CanvasColors, top, left, bottom, c.right);
            }
        }

        public void PaintWithHeader(string Token, TestCase tc, int tick_count, string title)
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: PaintWithHeader ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 " + "param tick_count "   + tick_count);
                sglobal.logger.WriteLine("input0 " + "param title " + "\"" + title + "\"");
            }
            
            //Print Next
            Compose(Clusters, true);

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Cyan;


            sglobal.logger.WriteMat( "Painted Canvas...", sglobal.single_star, sglobal.single_star,
                                     Canvas, SkyOrder, SkyOrder, CanvasColors);


            state = "@Exit: PaintWithHeader:  "        ;             
            state +=" Clusters.Count " + Clusters.Count;

            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }
        }

        public string kernel(Cluster c)
        {
            state  = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: kernel ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 " + "parameter Cluster c " + c);

                string[,] kernelCanvas = Canvas;
                FgBg[,] kernelColors = new FgBg[SkyOrder, SkyOrder];

                for (int i1=0; i1 < SkyOrder; i1++)
                {
                    for (int j1=0; j1 < SkyOrder; j1++)
                    {
                        kernelColors[i1, j1]  = DefaultColors[i1, j1];
                    }
                }
                
                for (int row = c.top; row <= c.bottom; row++)
                {
                    for (int col = c.left; col <= c.right; col++)
                    {
                        kernelCanvas[row, col] = c.pivot;
                        kernelColors[row, col] = new FgBg(c.fc, c.bc);
                        //kernelColors[c.top + row, c.left + col] = new FgBg(c.fc, c.bc);
                    }
                }

                sglobal.logger.WriteMat("c-mat ", CanvasIndices, CanvasIndices, Canvas,
                                        SkyOrder, SkyOrder, 
                                        kernelColors, c.top, c.left, c.bottom, c.right);                
                
            }

            Dictionary<string, int> phi_hist = new Dictionary<string, int>();
            int i, j;
            for (i = c.top; i <= c.bottom; i++)
            {
                for (j = c.left; j <= c.right; j++)
                {
                    if (!phi_hist.ContainsKey(sglobal.Sky[i, j]))
                    {
                        phi_hist.Add(sglobal.Sky[i, j], 1);
                    }
                    else
                    {
                        phi_hist[sglobal.Sky[i, j]]++;
                    }
                }
            }

            int maxval = phi_hist.Values.Max();
            var rslt = phi_hist.Where(x => x.Value == maxval);           

            

            state = "@Exit kernel " + rslt.ElementAt(0).Key;
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }

            return rslt.ElementAt(0).Key;
        }

        public void next(int tick_count)
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: next ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine("input member Clusters.Count: " + Clusters.Count);
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
            }

            sglobal.logger.WriteMat("Sky Just Before Kernel Churning...", sglobal.single_star, sglobal.single_star, sglobal.Sky,
                                    SkyOrder, SkyOrder, DefaultColors);

            Cluster newCluster = new Cluster();

            foreach (Cluster c in Clusters)
            {
                //compute next value of s based on surroundings
                Debug.Assert(c.top <= c.bottom && c.left <= c.right);
                string s = kernel(c);
                int midrow, midcol;
                midrow = (c.bottom - c.top) / 2;
                midcol = (c.right - c.left) / 2;


                //distribute nodes on boundary to both clusters
                if (((c.bottom - c.top + 1) % 2 == 1) && ((c.right - c.left + 1) % 2 == 1))
                {
                    sglobal.Sky[midrow, midcol] = s;
                }
                else if (((c.bottom - c.top + 1) % 2 == 1) && ((c.right - c.left + 1) % 2) == 0)
                {
                    sglobal.Sky[midrow, midcol] = s;
                    sglobal.Sky[midrow, midcol + 1] = s;
                }
                else if (((c.bottom - c.top + 1) % 2 == 0) && ((c.right - c.left + 1) % 2) == 1)
                {
                    sglobal.Sky[midrow, midcol] = s;
                    sglobal.Sky[midrow + 1, midcol] = s;
                }
                else
                {
                    sglobal.Sky[midrow, midcol] = s;
                    sglobal.Sky[midrow, midcol + 1] = s;
                    sglobal.Sky[midrow + 1, midcol] = s;
                    sglobal.Sky[midrow + 1, midcol + 1] = s;
                }
            }                                       

            foreach (Cluster c in Clusters)
            {
                c.ComputeSmallPhi();
                
            }

            Cluster System = new Cluster();
            System.SplitAndCombine(ref Clusters);
            System.ComputeSmallPhi();
            PHI = System.phi;

            state =  "@Exit: SharedCanvas.next" +  " tick_count " + tick_count;
            state += " Clusters.Count " + Clusters.Count;
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }


            //            sglobal.logger.WriteMat("Sky In Next Step...", sglobal.single_star, sglobal.single_star, sglobal.Sky,
            //                                    SkyOrder, SkyOrder, );
            //sglobal.logger.WriteLine("@Exit SharedCanvas.next Cluster Count: " + Clusters.Count);
        }

        
    }

    //========================================================================================================================
    //========================================================================================================================
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
        public string state;
        public bool deleted = false;

        //Cluster is mechanism of IIT 3.0
        //Cluster is cell which experinces toppling or avalanche of SOC
        //Cluster is set of UAS tracked together by a Radar Network
        public double phi;

        public override string ToString()
        {
            string s = this.GetType().Name + " OBJECT id " + id + " pivot " + pivot  + " phi "   + phi +
                       " [ top " + top  + " left "  + left  + " bottom "      + bottom + " right  " + right + " ] px, py, pvel " +
                       " background " + bc.ToString()     + " foreground "  + fc.ToString();

            return s;
        }

        //no concept of sub-clusters now as it is only top level
        //List<Cluster> subClusters = new List<Cluster>();

        public Cluster()
        {
            //state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: Cluster Constructor ";
            //if (sglobal.logger.entry_exit)
            //{
            //    sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
            //    //sglobal.logger.WriteLine("input0 " + "parameter Cluster c " + c);

            //}

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
            Debug.Assert(top <= bottom && left <= right);
        }

        public Cluster(string pivot, ConsoleColor cfg = ConsoleColor.Black, ConsoleColor cbg = ConsoleColor.Black) :
               this()
        {

            
            if (cfg != ConsoleColor.Black && cbg != ConsoleColor.White)
            {
                fc = cfg;
                bc = cbg;
            }

            this.pivot = pivot;
        }

        public Cluster(int top, int left, int bottom, int right, string pivot,
                       ConsoleColor cfg = ConsoleColor.Black, ConsoleColor cbg = ConsoleColor.Black) :
               this()
        {
            //state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: Cluster Constructor ";
            //if (sglobal.logger.entry_exit)
            //{
            //    sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
            //    //sglobal.logger.WriteLine("input0 " + "parameter Cluster c " + c);

            //}


            if (cfg != ConsoleColor.Black && cbg != ConsoleColor.White)
            {
                fc = cfg;
                bc = cbg;
            }

            this.pivot = pivot;
            this.top = top; this.left = left; this.bottom = bottom; this.right = right;
        }

        public void print(string text)
        {
            string[] cluster_row_indices  = new string[sglobal.SkyRow];
            string[] cluster_col_indices = new string[sglobal.SkyCol];
            
            FgBg[,]  cluster_colors   = new FgBg[sglobal.SkyRow, sglobal.SkyCol];




            for (int i = 0; i < sglobal.SkyRow; i++)
            {
                cluster_row_indices[i] = i.ToString();
            }

            for (int j = 0; j < sglobal.SkyCol; j++)
            {
                cluster_col_indices[j] = j.ToString();
            }

            for (int i = 0; i < sglobal.SkyRow; i++) 
            {
                for (int j = 0; j < sglobal.SkyCol; j++)
                {
                    if (i >= top && i <= bottom && j >= left && j <= right)
                    {
                        cluster_colors[i, j] = new FgBg(fc, bc);
                    }
                    else
                    {
                        cluster_colors[i, j] = new FgBg(ConsoleColor.Black, ConsoleColor.White);
                    }
                }
            }              
            

            sglobal.logger.WriteLine(text);
            //(bottom - top + 1), (right - left + 1);
            sglobal.logger.WriteMat("c-mat ", cluster_row_indices, cluster_col_indices, sglobal.Sky,
                       sglobal.SkyRow, sglobal.SkyCol,
                       cluster_colors , top, left, bottom, right);
        }

        public void ComputeSmallPhi()
        {
            //Recursively Traverse Through Sub Clusters and compute phi
            //for leaf nodes take it from IIT 3.0
            //treat each sub cluster as a  partition.
            //homogeneity will be treated as measure of integration
            //all in same direction, integrity is high
            //so phi is ratio of "cells with same direction"/"total number of cells"

            //Instead of Nested Nodes, we will take flat bed of nodes
            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: ComputeSmallPhi ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 " + "this.phi " + phi) ;               
            }

     

            Dictionary<string, int> phi_hist = new Dictionary<string, int>();

            int i, j;
            for (i = top; i <= bottom; i++)
            {
                for (j = left; j <= right; j++)
                {
                    if (!phi_hist.ContainsKey(sglobal.Sky[i, j]))
                    {
                        phi_hist.Add(sglobal.Sky[i, j], 1);
                    }
                    else
                    {
                        phi_hist[sglobal.Sky[i, j]]++;
                    }
                }
            }

            //if (globals.log > 0)
            {
                //string[] sarr = { "DEBUG: phi_hist pivot: ", "#1", "value: ", "#2" };

                //foreach (KeyValuePair<string, int> p in phi_hist)
                //{
                //    sglobal.logger.WriteLine("DEBUG: phi_hist pivot: " + p.Key + "DEBUG: phi_hist count: " + p.Value);

                //}
            }
            //data-homogeneity 
            int maxval = phi_hist.Values.Max();
            int total_cells = (right - left + 1) * (bottom - top + 1);
            phi = (double)maxval / (double)total_cells;

            state = "@Exit ComputeSmallPhi : " + phi;
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }
        }

        public List<Cluster> SplitMinPhi(ref List<Cluster> parent_cluster)
        {
            //split among all subclusters
            //split based on intracluster integreties - phis

            //if there are no edges between clusters delete them.
            //if there are edges check strenth {if both are mixes} and then split

            //Split Currrent Cluster into 4-Sub Clusters
            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: SplitMinPhi ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "parameter Cluster c " + c);
                sglobal.logger.WriteLine("input0 : " + "parameter Parent Cluster List", ConsoleColor.Blue, ConsoleColor.White);
                foreach (Cluster cc in parent_cluster)
                {
                    sglobal.logger.WriteLine(cc.ToString(), ConsoleColor.White, ConsoleColor.Blue);
                    cc.print("...split min phi...");
                }

            }



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

            state = "@Exit SplitMinPhi: " + " Output ref Param: Parent Cluster List";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
                foreach (Cluster cc in parent_cluster)
                {
                    sglobal.logger.WriteLine(cc.ToString());
                }
            }

            return parent_cluster;
        }

        public bool canBeCombined(Cluster c1, Cluster c2)
        {
            bool cbcombined = false;

            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: canBeCombined ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                
                sglobal.logger.WriteLine("input0 : " + "Cluster 0: " + c1.ToString());
                sglobal.logger.WriteLine("input1 : " + "Cluster 1: " + c2.ToString());
                List<Cluster> toBeClusters = new List<Cluster>();
                toBeClusters.Add(c1);
                toBeClusters.Add(c2);
                SharedCanvas sc = new SharedCanvas(Force.BLUE, sglobal.SkyRow, sglobal.Sky);
                for (int i=0; i < sglobal.SkyRow; i++)
                {
                    for (int j=0; j < sglobal.SkyCol; j++)
                    {
                        sc.Canvas[i, j] = sglobal.Sky[i, j];
                        sc.CanvasColors[i, j] = new FgBg(ConsoleColor.White, ConsoleColor.Black);
                    }
                }
                sc.CanvasColors = sc.DefaultColors;
                sc.Compose(toBeClusters, false);
                sglobal.logger.WriteMat("Would be Cluster Couple...", sglobal.single_star, sglobal.single_star,
                                        sc.Canvas, sglobal.SkyRow, sglobal.SkyCol, sc.CanvasColors);

                
            }
    

            Cluster c = new Cluster();

            //Adjacency check
            if (c1.top == c2.top || c1.bottom == c2.bottom)
            {
                if (c1.right == c2.left - 1)
                {
                    //aligned side-by-side c1-left, c2-right
                    cbcombined = true;
                }
                else if (c1.left == c2.right + 1)
                {
                    cbcombined = true;
                }
                else
                {
                    cbcombined = false;
                }
            }
            else if (c1.left == c2.left || c1.right == c2.right)
            {    //aligned on vertical (left or right) edges, so one above another

                if (c1.bottom == c2.top - 1)
                {
                    //c1 on top of c2
                    cbcombined = true;
                }
                else if (c2.bottom == c1.top - 1)
                {
                    //c2 on top of c
                    cbcombined = true;
                }
                else
                {
                    cbcombined = false;
                }
            }

 
            state = "@Exit :canBeCombined " + "cbcombined: " + cbcombined;
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);               
            }          

            return cbcombined;
        }

        //Containment Check
        //TODO:Check whether Combine can be made separate routine...
        //TODO:Check whether to preserve splitcount == combinecount
        //TODO:  => i.e.. preserve population count;

        public List<Cluster> SplitAndCombine(ref List<Cluster> top_level_clusters)
        {
            bool combined = false;
            List<int> visited = new List<int>();

            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: SplitAndCombine ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 param top_level_clusters ", ConsoleColor.White, ConsoleColor.Blue);
                foreach (Cluster cc in top_level_clusters)
                {
                    sglobal.logger.WriteLine(cc.ToString());
                }                
            }

            //except_list.AddRange(near_max_sc);          

            for (int iter1 = 0; iter1 < top_level_clusters.Count; iter1++)
            {
                combined = false;
                Cluster c1 = top_level_clusters[iter1];
                
                //check whether c1 is of low fitness or high
                if (sglobal.wheel.NextDouble() < phi)
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
                        //Math.Abs(c1.phi - c2.phi) < phi_tolerance
                        if (!c1.deleted && !c2.deleted && c1.id != c2.id && canBeCombined(c1, c2) && 
                            c1.phi > c1.phi_tolerance && c2.phi > c2.phi_tolerance)
                        {
                            //sited.Add(c2.id);
                            //spinning random roulette, for selection
                            //wheel will generate a random number between 0 and 0.1; So if it is less than phi,
                            //it will be combined. Very high phis will not be combined where as intermediate will
                            Cluster c = CombineTwoClusters(c1, c2, top_level_clusters);
                            top_level_clusters.Add(c);

                            c = top_level_clusters.Find(x => x.id == c1.id);

                            for (int i=0; i < top_level_clusters.Count; i++)
                            {
                                if (top_level_clusters[i].id == c1.id)
                                {
                                    top_level_clusters[i].deleted = true;
                                }

                                if (top_level_clusters[i].id == c2.id)
                                {
                                    top_level_clusters[i].deleted = true;
                                }
                            }                          

                            c = top_level_clusters.Find(x => x.id == c2.id);
                            top_level_clusters.Remove(c);
                            combined = true;
                            //c2 loop
                            break;                           
                        }                        
                    }

                    top_level_clusters.RemoveAll(x => x.deleted);
                }
            }

            state = this.GetType().Name + "OBJECT: id " + id + " METHOD: SplitAndCombine";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }
            state = "@Exit: SplitAndCombine " + "Output ref Param: top_level_list ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
                foreach (Cluster cc in top_level_clusters)
                {
                    sglobal.logger.WriteLine(cc.ToString());
                    cc.print(".............cc.id.ToString()............");
                }
            }

            return top_level_clusters;
        }

        public Cluster CombineTwoClusters(Cluster c1, Cluster c2, List<Cluster> parent_clusters)
        {
            //...similarly select nearly_max_c
            Cluster c = new Cluster();

            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: CombineTwoClusters ";
            if (sglobal.logger.entry_exit)
            {   
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 param : c1 " + c1);
                sglobal.logger.WriteLine("input0 param : c2 " + c2);
                sglobal.logger.WriteLine("input0 param : parent_clusters");
                foreach (Cluster cc in parent_clusters)
                {
                    sglobal.logger.WriteLine(cc.ToString());
                }                
            }
           

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

            //logging begins
            state = "@Exit : CombineTwoClusters Combined Cluster c: " + c;
            c.print("...combined cluster");

            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }          
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

            //logger configuration
            //logger file
            Console.WriteLine("UAS SWARM Attrition/Reinforcement Recommendion System......");
            Console.WriteLine("Started...");
            string date = DateTime.Now.ToString().Replace("-", "_");
            date = date.Replace(" ", "_");
            string now = DateTime.Now.ToString();
            date = date.Replace(":", "_");
            //sglobal.logger.Open("log_"+ input.tc.id + date + ".htm", false);
            //logger on/off types
            sglobal.logger.entry_exit = true;

            //***Test Configuration.....
            int test_case_id = 1;
            InitTestCases();            
            TestCase input_tc = TestCases.ElementAt(test_case_id);
            sglobal.logger.Open("log_" + input_tc.id + "_" + date + ".htm", false);
            SharedCanvas skyscope = new SharedCanvas(Force.BLUE, 2, input_tc.tc);
            skyscope.initCanvas(input_tc.tc);


            int tick_count = 0;            
            string title = "TC:" + input_tc.id.ToString() + "  " + input_tc.desc +
                          "Input Sky: in BEGINNING " + tick_count + " Num of Clusters " + skyscope.Clusters.Count;
            
            sglobal.init_chmap();
            sglobal.logger.WriteHead();
            sglobal.logger.WriteLine("UAS SWARM Attrition/Reinforcement Recommendion System......");
            sglobal.logger.WriteLine("Class Program Method: Main @Entry ", ConsoleColor.White, ConsoleColor.Blue);
            sglobal.logger.WriteLine(title, ConsoleColor.Red, ConsoleColor.Yellow);
           
            
            //sglobal.logger.WriteLine("Input RED Sky: ", );
            //sglobal.logger.Save(true);
            //skyscope.initCanvas(globals.Sky);
            //skyscope.Compose();
            //skyscope.Paint();


            string tick = "y";

            while (tick_count < 2 && tick != "n" && tick != "q")
            {
                skyscope.next(tick_count);
                skyscope.PaintWithHeader("TC:", input_tc, tick_count, " after next ");

                //Move tick to next state
                tick_count++;
                sglobal.tick_count = tick_count;

                //tick = Console.ReadKey().KeyChar.ToString().ToLower();
            }

            sglobal.logger.WriteLine("PROGRAM RAN TO COMPLETION");
            sglobal.logger.Close(true, true);

        }
    }
}



