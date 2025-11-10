namespace SimpleARM
{
    class Aircraft
    {
        public int x, y;

        public Aircraft(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Radar
    {
        public int range;
        public int x, y;
        public Radar(int x, int y, int range)
        {
            this.range = range;
            this.x = x;
            this.y = y;
        }
        public bool IsAircraftInRange(Aircraft aircraft)
        {
            // Simplified logic for demonstration purposes
            return (aircraft.x * aircraft.x + aircraft.y * aircraft.y) <= (range * range);
        }
    }
    internal class Program
    {
        public static Aircraft aircraft = new Aircraft(10, 10);
        public static Radar radar = new Radar(0, 0, 100);
        public static int num_iterations = 100;


        static void Main(string[] args)
        {

            int detected_count = 0;
            bool wasInRange = radar.IsAircraftInRange(aircraft);
            int entry_i = -1, exit_i = 1;
            for (int i = 0; i < num_iterations; i++)
            {
                //generate code to find values of i when aircraft enters radar range
                //and when exits radar range
                // Track when aircraft enters and exits radar range                
                aircraft.x += 1;
                aircraft.y += 1;

                bool isInRange = radar.IsAircraftInRange(aircraft);
                if (isInRange)
                {
                    detected_count++;
                }

                if (!wasInRange && isInRange)
                {
                    entry_i = i;
                    System.Console.WriteLine($"Aircraft entered radar range at iteration {i}.");
                }
                else if (wasInRange && !isInRange)
                {
                    exit_i = i;
                    System.Console.WriteLine($"Aircraft exited radar range at iteration {i}.");
                }
                wasInRange = isInRange;
            }

            // Console.WriteLine("Hello, World!");
            System.Console.WriteLine($"Aircraft detected {detected_count} times.");
        }
    }
}
