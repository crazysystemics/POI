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
    private  int findDetectionCountForPY(int paramAy, Radar radar, int ax_min = -100, int ax_max = 100, int num_samples = 10, 
                                         int radar_range = 2)
                                         
    {
        // Ensure sensible num_samples
        if (num_samples <= 0) num_samples = 1;

        // compute an integer step for ax iteration; ensure >= 1
        int step = (int)Math.Ceiling((2.0 * (ax_max - ax_min + 1)) / (double)num_samples);
        if (step < 1) step = 1;

        int detection_count = 0;

        //int ax_start = radar_x - ax_len;
        //int ax_end = radar_x + ax_len;
        for (int ax = ax_min; ax <= ax_max; ax += step)
        {
            Aircraft aircraft = new Aircraft(ax, paramAy);          

            double act_range = Math.Sqrt((radar.x - aircraft.x) * (radar.x - aircraft.x) +
                                         (radar.y - aircraft.y) * (radar.y - aircraft.y));

            if (radar.IsAircraftInRange(aircraft))
                detection_count++;
        }

        return detection_count;
    }



    // New method implementing the TODO described in comments:
    // - remove single `py` parameter, accept py_min and py_max
    // - run detection_count for each py in [py_min, py_max]
    // - return the py with the minimum detection count (optimal_y)
    // OptimalPY0 is when there is no error in radar position (radar_x, radar_y are fixed and known)
    //public  int FindFirstOptimalPY0(int py_min, int py_max,
    //                               int radar_x = 0, int radar_y = 0,
    //                               int ax_len = 100, int num_samples = 10, double radar_range = 2.5)
    //{
    //    if (py_max < py_min)
    //    {
    //        // invalid range -> return py_min as fallback
    //        return py_min;
    //    }

    //    int bestPy = py_min;
    //    int bestCount = int.MaxValue;
    //    int[] detnCounts = new int[num_samples];
    //    int index = 0;

    //    for (int py = py_min; py <= py_max; py++)
    //    {
            
    //        int count = findDetectionCountForPY(py, radar, ax_min: -100, ax_max: 100, num_samples: num_samples, radar_range: (int)radar_range);
            
    //        if (index < detnCounts.Length)
    //            detnCounts[index++] = count;

    //        if (count < bestCount)
    //        {
    //            bestCount = count;
    //            bestPy = py;
    //        }
    //    }

    //    return bestPy;
    //}

    public int[] FindDetectionCounts(Aircraft aircraft, Radar radar,
                        int aymin, int aymax, int aystep,
                        int axmin, int axmax, int ax_num_samples)
    {
        int numPySamples = (aymax - aymin) / aystep + 1;
        int[] detectionCounts = new int[numPySamples];
        int index = 0;
        for (int ay = aymin; ay <= aymax; ay += aystep)
        {
            int count = findDetectionCountForPY(ay, radar, axmin,  axmax, ax_num_samples);
            if (index < detectionCounts.Length)
                detectionCounts[index++] = count;
        }
        return detectionCounts;
    }
}
