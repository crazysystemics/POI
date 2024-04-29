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

    public void ProcessCommand(string command, string[] args)
    {
        if (command == "list")
        {
            commandList(args);
        }

        

        if (command == "quit")
        { 
            //Environment.Exit(0);
        }

        if(command == "show")
        {
            commandShow(args);
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

        if (args[0].ToLower() == "tracks-history")
        {
            GetTracksHistory(args);
        }

        if (args[0].ToLower() == "action")
        {
            GetActions(args);
        }

        if (args[0].ToLower() == "commands")
        {
            list_of_commands();
        }

        //if (args[0].ToLower() == "emitterRecords")
        //{
        //    list_of_emitterRecords();
        //}
    }

    //private void commandSet(string[] args)
    //{
    //    if (args[0].ToLower() == "prob")
    //    {
    //        SetProb(args);
    //    }

    //    if (args[0].ToLower() == "agein")
    //    {
    //        SetAgeIn(args);
    //    }

    //    if (args[0].ToLower() == "ageout")
    //    {
    //        SetAgeOut(args);
    //    }
    //}

    private void list_of_commands()
    {
        Console.WriteLine("\nList of Commands\n");
        Console.WriteLine("command 1: list commands");
        Console.WriteLine("command 2: List ticks <start_tick> <end_tick = start_tick>");
        Console.WriteLine("command 3: List tracks-history <track 1> <track 2> <track 3>...");
        Console.WriteLine("command 4: List action <I/U/ iD/oD>");
        Console.WriteLine("command 5: List [ticks/tracks /action] *");
        Console.WriteLine("command 6: List [ticks / tracks / action] <filter> | List [ticks / tracks / action] <filter>");
        Console.WriteLine("command 7: Show QsaTable ");
        Console.WriteLine("command 8: quit");
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
            Globals.trackParameters.end_tick = Globals.tick;
        }
        Globals.commandExecutive.DisplayRecords();
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

        Globals.commandExecutive.DisplayTracks();
    }

    public void GetTracksHistory(string[] args)
    {
        if (args[1] != "*")
        {
            Globals.trackParameters.tracks.Clear(); for (int i = 1; i < args.Length; i++)
            {
                Globals.trackParameters.tracks.Add(Convert.ToInt32(args[1]));
            }
        }

        Globals.commandExecutive.DisplayRecords();
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
                Globals.trackParameters.action = TrackState.ETF_iDELETE;
            }

            else if (args[1].ToLower() == "od")
            {
                Globals.trackParameters.action =
                TrackState.ETF_oDELETE;
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

   

    public void getERec(string[] args)
    {
        if (args[1] != "*")
        {
            Globals.trackParameters.tracks.Clear(); for (int i = 1; i < args.Length; i++)
            {
                Globals.trackParameters.erecs.Add(Convert.ToInt32(args[1]));
            }
        }

        Globals.commandExecutive.DisplayErecs();
    
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

   public void commandShow(string[] args)
    {
        //if (args[0].ToLower() == "qsatable")
        //{
        //    Console.WriteLine("QsaTable");
        //    Console.WriteLine("State\t\tAction-0\tAction-1\tAction-2");
        //    foreach (List<double> actionList in Globals.qLearner.Qsa)
        //    {
        //        Console.WriteLine($"{Globals.qLearner.Qsa.IndexOf(actionList)}\t\t{actionList[0].ToString("N3")}\t\t{actionList[1].ToString("N3")}\t\t{actionList[2].ToString("N3")}");
        //    }
        //}
    }
}