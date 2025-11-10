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
    /// Interaction logic for UC_PRISwitcher.xaml
    ///</summary>
    public partial class UC_PRISwitcher : UserControl, Entity_UC
    {
        Switcher s;
        public UC_PRISwitcher()
        {
            InitializeComponent();
        }

        public void Clear_all()
        {
            txt_PRImin.Clear();
            txt_PRImax.Clear();
            txt_Tolerance.Clear();

            txt_NoBatches.Clear();
            txt_Batch1.Clear();
            txt_NoPulse1.Clear();
            txt_PRI1.Clear();
            txt_Batch2.Clear();
            txt_NoPulse2.Clear();
            txt_PRI2.Clear();
            txt_Batch3.Clear();
            txt_NoPulse3.Clear();
            txt_PRI3.Clear();
            txt_Batch4.Clear();
            txt_NoPulse4.Clear();
            txt_PRI4.Clear();
            txt_Batch5.Clear();
            txt_NoPulse5.Clear();
            txt_PRI5.Clear();

            rbtn_hopper.IsChecked = false;
            rbtn_slider.IsChecked = false;
        }

        public void display(MVCEntity data)
        {
            this.Visibility = Visibility.Visible;
            Radar r = (Radar)data;

            txt_PRImin.Text = Convert.ToString(r.pri_min);
            txt_PRImax.Text = Convert.ToString(r.pri_max);
            txt_Tolerance.Text = Convert.ToString(r.tolerance);

            s = (Switcher)r.pri_agility1;
            txt_NoBatches.Text = Convert.ToString(s.no_of_batches);
            txt_Batch1.Text = Convert.ToString(1);
            txt_Batch2.Text = Convert.ToString(2);
            txt_Batch3.Text = Convert.ToString(3);
            txt_Batch4.Text = Convert.ToString(4);
            txt_Batch5.Text = Convert.ToString(5);
            txt_NoPulse1.Text = Convert.ToString(s.batches[0].no_of_pulses);
            txt_NoPulse2.Text = Convert.ToString(s.batches[1].no_of_pulses);
            txt_NoPulse3.Text = Convert.ToString(s.batches[2].no_of_pulses);
            txt_NoPulse4.Text = Convert.ToString(s.batches[3].no_of_pulses);
            txt_NoPulse5.Text = Convert.ToString(s.batches[4].no_of_pulses);
            txt_PRI1.Text = Convert.ToString(s.batches[0].pri);
            txt_PRI2.Text = Convert.ToString(s.batches[1].pri);
            txt_PRI3.Text = Convert.ToString(s.batches[2].pri);
            txt_PRI4.Text = Convert.ToString(s.batches[3].pri);
            txt_PRI5.Text = Convert.ToString(s.batches[4].pri);

            if (s.traversal_order == false)
                rbtn_slider.IsChecked = true;
            else
                rbtn_hopper.IsChecked = true;
        }

        public void Enable()
        {
            txt_PRImin.IsEnabled = true;
            txt_PRImax.IsEnabled = true;
            txt_Tolerance.IsEnabled = true;

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
            txt_PRI1.IsEnabled = true;
            txt_PRI2.IsEnabled = true;
            txt_PRI3.IsEnabled = true;
            txt_PRI4.IsEnabled = true;
            txt_PRI5.IsEnabled = true;
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
            if (txt_PRI1.Text == "") txt_PRI1.Text = "0";
            if (txt_PRI2.Text == "") txt_PRI2.Text = "0";
            if (txt_PRI3.Text == "") txt_PRI3.Text = "0";
            if (txt_PRI4.Text == "") txt_PRI4.Text = "0";
            if (txt_PRI5.Text == "") txt_PRI5.Text = "0";

            Batch[] batches = new Batch[] {new Batch(Convert.ToInt32(txt_Batch1.Text), Convert.ToInt32(txt_NoPulse1.Text), Convert.ToDouble(txt_PRI1.Text)),
            new Batch(Convert.ToInt32(txt_Batch2.Text), Convert.ToInt32(txt_NoPulse2.Text), Convert.ToDouble(txt_PRI2.Text)),
            new Batch(Convert.ToInt32(txt_Batch3.Text), Convert.ToInt32(txt_NoPulse3.Text), Convert.ToDouble(txt_PRI3.Text)),
            new Batch(Convert.ToInt32(txt_Batch4.Text), Convert.ToInt32(txt_NoPulse4.Text), Convert.ToDouble(txt_PRI4.Text)),
            new Batch(Convert.ToInt32(txt_Batch5.Text), Convert.ToInt32(txt_NoPulse5.Text), Convert.ToDouble(txt_PRI5.Text)) };

            Radar radar = new Radar(new UC_Radar_Info(), 0, Side.BLUE, new Switcher(), null, Convert.ToDouble(txt_PRImin.Text), Convert.ToDouble(txt_PRImax.Text),
                                 0.0, 0.0, 0.0, 0.0, Convert.ToDouble(txt_Tolerance.Text));

            Traversal_Order tr = 0;
            if (rbtn_slider.IsChecked == true)
                tr = Traversal_Order.SLIDER;
            if (rbtn_slider.IsChecked == true)
                tr = Traversal_Order.HOPPER;

            Switcher switcher = new Switcher(Convert.ToInt32(txt_NoBatches.Text), batches, tr);
            string[] data = null;

            radar.pri_agility1 = switcher.Set(ref radar.pri_agility1, data);
            radar.priagility = PRI_Agility.SWITCHER;

            return radar;
        }
    }
}
