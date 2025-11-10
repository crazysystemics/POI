using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for UC_Radar_Info.xaml
    ///</summary>
    public partial class UC_Radar_Info : UserControl, Entity_UC
    {
        List<UserControl> UC_list_pri = new List<UserControl>();
        List<UserControl> UC_list_freq = new List<UserControl>();
        Entity_UC UC_curr_pri;
        Entity_UC UC_curr_freq;

        public UC_Radar_Info()
        {
            InitializeComponent();

            UC_list_pri.Add(UC_PRI_F);
            UC_list_pri.Add(UC_PRI_S);
            UC_list_pri.Add(UC_PRI_J);
            UC_list_pri.Add(UC_PRI_SW);

            foreach (UserControl UC in UC_list_pri)
                UC.Visibility = Visibility.Hidden;

            UC_curr_pri = (Entity_UC)UC_PRI_F;
            ((UserControl)UC_curr_pri).Visibility = Visibility.Visible;

            UC_list_freq.Add(UC_Freq_P);
            UC_list_freq.Add(UC_Freq_B);

            foreach (UserControl UC in UC_list_freq)
                UC.Visibility = Visibility.Hidden;

            UC_curr_freq = (Entity_UC)UC_Freq_P;
            ((UserControl)UC_curr_freq).Visibility = Visibility.Visible;

        }

        public void Clear_all()
        {
            txt_RadarID.Clear();
            txt_PWmin.Clear();
            txt_PWmax.Clear();
            cmd_Select.IsSelected = true;
            cmd_FreqSelect.IsSelected = true;
            cmd_Select.IsSelected = true;

            UC_curr_pri.Clear_all();
            UC_curr_freq.Clear_all();
        }

        public void display(MVCEntity data)
        {
            Radar r = (Radar)data;
            txt_RadarID.Text = Convert.ToString(r.id);
            txt_PWmin.Text = Convert.ToString(r.pw_min);
            txt_PWmax.Text = Convert.ToString(r.pw_max);
            if (r.priagility == PRI_Agility.FIXED)
                cmb_Fixed.IsSelected = true;
            if (r.priagility == PRI_Agility.STAGGERED)
                cmb_Staggered.IsSelected = true;
            if (r.priagility == PRI_Agility.JITTERED)
                cmb_Jittered.IsSelected = true;
            if (r.priagility == PRI_Agility.SWITCHER)
                cmb_Switched.IsSelected = true;


            if (r.frequency_agility == Freq_Agility.BATCH_TO_BATCH)
                cmd_BatchToBatch.IsSelected = true;
            if (r.frequency_agility == Freq_Agility.PULSE_TO_PULSE)
                cmd_PulseToPulse.IsSelected = true;

            UC_curr_pri.display(r);
            UC_curr_freq.display(r);
        }

        public void Enable()
        {
            txt_RadarID.IsEnabled = true;
            txt_PWmin.IsEnabled = true;
            txt_PWmax.IsEnabled = true;
            cmb_FreqAgility.IsEnabled = true;
            cmb_PRIAgility.IsEnabled = true;

            //UC_curr_pri.Enable();
        }

        public MVCEntity get_user_input_entity()
        {
            if (txt_RadarID.Text == "")
            {
                return new Radar(this);
            }

            Radar r = (Radar)UC_curr_pri.get_user_input_entity();
            Radar r1 = (Radar)UC_curr_freq.get_user_input_entity();
            r.freq_agility = r1.freq_agility;
            r.frequency_agility = r1.frequency_agility;
            r.frequency_max = r1.frequency_max;
            r.frequency_min = r1.frequency_min;

            r.id = Convert.ToInt32(txt_RadarID.Text);
            r.side = Side.BLUE;
            if (txt_PWmin.Text != "")
                r.pw_min = Convert.ToDouble(txt_PWmin.Text);
            if (txt_PWmin.Text != "")
                r.pw_max = Convert.ToDouble(txt_PWmax.Text);

            return r;
        }

        private void Cmb_Fixed_Selected(object sender, RoutedEventArgs e)
        {
            foreach (UserControl UC in UC_list_pri)
                UC.Visibility = Visibility.Hidden;

            UC_curr_pri = (Entity_UC)UC_PRI_F;
            ((UserControl)UC_curr_pri).Visibility = Visibility.Visible;
            UC_curr_pri.Clear_all();
        }

        private void Cmb_Staggered_Selected(object sender, RoutedEventArgs e)
        {
            foreach (UserControl UC in UC_list_pri)
                UC.Visibility = Visibility.Hidden;

            UC_curr_pri = (Entity_UC)UC_PRI_S;
            ((UserControl)UC_curr_pri).Visibility = Visibility.Visible;
            UC_curr_pri.Clear_all();
        }

        private void Cmb_Jittered_Selected(object sender, RoutedEventArgs e)
        {
            foreach (UserControl UC in UC_list_pri)
                UC.Visibility = Visibility.Hidden;

            UC_curr_pri = (Entity_UC)UC_PRI_J;
            ((UserControl)UC_curr_pri).Visibility = Visibility.Visible;
            UC_curr_pri.Clear_all();
        }

        private void Cmb_Switched_Selected(object sender, RoutedEventArgs e)
        {
            foreach (UserControl UC in UC_list_pri)
                UC.Visibility = Visibility.Hidden;

            UC_curr_pri = (Entity_UC)UC_PRI_SW;
            ((UserControl)UC_curr_pri).Visibility = Visibility.Visible;
            UC_curr_pri.Clear_all();
        }

        private void Cmd_PulseToPulse_Selected(object sender, RoutedEventArgs e)
        {
            foreach (UserControl UC in UC_list_freq)
                UC.Visibility = Visibility.Hidden;

            UC_curr_freq = (Entity_UC)UC_Freq_P;
            ((UserControl)UC_curr_freq).Visibility = Visibility.Visible;
            UC_curr_freq.Clear_all();
        }

        private void Cmd_BatchToBatch_Selected(object sender, RoutedEventArgs e)
        {
            foreach (UserControl UC in UC_list_freq)
                UC.Visibility = Visibility.Hidden;

            UC_curr_freq = (Entity_UC)UC_Freq_B;
            ((UserControl)UC_curr_freq).Visibility = Visibility.Visible;
            UC_curr_freq.Clear_all();
        }
    }
}
