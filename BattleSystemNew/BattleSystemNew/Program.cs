﻿using System.Dynamic;

for (int i = 0; i < 10; i++)
{
    SimulationEngine sim = new SimulationEngine(1.0f);
    Console.WriteLine($"Number of ticks elapsed:{i+1}\n");
    foreach (var x in BattleSOS.SystemsOnField)
    {
        Console.WriteLine($"Vehicle {x.VehicleID} ({x.Type}) Position: {string.Join(",", x.CurrentPosition)}");
    }
}


abstract class BattleSystem
{
    public abstract string Type { get; set; }
    public abstract float[] CurrentPosition { get; set; }
    public abstract float[] Velocities { get; set; }
    public abstract int VehicleID { get; set; }
}

class Aircraft : BattleSystem
{
    public override string Type { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] Velocities { get; set; }
    public override int VehicleID { get; set; }
    public Aircraft(float[] velocities, float[] init_position)
    {
        this.Velocities = velocities;
        this.CurrentPosition = init_position;
        Type = "Aircraft";
        BattleSOS.s_BattleSystemsCount++;
        this.VehicleID = BattleSOS.s_BattleSystemsCount;
        BattleSOS.SystemsOnField.Add(this);
    }
}

class Tank : BattleSystem
{
    public override string Type { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] Velocities { get; set; }
    public override int VehicleID { get; set; }
    public Tank(float[] velocities, float[] init_position)
    {
        this.Velocities = velocities;
        this.CurrentPosition = init_position;
        Type = "Tank";
        BattleSOS.s_BattleSystemsCount++;
        this.VehicleID = BattleSOS.s_BattleSystemsCount;
        BattleSOS.SystemsOnField.Add(this);
    }
}

class BattleSOS
{
    //
    public static int s_BattleSystemsCount = 0;
    public static List<BattleSystem> SystemsOnField = new List<BattleSystem>();
}
class SimulationEngine
{
    public SimulationEngine(float TickTimer)
    {
        BattleSOS.SystemsOnField = new List<BattleSystem>();
        BattleSOS.SystemsOnField.Add(new Aircraft(new float[] { 0.0f, 0.0f }, new float[] { 0.0f, 0.0f }));
        BattleSOS.SystemsOnField.Add(new Aircraft(new float[] { 0.0f, 0.0f }, new float[] { 0.0f, 0.0f }));
        BattleSOS.SystemsOnField.Add(new Tank(new float[] { 0.0f, 0.0f }, new float[] { 0.0f, 0.0f }));
        BattleSOS.SystemsOnField.Add(new Tank(new float[] { 0.0f, 0.0f }, new float[] { 0.0f, 0.0f }));


        Console.WriteLine($"\nTick Duration = {TickTimer} second(s)");
        foreach (var system in BattleSOS.SystemsOnField)
        {
            string globalSituationAwareness = String.Empty;
            globalSituationAwareness += system.Get().ToString();
        }

        foreach (var system in BattleSOS.SystemsOnField)
        {
            string sharedSituationAwareness = String.Empty;
            system.Set(sharedSituationAwareness);            
        }
        System.OnTick();

        //    float[] position = system.CurrentPosition;
        //    for (int i = 0; i < 2; i++)
        //    {
        //        system.CurrentPosition[i] += system.Velocities[i] * TickTimer;
        //    }
        //}
    }
}