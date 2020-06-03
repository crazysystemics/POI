using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace small_world_swarm
{
    //TODO BEGIN:
    //1. Page Display
    //2. Print Header in different color
    //3. Make Title a parameter
    //4. Write multi histogram chart multi plot chart
    //TODO END:
    class Cluster
    {
        int cluster_id;
        List<Tuple<int, int>> edges = new List<Tuple<int, int>>();
        double cliqueness;
    }

    class Program
    {

        static void Main(string[] args)
        {
            ConsoleUiManager cm = new ConsoleUiManager();
            cm.RunTests();

            Console.ReadKey();
        }
    }
}
