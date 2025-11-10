using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Shapes;

namespace PFMG_UC
{
    /// <summary>
    /// Interaction logic for ViewEmitterTracks.xaml
    /// </summary>
    public partial class ViewEmitterTracks : Window
    {

        public List<EmitterTrackRecord> recordedETRs = new List<EmitterTrackRecord>();
        public int CurrentEmitterTrack;
        ObservableCollection<EmitterTrackRecord> pa_data { get; set; }
        public ViewEmitterTracks()
        {
            InitializeComponent();
            pa_data = new ObservableCollection<EmitterTrackRecord>();
        }

        private void btn_view_emitter_tracks_Click(object sender, RoutedEventArgs e)
        {
            pa_data.Clear();
            cmb_emitter_tracks.Items.Clear();
            recordedETRs.Clear();

            StreamReader reader = new StreamReader("erOutputFile12-Nov-23-03-22-.csv", true);//Globals.recFileName, true);
            string S = reader.ReadLine();
            while (S != null)
            {
                string[] data = S.Split(',');
                EmitterTrackRecord etr = new EmitterTrackRecord();

                etr.Tick = Convert.ToInt32(data[1]);
                etr.erID = Convert.ToInt32(data[3]);
                etr.eid = Convert.ToInt32(data[4]);
                etr.trackID = Convert.ToInt32(data[5]);

                etr.priCurrent = Convert.ToInt32(data[6]);
                etr.priMin = Convert.ToInt32(data[7]);
                etr.priMax = Convert.ToInt32(data[8]);
                etr.priTrackWindow = Convert.ToInt32(data[9]);

                etr.freqCurrent = Convert.ToInt32(data[10]);
                etr.freqMin = Convert.ToInt32(data[11]);
                etr.freqMax = Convert.ToInt32(data[12]);
                etr.freqTrackWindow = Convert.ToInt32(data[13]);

                etr.pwCurrent = Convert.ToInt32(data[14]);
                etr.pwMin = Convert.ToInt32(data[15]);
                etr.pwMax = Convert.ToInt32(data[16]);
                etr.pwTrackWindow = Convert.ToInt32(data[17]);

                //etr.azimuthTrackWindow = Convert.ToInt32(data[18]);
                //etr.ageIn = Convert.ToInt32(data[19]);
                //etr.ageOut = Convert.ToInt32(data[20]);

                etr.received = Convert.ToBoolean(data[2]);
                recordedETRs.Add(etr);

                S = reader.ReadLine();
            }

            int prevRecID = -1;
            foreach (EmitterTrackRecord record in recordedETRs)
            {
                if (record.trackID != prevRecID)
                {
                    pa_data.Add(record);
                    prevRecID = record.trackID;

                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = "Emitter_Track " + record.trackID;
                    comboBoxItem.Selected += ViewEmitterTrackRecoreds;
                    cmb_emitter_tracks.Items.Add(comboBoxItem);
                }
            }
            emitter_track_table.DataContext = pa_data;
        }

        private void ViewEmitterTrackRecoreds(object sender, RoutedEventArgs e)
        {
            pa_data.Clear();
            
            string[] s = ((ComboBoxItem)sender).Content.ToString().Split(' ');
            CurrentEmitterTrack = Convert.ToInt32(s[1]);
            foreach (EmitterTrackRecord record in recordedETRs)
            {
                if (record.trackID == CurrentEmitterTrack)
                {
                    pa_data.Add(record);
                }
            }

            emitter_track_table.DataContext = pa_data;
        }
    }
}
