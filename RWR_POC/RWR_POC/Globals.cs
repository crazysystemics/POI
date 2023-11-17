public static class Globals
{
    public static int Tick = 0;
    public static float TimeResolution = 0.10f;
    public static int pulseTravelSpeed = 1;
    public static int guID = 0;
    public static int gTrackID = 0;
    public static bool debugPrint = true;
    public static bool distDebugPrint = true;
    public static bool aircraftDebugPrint = true;
    public static string recFileName;
    public static Random randomNumberGenerator = new Random();

    public static void DebugWriteLine(string s)
    {
        Console.WriteLine(s);
    }

    public enum RadarTypes
    {
        EarlyWarning,
        Acquisition,
        FireControl,
        GCI
    }
}