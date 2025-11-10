using PFMG_UC;
using System;
using static PFMG_UC.MVCEntity;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows.Markup;

using System.Windows.Media.Media3D;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFMG_UC
{
    public class Record : MVCEntity
    {
        public int id;
        public string identifier;
        public Radar entry_radar;
        public Radar exit_radar;

        class RecordId : Identifier
        {
            public RecordId()
            {
                id = "REC";
                type = "Record";
            }
        }

        class RecordHeader : Header
        {
            string entity_no;
            string entity_type;
            int count;

            public RecordHeader(int pcount, Record po)
            {
                entity_no = "E5";
                entity_type = "Record";
                count = pcount;
                header = "Entity ID" + "," + entity_no + "\n" + "Entity Name" + "," + entity_type + "\n" + "Entity Count" + "," + count + "\n\n";
                header += "Identifier" + "," + "Record ID";
            }
        }

        public Record(UserControl puc)
        {
            identifier = "REC";
            this.UC = (Entity_UC)puc;
            entry_radar = new Radar(null);
            exit_radar = new Radar(null);
        }
        public Record(UserControl puc, int pid, Radar pentry_radar, Radar pexit_radar)
        {
            entry_radar = pentry_radar;
            exit_radar = pexit_radar;
            id = pid;
            identifier = "REC";
            this.UC = (Entity_UC)puc;
        }

        public override Header GetHeader(int count)
        {
            RecordHeader re_header = new RecordHeader(count, this);
            return re_header;
        }

        public override MVCEntity get_entity(string[] data, ref StreamReader reader)
        {
            Record r = new Record((UserControl)UC);
            r.id = Convert.ToInt32(data[1]);
            string S = reader.ReadLine();
            string[] data1;
            while (S != null)
            {
                data1 = S.Split(',');
                if (data1[0] == entry_radar.identifier)
                {
                    r.entry_radar = (Radar)entry_radar.get_entity(data1, ref reader);
                    break;
                }
                S = reader.ReadLine();
            }

            while (S != null)
            {
                S = reader.ReadLine();
                data1 = S.Split(',');
                if (data1[0] == exit_radar.identifier)
                {
                    r.exit_radar = (Radar)exit_radar.get_entity(data1, ref reader);
                    break;
                }
                S = reader.ReadLine();
            }
            return r;
        }

        public override Identifier get_Identifier()
        {
            RecordId re_id = new RecordId();
            return re_id;
        }

        public override bool Match(MVCEntity entity_list, MVCEntity entity)
        {
            Record re_list = (Record)entity_list;
            Record curr_re = (Record)entity;

            if (re_list.id == curr_re.id)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            Record po = this;

            string s = "REC" + "," + po.id + "," + "\n" + po.entry_radar.ToString();
            s += po.exit_radar.ToString();

            return s;
        }
    }
}
