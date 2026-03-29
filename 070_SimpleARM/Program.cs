using System;
using System.Diagnostics;

namespace SimpleARM
{
    enum SamplingMethod { EXHAUSTIVE_LINEAR, RANDOM_UNIFORM, RANDOM_GAUSSIAN }

    

    internal class Program
    {
        /*
         PSEUDOCODE / PLAN (detailed)
         -------------------------------------
         Goal:
           - Implement the comment that says:
             "To detection_count method parameters, remove py and add py_min and py_max
              retain all other parameters. run detection_count from py_min to py_max
              and return py with minimum detection count as optimal_y"

         Steps:
           1. Extract the logic that computes the number of detections for a single aircraft
              altitude (py) into a helper method `DetectionCountForY`.
              - Inputs: py, radar_x, radar_y, ax_len, num_samples
              - Compute an integer step for scanning aircraft x across [radar_x - ax_len, radar_x + ax_len]
                using num_samples. Ensure step >= 1 to avoid infinite loops.
              - For each ax in that range, create `Aircraft(ax, py)` and `Radar(radar_x, radar_y, range)`
                and call `IsAircraftInRange` to count detections.
              - Return the integer detection count.

           2. Implement `FindOptimalY` method that scans py from py_min to py_max (inclusive)
              and uses `DetectionCountForY` to compute detection counts per py.
              - Inputs: py_min, py_max, radar_x, radar_y, ax_len, num_samples
              - Iterate py integer values from py_min to py_max (step = 1).
              - Track the py value that yields the minimum detection count.
              - In case of ties, choose the py with lower detection count first encountered.
              - Return the optimal py (int). Optionally could return detection count as out/ref but the
                requirement is to "return py with minimum detection count".

           3. Replace/augment the old `detection_count` function with these clearer methods.
              - Keep public API small: `FindOptimalY` is the primary new function.
              - Ensure all arithmetic uses defensible defaults and avoids division by zero.

           4. Keep rest of file intact so program compiles and existing tests can run.

         End PSEUDOCODE
        */

     

       
  

      

        static void Main(string[] args)
        {           
           Tests tests = new Tests(); 
           tests.Test01();
        }
    }
}









