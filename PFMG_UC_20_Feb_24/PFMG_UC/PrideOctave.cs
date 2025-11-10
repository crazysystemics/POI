using PFMG_UC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static PFMG_UC.MVCEntity;
using System.Windows.Markup;

namespace PFMG_UC
{
    public class OctaveRange
    {
        public int octave_min;
        public int octave_max;

        public OctaveRange(int poctave_min, int poctave_max)
        {
            octave_min = poctave_min;
            octave_max = poctave_max;
        }
    }
    public class PrideOctave : MVCEntity
    {
        public Bands band;
        public string identifier;
        public int octave_id;
        public OctaveRange[] octave_range;
        public bool valid;

        class PrideOctaveId : Identifier
        {
            public PrideOctaveId()
            {
                id = "PO";
                type = "PrideOctave";
            }
        }

        class PrideOctaveHeader : Header
        {
            string entity_no;
            string entity_type;
            int count;

            public PrideOctaveHeader(int pcount, PrideOctave po)
            {
                entity_no = "E3";
                entity_type = "PrideOctave";
                count = pcount;
                header = "Entity ID" + "," + entity_no + "\n" + "Entity Name" + "," + entity_type + "\n" + "Entity Count" + "," + count + "\n\n";
                header += "Identifier" + "," + "PrideOctave ID" + "," + "Band";
            }
        }

        public PrideOctave(UserControl puc)
        {
            identifier = "PO";
            this.UC = (Entity_UC)puc;
            octave_range = new OctaveRange[6];
        }

        public PrideOctave(UserControl puc, int poctave_id, OctaveRange[] poctave_range)
        {
            octave_id = poctave_id;
            octave_range = poctave_range;

            identifier = "PO";
            this.UC = (Entity_UC)puc;
        }
        public override Header GetHeader(int count)
        {
            PrideOctaveHeader po_header = new PrideOctaveHeader(count, this);
            return po_header;
        }

        public override MVCEntity get_entity(string[] data, ref StreamReader reader)
        {
            reader.ReadLine();
            reader.ReadLine();
            string S;
            string[] data1 = null;

            OctaveRange[] octave_range = new OctaveRange[6];

            for (int i = 0; i < 6; i++)
            {
                S = reader.ReadLine();
                data1 = S.Split(',');
                octave_range[i] = new OctaveRange(Convert.ToInt32(data1[0]), Convert.ToInt32(data1[1]));
            }
            PrideOctave po = new PrideOctave((UserControl)UC, Convert.ToInt32(data[1]), octave_range);
            if (data[2] == "L") po.band = Bands.L;
            if (data[2] == "A") po.band = Bands.A;
            if (data[2] == "B") po.band = Bands.B;
            if (data[2] == "C") po.band = Bands.C;
            if (data[2] == "D") po.band = Bands.D;
            return po;
        }

        public override Identifier get_Identifier()
        {
            PrideOctaveId po_id = new PrideOctaveId();
            return po_id;
        }

        public override bool Match(MVCEntity entity_list, MVCEntity entity)
        {
            PrideOctave po_list = (PrideOctave)entity_list;
            PrideOctave curr_po = (PrideOctave)entity;

            if (po_list.octave_id == curr_po.octave_id)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            PrideOctave po = this;

            string s = "PO" + "," + po.octave_id + "," + po.band;
            s += "\n" + "OCTAVE RANGE" + "\n" + "Octave min" + "," + "Octave max";

            for (int i = 0; i < 6; i++)
            {
                s += "\n" + po.octave_range[i].octave_min + "," + po.octave_range[i].octave_max;
            }
            s += "\n";
            return s;
        }
    }
}

