
using System;
namespace SimpleARM
{
    //Notes
    //1. Turn by Turn Attack and Defence
    //2. Red attacks Blue, Blue defends and vice versa
    //3. Later both attack and defend in full-duplex mode
    //3A Problem may be to define Mission Effectiveness of EW-Suite (on let us say S3 kind of aircraft)
    //   Without EW-Suite, With EW-Suite (various modes)
    //4. Define a problem, where given radars position and their range, determine safe flight path
    //4A. Start with single radar and single aircraft
    //4A.1 With Single Radar known position and visibility Radius (sector) - with uncertainty factored in?
    //41.2 What improvement can be made if real-time position by RWR is given? (visibility is still uncertain)
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
            //Console.WriteLine($"Radar at ({x}, {y}) aircraft at ({aircraft.x}, {aircraft.y}) actual range ({act_range})");
            return inRange;
        }
    }
    internal class Program
    {
        public static Aircraft aircraft  = new Aircraft(0.0, 0.0);
        public static Radar    radar     = new Radar(0.0, 0.0, 2.5);
        public static int      num_iterations = 100;

        static void Main(string[] args)
        {

            double parameter_distance = 0.0;
            double dist_from_radar = radar.range;
            int detected_count = 0;
            bool wasInRange = radar.IsAircraftInRange(aircraft);
            int entry_i = -1, exit_i =-1;
            double pd = radar.x - radar.range;
                
            
            for (int iteration_no  = 0; iteration_no < num_iterations; iteration_no++)
            {
                parameter_distance = parameter_distance + radar.range / num_iterations;
                //TODO: Generate 2D state-space to see to determine flight path

                for (int dist = 0; dist < num_iterations; i++)
            {
                //generate code to find values of i when aircraft enters radar range
                //and when exits radar range
                // Track when aircraft enters and exits radar range                
                //aircraft.x += 1.0;
                //aircraft.y += 1.0;
                aircraft = new Aircraft(aircraft.x , 
                                          pd);



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

                pd = pd + radar.range / num_iterations
                wasInRange = isInRange;
            }

            //Console.WriteLine("Hello, World!");
            System.Console.WriteLine($"Aircraft detected {detected_count} times.");
            //System.Console.WriteLine($"Aircraft entered radar range at iteration {entry_i}.");
            //System.Console.WriteLine($"Aircraft exited radar range at iteration {exit_i}.");
        }
    }
}
