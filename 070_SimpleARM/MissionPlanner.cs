using SimpleARM;
using System;

namespace SimpleARM
{
    public class MissionPlanner
    {
        public MissionPlanner()
        {
        }

        public int calcOptimalPY(
                          int radar_x, int radar_y,
                          int visibility_radius = 3
                         )
        {
            //let us assume optimal_y maximally away from radar.y
            return radar_y + visibility_radius;
        }

        // Helper: compute detection count for a single aircraft altitude `py`.
        public int findDetectionCountForPY(double paramAy, Radar radar, int ax_min = -100, int ax_max = 100, int num_ax_samples = 10,
                                             int radar_range = 2)

        {
            // Ensure sensible num_ax_samples
            if (num_ax_samples <= 0) num_ax_samples = 1;

            // Compute step size from num_ax_samples (primary parameter)
            int ax_step = (int)Math.Ceiling((double)(ax_max - ax_min + 1) / num_ax_samples);
            if (ax_step < 1) ax_step = 1;

            int detection_count = 0;

            for (int ax = ax_min; ax <= ax_max; ax += ax_step)
            {
                Aircraft aircraft = new Aircraft(ax, paramAy);

                double act_range = Math.Sqrt((radar.x - aircraft.x) * (radar.x - aircraft.x) +
                                             (radar.y - aircraft.y) * (radar.y - aircraft.y));

                if (radar.IsAircraftInRange(aircraft))
                    detection_count++;
            }

            return detection_count;
        }

        // Captures iterated ay values in debug_ays reference parameter
        public int[] FindDetectionCounts(Aircraft aircraft, Radar radar,
                            double aymin, double aymax, int num_ay_samples,
                            int axmin, int axmax, int num_ax_samples,
                            double[]? out_debug_ays = null)
        {
            // Ensure sensible num_ay_samples
            if (num_ay_samples <= 0) num_ay_samples = 1;

            // Compute step size from num_ay_samples (primary parameter)
            double ay_step = (aymax - aymin) / num_ay_samples;
            if (ay_step < 1) ay_step = 1;

            // Calculate actual iterations using formula - O(1) instead of O(n)
            int actual_iterations = (int)((aymax - aymin) / ay_step) + 1;

            // Array dimension based on actual iterations
            int[] detectionCounts = new int[actual_iterations];

            int index = 0;

            for (double ay = aymin; ay <= aymax && index < actual_iterations; ay += ay_step)
            {
                if (out_debug_ays != null && index < out_debug_ays.Length)
                    out_debug_ays[index] = (int)ay;

                // count is detections for current ay
                int count = findDetectionCountForPY(ay, radar, axmin, axmax, num_ax_samples);
                if (index < detectionCounts.Length)
                    detectionCounts[index++] = count;
            }
            return detectionCounts;
        }

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


        public  bool isoptimal_y(
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

            int[] detect_counts = SGlobal.MissionPlanner.FindDetectionCounts(aircraft, radar,
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

        public double find_optimal_y(double minay, double maxay, Radar radar)
        {
            // Find the minimum y between minay and maxay that is OUTSIDE radar detection range
            // Radar detection zone: [radar.y - radar.range, radar.y + radar.range]

            double radar_min = radar.y - radar.range;
            double radar_max = radar.y + radar.range;

            // Priority 1: Return minimum y if entire range is already outside radar
            if (maxay < radar_min)  // Entire range is below radar
                return minay;

            if (minay > radar_max)  // Entire range is above radar
                return minay;

            // Priority 2: Try to go below radar (prefer lower altitudes for stealth)
            if (minay < radar_min)
                return radar_min - 1;

            // Priority 3: Go above radar (if we can't go below)
            if (maxay > radar_max)
                return radar_max + 1;

            // Priority 4: Trapped inside radar range - return minay as least-worst option
            return minay;
        }
    }
}


