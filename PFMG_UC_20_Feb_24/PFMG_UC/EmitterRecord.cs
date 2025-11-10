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
    public enum JammerTechniques
    {
        ACTIVE, PASSIVE
    }
    public class EmitterRecord : Radar
    {
        public string er_identifier;
        public int er_id;
        public double freq_track_window;
        public double pri_track_window;
        public double pw_track_window;
        public double azimuth_track_window;
        public int AGEIN;
        public int AGEOUT;

        public JammerTechniques jammer_technique;
        public int jammer_technique_key;

        class EmitterRecordId : Identifier
        {
            public EmitterRecordId()
            {
                id = "ER";
                type = "EmitterRecord";
            }
        }

        class EmitterRecordHeader : Header
        {
            string entity_no;
            string entity_type;
            int count;

            public EmitterRecordHeader(int pcount, EmitterRecord po)
            {
                entity_no = "E4";
                entity_type = "EmitterRecord";
                count = pcount;
                header = "Entity ID" + "," + entity_no + "\n" + "Entity Name" + "," + entity_type + "\n" + "Entity Count" + "," + count + "\n\n";
            }
        }

        public EmitterRecord(UserControl puc, Radar r) : base(puc, r.id, r.side, r.pri_agility1, r.freq_agility,
        r.pri_min, r.pri_max, r.pw_min, r.pw_max, r.frequency_min, r.frequency_max, r.tolerance)
        {
            er_identifier = "ER";
            this.UC = (Entity_UC)puc;
        }

        public EmitterRecord(UserControl puc, Radar r, int pid, double pfreq_track_window, double ppri_track_window, double ppw_track_window,
        double pazimuth_track_window, int pagein, int pageout, int pjammer_tech_key) : base(puc, r.id, r.side, r.pri_agility1, r.freq_agility,
        r.pri_min, r.pri_max, r.pw_min, r.pw_max, r.frequency_min, r.frequency_max, r.tolerance)
        {
            er_id = pid;
            freq_track_window = pfreq_track_window;
            pri_track_window = ppri_track_window;
            pw_track_window = ppw_track_window;
            azimuth_track_window = pazimuth_track_window;
            AGEIN = pagein;
            AGEOUT = pageout;
            jammer_technique = JammerTechniques.ACTIVE;
            jammer_technique_key = pjammer_tech_key;

            identifier = "ER";
            this.UC = (Entity_UC)puc;
        }

        public override Header GetHeader(int count)
        {
            EmitterRecordHeader er_header = new EmitterRecordHeader(count, this);
            return er_header;
        }

        public override MVCEntity get_entity(string[] data, ref StreamReader reader)
        {
            Radar r = new Radar(null);
            reader.ReadLine();
            reader.ReadLine();
            string S = reader.ReadLine(); ;
            string[] data1 = S.Split(','); 
            r = (Radar)r.get_entity(data1, ref reader);

            EmitterRecord er = new EmitterRecord((UserControl)UC, r, Convert.ToInt32(data[1]), Convert.ToDouble(data[2]), Convert.ToDouble(data[3]), Convert.ToDouble(data[4]),
            Convert.ToDouble(data[5]), Convert.ToInt32(data[6]), Convert.ToInt32(data[7]), Convert.ToInt32(data[9]));
            return er;
        }

        public override Identifier get_Identifier()
        {
            EmitterRecordId er_id = new EmitterRecordId();
            return er_id;
        }

        public override bool Match(MVCEntity entity_list, MVCEntity entity)
        {
            EmitterRecord er_list = (EmitterRecord)entity_list;
            EmitterRecord curr_er = (EmitterRecord)entity;

            if (er_list.id == curr_er.id)
            {
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            EmitterRecord er = this;


            string s = "Identifier" + "," + "EmitterRecord ID" + "," + "Freq Track Window" + "," + "PRI Track Window" + "," + "PW Track Window" + "," + "Azimuth Track Window"
                            + "," + "AGE_IN" + "," + "AGE_OUT" + "," + "Jammer Technique" + "," + "Jammer Technique key" + "\n";

            s += "ER" + "," + er.id + "," + er.freq_track_window + "," + er.pri_track_window + "," + er.pw_track_window + "," + er.azimuth_track_window + "," +
    er.AGEIN + "," + er.AGEOUT + "," + er.jammer_technique + "," + er.jammer_technique_key + "\n";
            s += "Weapon Selected" + "\n" + "Identifier" + "," + "Radar ID" + "," + "Side" + "," + "PRI min" + "," + "PRI max" +
    "," + "PW min" + "," + "PW max" + "," + "Frequency min" + "," + "Frequency max" + "," + "PRI Agility" + "," + "Frequency Agiltiy" + "," + "Tolerance" + "\n";
            s += base.ToString();

            return s;
        }
    }
}
