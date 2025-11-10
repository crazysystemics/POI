using System.Text.Json;
using System.Text.Json.Nodes;

public class MissionInputParser
{
    public class AircraftJSON
    {
        public int id { get; set; }
        public List<int[]> waypoints { get; set; }
    }
    string jsonString = @"{""id"": 0, ""waypoints"": [[50, 50], [85, 85], [135, 85], [170, 50], [135, 15], [85, 15], [50, 50]]}";
    public void ParseJSON()
    {
        AircraftJSON aircraftJSON = JsonSerializer.Deserialize<AircraftJSON>(jsonString);
    }
}