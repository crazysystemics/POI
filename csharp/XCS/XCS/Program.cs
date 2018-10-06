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
            
            console_win win1 = new console_win();

            Xcs_Environment xcs_env = new Xcs_Environment();

            uint start_tick = 0, elapsed_time = 0;
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

            //while(true)
            //{
            //    globals.tick++;
            //    be.UpdateRwrRxBuf(battle.radars.First(), battle.aircraft, battle.rwr);
            //    battle.radars.First().Update();
            //    //battle.aircraft.Update();
            //}
            //as long as aricraft is not hit and mission is not complete
            //continue execution


            battle.SetUp();
            BattleEngine be = new BattleEngine();


           globals.Init_Log();
           globals.Init_Display(ref win1);
 
            //win2.clear();

            List<Action> acts = new List<Action>();
            int action_index = 0;
            Action act;

            while (globals.not_over && !battle.ac_hit && !battle.MissionComplete())
            {
                globals.tick++;
                globals.dumpLog(globals.tick.ToString(), globals.onconsole, globals.onfile);

                if (globals.scenario_num == 1)
                {
                    battle.aircraft.Update();
                    battle.radars.First().Update();
                    battle.rwr.Update();
                    globals.displayScenario1(ref win1, ref globals.win2, battle);
                }

                List<string> sl = new List<string>();
                globals.dumpLog("MAIN-LOOP-BEGIN:", globals.onconsole, globals.onfile);



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

                        //sl = new List<string>();
                        //sl.Add("INITIATING-RECEPTION[S0->S1]");
                        //acts[action_index].fieldvalsToString(ref sl);
                        //globals.dumpLog(sl, globals.onconsole, globals.onfile);
                    }
                    if (battle.rwr.rxunit.state == (uint)State.STATE1)
                    {
                        elapsed_time = (globals.tick - start_tick);
                        if (elapsed_time < acts[action_index].duration)
                        {
                            //globals.dumpLog("ACTION-LIST@WHILE-RCPTN-ON:actlist[" + acts.Count.ToString() + "] ActionIndex[" + action_index.ToString() + "] elapsedTime[" + elapsed_time.ToString() + "]",
                            //                 globals.onconsole, globals.onfile);


                            //add radar-record to rxbuf
                            //be.dumpLog("state:" + battle.rwr.rxunit.state.ToString() + " ActIndex:" + action_index.ToString());
                            be.UpdateRwrRxBuf(battle.radars.First(), battle.aircraft, battle.rwr);
                        }
                        else
                        {
                            battle.rwr.rxunit.stopRx(acts[action_index].band);

                            for(int i=0; i<battle.rwr.rxunit.rxBufCount; i++)
                            {
                                acts[action_index].symbol += battle.rwr.rxunit.rxbuf[i].symbol + " ";
                            }

                            //move to next band
                            action_index++;

                            //globals.dumpLog("ACTION-LIST@1-F:moved to action" + action_index.ToString(),
                            //        globals.onconsole, globals.onfile);

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
                    //if (battle.rwr.rxunit.rxBufCount > 0)
                    globals.dumpLog("ALL-ACTIONS-DONE/XCS-PROC-BEGIN:", globals.onconsole, globals.onfile);

                    //Log Messages
                    globals.dumpLog("ACTIONS:", globals.onconsole, globals.onfile);
                    foreach (Action succ_act in acts)
                    {
                        globals.dumpLog(succ_act.band.ToString() + "\t" + succ_act.duration.ToString(), globals.onconsole, globals.onfile);
                    }
                    if (battle.rwr.rxunit.rxBufCount > 0)
                    {
                        globals.dumpLog("RADARS_RCVD:", !globals.onconsole, !globals.onfile);
                        globals.dumpLog(Radar.fieldNamesToString(), globals.onconsole, globals.onfile);
                        sl = new List<string>();
                        for (int i = 0; i < battle.rwr.rxunit.rxBufCount; ++i)
                        {
                            battle.rwr.rxunit.rxbuf[i].fieldvalsToString(ref sl);
                        }
                        globals.dumpLog(sl, globals.onconsole, globals.onfile);
                    }
                    else
                    {
                        globals.dumpLog("NO RADARS RCVD ", globals.onconsole, globals.onfile);
                    }





                    xcs_env.sigma_radars = new List<Radar>();
                    bool matching_found = false;
                    List<List<Intercept>> new_icept_trks = new List<List<Intercept>>();

                    //Log Messages

                    sl = new List<string>();
                    List<string> sl_icept = new List<string>();


                    for (int i = 0; i < battle.rwr.rxunit.rxBufCount; ++i)
                    {
                        Radar r = battle.rwr.rxunit.rxbuf[i];
                        xcs_env.sigma_radars.Add(r);

                        uint priority = 0;
                        Intercept new_icept = new Intercept(0, r.radarId, r.freq, r.pri, r.mb_azim, priority);
                        //Log Messages
                        {

                            sl_icept = new List<string>();
                            sl_icept.Add("INTERCEPTS:");


                            sl_icept.Add(new_icept.fieldNamesToString());
                            new_icept.fieldvalsToString(ref sl_icept);
                            globals.dumpLog(sl_icept, globals.onconsole, globals.onfile);
                        }

                        foreach (List<Intercept> icepts in battle.rwr.InterceptTracks)
                        {
                            //move to next track (icepts)
                            Intercept icept = icepts.Last();

                            //Log Messages
                            {
                                //get log for latest attributes in (icepts) track
                                icept.fieldvalsToString(ref sl_icept);
                                globals.dumpLog(sl_icept.Last(), globals.onconsole, globals.onfile);
                            }

                            if (!matching_found && new_icept.isSameAs(icept))
                            {
                                new_icept.trackid = icept.trackid;
                                icepts.Add(new_icept);
                                matching_found = true;
                                new_icept_trks.Add(icepts);
                            }


                        }

                        if (!matching_found)
                        {
                            //new track
                            List<Intercept> new_icept_trk = new List<Intercept>();

                            int new_track_id = battle.rwr.InterceptTracks.Count + 1;
                            new_icept.trackid = (uint)new_track_id;
                            new_icept_trk.Add(new_icept);
                            new_icept_trks.Add(new_icept_trk);

                            //Log Messages
                            {
                                sl = new List<string>();
                                new_icept.fieldvalsToString(ref sl);

                                globals.dumpLog(sl, globals.onconsole, globals.onfile);
                                globals.dumpLog("NEW-TRACK-THIS(ABOVE)-INTERCEPT", globals.onconsole, globals.onfile);
                            }

                        }
                    }
                    battle.rwr.InterceptTracks = new_icept_trks;

                    //determine actions for next cycles through xcs
                    //reset rxbufcount and alpha_actions
                    battle.rwr.rxunit.rxBufCount = 0;
                    xcs_env.alpha_actions = new List<Action>();

                    xcs_env.runNextCycle();

                    //display routinne
                    uint st_x = 5, st_y, ix = 0, xm = 3, ym = 5, x_w, y_h, index;
                    string[] sym1 = { "R1", "R2", "R3", "R4" };
                    string[] sym2 = { "R5", "R6", "R7", "R8" };
                    string[] sym3 = { "R9", "R10", "R11", "R12" };
                    string[] sym4 = { "R13", "R14", "R15", "R16" };
                    
                    win1.clear();
                    win1.puttext(1, ym * 0 + ym / 2, "[A]");
                    win1.puttext(1, ym * 1 + ym / 2 , "[B]");
                    win1.puttext(1, ym * 2 + ym / 2 , "[C]");
                    win1.puttext(1, ym * 3 + ym / 2 , "[D]");
                    win1.puttext(20, 20, "Search Plan (Time Vs Frequency", ConsoleColor.White, ConsoleColor.Black);
                    Random random = new Random();
                    string str_sym;
                    foreach (Action a in xcs_env.alpha_actions)
                    {
                        st_y = (a.band - 1) * ym;
                        x_w = a.duration * xm;
                        y_h = ym;
                        win1.draw_box(st_x, st_y, x_w, y_h);
                        index = (uint)random.Next(4);
                        switch (a.band)
                        {
                            case 1: str_sym = sym1[(int)index]; break;
                            case 2: str_sym = sym2[(int)index]; break;
                            case 3: str_sym = sym3[(int)index]; break;
                            case 4: str_sym = sym4[(int)index]; break;
                            default:
                                str_sym = "Ru";
                                break;
                        }

                        win1.puttext(st_x + x_w/2, st_y + y_h/2, str_sym, ConsoleColor.White, ConsoleColor.Red);
                        win1.draw();
                        st_x = st_x + x_w;
                    }

                    sl = new List<string>();
                    xcs_env.fieldvalsToString(ref sl);
                    globals.dumpLog(sl, globals.onconsole, globals.onfile);

                    acts = xcs_env.alpha_actions;
                    action_index = 0;

                    //Logging Messages
                    if (acts.Count == 0)
                    {
                        globals.dumpLog(globals.tick.ToString() + "ACTION-LIST@EMPTY-ACTS-TO-EXEC:",
                                                                  globals.onconsole, globals.onfile);
                    }
                    else
                    {
                        globals.dumpLog("ACTION-LIST-HAS-ACTIONS: " + acts.Count,
                                        globals.onconsole, globals.onfile);

                        foreach (Action act1 in acts)
                        {
                            act1.fieldvalsToString(ref sl);
                            globals.dumpLog(sl.Last(), globals.onconsole, globals.onfile);
                        }
                    }

                    //Logging Messages
                    {
                        sl = new List<string>();
                        xcs_env.fieldvalsToString(ref sl);
                        if (sl.Count > 0)
                        {
                            globals.dumpLog(sl, true, globals.onfile);
                        }
                    }
                }

            }//Main

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