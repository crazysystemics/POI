
using System;
namespace SimpleARM
{
    //Notes
    //1. Turn by Turn Attack and Defence
    //2. Red attacks Blue, Blue defends and vice versa
    //3. Later both attack and defend in full-duplex mode
    //4. Define a problem, where given radars position and their range, determine safe flight path
    //4A. Start with single radar and single aircraft
    //4B. Next Two Radars and Single Aircraft

    class Aircraft
    {
        public double x, y;

        public Aircraft(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Radar
    {
        public double range;
        public double x, y;
        public Radar(double x, double y, double range)
        {
            this.range = range;
            this.x = x;
            this.y = y;
        }
        public bool IsAircraftInRange(Aircraft aircraft)
        {
            // Simplified logic for demonstration purposes
            double act_range = Math.Sqrt((x - aircraft.x) * (x - aircraft.x) + (y - aircraft.y) * (y - aircraft.y));
            bool inRange = (act_range <= range);
            Console.WriteLine($"Radar at ({x}, {y}) aircraft at ({aircraft.x}, {aircraft.y}) actual range ({act_range})");
            return inRange;
        }
    }
    internal class Program
    {
        public static Aircraft aircraft  = new Aircraft(-2.5, 2.4);
        public static Radar    radar     = new Radar(0.0, 0.0, 2.5);
        public static int      num_iterations = 100;

        static void Main(string[] args)
        {

            int detected_count = 0;
            bool wasInRange = radar.IsAircraftInRange(aircraft);
            int entry_i = -1, exit_i =-1;
            for (int i = 0; i < num_iterations; i++)
            {
                //generate code to find values of i when aircraft enters radar range
                //and when exits radar range
                // Track when aircraft enters and exits radar range                
                aircraft.x += 1.0;
                //aircraft.y += 1.0;

                bool isInRange = radar.IsAircraftInRange(aircraft);
                if (isInRange)
                {
                    detected_count++;
                }

                if (!wasInRange && isInRange)
                {
                    entry_i = i;
                    
                }
                else if (wasInRange && !isInRange)
                {
                    exit_i = i;
                    
                }
                wasInRange = isInRange;
            }

            //Console.WriteLine("Hello, World!");
            System.Console.WriteLine($"Aircraft detected {detected_count} times.");
            System.Console.WriteLine($"Aircraft entered radar range at iteration {entry_i}.");
            System.Console.WriteLine($"Aircraft exited radar range at iteration {exit_i}.");
        }
    }
}
