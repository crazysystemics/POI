namespace SimpleARM
{
    public class Aircraft(double x = 0.0, double y = 0.0)
    {
        public double x = x, y = y;
    }

    public class Radar(int x = 0, int y = 0, int range = 0)
    {
        public int x = x, y = y, range = range;

        public bool IsAircraftInRange(Aircraft aircraft)
        {
            double act_range = Math.Sqrt((x - aircraft.x) * (x - aircraft.x) +
                                         (y - aircraft.y) * (y - aircraft.y));
            return act_range <= range + 0.11; // 0.11 is floating point tolerance
        }
    }

    public class Battlespace(Radar radar, Aircraft aircraft,
                              double axMin, double axMax, double ayMin, double ayMax)
    {
        //Initial Configuration of enemy
        public Radar Radar = radar;
        public Aircraft Aircraft = aircraft;
        public double ox, oy;
        public double AxMin = axMin, AxMax = axMax;
        public double AyMin = ayMin, AyMax = ayMax;

        // Mission results — populated by MissionPlanner after planning
        // Result or Solution for Blue (Self)
        public double OptimalAy;

        //fitness metric associated with solution optimal y
        public int OptimalAyDetectCount;
        //In this case it is Parameter Distance vs Detection Count
        Dictionary<double, int> fitnessVector = new Dictionary<double, int>();

        //Result of Validation
        public bool IsAyOptimal;
    }
}
