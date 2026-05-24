using SimpleARM;
using System;

namespace SimpleARM
{
    public class MissionPlanner
    {
        public MissionPlanner()
        {
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

        

        public (bool passed, double passRate) isoptimal_y_statistical(
            double testy,
            Radar[] sampleRadars,
            int axmin, int axmax, int num_ax_samples,
            double aymin, double aymax, int ay_num_samples,
            double required_pass_rate)
        {
            if (sampleRadars == null || sampleRadars.Length == 0)
                return (false, 0.0);

            Aircraft tempAircraft = new Aircraft(axmin, testy);
            int pass_count = 0;

            foreach (var sampleRadar in sampleRadars)
            {
                // testy passes for this sample if it achieves the minimum detection count.
                // Less strict than isoptimal_y: does not require testy to be the lowest y
                // achieving that count — only that it is not beaten by any sampled y.
                int testy_count = findDetectionCountForPY(testy, sampleRadar, axmin, axmax, num_ax_samples);
                int[] detect_counts = SGlobal.MissionPlanner.FindDetectionCounts(
                    tempAircraft, sampleRadar,
                    aymin, aymax, ay_num_samples,
                    axmin, axmax, num_ax_samples);
                if (testy_count <= detect_counts.Min())
                    pass_count++;
            }

            double passRate = (double)pass_count / sampleRadars.Length;
            return (passRate >= required_pass_rate, passRate);
        }

        public  bool isoptimal_y(
                                        double testy, Aircraft aircraft, Radar radar,
                                        int axmin, int axmax, int ax_num_samples,
                                        double aymin, double aymax, int ay_num_samples,
                                        double[] debug_ays,
                                        int[] debug_detect_counts
                                       )
        {
            //[CLAUDE] Validation in case of Test05 should be statistical
            //[CLAUDE] There will be a non-zero probability that for some visibility zone
            //[CLAUDE] tesy will fail. But for a percentage of cases, it should pass
            //[CLAUDE] It should accept pass percentage as a parameter

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

            // Compute testy's detection count directly — testy may not fall on the sampled grid.
            int testy_count = findDetectionCountForPY(testy, radar, axmin, axmax, ax_num_samples);

            int minVal = detect_counts.Min();

            // testy must achieve the minimum detection count.
            if (testy_count > minVal)
                return false;


            // No sampled y strictly below testy may achieve a count <= testy_count;
            // that would mean a lower (safer) altitude already beats testy.
            for (int i = 0; i < detect_counts.Length; i++)
            {
                double sampled_y = aymin + i * aystep;
                if (sampled_y < testy && detect_counts[i] <= testy_count)
                    return false;
            }

            return true;
        }

        
    }
}


