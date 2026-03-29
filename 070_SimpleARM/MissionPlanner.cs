using SimpleARM;
using System;

public class MissionPlanner
{
	public MissionPlanner()
	{
	}

    public  int calcOptimalPY(int min_y, int max_y,
                      int radar_x, int radar_y                    
                     )
    {
        //let us assume optimal_y maximally away from radar.y
        return Math.Max(Math.Abs(max_y - radar_y), Math.Abs(min_y - radar_y));
    }

    // Helper: compute detection count for a single aircraft altitude `py`.
    // This was extracted from the original `detection_count` logic.
    private  int findDetectionCountForPY(int py, int radar_x = 0, int radar_y = 0,
                                         int ax_len = 100, int num_samples = 10, double radar_range = 2.5)
    {
        // Ensure sensible num_samples
        if (num_samples <= 0) num_samples = 1;

        // compute an integer step for ax iteration; ensure >= 1
        int step = (int)Math.Ceiling((2.0 * ax_len) / (double)num_samples);
        if (step < 1) step = 1;

        int detection_count = 0;

        int ax_start = radar_x - ax_len;
        int ax_end = radar_x + ax_len;

        for (int ax = ax_start; ax <= ax_end; ax += step)
        {
            Aircraft ac = new Aircraft(ax, py);
            Radar radar = new Radar(radar_x, radar_y, radar_range);

           

            if (radar.IsAircraftInRange(ac))
                detection_count++;
        }

        return detection_count;
    }

    // New method implementing the TODO described in comments:
    // - remove single `py` parameter, accept py_min and py_max
    // - run detection_count for each py in [py_min, py_max]
    // - return the py with the minimum detection count (optimal_y)
    // OptimalPY0 is when there is no error in radar position (radar_x, radar_y are fixed and known)
    public  int FindOptimalPY0(int py_min, int py_max,
                                   int radar_x = 0, int radar_y = 0,
                                   int ax_len = 100, int num_samples = 10, double radar_range = 2.5)
    {
        if (py_max < py_min)
        {
            // invalid range -> return py_min as fallback
            return py_min;
        }

        int bestPy = py_min;
        int bestCount = int.MaxValue;

        for (int py = py_min; py <= py_max; py++)
        {
            int count = findDetectionCountForPY(py, radar_x, radar_y, ax_len, num_samples, radar_range);
            if (count < bestCount)
            {
                bestCount = count;
                bestPy = py;
            }
        }

        return bestPy;
    }
}
