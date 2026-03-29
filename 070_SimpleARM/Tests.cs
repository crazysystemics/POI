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
        public  void Test01()
        {
            // Example usage of the new FindOptimalY method:
            int calculatedPY0 = planner.calcOptimalPY(
                                                    min_y: 0, max_y: 100,
                                                    radar_x: 100, radar_y: 0
                                              );
            int optimalPY0 = planner.FindOptimalPY0(
                                                    py_min: 0, py_max: 100,
                                                    radar_x: 100, radar_y: 0
                                              );

            Debug.Assert(calculatedPY0 == optimalPY0, $"Test01 failed: expected {calculatedPY0}, got {optimalPY0}");
        }

    }
}
