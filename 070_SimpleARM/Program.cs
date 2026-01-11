
using System;
namespace SimpleARM
{
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





