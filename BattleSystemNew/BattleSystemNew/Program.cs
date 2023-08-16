using System.IO.Enumeration;

namespace BattleSystemTest_Aug9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SimulationEngine simulationEngine = new SimulationEngine();
            for (int i = 0; i < 100; i++)
            {
                simulationEngine.RunSimulationEngine();
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

        public Aircraft(List<float[]> waypoints, float[] leg_velocities)
        {

            /* The Aircraft class takes a list of waypoints in form of array of length 2 as first argument
             * and and array of length (waypoints.Count - 1) representing velocities in each leg as second argument */


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
        public SimulationEngine()
        {
            BattleSOS.SystemsOnField = new List<BattleSystem>
            { new Aircraft(new List<float[]> {new float[] { 0.0f, 0.0f },
                                              new float[] { 5.0f, 4.0f },
                                              new float[] { 15.0f, 4.0f },
                                              new float[] { 20.0f, 0.0f } }, new float[] { 1.0f, 2.0f, 1.5f }),
            }; // Argument 1 = List of floating point arrays of waypoints. Argument 2 = Floating point array of leg velocities
        }

        public void RegisterVehicle(BattleSystem vehicle)
        {
            BattleSOS.SystemsOnField.Add(vehicle);
        }

        public void RunSimulationEngine()
        {
            List<float[]> globalSituationAwareness = new List<float[]>();



            foreach (var vehicle in BattleSOS.SystemsOnField)
            {
                globalSituationAwareness.Add(vehicle.Get());
            }

            foreach (var vehicle in BattleSOS.SystemsOnField)
            {
                vehicle.OnTick(1.0f);
            }

            foreach (var vehicle in BattleSOS.SystemsOnField)
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
                        }
                    }
                }
                if (!vehicle.VehicleHasStopped) // If Vehicle has stopped, stop calculating new values for position
                {
                    vehicle.Set();
                    Console.WriteLine($"({vehicle.CurrentPosition[0]},{vehicle.CurrentPosition[1]})");
                }
            }
        }
    }
}
