using RWR_POC_GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

class TrackGenerator
{
    //PFM pfm = new PFM();

    //RWR rwr = new RWR();

    List<EmitterTrackRecord> emitterTrackFile = new List<EmitterTrackRecord>();
    List<EmitterRecord> emitterRecords_Table = new List<EmitterRecord>();
    List<EmitterRecord> emitterRecords_unstable = new List<EmitterRecord>();
    List<EmitterRecord> emitterRecords_stable = new List<EmitterRecord>();
    List<int> tracks = new List<int>();
    int tr = 0;

    public TrackGenerator()
    {
      
        //pfm.FormPFMTable();
        for (int i = 0; i < 100; i++)
            Globals.trackParameters.tracks.Add(i);
    }

    //public void RunSimulation()

    //{
    //    Console.WriteLine("\t\t EmitterRecord");

    //    Console.WriteLine();

    //    Console.WriteLine("Tick No.\tTrack ID\tAGEIN\tAGEOUT\t\tVisibility\t\tACTION\t\tSTATE\t\tAgingOUT count");

    //    Console.WriteLine();



    //    emitterTrackFile.Clear();

    //    while (Globals.Tick < 10)

    //    {
    //        foreach (EmitterTrackRecord etr in emitterTrackFile)
    //            etr.received = false;

    //        emitterRecords_Table = rwr.getEmitterRecord();

    //        foreach (EmitterRecord er in emitterRecords_Table)
    //        {
    //            EmitterID pfmeid = new EmitterID(0, "eid", 0, 0, 0, 0, 0, 0, 0, 0, 0, Globals.ageIn, Globals.ageOut);

    //            rwr.ManageTracks(er, pfmeid, emitterTrackFile);
    //        }

    //        //All emitter records are processed

    //        //ON DELETE


    //        List<EmitterTrackRecord> tempEFT = new List<EmitterTrackRecord>();

    //        foreach (EmitterTrackRecord etr in emitterTrackFile)

    //        {


    //            if (etr.received)

    //                tempEFT.Add(etr);


    //            else

    //            {

    //                // On Delete
    //                if (etr.ageIn > 0)

    //                {

    //                    // not copying to tempETF is deleting
    //                    etr.valid = false;

    //                    if (Globals.Tick >= Globals.trackParameters.start_tick && Globals.Tick <= Globals.trackParameters.end_tick
    //                    && (Globals.trackParameters.action == ACTION.IDELETE || Globals.trackParameters.action == ACTION.ALL))
    //                    {

    //                        if (Globals.trackParameters.tracks.Contains(etr.trackID))

    //                        {

    //                            Console.WriteLine($"{Globals.Tick}\t\t{etr.trackID}\t\t{etr.ageIn}\t{etr.ageOut}\t\tNOT RCV \t\tiDELETE");

    //                        }

    //                    }

    //                    // Globals.recorded Tracks.Add(new RecordedData(Globals. Tick, etr.trackID, etr.ageln, etr.ageOut, etr.vis, etr.action, string state, etr.ageinoutcount));
    //                }

    //                else

    //                {

    //                    etr.ageOut--;

    //                    etr.AgingOutCount++;
    //                    if (etr.ageOut > 0)

    //                    {

    //                        etr.valid = true;

    //                        tempEFT.Add(etr);

    //                        Console.WriteLine($"{Globals.Tick}\t\t{etr.trackID}\t\t{etr.ageIn}\t{etr.ageOut}\t\tNOT RCV\t\tAGING OUT");

    //                    }

    //                    else

    //                    {

    //                        // not copying to tempETF is deleting

    //                        if (Globals.Tick >=
    //                        Globals.trackParameters.start_tick && Globals.Tick <=

    //                        Globals.trackParameters.end_tick
    //                        && (Globals.trackParameters.action ==

    //                        ACTION.ODELETE || Globals.trackParameters.action ==

    //                        ACTION.ALL))

    //                        {
    //                            if (Globals.trackParameters.tracks.Contains(etr.trackID))

    //                            {

