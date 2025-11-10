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
    /// Interaction logic for UC_FreqBatchToBatch.xaml
    ///</summary>
    public partial class UC_FreqBatchToBatch : UserControl, Entity_UC
    {
        Batch_to_Batch b;

        public UC_FreqBatchToBatch()
        {
            InitializeComponent();
        }
        public void Clear_all()
        {
            txt_Freqmin.Clear();
            txt_Freqmax.Clear();

            txt_NoBatches.Clear();
            txt_Batch1.Clear();
            txt_NoPulse1.Clear();
            txt_Freq1.Clear();
            txt_Batch2.Clear();
            txt_NoPulse2.Clear();
            txt_Freq2.Clear();
            txt_Batch3.Clear();
            txt_NoPulse3.Clear();
            txt_Freq3.Clear();
            txt_Batch4.Clear();
            txt_NoPulse4.Clear();
            txt_Freq4.Clear();
            txt_Batch5.Clear();
            txt_NoPulse5.Clear();
            txt_Freq5.Clear();

            rbtn_hopper.IsChecked = false;
            rbtn_slider.IsChecked = false;
        }

        public void display(MVCEntity data)
        {
            this.Visibility = Visibility.Visible;
            Radar r = (Radar)data;

            txt_Freqmin.Text = Convert.ToString(r.frequency_min);
            txt_Freqmax.Text = Convert.ToString(r.frequency_max);

            b = (Batch_to_Batch)r.freq_agility;
            txt_NoBatches.Text = Convert.ToString(b.no_of_batches);
            txt_Batch1.Text = Convert.ToString(1);
            txt_Batch2.Text = Convert.ToString(2);
            txt_Batch3.Text = Convert.ToString(3);
            txt_Batch4.Text = Convert.ToString(4);
            txt_Batch5.Text = Convert.ToString(5);
            txt_NoPulse1.Text = Convert.ToString(b.batches[0].no_of_pulses);
            txt_NoPulse2.Text = Convert.ToString(b.batches[1].no_of_pulses);
            txt_NoPulse3.Text = Convert.ToString(b.batches[2].no_of_pulses);
            txt_NoPulse4.Text = Convert.ToString(b.batches[3].no_of_pulses);
            txt_NoPulse5.Text = Convert.ToString(b.batches[4].no_of_pulses);
            txt_Freq1.Text = Convert.ToString(b.batches[0].Freq);
            txt_Freq2.Text = Convert.ToString(b.batches[1].Freq);
            txt_Freq3.Text = Convert.ToString(b.batches[2].Freq);
            txt_Freq4.Text = Convert.ToString(b.batches[3].Freq);
            txt_Freq5.Text = Convert.ToString(b.batches[4].Freq);

            if (b.traversal_order == false)
                rbtn_slider.IsChecked = true;
            else
                rbtn_hopper.IsChecked = true;
        }

        public void Enable()
        {
            txt_Freqmin.IsEnabled = true;
            txt_Freqmax.IsEnabled = true;

            txt_NoBatches.IsEnabled = true;
            txt_Batch1.IsEnabled = true;
            txt_Batch2.IsEnabled = true;
            txt_Batch3.IsEnabled = true;
            txt_Batch4.IsEnabled = true;
            txt_Batch5.IsEnabled = true;
            txt_NoPulse1.IsEnabled = true;
            txt_NoPulse2.IsEnabled = true;
            txt_NoPulse3.IsEnabled = true;
            txt_NoPulse4.IsEnabled = true;
            txt_NoPulse5.IsEnabled = true;
            txt_Freq1.IsEnabled = true;
            txt_Freq2.IsEnabled = true;
            txt_Freq3.IsEnabled = true;
            txt_Freq4.IsEnabled = true;
            txt_Freq5.IsEnabled = true;
        }

        public MVCEntity get_user_input_entity()
        {
            if (this.txt_NoBatches.Text == "")
            {
                return new Radar(new UC_Radar_Info());
            }

            if (txt_NoBatches.Text == "") txt_NoBatches.Text = "0";
            if (txt_Batch1.Text == "") txt_Batch1.Text = "0";
            if (txt_Batch2.Text == "") txt_Batch2.Text = "0";
            if (txt_Batch3.Text == "") txt_Batch3.Text = "0";
            if (txt_Batch4.Text == "") txt_Batch4.Text = "0";
            if (txt_Batch5.Text == "") txt_Batch5.Text = "0";
            if (txt_NoPulse1.Text == "") txt_NoPulse1.Text = "0";
            if (txt_NoPulse2.Text == "") txt_NoPulse2.Text = "0";
            if (txt_NoPulse3.Text == "") txt_NoPulse3.Text = "0";
            if (txt_NoPulse4.Text == "") txt_NoPulse4.Text = "0";
            if (txt_NoPulse5.Text == "") txt_NoPulse5.Text = "0";
            if (txt_Freq1.Text == "") txt_Freq1.Text = "0";
            if (txt_Freq2.Text == "") txt_Freq2.Text = "0";
            if (txt_Freq3.Text == "") txt_Freq3.Text = "0";
            if (txt_Freq4.Text == "") txt_Freq4.Text = "0";
            if (txt_Freq5.Text == "") txt_Freq5.Text = "0";

            Batch_to_Batch.Batch[] batches = new Batch_to_Batch.Batch[] {
            new Batch_to_Batch.Batch(Convert.ToInt32(txt_Batch1.Text), Convert.ToInt32(txt_NoPulse1.Text), Convert.ToDouble(txt_Freq1.Text)),
            new Batch_to_Batch.Batch(Convert.ToInt32(txt_Batch2.Text), Convert.ToInt32(txt_NoPulse2.Text), Convert.ToDouble(txt_Freq2.Text)),
            new Batch_to_Batch.Batch(Convert.ToInt32(txt_Batch3.Text), Convert.ToInt32(txt_NoPulse3.Text), Convert.ToDouble(txt_Freq3.Text)),
            new Batch_to_Batch.Batch(Convert.ToInt32(txt_Batch4.Text), Convert.ToInt32(txt_NoPulse4.Text), Convert.ToDouble(txt_Freq4.Text)),
            new Batch_to_Batch.Batch(Convert.ToInt32(txt_Batch5.Text), Convert.ToInt32(txt_NoPulse5.Text), Convert.ToDouble(txt_Freq5.Text)) };

            Radar radar = new Radar(new UC_Radar_Info(), 0, Side.BLUE, null, new Batch_to_Batch(), 0.0, 0.0,
                                 0.0, 0.0, Convert.ToDouble(txt_Freqmin.Text), Convert.ToDouble(txt_Freqmax.Text), 0);

            Traversal_Order tr = 0;
            if (rbtn_slider.IsChecked == true)
                tr = Traversal_Order.SLIDER;
            if (rbtn_slider.IsChecked == true)
                tr = Traversal_Order.HOPPER;

            Batch_to_Batch bt = new Batch_to_Batch(Convert.ToInt32(txt_NoBatches.Text), batches, tr);
            string[] data = null;

            radar.freq_agility = bt.Set(ref radar.freq_agility, data);
            radar.frequency_agility = Freq_Agility.BATCH_TO_BATCH;

            return radar;
        }
    }
}