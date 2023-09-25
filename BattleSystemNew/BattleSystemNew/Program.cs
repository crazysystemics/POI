/* Classes contained in this project:
 * 1. Simulation Model (abstract)
 *  - Derived classes:
 *    a. Physical Simulation Engine (PSE)
 *    b. Battle System (abstract)
 *    - Derived classes:
 *      i. Aircraft
 *      ii. Radar
 * 
 * 2. Situational Awareness
 * 3. Discrete Time Simulation Engine (DTSE)
 * 4. Object Register
 * 5. Globals
 * 6. Emitter (currently not implemented)
 * 
 * It also contains a Program class (in this file) that runs the main program.
 * The main program first registers Battle System objects using the Object Register class.
 * Object Register also assigns an identifier to each object registered with it.
 * 
 * The DTSE initializes a Simulation Model list and adds all objects registered with
 * Object Register to it. It also instantiates a PSE object, which in turn creates
 * a Sitautional Awareness class list. The DTSE then calls Get(), OnTick() and Set()
 * methods on every object in the Simulation Model list.
 */


namespace BattleSystem
{
    class Program
    {
        static void Main(string[] args)
        {

            ObjectRegister.registerObject(new Aircraft(new List<float[]>
                                                        {
                                                         // Waypoints

                                                         new float[] { 5.0f, 5.0f, 0.0f },
                                                         new float[] { 10.0f, 10.0f, 1.0f },
                                                         new float[] { 15.0f, 10.0f, 2.0f },
                                                         new float[] { 20.0f, 5.0f, 3.0f },
                                                         new float[] { 15.0f, 0.0f, 4.0f },
                                                         new float[] { 10.0f, 0.0f, 5.0f },
                                                         new float[] { 5.0f, 5.0f, 6.0f }, },
                                                         
                                                         // Range
                                                         7.5f));

            ObjectRegister.registerObject(new Radar(new List<float[]>
                                                        {new float[] { 15.0f, 10.0f, 0.0f }},
                                                          7.5f));

            ObjectRegister.registerObject(new Radar(new List<float[]>
                                                        {new float[] { 25.0f, 5.0f, 0.0f }},
                                                         7.5f));

            ObjectRegister.registerObject(new Radar(new List<float[]>
                                                        {new float[] { 15.0f, 0.0f, 0.0f }},
                                                         7.5f));

            DiscreteTimeSimulationEngine DTSE = new DiscreteTimeSimulationEngine();

            while (!DTSE.allVehiclesStopped)
            {
                DTSE.RunSimulationEngine();
                Console.WriteLine("\n----------------\n\nPress Enter/Return to display next tick");
                Console.ReadLine();
            }
        }
    }
}