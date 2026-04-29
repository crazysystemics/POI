using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace SimpleARM
{
     class Tests
    {
        MissionPlanner  planner = new MissionPlanner();
        int radar_x = 50, radar_y = 0, visibility_radius = 3;
        public  void Test01()
        {
            // Example usage of the new FindOptimalY method:
            int calculatedPY0 = planner.calcOptimalPY(
                                        radar_x, radar_y, 
                                        visibility_radius
                                              );
            int fistOptimalY0 = planner.FindFirstOptimalPY0(
                                                    py_min: 0, py_max: 100,
                                                    radar_x, radar_y
                                              );


            Debug.Assert(calculatedPY0 == fistOptimalY0, $"Test01 failed: expected {calculatedPY0}, got {fistOptimalY0}");
        }

    }
}
