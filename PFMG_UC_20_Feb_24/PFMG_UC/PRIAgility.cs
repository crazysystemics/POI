using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PFMG_UC
{
    public enum Traversal_Order
    { SLIDER, HOPPER }

    public interface IPRI
    {
        PRIAgility GetPRI();
        ref PRIAgility Set(ref PRIAgility pri_agility, string[] data);
    }

    public class PRIAgility
    {

    }

    public class Fixed : PRIAgility, IPRI
    {
        public PRIAgility GetPRI()
        {
            return this;
        }

        public ref PRIAgility Set(ref PRIAgility pri_agility, string[] data)
        {
            return ref pri_agility;
        }

        public override string ToString()
        {

            return "";
        }
    }

    public class Sub_PRI
    {
        public int level_no;
        public double pri_min;
        public double pri_max;

        public Sub_PRI(int plevel_no, double ppri_min, double ppri_max)
        {
            level_no = plevel_no;
            pri_min = ppri_min;
            pri_max = ppri_max;
        }
    }
    public class Staggered : PRIAgility, IPRI
    {
        public int no_of_levels;
        public Sub_PRI[] sub_pris;

        public Staggered()
        {

        }

        public Staggered(int pno_of_levels, Sub_PRI[] psub_pris)
        {
            no_of_levels = pno_of_levels;
            sub_pris = psub_pris;
        }

        public ref PRIAgility Set(ref PRIAgility pri_agility, string[] data)
        {
            pri_agility = this;
            return ref pri_agility;
        }

        public PRIAgility GetPRI()
        {
            return this;
        }

        public override string ToString()
        {
            Staggered st = this;
            string s;

            s = "\n" + PRI_Agility.STAGGERED + "\n" + "No of Levels" + "," + "Level" + "," + "Sub PRI min" + "," + "Sub PRI max";

            for (int i = 0; i < 3; i++)
            {
                s += "\n" + st.no_of_levels + "," + st.sub_pris[i].level_no + "," + st.sub_pris[i].pri_min + "," + st.sub_pris[i].pri_max;
            }
            return s;
        }
    }

    public class Jittered : PRIAgility, IPRI
    {
        public double jitter_percentage;

        public Jittered()
        {

        }

        public Jittered(double pjitter_percentage)
        {
            jitter_percentage = pjitter_percentage;
        }

        public PRIAgility GetPRI()
        {
            return this;
        }

        public ref PRIAgility Set(ref PRIAgility pri_agility, string[] data)
        {
            pri_agility = new Jittered(Convert.ToDouble(data[1]));

            return ref pri_agility;
        }

        public override string ToString()
        {
            Jittered jt = this;

            string s = "\n" + PRI_Agility.JITTERED + "\n" + "Jitter Percentage" + "," + jt.jitter_percentage;
            return s;
        }
    }


    public class Batch
    {
        public int batch_no;
        public int no_of_pulses;
        public double pri;

        public Batch(int pbatch_no, int pno_of_pulse, double ppri)
        {
            batch_no = pbatch_no;
            no_of_pulses = pno_of_pulse;
            pri = ppri;
        }
    }

    public class Switcher : PRIAgility, IPRI
    {
        public int no_of_batches;
        public bool traversal_order;
        public Batch[] batches;
        public Switcher()
        {
        }

        public Switcher(int pno_of_batches, Batch[] pbatches, Traversal_Order ptraversal_order)
        {
            no_of_batches = pno_of_batches;
            traversal_order = Convert.ToBoolean(ptraversal_order);
            batches = pbatches;
        }

        public PRIAgility GetPRI()
        {
            return this;
        }

        public ref PRIAgility Set(ref PRIAgility pri_agility, string[] data)
        {
            pri_agility = this;
            return ref pri_agility;
        }

        public override string ToString()
        {
            Switcher sw = this;

            string s = "\n" + PRI_Agility.SWITCHER + "\n" + "No of Batches" + "," + "Traversal Order" + "\n" + sw.no_of_batches + "," + Convert.ToInt32(sw.traversal_order) + "\n";
            s += "Batch No" + "," + "No of Pulses" + "," + "PRI";
            for (int i = 0; i < 5; i++)
            {
                s += "\n" + i + 1 + "," + sw.batches[i].no_of_pulses + "," + sw.batches[i].pri;
            }
            return s;
        }
    }
}
