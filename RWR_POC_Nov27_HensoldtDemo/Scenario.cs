using RWR_POC_GUI;
using System;
using System.Collections.Generic;

public class Scenario
{
    public List<SimpleRadar> radars = new List<SimpleRadar>();
    public int radarNumber;
    public Random radarRandomizer;

    public Aircraft chosenAircraft;
    public List<Aircraft> aircrafts;
    public List<RWR> rwr = new List<RWR>();
    public int chooseAircraft;

    public Scenario(int radarNumber)
    {
        for (int i = 0;  i < radarNumber; i++)
        {
            Position radarPos = new Position(Globals.randomNumberGenerator.Next(40, 180), Globals.randomNumberGenerator.Next(10, 90));
            radars.Add(new SimpleRadar(radarPos, Globals.randomNumberGenerator.Next(10, 30), 15, Globals.Tick, i + 10));
        }

        aircrafts = new List<Aircraft>()
        {
            new Aircraft(new List<Position>()
            {
                new Position(50, 50),
                new Position(85, 85),
                new Position(135, 85),
                new Position(170, 50),
                new Position(135, 15),
                new Position(85, 15),
                new Position(50, 50)
            }, 0),

            new Aircraft(new List<Position>()
            {
                new Position(85, 15),
                new Position(85, 85),
                new Position(135, 85),
                new Position(135, 15),
                new Position(85, 15)
            }, 0),

            new Aircraft(new List<Position>()
            {
                new Position(50, 50),
                new Position(170, 50)
            }, 0)
        };

        chosenAircraft = aircrafts[Globals.randomNumberGenerator.Next(0, aircrafts.Count)];
        chosenAircraft.rwr = new RWR(ref chosenAircraft.position, 1);
    }
}