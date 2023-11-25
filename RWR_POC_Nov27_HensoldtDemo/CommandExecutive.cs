using RWR_POC_GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class CommandExecutive
    {
        //public string command;
        public string[] commandTokens;

        public void ParseCommand(String command)
        {
            string[] tokens = command.Split(' ');
            string trimmedCommand = String.Empty;
            List<string> trimmedArg = new List<string>();
            int index = 0;
            foreach (string token in tokens)
            {
                if (index == 0)
                {
                    trimmedCommand = token.ToLower();
                    index++;
                }
                else
                {
                    trimmedArg.Add(token.TrimEnd());
                    index++;
                }
            }
            ExecuteCommand(trimmedCommand, trimmedArg);
        }

        public void ExecuteCommand(string command, List<string> args)
        {
            TrackGenerator tg = new TrackGenerator();
            tg.ProcessCommand(command, args.ToArray());
        }

        public void ParsePipelineCommand(string pipelineCommand)
        {
            string[] arg = pipelineCommand.Split('|');
            foreach (string command in arg)
                ParseCommand(command);
        }

    public void DisplayRecords()
    {
        Console.WriteLine("\t\t EmitterRecord");
        Console.WriteLine();
        Console.WriteLine("Tick No.\tTrack ID\tAGEIN\tAGEOUT\t\tVisibility\t\tACTION\t\tSTATE\t\tAgingOUT count");
        Console.WriteLine();
        //Globals.Tick = 0;
        foreach (RecordedData rd in Globals.recordedList)
        {
            if (rd.tick >= Globals.trackParameters.start_tick && rd.tick <=
                    Globals.trackParameters.end_tick)
            {
                if (Globals.trackParameters.tracks.Contains(rd.trackID))
                {
                    Console.WriteLine($"{rd.tick}\t\t{rd.trackID}\t\t{rd.ageIn}\t{rd.ageOut}\t\t{rd.visibility}\t\t{rd.action}\t\t{rd.state}\t\t{rd.agingOutCount}");
                }
            }
        }
    }
}

