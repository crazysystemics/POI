
using System;
using System.Diagnostics;

namespace SimpleARM
{
    enum SamplingMethod { EXHAUSTIVE_LINEAR, RANDOM_UNIFORM, RANDOM_GAUSSIAN}

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
        //public static Aircraft aircraft = new Aircraft();
       // public static Radar radar = new Radar();
        
        public static double estimate_height_01(Aircraft aircraft, Radar radar, int num_iterations)
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
        public static double estimate_height_02(Aircraft aircraft, Radar radar, int num_simulations, int num_iterations)
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
            
            Random rand = new Random();

            //Estimation of Optimal Height
            int sim = 0;
            //for (int sim = 0; sim < num_simulations/2; sim++)
            {
                // radar_x_error varies from -radar_error_band to + radar_error_band
                double radar_x_error = (rand.NextDouble() * 2 - 1) * radar_position_x_error_band;
                Radar radar_with_error = new Radar(radar_start_x + radar_x_error, radar_start_y, radar_range);
                Aircraft aircraft_sim = new Aircraft(radar_with_error.x - radar_with_error.range - 1.0, test_aircraft_y);


                Debug.Assert(Math.Abs(radar_with_error.x - radar_start_x) <= 0.0001 &&
                             Math.Abs(radar_with_error.y - radar_start_y) <= 0.0001);
                             
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
        public static double evaluate_solution(
                                            Aircraft aircraft, Radar radar,

                                            //Solution to be evaluated is aircraft.y
                                            double aircraft_y,

                                            //Fixed Parameters, for example, radar position and range
                                            double radar_x, 
                                            double radar_y,
                                            double radar_range,
                                            //error band should be deterministically increased in steps from min to max
                                            //once an error band is selected, num_trials simulations are run with
                                            //random radar positions based on random sample chosen based on error band
                                            double radar_origin_x_error_band_min,
                                            double radar_origin_x_error_band_max,
                                            SamplingMethod radar_x_error_sampling_method = SamplingMethod.RANDOM_UNIFORM,

                                            //Fixed Parameters related to Aircraft
                                            double aircraft_x_min=0.0,
                                            double aircraft_x_max=0.0,

                                            int num_trials=0,
                                            Random? rand=null,
                                            SamplingMethod radar_x_sampling_method = SamplingMethod.RANDOM_UNIFORM,
                                            SamplingMethod aircraft_x_sampling_method = SamplingMethod.EXHAUSTIVE_LINEAR

                                        )
        {
            if (rand == null)
            {
                rand = new Random();
            }
            double radar_start_x = radar.x;
            double radar_start_y = radar.y;
            aircraft.y = aircraft_y;

           
            int success_count = 0;

            double ax_step = 0;
           

            if (radar_x_sampling_method !=  SamplingMethod.RANDOM_UNIFORM)
            {
                Debug.Assert(false, "Prob Distn Not defined for Radar_x");
            }

            if (aircraft_x_sampling_method == SamplingMethod.EXHAUSTIVE_LINEAR)
            {
                ax_step = (aircraft_x_max - aircraft_x_min) / (double)num_trials;
            }
            {
                Debug.Assert(false, "Prob Distn Not defined for Aircraft_x");
            }


            //Evaluate Solution by Running Mission with Random Inputs
            //Here input varioation is in error associated with Radar x
            int trial_count = 0;
            while ( trial_count < num_trials)
            {
                //TODOGenerate Random Radar Position based on Uniform Distribution
                double radar_x_error = (rand.NextDouble() * 2 - 1) * radar_origin_x_error_band;
                Radar radar_with_error = new Radar(radar_start_x + radar_x_error, radar_start_y, radar.range);

                //aircraft.y is the solution
                //vary aircraft.x through its range and find how many times it is detected
                //since aircraft_x is EXHAUSTIVE, every random point of radar_x is checked with ALL_x'es of aircraft

                //Random Set, Exhaustive Set
                //In one evaluation run, one sample from each random variable is chosen, EXHAUSTIVE set for all points in sample set
                //Let us say r1, r2, r3 are random sets and
                //s1, s2, s3, s4 or exhaustive sampling
                // total number of iterations would be [s1 * s2 * s3 * s4]
                // in each of these iterations one variable will be chosen from r1, r2, r3
                while (aircraft.x < aircraft_x_max )
                {
                    bool isInRange = radar.IsAircraftInRange(aircraft);
                    if (isInRange)
                    {
                        //Detected
                        success_count++;
                    }

                    aircraft.x += ax_step;
                }

                trial_count++;
            }
            return (double)success_count/(double)num_trials;
        }
        public static void configure_run_simualtions()
        {
            Random rand = new Random();
            int num_radar_x_error_simulations = 10;
            int num_aircraft_y_simulations = 10;
            //double aircraft_start_x = -2.5, aircraft_start_y = 0.0;
            double radar_start_x = 0.0, radar_start_y = 0.0;
            double radar_range = 2.5;
            double test_radar_x_error_band = 0.0; // error band for radar x position            
            double optimal_aircraft_y = estimate_height_02(10);
            int detection_count = 0;
            int upper_detection_threshold = 3;
            int successful_missions = 0;

            test_radar_x_error_band = 0.0;
            //for (radar_position_x_error_band = 0.0; radar_position_x_error_band <= 1.5; radar_position_x_error_band += 0.25)
            {
                successful_missions = 0;

                for (int sim = 0; sim < num_radar_x_error_simulations; sim++)
                {
                    //Place Radar with Error
                    double radar_x_error = (rand.NextDouble() * 2 - 1) * test_radar_x_error_band;

                    Radar radar_with_error = new Radar(radar_start_x + radar_x_error, radar_start_y, radar_range);

                    //Optimal Height is computed in this routine
                    optimal_aircraft_y = estimate_height_02(num_aircraft_y_simulations);


                    Aircraft aircraft_sim = new Aircraft(radar_with_error.x - radar_with_error.range - 1.0,
                                                    optimal_aircraft_y);
                    //added test comment
                    detection_count = evaluate_solution(radar_with_error, aircraft_sim, optimal_aircraft_y);
                    if (detection_count <= upper_detection_threshold)
                        successful_missions++;
                }

                double success_rate = (double)(successful_missions / num_simulations) * 100.0;

                Console.WriteLine($"error band: {test_radar_x_error_band} mission success rate:{success_rate}");
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





