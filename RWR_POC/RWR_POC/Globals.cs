﻿static class Globals
{
    public static int Tick = 0;
    public static float TimeResolution = 0.10f;
    public static int pulseTravelSpeed = 1;
    public static bool debugPrint = true;
    public static bool distDebugPrint = true;
    public static bool aircraftDebugPrint = true;

    public static void debugWriteLine(string s)
    {
        Console.WriteLine(s);
    }
}