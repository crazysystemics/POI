using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace PFMG_UC
{
    public enum Side
    {
        BLUE, RED
    }

    public enum PRI_Agility
    {
        FIXED, STAGGERED, JITTERED, SWITCHER
    }

    public enum Freq_Agility
    {
        PULSE_TO_PULSE, BATCH_TO_BATCH
    }

    public class Radar : Weapon_System
    {
        public int id;
        public Side side;
        public string identifier;

        public double pri_min;
        public double pri_max;
        public double pw_min;
        public double pw_max;
        public double frequency_min;
        public double frequency_max;
        public double tolerance;
        public PRI_Agility priagility;
        public PRIAgility pri_agility1;
        public Freq_Agility frequency_agility;
        public Frequency_Agility freq_agility;

        class RadarId : Identifier
        {
            public RadarId()
            {
                id = "R";
                type = "Radar";
            }
        }

        class RadarHeader : Header
        {
            string entity_no;
            string entity_type;
            int count;

            public RadarHeader(int pcount, Radar r)
            {
                entity_no = "E1";
                entity_type = "Radar";
                count = pcount;
                header = "Entity ID" + "," + entity_no + "\n" + "Entity Name" + "," + entity_type + "\n" + "Entity Count" + "," + count + "\n\n";
                header += "Identifier" + "," + "Radar ID" + "," + "Side" + "," + "PRI min" + "," + "PRI max" +
        "," + "PW min" + "," + "PW max" + "," + "Frequency min" + "," + "Frequency max" + "," + "PRI Agility" + "," + "Frequency Agiltiy" + "," + "Tolerance";
            }
        }
        public Radar(UserControl puc)
        {
            identifier = "R";
            this.UC = (Entity_UC)puc;
            pri_agility1 = null;
        }

        public Radar(UserControl puc, int pid, Side pside, PRIAgility ppri_agility, Frequency_Agility pfreq_agility, double ppri_min = 0, double ppri_max = 0, double ppw_min = 0, double ppw_max = 0,
        double pfrequency_min = 0, double pfrequency_max = 0, double ptolerance = 0)
        {
            id = pid;
            side = pside;
            pri_min = ppri_min;
            pri_max = ppri_max;
            pw_min = ppw_min;
            pw_max = ppw_max;
            frequency_min = pfrequency_min;
            frequency_max = pfrequency_max;
            pri_agility1 = ppri_agility;
            freq_agility = pfreq_agility;

            if (ppri_agility is Fixed) priagility = PRI_Agility.FIXED;
            if (ppri_agility is Staggered) priagility = PRI_Agility.STAGGERED;
            if (ppri_agility is Jittered) priagility = PRI_Agility.JITTERED;
            if (ppri_agility is Switcher) priagility = PRI_Agility.SWITCHER;

            if (pfreq_agility is Pulse_to_Pulse) frequency_agility = Freq_Agility.PULSE_TO_PULSE;
            if (pfreq_agility is Batch_to_Batch) frequency_agility = Freq_Agility.BATCH_TO_BATCH;
            //frequency_agility = pfrequency_agility;
            tolerance = ptolerance;
            identifier = "R";
            this.UC = (Entity_UC)puc;
        }

        public override string ToString()
        {
            Radar r = this;

            string s = "R" + "," + r.id + "," + r.side + "," + r.pri_min + "," + r.pri_max + "," + r.pw_min
                                + "," + r.pw_max + "," + r.frequency_min + "," + r.frequency_max + "," + r.priagility + "," + r.frequency_agility + "," + r.tolerance;

            s += r.pri_agility1.ToString();
            s += r.freq_agility.ToString();
            return s;
        }

        public override bool Match(MVCEntity w_list, MVCEntity curr_w)
        {
            Radar r_list = (Radar)w_list;
            Radar curr_radar = (Radar)curr_w;

            if (r_list.id == curr_radar.id)
            {
                return true;
            }
            return false;
        }

        public override MVCEntity get_entity(string[] data, ref StreamReader reader)
        {
            //Freq_AgilityFreqAgility = 0;
            Side side;

            if (data[1] == "BLUE")
                side = Side.BLUE;
            else
                side = Side.RED;

            Radar r = null;

            r = new Radar((UserControl)UC, Convert.ToInt32(data[1]), side, null, null, Convert.ToDouble(data[3]), Convert.ToDouble(data[4]),
        Convert.ToDouble(data[5]), Convert.ToDouble(data[6]), Convert.ToDouble(data[7]), Convert.ToDouble(data[8]), Convert.ToDouble(data[11]));

            if (data[9] == "FIXED")
            {
                r.pri_agility1 = new Fixed();
                r.priagility = PRI_Agility.FIXED;
            }
            else if (data[9] == "STAGGERED")
            {
                reader.ReadLine();
                reader.ReadLine();
                string[] data1 = null;

                Sub_PRI[] sub_pri = new Sub_PRI[3];
                for (int i = 0; i < 3; i++)
                {
                    string S = reader.ReadLine();
                    data1 = S.Split(',');
                    sub_pri[i] = new Sub_PRI(Convert.ToInt32(data1[1]), Convert.ToInt32(data1[2]), Convert.ToDouble(data1[3]));
                }

                Staggered s = new Staggered();
                s.no_of_levels = Convert.ToInt32(data1[0]);
                s.sub_pris = sub_pri;

                r.pri_agility1 = s.Set(ref r.pri_agility1, data1);
                r.priagility = PRI_Agility.STAGGERED;
            }
            else if (data[9] == "JITTERED")
            {
                reader.ReadLine();
                string S = reader.ReadLine();
                string[] data1 = S.Split(',');

                Jittered j = new Jittered();

                r.pri_agility1 = j.Set(ref r.pri_agility1, data1);
                r.priagility = PRI_Agility.JITTERED;
            }
            else if (data[9] == "SWITCHER")
            {
                reader.ReadLine();
                reader.ReadLine();
                string S = reader.ReadLine();
                string[] data2 = S.Split(',');

                Batch[] bt = new Batch[5];

                reader.ReadLine();
                string[] data1 = null;

                for (int i = 0; i < 5; i++)
                {
                    S = reader.ReadLine();
                    data1 = S.Split(',');
                    bt[i] = new Batch(Convert.ToInt32(data1[0]), Convert.ToInt32(data1[1]), Convert.ToDouble(data1[2]));
                }
                Traversal_Order tr;
                if (data2[1] == "0")
                    tr = Traversal_Order.SLIDER;
                else
                    tr = Traversal_Order.HOPPER;

                Switcher sw = new Switcher(Convert.ToInt32(data2[0]), bt, tr);
                r.pri_agility1 = sw.Set(ref r.pri_agility1, data1);
                r.priagility = PRI_Agility.SWITCHER;
            }

            if (data[10] == "PULSE_TO_PULSE")
            {
                reader.ReadLine();
                string S = reader.ReadLine();
                string[] data1 = S.Split(',');

                Pulse_to_Pulse j = new Pulse_to_Pulse();

                r.freq_agility = j.Set(ref r.freq_agility, data1);
                r.frequency_agility = Freq_Agility.PULSE_TO_PULSE;
            }
            else if (data[10] == "BATCH_TO_BATCH")
            {
                reader.ReadLine();
                reader.ReadLine();
                string S = reader.ReadLine();
                string[] data2 = S.Split(',');

                Batch_to_Batch.Batch[] bt = new Batch_to_Batch.Batch[5];

                reader.ReadLine();

                string[] data1 = null;

                for (int i = 0; i < 5; i++)
                {
                    S = reader.ReadLine();
                    data1 = S.Split(',');
                    bt[i] = new Batch_to_Batch.Batch(Convert.ToInt32(data1[0]), Convert.ToInt32(data1[1]), Convert.ToDouble(data1[2]));

                }
                Traversal_Order tr;
                if (data2[1] == "0")
                    tr = Traversal_Order.SLIDER;
                else
                    tr = Traversal_Order.HOPPER;

                Batch_to_Batch btb = new Batch_to_Batch(Convert.ToInt32(data2[0]), bt, tr);
                r.freq_agility = btb.Set(ref r.freq_agility, data1);
                r.frequency_agility = Freq_Agility.BATCH_TO_BATCH;
            }
            return r;
        }

        public override Identifier get_Identifier()
        {
            RadarId r_id = new RadarId();
            return r_id;
        }

        public override Header GetHeader(int count)
        {
            RadarHeader r_header = new RadarHeader(count, this);
            return r_header;
        }
    }
}
