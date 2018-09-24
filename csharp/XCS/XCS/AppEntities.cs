using System;
using System.Collections.Generic;
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
        public static uint tick;
    }

  
       
    


    //--------------------------------RWR related Tasks -------------------------------------------

    class Intercept
    {
        public uint radarid;
        public uint tick;
        public uint band;
        public uint priority;
        public uint rect;

        public void Print()
        {
            Console.Write(band.ToString() + priority.ToString() + rect.ToString());
        }

        public Intercept(uint rid, uint t, uint b, uint p, uint r)
        {
            radarid = rid;
            tick = t;
            band = b;
            priority = p;
            rect = r;
        }
    }

    class interceptTrack
    {
        public int srcid;
        Intercept[] track;
    }

      
    class Rwr
    {
        //This list is unknown to RWR. Included here
        //Just because it is concerned  with this Rwr
        //Global data  Unknown to Rwr
        public uint band;
        public List<Intercept> curIlluminations;        
        public int[,] InterceptMat;
        public Dictionary<int, Intercept[]> InterceptTracks;
        public RxUnit rxunit = new RxUnit();


        //set by battle engine
        public uint azim;

        public List<Intercept> curIntercepts;
        
        public Rwr()
        {
            InterceptMat = new int[20, 20];
            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 20; j++)
                    InterceptMat[i, j] = 0;

            curIlluminations = new List<Intercept>();
            InterceptTracks = new Dictionary<int, Intercept[]>();
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

        public Aircraft(List<TimedWayPoint> p)
        {
            plannedPath = p;
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
        public uint scanrate;
        public uint scan_interval;
        public uint[] freq;
        public uint pri;
        public uint pw;
        public uint posx;
        public uint posy;
        public uint mb_azim; //main beam azimuth

        public Radar(uint pradarId, uint prtype, uint pscanrate, uint pscan_interval, uint[] pfreq, uint ppri, uint ppw, uint x, uint y)
        {
            radarId       = pradarId;
            rtype         = prtype;
            scanrate      = pscanrate;
            scan_interval = pscan_interval;
            freq          = pfreq;
            pri           = ppri;
            pw = ppw;
            posx = x;
            posy = y;
            
        }
    }

    class RxUnit
    {
        public uint rxFreqMin;
        public uint rxFreqMax;

        public void startRx(uint band)
        {
        }

        public void stopRx(uint band)
        {
        }
        public int duration;
        public bool phi;//no input;
        public Radar[] rxbuf = new Radar[32];
        public int rxstatus;
    }

    class Battle
    {
        public List<RadarType> radarTypes;
        public List<Radar> radars;
        public Aircraft aircraft;
        public Rwr rwr;
        public bool ac_hit;
        public string[,] global_view = new string[20, 20]; 

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

        public void SetUp()
        {
            //new int[10, 8];           
            double[,] init_radar_types = {
                                          { 0.0, 0.0, 1.0, 2.0, 500.0, 800.0,  0, 0, 2.5, 0.8 },
                                          { 1.0, 0.0, 3.0, 4.0, 500.0, 800.0,  0, 0, 2.5, 0.8 },
                                          { 2.0, 0.0, 5.0, 6.0, 500.0, 800.0,  0, 0, 2.5, 0.8 }
                                        };



            radars = new List<Radar>();

            radarTypes = new List<RadarType>();

            for (uint i = 0; i < 3; i++)
            {
                double[] rtypebuf = new double[10];
                for (int j = 0; j < 0; j++)
                {
                    Console.WriteLine(i.ToString() + ":" + j.ToString());
                    rtypebuf[j] = init_radar_types[i, j];

                }
                radarTypes.Add(new RadarType(rtypebuf));
            }

            //Radars                           
            uint[] radar_ids = { 1, 2, 3, 4 };
            uint[] ref_radar_types = { 4, 1, 3, 1, 4, 4, 3, 3 };
            uint[] scan_rate = { 2, 1, 3, 1, 3, 1, 2, 2 };
            uint[] radar_x = { 1, 3, 12, 14, 5, 5, 6, 7 };
            uint[] radar_y = { 10, 10, 5, 5, 5, 7, 8, 7 };
            uint[,] freqs = { { 1, 1, 1, 1 }, { 2, 2, 2, 2 }, { 3, 3, 3, 3 }, { 4, 4, 4, 4 } ,
                                        { 1, 1, 1, 1 }, { 2, 2, 2, 2 }, { 3, 3, 3, 3 }, { 4, 4, 4, 4 }
                                      };
            uint[] pris = { 500, 530, 540, 560, 500, 530, 540, 560 };
            uint[] pws = { 50, 53, 54, 56, 500, 530, 540, 560 };


            for (uint i = 0; i < radar_ids.Length; i++)
            {
                uint[] freqbuf = new uint[10];
                for (int j = 0; j < 4; j++)
                {
                    freqbuf[j] = freqs[i, j];
                }

                radars.Add(new Radar(radar_ids[i], ref_radar_types[i], scan_rate[i], 0,
                                         freqbuf, pris[i], pws[i], radar_x[i], radar_y[i]));
            }

            //Aircraft Timed-Plan
            uint[] aircraft_times = { 1, 2, 3, 4 };
            uint[] aircraft_x     = { 8, 8, 8, 8 };
            uint[] aircraft_y     = { 0, 2, 4, 15 };

            List<TimedWayPoint> pp = new List<TimedWayPoint>();
            for (int i = 0; i < aircraft_times.Length; i++)
            {
                pp.Add(new TimedWayPoint(aircraft_x[i], aircraft_y[i], 0, aircraft_times[i]));
            }

            aircraft = new Aircraft(pp);
            aircraft.x = aircraft_x[0];
            aircraft.y = aircraft_y[0];

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