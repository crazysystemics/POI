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
    /// Interaction logic for UC_EmitterRecord.xaml
    ///</summary>
    public partial class UC_EmitterRecord : UserControl, Entity_UC
    {
        List<UserControl> UC_list = new List<UserControl>();
        Entity_UC UC_curr;
        public List<MVCEntity> mvc_list;

        public UC_EmitterRecord()
        {
            InitializeComponent();
            UC_list.Add(UC_R);

            foreach (UserControl UC in UC_list)
                UC.Visibility = Visibility.Hidden;

            UC_curr = (Entity_UC)UC_R;
            ((UserControl)UC_curr).Visibility = Visibility.Visible;

        }

        public void Clear_all()
        {
            string s = "Available Radars" + "\n";
            foreach (MVCEntity entity in mvc_list)
            {
                s += ((Radar)entity).id + ",";
            }
            lab_RadarList.Content = s;

            UC_curr.Clear_all();

            txt_EmitterRecordID.Clear();
            txt_FreqTrackWindow.Clear();
            txt_PRITrackWindow.Clear();
            txt_PWTrackWindow.Clear();
            txt_AzimuthTrackWindow.Clear();
            txt_agein.Clear();
            txt_ageout.Clear();
        }

        public void display(MVCEntity data)
        {
            string s = "Available Radars" + "\n";
            foreach (MVCEntity entity in mvc_list)
            {
                s += ((Radar)entity).id + ",";
            }
            lab_RadarList.Content = s;

            EmitterRecord er = (EmitterRecord)data;
            txt_EmitterRecordID.Text = Convert.ToString(er.id);
            txt_FreqTrackWindow.Text = Convert.ToString(er.freq_track_window);
            txt_PRITrackWindow.Text = Convert.ToString(er.pri_track_window);
            txt_PWTrackWindow.Text = Convert.ToString(er.pw_track_window);
            txt_AzimuthTrackWindow.Text = Convert.ToString(er.azimuth_track_window);
            txt_agein.Text = Convert.ToString(er.AGEIN);
            txt_ageout.Text = Convert.ToString(er.AGEOUT);

            UC_curr.display(er);
        }

        public void Enable()
        {
            UC_curr.Enable();

            txt_EmitterRecordID.IsEnabled = true;
            txt_FreqTrackWindow.IsEnabled = true;
            txt_PRITrackWindow.IsEnabled = true;
            txt_PWTrackWindow.IsEnabled = true;
            txt_AzimuthTrackWindow.IsEnabled = true;
            txt_agein.IsEnabled = true;
            txt_ageout.IsEnabled = true;
        }

        public MVCEntity get_user_input_entity()
        {
            if (this.txt_EmitterRecordID.Text == "")
            {
                return new EmitterRecord(this, new Radar(null));
            }

            Radar r = (Radar)UC_curr.get_user_input_entity();
            EmitterRecord er = new EmitterRecord(this, r);


            er.id = Convert.ToInt32(txt_EmitterRecordID.Text);

            if (txt_FreqTrackWindow.Text != "")
                er.freq_track_window = Convert.ToDouble(txt_FreqTrackWindow.Text);
            if (txt_PRITrackWindow.Text != "")
                er.pri_track_window = Convert.ToDouble(txt_PRITrackWindow.Text);
            if (txt_PWTrackWindow.Text != "")
                er.pw_track_window = Convert.ToDouble(txt_PWTrackWindow.Text);
            if (txt_AzimuthTrackWindow.Text != "")
                er.azimuth_track_window = Convert.ToDouble(txt_AzimuthTrackWindow.Text);
            if (txt_agein.Text != "")
                er.AGEIN = Convert.ToInt32(txt_agein.Text);
            if (txt_ageout.Text != "")
                er.AGEOUT = Convert.ToInt32(txt_ageout.Text);

            return er;
        }
        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
            foreach (MVCEntity entity in mvc_list)
            {
                if (UC_R.txt_RadarID.Text == ((Radar)entity).id.ToString())
                {
                    UC_curr.display(entity);
                }
            }
        }
    }
}
