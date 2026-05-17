using System;
using System.Diagnostics;

namespace SimpleARM
{
    class Test
    {
        public static void Test01()
        {
            //TBD-IMPORTANT
            //Abstractions are
            //Battlespace, Radar, Aircraft, MissionPlanner
            //Battlespace belongs to Defence and Aerospace domain
            //MissionPlanner belongs to Computation-AIML domain
            //Same topology is to be continued in further development.
            MissionPlanner planner = new();

            //Testing Optimal Height
            Radar radar = new(0, 0, 100);
            Aircraft aircraft = new(0.0, 50.0);
            double aymin = 50.0, aymax = 150.0;   //TBD: should convert to double. input dimensions. need to be  improved;
            int axmin = -100, axmax = 100; //TBD: input dimensions. need to be  improved;

            //Engineering Information
            int ay_num_samples = 10;
            int ax_num_samples = 201; // step=1 so ax=0 (radar.x) is always sampled

            //Debug Information - Calculate actual array size needed
            double ay_step = (aymax - aymin) / ay_num_samples;
            int actual_ay_iterations = (int)Math.Ceiling((aymax - aymin) / ay_step) + 1;
            
            int[] debug_detect_counts = new int[actual_ay_iterations];
            double[] debug_ays = new double[actual_ay_iterations];
            //======================================================================
           
            Battlespace bspace = new(radar, aircraft,axmin, axmax, aymin, aymax);
            bspace.OptimalAy = planner.find_optimal_y(bspace.AyMin,bspace.AyMax, radar);
            bspace.IsAyOptimal = planner.isoptimal_y(bspace.OptimalAy, aircraft, radar, 
                                                    (int)bspace.AxMin,(int) bspace.AxMax, ax_num_samples,
                                                    (int)bspace.AyMin, (int)    bspace.AyMax, ay_num_samples,
                                                    debug_ays,
                                                    debug_detect_counts);//TBD: need to convert to double and improve 

            Debug.Assert(debug_ays.Length == debug_detect_counts.Length);
            
            Console.WriteLine($"optimal_y = {bspace.OptimalAy}, y_is_optimal = {bspace.IsAyOptimal}");
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
