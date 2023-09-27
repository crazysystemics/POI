namespace RWRPOC
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