using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//first commit
//first commit + delta1

namespace selfoc_evoln
{   

    
    class Program
    {
        public static soc_population blue_uas = new soc_population(32);
        public static cyclic_counter ci = new cyclic_counter(32);
        public static Random r = new Random(5);

        static void Main(string[] args)
        {
            sglobal.sw  = new StreamWriter("soc.csv");
            blue_uas.init_population();
            
            sglobal.sw.WriteLine(blue_uas[blue_uas.minfit()]);
            for (int step = 0; step <  1000 ; step++)
            {
                blue_uas.evolve_next_step();
            }

            sglobal.sw.Close();
        }
    }
}
