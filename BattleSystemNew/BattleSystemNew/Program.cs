/* This program contains an Abstract class called BattleSystemClass, from which other classes like
 * Aircraft and Radar inherit.
 * 
 * The static class ObjectRegister starts with registering objects to a List that will be used to initialize
 * the DiscreteTimeSimulationEngine.
 * 
 * The DiscreteTimeSimulationengine calls the Get(), OnTick() and Set() methods on a PhysicalSimulationEngine.
 * 
 * The PhysicalSimulationEngine is initialized with an empty list of BattleSystemClass objects. The Get() method
 * of this class copies the situationalAwareness list registered to the DTSE in order to perform computations
 * and subsequent manipulations. This is copied into a new list called physicalSituationalAwareness.
 * 
 * The OnTick() method of this class iterates through the physicalSituationalAwareness List and performs relevant
 * computations (currently only computes new positions for Aircraft objects). It also displays current positions
 * and velocities of all the objects in the list and also performs a check for any objects visible
 * to a radar or an Aircraft RWR (currently a part of the Aircraft object) and displays its distance and azimuth.
 * 
 * The Set() method of this class performs a distance check between objects in physicalSituationalAwareness and
 * adds objects to the ObjectsVisible property if it is within the given range. This method also sets new values
 * for position (and other attributes/properties) that were computed in the OnTick() method. The new values are applied
 * to the objects in the original situationalAwareness list maintained by the DTSE. It also updates the ObjectsVisible
 * list in the objects of situationalAwarness.
 *  */


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