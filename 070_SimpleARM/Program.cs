
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
    //parameter_distance is x y component of distance from radar
    //aproach_distance is y component of distance from radar
    //parameter_distance will vary from -radar.range - delta_parameter_distance to + radar.range + delta_parameter_distance
    //approach_dist will vary from -radar.range - delta_approach_distance  to + radar.range + delta_approach_distance

    //27-11-2025 THURSDAY 
    //LOG-----------------------------------------------
    //IDEAS---------------------------------------------
    //1. Decision Point - a point in vertical strip from which aircraft enters battlefield and goes in straight line
    //2. For one radar and deterministic model, it is straight-forward, just tangetial to radar circle
    //   escaping its visibility
    //3. But when errors are factored in, it becomes complex. Errors may be in position of radar , its range
    //4. Another source of complexity is multiple radars
    //5. Multiple Radars + Errors in position and range of radars make it more complex
    //6. How RWR can be used to improve situation?
    //7. Mission Effectiveness with or without RWR can be studied


    //28-11-2025 THURSDAY 
    //LOG-----------------------------------------------
    //1. ay is varied from -radar.range to + radar.range 
    //2. For each ay, aircraft is moved from left to right in num_iterations steps
    //3. For each position of aircraft, check if it is in radar range
    //4. [ax X ay] constitutes the state space
    //IDEAS---------------------------------------------


    //02-12-2025 TUESDSAY
    //LOG-----------------------------------------------
    //1. detection_count is coming as 1 for every ay. Should examine why.

    //05-12-2025 FRIDAY
    //LOG-----------------------------------------------
    //1. detection_count is incrementing now.
    //2. Many problems were there. But main one was that aircraft position was not updated properly in inner loop.
    //3. increment step was divided by iteration_no instead of num_iterations
    //15-12-2025 MONDAY
    //   TODO:
    //       1.Error in position of radar
    //IDEAS-----------------------------------------------
    //1. Plot graph of detection_count v/s ay to see how it varies.
    //2. DO F11 and verify the flow
    //3. Try two/three radars and see how effectivness is calculated (vertical strip, straight flight path)
    //4. Check the effectivness when there is error in radar position/range
    //5. Check the effectivness when RWR information is used to update position of radar (with error)



    class Aircraft
    {
        public double x, y;

        public Aircraft(double x=0.0, double y = 0.0)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Radar
    {
        public double range;
        public double x, y;
        public Radar(double x= 0.0, double y = 0.0, double range = 0.0)
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
        public static Aircraft aircraft = new Aircraft();
        public static Radar radar   = new Radar();
        public static int num_iterations = 100;

        static void Main(string[] args)
        {
            //input parameters
            double aircraft_start_x = -2.5, aircraft_start_y = 0.0;
            double radar_start_x = 0.0, radar_start_y = 0.0;    
            double radar_range = 2.5;
            double num_samples = 10;
            double parameter_distance_step = (radar_range * 2 )/ (double) num_samples;

           //output parameters
            int detected_count = 0;

            radar = new Radar(radar_start_x, radar_start_y, radar_range);
            aircraft = new Aircraft(aircraft_start_x, aircraft_start_y);

            double parameter_distance;            
            bool wasInRange = radar.IsAircraftInRange(aircraft);         
            int entry_i = -1, exit_i = -1;

            aircraft_start_x = radar.x - radar.range - 1.0;
            aircraft_start_y = radar.y;
            


            for (parameter_distance = -radar.range;
                 parameter_distance < radar.range;
                 parameter_distance += parameter_distance_step)
            {
                aircraft = new Aircraft(aircraft_start_x, aircraft_start_y + parameter_distance);
                detected_count = 0;

                for (int iteration_no = 0; iteration_no < num_iterations; iteration_no++)
                {
                    aircraft = new Aircraft(aircraft.x + radar.range * 2/ num_iterations, 
                                            aircraft.y );

                    bool isInRange = radar.IsAircraftInRange(aircraft);
                    if (isInRange)
                    {
                        detected_count++;
                    }

                    if (!wasInRange && isInRange)
                    {
                        entry_i = iteration_no;

                    }
                    else if (wasInRange && !isInRange)
                    {
                        exit_i = iteration_no;

                    }

                    wasInRange = isInRange;

                }

                Console.WriteLine($"{aircraft.y}, {detected_count} ");
                //System.Console.WriteLine($"Parameter {parameter_distance} \t Detection {detected_count} times.");
                //System.Console.WriteLine($"Aircraft entered radar range at iteration {entry_i}.");
                //System.Console.WriteLine($"Aircraft exited radar range at iteration {exit_i}.");
            }
        }
    }
}
