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
        /// 


        static void Main(string[] args)
        {
            Battle battle = new Battle();
            double epsilon = 0.0001;
            uint max_width = 174, max_height = 48;
            console_win win1 = new console_win();

            Xcs_Environment xcs_env = new Xcs_Environment();

            uint start_tick = 0;
            battle = new Battle();



            // DISPLAY CONFIGURATION BEGIN
            /* instantiate the window views */

            //console_win win2 = new console_win(w + 2, 1, w, h, ConsoleColor.Cyan, ConsoleColor.Black);
            //console_win win3 = new console_win(1, h + 2, w, h, ConsoleColor.Yellow, ConsoleColor.Black);
            //console_win win4 = new console_win(w + 2, h + 2, w, h, ConsoleColor.Gray, ConsoleColor.Black);

            uint x = 1, y = 1;
            bool xdir = true, ydir = true;

            //win1.clear();
            //win2.clear();
            //win3.clear();
            //win4.clear();

            //win1.puttext(x, y, "POI-XCS", ConsoleColor.Yellow, ConsoleColor.Red);
            //win1.draw();         

            //win1.show_view1();

            //win2.show_view2();
            //win3.show_view3();
            //win4.show_view4();
            //DISPLAY CONFIGURATION END
            globals.scenario_num = 1;
            battle.SetUp();
            BattleEngine be = new BattleEngine();
            //while(true)
            //{
            //    globals.tick++;
            //    be.UpdateRwrRxBuf(battle.radars.First(), battle.aircraft, battle.rwr);
            //    battle.radars.First().Update();
            //    //battle.aircraft.Update();
            //}
            //as long as aricraft is not hit and mission is not complete
            //continue execution
            List<Action> acts = new List<Action>();
            int action_index = 0;
            Action act;

            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                    battle.global_view[i, j] = String.Empty;


            displayInit(ref win1);


            while (!battle.ac_hit && !battle.MissionComplete())
            {
                globals.tick++;

                if (globals.scenario_num == 1)
                {
                    battle.aircraft.Update();
                    battle.radars.First().Update();
                    battle.rwr.Update();
                    displayScenario1(ref win1, battle);
                }

                //receive in all bands of particular regime
                if (action_index < acts.ToArray().Length)
                {
                    //reception-phase, let us say acts are sorted in time and non-overlapped

                    if (battle.rwr.rxunit.state == (uint)State.STATE0)
                    {
                        //idle
                        start_tick = globals.tick;
                        battle.rwr.rxunit.startRx(acts[action_index].band);
                        battle.rwr.duration = acts[action_index].duration;
                        battle.rwr.band = acts[action_index].band;
                        battle.rwr.rxunit.state = (uint)State.STATE1;
                    }
                    if (battle.rwr.rxunit.state == (uint)State.STATE1)
                    {
                        if ((globals.tick - start_tick) < acts[action_index].duration)
                        {
                            //add radar-record to rxbuf
                            be.UpdateRwrRxBuf(battle.radars.First(), battle.aircraft, battle.rwr);
                        }
                        else
                        {

                            battle.rwr.rxunit.stopRx(acts[action_index].band);
                            //move to next band
                            action_index++;
                            battle.rwr.rxunit.state = (uint)State.STATE0;
                        }
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
                    if (battle.rwr.rxunit.rxBufCount > 0)
                    {

                        for(int i = 0; i < battle.rwr.rxunit.rxBufCount; ++i)
                        {
                            xcs_env.sigma_radars.Add(battle.rwr.rxunit.rxbuf[i]);
                        }
                        battle.rwr.rxunit.rxBufCount = 0;

                        xcs_env.alpha_actions = new List<Action>();
                        xcs_env.runNextCycle();
                        acts = xcs_env.alpha_actions;
                    }
                    else
                    {
                        acts = xcs_env.xcs.default_actions;                        
                    }

                    action_index = 0;
                }
            }

        } //Main

        static void displayInit(ref console_win win)
        {
            uint w = 174 / 2; // (uint)Console.WindowWidth / 2;
            uint h = 48; // (uint)Console.WindowHeight;
            win = new console_win(1, 1, w, h, ConsoleColor.Green, ConsoleColor.Black);
        }

        static void displayScenario1(ref console_win win, Battle b)
        {
            //win.clear();
            win.puttext(b.radars.First().posx, b.radars.First().posy, b.radars.First().symbol, ConsoleColor.White, ConsoleColor.Red);
            win.puttext(b.aircraft.x, b.aircraft.y, b.aircraft.symbol, ConsoleColor.White, ConsoleColor.Blue);
            //win.draw();
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