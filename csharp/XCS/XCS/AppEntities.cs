using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POI_XCS
{
    class Pair<T1, T2>
    {
        public T1 first;
        public T2 second;

        public Pair(T1 f, T2 s)
        {
            first = f;
            second = s;
        }

        public T1 getfirst()
        {
            return first;
        }

        public T2 getsecond()
        {
            return second;
        }
    }

    static class globals
    {
        public static StreamWriter LogFile;
        public static uint tick;
        public static uint scenario_num;
        public static bool onconsole=true;
        public static bool onfile = true;
        public static bool not_over = true;

        public static string path = "C:\\Users\\rvjos\\Documents\\";
        public static string filename = "poi_xcs_log_02.txt";

        public static void Init_Log()
        {
                                      
            LogFile = new StreamWriter(path+filename);
            LogFile.WriteLine("BEGIN:" + DateTime.Now.ToString());
            LogFile.Close();
        }

        public static void dumpLog(string s, bool onConsole=true, bool onFile=true)
        {
            //onConsole
            if (onConsole)
            {
                Console.WriteLine("[" + globals.tick.ToString() + "]:" + s);
            }

            //onFile
            if (onFile)
            {
                LogFile = new StreamWriter(path+filename, true);
                LogFile.WriteLine("[" + globals.tick.ToString() + "]:" + s);
                LogFile.Close();
            }
        }

        //----------------------------------------------------- DISPLAY------------------------------------------
        static uint max_width = 174, max_height = 48;
        static uint w = 174 / 2, h = 48 / 2;

        public static void dumpLog(List<string> sl, bool onConsole=true, bool onFile=true)
        {
            //onConsole
            foreach (string s in sl)
            {
                dumpLog(s, onConsole, onFile);
            }       
        }

        public static console_win win2;
        public static void Init_Display(ref console_win win)
        {
            uint w = 174 / 2; // (uint)Console.WindowWidth / 2;
            uint h = 48; // (uint)Console.WindowHeight;
            win = new console_win(1, 1, w, h, ConsoleColor.Green, ConsoleColor.Black);
            win2 = new console_win(w + 2, 1, w, h, ConsoleColor.Cyan, ConsoleColor.Black);
        }


        public static void displayScenario1(ref console_win win, ref console_win win2, Battle b)
        {
            //win.clear();
            win.puttext(b.radars.First().posx, b.radars.First().posy, b.radars.First().symbol, ConsoleColor.White, ConsoleColor.Red);
            win.puttext(b.aircraft.x, b.aircraft.y, b.aircraft.symbol, ConsoleColor.White, ConsoleColor.Blue);


            win2.puttext(72, 3, b.radars.First().symbol, ConsoleColor.White, ConsoleColor.Red);
            win2.draw_box(62, 1, 21, 5);

            //win.draw();
            //win2.draw();

            //Console.ResetColor();
        }

    }

    //--------------------------------RWR related Tasks -------------------------------------------

    class Intercept
    {
        public uint trackid;
        public uint radarid;
        public uint freq;
        public uint priority;
        public uint mb_azim;
        public uint pri;

        public void Print()
        {
            //Console.Write(band.ToString() + priority.ToString() + rect.ToString());
        }

        public string fieldNamesToString()
        {
            return "INTERCEPT" + "\t" +
                "trackid"  + "\t" +
                "radarid"  + "\t" +
                "freq"     + "\t" +
                "pri" + "\t" +
                "mb_azim"  + "\t" +
                "priority";
        }

        public void fieldvalsToString(ref List<string> sl)
        {
            sl.Add("INTERCEPT");
            sl.Add(trackid.ToString() + "\t" +
            radarid.ToString()        + "\t" +
            freq.ToString()           + "\t" +
            pri.ToString()            + "\t" +
            mb_azim.ToString()        + "\t" +
            priority.ToString()       + "\t"             
            );
        }      
    
        

        public Intercept(uint trackid, uint rid, uint freq, uint pri, uint mb_azim, uint priority)
        {
            this.trackid = trackid;
            radarid = rid;
            this.freq = freq;
            this.pri = pri;
            this.mb_azim = mb_azim;
            this.priority = priority;          
        }

        public bool isSameAs(Intercept icept)
        {
            int azim_window = 5; //in deg;
            int min_azim = (int)mb_azim - azim_window;
            if (min_azim < 0)
            {
                //min_azim will be subtracted from 360 as it is -ve
                min_azim = 360 + min_azim;
            }

            int max_azim = ((int)mb_azim + azim_window) % 360;
            if (freq == icept.freq && pri == icept.pri)
            {
                if (min_azim < max_azim)
                {
                    if (icept.mb_azim >= min_azim && icept.mb_azim <= max_azim)
                        return true;
                    else
                        return false;
                }
                if (icept.mb_azim >= min_azim && icept.mb_azim < 360 ||
                    icept.mb_azim > 0 && icept.mb_azim < max_azim)
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
    }

    class interceptTrack
    {
        public int srcid;
        List<Intercept> track;
    }

      
    class Rwr
    {
        //This list is unknown to RWR. Included here
        //Just because it is concerned  with this Rwr
        //Global data  Unknown to Rwr
        public uint band=1;
        public uint duration = 1;
        public RxUnit rxunit = new RxUnit();
        public List<List<Intercept>> InterceptTracks;
        
        //set by battle engine
        public List<Intercept> curIlluminations;
        public List<Intercept> curIntercepts;
        public int[,] InterceptMat;
        public uint azim;

        
        
        public Rwr()
        {
            InterceptMat = new int[20, 20];
            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 20; j++)
                    InterceptMat[i, j] = 0;

            curIlluminations = new List<Intercept>();
            InterceptTracks = new List<List<Intercept>>();
        }

        public void fieldvalsToString(ref List<string> sl)
        {
            sl.Add("RWR");
            sl.Add(band.ToString() + "\t" + duration.ToString() + "\t");
            foreach (List<Intercept> itrk in InterceptTracks)
            {
                sl.Add("TRACK");
                foreach (Intercept icept in itrk)
                {
                    icept.fieldvalsToString(ref sl);
                }
            }
        }

        public void Update()
        {
    
        }

    }

    //--------------------------------

    class TimedWayPoint
    {
        public uint x;
        public uint y;
        public uint z;
        public uint time;

        public TimedWayPoint(uint x, uint y, uint z, uint time)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.time = time;
        }

    }


    //--------------------------------------------------------------------

    class Aircraft
    {
        public List<TimedWayPoint> plannedPath;
        public uint x;
        public uint y;
        public uint azim;
        public string symbol;

        public Aircraft(List<TimedWayPoint> p)
        {
            plannedPath = p;
        }

        public void fieldvalsToString(ref List<string> sl)
        {
            string s = "AIRCRAFT\r\n";
            s = x.ToString() + "\t" +
            y.ToString() + "\t" +
            azim.ToString() + "\t" +
            symbol.ToString();
            sl.Add(s);
            
            foreach (TimedWayPoint twp in plannedPath)
            {
               sl.Add("twp\t" + twp.time.ToString() + "\t"+ twp.x.ToString() + "\t" + 
                                        twp.y.ToString() + "\t" + twp.z.ToString() + "\r\n");
            }

            
        }
           

        public void Update()
        {
            if (globals.scenario_num == 1)
            {
                //assuming it receives every tick
                double dist = 333.0 / 1000.0; //in km
                dist = dist * 0.75; //in pixels (48rows/64km)
                y = (uint)Math.Round(y + dist); // in km
            }
            
            if (Math.Abs(x - 20) < 0.01 && Math.Abs(y - 10) < 0.01)
            {
                globals.not_over = false ;
            }
        }
    }


    //---------------------------------------------------------

    class RadarType
    {
        public uint rtypeid;
        public uint priority;
        public uint freqmin;
        public uint freqmax;
        public uint pri_min;
        public uint pri_max;
        public uint intra_pulse_type;
        public uint freq_agility_type;
        public uint scan_type;//0-circular, 1-sector, 2-conical, 3
        public double scan_time_avg;
        public double scan_time_std;

        public RadarType(double[] init_arr)
        {
            rtypeid = Convert.ToUInt32(Math.Round(init_arr[0], 0));
            priority = Convert.ToUInt32(Math.Round(init_arr[1], 0));
            freqmin = Convert.ToUInt32(Math.Round(init_arr[2], 0));
            freqmax = Convert.ToUInt32(Math.Round(init_arr[3], 0));
            pri_min = Convert.ToUInt32(Math.Round(init_arr[0], 0));
            pri_max = Convert.ToUInt32(Math.Round(init_arr[0], 0));
            intra_pulse_type = Convert.ToUInt32(Math.Round(init_arr[0], 0));
            scan_type = Convert.ToUInt32(Math.Round(init_arr[0], 0));
            scan_time_avg = Convert.ToUInt32(Math.Round(init_arr[0], 0));
            scan_time_std = Convert.ToUInt32(Math.Round(init_arr[0], 0));
        }
    }



   
    class Radar
    {
        public uint radarId;
        public uint rtype;
        public string symbol;
        public uint scanrate;
        public uint scan_interval;
        public uint freq;
        public uint pri;
        public uint pw;
        public uint posx;
        public uint posy;
        public uint mb_azim;
        public uint beam_width;//main beam azimuth

        public static string fieldNamesToString()
        {
            return "radarId" + "\t" +
            "symbol" + "\t" +
            "scanrate" + "\t" +
            "scan_interval" + "\t" +
            "freq" + "\t" +
            "pri" + "\t" +
            "pw" + "\t" +
            "posx" + "\t" +
            "posy" + "\t" +
            "mb_azim" + "\t" +
            "beam_width" + "\t";
       
    }

        public void fieldvalsToString (ref List<string> sl)
        {
            string s = "CLASS:RADAR\r\n";
            s += radarId.ToString() + "\t" +
                rtype.ToString() + "\t" +
                symbol.ToString() + "\t" +
                scanrate.ToString() + "\t" +
                scan_interval.ToString() + "\t" +
                freq.ToString() + "\t" +
                pri.ToString() + "\t" +
                pw.ToString() + "\t" +
                posx.ToString() + "\t" +
                posy.ToString() + "\t" +
                mb_azim.ToString() + "\t" +
                beam_width.ToString() + "\t" +
                radarId.ToString();

               

                sl.Add(s);            
        }

        public Radar(uint pradarId, uint prtype, uint pscanrate, uint pscan_interval, uint pfreq, uint ppri, uint ppw,
                     uint x, uint y, string sym="R1")
        {
            radarId       = pradarId;
            rtype         = prtype;
            scanrate      = pscanrate;
            //scan_interval = pscan_interval;
            scan_interval = 72;
            freq          = pfreq;
            pri           = ppri;
            pw            = ppw;
            symbol        = sym;
            posx          = x;
            posy          = y;
            beam_width = 10;

            
        }

 

        public void Update()
        {
            if (globals.tick % 5 == 0)
            {
                mb_azim++;
                mb_azim = mb_azim % 360;
            }
                
        }
    }

    enum State
    {
        STATE0=0,STATE1, STATE2, STATE3
    }


    class RxUnit
    {
        public uint rxFreqMin;
        public uint rxFreqMax;
        
        public bool radarDetected;
        public uint state;
        public int  duration;
        public bool phi;//no input;

        public uint rxBufCount = 0;
        public Radar[] rxbuf = new Radar[256];

        public void startRx(uint band)
        {          
        }

        public void stopRx(uint band)
        {
        }
        
        
    }

    class Battle
    {
        public List<RadarType> radarTypes;
        public List<Radar> radars;
        public Aircraft aircraft;
        public Rwr rwr;
        public bool ac_hit;
        public string[,] global_view = new string[20, 20]; 

        public Battle()
        {
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                    global_view[i, j] = String.Empty;
        }

        public bool MissionComplete()
        {
            int count_pathpts = aircraft.plannedPath.Count;

            //Last point reached, mission complete
            if (aircraft.x == aircraft.plannedPath[count_pathpts-1].x &&
                aircraft.y == aircraft.plannedPath[count_pathpts-1].y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public uint[,] BattleState = new uint[18, 25];

        public void SetUp_Scenario_1()
        {
            int num_of_radars = 1;
            int num_of_ac     = 1;
            int ac_to_radar_dist = 64; //in km;
            int radar_rotation   = 5; //deg per sec;
            int radar_beamwidth  = 10; //deg
            int ac_vel = 333; //1mach in m/s
            double tick_resoln; //1s
            //trajectory x, y++
              
        }

        public void SetUp()
        {
            
            
            
            //new int[10, 8];           
            double[,] init_radar_types = {
                                          { 0.0, 0.0, 1.0, 2.0, 500.0, 800.0,  0, 0, 2.5, 0.8 },
                                          { 1.0, 0.0, 3.0, 3.1, 500.0, 800.0,  0, 0, 2.5, 0.8 },
                                          { 2.0, 0.0, 5.0, 6.0, 500.0, 800.0,  0, 0, 2.5, 0.8 }
                                        };



            radars = new List<Radar>();
            radarTypes = new List<RadarType>();

            //Aircraft Timed-Plan
            uint[] aircraft_times = { 1, 2, 3, 4 };
            uint[] aircraft_x = { 8, 8, 8, 8 };
            uint[] aircraft_y = { 0, 2, 4, 15 };

            List<TimedWayPoint> pp = new List<TimedWayPoint>();
            for (int i = 0; i < aircraft_times.Length; i++)
            {
                pp.Add(new TimedWayPoint(aircraft_x[i], aircraft_y[i], 0, aircraft_times[i]));
            }

            //Radars                           
            uint[] radar_ids = { 1, 2, 3, 4 };
            uint[] ref_radar_types = { 4, 1, 3, 1, 4, 4, 3, 3 };
            uint[] scan_rate = { 2, 1, 3, 1, 3, 1, 2, 2 };
            uint[] radar_x = { 1, 3, 12, 14, 5, 5, 6, 7 };
            uint[] radar_y = { 10, 10, 5, 5, 5, 7, 8, 7 };
            //uint[,] freqs = { { 1, 1, 1, 1 }, { 2, 2, 2, 2 }, { 3, 3, 3, 3 }, { 4, 4, 4, 4 } ,
            //  { 1, 1, 1, 1 }, { 2, 2, 2, 2 }, { 3, 3, 3, 3 }, { 4, 4, 4, 4 }
            //};
            uint[] freqs = { 3, 3, 7, 15 };
            uint[] pris =  { 500, 530, 540, 560, 500, 530, 540, 560 };
            uint[] pws =   { 50, 53, 54, 56, 500, 530, 540, 560 };

            aircraft = new Aircraft(pp);
            

            if (globals.scenario_num == 1)
            {
                int num_radars = 1;

                radar_ids[0] = 1;
                
                ref_radar_types[0] = 1;
                scan_rate[0] = 5;
                radar_x[0] = 42;
                radar_y[0] = 47;                

                for (uint i = 0; i < num_radars; i++)
                {
                    radars.Add(new Radar(radar_ids[i], ref_radar_types[i], scan_rate[i], 0,
                                             freqs[i], pris[i], pws[i], radar_x[i], radar_y[i]));
                }

                aircraft.x = 42;
                aircraft.y = 0;
                aircraft.symbol = "A";
            }

            rwr = new Rwr();

            global_view = new string[20, 20];

            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 20; j++)
                    global_view[i,j] = String.Empty;


        }

        public void printMat(int[,] mat, int rows, int cols)
        {
            for (uint i = 0; i < rows; i++)
            {
                for (uint j = 0; j < cols; j++)
                {
                    //pale blue sky
                    Console.Write(mat[i, j].ToString().PadLeft(3));
                }
                Console.WriteLine("");
            }

        }

        public void printMat(string[,] smat, int rows, int cols)
        {
            for (uint i = 0; i < rows; i++)
            {
                for (uint j = 0; j < cols; j++)
                {
                    if (smat[i, j] == String.Empty)
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("    " + "\t");
                    }
                    else if (smat[i,j].StartsWith("A"))
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(smat[i, j] + "\t");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(smat[i, j] + "\t");
                    }
                }
                Console.WriteLine("");
            }

        }

        public void PrintFreqTimeIntercept(int[,] interceptMat)
        {
            //freq vs time
            //freq moves from top to bottom
            //int[,] interceptMat = new int[20, 20];
           

            uint[,] rects = { {1, 1,  7,  11},
                              {9, 9, 15, 17},
                              {9, 1, 1, 17 } };

            for (uint i = 0; i < 20; i++)
                for (uint j = 0; j < 20; j++)
                    interceptMat[i, j] = 0;

            foreach (Radar r in radars)
            {
                interceptMat[r.posx * 2, r.posy * 2] = Convert.ToInt32(r.radarId);
            }

            for (int rectno = 0; rectno < 2; rectno++)
            {
                uint top = rects[rectno, 0];
                uint left = rects[rectno, 1];
                uint bottom = rects[rectno, 2];
                uint right = rects[rectno, 3];

                if (left % 2 == 0 || top % 2 == 0 || right % 2 == 0 || bottom % 2 == 0)
                {
                    throw (new Exception("Rect at Even coords!!"));
                }

                for (uint row = top; row <= bottom; row++)
                {
                    //Traversing Vertical edge of rect, so rows
                    if (interceptMat[row, left] > 0)
                        throw new Exception("Ouch! Boundary on Top of Radar!!!");
                    else if (interceptMat[row, left] == 0)
                        interceptMat[row, left] = -1;
                    else if (interceptMat[row, left] == -1)
                        interceptMat[row, left] = -2;
                }



                for (uint row = top; row <= bottom; row++)
                {
                    //Traversing Vertical edge of rect, so rows
                    if (interceptMat[row, right] > 0)
                        throw new Exception("Ouch! Boundary on Top of Radar!!!");
                    else if (interceptMat[row, right] == 0)
                        interceptMat[row, right] = -1;
                    else if (interceptMat[row, right] == -1)
                        interceptMat[row, right] = -2;
                }


                for (uint col = left; col <= right; col++)
                {
                    //Traversing Horizontal edge of rect, so cols
                    //Don't bother if it is 1

                    if (interceptMat[top, col] > 0)
                        throw new Exception("Ouch! Boundary on Top of Radar!!!");
                    else if (interceptMat[top, col] == 0)
                        interceptMat[top, col] = -2;
                    else if (interceptMat[top, col] == -1)
                        interceptMat[top, col] = -3;
                }

                //Console.WriteLine("After Top Print");
                //printMat(interceptMat, 20, 20);

                for (uint col = left; col <= right; col++)
                {
                    //Traversing Horizontal edge of rect, so cols
                    if (interceptMat[bottom, col] > 0)
                        throw new Exception("Ouch! Boundary on Top of Radar!!!");
                    else if (interceptMat[bottom, col] == 0)
                        interceptMat[bottom, col] = -2;
                    else if (interceptMat[bottom, col] == -1)
                        interceptMat[bottom, col] = -3;
                }

                //  Console.WriteLine("After Bottom Print");
                // printMat(interceptMat, 20, 20);
            }

            //intercept mat is formed - print it
            Console.WriteLine("        Time-Frequency Map");
            for (uint i = 0; i < 20; i++)
            {
                string pstr = String.Empty;

                for (uint j = 0; j < 20; j++)
                {
                    if (interceptMat[i, j] > 0)
                    {
                        //+ve value Radar
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        pstr = interceptMat[i, j].ToString() + " ";
                    }
                    if (interceptMat[i, j] == 0)
                    {
                        //pale blue sky
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        pstr = "  ";
                    }
                    else if (interceptMat[i, j] == -1)
                    {
                        //pale blue sky
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        pstr = "||";
                    }
                    else if (interceptMat[i, j] == -2)
                    {
                        //pale blue sky
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        pstr = "--";
                    }
                    else if (interceptMat[i, j] == -3)
                    {
                        //pale blue sky
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        pstr = "++";
                    }

                    Console.Write(pstr);
                }
                Console.WriteLine("");
            }
            Console.ResetColor();
            Console.WriteLine("X-AXIS: TIME 1 to 10 Ticks");
            Console.WriteLine("Y-AXIS: FREQ 1 to 10 GHz");

        }

        public void Go()
        {

            Console.WriteLine("go");
        }
    }


}