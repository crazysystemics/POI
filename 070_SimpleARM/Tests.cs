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
                                                    (int)bspace.AyMin,(int) bspace.AyMax, ay_num_samples,
                                                    debug_ays,
                                                    debug_detect_counts);//TBD: need to convert to double and improve 

            bspace.OptimalAyDetectCount = 
                planner.findDetectionCountForPY(bspace.OptimalAy, radar, 
                                                axmin, axmax, ax_num_samples);

            //Solution found is validated to be optimal
            Debug.Assert(bspace.IsAyOptimal);
            //Validate isoptimal_y over the ay range.
            int debug_ay_index = 0;
            foreach(double debug_ay in debug_ays)
            {
                if (debug_ay != bspace.OptimalAy)
                {
                    //(bspace.OptimalAy >= debug_ay) ==>
                    //                bspace.OptimalAyDetectCount < debug_detect_counts[debug_ay_index])
                    //by rule p=>q is equivalent to !p or q
                    Debug.Assert(bspace.OptimalAy <= debug_ay ||
                                 bspace.OptimalAyDetectCount <
                                        debug_detect_counts[debug_ay_index]);                             
                }
                debug_ay_index++;
            }
            Debug.Assert(debug_ays.Length == debug_detect_counts.Length);          


            Console.WriteLine($"optimal_y = {bspace.OptimalAy}," + 
                             $" y_is_optimal = {bspace.IsAyOptimal}, " +
                             $" optimal_detection_count = {bspace.OptimalAyDetectCount} "    );
            if (SGlobal.debug)
            {
                for (int i = 0; i < debug_ays.Length; i++)
                {
                    Console.WriteLine($"ay[{i}] = {debug_ays[i]}, detect_count[{i}] = {debug_detect_counts[i]}");
                }
            }
        }

        public static void Test02()
        {
            //APPLICATION-NOTES
            //Abstractions are
            //Layer: Application
            //Domain: Defence and Aerospace
            //Battlespace
            //BLUE, Aircraft, MissionPlanner
            //RED,  Radar
            //Battlefield a rectangular area defined by BfMinX, BfMaxX, BfMinY, BfMaxY
            //axmin, axmax, aymin, aymax where blue aircraft can fly
            //radar is at radar_x, radar_y and has a visibility
            //              radar_x - radar_range to radar_x + radar_range and
            //              radar_y - radar_range to radar_y + radar_range
            //Layer: Computation
            //           Mission(Mission is like Algorithm,Computation,as it is Transformation)
            //           Mission Plan (is like Algorithm Design)
            //           Use Methods in MissionPlanner class to find optimal y for blue aircraft
            //           so that detection count for it is minimized by Radar
            //Layer: Knowledge Base
            //           Knowledge is captured in MissionPlanner and Program methods
            //Additional Notes:
            //Battlespace belongs to Defence and Aerospace domain
            //MissionPlanner belongs to Computation-AIML domain
            //Same topology is to be continued in further development.
            // BLUE
            MissionPlanner planner = new();
            Aircraft aircraft = new(0.0, 50.0);

            // RED
            Radar radar = new(0, 0, 100);

            // Application Layer: Battlefield — full terrain, larger than flight zone
            double bfMinX = -200, bfMaxX = 200, bfMinY = -100, bfMaxY = 300;

            // Aircraft flight zone — sub-region of Battlefield where BLUE can fly
            double axmin = -100, axmax = 100, aymin = 50.0, aymax = 150.0;

            // Engineering parameters
            int ay_num_samples = 10;
            int ax_num_samples = 201; // step=1 so ax=0 (radar.x) is always sampled

            // Debug arrays
            double ay_step = (aymax - aymin) / ay_num_samples;
            int actual_ay_iterations = (int)Math.Ceiling((aymax - aymin) / ay_step) + 1;
            int[] debug_detect_counts = new int[actual_ay_iterations];
            double[] debug_ays = new double[actual_ay_iterations];
            //======================================================================

            // Computation Layer: MissionPlanner finds optimal y within aircraft flight zone
            Battlespace bspace = new(radar, aircraft, axmin, axmax, aymin, aymax);
            bspace.OptimalAy = planner.find_optimal_y(bspace.AyMin, bspace.AyMax, radar);
            bspace.IsAyOptimal = planner.isoptimal_y(bspace.OptimalAy, aircraft, radar,
                                                    (int)bspace.AxMin, (int)bspace.AxMax, ax_num_samples,
                                                    (int)bspace.AyMin, (int)bspace.AyMax, ay_num_samples,
                                                    debug_ays,
                                                    debug_detect_counts);
            bspace.OptimalAyDetectCount =
                planner.findDetectionCountForPY(bspace.OptimalAy, radar,
                                                (int)axmin, (int)axmax, ax_num_samples);

            // Validate: flight zone is within Battlefield
            Debug.Assert(axmin >= bfMinX && axmax <= bfMaxX && aymin >= bfMinY && aymax <= bfMaxY);
            // Validate: optimal y is within aircraft flight zone
            Debug.Assert(bspace.OptimalAy >= aymin && bspace.OptimalAy <= aymax);
            // Validate: optimal y is within Battlefield
            Debug.Assert(bspace.OptimalAy >= bfMinY && bspace.OptimalAy <= bfMaxY);
            // Validate: optimal y is actually optimal
            Debug.Assert(bspace.IsAyOptimal);

            // Validate isoptimal_y over the ay range
            int debug_ay_index = 0;
            foreach (double debug_ay in debug_ays)
            {
                if (debug_ay != bspace.OptimalAy)
                {
                    //(bspace.OptimalAy >= debug_ay) ==>
                    //                bspace.OptimalAyDetectCount < debug_detect_counts[debug_ay_index])
                    //by rule p=>q is equivalent to !p or q
                    Debug.Assert(bspace.OptimalAy <= debug_ay ||
                                 bspace.OptimalAyDetectCount <
                                        debug_detect_counts[debug_ay_index]);
                }
                debug_ay_index++;
            }
            Debug.Assert(debug_ays.Length == debug_detect_counts.Length);

            Console.WriteLine($"Battlefield: x=[{bfMinX},{bfMaxX}], y=[{bfMinY},{bfMaxY}]");
            Console.WriteLine($"Flight zone: x=[{axmin},{axmax}], y=[{aymin},{aymax}]");
            Console.WriteLine($"optimal_y={bspace.OptimalAy}, is_optimal={bspace.IsAyOptimal}, detect_count={bspace.OptimalAyDetectCount}");
            if (SGlobal.debug)
            {
                for (int i = 0; i < debug_ays.Length; i++)
                {
                    Console.WriteLine($"ay[{i}] = {debug_ays[i]}, detect_count[{i}] = {debug_detect_counts[i]}");
                }
            }
        }

        public static void Test03()
        {
            //Purpose: To verify Step 2 in Plan.txt
            //Abstractions are
            //Layer: Application
            //APPLICATION-NOTES
            //Domain: Defence and Aerospace
            //Battlespace
            //BLUE, Aircraft, MissionPlanner
            //RED,  Radar
            //Battlefield a rectangular area defined by BfMinX, BfMaxX, BfMinY, BfMaxY
            //axmin, axmax, aymin, aymax where blue aircraft can fly
            //radar is at radar_x, radar_y and has a visibility
            //              radar_x - radar_range to radar_x + radar_range and
            //              radar_y - radar_range to radar_y + radar_range
            //Layer: Computation
            //           Mission(Mission is like Algorithm,Computation,as it is Transformation)
            //           Mission Plan (is like Algorithm Design)
            //           Use Methods in MissionPlanner class to find optimal y for blue aircraft
            //           so that detection count for it is minimized by Radar
            //Layer: Knowledge Base
            //           Knowledge is captured in MissionPlanner and Program methods
            //Additional Notes and Constraints:
            //Use Mission Planner methods only to find optimal y
            //Battlespace belongs to Defence and Aerospace domain
            //MissionPlanner belongs to Computation-AIML domain
            //Algorithm (2D Rectangle Overlap between aircraft flight zone and radar visibility):
            //  Step 1. Compute extended radar visibility rectangle (x error + range, y range)
            //  Step 2. Compute 2D overlap rectangle with aircraft flight zone
            //  Step 3. Dispatch on overlap state:
            //          a. NO OVERLAP            → safe, no y check needed
            //          b. PARTIAL OVERLAP        → find y in non-overlapped region of aircraft flight
            //          c. AIRCRAFT ENGULFS RADAR → same as b (find optimal y outside radar y bounds)
            //          d. RADAR ENGULFS AIRCRAFT → no escape, choose min y
            //  Test03 demonstrates state (a) and state (c)
            //======================================================================================

            // BLUE
            MissionPlanner planner = new();
            Aircraft aircraft = new(0.0, 50.0);

            // RED: nominal radar position
            int radar_nominal_x = 0, radar_y = 0, radar_range = 100;
            int radar_x_max_error = 30; // radar.x can be anywhere in [−30, +30]

            // Step 1: extended radar visibility rectangle
            double radarVisXMin = radar_nominal_x - radar_x_max_error - radar_range; // = -130
            double radarVisXMax = radar_nominal_x + radar_x_max_error + radar_range; // = +130
            double radarVisYMin = radar_y - radar_range;                              // = -100
            double radarVisYMax = radar_y + radar_range;                              // = +100

            // Battlefield (Application Layer)
            double bfMinX = -300, bfMaxX = 300, bfMinY = -100, bfMaxY = 300;

            // ---------------------------------------------------------------
            // Scenario A: state (a) NO OVERLAP
            // Aircraft flies entirely above radar y visibility — no 2D overlap
            // ---------------------------------------------------------------
            double axminA = -200, axmaxA = 200;
            double ayminA = 110,  aymaxA = 200; // above radar vis y [-100, 100]

            // Step 2: overlap rectangle
            double ovXMinA = Math.Max(axminA, radarVisXMin);
            double ovXMaxA = Math.Min(axmaxA, radarVisXMax);
            double ovYMinA = Math.Max(ayminA, radarVisYMin);
            double ovYMaxA = Math.Min(aymaxA, radarVisYMax);

            // Step 3a: no y overlap (ovYMaxA=100 < ovYMinA=110) → no 2D overlap → safe
            bool noOverlapA = ovXMaxA < ovXMinA || ovYMaxA < ovYMinA;
            Debug.Assert(noOverlapA, "Scenario A: expected no 2D overlap");
            Console.WriteLine("Scenario A (NO OVERLAP): safe — no y optimization needed");
            Console.WriteLine($"  Radar vis: x=[{radarVisXMin},{radarVisXMax}], y=[{radarVisYMin},{radarVisYMax}]");
            Console.WriteLine($"  Aircraft:  x=[{axminA},{axmaxA}], y=[{ayminA},{aymaxA}] — fully outside");

            // ---------------------------------------------------------------
            // Scenario B: state (c) AIRCRAFT ENGULFS RADAR
            // Aircraft x-range engulfs extended radar x visibility;
            // y overlaps partially → find optimal y outside radar y bounds (Test02 approach)
            // ---------------------------------------------------------------
            double axminB = -200, axmaxB = 200;
            double ayminB = 50,   aymaxB = 150;

            // Step 2: overlap rectangle
            double ovXMinB = Math.Max(axminB, radarVisXMin);
            double ovXMaxB = Math.Min(axmaxB, radarVisXMax);
            double ovYMinB = Math.Max(ayminB, radarVisYMin);
            double ovYMaxB = Math.Min(aymaxB, radarVisYMax);

            // Step 3c: overlap exists and aircraft engulfs radar in x
            bool overlapExistsB = ovXMaxB >= ovXMinB && ovYMaxB >= ovYMinB;
            bool xEngulfB       = axminB <= radarVisXMin && axmaxB >= radarVisXMax;
            Debug.Assert(overlapExistsB && xEngulfB, "Scenario B: expected overlap with aircraft x-engulf");
            Console.WriteLine("\nScenario B (AIRCRAFT ENGULFS RADAR in x): find optimal y (Test02 approach)");
            Console.WriteLine($"  Aircraft:  x=[{axminB},{axmaxB}], y=[{ayminB},{aymaxB}]");
            Console.WriteLine($"  Overlap:   x=[{ovXMinB},{ovXMaxB}], y=[{ovYMinB},{ovYMaxB}]");

            Radar radar = new(radar_nominal_x, radar_y, radar_range);
            Battlespace bspace = new(radar, aircraft, axminB, axmaxB, ayminB, aymaxB);

            // Validate: flight zone within Battlefield
            Debug.Assert(axminB >= bfMinX && axmaxB <= bfMaxX && ayminB >= bfMinY && aymaxB <= bfMaxY);

            // Computation Layer: find optimal y
            bspace.OptimalAy = planner.find_optimal_y(bspace.AyMin, bspace.AyMax, radar);

            // Validate: optimal y within flight zone and Battlefield
            Debug.Assert(bspace.OptimalAy >= ayminB && bspace.OptimalAy <= aymaxB);
            Debug.Assert(bspace.OptimalAy >= bfMinY && bspace.OptimalAy <= bfMaxY);

            int ax_num_samples = 401;
            int ay_num_samples = 10;
            double ay_step = (aymaxB - ayminB) / ay_num_samples;
            int actual_ay_iterations = (int)Math.Ceiling((aymaxB - ayminB) / ay_step) + 1;
            int[] debug_detect_counts = new int[actual_ay_iterations];
            double[] debug_ays = new double[actual_ay_iterations];

            bspace.IsAyOptimal = planner.isoptimal_y(bspace.OptimalAy, aircraft, radar,
                                                     (int)bspace.AxMin, (int)bspace.AxMax, ax_num_samples,
                                                     (int)bspace.AyMin, (int)bspace.AyMax, ay_num_samples,
                                                     debug_ays, debug_detect_counts);
            bspace.OptimalAyDetectCount = planner.findDetectionCountForPY(bspace.OptimalAy, radar,
                                                                           (int)axminB, (int)axmaxB, ax_num_samples);
            Debug.Assert(bspace.IsAyOptimal);

            Console.WriteLine($"  optimal_y={bspace.OptimalAy}, is_optimal={bspace.IsAyOptimal}, detect_count={bspace.OptimalAyDetectCount}");
            if (SGlobal.debug)
            {
                for (int i = 0; i < debug_ays.Length; i++)
                    Console.WriteLine($"  ay[{i}]={debug_ays[i]}, detect_count[{i}]={debug_detect_counts[i]}");
            }
        }

        public static void Test04()
        {
            //Purpose: To verify Step 3  in Plan.txt, errror in both x and y dimensions 
            //Abstractions are
            //Layer: Application
            //APPLICATION-NOTES
            //Domain: Defence and Aerospace
            //Battlespace
            //BLUE, Aircraft, MissionPlanner
            //RED,  Radar
            //Battlefield a rectangular area defined by BfMinX, BfMaxX, BfMinY, BfMaxY
            //axmin, axmax, aymin, aymax where blue aircraft can fly
            //radar is at radar_x, radar_y and has a visibility
            //              radar_x - radar_range to radar_x + radar_range and
            //              radar_y - radar_range to radar_y + radar_range
            //Layer: Computation
            //           Mission(Mission is like Algorithm,Computation,as it is Transformation)
            //           Mission Plan (is like Algorithm Design)
            //           Use Methods in MissionPlanner class to find optimal y for blue aircraft
            //           so that detection count for it is minimized by Radar
            //Layer: Knowledge Base
            //           Knowledge is captured in MissionPlanner and Program methods
            //Additional Notes and Constraints:
            //Use Mission Planner methods only to find optimal y
            //Battlespace belongs to Defence and Aerospace domain
            //MissionPlanner belongs to Computation-AIML domain
            //Algorithm (2D Rectangle Overlap between aircraft flight zone and radar visibility):
            //  Step 1. Compute extended radar visibility rectangle (x error + range, y errorange)
            //  Step 2. Compute 2D overlap rectangle with aircraft flight zone
            //  Step 3. Dispatch on overlap state:
            //          a. NO OVERLAP            → safe, no y check needed
            //          b. PARTIAL OVERLAP        → find y in non-overlapped region of aircraft flight
            //          c. AIRCRAFT ENGULFS RADAR → same as b (find optimal y outside radar y bounds)
            //          d. RADAR ENGULFS AIRCRAFT → no escape, choose min y
            //  Test03 demonstrates state (a) and state (c)
            //======================================================================================

            // BLUE
            MissionPlanner planner = new();
            Aircraft aircraft = new(0.0, 50.0);

            // RED: nominal radar position — error in BOTH x and y (Plan step 3)
            int radar_nominal_x = 0, radar_y = 0, radar_range = 100;
            int radar_x_max_error = 30; // radar.x can vary by ±30
            int radar_y_max_error = 20; // radar.y can vary by ±20  (new in Test04)

            // Step 1: extended radar visibility rectangle — x error + range, y error + range
            double radarVisXMin = radar_nominal_x - radar_x_max_error - radar_range; // = -130
            double radarVisXMax = radar_nominal_x + radar_x_max_error + radar_range; // = +130
            double radarVisYMin = radar_y - radar_y_max_error - radar_range;         // = -120 (wider than Test03)
            double radarVisYMax = radar_y + radar_y_max_error + radar_range;         // = +120 (wider than Test03)

            // Battlefield (Application Layer)
            double bfMinX = -300, bfMaxX = 300, bfMinY = -150, bfMaxY = 300;

            // ---------------------------------------------------------------
            // Scenario A: state (a) NO OVERLAP
            // Aircraft flies above extended radar y visibility (including y error)
            // aymin=130 > radarVisYMax=120 → no y overlap → safe
            // (Test03 used aymin=110 which now overlaps since y error raised vis to 120)
            // ---------------------------------------------------------------
            double axminA = -200, axmaxA = 200;
            double ayminA = 130, aymaxA = 200; // above radarVisYMax=120

            // Step 2: overlap rectangle
            double ovXMinA = Math.Max(axminA, radarVisXMin);
            double ovXMaxA = Math.Min(axmaxA, radarVisXMax);
            double ovYMinA = Math.Max(ayminA, radarVisYMin);
            double ovYMaxA = Math.Min(aymaxA, radarVisYMax);

            // Step 3a: no y overlap (ovYMaxA=120 < ovYMinA=130) → no 2D overlap → safe
            bool noOverlapA = ovXMaxA < ovXMinA || ovYMaxA < ovYMinA;
            Debug.Assert(noOverlapA, "Scenario A: expected no 2D overlap");
            Console.WriteLine("Scenario A (NO OVERLAP, x+y errors): safe — no y optimization needed");
            Console.WriteLine($"  Radar vis (incl. errors): x=[{radarVisXMin},{radarVisXMax}], y=[{radarVisYMin},{radarVisYMax}]");
            Console.WriteLine($"  Aircraft: x=[{axminA},{axmaxA}], y=[{ayminA},{aymaxA}] — fully outside");

            // ---------------------------------------------------------------
            // Scenario B: state (c) AIRCRAFT ENGULFS RADAR (both x and y errors accounted for)
            // Aircraft x engulfs extended radar x; y overlaps partially
            // Use effectiveRadar with range expanded by y_max_error for find_optimal_y
            // → optimal_y is higher than Test03 because y error widens the avoidance zone
            // ---------------------------------------------------------------
            double axminB = -200, axmaxB = 200;
            double ayminB = 50,   aymaxB = 200;

            // Step 2: overlap rectangle
            double ovXMinB = Math.Max(axminB, radarVisXMin);
            double ovXMaxB = Math.Min(axmaxB, radarVisXMax);
            double ovYMinB = Math.Max(ayminB, radarVisYMin);
            double ovYMaxB = Math.Min(aymaxB, radarVisYMax);

            // Step 3c: overlap exists and aircraft engulfs radar in x
            bool overlapExistsB = ovXMaxB >= ovXMinB && ovYMaxB >= ovYMinB;
            bool xEngulfB       = axminB <= radarVisXMin && axmaxB >= radarVisXMax;
            Debug.Assert(overlapExistsB && xEngulfB, "Scenario B: expected overlap with aircraft x-engulf");
            Console.WriteLine("\nScenario B (AIRCRAFT ENGULFS RADAR, x+y errors): find optimal y");
            Console.WriteLine($"  Aircraft: x=[{axminB},{axmaxB}], y=[{ayminB},{aymaxB}]");
            Console.WriteLine($"  Overlap:  x=[{ovXMinB},{ovXMaxB}], y=[{ovYMinB},{ovYMaxB}]");

            // Effective radar: expand range by y_max_error to absorb y positional uncertainty
            Radar effectiveRadar = new(radar_nominal_x, radar_y, radar_range + radar_y_max_error); // range=120
            Battlespace bspace = new(effectiveRadar, aircraft, axminB, axmaxB, ayminB, aymaxB);

            // Validate: flight zone within Battlefield
            Debug.Assert(axminB >= bfMinX && axmaxB <= bfMaxX && ayminB >= bfMinY && aymaxB <= bfMaxY);

            // Computation Layer: find optimal y using effective radar (accounts for y error)
            bspace.OptimalAy = planner.find_optimal_y(bspace.AyMin, bspace.AyMax, effectiveRadar);

            // Validate: optimal y clears the extended radar y visibility
            Debug.Assert(bspace.OptimalAy > radarVisYMax,
                $"Scenario B: optimal_y={bspace.OptimalAy} must exceed extended radar vis y max={radarVisYMax}");
            // Validate: optimal y within flight zone and Battlefield
            Debug.Assert(bspace.OptimalAy >= ayminB && bspace.OptimalAy <= aymaxB);
            Debug.Assert(bspace.OptimalAy >= bfMinY && bspace.OptimalAy <= bfMaxY);

            int ax_num_samples = 401;
            int ay_num_samples = 10;
            double ay_step = (aymaxB - ayminB) / ay_num_samples;
            int actual_ay_iterations = (int)Math.Ceiling((aymaxB - ayminB) / ay_step) + 1;
            int[] debug_detect_counts = new int[actual_ay_iterations];
            double[] debug_ays = new double[actual_ay_iterations];

            bspace.IsAyOptimal = planner.isoptimal_y(bspace.OptimalAy, aircraft, effectiveRadar,
                                                     (int)bspace.AxMin, (int)bspace.AxMax, ax_num_samples,
                                                     (int)bspace.AyMin, (int)bspace.AyMax, ay_num_samples,
                                                     debug_ays, debug_detect_counts);
            bspace.OptimalAyDetectCount = planner.findDetectionCountForPY(bspace.OptimalAy, effectiveRadar,
                                                                           (int)axminB, (int)axmaxB, ax_num_samples);
            Debug.Assert(bspace.IsAyOptimal);

            Console.WriteLine($"  Battlefield: x=[{bfMinX},{bfMaxX}], y=[{bfMinY},{bfMaxY}]");
            Console.WriteLine($"  Flight zone: x=[{axminB},{axmaxB}], y=[{ayminB},{aymaxB}]");
            Console.WriteLine($"  optimal_y={bspace.OptimalAy} (Test03 would give 101; y error raises it to {bspace.OptimalAy})");
            Console.WriteLine($"  is_optimal={bspace.IsAyOptimal}, detect_count={bspace.OptimalAyDetectCount}");
            if (SGlobal.debug)
            {
                for (int i = 0; i < debug_ays.Length; i++)
                    Console.WriteLine($"  ay[{i}]={debug_ays[i]}, detect_count[{i}]={debug_detect_counts[i]}");
            }
        }

        public static void Test05()
        {
            //Purpose: Radar Position Error is Probabilistic — Monte Carlo Validation
            //Plan.txt: Step 4
            //
            //Abstractions
            //Layer: Application
            //APPLICATION-NOTES
            //Domain: Defence and Aerospace
            //Battlespace: BLUE (Aircraft, MissionPlanner) vs RED (Radar)
            //Battlefield: BfMinX, BfMaxX, BfMinY, BfMaxY
            //axmin, axmax, aymin, aymax: BLUE aircraft flight zone
            //
            //Radar position error distributions:
            //    radar_x_error {SYMMETRIC, DistributionType.UNIFORM}
            //    radar_y_error {SYMMETRIC, DistributionType.GAUSSIAN}
            //
            //Layer: Computation — Monte Carlo approach:
            //    1. Draw N samples of (radar_x, radar_y) from their distributions
            //    2. Classify each sample by 2D overlap state
            //    3. Compute effectiveRadar absorbing Gaussian y uncertainty with k*sigma
            //    4. Find optimal_y via find_optimal_y(effectiveRadar)
            //    5. Validate statistically: assert actual_pass_rate >= required_pass_rate
            //
            //Algorithm (2D Rectangle Overlap between aircraft flight zone and radar visibility):
            //    a. NO OVERLAP             -> safe, no y optimization needed
            //    b. PARTIAL OVERLAP        -> find y in non-overlapped region
            //    c. AIRCRAFT ENGULFS RADAR -> same as b (find optimal y outside radar y bounds)
            //    d. RADAR ENGULFS AIRCRAFT -> no escape, choose min y
            //
            //Aircraft x=[-120,120] with Uniform x in [-30,30]:
            //    aircraft engulfs vis x when |rx| <= 20  (~66% of samples)
            //    partial overlap in x    when |rx| >  20  (~33% of samples)
            //======================================================================================

            MissionPlanner planner = new();
            Aircraft aircraft = new(0.0, 50.0);

            // RED: probabilistic radar position
            int radar_range = 100;

            // Radar x: SYMMETRIC, UNIFORM in [radar_x_uniform_min, radar_x_uniform_max]
            DistributionType radar_x_distribution = DistributionType.UNIFORM;
            int radar_x_uniform_min = -30, radar_x_uniform_max = +30;

            // Radar y: SYMMETRIC, GAUSSIAN(mean=0, sigma=7)
            DistributionType radar_y_distribution = DistributionType.GAUSSIAN;
            int radar_y_mean = 0;
            double radar_y_sigma = 7.0;

            // Battlefield (Application Layer)
            double bfMinX = -300, bfMaxX = 300, bfMinY = -150, bfMaxY = 300;

            // Aircraft flight zone — narrower x than Test03/04 to produce mixed overlap states
            double axmin = -120, axmax = 120;
            double aymin = 50.0, aymax = 200.0;

            // ---------------------------------------------------------------
            // Monte Carlo: draw N=200 samples and classify overlap state per sample
            // ---------------------------------------------------------------
            int N = 200;
            Random rng = new Random(42); // fixed seed for reproducibility
            Radar[] sampleRadars = new Radar[N];

            int cntNoOverlap = 0, cntPartial = 0, cntAircraftEngulfs = 0, cntRadarEngulfs = 0;

            for (int i = 0; i < N; i++)
            {
                // Uniform x sample in [radar_x_uniform_min, radar_x_uniform_max]
                double rx = radar_x_uniform_min + rng.NextDouble() * (radar_x_uniform_max - radar_x_uniform_min);

                // Gaussian y sample via Box-Muller (1.0 - NextDouble() ensures Log argument > 0)
                double u1 = 1.0 - rng.NextDouble(), u2 = 1.0 - rng.NextDouble();
                double ry = radar_y_mean + radar_y_sigma * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

                sampleRadars[i] = new Radar((int)Math.Round(rx), (int)Math.Round(ry), radar_range);

                // Classify 2D overlap state for this sample
                double visXMin = rx - radar_range, visXMax = rx + radar_range;
                double visYMin = ry - radar_range, visYMax = ry + radar_range;
                double ovXMin = Math.Max(axmin, visXMin), ovXMax = Math.Min(axmax, visXMax);
                double ovYMin = Math.Max(aymin, visYMin), ovYMax = Math.Min(aymax, visYMax);

                if (ovXMax < ovXMin || ovYMax < ovYMin)
                {
                    cntNoOverlap++;
                }
                else
                {
                    bool visEngulfsAircraftX = visXMin <= axmin && visXMax >= axmax;
                    bool visEngulfsAircraftY = visYMin <= aymin && visYMax >= aymax;
                    bool aircraftEngulfsVisX = axmin <= visXMin && axmax >= visXMax;

                    if (visEngulfsAircraftX && visEngulfsAircraftY)
                        cntRadarEngulfs++;
                    else if (aircraftEngulfsVisX)
                        cntAircraftEngulfs++;
                    else
                        cntPartial++;
                }
            }

            Console.WriteLine($"[Test05] {N} Monte Carlo samples:");
            Console.WriteLine($"  x~{radar_x_distribution}({radar_x_uniform_min},{radar_x_uniform_max}), y~{radar_y_distribution}(mean={radar_y_mean},σ={radar_y_sigma})");
            Console.WriteLine($"  States: NO_OVERLAP={cntNoOverlap}, AIRCRAFT_ENGULFS={cntAircraftEngulfs}, PARTIAL={cntPartial}, RADAR_ENGULFS={cntRadarEngulfs}");

            // ---------------------------------------------------------------
            // Effective radar: absorb Gaussian y uncertainty with k=2σ
            // 2σ gives ~97.7% one-sided coverage → expected pass rate ≈ 98.4%
            // P(ry < effectiveRange - radar_range) = P(N(0,1) < (2*7)/7) = P(Z < 2) ≈ 97.7%
            // but pass needs ry+100 < optimal_y=115 → ry < 15 = 2.14σ → P ≈ 98.4%
            // ---------------------------------------------------------------
            double k_sigma = 2.0;
            int effectiveRange = (int)Math.Ceiling(radar_range + k_sigma * radar_y_sigma); // = 114

            Radar effectiveRadar = new Radar(0, radar_y_mean, effectiveRange);
            Battlespace bspace = new(effectiveRadar, aircraft, axmin, axmax, aymin, aymax);
            bspace.OptimalAy = planner.find_optimal_y(aymin, aymax, effectiveRadar);

            Debug.Assert(axmin >= bfMinX && axmax <= bfMaxX && aymin >= bfMinY && aymax <= bfMaxY,
                "Flight zone must lie within Battlefield");
            Debug.Assert(bspace.OptimalAy >= aymin && bspace.OptimalAy <= aymax);
            Debug.Assert(bspace.OptimalAy >= bfMinY && bspace.OptimalAy <= bfMaxY);

            Console.WriteLine($"\n  effectiveRange={effectiveRange} (nominal {radar_range} + {k_sigma}*sigma={k_sigma * radar_y_sigma})");
            Console.WriteLine($"  optimal_y={bspace.OptimalAy}  (theoretical pass rate ≈ 98.4%)");

            // ---------------------------------------------------------------
            // Statistical validation: assert >= 95% of samples have optimal_y as optimal
            // ---------------------------------------------------------------
            int ax_num_samples = 241;
            int ay_num_samples = 15;
            double required_pass_rate = 0.95;

            var (isStatOptimal, actualPassRate) = planner.isoptimal_y_statistical(
                bspace.OptimalAy, sampleRadars,
                (int)axmin, (int)axmax, ax_num_samples,
                aymin, aymax, ay_num_samples,
                required_pass_rate);

            bspace.IsAyOptimal = isStatOptimal;
            Console.WriteLine($"  Statistical validation: actual_pass_rate={actualPassRate:P1}, threshold={required_pass_rate:P0}, is_optimal={bspace.IsAyOptimal}");
            Debug.Assert(bspace.IsAyOptimal,
                $"optimal_y={bspace.OptimalAy} must be optimal for >= {required_pass_rate:P0} of {N} samples");
        }

    }
}
