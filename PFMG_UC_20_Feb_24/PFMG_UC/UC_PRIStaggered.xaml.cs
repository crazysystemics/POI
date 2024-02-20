using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
    /// Interaction logic for UC_PRIStaggered.xaml
    ///</summary>
    public partial class UC_PRIStaggered : UserControl, Entity_UC
    {
        Staggered s;
        public UC_PRIStaggered()
        {
            InitializeComponent();
        }

        public void Clear_all()
        {
            txt_PRImin.Clear();
            txt_PRImax.Clear();
            txt_Tolerance.Clear();

            txt_NoLevels.Clear();
            txt_SubPRI1_min.Clear();
            txt_SubPRI2_min.Clear();
            txt_SubPRI3_min.Clear();
            txt_SubPRI1_max.Clear();
            txt_SubPRI2_max.Clear();
            txt_SubPRI3_max.Clear();
        }

        public void display(MVCEntity data)
        {
            this.Visibility = Visibility.Visible;
            Radar r = (Radar)data;

            txt_PRImin.Text = Convert.ToString(r.pri_min);
            txt_PRImax.Text = Convert.ToString(r.pri_max);
            txt_Tolerance.Text = Convert.ToString(r.tolerance);

            s = (Staggered)r.pri_agility1;
            txt_NoLevels.Text = Convert.ToString(s.no_of_levels);

            txt_SubPRI1_min.Text = s.sub_pris[0].pri_min.ToString();
            txt_SubPRI2_min.Text = s.sub_pris[1].pri_min.ToString();
            txt_SubPRI3_min.Text = s.sub_pris[2].pri_min.ToString();
            txt_SubPRI1_max.Text = s.sub_pris[0].pri_max.ToString();
            txt_SubPRI2_max.Text = s.sub_pris[1].pri_max.ToString();
            txt_SubPRI3_max.Text = s.sub_pris[2].pri_max.ToString();
        }

        public void Enable()
        {
            txt_PRImin.IsEnabled = true;
            txt_PRImax.IsEnabled = true;
            txt_Tolerance.IsEnabled = true;
            txt_NoLevels.IsEnabled = true;
            txt_SubPRI1_min.IsEnabled = true;
            txt_SubPRI2_min.IsEnabled = true;
            txt_SubPRI3_min.IsEnabled = true;
            txt_SubPRI1_max.IsEnabled = true;
            txt_SubPRI2_max.IsEnabled = true;
            txt_SubPRI3_max.IsEnabled = true;
        }

        public MVCEntity get_user_input_entity()
        {
            if (this.txt_NoLevels.Text == "")
            {
                return new Radar(new UC_Radar_Info());
            }

            if (txt_SubPRI1_min.Text == "") txt_SubPRI1_min.Text = "0";
            if (txt_SubPRI2_min.Text == "") txt_SubPRI2_min.Text = "0";
            if (txt_SubPRI3_min.Text == "") txt_SubPRI3_min.Text = "0";
            if (txt_SubPRI1_max.Text == "") txt_SubPRI1_max.Text = "0";
            if (txt_SubPRI2_max.Text == "") txt_SubPRI2_max.Text = "0";
            if (txt_SubPRI3_max.Text == "") txt_SubPRI3_max.Text = "0";

            Radar radar = new Radar(new UC_Radar_Info(), 0, Side.BLUE, new Staggered(), null, Convert.ToDouble(txt_PRImin.Text),
            Convert.ToDouble(txt_PRImax.Text), 0.0, 0.0, 0.0, 0.0, Convert.ToDouble(txt_Tolerance.Text));

            Sub_PRI[] sbu_pri = new Sub_PRI[]
                    {
            new Sub_PRI(0, Convert.ToDouble(txt_SubPRI1_min.Text), Convert.ToDouble(txt_SubPRI1_max.Text)),
            new Sub_PRI(1, Convert.ToDouble(txt_SubPRI2_min.Text), Convert.ToDouble(txt_SubPRI2_max.Text)),
            new Sub_PRI(2, Convert.ToDouble(txt_SubPRI3_min.Text), Convert.ToDouble(txt_SubPRI3_max.Text)),
                };
            string[] data = null;
            s = new Staggered(Convert.ToInt32(txt_NoLevels.Text), sbu_pri);
            radar.priagility = PRI_Agility.STAGGERED;
            s.Set(ref radar.pri_agility1, data);

            return radar;
        }
    }
}
