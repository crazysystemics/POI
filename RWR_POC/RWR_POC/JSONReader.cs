using System.Text.Json;

public class JSONReader
{

    public class AircraftListJSON
    {
        public List<AircraftJSON> aircraftJSONs { get; set; } = new List<AircraftJSON>();
    }

    public class RadarListJSON
    {
        public List<RadarJSON> radarJSONs { get; set; } = new List<RadarJSON>();
    }
    public class AircraftJSON
    {
        public int id { get; set; }
        public List<Position> waypoints { get; set; }
    }

    public class RadarJSON
    {
        public int id { get; set; }
        public PulseJSON initPulse { get; set; }
        public Position position { get; set; }
        public int pulseRepetitionInterval { get; set; }
        public int txTick { get; set; }
        public int radius { get; set; }
    }

    public class PulseJSON
    {
        public int pulseWidth { get; set; }
        public int amplitude { get; set; }
        public int frequency { get; set; }
        public int timeOfTraversal { get; set; }
        public int angleOfTraversal { get; set; }
    }

    public AircraftJSON ParseSingleAircraft(string filename)
    {
        string fileName = filename;
        string jsonString = File.ReadAllText(fileName);

        AircraftJSON aircraftJSON = JsonSerializer.Deserialize<AircraftJSON>(jsonString);
        return aircraftJSON;
    }

    public RadarJSON ParseSingleRadar(string filename)
    {
        string fileName = filename;
        string jsonString = File.ReadAllText(fileName);

        RadarJSON radarJSON = JsonSerializer.Deserialize<RadarJSON>(jsonString);
        return radarJSON;
    }

    public AircraftListJSON ParseAircraftList(string filename)
    {
        string fileName = filename;
        string jsonString = File.ReadAllText(fileName);
        AircraftListJSON aircraftListJSON = JsonSerializer.Deserialize<AircraftListJSON>(jsonString);
        return aircraftListJSON;
    }

    public RadarListJSON ParseRadarList(string filename)
    {
        string fileName = filename;
        string jsonString = File.ReadAllText(fileName);
        RadarListJSON radarListJSON = JsonSerializer.Deserialize<RadarListJSON>(jsonString);
        return radarListJSON;
    }

    public void ReadJSON(string filename, string type)
    {
        string fileName = filename;
        string jsonString = File.ReadAllText(fileName);

        if (type == "aircraft")
        {
            AircraftJSON aircraftJSON = JsonSerializer.Deserialize<AircraftJSON>(jsonString);
            Console.WriteLine($"ID: {aircraftJSON.id}\nWaypoints:\n");
            for (int i = 0; i < aircraftJSON.waypoints.Count; i++)
            {
                Console.WriteLine($"\t({aircraftJSON.waypoints[i].x}, {aircraftJSON.waypoints[i].y})");
            }
            Console.WriteLine();
        }
        if (type == "radar")
        {
            RadarJSON radarJSON = JsonSerializer.Deserialize<RadarJSON>(jsonString);
            Console.WriteLine($"ID: {radarJSON.id}\n" +
                              $"Pulse:\n\t" +
                              $"Pulse Width: {radarJSON.initPulse.pulseWidth}\n\t" +
                              $"Amplitude: {radarJSON.initPulse.amplitude}\n\t" +
                              $"Frequency: {radarJSON.initPulse.frequency}\n\t" +
                              $"Time of Traversal: {radarJSON.initPulse.timeOfTraversal}\n\n" +
                              $"Position: {radarJSON.position.x}, {radarJSON.position.y}\n" +
                              $"PRI: {radarJSON.pulseRepetitionInterval}\n" +
                              $"Radius: {radarJSON.radius}\n\n");
        }

        if (type == "aircraftlist")
        {
            AircraftListJSON aircraftListJSON = JsonSerializer.Deserialize<AircraftListJSON>(jsonString);
            for (int i = 0; i < aircraftListJSON.aircraftJSONs.Count; i++)
            {
                Console.WriteLine($"ID: {aircraftListJSON.aircraftJSONs[i].id}\nWaypoints:\n");
                for (int j = 0; j < aircraftListJSON.aircraftJSONs[i].waypoints.Count; j++)
                {
                    Console.WriteLine($"\t({aircraftListJSON.aircraftJSONs[i].waypoints[j].x}," +
                                      $"{aircraftListJSON.aircraftJSONs[i].waypoints[j].y})");
                }
                Console.WriteLine();
            }
        }

        if (type == "radarlist")
        {
            RadarListJSON radarListJSON = JsonSerializer.Deserialize<RadarListJSON>(jsonString);
            for (int i = 0; i < radarListJSON.radarJSONs.Count; i++)
            {
                Console.WriteLine($"Radar:\n\nID: {radarListJSON.radarJSONs[i].id}\n" +
                                  $"Pulse: \n\t" +
                                  $"Pulse Width: {radarListJSON.radarJSONs[i].initPulse.pulseWidth}\n\t" +
                                  $"Amplitude: {radarListJSON.radarJSONs[i].initPulse.amplitude}\n\t" +
                                  $"Frequency: {radarListJSON.radarJSONs[i].initPulse.frequency}\n\t" +
                                  $"Time of Traversal: {radarListJSON.radarJSONs[i].initPulse.timeOfTraversal}\n\n" +
                                  $"Position: {radarListJSON.radarJSONs[i].position.x}, {radarListJSON.radarJSONs[i].position.x}\n" +
                                  $"PRI: {radarListJSON.radarJSONs[i].pulseRepetitionInterval}\n" +
                                  $"Radius: {radarListJSON.radarJSONs[i].radius}\n\n");
            }
        }
    }
}