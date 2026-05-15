using SimpleARM;
using System;

public class MissionPlanner
{
	public MissionPlanner()
	{
	}

    public  int calcOptimalPY(
                      int radar_x, int radar_y,
                      int visibility_radius = 3
                     )
    {
        //let us assume optimal_y maximally away from radar.y
        return radar_y + visibility_radius;
    }

    // Helper: compute detection count for a single aircraft altitude `py`.
    // This was extracted from the original `detection_count` logic.
    private  int findDetectionCountForPY(int paramAy, Radar radar, int ax_min = -100, int ax_max = 100, int num_ax_samples = 10, 
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
                        int aymin, int aymax, int num_ay_samples,
                        int axmin, int axmax, int num_ax_samples,
                        int[] out_debug_ays=null)
    {
        // Ensure sensible num_ay_samples
        if (num_ay_samples <= 0) num_ay_samples = 1;

        // Compute step size from num_ay_samples (primary parameter)
        int ay_step = (int)Math.Ceiling((double)(aymax - aymin + 1) / num_ay_samples);
        if (ay_step < 1) ay_step = 1;

        // Calculate actual iterations using formula - O(1) instead of O(n)
        int actual_iterations = (aymax - aymin) / ay_step + 1;

        // Array dimension based on actual iterations
        int[] detectionCounts = new int[actual_iterations];
        
        int index = 0;

        for (int ay = aymin; ay <= aymax && index < actual_iterations; ay += ay_step)
        {
            if ( out_debug_ays != null && index < out_debug_ays.Length)
                        out_debug_ays[index] = ay;
            //count is detections for current ay
            int count = findDetectionCountForPY(ay, radar, axmin, axmax, num_ax_samples);
            if (index < detectionCounts.Length)
                detectionCounts[index++] = count;
        }
        return detectionCounts;
    }
}


