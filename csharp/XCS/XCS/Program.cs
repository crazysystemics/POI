using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POI_XCS
{
    class Program
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Battle battle = new Battle();
            uint tick = 0;
            double epsilon = 0.0001;
            
            Xcs_Environment xcs_env =new Xcs_Environment();

            uint start_tick = 0;
            battle = new Battle();

            //for measuring console width and length-helper code
            //for (uint repeat = 0; repeat < 30; repeat++)
            //{
            //    string str = String.Empty;
            //    for (uint i = 0; i < 10; i++)
            //    {
            //       str += (repeat % 10).ToString();
            //    }
            //    Console.Write(str);
            //}
            
            battle.SetUp();
            //as long as aricraft is not hit and mission is not complete
            //continue execution
            List<Action> acts = new List<Action>();
            int action_index = 0;
            Action act;

            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                    battle.global_view[i, j] = String.Empty;
            Console.Clear();
            while (!battle.ac_hit && !battle.MissionComplete())
            {
                tick++;
                if (action_index < acts.ToArray().Length)
                {
                    //reception-phase, let us say acts are sorted in time and non-overlapped
                    if (battle.rwr.rxunit.rxstatus == 0)
                    {
                        start_tick = tick;
                        battle.rwr.rxunit.startRx(acts[action_index].band);
                    }
                    else if (tick < start_tick + battle.rwr.rxunit.duration)
                    {
                        battle.rwr.rxunit.stopRx(acts[action_index].band);
                        //move to next band
                        action_index++;
                    }
                }
                else
                {
                    //process data and run xcs algorithm - since this is algorithmic component
                    //it is supposed to execute in a single tick. otherwise, ticks can be incremented
                    //here only without disturbing global flow.
                    //process all the radars received in current reception

                    //view routines also may be invoked here

                    //xcs component
                    xcs_env.sigma_radars = battle.radars;
                    xcs_env.alpha_actions = new List<Action>();
                    xcs_env.runNextCycle();
                    acts = xcs_env.alpha_actions;                    
                }
            }
  
        }

    }
}

           //battle.global_view[14 - battle.aircraft.y, battle.aircraft.x]= "A"+battle.aircraft.x.ToString() + ":" +
           //                                                                  battle.aircraft.y.ToString() + ":" +
           //                                                                  battle.aircraft.azim;

           //     Console.WriteLine("Global View at Tick: " + tick.ToString());
           //     battle.printMat(battle.global_view, 15, 15);

           //     Console.WriteLine("Intercept Matrix:");
           //     //battle.printMat(battle.rwr.InterceptMat, 20, 20);
           //     //battle.PrintFreqTimeIntercept(battle.rwr.InterceptMat);


           //     //Update cycle
           //     tick++;

           //     foreach (Radar r in battle.radars)
           //         r.mb_azim = (r.mb_azim + 1) % 360;

           //     // battle.aircraft.x++;
           //     battle.aircraft.y++;
           //     battle.rwr.band = (battle.rwr.band + 1) % 4;

           //     //assuming all flight happens in first-quadrant
           //     if (Math.Abs((double) battle.aircraft.x) < epsilon)
           //         battle.aircraft.azim = 90;
           //     else
           //         battle.aircraft.azim = Convert.ToUInt32(Math.Atan((double) battle.aircraft.y / (double) battle.aircraft.x));  

    
                //    battle.global_view[14 - r.posy, r.posx] = r.radarId.ToString() + ":" +
                //                                            r.mb_azim.ToString() + ":";                                                        
  
                //    //assuming spot beam (0-deg tolerance)
                //    if (battle.rwr.azim == r.mb_azim && battle.rwr.band == r.freq[0] / 4)
                //    {
                //        //Intercept icpt = new Intercept(r.radarId, tick, battle.rwr.band, 0, 0);
                //        battle.rwr.InterceptMat[20 - r.freq[0], tick] = (int) r.radarId;                       
                //    }
                //}

          //private static void NewMethod(Battle battle)
        //{
        //    battle.rwr.rxunit.startRx();
        //}