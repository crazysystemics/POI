﻿using System.IO.Enumeration;

namespace BattleSystemTest_Aug9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SimulationEngine simulationEngine = new SimulationEngine();
            for (int i = 0; i < 20; i++)
            {
                simulationEngine.RunSimulationEngine();
            }
        }
    }


    abstract class BattleSystem
    {
        public abstract string Type { get; set; }
        public abstract float[] CurrentPosition { get; set; }
        public abstract List<float[]> Velocities { get; set; }
        public abstract float[] LegVelocities { get; set; }
        public abstract List<float[]> FlightPath { get; set; }
        public abstract float[] NewPositionTemp { get; set; }
        public abstract bool FlightHasStopped { get; set; }
        public abstract int InLeg { get; set; }
        public abstract float[] Get();
        public abstract void Set();
        public abstract void OnTick(float duration);
    }

    class Aircraft : BattleSystem
    {
        public override string Type { get; set; }
        public override float[] CurrentPosition { get; set; }
        public override List<float[]> Velocities { get; set; }
        public override float[] LegVelocities { get; set; }
        public override float[] NewPositionTemp { get; set; }
        public override List<float[]> FlightPath { get; set; }
        public override bool FlightHasStopped { get; set; }
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
            this.FlightPath = waypoints;
            this.LegVelocities = leg_velocities;
            this.Velocities = new List<float[]>();
            for (int i = 0; i < leg_velocities.Length; i++) // Decomposing leg velocities during instance construction
            {
                float[] vel = new float[2];
                float euclid_len;
                float x_len;
                float y_len;
                x_len = this.FlightPath[i + 1][0] - this.FlightPath[i][0];
                y_len = this.FlightPath[i + 1][1] - this.FlightPath[i][1];
                euclid_len = (float)Math.Sqrt((x_len * x_len) + (y_len * y_len));
                vel[0] = this.LegVelocities[i] * (x_len / euclid_len);
                vel[1] = this.LegVelocities[i] * (y_len / euclid_len);
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
                                              new float[] { 4.0f, 3.0f },
                                              new float[] { 16.0f, 3.0f },
                                              new float[] { 20.0f, 0.0f } }, new float[] { 1.0f, 2.0f, 1.5f }), };
        }
        public void RunSimulationEngine()
        {
            List<float[]> globalSituationAwareness = new List<float[]>();



            foreach (var system in BattleSOS.SystemsOnField)
            {
                globalSituationAwareness.Add(system.Get());
            }

            foreach (var system in BattleSOS.SystemsOnField)
            {
                system.OnTick(1.0f);
            }

            foreach (var system in BattleSOS.SystemsOnField)
            {
                for (int i = 0; i < system.LegVelocities.Length; i++)
                {
                    if (system.CurrentPosition[0] >= system.FlightPath[i][0])
                    {
                        system.InLeg = i;
                    }
                    if (system.CurrentPosition[1] < 0)
                    {
                        system.FlightHasStopped = true;
                    }
                }
                if (!system.FlightHasStopped)
                {
                    system.Set();
                    Console.WriteLine($"({system.CurrentPosition[0]},{system.CurrentPosition[1]})");
                }
            }
        }
    }
}
