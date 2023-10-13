static class Globals
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
    public static float DistanceCalculator(float[] obj1, float[] obj2)
    {
        float x = obj1[0] - obj2[0];
        float y = obj1[1] - obj2[1];
        return MathF.Sqrt((x * x) + (y * y));
    }

    public static float AngleCalculator(float[] obj1, float[] obj2)
    {
        float x = obj2[0] - obj1[0];
        float y = obj2[1] - obj1[1];
        float v = MathF.Atan2(y, x);
        return v;
    }
}