    //                                Console.WriteLine($"{Globals.Tick}\t\t{etr.trackID} \t\t{etr.ageIn}\t{etr.ageOut}\t\tNOT RCV \t\toDELETE\t\t\t\t{etr.AgingOutCount}");

    //                            }

    //                        }

    //                        // Globals.recorded Tracks.Add(new RecordedData(Globals. Tick, etr.trackID));

    //                    }

    //                }

    //            }

    //        }


    //        emitterTrackFile = tempEFT;

    //        Globals.Tick++;


    //    }

    //}


    public void ProcessCommand(string command, string[] args)
    {
        if (command == "list")
        {
            commandList(args);
        }

        if (command == "set")
        {
            commandSet(args);
        }

        if (command == "ls")
        {
            list_of_commands();
        }
    }

    private void commandList(string[] args)
    {
        if (args[0].ToLower() == "ticks")
        {
            GetTicks(args);
        }

        if (args[0].ToLower() == "tracks")
        {
            GetTracks(args);
        }

        if (args[0].ToLower() == "action")
        {
            GetActions(args);
        }
    }

    private void commandSet(string[] args)
    {
        if (args[0].ToLower() == "prob")
        {
            SetProb(args);
        }

        if (args[0].ToLower() == "agein")
        {
            SetAgeIn(args);
        }

        if (args[0].ToLower() == "ageout")
        {
            SetAgeOut(args);
        }
    }

    private void list_of_commands()
    {
        Console.WriteLine("\nList of Commands\n"); Console.WriteLine("command 1: List ticks <start_tick> <end_tick = start_tick>");
        Console.WriteLine("command 2: List track <track 1> <track 2> <track 3>...");
        Console.WriteLine("command 3: List action <I/U/ iD/oD>");
        Console.WriteLine("command 4: List [ticks/tracks /action] *");
        Console.WriteLine("command 5: List [ticks / tracks / action] <filter> | List [ticks / tracks / action] <filter>");
        Console.WriteLine("command 6: List [ticks/tracks /action] | Set prob | Set age_in age_out \n");
    }

    public void GetTicks(string[] args)
    {
        if (args[1] != "*")
        {
            Globals.trackParameters.start_tick = Convert.ToInt32(args[1]);
            int end_tick;
            if (args.Length == 2)
            {
                Globals.trackParameters.end_tick = Globals.trackParameters.start_tick;
            }

            else
            {
                Globals.trackParameters.end_tick = Convert.ToInt32(args[2]);
            }
        }
        else
        {
            Globals.trackParameters.end_tick = Globals.Tick;
        }
    }

    public void GetTracks(string[] args)
    {
        if (args[1] != "*")
        {
            Globals.trackParameters.tracks.Clear(); for (int i = 1; i < args.Length; i++)
            {
                Globals.trackParameters.tracks.Add(Convert.ToInt32(args[1]));
            }
        }
    }

    public void GetActions(string[] args)
    {
        if (args[1] != "*")
        {
            if (args[1].ToLower() == "i")
            {
                Globals.trackParameters.action =
                TrackState.ETF_INSERTED;
            }

            else if (args[1].ToLower() == "u")
            {
                Globals.trackParameters.action =
                TrackState.ETF_UPDATED;
            }

            else if (args[1].ToLower() == "id")
            {
                Globals.trackParameters.action = TrackState.ETF_DELETED;
            }

            else
            {
                Globals.trackParameters.action =
                TrackState.ETF_DELETED;
            }
        }
    }

    public void GetLengthOfTrack(string[] args)
    {
        foreach (EmitterTrackRecord etr in emitterTrackFile)
        {
            int count = 0;
            if (etr.trackID == int.Parse(args[1])) count++;
            Console.WriteLine($" {count} ");
        }
    }

    public void SetProb(string[] args)
    {
        Globals.matchProbability = double.Parse(args[1]);
    }

    public void SetAgeIn(string[] args)
    {
        Globals.ageIn = int.Parse(args[1]);
    }

    public void SetAgeOut(string[] args)
    {
        Globals.ageOut = int.Parse(args[1]);
    }

   
}




