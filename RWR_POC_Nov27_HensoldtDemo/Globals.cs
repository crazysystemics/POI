using RWR_POC_GUI;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Threading;

public static class Globals
{
    // Episode configuration
    public static Random randomNumberGenerator = new Random(10);
    public static int flightPathNumber = -1;
    public static int radarSetNumber = -1;

    public static int Tick = 0;
    public static float TimeResolution = 0.10f;
    public static int pulseTravelSpeed = 1;
    public static int guID = 0;
    public static int gTrackID = 0;
    public static DebugLevel debugPrint;
    public static string recFileName;
    public static string trackRecFileName;
    public static DispatcherTimer timer;
    public static MainWindow mainWindow;
    public static QLearner qLearner = new QLearner();
    public static TrackParameters trackParameters = new TrackParameters();
    public static double matchProbability = 0.5;
    public static int ageIn;
    public static int ageOut;

    public static int action;

    public static bool isDone = false;
    public static CommandExecutive commandExecutive = new CommandExecutive();

    public static List<RecordedData> recordedList = new List<RecordedData>();
    public static List<EmitterTrackRecord> emitterTrackFile = new List<EmitterTrackRecord>();

    public static void DebugWriteLine(string s)
    {
        Console.WriteLine(s);
    }

    public static void ExecuteShell()
    {
        Globals.timer.Stop();
        TrackGenerator trackGenerator = new TrackGenerator();

        string command = string.Empty;
        while (command != "quit")
        {
            Console.WriteLine("Enter the Command");
             command = Console.ReadLine();
            Globals.commandExecutive.ParsePipelineCommand(command);
            Globals.commandExecutive.DisplayRecords();
        }
        Globals.timer.Start();
    }

    public enum RadarTypes
    {
        EARLYWARNING,
        ACQUISITION,
        FIRECONTROL,
        GCI,
        SIMPLE
    }

    public enum DebugLevel
    {
        NONE,
        BRIEF,
        SPOT,
        VERBOSE
    }
}