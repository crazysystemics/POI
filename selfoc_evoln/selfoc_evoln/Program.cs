﻿using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfoc_evoln
{
    class cyclic_counter
    {
        public int size;
        public int count;

        public static implicit operator int(cyclic_counter c) => c.count;
        public static implicit operator cyclic_counter(int ci) => new cyclic_counter(ci);
        //public static explicit operator int(cyclic_counter c) => c.count;



        public cyclic_counter(int c = 32)
        {
            size = 32;
            count = 0;
        }

        static public cyclic_counter operator ++(cyclic_counter c)
        {
            c.count = (c.count + 1) % c.size;
            return c;
        }

        static public cyclic_counter operator --(cyclic_counter c)
        {
            c.count = (c.count + c.size - 1) % c.size;//
            return c;
        }
    }

    class soc_species
    {
        
        public double fitness;

        public static implicit operator double(soc_species ss) => ss.fitness;        
 
        public soc_species()
        {            
            
        }  

    }

    class soc_population
    {
        public  int size;
        double[]    buf;      
               
        public soc_population(int psize)
        {
            size = psize;
            buf = new double[size];
        }

        public double this[cyclic_counter key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        public double GetValue(cyclic_counter index)
        {
            Debug.Assert(index < 2 * size);
            if (index < 0)
            {
                index = size - index;
            }
            else if (index >= size)
            {
                index = size + index;
            }
            return buf[index];
        }

        public void SetValue(cyclic_counter index, double value)
        {
            buf[index] = value;
        }

        override public string ToString()
        {
            string s = String.Empty;
            for(int i = 0; i < buf.Length; i++)
            {
                s = s + buf[i];
                if (i < buf.Length - 1)
                {
                    s = s + ",";
                }
            }

            return s;
        }

    }
    class Program
    {
        public static soc_population blue_uas = new soc_population(32);
        public static cyclic_counter ci = new cyclic_counter(32);
        public static Random r = new Random(5);
        public static StreamWriter sw;

        public static void init_population()
        {
            bool first = true;
            for (ci = 0;ci < ci.size; ci++)
            {
                if (ci == 0)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        break;
                    }
                }
                blue_uas[ci] = r.NextDouble();
            }
        }

        static public cyclic_counter minfit(soc_population sp)
        {
            double minf = 1.0;
            cyclic_counter minci = 0;
            bool first = true;
            for (cyclic_counter ci = 0; ci < blue_uas.size; ci++)
            {
                if (ci == 0)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        break;
                    }
                }
                


                if (blue_uas[ci] < minf)
                {
                    minf = blue_uas[ci];
                    minci.count = ci.count;
                }
            }
            return minci;
        }

        static public void evolve_next_step()
        {
            cyclic_counter minci = minfit(blue_uas);

            //replace 1-left, 1-cur, 1-right with random numbers
            //totally 3 clusters and 3 cells for each totally 3*3 =9
            //since 9 cells have to be replaced, 4-left 1-cur 4-right
            for(cyclic_counter index = minci - 4; index < minci + 4; minci++)
            {
                blue_uas[index] = r.NextDouble();
                sw.WriteLine(blue_uas.ToString());
                
            }

        }        
        
        static void Main(string[] args)
        {
            sw  = new StreamWriter("soc.csv");
            init_population();
            for (int step = 0; step < 10; step++)
            {
                evolve_next_step();
            }
            sw.Close();
        }
    }
}