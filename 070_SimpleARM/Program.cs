
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
    //17-12-2025 MONDAY
    //   TODO:
    //       1.Error in position of radar
    //       2.Error in range of radar
    //       3.Use of RWR information (with error) to update radar position    
    //       4.When taken average, will effectiveness be maximum at 0 error position/range?
    //       5.Plot graph of effectiveness v/s error in position/range and very hypothesis in point 4
    //18-12-2025 THURSDAY
    //  1. [LOG-]Found optimal aircraft y position with minimum detection count
    //  2. [IDEA]minimum detection count is mission effectiveness in this case
    //  3. [TODO]Introduce Error in radar position/range
    //  4. [TODO]Find optimal aircraft y position with minimum detection count
    //  5. [TODO]Test optimal y with actual aircraft position
    //  6. [TODO]How can we minimize detection count for different errors?
    //20-12-2025 SATURDAY
    // BLUE-ATTACK:
    // In Main Method
    // 1. [TODO] Run once with all heights and find optimal height with minimum detection count
    // 2. [TODO] But Radar position is error
    // 3. [TODO] Place Radar with Error and optimal launch point
    // 4. [TODO] Repeat Launches with same lauch height but with different random radar positions within error band
    // 5. [TODO] Restrict randomness to x dimension only first
    // 4. [INSIGHT] Across multiple simulations, radar position error should average to zero
    // ------------
    // Spent one hour coding 
    //----------------
    //1.[LOG] Completed estimate_height_02 method to find optimal height with radar position error
    //2.[LOG] Completed configure_run_simulations method to run multiple simulations with random radar positions
    //3 [TODO] Run simulations with different error bands and find success rate (should get from 0 to 100%)
    //4.[TODO] Vary Radar Position in Y axis also
    // RED-DEFENCE
    //24-12-2025 WEDNESDAY
    //1. [LOG] Updated .NET to 8.0 to support VS 2026
    //2. [LOG] Ran simulations with different error bands and found success rate (got from 0 to 100%)
    //3. [LOG] Mission success is 0 even for 0 error band. Need to investigate.
    //31-12-2025 WEDNESDAY
    //1. [LOG] Git is not working (refer GITHUB COPILOT)
    //2. [TODO]Download SysInternals Handle and Proess Explorer to find file locks





    class Aircraft
    {
        public double x, y;

        public Aircraft(double x = 0.0, double y = 0.0)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Radar
    {
        public double range;
        public double x, y;
        public Radar(double x = 0.0, double y = 0.0, double range = 0.0)
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
    // ... (existing classes)

    internal class Program
    {
        public static Aircraft aircraft = new Aircraft();
        public static Radar radar = new Radar();
        public static int num_iterations = 100;
        public static double estimate_height_01()
        {
            double aircraft_start_x = -2.5, aircraft_start_y = 0.0;
            double radar_start_x = 0.0, radar_start_y = 0.0;
            double radar_range = 2.5;
            int num_samples = 10;
            double parameter_distance_step = (radar_range * 2) / (double)num_samples;

            int detected_count = 0;
            radar = new Radar(radar_start_x, radar_start_y, radar_range);
            aircraft = new Aircraft(aircraft_start_x, aircraft_start_y);

            double parameter_distance;
            bool wasInRange = radar.IsAircraftInRange(aircraft);
            int entry_i = -1, exit_i = -1;
            int min_detection_count = num_iterations + 1;
            double optimal_aircraft_y = 0;

            // 1. Find optimal height with minimum detection count (no radar error)
            for (parameter_distance = -radar.range;
                 parameter_distance < radar.range;
                 parameter_distance += parameter_distance_step)
            {
                aircraft = new Aircraft(aircraft_start_x, aircraft_start_y + parameter_distance);
                detected_count = 0;
                wasInRange = radar.IsAircraftInRange(aircraft);

                for (int iteration_no = 0; iteration_no < num_iterations; iteration_no++)
                {
                    aircraft = new Aircraft(aircraft.x + radar.range * 2 / num_iterations, aircraft.y);
                    bool isInRange = radar.IsAircraftInRange(aircraft);
                    if (isInRange)
                        detected_count++;

                    if (!wasInRange && isInRange)
                        entry_i = iteration_no;
                    else if (wasInRange && !isInRange)
                        exit_i = iteration_no;

                    wasInRange = isInRange;
                }

                if (detected_count < min_detection_count)
                {
                    min_detection_count = detected_count;
                    optimal_aircraft_y = aircraft.y;
                }
            }

            Console.WriteLine($"Optimal Aircraft Y Position: {optimal_aircraft_y} with minimum detections: {min_detection_count}");

            return optimal_aircraft_y;
        }
        public static double estimate_height_02()
        {
            // 2. Radar position error (randomness in x only)

            double aircraft_start_x = -2.5, aircraft_start_y = 0.0;
            double radar_start_x = 0.0, radar_start_y = 0.0;
            double radar_range = 2.5;
            int num_samples = 10;
            double parameter_distance_step = (radar_range * 2) / (double)num_samples;
            bool wasInRange = radar.IsAircraftInRange(aircraft);
            int min_detection_count = int.MaxValue;
            double test_aircraft_y = 0.0;
            double optimal_aircraft_y = 0.0;

            radar = new Radar(radar_start_x, radar_start_y, radar_range);
            aircraft = new Aircraft(aircraft_start_x, aircraft_start_y);

            aircraft_start_x = radar.x - radar.range - 1.0;
            aircraft_start_y = radar.y;

            double radar_position_x_error_band = 0.0; // error band for radar x position
            int num_simulations = 100;
            Random rand = new Random();

            //Estimation of Optimal Height
            for (int sim = 0; sim < num_simulations/2; sim++)
            {
                // radar_x_error varies from -radar_error_band to + radar_error_band
                double radar_x_error = (rand.NextDouble() * 2 - 1) * radar_position_x_error_band;
                Radar radar_with_error = new Radar(radar_start_x + radar_x_error, radar_start_y, radar_range);
                Aircraft aircraft_sim = new Aircraft(radar_with_error.x - radar_with_error.range - 1.0, test_aircraft_y);
                int sim_detected_count = 0;
                bool sim_wasInRange = radar_with_error.IsAircraftInRange(aircraft_sim);

                for (int iteration_no = 0; iteration_no < num_iterations; iteration_no++)
                {
                    aircraft_sim.x += radar_with_error.range * 2 / num_iterations;

                    bool isInRange = radar_with_error.IsAircraftInRange(aircraft_sim);
                    if (isInRange)
                        sim_detected_count++;
                    sim_wasInRange = isInRange;
                }

                if (sim_detected_count < min_detection_count)
                {
                    min_detection_count = sim_detected_count;
                    optimal_aircraft_y = test_aircraft_y;
                }

                test_aircraft_y += radar.range / num_simulations; // increment aircraft y for next simulation

                
            }

            return test_aircraft_y;
        }      
        public static int simulate_mission(Radar radar, Aircraft aircraft, double plannedHeight)
        {
            aircraft.y = plannedHeight;
            int detection_count = 0;
            //Run Mission
            for (int iteration_no = 0; iteration_no < num_iterations; iteration_no++)
            {
                aircraft.x += radar.range * 2 / num_iterations;
                bool isInRange = radar.IsAircraftInRange(aircraft);
                if (isInRange)
                {
                    //Detected
                    detection_count++;
                }
            }
            return detection_count;
        }
        public static void configure_run_simualtions()
        {
            Random rand = new Random();
            int num_simulations = 100;
            //double aircraft_start_x = -2.5, aircraft_start_y = 0.0;
            double radar_start_x = 0.0, radar_start_y = 0.0;
            double radar_range = 2.5;
            double radar_position_x_error_band = 0.0; // error band for radar x position            
            double optimal_aircraft_y = estimate_height_02();
            int detection_count = 0;
            int upper_detection_threshold = 3;
            int successful_missions = 0;

            for (radar_position_x_error_band = 0.0; radar_position_x_error_band <= 1.5; radar_position_x_error_band += 0.25)
            {
                successful_missions = 0;

                for (int sim = 0; sim < num_simulations; sim++)
                {
                    //Place Radar with Error
                    double radar_x_error = (rand.NextDouble() * 2 - 1) * radar_position_x_error_band;
                    Radar radar_with_error = new Radar(radar_start_x + radar_x_error, radar_start_y, radar_range);
                    optimal_aircraft_y = estimate_height_02();
                    Aircraft aircraft_sim = new Aircraft(radar_with_error.x - radar_with_error.range - 1.0,
                                                    optimal_aircraft_y);
                    //added test comment
                    detection_count = simulate_mission(radar_with_error, aircraft_sim, optimal_aircraft_y);
                    if (detection_count <= upper_detection_threshold)
                        successful_missions++;
                }

                double success_rate = (double)(successful_missions / num_simulations) * 100.0;

                Console.WriteLine($"error band: {radar_position_x_error_band} mission success rate:{success_rate}");
            }

        }
        static void Main(string[] args)
        {
            //Testing Optimal Height
            //Run simulations with random radar positions within error band (across x) 
            //and calculate average detection count
            //Optimation Height Estimation (Training may be any method) this is Test           
            configure_run_simualtions();
        }
    }
}





