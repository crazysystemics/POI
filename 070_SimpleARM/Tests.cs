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
            //Testing Optimal Height
            //Run simulations with random radar positions within error band (across x) 
            //and calculate average detection count
            //Optimation Height Estimation (Training may be any method) this is Test
            int optimal_y = 0;

            Radar radar = new Radar(0, 0, 100);
            Aircraft aircraft = new Aircraft(0.0, 50.0);
            //configure_run_simualtions();
            optimal_y = Program.find_optimal_y(50, 150, radar);
            bool y_is_optimal = Program.isoptimal_y(optimal_y, aircraft, radar, -100, 100, 10);
            Debug.Assert( y_is_optimal, $"Optimal Y {optimal_y} is not optimal according to detection counts.");
            //validate whether optimal_y is indeed optimal by running multiple simulations with random radar positions
            //and calculating detection count for optimal_y and comparing it with detection count for other y values in the range
            //double detect_number =  validate_optimal_y( 10, optimal_y, 0, 100, 10, 90, 0, 0, 1, ref best_score);
            Console.WriteLine($"Optimal Y: {optimal_y}");


        }

    }
}
