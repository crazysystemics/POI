using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG_UC
{
    public interface IFREQ
    {
        Frequency_Agility GetFreq();
        ref Frequency_Agility Set(ref Frequency_Agility pri_agility, string[] data);
    }

    public class Frequency_Agility
    {

    }

    public class Pulse_to_Pulse : Frequency_Agility, IFREQ
    {
        public double percentage;

        public Pulse_to_Pulse()
        {

        }

        public Pulse_to_Pulse(double pjitter_percentage)
        {
            percentage = pjitter_percentage;
        }

        public Frequency_Agility GetFreq()
        {
            return this;
        }

        public ref Frequency_Agility Set(ref Frequency_Agility freq_agility, string[] data)
        {
            freq_agility = new Pulse_to_Pulse(Convert.ToDouble(data[1]));

            return ref freq_agility;
        }

        public override string ToString()
        {
            Pulse_to_Pulse jt = this;

            string s = "\n" + Freq_Agility.PULSE_TO_PULSE + "\n" + "Percentage" + "," + jt.percentage + "\n";
            return s;
        }
    }

    public class Batch_to_Batch : Frequency_Agility, IFREQ
    {
        public int no_of_batches;
        public bool traversal_order;
        public Batch[] batches;

        public class Batch
        {
            public int batch_no;
            public int no_of_pulses;
            public double Freq;

            public Batch(int pbatch_no, int pno_of_pulse, double pfreq)
            {
                batch_no = pbatch_no;
                no_of_pulses = pno_of_pulse;
                Freq = pfreq;
            }
        }
        public Batch_to_Batch()
        {

        }

        public Batch_to_Batch(int pno_of_batches, Batch[] pbatches, Traversal_Order ptraversal_order)
        {
            no_of_batches = pno_of_batches;
            traversal_order = Convert.ToBoolean(ptraversal_order);
            batches = pbatches;
        }

        public Frequency_Agility GetFreq()
        {
            return this;
        }

        public ref Frequency_Agility Set(ref Frequency_Agility freq_agility, string[] data)
        {
            freq_agility = this;
            return ref freq_agility;
        }

        public override string ToString()
        {
            Batch_to_Batch sw = this;

            string s = "\n" + Freq_Agility.BATCH_TO_BATCH + "\n" + "No of Batches" + "," + "Traversal Order" + "\n" + sw.no_of_batches + "," + Convert.ToInt32(sw.traversal_order) + "\n";
            s += "Batch No" + "," + "No of Pulses" + "," + "Frequency";
            for (int i = 0; i < 5; i++)
            {
                s += "\n" + i + 1 + "," + sw.batches[i].no_of_pulses + "," + sw.batches[i].Freq;
            }
            s += "\n";
            return s;
        }
    }
}
