using RWR_POC_GUI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class RadarDataset
{
    public Dictionary<int, List<SimpleRadar>> radarSets = new Dictionary<int, List<SimpleRadar>>();
    public RadarDataset()
    {
        List<SimpleRadar> set1 = new List<SimpleRadar>()
        {
            new SimpleRadar(new Position(110, 50), 40, 15, Globals.Tick, 2)
        };
        List<SimpleRadar> set2 = new List<SimpleRadar>()
        {
            new SimpleRadar(new Position(130, 30), 40, 15, Globals.Tick, 1)
        };
        List<SimpleRadar> set3 = new List<SimpleRadar>()
        {
            new SimpleRadar(new Position(110, 50), 50, 15, Globals.Tick, 1),
            new SimpleRadar(new Position(110, 90), 20, 15, Globals.Tick, 2),
            new SimpleRadar(new Position(110, 10), 20, 15, Globals.Tick, 3)
        };
        List<SimpleRadar> set4 = new List<SimpleRadar>()
        {
            new SimpleRadar(new Position(80, 90), 20, 15, Globals.Tick, 1),
            new SimpleRadar(new Position(140, 90), 20, 15, Globals.Tick, 2),
            new SimpleRadar(new Position(140, 10), 20, 15, Globals.Tick, 3),
            new SimpleRadar(new Position(80, 10), 20, 15, Globals.Tick, 4)
        };

        this.radarSets.Add(0, set1);
        this.radarSets.Add(1, set2);
        this.radarSets.Add(2, set3);
        this.radarSets.Add(3, set4);
    }
}