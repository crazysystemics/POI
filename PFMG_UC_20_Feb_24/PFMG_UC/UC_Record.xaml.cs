using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PFMG_UC
{
    ///<summary>
    /// Interaction logic for UC_Record.xaml
    ///</summary>
    public partial class UC_Record : UserControl, Entity_UC
    {
        List<UserControl> UC_list = new List<UserControl>();

        public UC_Record()
        {
            InitializeComponent();
            UC_list.Add(UC_REntry);
            UC_list.Add(UC_RExit);
        }

        public void Clear_all()
        {
            foreach (UserControl UC in UC_list)
            {
                ((Entity_UC)UC).Clear_all();
            }
        }

        public void display(MVCEntity data)
        {
            Record r = (Record)data;
            txt_RecordID.Text = r.id.ToString();
            ((Entity_UC)UC_REntry).display(r.entry_radar);
            ((Entity_UC)UC_RExit).display(r.exit_radar);
        }

        public void Enable()
        {
            foreach (UserControl UC in UC_list)
            {
                ((Entity_UC)UC).Enable();
            }
        }

        public MVCEntity get_user_input_entity()
        {
            if (this.txt_RecordID.Text == "")
            {
                return new Record(this);
            }

            Radar ren = (Radar)((Entity_UC)UC_REntry).get_user_input_entity();
            Radar rex = (Radar)((Entity_UC)UC_RExit).get_user_input_entity();
            Record re = new Record(this, Convert.ToInt32(txt_RecordID.Text), ren, rex);

            return re;
        }
    }
}
