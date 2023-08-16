using System.IO.Enumeration;

namespace BattleSystemTest_Aug9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SimulationEngine simulationEngine = new SimulationEngine();
            simulationEngine.RegisterVehicle(new Aircraft(new List<float[]>{new float[] { 50.0f, 0.0f },
                                                                            new float[] { 35.0f, 0.0f },
                                                                            new float[] { 20.0f, 0.0f },
                                                                            new float[] { 5.0f, 0.0f } },
                                                                            new float[] { 1.0f, 1.0f, 1.0f }, 10.0f));
            simulationEngine.RegisterVehicle(new Radar(new List<float[]> { new float[] { 20.0f, 0.0f },},
                                                                           new float[] { 0.0f }, 10.0f));
            while (!simulationEngine.allVehiclesStopped)
            {
                simulationEngine.RunSimulationEngine(1.0f);
                Console.ReadLine();
            }
        }
    }


    abstract class BattleSystem
    {
        public abstract string Type { get; set; }
        public abstract float[] CurrentPosition { get; set; }
        public abstract float[] NewPositionTemp { get; set; }
        public abstract float[] LegVelocities { get; set; }
        public abstract List<float[]> VehiclePath { get; set; }
        public abstract List<float[]> Velocities { get; set; }
        public abstract bool VehicleHasStopped { get; set; }
        public abstract int InLeg { get; set; }
        public abstract int VehicleID { get; set; }
        public abstract float RadarRange { get; set; }
        public abstract float[] Get();
        public abstract void Set();
        public abstract void OnTick(float duration);
    }

    class Aircraft : BattleSystem
    {
        public override string Type { get; set; }
        public override float[] CurrentPosition { get; set; }
        public override float[] NewPositionTemp { get; set; }
        public override float[] LegVelocities { get; set; }
        public override List<float[]> Velocities { get; set; }
        public override List<float[]> VehiclePath { get; set; }
        public override bool VehicleHasStopped { get; set; }
        public override int InLeg { get; set; }
        public override int VehicleID { get; set; }
        public override float RadarRange { get; set; }
        public override float[] Get()
        {
            return this.CurrentPosition;
        }
        public override void Set()
        {
            for (int i = 0; i < 2; i++)
            {
                this.CurrentPosition[i] = this.NewPositionTemp[i];
            }
        }
        public override void OnTick(float duration)
        {
            this.NewPositionTemp[0] = (float) Math.Round(this.CurrentPosition[0] + (this.Velocities[this.InLeg][0] * duration),4);
            this.NewPositionTemp[1] = (float) Math.Round(this.CurrentPosition[1] + (this.Velocities[this.InLeg][1] * duration),4);
        }

        public Aircraft(List<float[]> waypoints, float[] leg_velocities, float radarRange)
        {

            /* The Aircraft class takes a list of waypoints in form of array of length 2 as first argument
             * and and array of length (waypoints.Count - 1) representing velocities in each leg as second argument.
             * The third argument is the range of the internal radar system */


            this.VehiclePath = waypoints;
            this.LegVelocities = leg_velocities;
            this.Velocities = new List<float[]>();
            for (int i = 0; i < leg_velocities.Length; i++) // Decomposing leg velocities during instance construction
            {
                float[] vel = new float[2];
                float euclid_len;
                float x_len;
                float y_len;
                x_len = this.VehiclePath[i + 1][0] - this.VehiclePath[i][0]; // x2 - x1
                y_len = this.VehiclePath[i + 1][1] - this.VehiclePath[i][1]; // y2 - y1
                euclid_len = (float)Math.Sqrt((x_len * x_len) + (y_len * y_len)); // Euclidean distance formule
                vel[0] = this.LegVelocities[i] * (x_len / euclid_len); // x_vel = vel * cos(angle)
                vel[1] = this.LegVelocities[i] * (y_len / euclid_len); // y_vel = vel * sin(angle)
                this.Velocities.Add(vel);
            }
            this.CurrentPosition = waypoints[0];
            this.NewPositionTemp = waypoints[0];
            Type = "Aircraft";
            this.InLeg = 0;
            this.RadarRange = radarRange;
            BattleSOS.s_BattleSystemsCount++;
            this.VehicleID = BattleSOS.s_BattleSystemsCount;
        }
    }

    class Radar : BattleSystem
    {
        // Object of Radar class has the same properties as Aircraft but only one array of "waypoints" and zero velocity
        // (except in cases of mobile radar arrays)

        public override string Type { get; set; }
        public override float[] CurrentPosition { get; set; }
        public override float[] NewPositionTemp { get; set; }
        public override float[] LegVelocities { get; set; }
        public override List<float[]> Velocities { get; set; }
        public override List<float[]> VehiclePath { get; set; }
        public override bool VehicleHasStopped { get; set; }
        public override int InLeg { get; set; }
        public override int VehicleID { get; set; }
        public override float RadarRange { get; set; }
        public override float[] Get()
        {
            return this.CurrentPosition;
        }
        public override void Set()
        {
            for (int i = 0; i < 2; i++)
            {
                this.CurrentPosition[i] = this.NewPositionTemp[i];
            }
        }
        public override void OnTick(float duration)
        {
            this.NewPositionTemp[0] = (float)Math.Round(this.CurrentPosition[0] + (this.Velocities[this.InLeg][0] * duration), 4);
            this.NewPositionTemp[1] = (float)Math.Round(this.CurrentPosition[1] + (this.Velocities[this.InLeg][1] * duration), 4);
        }

        public Radar(List<float[]> coordinates, float[] leg_velocities, float radarRange)
        {

            /* The Radar class takes a List of array of coordinates of length 2 as first argument.
             * Only one element would be present in the list.
             * Second argument is leg_velocites, which would be an array of single float with value 0.0
             * since the radars would be fixed to their position.
             * Third argument is the Radar Range */


            this.VehiclePath = coordinates;
            this.LegVelocities = leg_velocities;
            this.Velocities = new List<float[]>();
            for (int i = 0; i < leg_velocities.Length; i++) // Decomposing leg velocities during instance construction
            {
                float[] vel = new float[2];
                vel[0] = 0.0f;
                vel[1] = 0.0f;
                this.Velocities.Add(vel);
            }
            this.CurrentPosition = coordinates[0];
            this.NewPositionTemp = coordinates[0];
            Type = "Radar";
            this.InLeg = 0;
            this.RadarRange = radarRange;
            BattleSOS.s_BattleSystemsCount++;
            this.VehicleID = BattleSOS.s_BattleSystemsCount;
        }
    }


    class BattleSOS
    {
        //
        public static int s_BattleSystemsCount = 0;
        public static List<BattleSystem> SystemsOnField;
    }
    class SimulationEngine
    {
        public int vehiclesStopped = 0;
        public bool allVehiclesStopped = false;
        public SimulationEngine()
        {
            BattleSOS.SystemsOnField = new List<BattleSystem>
            { new Aircraft(new List<float[]> {new float[] { 0.0f, 0.0f }, // Waypoints
                                              new float[] { 10.0f, 0.0f },
                                              new float[] { 25.0f, 0.0f },
                                              new float[] { 50.0f, 0.0f } },
                                              new float[] { 1.0f, 1.0f, 1.0f }, // Velocities
                                              10.0f) // Radar Range
            };
        }

        public void RegisterVehicle(BattleSystem vehicle)
        {
            BattleSOS.SystemsOnField.Add(vehicle);
        }

        public void RunSimulationEngine(float timer)
        {
            List<float[]> globalSituationAwareness = new List<float[]>();



            foreach (var vehicle in BattleSOS.SystemsOnField)
            {
                globalSituationAwareness.Add(vehicle.Get());
            }

            foreach (var vehicle in BattleSOS.SystemsOnField)
            {
                if (vehicle.Type != "Radar") // Excluding all Radar type objects from computatuion of new positions.
                {
                    foreach (var battlefield_vehicle in BattleSOS.SystemsOnField)
                    {
                        if (battlefield_vehicle.CurrentPosition[0] != vehicle.CurrentPosition[0]) // Excluding itself from detecting objects in range
                        {
                            if (((float)Math.Abs((battlefield_vehicle.CurrentPosition[0] - vehicle.CurrentPosition[0])) <= vehicle.RadarRange)
                                && ((float)Math.Abs((battlefield_vehicle.CurrentPosition[1] - vehicle.CurrentPosition[1])) <= vehicle.RadarRange))
                            {
                                // Check for other objects in BattleSOS.SystemsOnField that are within Radar Range of given object.

                                if (battlefield_vehicle.Type == "Radar")
                                {
                                    Console.WriteLine($"{battlefield_vehicle.Type} {battlefield_vehicle.VehicleID} in range of {vehicle.Type} {vehicle.VehicleID}");
                                }
                                else
                                {
                                    Console.WriteLine($"{battlefield_vehicle.Type} {battlefield_vehicle.VehicleID} Collision Warning with {vehicle.Type} {vehicle.VehicleID}");
                                }
                            }
                        }
                    }
                }
                vehicle.OnTick(timer);
            }

            foreach (var vehicle in BattleSOS.SystemsOnField)
            {
                if(vehicle.Type != "Radar") // Excluding all Radar type objects from Setting of new positions.
                {
                    for (int i = 0; i < vehicle.LegVelocities.Length; i++)
                    {
                        if (vehicle.VehiclePath[i][0] > vehicle.VehiclePath[i + 1][0])
                        {
                            if (vehicle.CurrentPosition[0] <= vehicle.VehiclePath[i][0])
                            {
                                vehicle.InLeg = i;
                            }
                            if ((vehicle.InLeg == vehicle.Velocities.Count - 1)
                                && vehicle.CurrentPosition[0] <= vehicle.VehiclePath[i + 1][0])
                            {
                                // Checks whether the Vehicle is in last leg and has stopped
                                vehicle.VehicleHasStopped = true;
                                vehiclesStopped++;
                            }
                        }
                        else
                        {
                            if (vehicle.CurrentPosition[0] >= vehicle.VehiclePath[i][0])
                            {
                                vehicle.InLeg = i;
                            }
                            if ((vehicle.InLeg == vehicle.Velocities.Count - 1)
                                && vehicle.CurrentPosition[0] >= vehicle.VehiclePath[i + 1][0])
                            {
                                // Checks whether the Vehicle is in last leg and has stopped
                                vehicle.VehicleHasStopped = true;
                                vehiclesStopped++;
                            }
                        }
                    }
                }
                
                if (!vehicle.VehicleHasStopped) // If Vehicle has stopped, stop calculating new values for position
                {
                    vehicle.Set();
                    Console.WriteLine($"({vehicle.CurrentPosition[0]},{vehicle.CurrentPosition[1]})");
                }
                else if (vehicle.VehicleHasStopped)
                {
                    Console.WriteLine("Vehicle reached end of path");
                }
                if (vehiclesStopped == BattleSOS.SystemsOnField.Count)
                {
                    this.allVehiclesStopped = true;
                }
            }
        }
    }
}
