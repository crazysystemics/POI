using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace SimpleARM
{
     class Test
    {
        //MissionPlanner  planner = new MissionPlanner();
        //int radar_x = 50, radar_y = 0, visibility_radius = 3;
        //public  void Test01()
        //{
        //    // Example usage of the new FindOptimalY method:
        //    int calculatedPY0 = planner.calcOptimalPY(
        //                                radar_x, radar_y, 
        //                                visibility_radius
        //                                      );
        //    int fistOptimalY0 = planner.FindFirstOptimalPY0(
        //                                            py_min: 0, py_max: 100,
        //                                            radar_x, radar_y
        //                                      );


        //    Debug.Assert(calculatedPY0 == fistOptimalY0, $"Test01 failed: expected {calculatedPY0}, got {fistOptimalY0}");
        //}
        public void Test01()
        {
            MissionPlanner planner = new MissionPlanner();

            //Testing Optimal Height
            Radar radar = new Radar(0, 0, 100);
            Aircraft aircraft = new Aircraft(0.0, 50.0);
            double minay = 50.0, maxay = 150.0;
            double optimal_y = 0;

            //Engineering Information
            int ay_num_samples = 10;
            int ax_num_samples = 10;

            //Debug Information - Calculate actual array size needed
            double ay_step = (maxay - minay) / ay_num_samples;
            int actual_ay_iterations = (int)Math.Ceiling((maxay - minay) / ay_step) + 1;
            
            int[] debug_detect_counts = new int[actual_ay_iterations];
            double[] debug_ays = new double[actual_ay_iterations];
           
            optimal_y = planner.find_optimal_y(minay, maxay, radar);
            bool y_is_optimal = planner.isoptimal_y(optimal_y, aircraft, radar, 
                                                    axmin:-100, axmax:100, ax_num_samples,
                                                    minay, maxay, ay_num_samples,
                                                    debug_ays,
                                                    debug_detect_counts);
            Debug.Assert(debug_ays.Length == debug_detect_counts.Length);
            
            if (SGlobal.debug)
            {
                for (int i = 0; i < debug_ays.Length; i++)
                {
                    Console.WriteLine($"ay[{i}] = {debug_ays[i]}, detect_count[{i}] = {debug_detect_counts[i]}");
                }
            }
        }

    }
}
