using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /// Interaction logic for UC_SearchRegime.xaml
    ///</summary>
    public partial class UC_SearchRegime : UserControl, Entity_UC
    {
        ObservableCollection<PulseAmplitudeRecord> pa_data { get; set; }
        public UC_SearchRegime()
        {
            InitializeComponent();
            pa_data = new ObservableCollection<PulseAmplitudeRecord>();
        }

        public void Clear_all()
        {
            txt_SearchRegID.Clear();

            rbtn_L.IsChecked = false;
            rbtn_A.IsChecked = false;
            rbtn_B.IsChecked = false;
            rbtn_C.IsChecked = false;
            rbtn_D.IsChecked = false;

            txt_ReceptionTime.Clear();
            pa_data.Clear();
            for (int i = 0; i < 64; i++)
            {
                pa_data.Add(new PulseAmplitudeRecord(i + 1, 0, 0));
            }
            dg_pulse_amplitude_table.DataContext = pa_data;

            txt_PRImax1.Clear();
            txt_PRImax2.Clear();
            txt_PRImax3.Clear();
            txt_PRImax4.Clear();
            txt_PRImax5.Clear();

            txt_PRImin1.Clear();
            txt_PRImin2.Clear();
            txt_PRImin3.Clear();
            txt_PRImin4.Clear();
            txt_PRImin5.Clear();

            txt_Freqmax1.Clear();
            txt_Freqmax2.Clear();
            txt_Freqmax3.Clear();
            txt_Freqmax4.Clear();
            txt_Freqmax5.Clear();

            txt_Freqmin1.Clear();
            txt_Freqmin2.Clear();
            txt_Freqmin3.Clear();
            txt_Freqmin4.Clear();
            txt_Freqmin5.Clear();

            txt_Sec1.Clear();
            txt_Sec2.Clear();
            txt_Sec3.Clear();
            txt_Sec4.Clear();
            txt_Sec5.Clear();

            txt_ReceptionTime.Clear();
        }

        public void display(MVCEntity data)
        {
            SearchRegime sr = (SearchRegime)data;
            txt_SearchRegID.Text = sr.id.ToString();
            if (sr.band == Bands.L) rbtn_L.IsChecked = true;
            if (sr.band == Bands.A) rbtn_A.IsChecked = true;
            if (sr.band == Bands.B) rbtn_B.IsChecked = true;
            if (sr.band == Bands.C) rbtn_C.IsChecked = true;
            if (sr.band == Bands.D) rbtn_D.IsChecked = true;

            txt_ReceptionTime.Text = sr.reception_time.ToString();

            for (int i = 0; i < 64; i++)
            {
                pa_data.Add(new PulseAmplitudeRecord(sr.pulse_amplitude_table[i].sector_num, sr.pulse_amplitude_table[i].no_of_pulses, sr.pulse_amplitude_table[i].min_amplitude));
            }
            dg_pulse_amplitude_table.DataContext = pa_data;

            txt_PRImin1.Text = sr.pri_tracker[0].pri_min.ToString();
            txt_PRImax1.Text = sr.pri_tracker[0].pri_max.ToString();
            txt_PRImin2.Text = sr.pri_tracker[1].pri_min.ToString();
            txt_PRImax2.Text = sr.pri_tracker[1].pri_max.ToString();
            txt_PRImin3.Text = sr.pri_tracker[2].pri_min.ToString();
            txt_PRImax3.Text = sr.pri_tracker[2].pri_max.ToString();
            txt_PRImin4.Text = sr.pri_tracker[3].pri_min.ToString();
            txt_PRImax4.Text = sr.pri_tracker[3].pri_max.ToString();
            txt_PRImin5.Text = sr.pri_tracker[4].pri_min.ToString();
            txt_PRImax5.Text = sr.pri_tracker[4].pri_max.ToString();

            txt_Freqmin1.Text = sr.freq_tracker[0].freq_min.ToString();
            txt_Freqmax1.Text = sr.freq_tracker[0].freq_max.ToString();
            txt_Freqmin2.Text = sr.freq_tracker[1].freq_min.ToString();
            txt_Freqmax2.Text = sr.freq_tracker[1].freq_max.ToString();
            txt_Freqmin3.Text = sr.freq_tracker[2].freq_min.ToString();
            txt_Freqmax3.Text = sr.freq_tracker[2].freq_max.ToString();
            txt_Freqmin4.Text = sr.freq_tracker[3].freq_min.ToString();
            txt_Freqmax4.Text = sr.freq_tracker[3].freq_max.ToString();
            txt_Freqmin5.Text = sr.freq_tracker[4].freq_min.ToString();
            txt_Freqmax5.Text = sr.freq_tracker[4].freq_max.ToString();

            txt_Sec1.Text = sr.sector_num[0].ToString();
            txt_Sec2.Text = sr.sector_num[1].ToString();
            txt_Sec3.Text = sr.sector_num[2].ToString();
            txt_Sec4.Text = sr.sector_num[3].ToString();
            txt_Sec5.Text = sr.sector_num[4].ToString();
        }

        public void Enable()
        {
            txt_SearchRegID.IsEnabled = true;

            rbtn_L.IsEnabled = true;
            rbtn_A.IsEnabled = true;
            rbtn_B.IsEnabled = true;
            rbtn_C.IsEnabled = true;
            rbtn_D.IsEnabled = true;

            txt_ReceptionTime.IsEnabled = true;

            txt_PRImax1.IsEnabled = true;
            txt_PRImax2.IsEnabled = true;
            txt_PRImax3.IsEnabled = true;
            txt_PRImax4.IsEnabled = true;
            txt_PRImax5.IsEnabled = true;

            txt_PRImin1.IsEnabled = true;
            txt_PRImin2.IsEnabled = true;
            txt_PRImin3.IsEnabled = true;
            txt_PRImin4.IsEnabled = true;
            txt_PRImin5.IsEnabled = true;

            txt_Freqmax1.IsEnabled = true;
            txt_Freqmax2.IsEnabled = true;
            txt_Freqmax3.IsEnabled = true;
            txt_Freqmax4.IsEnabled = true;
            txt_Freqmax5.IsEnabled = true;

            txt_Freqmin1.IsEnabled = true;
            txt_Freqmin2.IsEnabled = true;
            txt_Freqmin3.IsEnabled = true;
            txt_Freqmin4.IsEnabled = true;
            txt_Freqmin5.IsEnabled = true;

            txt_Sec1.IsEnabled = true;
            txt_Sec2.IsEnabled = true;
            txt_Sec3.IsEnabled = true;
            txt_Sec4.IsEnabled = true;
            txt_Sec5.IsEnabled = true;

            txt_ReceptionTime.IsEnabled = true;
        }

        public MVCEntity get_user_input_entity()
        {
            if (this.txt_SearchRegID.Text == "")
            {
                return new SearchRegime(this);
            }

            SearchRegime sr = new SearchRegime(this);

            sr.id = Convert.ToInt32(txt_SearchRegID.Text);
            if (rbtn_L.IsChecked == true) sr.band = Bands.L;
            if (rbtn_A.IsChecked == true) sr.band = Bands.A;
            if (rbtn_B.IsChecked == true) sr.band = Bands.B;
            if (rbtn_C.IsChecked == true) sr.band = Bands.C;
            if (rbtn_D.IsChecked == true) sr.band = Bands.D;

            if (txt_ReceptionTime.Text == "") txt_ReceptionTime.Text = "0";

            if (txt_PRImin1.Text == "") txt_PRImin1.Text = "0";
            if (txt_PRImin2.Text == "") txt_PRImin2.Text = "0";
            if (txt_PRImin3.Text == "") txt_PRImin3.Text = "0";
            if (txt_PRImin4.Text == "") txt_PRImin4.Text = "0";
            if (txt_PRImin5.Text == "") txt_PRImin5.Text = "0";
            if (txt_PRImax1.Text == "") txt_PRImax1.Text = "0";
            if (txt_PRImax2.Text == "") txt_PRImax2.Text = "0";
            if (txt_PRImax3.Text == "") txt_PRImax3.Text = "0";
            if (txt_PRImax4.Text == "") txt_PRImax4.Text = "0";
            if (txt_PRImax5.Text == "") txt_PRImax5.Text = "0";

            if (txt_Freqmin1.Text == "") txt_Freqmin1.Text = "0";
            if (txt_Freqmin2.Text == "") txt_Freqmin2.Text = "0";
            if (txt_Freqmin3.Text == "") txt_Freqmin3.Text = "0";
            if (txt_Freqmin4.Text == "") txt_Freqmin4.Text = "0";
            if (txt_Freqmin5.Text == "") txt_Freqmin5.Text = "0";
            if (txt_Freqmax1.Text == "") txt_Freqmax1.Text = "0";
            if (txt_Freqmax2.Text == "") txt_Freqmax2.Text = "0";
            if (txt_Freqmax3.Text == "") txt_Freqmax3.Text = "0";
            if (txt_Freqmax4.Text == "") txt_Freqmax4.Text = "0";
            if (txt_Freqmax5.Text == "") txt_Freqmax5.Text = "0";

            if (txt_Sec1.Text == "") txt_Sec1.Text = "0";
            if (txt_Sec2.Text == "") txt_Sec2.Text = "0";
            if (txt_Sec3.Text == "") txt_Sec3.Text = "0";
            if (txt_Sec4.Text == "") txt_Sec4.Text = "0";
            if (txt_Sec5.Text == "") txt_Sec5.Text = "0";

            sr.reception_time = Convert.ToDouble(txt_ReceptionTime.Text);

            pa_data = (ObservableCollection<PulseAmplitudeRecord>)dg_pulse_amplitude_table.DataContext;
            sr.pulse_amplitude_table = pa_data.ToArray<PulseAmplitudeRecord>();

            sr.pri_tracker[0] = new PRIRecord(Convert.ToDouble(txt_PRImin1.Text), Convert.ToDouble(txt_PRImax1.Text));
            sr.pri_tracker[1] = new PRIRecord(Convert.ToDouble(txt_PRImin2.Text), Convert.ToDouble(txt_PRImax2.Text));
            sr.pri_tracker[2] = new PRIRecord(Convert.ToDouble(txt_PRImin3.Text), Convert.ToDouble(txt_PRImax3.Text));
            sr.pri_tracker[3] = new PRIRecord(Convert.ToDouble(txt_PRImin4.Text), Convert.ToDouble(txt_PRImax4.Text));
            sr.pri_tracker[4] = new PRIRecord(Convert.ToDouble(txt_PRImin5.Text), Convert.ToDouble(txt_PRImax5.Text));

            sr.freq_tracker[0] = new FreqRecord(Convert.ToDouble(txt_Freqmin1.Text), Convert.ToDouble(txt_Freqmax1.Text));
            sr.freq_tracker[1] = new FreqRecord(Convert.ToDouble(txt_Freqmin2.Text), Convert.ToDouble(txt_Freqmax2.Text));
            sr.freq_tracker[2] = new FreqRecord(Convert.ToDouble(txt_Freqmin3.Text), Convert.ToDouble(txt_Freqmax3.Text));
            sr.freq_tracker[3] = new FreqRecord(Convert.ToDouble(txt_Freqmin4.Text), Convert.ToDouble(txt_Freqmax4.Text));
            sr.freq_tracker[4] = new FreqRecord(Convert.ToDouble(txt_Freqmin5.Text), Convert.ToDouble(txt_Freqmax5.Text));

            sr.sector_num[0] = Convert.ToInt32(txt_Sec1.Text);
            sr.sector_num[1] = Convert.ToInt32(txt_Sec2.Text);
            sr.sector_num[2] = Convert.ToInt32(txt_Sec3.Text);
            sr.sector_num[3] = Convert.ToInt32(txt_Sec4.Text);
            sr.sector_num[4] = Convert.ToInt32(txt_Sec5.Text);

            return sr;
        }
    }
}
