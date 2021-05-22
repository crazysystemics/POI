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
            //sglobal.sw  = new StreamWriter("soc.csv");
            sglobal.r   = new Random(5);
            blue_uas.init_population();
            //sglobal.dsw = new StreamWriter("debug1.csv");

            //TODO:Encapsulate in a method
            if (sglobal.sw == null)
            {
                sglobal.sw = new StreamWriter("soc.csv");
            }
            else
            {
                sglobal.sw = new StreamWriter("soc.csv", true);
            }

            sglobal.sw.WriteLine(blue_uas[blue_uas.minfit()]);
            string header = String.Empty;
            header = "step";
            for (int i=0; i<32; i++)
            {
                header += ", fit-" + i.ToString();
            } 
            
            sglobal.sw.WriteLine(header);

            List<string> fit_csv = new List<string>();
            for (int step = 0; step <  100 ; step++)
            {
                string mfcs = step.ToString();
                fit_csv = new List<string>();
                fit_csv.Add(mfcs) ;
                blue_uas.evolve_next_step();
                double minfitc = 1.0;
                int mini = 0;
                for (int i = 0; i < blue_uas.buf.Length; i++)
                {
                  
                    if  (blue_uas.buf[i].fitness < minfitc)
                    {
                        minfitc = blue_uas.buf[i].fitness;
                        mini = i;
                    }
                    fit_csv.Add(blue_uas.buf[i].fitness.ToString());
                }
                


                mini++;//skip step
                int count = 0;
                foreach (string s in fit_csv)
                {
                    if (count == 0) //step column
                    {
                        sglobal.sw.Write(s);
                    }
                    else if (count == mini)
                    {
                        sglobal.sw.Write("," + " VVV " + s);
                    }
                    else
                    {
                        sglobal.sw.Write("," + s);
                    }
                    count++;
                }

                sglobal.sw.WriteLine();
            }


            fit_csv.TrimExcess();          
            sglobal.sw.Close();
            
            //new StreamWriter("ddebug.csv")
            //sglobal.dsw.Close();
        }
    }
}
