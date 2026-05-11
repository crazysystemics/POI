using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleARM
{
    enum SamplingMethod { EXHAUSTIVE_LINEAR, RANDOM_UNIFORM, RANDOM_GAUSSIAN }

    public class Aircraft
    {
        public double x, y;

        public Aircraft(double x = 0.0, double y = 0.0)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Radar
    {
        public int range;
        public int x, y;
        public Radar(int x = 0, int y = 0, int range = 0)
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

        public static int find_optimal_y(int minay, int maxay, Radar radar)                
        {
            //let us assume optimal_y maximally away from radar.y
            return Math.Max(Math.Abs(maxay - radar.y), Math.Abs(minay - radar.y));
        }

        //Find the mission effectiveness (detection rate) for a given testy and radar error range
        //by simulating multiple runs with random radar positions and calculating the average detection count
        public static double FindMissionEffectiveness(                            
                            int testy,
                            int ac_min_x, int ac_max_x,
                            int min_radar_x, int max_radar_x, int radar_y,
                            int min_radar_x_error, int max_radar_x_error,
                            int num_samples,
                            ref int best_score_index)
        {
            // Validate inputs
            if (num_samples <= 0)
                num_samples = 1;

            if (ac_max_x < ac_min_x || max_radar_x < min_radar_x)
            {
                best_score_index = -1;
                return 0.0;
            }

            // Use a reasonable default radar range if not provided elsewhere in the program.
            double radar_range = 2.5;

            Random rand = new Random();

            int radarSamples = num_samples;
            double radarStep = (radarSamples > 1) ? (max_radar_x - min_radar_x) / (double)(radarSamples - 1) : 0.0;
            double acStep = (radarSamples > 1) ? (ac_max_x - ac_min_x) / (double)(radarSamples - 1) : (ac_max_x - ac_min_x);

            long totalIterations = 0;
            long detectionCount = 0;
            Aircraft ac;

            for (int cur_ac_x = ac_min_x; cur_ac_x < ac_max_x; cur_ac_x += ((ac_max_x) - (ac_min_x)) / num_samples)
            {

                ac = new Aircraft(ac_min_x, testy);

                // Radar position without error
                for (double cur_radar_x = min_radar_x; cur_radar_x < max_radar_x;
                           cur_radar_x += (max_radar_x - min_radar_x) / num_samples)
                {

                    //given radar_x_min and radar_x_max
                    //      radar_y_min and radar_y_max 
                    //      generate random_x, and random_y between min and max
                    //      mission_success_rate = 1 -(detection_count/num_trials)

                    for (int i = 0; i < radarSamples; i++)
                    {
                        // Linearly increment the radar x error range between min and max
                        double cur_radar_x_error_max = (radarSamples > 1)
                            ? (min_radar_x_error + i * (double)(max_radar_x_error - min_radar_x_error) / (radarSamples - 1))
                            : min_radar_x_error;

                        // Sample an actual error from uniform distribution [0, cur_radar_x_error_max]
                        double cur_radar_x_error_sample = rand.NextDouble() * Math.Max(0.0, cur_radar_x_error_max);

                        // Offset so that error is centered around the nominal position
                        double cur_radar_x_with_error = cur_radar_x - (cur_radar_x_error_max / 2.0) + cur_radar_x_error_sample;

                        Radar radar = new Radar((int)cur_radar_x_with_error, radar_y, (int)radar_range);

                        // Single check at ac_min_x when acStep is effectively zero
                        totalIterations++;

                        if (radar.IsAircraftInRange(ac))
                            detectionCount++;
                    }

                }

            }

            // Avoid division by zero
            if (totalIterations == 0)
            {
                return 0.0;
            }


            return (double)detectionCount / (double)totalIterations;
        }


        public static bool isoptimal_y(
                                        int testy, Aircraft aircraft, Radar radar,
                                        int axmin, int axmax, int ax_num_samples
                                       )
        {
           
            
            aircraft.y = testy;

            int aymin  = (int)radar.y;
            int aymax  = (int)(radar.y + radar.range);
            int aystep = (int)((aymax - aymin) / 10.0); 

            //[***] write a function to find detection_count for a given py
            //[***] min_detection = find_index_of_minimum(detection_counts)
            //[***] assert that pymin + min_detection * pstep == pdist
            int[] detect_counts = sglobal.MissionPlanner.FindDetectionCounts(aircraft, radar, 
                                                                               aymin, aymax, aystep,
                                                                               axmin,axmax, ax_num_samples);

            // Guard against empty/null results
            if (detect_counts == null || detect_counts.Length == 0)
                return false;

            // Find index of minimum detection count without relying on LINQ
            int minVal = int.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < detect_counts.Length; i++)
            {
                if (detect_counts[i] < minVal)
                {
                    minVal = detect_counts[i];
                    minIndex = i;
                }
            }

            if (minIndex < 0)
                return false;

            int minPy = aymin + minIndex * aystep;
            return (minPy == testy);
        }
        static void Main(string[] args)
        {
            Test test = new Test();
            test.Test01();

        }
    }
}

















