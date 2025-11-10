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
    /// Interaction logic for UC_FreqPulseToPulse.xaml
    ///</summary>
    public partial class UC_FreqPulseToPulse : UserControl, Entity_UC
    {
        Pulse_to_Pulse p;
        public UC_FreqPulseToPulse()
        {
            InitializeComponent();
        }
        public void Clear_all()
        {
            txt_Freqmin.Clear();
            txt_Freqmax.Clear();
            txt_Percent.Clear();
        }

        public void display(MVCEntity data)
        {
            this.Visibility = Visibility.Visible;
            Radar r = (Radar)data;

            txt_Freqmin.Text = Convert.ToString(r.frequency_min);
            txt_Freqmax.Text = Convert.ToString(r.frequency_max);

            p = (Pulse_to_Pulse)r.freq_agility;
            txt_Percent.Text = Convert.ToString(p.percentage);
        }

        public void Enable()
        {
            txt_Freqmin.IsEnabled = true;
            txt_Freqmax.IsEnabled = true;

            txt_Percent.IsEnabled = true;
        }

        public MVCEntity get_user_input_entity()
        {
            if (this.txt_Percent.Text == "")
            {
                return new Radar(new UC_Radar_Info());
            }

            Radar radar = new Radar(new UC_Radar_Info(), 0, Side.BLUE, null, new Pulse_to_Pulse(), 0, 0,
                                 0.0, 0.0, Convert.ToDouble(txt_Freqmin.Text), Convert.ToDouble(txt_Freqmax.Text), 0);

            string[] data = { "0", txt_Percent.Text };
            p = new Pulse_to_Pulse();
            radar.frequency_agility = Freq_Agility.PULSE_TO_PULSE;
            p.Set(ref radar.freq_agility, data);

            return radar;
        }
    }
}
