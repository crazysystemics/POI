using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfoc_evoln
{
    class soc
    {

        public static void init_population()
        {
            bool first = true;
            for (cyclic_counter ci = 0; ci < ci.size; ci++)
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
            double minf = blue_uas.buf[minci];
            sw.WriteLine(minf.ToString());

            //replace 1-left, 1-cur, 1-right with random numbers
            //totally 3 clusters and 3 cells for each totally 3*3 =9
            //since 9 cells have to be replaced, 4-left 1-cur 4-right
            int index;
            int min_index = minci.count;
            int brndm_index = (min_index + blue_uas.size - 4) % blue_uas.size;
            int erndm_index = (min_index + blue_uas.size + 4 + 1) % blue_uas.size;

            index = brndm_index;
            prev_bu = blue_uas.buf;
            for (index = brndm_index; index != erndm_index; index = (index + 1) % blue_uas.size)
            {
                double d = r.NextDouble();
                blue_uas.buf[index] = d;
            }
        }
    }
}
