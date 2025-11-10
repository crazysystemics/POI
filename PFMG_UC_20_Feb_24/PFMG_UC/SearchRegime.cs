using PFMG_UC;
using System;
using static PFMG_UC.MVCEntity;
using System.Security.Cryptography;
using System.Windows.Controls;

using System.Windows.Markup;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG_UC
{
    public enum Bands
    {
        L, A, B, C, D
    }

    public enum HWConfig
    {
        NO_OF_PRI_TRACKERS = 5, NO_OF_FREQ_TRACKERS = 5
    }

    //public class Mission_Definition
    //{

    //}

    public class PulseAmplitudeRecord
    {
        public int sector_num { get; set; }
        public int no_of_pulses { get; set; }
        public double min_amplitude { get; set; }


        public PulseAmplitudeRecord(int psector_num, int pno_of_pulses, double pmin_amplitude)
        {
            sector_num = psector_num;
            no_of_pulses = pno_of_pulses;
            min_amplitude = pmin_amplitude;
        }
    }

    public class PRIRecord
    {
        public double pri_min;
        public double pri_max;

        public PRIRecord(double ppri_min, double ppri_max)
        {
            pri_min = ppri_min;
            pri_max = ppri_max;
        }
    }
    public class FreqRecord
    {
        public double freq_min;
        public double freq_max;

        public FreqRecord(double pfreq_min, double pfreq_max)
        {
            freq_min = pfreq_min;
            freq_max = pfreq_max;
        }
    }

    public class SearchRegime : MVCEntity
    {
        public int id;
        public string identifier;
        public Bands band;
        public double reception_time;
        public PulseAmplitudeRecord[] pulse_amplitude_table;

        public int[] sector_num;
        public PRIRecord[] pri_tracker;
        public FreqRecord[] freq_tracker;

        class SearchRegimeId : Identifier
        {
            public SearchRegimeId()
            {
                id = "SR";
                type = "SearchRegime";
            }
        }

        class SearchRegimeHeader : Header
        {
            string entity_no;
            string entity_type;
            int count;

            public SearchRegimeHeader(int pcount, SearchRegime sr)
            {
                entity_no = "E2";
                entity_type = "SearchRegime";
                count = pcount;
                header = "Entity ID" + "," + entity_no + "\n" + "Entity Name" + "," + entity_type + "\n" + "Entity Count" + "," + count + "\n\n";
                header += "Identifier" + "," + "SearchRegime ID" + "," + "Band" + "," + "Reception Time";
            }
        }

        public SearchRegime(UserControl puc)
        {
            identifier = "SR";
            this.UC = (Entity_UC)puc;
            pulse_amplitude_table = new PulseAmplitudeRecord[64];
            pri_tracker = new PRIRecord[Convert.ToInt32(HWConfig.NO_OF_PRI_TRACKERS)];
            freq_tracker = new FreqRecord[Convert.ToInt32(HWConfig.NO_OF_PRI_TRACKERS)];
            sector_num = new int[Convert.ToInt32(HWConfig.NO_OF_PRI_TRACKERS)];
        }

        public SearchRegime(UserControl puc, int pid, double preception_time, PulseAmplitudeRecord[] ppulse_amplitude_table, PRIRecord[] ppri_tracker, FreqRecord[] pfreq_tracker, int[] psector_mun)
        {
            id = pid;
            reception_time = preception_time;
            pulse_amplitude_table = ppulse_amplitude_table;
            pri_tracker = ppri_tracker;
            freq_tracker = pfreq_tracker;
            sector_num = psector_mun;
            identifier = "SR";
            this.UC = (Entity_UC)puc;
        }

        public override Header GetHeader(int count)
        {
            SearchRegimeHeader sr_header = new SearchRegimeHeader(count, this);
            return sr_header;
        }

        public override MVCEntity get_entity(string[] data, ref StreamReader reader)
        {
            reader.ReadLine();
            reader.ReadLine();
            string S;
            string[] data1 = null;
            PulseAmplitudeRecord[] pulse_amplitude_table = new PulseAmplitudeRecord[64];

            for (int i = 0; i < 64; i++)
            {
                S = reader.ReadLine();
                data1 = S.Split(',');
                pulse_amplitude_table[i] = new PulseAmplitudeRecord(Convert.ToInt32(data1[0]), Convert.ToInt32(data1[1]), Convert.ToDouble(data1[2]));
            }

            PRIRecord[] pri_tracker = new PRIRecord[5];
            FreqRecord[] freq_tracker = new FreqRecord[5];
            int[] sector_num = new int[5];
            reader.ReadLine();
            reader.ReadLine();

            for (int i = 0; i < Convert.ToInt32(HWConfig.NO_OF_PRI_TRACKERS); i++)
            {
                S = reader.ReadLine();
                data1 = S.Split(',');
                sector_num[i] = Convert.ToInt32(data1[0]);
                pri_tracker[i] = new PRIRecord(Convert.ToDouble(data1[1]), Convert.ToDouble(data1[2]));
                freq_tracker[i] = new FreqRecord(Convert.ToDouble(data1[3]), Convert.ToDouble(data1[4]));
            }

            SearchRegime sr = new SearchRegime((UserControl)UC, Convert.ToInt32(data[1]), Convert.ToDouble(data[3]), pulse_amplitude_table, pri_tracker, freq_tracker, sector_num);
            if (data[2] == "L") sr.band = Bands.L;
            if (data[2] == "A") sr.band = Bands.A;
            if (data[2] == "B") sr.band = Bands.B;
            if (data[2] == "C") sr.band = Bands.C;
            if (data[2] == "D") sr.band = Bands.D;
            return sr;
        }

        public override Identifier get_Identifier()
        {
            SearchRegimeId sr_id = new SearchRegimeId();
            return sr_id;
        }

        public override bool Match(MVCEntity entity_list, MVCEntity entity)
        {
            SearchRegime sr_list = (SearchRegime)entity_list;
            SearchRegime curr_sr = (SearchRegime)entity;

            if (sr_list.id == curr_sr.id)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            SearchRegime sr = this;

            string s = "SR" + "," + sr.id + "," + sr.band + "," + sr.reception_time;
            s += "\n" + "PULSE_AMPLITUDE_TABLE" + "\n" + "Sector No" + "," + "No of Pulses" + "," + "Min Amplitude";

            for (int i = 0; i < 64; i++)
            {
                s += "\n" + sr.pulse_amplitude_table[i].sector_num + "," + sr.pulse_amplitude_table[i].no_of_pulses + "," + sr.pulse_amplitude_table[i].min_amplitude;
            }

            s += "\n" + "PRI_and_Freq_TRACKER" + "\n" + "Sector Number" + "," + "PRI_min" + "," + "PRI_max" + "," + "Freq_min" + "," + "Freq_max";

            for (int i = 0; i < Convert.ToInt32(HWConfig.NO_OF_PRI_TRACKERS); i++)
            {
                s += "\n" + sr.sector_num[i] + "," + sr.pri_tracker[i].pri_min + "," + sr.pri_tracker[i].pri_max + "," + sr.freq_tracker[i].freq_min + "," + sr.freq_tracker[i].freq_max;
            }

            s += "\n";
            return s;
        }
    }
}
