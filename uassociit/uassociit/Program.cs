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
        public int level = 0;
        public int call_level = 0;
        public StreamWriter logp;
        public string log_file_name;
        public List<string> slist = new List<string>();
        public bool entry_exit = false;



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
            if (level < 0)
            {
                return;
            }
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

            if (level < 0)
            {
                return;
            }

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
                    Console.Write(Mat[i, j] + " ");

                }
                Console.WriteLine();
            }
            Console.ResetColor();

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

    public enum Direction
    {
        LEFT, RIGHT, TOP, BOTTOM
    }

    public enum MElement
    {
        A, B, C
    }

    //renamed globals to sglobal to avoid conflict with global in intellisense

    static class sglobal
    {
        public static Random wheel = new Random();
        public static bool debug = false, info = false, error = false;
        public static bool close_stream = false;
        public static int tick_limit = 1;
        public static int tick_count = 0;
        public static string[] single_star = new string[] { "*" };
        public static Log logger = new Log();
        public static Direction SearchDir = Direction.LEFT;

        public static int Size = 3;
        public static int Base = 2;
        public static int Dimension = 1;

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

        public static bool? ui2bool(uint? ui)
        {
            if (!ui.HasValue)
            {
                return null;
            }
            return ui.Value == 1 ? true : false;
        }

        public static uint? bool2ui(bool? b)
        {
            if (!b.HasValue)
            {
                return null;
            }

            return (uint?)(b.Value ? 1 : 0);
        }

        public static Dictionary<ConsoleColor, string> chmap = new Dictionary<ConsoleColor, string>();





        //var copy2d = orig2d.Select(a => a.ToArray()).ToArray();
        public static string[,] getcopy(ref string[,] srcmat, int rows, int cols)
        {
            string[,] destmat = new string[rows, cols];
            rows = (rows == -1) ? SkyRow : rows;
            cols = (cols == -1) ? SkyCol : cols;

            for (int i = 0; i < SkyRow; i++)
            {
                for (int j = 0; j < SkyCol; j++)
                {
                    destmat[i, j] = srcmat[i, j];
                }
            }

            return destmat;
        }

        public static FgBg[,] getcopy(ref FgBg[,] srcmat, int rows, int cols)
        {
            FgBg[,] destmat = new FgBg[rows, cols];
            rows = (rows == -1) ? SkyRow : rows;
            cols = (cols == -1) ? SkyCol : cols;

            for (int i = 0; i < SkyRow; i++)
            {
                for (int j = 0; j < SkyCol; j++)
                {
                    destmat[i, j] = srcmat[i, j];
                }
            }

            return destmat;
        }

        public static void init_chmap()
        {
            chmap.Add(ConsoleColor.Black, "black");
            chmap.Add(ConsoleColor.Blue, "blue");
            chmap.Add(ConsoleColor.Cyan, "cyan");
            chmap.Add(ConsoleColor.DarkBlue, "darkblue");

            chmap.Add(ConsoleColor.DarkCyan, "darkcyan");
            chmap.Add(ConsoleColor.DarkGray, "darkgray");
            chmap.Add(ConsoleColor.DarkGreen, "darkgreen");
            chmap.Add(ConsoleColor.DarkMagenta, "darkmagenta");

            chmap.Add(ConsoleColor.DarkRed, "darkred");
            chmap.Add(ConsoleColor.DarkYellow, "darkyellow");
            chmap.Add(ConsoleColor.Gray, "gray");
            chmap.Add(ConsoleColor.Green, "green");

            chmap.Add(ConsoleColor.Magenta, "magenta");
            chmap.Add(ConsoleColor.Red, "red");
            chmap.Add(ConsoleColor.White, "white");
            chmap.Add(ConsoleColor.Yellow, "yellow");
        }
    }

    static class BitExtensions
    {
        public static int SetBitTo1(this int value, int position)
        {
            // Set a bit at position to 1.
            return value |= (1 << position);
        }

        public static int SetBitTo0(this int value, int position)
        {
            // Set a bit at position to 0.
            return value & ~(1 << position);
        }

        public static bool IsBitSetTo1(this int value, int position)
        {
            // Return whether bit at position is set to 1.
            return (value & (1 << position)) != 0;
        }

        public static bool IsBitSetTo0(this int value, int position)
        {
            // If not 1, bit is 0.
            return !IsBitSetTo1(value, position);
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
        public List<double> prev_phiscape = new List<double>();

        public SharedCanvas(Force side, int order, string[,] b)
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: SharedCanvas ";
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

            sglobal.logger.WriteMat("Clearing Canvas...", sglobal.single_star, sglobal.single_star,
                                    Canvas, this.SkyOrder, this.SkyOrder, CanvasColors, top, left);

            Console.WriteLine();
        }


        //public void Compose(List<Cluster> clusters_to_compose, bool print_cluster)
        //{

        //    state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: Compose ";
        //    if (sglobal.logger.entry_exit)
        //    {
        //        sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
        //        //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
        //    }

        //    foreach (Cluster c in clusters_to_compose)
        //    {

        //        for (int i = c.top; i <= c.bottom; i++)
        //        {
        //            for (int j = c.left; j <= c.right; j++)
        //            {

        //                //sglobal.logger.WriteLine("DEBUG: " + i + " " + j);

        //                Canvas[i, j] = sglobal.Sky[i, j];
        //                CanvasColors[i, j] = new FgBg(c.fc, c.bc);
        //            }
        //        }

        //        if (print_cluster)
        //        {

        //            sglobal.logger.WriteLine("Cluster id = " + c + " ", ConsoleColor.White, ConsoleColor.Black);
        //            sglobal.logger.WriteMat("Composed Canvas", sglobal.single_star, sglobal.single_star,
        //                                    Canvas, this.SkyOrder, this.SkyOrder, CanvasColors, c.top, c.left, c.bottom, c.right);
        //        }
        //    }
        //    state = "@Exit: Compose..printing done in parent function ";
        //    sglobal.logger.WriteLine(state);

        //}

        //TODO Compose Canvas should be called from inside this function
        public void Compose(ref string[,] iomat, ref FgBg[,] iomat_colors,
                            int rows, int cols, List<Cluster> pClusters, bool print_cluster)
        {

            if (sglobal.logger.entry_exit)
            {

                //sglobal.logger.WriteLine("@Entry Compose, ConsoleColor.White, ConsoleColor.Black);
                //sglobal.logger.WriteMat("Composed Canvas", sglobal.single_star, sglobal.single_star,
                //                        Canvas, this.SkyOrder, this.SkyOrder, CanvasColors, top, left, bottom, c.right);
                state = "@Entry: Class:" + this.GetType().Name + " METHOD: Compose ";
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("[input param 0 pClusters]");
                foreach (Cluster c in pClusters)
                {
                    sglobal.logger.WriteLine(c.ToString());
                }
                sglobal.logger.WriteLine("[/input param 0 pClusters>]");
                sglobal.logger.WriteLine("[input param 1 print_cluster]");
                sglobal.logger.WriteLine(print_cluster.ToString());
                sglobal.logger.WriteLine("[input param 1 print_cluster]");
                //sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
            }

            //Compose the loop
            foreach (Cluster c in pClusters)
            {
                //background DarkYellow foreground White
                for (int i = c.top; i <= c.bottom; i++)
                {
                    for (int j = c.left; j <= c.right; j++)
                    {
                        //sglobal.logger.WriteLine("DEBUG: " + i + " " + j);
                        iomat[i, j] = sglobal.Sky[i, j];
                        //ConsoleColor conc = c.bc;
                        //if (conc == ConsoleColor.DarkYellow)
                        //{
                        //    conc = ConsoleColor.DarkRed;
                        //}

                        iomat_colors[i, j] = new FgBg(c.fc, c.bc);
                    }
                }

                if (print_cluster)
                {

                    sglobal.logger.WriteLine("Cluster id = " + c + " ", ConsoleColor.White, ConsoleColor.Black);
                    sglobal.logger.WriteMat("Composed Canvas", sglobal.single_star, sglobal.single_star,
                                            Canvas, this.SkyOrder, this.SkyOrder, CanvasColors, c.top, c.left, c.bottom, c.right);
                }
            }

            state = "@Exit: Compose..printing done in parent function ";
            sglobal.logger.WriteLine(state);

        }



        public void PaintWithHeader(TestCase tc)
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: PaintWithHeader ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 " + "param tick_count " + sglobal.tick_count);
                //sglobal.logger.WriteLine("input0 " + "param title " + "\"" + title + "\"");
            }

            //Print Next
            //Compose(Clusters, true);
            Compose(ref Canvas, ref CanvasColors, SkyOrder, SkyOrder, Clusters, true);

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Cyan;


            sglobal.logger.WriteMat("Painted Canvas...", sglobal.single_star, sglobal.single_star,
                                     Canvas, SkyOrder, SkyOrder, CanvasColors);


            state = "@Exit: PaintWithHeader:  ";
            state += " Clusters.Count " + Clusters.Count;

            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }
        }

        public void kernel(ref string[,] Mat, int rows, int cols)
        {
            int i, j;

            //TODO IMPORTANT We can write GetCluster which tells the cluster to which a Mat[i,j] belongs to
            //Based on that both cluster-members and surrounding neighbors (same and non cluster) can be used to represent values
            //Cluster members represent functional cohesion whereas grid neibhorhood repesents spatial or temportal cohesion
            //currently kernet unchanges the matrix. so focus is on clustering.

            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: kernel ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "parameter Cluster c " + c);

                //string[,] kernelCanvas = sglobal.Sky;
                //FgBg[,] kernelColors = DefaultColors; // new FgBg[SkyOrder, SkyOrder];
            }




            //for (int i1=0; i1 < SkyOrder; i1++)
            //{
            //    for (int j1=0; j1 < SkyOrder; j1++)
            //    {
            //        kernelColors[i1, j1]  = DefaultColors[i1, j1];
            //    }
            //}

            //for (int row = c.top; row <= c.bottom; row++)
            //{
            //    for (int col = c.left; col <= c.right; col++)
            //    {
            //        kernelCanvas[row, col] = Canvas[row,col];// c.pivot;
            //        //kernelColors[row, col] = kernelColors[row, col];
            //        //new FgBg(c.fc, c.bc);
            //        //kernelColors[c.top + row, c.left + col] = new FgBg(c.fc, c.bc);
            //    }
            //}
            if (sglobal.logger.entry_exit)
            {
                //Composing entry sky to which kernel is applied
                Compose(ref Canvas, ref CanvasColors, SkyOrder, SkyOrder, Clusters, false);
                //Compose(Clusters, false);

                sglobal.logger.WriteMat("entry c-mat kernel", CanvasIndices, CanvasIndices, Canvas,
                                        rows, cols,
                                        CanvasColors);

            }

            //Dictionary<string, int> phi_hist = new Dictionary<string, int>();
            ////int i, j;
            //for (i = c.top; i <= c.bottom; i++)
            //{
            //    for (j = c.left; j <= c.right; j++)
            //    {
            //        if (!phi_hist.ContainsKey(sglobal.Sky[i, j]))
            //        {
            //            phi_hist.Add(sglobal.Sky[i, j], 1);
            //        }
            //        else
            //        {
            //            phi_hist[sglobal.Sky[i, j]]++;
            //        }
            //    }
            //}

            //int maxval = phi_hist.Values.Max();
            //var rslt = phi_hist.Where(x => x.Value == maxval);           



            state = "@Exit kernel...";// + rslt.ElementAt(0).Key;
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state);
            }

            //return rslt.ElementAt(0).Key;
        }

        public void next(TestCase tc)
        {
            state = "@Entry: Class:" + this.GetType().Name + " Object: id TBD" + " METHOD: next tick_count " + sglobal.tick_count;
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.Black, ConsoleColor.Cyan);
                sglobal.logger.WriteLine("input member Clusters.Count: " + Clusters.Count);
            }

            //========================1.0 Apply Kernel================================
            sglobal.logger.WriteLine("1.0 Applying Kernel: #Clusters: " + Clusters.Count, ConsoleColor.White, ConsoleColor.Magenta);

            sglobal.logger.WriteMat("Sky Just Before Kernel Churning...", sglobal.single_star, sglobal.single_star, sglobal.Sky,
                                    SkyOrder, SkyOrder, DefaultColors);

            Cluster newCluster = new Cluster();

            //Currently kernel is doing nothing. 
            //It should be member of Cluster Members and Spatial neighborhood
            kernel(ref sglobal.Sky, SkyOrder, SkyOrder);


            var tempCanvas = sglobal.getcopy(ref sglobal.Sky, SkyOrder, SkyOrder);
            var tempColors = sglobal.getcopy(ref DefaultColors, SkyOrder, SkyOrder);
            Compose(ref tempCanvas, ref tempColors, SkyOrder, SkyOrder, Clusters, false);
            sglobal.logger.WriteMat("in SharedCanvas.next:After Kernel Operation...",
                                     sglobal.single_star, sglobal.single_star,
                                     tempCanvas, SkyOrder, SkyOrder, tempColors);


            //========================2.0 Compute Small Phi==============================
            sglobal.logger.WriteLine("2.0 Computing Small Phi of Clusters. #Clusters: " + Clusters.Count,
                                      ConsoleColor.White, ConsoleColor.Magenta);
            foreach (Cluster c in Clusters)
            {
                c.ComputeSmallPhi();
            }

            List<double> phiscape = new List<double>();
            foreach (Cluster c in Clusters)
            {
                phiscape.Add(c.phi);
            }


            bool max_phiscape_change = false;
            if (prev_phiscape.Count == 0)
            {
                max_phiscape_change = true;
            }
            else
            {
                double curphimax = phiscape.Max();
                double prevphimax = prev_phiscape.Max();
                {
                    if (curphimax != prevphimax)
                    {
                        max_phiscape_change = true;
                        //break;
                    }


                    if (!max_phiscape_change)
                    {
                        sglobal.SearchDir = (Direction)(((int)sglobal.SearchDir + 1) % 4);
                    }
                }
            }

            prev_phiscape = new List<double>(phiscape);
            //======================= 3.0 SplitAndCombine ==============================
            sglobal.logger.WriteLine("3.0 Split and Combine.  #Clusters: " + Clusters.Count,
                                      ConsoleColor.White, ConsoleColor.Magenta);
            Cluster System = new Cluster();
            System.SplitAndCombine(ref Clusters);





            tempCanvas = sglobal.getcopy(ref sglobal.Sky, SkyOrder, SkyOrder);
            tempColors = sglobal.getcopy(ref DefaultColors, SkyOrder, SkyOrder);
            Compose(ref tempCanvas, ref tempColors, SkyOrder, SkyOrder, Clusters, false);
            sglobal.logger.WriteMat("After System.SplitAndCombine... ",
                                     sglobal.single_star, sglobal.single_star,
                                     tempCanvas, SkyOrder, SkyOrder, tempColors);


            //========================4.0 Compute System PHI=================
            sglobal.logger.WriteLine("4.0 Compute System PHI  #Clusters: " + Clusters.Count,
                                      ConsoleColor.White, ConsoleColor.Magenta);
            System.top = 0;
            System.left = 0;
            System.bottom = SkyOrder - 1;
            System.right = SkyOrder - 1;
            System.ComputeSmallPhi();
            PHI = System.phi;


            //========================5.0 Paint the Sky =======================
            sglobal.logger.WriteLine("5.0 PaintWithHeader at end of next tick_count " + sglobal.tick_count,
                                      ConsoleColor.White, ConsoleColor.Magenta);
            PaintWithHeader(tc);


            state = " @Exit: SharedCanvas.next" + " tick_count " + sglobal.tick_count;
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


    class Purview
    {
        public static int sid = 0;
        public int id;
        public Mechanism Whole; //ABCc
        public Triplet<bool?> ElementState;

        public Mechanism InPurviewCurrent;//BCc
        public Mechanism InPurviewCause; //Ap
        public Mechanism InPurviewEffect;//?

        public Mechanism CoreCause;
        public Mechanism CoreEffect;


        public double cause_phi_max;
        public double effect_phi_max;
        public double ce_phi_max;


        public List<Partition> Partitions = new List<Partition>();

        public double phi_max;

        public Purview()
        {
            sid++;
            id = sid;
        }

        public Purview(Purview pur) : this()
        {
            Whole = pur.Whole;
            ElementState.Set(false, false, false);
            // = new Triplet<bool?>(null, false, false); //BCc=00
            InPurviewCurrent = pur.InPurviewCurrent;
            InPurviewCause = pur.InPurviewCause;
            InPurviewEffect = pur.InPurviewEffect;
            Partitions = pur.Partitions;
        }
    }

    class ProbDistribn
    {
        public int dist_index;
        public int count;
        public double prob;
    }

    class Partition
    {
        //when we derive from whole, the parts that are not derived are made null.
        //this can be made to generate null.

        // Cc/[]
        public Mechanism P1Cause;  //[]
        public Mechanism P1Current; //Cc

        // Bc/Ap
        public Mechanism P2Current;//Bc
        public Mechanism P2Effect; //?

        // BCc/Ap = (Cc/[]) X (Bc/Ap)
        // Result/Given = (P1Result/P1Given) * (P2Result/P2Given)

        public int P1count = 0;
        public int P2count = 0;
        public int P1xP2count = 0;
        public int Wholecount = 0;
        
        

        public ProbDistribn[] P1ProbDistribn = new ProbDistribn[(int)Math.Round(Math.Pow(2, 3))];
        public ProbDistribn[] P2ProbDistribn = new ProbDistribn[(int)Math.Round(Math.Pow(2, 3))];
        public ProbDistribn[] P1xP2 = new ProbDistribn[(int)Math.Round(Math.Pow(2, 3))];
        public ProbDistribn[] WholePurviewProbDistribn = new ProbDistribn[(int)Math.Round(Math.Pow(2, 3))];

        public double getmaxProbOfPartn ()
        {
            maxP1xP2 = 0.0;
            for (int i = 0; i < P1xP2count; i++)
            {
                if (P1xP2[i].prob > maxP1xP2)
                {
                    maxP1xP2 = P1xP2[i].prob;
                }
            }

            return maxP1xP2;
        }
        Mechanism Distance;
        double maxP1xP2 = 0.0;

        public double Phi;
    }



    class Mechanism
    {
        public static int sid = 0;
        public int id;

        //Figure 8 in phenomology paper
        //Mechanism BC, Purview All ABC combinations (BC/Ap) (Result/Given) etc.,
        //Partition is Splitting of That Purview into two parts IN-PURVIEW (P1Result,.....) OUT-PURVIEW(P2Result)

        //locked will be indexed by MElements.A, MElements.B, MElements.C
        //eg locked[MElements.A] = true
        public uint?[] locked = new uint?[3];
        public string[] signatures = new string[3];

        Dictionary<string, bool> FreqLock = new Dictionary<string, bool>();
        Dictionary<int, string> PosFreq = new Dictionary<int, string>();
        Dictionary<string, int> FreqPos = new Dictionary<string, int>();
        void foo()
        {
            FreqLock["A"] = true;
            PosFreq[0] = "A";
            int locn = FreqPos["A"];
            PosFreq[locn] = "B";
        }

        //now only one kind of mechanism. Tomrrow different kind of mechanisms may be there.
        public int Size = 3; //(3 cells LEFT CENTER RIGHT or variables A, B, C)
        public int Base = 2;
        public int Order = 1;
        public int RepLen = 0;


        //t(n-1) and t(n+1)
        public double InfoContent = 0.0;

        public int cause_repo_count = 0;
        public int effect_repo_count = 0;
        public List<Mechanism> CauseReportoire = new List<Mechanism>();
        public List<Mechanism> EffectReportoire = new List<Mechanism>();

        //Mechanism Cause, Effect;
        public Mechanism CoreCause, CoreEffect;

        //Partitions for phi-max
        //public List<Partition> CausePartitions = new List<Partition>();
        //public List<Partition> EffectPartitions = new List<Partition>();
        //public Partitio MIP = new Partition();

        //Phi related metrics
        public double CausePhi = 0.0;
        public double EffectPhi = 0.0;
        public double Phi = 0.0;
        public double Cause_Phi_Max = 0.0;
        public double Dest_Phi_Max = 0.0;
        public double Phi_Max = 0.0;

        public Mechanism(uint? a, uint? b, uint? c)
        {
            id++;
            sid = id;

            locked[0] = a;
            locked[1] = b;
            locked[2] = c;

            Size = 3;
            Base = 2;
            Order = 1;

            FreqLock.Add("A", false);
            FreqLock.Add("B", false);
            FreqLock.Add("C", false);

            FreqPos.Add("A", -1);
            FreqPos.Add("B", 0);
            FreqPos.Add("C", 1);

            PosFreq.Add(-1, "A");
            PosFreq.Add(0, "B");
            PosFreq.Add(1, "C");
        }

        public int loc(string freq)
        {
            for (int i = 0; i < signatures.Length; i++)
            {
                if (signatures[i] == freq)
                {
                    return i;
                }
            }

            Debug.Assert(false);
            return -1;
        }

        public uint? GetPackedLock()
        {
            return locked[2] * 100 + locked[10] + locked[0];
        }

        public void SetLockSig(Triplet<uint?> locks = null, uint? packed_locks = null, string[] sigs = null)
        {
            if (locks != null)
            {
                locked[0] = locks.left;
                locked[1] = locks.center % 2;
                locked[2] = locks.right % 4;
            }

            if (locks != null)
            {
                locked[0] = packed_locks % 2;
                locked[1] = (packed_locks / 2) % 2;
                locked[2] = packed_locks % 4;
            }

            if (sigs != null)
            {
                signatures[0] = sigs[(uint)locked[0]];
                signatures[1] = sigs[(uint)locked[1]];
                signatures[2] = sigs[(uint)locked[2]];
            }
        }

        public uint?[] Kernel(uint?[] in_locked)
        {
            //Every uas receives all signatures sig 0 to sig2 (sig n-1)
            //each cell attempts lock signature[i] 
            //e.g., signatre[0] == "A" means UAS 0 trying to track/jam  in freq "A"
            //if it is effective locked[UAS-0] => locked[0] becomes true
            //Every UAS in cluster switch their locking frequency simultaneously
            //For the sake of modeling they are governed by  
            //(UAS-1 | UAS-2), (UAS-0 & UAS-1), (UAS-1 ^ UAS-2)
            Mechanism m = new Mechanism(in_locked[1] | in_locked[2],
                                        in_locked[0] & in_locked[2],
                                        in_locked[0] ^ in_locked[1]);

            //or-and-xor
            //sig[0]=sig[1] or sig[2]
            //sig[1]=sig[0] and sig[2]
            //sig[2]=sig[0] xor sig[1]
            //locked(t-1) [or-and-xor] locked(t)estim
            //sig(t-1) X locked(t)estim=sig(t)estim
            //TPM(sig(t-1), sig(t)estim)  = locked(t)estim - locked(t-1)estim = [
            //w * sig(t-1) + (1-w) * sig(t)estim => sig(t)
            //locked(t)observed = s

            //rotate-signature
            string temp;
            temp = m.signatures[2];
            m.signatures[2] = m.signatures[1];
            m.signatures[1] = m.signatures[0];
            m.signatures[0] = temp;

            return m.locked;
        }

        //Get Cause Effect Reportoire of this Mechanism
        public double DistanceFrom(Mechanism m_src)
        {
            return 0.0;
        }
        public double GetCauseInformation()
        {
            return 0.0;

        }

        enum ePartition { P1, P2 };
        enum Row { SLNO, Ap, Bp, Cp, Ac, Bc, Cc, Va, Vb, Vc };

        

        public double GetPhi(Purview inpurview)
        { 
            List<Mechanism> CauseRep = new List<Mechanism>();
            uint?[,] shadow_counter = new uint?[Size * 3 + 1, 8];
            int[] outsiders = new int[Size * 3,];
           // string[] header = string ["Ap", "Bp", "Cp", "Ac", "Bc", "Cc"];
            double[] Partitions = new double[(int)Math.Round(Math.Pow(2, Size))];


            //minority, ie., 1 element is set to null
            ePartition[] PartInfo = { ePartition.P1, ePartition.P1, ePartition.P2, ePartition.P1, ePartition.P1, ePartition.P2 };//Should be initialized;
            uint?[,] PartnInfoTable = { { null, 1, 2 }, { 0, null, 2 }, { 0, 1, null }, { 0, 1, 2 } };

            uint?[] inp_locked = { 0, 0, 0 };
            int[] vcols = new int[2]; 
            uint? null_ele = null;
            double phi = 0.0;
            Partition p = new Partition();

            //Process Each Partition
            for (int partn_index = 0; partn_index < 4; partn_index++)
            {
                uint?[] partn_info = new uint?[Size];
                partn_info[0] = PartnInfoTable[partn_index, 0];//sglobal.bool2ui(inpurview.ElementState.left);
                partn_info[1] = PartnInfoTable[partn_index, 0]; sglobal.bool2ui(inpurview.ElementState.center);
                partn_info[2] = PartnInfoTable[partn_index, 0]; sglobal.bool2ui(inpurview.ElementState.center);

                //mark partition boundary, ie null-found and null-ele
                for (int j = 0; j < Size; j++)
                {
                    if (partn_info[j] == null)
                    {
                        //vcols[0] = (int)Row.Va + (j + 1) % 3;
                        //vcols[1] = (int)Row.Va + (vcols[0] + 1) % 3;
                        //null_found = true;
                        null_ele = (uint?)j;                       
                        break;
                    }
                }

                //construct truth-table
                for (uint count = 0; count < 8; count++)
                {
                    //Ap, Bp, Cp
                    uint? Cp = (count & 0x04) >> 2;
                    uint? Bp = (count & 0x02) >> 1;
                    uint? Ap = (count & 0x01);

                    //uint d2 = count / (nbase * nbase);
                    //uint d1 = (count % d2) / nbase;
                    //uint d0 = (count % (d1 * nbase + d2));

                    //for (int output = (int)Row.Ac; output <= (int)Row.Cc; output++)
                    //{
                    shadow_counter[(int)count, (int)Row.SLNO] = count;
                    shadow_counter[(int)count, (int)Row.Ap] = locked[0];
                    shadow_counter[(int)count, (int)Row.Bp] = locked[1];
                    shadow_counter[(int)count, (int)Row.Cp] = locked[2];

                    uint?[] temp_locked = new uint?[3];
                    temp_locked[0] = locked[0];
                    temp_locked[1] = locked[1];
                    temp_locked[2] = locked[2];

                    //Calculate Ac, Bc, Cc
                    temp_locked = Kernel(temp_locked);
                    shadow_counter[(int)count, (int)Row.Ac] = temp_locked[0];
                    shadow_counter[(int)count, (int)Row.Bc] = temp_locked[1];
                    shadow_counter[(int)count, (int)Row.Cc] = temp_locked[2];

                    uint prob_index1 = 0;
                    uint prob_index2 = 0;


                    //Current row {Ac, Bc, Cc} met purview conditions. Note its past, partitionwise and whollistically
                    if ((inpurview.ElementState.left == null || sglobal.ui2bool(temp_locked[0]) == inpurview.ElementState.left) &&
                         (inpurview.ElementState.center == null || sglobal.ui2bool(temp_locked[1]) == inpurview.ElementState.center) &&
                         (inpurview.ElementState.right == null || sglobal.ui2bool(temp_locked[2]) == inpurview.ElementState.right))
                    {
                        p.P1count = 0;
                        p.P2count = 0;
                        for (int col_index = (int)Row.Ap; col_index < (int)Row.Cp; col_index++)
                        {
                            if (col_index - (int)Row.Ap == null_ele)
                            {
                                //current column is null ie., current element is partition 2
                                prob_index2 = prob_index2 | 1;
                                prob_index2 <<= 1;
                            }
                            else
                            {
                                //current column is not null ie., current element is partition 1
                                prob_index1 = prob_index1 | 1;
                                prob_index1 <<= 1;
                            }
                        }

                        //A row is processed where hit is found. update hit counts of both partitions
                        p.P1ProbDistribn[p.P1count].dist_index = (int)prob_index1;
                        p.P1ProbDistribn[p.P1count].count++;

                        p.P2ProbDistribn[p.P2count].dist_index = (int)prob_index2;
                        p.P2ProbDistribn[p.P2count].count++;
                    }

                }

                //truth-table is completed. Partition p's both parts reportoire's are processed
                //Find probabilities from counts
                for (int p1 = 0; p1 < p.P1count; p1++)
                {
                    p.P1ProbDistribn[p1].prob = (double)p.P1ProbDistribn[p1].count / (double)p.P1count;
                }

                for (int p2 = 0; p2 < p.P1count; p2++)
                {
                    p.P1ProbDistribn[p2].prob = (double)p.P2ProbDistribn[p2].count / (double)p.P2count;
                }

                //Find cross-product
                p.P1xP2count = 0;
                if (p.P1count == 0)
                {
                    //P1 partition is NULL, P2 is whole
                    p.P1xP2count = p.P1count;
                    p.P1xP2 = p.P1ProbDistribn;

                }
                else if (p.P2count == 0)
                {
                    //P2 partition is NULL, P1 is whole
                    p.P1xP2count = p.P1count;
                    p.P1xP2 = p.P1ProbDistribn;
                }
                else {
                    //Both P1 and P2 are not NULL, Take Cross Product P1xP2
                    for (int p1 = 0; p1 < p.P1count; p1++)
                    {
                        for (int p2 = 0; p2 < p.P2count; p2++)
                        {
                            p.P1xP2[p.P1xP2count].dist_index = p.P1xP2count;
                            p.P1xP2[p.P1xP2count].prob = p.P1ProbDistribn[p1].prob * p.P2ProbDistribn[p2].prob;
                            p.P1xP2count++;
                        }
                    }
                }

                double max = p.getmaxProbOfPartn();
                
                if ( > phi)
                {
                    if (max > phi)
                    {
                        phi = max;                        
                    }
                }
            }

            return phi;
        }


        public double GetEffectInformation() { return 0.0; }
        public List<Mechanism> GetEffectReportoire()
        {
            List<Mechanism> EffectRep = new List<Mechanism>();
            if (partition2 == null)
            {
                for (uint cid = 0; cid < 8; cid++)
                {
                    uint nbase = 2;
                    uint d2 = cid / (nbase * nbase);
                    uint d1 = (cid % d2) / nbase;
                    uint d0 = cid % (d1 * nbase + d2);

                    //which among 8 effects are this's effects
                    Mechanism effect = new Mechanism(d2, d1, d0);
                    if (effect.Kernel().locked[0] == this.locked[0] &&
                         effect.Kernel().locked[1] == this.locked[1] &&
                         effect.Kernel().locked[2] == this.locked[2])
                    {
                        EffectRep.Add(effect);
                    }
                }
            }

            return EffectRep;
        }
        
        public void GetPhiMax(Triplet<bool> ElemState, ref double cause_phi_max,  ref Purview ccphimax)
        {
            //null,false,false
            List<Purview> PurviewList = new List<Purview>();
            double cause_phi_max = 0.0;
            Purview core_cause;

            //int boundary = 0;
            for (int cause = 0; cause < Math.Round(Math.Pow(2, Size)); cause++)
            {
                //a, b, c, ab, bc, ac, abc
                //bcc = null-0-0
                //ap     1-null-null
                // .nn, n.n, nn., ..n, .n., n..
                //   001, 010,  011,100, 101, 110, 111
                //
                int[] counter = new int[3];
                counter[2] = cause % 4;
                counter[1] = (cause / 2) % 2;
                counter[0] = cause % 2;


                Purview p = new Purview();
                p.Whole = this;

                //::BCc/Ap, BCc/Bp
                for (int k = 0; k < p.Whole.Size; k++)
                {
                    //Bc=00 means Whole.locked = { null, 0, 0 }
                    //BCc/Ap means purviewlist[0]cause = {111, null, null} that means Ap can assume only those values that can make BCc 00
                    //where as BCp can have unconstrained distribution
                    //BCc/Ap effect means purviewlist effect = {222, null, null} means Ap should become kernel result of BCc 
                    //Ac can have unconstrained distributin uc.
                    if (counter[k] == locked[k])
                    {
                        //here 111 means not null 
                        p.InPurviewCause.locked[k] = 111;
                        //222 means current purview becomes nont null effect
                        p.InPurviewEffect.locked[k] = 222;
                    }
                    else
                    {
                        //
                        p.InPurviewCause.locked[k] = null;
                        p.InPurviewEffect.locked[k] = null;
                    }
                }


                //P1 is given result, it should be evaluated against all other MIP-Cause Reportoires
                //Every iteration gives phi of current Purview with its cause and effect
                p.InPurviewCurrent.Phi= GetPhi(p);

                if (p.InPurviewCurrent.Phi > cause_phi_max)
                {
                    cause_phi_max = p.InPurviewCurrent.Phi;
                    ccphimax = p;
                }
                p.InPurviewCurrent.EffectReportoire = p.InPurviewCurrent.GetEffectReportoire();

                PurviewList.Add(p);
            }

            
        }


    }

    class Triplet<T>
    {
        public T left, center, right;
        public int nbase;

        //Generics Constructor is not allowed
        public void Set(T left, T center, T right)
        {
            this.left = left;
            this.center = center;
            this.right = right;

        }

        public Triplet<T> Get()
        {
            return this;
        }

        public Triplet<T> obj
        {
            get
            {
                return this;
            }

            set
            {
                left = value.left;
                center = value.center;
                right = value.right;
            }
        }

        //public static Counter operator ++(Counter c)
        //{
        //    return new Counter(c.v + 1);
        //}

    }

    class UAS_SOC_IIT_Engine
    {
        string[] BlueForce;
        bool[] BlueDistance;
        double[] BlueScore;
        string[] BlueCluster;
        string[] RedForce;
        public int num_elems;
        public int cluster_size;
        string[] RedSignatures = { "C", "A", "B" };

        //double[] RedScore = new double[num_elems];
        //simple right-rotate is considered. random permutation also can be considered
        public static string[,] Permutations = new string[9, 3];
        public int nbase;
        public int digits;
        public Mechanism[] Mechanisms;


        public UAS_SOC_IIT_Engine(int num_elems, int nbase = 3, int ndigits = 3)
        {
            Debug.Assert(num_elems % 3 == 0);
            this.num_elems = num_elems;
        }

        public void init()
        {

            BlueForce = new string[num_elems];
            BlueDistance = new bool[num_elems];
            BlueScore = new double[num_elems];
            RedForce = new string[num_elems];
            Mechanisms = new Mechanism[num_elems];

            for (int i = 0; i < num_elems; i++)
            {

                Random r = new Random();
                int rn = r.Next(3);
                if (rn == 0) BlueForce[i] = "A";
                else if (rn == 1) BlueForce[i] = "B";
                else if (rn == 2) BlueForce[i] = "C";

                rn = r.Next(3);
                if (rn == 0) RedForce[i] = "A";
                else if (rn == 1) RedForce[i] = "B";
                else if (rn == 2) RedForce[i] = "C";
            }

            //Calculate initial distance vector
            for (int pos = 1; pos < num_elems - 1; pos++)
            {
                Triplet<string> bluecode = new Triplet<string>();
                bluecode.Set(BlueForce[pos - 1], BlueForce[pos], BlueForce[pos + 1]);

                Triplet<string> redcode = new Triplet<string>();
                redcode.Set(RedForce[pos - 1], RedForce[pos], RedForce[pos + 1]);

                Triplet<bool> distance = new Triplet<bool>();
                distance.Set(BlueForce[pos - 1] == RedForce[pos - 1],
                             BlueForce[pos] == RedForce[pos],
                             BlueForce[pos + 1] == RedForce[pos + 1]);

                BlueDistance[pos - 1] = distance.left;
                BlueDistance[pos] = distance.center;
                BlueDistance[pos + 1] = distance.right;

                //Clustering Logic
                if (BlueScore[pos] >= BlueScore[pos - 1] &&
                    BlueScore[pos] >= BlueScore[pos + 1]
                   )
                {

                    if (pos - 2 >= 0 && BlueScore[pos] >= BlueScore[pos - 2] &&
                        pos + 2 < num_elems && BlueScore[pos] >= BlueScore[pos + 2])
                    {
                        //BlueScore[pos] can become center of cluster
                        BlueCluster[pos] = "0";
                        BlueCluster[pos - 1] = "1";
                        BlueCluster[pos + 1] = "-1";
                    }
                    else
                    {
                        BlueCluster[pos] = "*";
                    }
                }
                else
                {
                    BlueCluster[pos] = "*";
                }


            }
        }

        public void next()
        {
            //process every element but first and last in BlueForce
            //The state at this state is t0 or tcur
            //MECHANISM is XOR for RED AND BLUE
            //BUT WHICH CLUSTER
            double min_cephimax = 1.0;
            double avg_phi = 0.0;
            int mincepos = 0;
            int mutation_begin = -4;
            int mutation_end = 4;
            Purview core_cause = new Purview();
            Triplet<bool> elemstate = new Triplet<bool?>();

            double mincphi = 0.0;

            for (int pos = 1; pos < num_elems - 1; pos++)
            {
                int sindex=0;

                uint?[] cur_locked = Mechanisms[pos].locked;
                double cphi = 0.0;
                Purview ccphimax;

                //current state of tracking with RED UAS
                Mechanism m = new Mechanism(cur_locked[0], cur_locked[1], cur_locked[2]);
                elemstate.left = (bool)(sglobal.ui2bool(cur_locked[0]));
                elemstate.center = (bool)sglobal.ui2bool(cur_locked[1]);
                elemstate.right = (bool)sglobal.ui2bool(cur_locked[2]);

                m.GetPhiMax(elemstate, ref cphi, ref core_cause);
                
                if (cphi < mincphi)
                {
                    mincphi = cphi;
                    mincepos = pos;
                }

                        
            }

            for (int pos = 1; pos < num_elems - 1; pos++)
            {
                mutation_begin = mincepos - 4;
                mutation_end = mincepos + 4;
                if (pos >= mutation_begin && pos <= mutation_end)
                {
                    //phi as kalman gain => futuristic
                    //mutation

                    string[] sigs = { "ABC", "BCA", "CAB", "BAC", "ACB", "CBA" };

                    Random rfreq = new Random(5);
                    string rsig = sigs[rfreq.Next(5)];
                    Mechanisms[pos].signatures[0] = rsig[0].ToString();
                    Mechanisms[pos].signatures[1] = rsig[1].ToString();
                    Mechanisms[pos].signatures[2] = rsig[2].ToString();
                }
                else
                {
                    //hill_climbing phase
                    //Mechanism cur_mech = new Mechanism(distance.left, distance.center, distance.right);
                    //Multiple Reportoires possible only in case of multiple distributions
                    //Compute Probability Distribution of Causes
                    Mechanism km = Mechanisms[pos].Kernel();
                }


                //TODO: promote cluster_len as member variable
                int cluster_len = 3;
                for (int sindex = 0; sindex < cluster_len; sindex++)
                {
                    Mechanisms[pos].locked[sindex] =
                        (Mechanisms[pos].signatures[sindex] == RedSignatures[sindex]) ? (uint)1 : (uint)0;
                }

                //Using OAX machine, and presereving current ones what would be core effect?
                uint? Diff0cur = (Convert.ToInt32(Mechanisms[pos].locked) == 1) ? (uint?)1 : (uint?)null;
                uint? Diff1cur = (Convert.ToInt32(Mechanisms[pos].locked) == 1) ? (uint?)1 : (uint?)null;
                uint? Diff2cur = (Convert.ToInt32(Mechanisms[pos].locked) == 1) ? (uint?)1 : (uint?)null;

                //Find DIFF (with true as is and false as null)
                Mechanisms[pos].locked[0] = Diff0cur;
                Mechanisms[pos].locked[1] = Diff1cur;
                Mechanisms[pos].locked[2] = Diff2cur;


            }

        }
    }

    //==================================================================================================
    //==================================================================================================
    class Cluster
    {
        public static int? sid;
        public int id;
        double min_combine_phi = 0.6; //force combining always
        double max_split_phi = 0.59; //disable splitting

        //public int locn;
        //public int numClusters;
        public ConsoleColor bc, fc;
        public string pivot;
        public int px, py, pvel;
        public int top, left, right, bottom;
        public string state;
        public bool deleted = false;
        public bool ignore = false;
        public bool selected = false;

        //Cluster is mechanism of IIT 3.0
        //Cluster is cell which experinces toppling or avalanche of SOC
        //Cluster is set of UAS tracked together by a Radar Network
        public double phi;

        public override string ToString()
        {
            string s = this.GetType().Name + ":id " + id + " deleted: " + deleted.ToString() + " pivot " + pivot + " phi " + phi +
                       " [ top " + top + " left " + left + " bottom " + bottom + " right  " + right + " ] px, py, pvel " +
                       " background " + bc.ToString() + " foreground " + fc.ToString();

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

            int colint = id % 15;
            if (colint >= (int)ConsoleColor.DarkYellow)
            {
                colint++;
            }

            bc = (ConsoleColor)(colint);
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
            string[] cluster_row_indices = new string[sglobal.SkyRow];
            string[] cluster_col_indices = new string[sglobal.SkyCol];

            FgBg[,] cluster_colors = new FgBg[sglobal.SkyRow, sglobal.SkyCol];




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
                       cluster_colors, top, left, bottom, right);
        }

        public bool contains(int posr, int posc)
        {
            if (posr >= top && posr <= bottom && posc >= left && posc <= bottom)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                sglobal.logger.WriteLine("input0 " + "this.phi " + phi);
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

        public List<Cluster> SplitMinPhi(Cluster cc)
        {
            //split among all subclusters
            //split based on intracluster integreties - phis

            //if there are no edges between clusters delete them.
            //if there are edges check strenth {if both are mixes} and then split

            //Split Currrent Cluster into 4-Sub Clusters
            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + cc.id + " METHOD: SplitMinPhi ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                //sglobal.logger.WriteLine("input0 " + "parameter Cluster c " + c);
                sglobal.logger.WriteLine("input0 : " + "parameter Parent Cluster List",
                                          ConsoleColor.Blue, ConsoleColor.White);
                //foreach (Cluster cc in parent_cluster)
                //{

                sglobal.logger.WriteLine(cc.ToString(), ConsoleColor.Black, ConsoleColor.White);
                //cc.print("...split min phi...");
                //}

            }

            List<Cluster> temp_parent_cluster = new List<Cluster>();

            //foreach (Cluster cc in parent_cluster)
            {
                Cluster c0 = null, c1 = null, c2 = null, c3 = null;


                int lr = (cc.left + cc.right) / 2;
                int tb = (cc.top + cc.bottom) / 2;

                int cluster_height = cc.bottom - cc.top + 1;
                int cluster_width = cc.right - cc.left + 1;

                if (cluster_height % 2 != 0 && cluster_width % 2 != 0)
                {

                    //Can't be split horizontally or vertically
                    //let c0 hold the incoming cluster as it is
                    c0 = new Cluster();
                    c0.top = cc.top; c0.left = cc.left; c0.bottom = cc.bottom; c0.right = cc.right;
                }
                else if (cluster_height % 2 != 0 && cluster_width % 2 == 0)
                {
                    //cluster split into 2 clusters, vertically, along y-axis
                    c0 = new Cluster(); c1 = new Cluster();
                    c0.top = cc.top; c0.left = cc.left; c0.bottom = (tb); c0.right = (lr);
                    c1.top = cc.top; c1.left = (lr + 1); c1.bottom = (tb); c1.right = cc.right;
                }
                else if (cluster_height % 2 == 0 && cluster_width % 2 != 0)
                {
                    //cluster split into 2 clusters, horizontally, along x-axis
                    c2 = new Cluster(); c3 = new Cluster();
                    c2.top = cc.top; c2.left = cc.left; c2.bottom = (tb); c2.right = right;
                    c3.top = (tb + 1); c3.left = cc.left; c3.bottom = cc.bottom; c3.right = right;
                }
                else
                {
                    Debug.Assert(cluster_height % 2 == 0 && cluster_width % 2 == 0);
                    //Cluster is split both horizontally and vertically
                    c0 = new Cluster(); c1 = new Cluster(); c2 = new Cluster(); c3 = new Cluster();

                    c0.top = cc.top; c0.left = cc.left; c0.bottom = (tb); c0.right = (lr);
                    c1.top = cc.top; c1.left = (lr + 1); c1.bottom = (tb); c1.right = cc.right;
                    c2.top = (tb + 1); c2.left = cc.left; c2.bottom = cc.bottom; c2.right = (lr);
                    c3.top = (tb + 1); c3.left = (lr + 1); c3.bottom = cc.bottom; c3.right = cc.right;
                }

                Cluster tempc = new Cluster();
                string cc_pivot = cc.pivot;

                //temp_parent_cluster.RemoveAll(x => x.id == cc.id);
                for (int i = 0; i < temp_parent_cluster.Count; i++)
                {
                    if (temp_parent_cluster[i].id == cc.id)
                    {
                        temp_parent_cluster[i].deleted = true;
                    }
                }

                //inherit characteristics from current cluster
                if (c0 != null)
                {
                    tempc = new Cluster();
                    tempc.pivot = cc_pivot;
                    tempc.ignore = true;
                    tempc.top = c0.top; tempc.left = c0.left; tempc.bottom = c0.bottom; tempc.right = c0.right;
                    temp_parent_cluster.Add(tempc);
                }

                if (c1 != null)
                {
                    tempc = new Cluster();
                    tempc.pivot = cc_pivot;
                    tempc.ignore = true;
                    tempc.top = c1.top; tempc.left = c1.left; tempc.bottom = c1.bottom; tempc.right = c1.right;
                    temp_parent_cluster.Add(tempc);
                }

                if (c2 != null)
                {
                    tempc = new Cluster();
                    tempc.pivot = cc_pivot;
                    tempc.ignore = true;
                    tempc.top = c2.top; tempc.left = c2.left; tempc.bottom = c2.bottom; tempc.right = c2.right;
                    temp_parent_cluster.Add(tempc);
                }

                if (c3 != null)
                {
                    tempc = new Cluster();
                    tempc.pivot = cc.pivot;
                    tempc.ignore = true;
                    tempc.top = c3.top; tempc.left = c3.left; tempc.bottom = c3.bottom; tempc.right = c3.right;
                    temp_parent_cluster.Add(tempc);
                }
            }


            state = "@Exit SplitMinPhi: " + " Output ref Param: Parent Cluster List Count: "
                                          + temp_parent_cluster.Count;
            sglobal.logger.WriteLine(state);

            return temp_parent_cluster;
        }

        public bool canBeCombined(Direction d, Cluster c1, Cluster c2)
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
                for (int i = 0; i < sglobal.SkyRow; i++)
                {
                    for (int j = 0; j < sglobal.SkyCol; j++)
                    {
                        sc.Canvas[i, j] = sglobal.Sky[i, j];
                        if (c1.contains(i, j))
                        {
                            sc.CanvasColors[i, j] = new FgBg(c1.fc, c1.bc);
                        }

                        if (c2.contains(i, j))
                        {
                            sc.CanvasColors[i, j] = new FgBg(c2.fc, c2.bc);
                        }
                        else
                        {
                            sc.CanvasColors[i, j] = new FgBg(ConsoleColor.White, ConsoleColor.Black);
                        }
                    }
                }
                //sc.CanvasColors = sc.DefaultColors;
                //sc.Compose(toBeClusters, false);


                sc.Compose(ref sc.Canvas, ref sc.CanvasColors, sc.SkyOrder, sc.SkyOrder, sc.Clusters, false);
                sglobal.logger.WriteMat("Would be Cluster Couple...", sglobal.single_star, sglobal.single_star,
                                        sc.Canvas, sglobal.SkyRow, sglobal.SkyCol, sc.CanvasColors);
            }

            Cluster c = new Cluster();



            //Adjacency check
            //ANCHOR: Should make RANDOM JUMP POSSIBLE
            if (c1.top == c2.top || c1.bottom == c2.bottom)
            {
                if ((d == Direction.RIGHT) && (c1.right == c2.left - 1))
                {
                    //aligned side-by-side c1-left, c2-right
                    cbcombined = true;
                }
                else if ((d == Direction.LEFT) && (c1.left == c2.right + 1))
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
                if ((d == Direction.BOTTOM) && (c1.bottom == c2.top - 1))
                {
                    //c1 on top of c2
                    cbcombined = true;
                }
                else if ((d == Direction.TOP) && (c2.bottom == c1.top - 1))
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

        public Cluster CombineTwoClusters(Cluster c1, Cluster c2, List<Cluster> parent_clusters)
        {
            //...similarly select nearly_max_c
            Cluster c = new Cluster();

            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + c.id + " METHOD: CombineTwoClusters ";
            if (sglobal.logger.entry_exit)
            {

                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 param : c1 " + c1);
                sglobal.logger.WriteLine("input0 param : c2 " + c2);
                //sglobal.logger.WriteLine("input0 param : parent_clusters");
                //foreach (Cluster cc in parent_clusters)
                //{
                //    sglobal.logger.WriteLine(cc.ToString());
                //}
            }


            if (c1.top == c2.top || c1.bottom == c2.bottom)
            {
                if (c1.right == c2.left - 1)
                {
                    //aligned side-by-side c1-left, c2-right
                    c.pivot = c1.pivot;
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
                else if (c2.bottom == c1.top - 1)
                {
                    //aligned one below another c2-top, c1-below
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

        public List<Cluster> SplitAndCombine(ref List<Cluster> top_level_clusters)
        {
            bool combined = false;
            bool split = false;
            List<Cluster> next_gen_clusters = new List<Cluster>();
            //List<int> visited = new List<int>();

            state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: SplitAndCombine ";
            if (sglobal.logger.entry_exit)
            {
                sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
                sglobal.logger.WriteLine("input0 param top_level_clusters ", ConsoleColor.White, ConsoleColor.Blue);
                for (int i = 0; i < top_level_clusters.Count; i++)
                {
                    top_level_clusters[i].ignore = false;
                    sglobal.logger.WriteLine(top_level_clusters[i].ToString());
                }
            }

            //except_list.AddRange(near_max_sc);          

            int iter1 = 0;
            for (iter1 = 0; iter1 < top_level_clusters.Count; iter1++)
            {
                if (top_level_clusters[iter1].deleted)
                {
                    //Deleted Cluster. Move forward.
                    continue;
                }
                combined = split = false;
                //iter1 = (iter1 + 1) % top_level_clusters.Count;
                Cluster c1 = top_level_clusters[iter1];

                //check whether c1 is of low fitness or high
                if (c1.phi < max_split_phi)
                {
                    //c1 fitness is low. Split it    

                    next_gen_clusters.AddRange(SplitMinPhi(c1));
                    top_level_clusters[iter1].deleted = true;
                }
                else
                {
                    //c1 fitness is high. Combine it with another high phi top-level cluster                    {
                    //check whether it is already visited
                    //var rslt = visited.Find(x => x == c2.id);
                    //for (int dir = 0; dir < 4; dir++)
                    {
                        //Direction SearchDir = (Direction)dir;
                        for (int iter2 = 0; !combined && iter2 < top_level_clusters.Count; iter2++)
                        {
                            Cluster c2 = top_level_clusters[iter2];
                            //Math.Abs(c1.phi - c2.phi) < phi_tolerance
                            //So if it is more than minimum phi, it will branch
                            if (!c1.deleted && !c2.deleted && c1.id != c2.id && canBeCombined(sglobal.SearchDir, c1, c2) &&
                                !c1.ignore && !c2.ignore &&
                                c1.phi > c1.min_combine_phi && c2.phi > c2.min_combine_phi)
                            {
                                //sited.Add(c2.id);                                
                                //it will be combined.                                
                                Cluster c = CombineTwoClusters(c1, c2, top_level_clusters);
                                next_gen_clusters.Add(c);

                                //c = top_level_clusters.Find(x => x.id == c1.id);

                                //mark parents of combined clusters as 'deleted'
                                for (int i = 0; i < top_level_clusters.Count; i++)
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

                                //c = top_level_clusters.Find(x => x.id == c2.id);
                                //top_level_clusters.Remove(c);
                                combined = true;
                            }
                        }
                    }
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
                sglobal.logger.WriteLine("top_level_clusters....");
                foreach (Cluster cc in top_level_clusters)
                {
                    sglobal.logger.WriteLine(cc.ToString());
                    cc.print(".............cc.id.ToString()............");
                }
            }

            top_level_clusters.RemoveAll(x => x.deleted);
            top_level_clusters.AddRange(next_gen_clusters);
            return top_level_clusters;
        }

        //Containment Check
        //TODO:Check whether Combine can be made separate routine...
        //TODO:Check whether to preserve splitcount == combinecount
        //TODO:  => i.e.. preserve population count;

        //public List<Cluster> SplitAndCombine(ref List<Cluster> top_level_clusters)
        //{
        //    bool combined = false;
        //    List<int> visited = new List<int>();
        //    int iter1 = 0;

        //    state = "@Entry: Class:" + this.GetType().Name + " Object: id " + id + " METHOD: SplitAndCombine ";
        //    if (sglobal.logger.entry_exit)
        //    {
        //        sglobal.logger.WriteLine(state, ConsoleColor.White, ConsoleColor.Blue);
        //        sglobal.logger.WriteLine("input0 param top_level_clusters ", ConsoleColor.White, ConsoleColor.Blue);

        //        foreach (Cluster cc in top_level_clusters)
        //        {
        //            sglobal.logger.WriteLine(cc.ToString());
        //        }
        //    }

        //    for (iter1 = 0; iter1 < top_level_clusters.Count; iter1++)
        //    {
        //        top_level_clusters[iter1].visited = false;
        //    }

        //    int iter_start = -2;
        //    int inter_end = top_level_clusters.Count;
        //   // Dictionary<int, int> pairs = new Dictionary<int, int>();


        //    while (!combined)
        //    {
        //        bool first_iter = true;
        //        iter_start++;
        //        iter1 = iter_start;

        //        for (int i=0; i < top_level_clusters.Count; i++)
        //        {
        //            top_level_clusters[i].selected = false;
        //        }




        //        ////while (true)
        //        //{
        //        //    //Console.WriteLine("In Loop...");
        //        //    var rslt = top_level_clusters.FindAll(x => x.selected);
        //        //    if (!first_iter && ( rslt.Count == 0))
        //        //    {
        //        //        break;
        //        //    }
        //        //    first_iter = false;


        //        //    combined = false;
        //        //    iter1 = (iter1 + 1) % top_level_clusters.Count;
        //        //    top_level_clusters[iter1].selected = true;


        //            Cluster c1 = top_level_clusters[iter1];

        //            //overlap check. check whether two clusters overlap
        //            //for (int i=0; i < top_level_clusters.Count; i++)
        //            //{
        //            //    if ((c1.id != top_level_clusters[i].id) &&
        //            //        (top_level_clu.contains(c2.top, c2.left) || c1.contains(c2.bottom, c2.right)))
        //            //    {
        //            //        //yes overlap is there, cannot be combined
        //            //        return false;
        //            //    }

        //            //}



        //            //check whether c1 is of low fitness or high
        //            if (c1.phi < max_split_phi)
        //            {
        //                //c1 fitness is low. Split it
        //                //Split cluster will be deleted and newly added will be marked visited
        //                //inside the SplitMinPhi
        //                if (!c1.deleted && !c1.visited)
        //                {
        //                    SplitMinPhi(ref top_level_clusters);
        //                }
        //            }
        //            else
        //            {
        //                //c1 fitness is high. Combine it with another high phi top-level cluster                    {
        //                //check whether it is already visited
        //                //var rslt = visited.Find(x => x == c2.id);

        //                for (int iter2 = 0; iter2 < top_level_clusters.Count; iter2++)
        //                {
        //                    Cluster c2 = top_level_clusters[iter2];
        //                    //Math.Abs(c1.phi - c2.phi) < phi_tolerance
        //                    if (!c1.deleted && !c2.deleted && !c1.visited && !c1.visited &&
        //                        c1.id != c2.id && canBeCombined(c1, c2) &&
        //                        c1.phi > c1.min_combine_phi && c2.phi > c2.min_combine_phi)
        //                    {
        //                        //sited.Add(c2.id);
        //                        //spinning random roulette, for selection
        //                        //wheel will generate a random number between 0 and 0.1; So if it is less than phi,
        //                        //it will be combined. Very high phis will not be combined where as intermediate will
        //                        Cluster c = CombineTwoClusters(c1, c2, top_level_clusters);
        //                        top_level_clusters.Add(c);

        //                        c = top_level_clusters.Find(x => x.id == c1.id);

        //                        for (int i = 0; i < top_level_clusters.Count; i++)
        //                        {
        //                            if (top_level_clusters[i].id == c1.id)
        //                            {
        //                                top_level_clusters[i].deleted = true;
        //                            }

        //                            if (top_level_clusters[i].id == c2.id)
        //                            {
        //                                top_level_clusters[i].deleted = true;
        //                            }
        //                        }

        //                        c = top_level_clusters.Find(x => x.id == c2.id);
        //                        top_level_clusters.Remove(c);
        //                        combined = true;
        //                        //c2 loop
        //                        break;
        //                    }
        //                }
        //                //top_level_clusters.RemoveAll(x => x.deleted);



        //            }
        //        }
        //    }
        //    state = this.GetType().Name + "OBJECT: id " + id + " METHOD: SplitAndCombine";
        //    if (sglobal.logger.entry_exit)
        //    {
        //        sglobal.logger.WriteLine(state);
        //    }
        //    state = "@Exit: SplitAndCombine " + "Output ref Param: top_level_list ";
        //    if (sglobal.logger.entry_exit)
        //    {
        //        sglobal.logger.WriteLine(state);
        //        sglobal.logger.WriteLine("top_level_clusters....");
        //        foreach (Cluster cc in top_level_clusters)
        //        {
        //            sglobal.logger.WriteLine(cc.ToString());
        //            cc.print(".............cc.id.ToString()............");
        //        }
        //    }

        //    return top_level_clusters;
        //}       
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

            //***Run Configurations.....

            int test_case_id = 3;
            sglobal.tick_limit = 2;
            sglobal.tick_count = 0;
            InitTestCases();
            TestCase input_tc = TestCases.ElementAt(test_case_id);
            sglobal.logger.Open("log_" + input_tc.id + "_" + date + ".htm", false);
            SharedCanvas skyscope = new SharedCanvas(Force.BLUE, 2, input_tc.tc);
            skyscope.initCanvas(input_tc.tc);



            string title = "TC:" + input_tc.id.ToString() + "  " + input_tc.desc +
                           "Input Sky: in BEGINNING " +
                            sglobal.tick_count + " Num of Clusters " + skyscope.Clusters.Count;

            sglobal.init_chmap();
            sglobal.logger.WriteHead();
            sglobal.logger.WriteLine("UAS SWARM Attrition/Reinforcement Recommendion System......");
            sglobal.logger.WriteLine("Class Program Method: Main @Entry ", ConsoleColor.White, ConsoleColor.Blue);
            sglobal.logger.WriteLine(title, ConsoleColor.Red, ConsoleColor.Yellow);
            FgBg testcolor = new FgBg(ConsoleColor.DarkRed, ConsoleColor.DarkYellow); // background DarkYellow foreground White
            sglobal.logger.WriteLine("Test Yellow Color...", testcolor.Bg, testcolor.Fg);



            //sglobal.logger.WriteLine("Input RED Sky: ", );
            //sglobal.logger.Save(true);
            //skyscope.initCanvas(globals.Sky);
            //skyscope.Compose();
            //skyscope.Paint();


            string tick = "y";

            while (sglobal.tick_count < sglobal.tick_limit && tick != "n" && tick != "q")
            {
                skyscope.next(input_tc);

                //Move tick to next state
                //TODO: Delete tick_count and keep only sglobal.tick_count
                sglobal.tick_count++;
            }
            Console.WriteLine("Press Any Key...");
            tick = Console.ReadKey().KeyChar.ToString().ToLower();

            sglobal.logger.WriteLine("PROGRAM RAN TO COMPLETION");
            sglobal.logger.Close(true, true);
        }
    }
}

    //        foreach (Partition partn in partition_list)
    //        {
    //            double info1 = partn.P1Cause.GetCauseInformation();
    //            double info2 = partn.P1Current.GetCauseInformation();
    //            double phi;

    //            if (info1 <= info2)
    //            {
    //                phi = partn.P1Current.DistanceFrom(partn.P1Cause);
    //                core_mech = partn.P1Current;
    //            }
    //            else
    //            {
    //                phi = partn.P1Cause.DistanceFrom(partn.P1Current);
    //                core_mech = partn.P1Cause;
    //            }

    //            if (phi > pur.cause_phi_max)
    //            {
    //                pur.cause_phi_max = phi;
    //                pur.CoreCause = core_mech;
    //            }
    //        }

    //        //List<Partition> partition_list = pur.Whole.GetPartitionList(pur.InPurviewCurrent);
    //        //Effect Side
    //        pur.effect_phi_max = 0.0;
    //        foreach (Partition partn in partition_list)
    //        {
    //            double info1 = partn.P2Effect.GetEffectInformation();
    //            double info2 = partn.P2Current.GetEffectInformation();
    //            double phi;
    //            if (info1 <= info2)
    //            {
    //                phi = partn.P2Current.DistanceFrom(partn.P2Effect);
    //                core_mech = partn.P2Current;
    //            }
    //            else
    //            {
    //                phi = partn.P2Effect.DistanceFrom(partn.P2Current);
    //                core_mech = partn.P2Effect;
    //            }

    //            if (phi > pur.effect_phi_max)
    //            {
    //                pur.effect_phi_max = phi;
    //                pur.CoreEffect = core_mech;
    //            }

    //            pur.ce_phi_max = pur.cause_phi_max <= pur.effect_phi_max ? pur.cause_phi_max : pur.effect_phi_max;
    //        }

    //        new_purviews.Add(new Purview(pur));
    //        if (min_cephimax < pur.ce_phi_max)
    //        {
    //            //every position will define one purview
    //            avg_phi += pur.ce_phi_max;
    //            min_cephimax = pur.ce_phi_max;
    //            mincepos = pos;
    //        }
    //    }
    //}

    //        for (uint cid = 0; cid < 8; cid++)
    //        {


    //            Mechanism cause = new Mechanism(d2, d1, d0);
    //            if (cause.Kernel().locked[0] == this.locked[0] &&
    //                cause.Kernel().locked[1] == this.locked[1] &&
    //                cause.Kernel().locked[2] == this.locked[2])
    //            {
    //                CauseRep.Add(cause);
    //            }
    //        }
    //    }

    //    return CauseRep;
    //}

    ////enable virtual columns
    //if ( j == 0)
    //{
    //    vcol1 = Row.Vb;
    //    vcol2 = Row.Vc;
    //}
    //else if (j ==1)
    //{
    //    vcol1 = Row.Va;
    //    vcol2 = Row.Vc;
    //}
    //else
    //{
    //    vcol1 = Row.Va;
    //    vcol2 = Row.Vb;
    //}

    //shadow_counter[0] = shadow_counter[1] = shadow_counter[2] = 0;
    //string mode = "IPV_NORMAL_OPV_NOISE"; //IPV Inside Purview Variable, OPV = Outside....
    //if (locked[p] == NULL)
    //{

    //}


    //else 
    //if (BlueScore[pos] >= BlueScore[pos - 1] &&
    //         BlueScore[pos] >= BlueScore[pos + 1])
    //{ 

    //it does'nt matter which cluster has engaged each other.
    //this enables the arrangement of two forces standing face to face
    //every cluster will try to engage dead-opposite cluster to them. only difference
    //in locking-signature


    //Triplet<string> bluecode = new Triplet<string>();
    //bluecode.Set(BlueForce[pos - 1], BlueForce[pos], BlueForce[pos + 1]);

    //Triplet<string> redcode = new Triplet<string>();
    //redcode.Set(RedForce[pos - 1], RedForce[pos], RedForce[pos + 1]);

    //it does'nt matter which cluster has engaged each other.
    //this enables the arrangement of two forces standing face to face
    //every cluster will try to engage dead-opposite cluster to them. only difference
    //in locking-signature
    //distance = new Triplet<bool>();
    //distance.Set(BlueForce[pos - 1] == RedForce[pos - 1],
    //             BlueForce[pos] == RedForce[pos],
    //             BlueForce[pos + 1] == RedForce[pos + 1]);

    //Mechansisms[pos] = new Mechanism(distance.left, distance.center, distance.right);


    //public List<Partition> GetPartitionList(Purview inpurview)
    //{
    //    //this is supposed to be current
    //    List<Partition> PartitionList = new List<Partition>();

    //    Partition p = new Partition();
    //    p.P1Cause.SetLockSig(null, 900);
    //    p.P1Current.SetLockSig(null, 900);

    //    PartitionList.Add(p);

    //    return PartitionList;





    //This  generates partitions of  inpurview.StateElement.
    //inpurview.StateElements => BCc:00 
    //inpurview.InPurviewCurrent => BCc 
    //inpurview.InPurviewCause   => Ap
    //------------------------------------------------------------
    //PartitionList p
    //p.P1CauseGiven   => [] =>null, null,  null
    //p.P1CauseP1Result => Cc
    //
    //p.P2CauseGiven = Ap = 1, null, null
    //p.P2CauseRslt = []
    //generate binary counter till 0 to 2^(n-1), for 3 values 2^(3-1) => 2^2 => 4
    //000, ,001 010, 011, 100

    //Find Core Cause and Effect with correspondin phimax

    //List<Purview> purviews = Mechanisms[pos].GetPurviewList();
    //List<Purview> new_purviews = new List<Purview>();

                //Cause Side
//                foreach (Purview pur in purviews)
//                {
//                    List<Partition> partition_list = pur.Whole.GetPartitionList(pur.InPurviewCurrent);
//    pur.cause_phi_max = 0.0;
//                    Mechanism core_mech;
//}



    //{phi|ABC}, {A|BC}, {B|AC}, {C|AB}
    //partitions can be generated with nC0 + nC1 +......nCn
    //for (int count = 0; count < Math.Round(Math.Pow(2, inpurview.Whole.Size)-1); count++)
    //in interest of time, we will hardcode combinations for three literals.
    //{
    //    for (int partn_bar = 0; partn_bar < inpurview.Whole.Size; partn_bar++)
    //    {

    //        inpurview.InPurviewCurrent.locked[partn_bar] = ;
    //        p.P1Cause = inpurview.InPurviewCause;

    //        p.P2Current = inpurview.InPurviewCurrent.locked[xxx];
    //        p.P2Effect = inpurview.InPurviewEffect.locked[xxx];
    //    }
    //}
    //return new List<Partition>();
}

//public double GetPhi()
//{
//    //cause-phi
//    double minphi = 1.0;
//    Mechanism mip = new Mechanism(false, false, false);
//    foreach (Mechanism cr in CauseReportoire)
//    {
//        double distance = p.GetDistance(cr);
//        if (distance < minphi)
//        {
//            minphi = distance;
//            mip = cr;
//        }
//    }

//    double cphi = minphi;
//    Mechanism cmip = mip;

//    //effect-phi
//    minphi = 1.0;
//    mip = new Mechanism(false, false, false);
//    foreach (Mechanism cr in CauseReportoire)
//    {
//        double distance = GetDistance(cr);
//        if (distance < minphi)
//        {
//            minphi = distance;
//            mip = cr;
//        }
//    }

//    double ephi = minphi;
//    Mechanism emip = mip;

//    return (cphi < ephi) ? cphi : ephi;
//}

//public List<Partition> GetPartitionList(Triplet<string> whole, ref int minIndex)
//{
//    return null;
//}

////Different prob distributions will be caused only when random inputs are there
//public double PhiOfCauseReportoire(Triplet<string> whole, int cause_reportoire_id)
//{

//    return 0.0;
//}

//public double PhiOfEffectReportoire(Triplet<string> whole, int cause_reportoire_id)
//{

//    return 0.0;
//}

//public void Distance(double[] reportoire1, double[] reportoire2)
//{

//}



//string[,] tempCanvas = new string[SkyOrder, SkyOrder];
//FgBg[,] tempColors1 = new FgBg[SkyOrder, SkyOrder];
//for (int i = 0; i < SkyOrder; i++)
//{
//    for (int j = 0; j < SkyOrder; j++)
//    {
//        tempCanvas[i, j] = sglobal.Sky[i, j];
//        tempColors1[i, j] = DefaultColors[i, j];
//    }
//}