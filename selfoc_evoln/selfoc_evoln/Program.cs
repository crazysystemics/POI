using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Tasks 
//DONE
//00_01: Initial Baseline: fitness for  m(100) steps x n(32) species in each step 
//TODO
//00_02: [*]Evolving fitness using SOC algorithm (mutating over neighbors fitness)                        
//00_03: IIT (Compute-Phi, Given a Mechanism, Inputs and Outputs)
//00_04: IIT + SOC :: Phi as evolving parameter
//00_05: UAS_Swarm (Linear), Blue vs Red Battle (a Random Simulation)
//00_06: UAS_Swarm => (IIT Network) => phi => phi as SOC parameter

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
            //open the file
            if (sglobal.sw == null)
            {
                sglobal.sw = new StreamWriter("soc.csv");
            }
            else
            {
                sglobal.sw = new StreamWriter("soc.csv", true);
            }

            //TODO: make a proper conversion function
            //      within the class
            double[] bcopy = new double[blue_uas.buf.Length];
            for (int bi = 0; bi < blue_uas.buf.Length; bi++)
            {
                bcopy[bi] = blue_uas.buf[bi].fitness;
            }

            //sglobal.sw.WriteLine(blue_uas[blue_uas.minfit()].ToString());
            string fitness_s = String.Empty;
            for(int index = 0; index < blue_uas.buf.Length; index++)
            {
                string delim = String.Empty;
                delim = (index == 0 ? "init_fit ," : ",");
                double d = blue_uas.buf[index].fitness;
                fitness_s += delim + blue_uas.buf[index].fitness.ToString();
            }

            sglobal.sw.WriteLine(fitness_s);

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

                //Find minimum fitness
                for (int i = 0; i < blue_uas.buf.Length; i++)
                {                  
                    if  (blue_uas.buf[i].fitness < minfitc)
                    {
                        minfitc = blue_uas.buf[i].fitness;
                        mini = i;
                    }
                }


                //minimum fitness is computed. 
                //replace it and neighbors with random numbers
                int cluster_size = 9;
                int start = (mini-cluster_size / 2 
                             + blue_uas.buf.Length ) % blue_uas.buf.Length;
                int end  = (mini+cluster_size / 2 
                              + blue_uas.buf.Length) % blue_uas.buf.Length;

                for (int i = start; i != end; i++)
                {
                    //end of line encountered while going
                    //from start to end
                    if (i==blue_uas.buf.Length)
                    {
                        i = 0;
                    }
                    blue_uas.buf[i].fitness = r.NextDouble();
                    
                }

                //Find minimum fitness
                for (int i = 0; i < blue_uas.buf.Length; i++)
                {
                    fit_csv.Add(blue_uas.buf[i].fitness.ToString());
                }

                //Write results into file
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



                              
            sglobal.sw.Close();

            //fit_csv.TrimExcess(); 
            //new StreamWriter("ddebug.csv")
            //sglobal.dsw.Close();
        }
    }
}
