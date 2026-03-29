using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleARM
{
    internal class BattleSystem
    {
    }

    class Aircraft
    {
        public double x, y;

        public Aircraft(double x = 0.0, double y = 0.0)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Radar
    {
        public double range;
        public double x, y;
        public Radar(double x = 0.0, double y = 0.0, double range = 0.0)
        {
            this.range = range;
            this.x = x;
            this.y = y;
        }
        public bool IsAircraftInRange(Aircraft aircraft)
        {
            // Compute actual distance between radar and aircraft
            double act_range = Math.Sqrt((x - aircraft.x) * (x - aircraft.x) + (y - aircraft.y) * (y - aircraft.y));
            bool inRange = (act_range <= range);

            // Write when aircraft.y is 3 (use tolerant comparison for floating point)
            string detected = "";
            if (Math.Abs(aircraft.y - 3.0) < 1e-9)
            {
                detected = inRange ? "detected" : "not detected";               
            }

            if (aircraft.y == 3.0)
            {
                Console.WriteLine($"Radar at ({x}, {y}) aircraft at ({aircraft.x}, {aircraft.y}) actual range ({act_range:F3}) -> {detected}");
            }

            return inRange;
        }
    }
}
