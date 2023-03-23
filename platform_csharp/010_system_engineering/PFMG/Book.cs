using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PFMG
{
    public class Book : MVCEntity
    {
        public int id;
        public string name;
        public string identifier;

        public Book(UserControl puc)
        {
            UC = (Entity_UC)puc;
        }
        public Book(UserControl puc, int pid, string pname = null)
        {
            UC = (Entity_UC)puc;   
            this.id = pid;
            this.name = pname;
            identifier = "B";
        }

        public override bool Match(MVCEntity list_entity, MVCEntity e)
        {
            if(((Book)list_entity).id == ((Book)e).id)
            {
                return true;
            }
            return false;
        }

        public override MVCEntity get_input_file_entity(string[] data)
        {
            return new Book(null, Convert.ToInt32(data[1]), data[2]);
        }

        public override string ToString()
        {
            string s = identifier + "," + id + "," + name;
            return s;
        }
    }
}
