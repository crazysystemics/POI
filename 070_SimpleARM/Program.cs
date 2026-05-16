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
                return inRange;
        }
    }

    internal class Program
    {
        public static int find_optimal_y(double minay, double maxay, Radar radar)                
        {
            //let us assume optimal_y maximally away from radar.y
            //  radar.y-radar.range to radar.y + rdar.range is one rectangle
            //  Overlapping rectangle is
            //  from max(minay, radar.y - radar.range) to min(maxay, radar.y + radar.range)
            //  Minimum point between minay and maxay that is outside the radar range
            //  ie., Overlapping rectangle's upper bound + next point or lower bound - next point
            return (int)Math.Max(Math.Abs(maxay - radar.y), Math.Abs(minay - radar.y));
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
                                        double testy, Aircraft aircraft, Radar radar,
                                        int axmin, int axmax, int ax_num_samples,
                                        double aymin, double aymax, int ay_num_samples,
                                        double[] debug_ays,
                                        int[] debug_detect_counts
                                       )
        {         
            
            aircraft.y = testy;

            double aystep = (aymax - aymin) / ay_num_samples;
            if (aystep < 1) aystep = 1;

            int[] detect_counts = sglobal.MissionPlanner.FindDetectionCounts(aircraft, radar, 
                                                                               aymin, aymax, ay_num_samples,
                                                                               axmin, axmax, ax_num_samples,
                                                                               debug_ays);
            
            // Copy detection counts element by element to debug_detect_counts
            int copyLength = Math.Min(detect_counts.Length, debug_detect_counts.Length);
            Array.Copy(detect_counts, debug_detect_counts, copyLength);
            
            if (detect_counts == null || detect_counts.Length == 0)
                return false;

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

            double minPy = aymin + minIndex * aystep;
            return (minPy == testy);
        }
        static void Main(string[] args)
        {
            Test test = new Test();
            test.Test01();

        }
    }
}

































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































