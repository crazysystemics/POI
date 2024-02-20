using System;
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
    /// Interaction logic for UC_PrideOctave.xaml
    ///</summary>
    public partial class UC_PrideOctave : UserControl, Entity_UC
    {
        public UC_PrideOctave()
        {
            InitializeComponent();
        }

        public void Clear_all()
        {
            txt_PrideOctaveID.Clear();
            txtbox_50_100.IsChecked = false;
            txtbox_100_200.IsChecked = false;
            txtbox_200_400.IsChecked = false;
            txtbox_400_500.IsChecked = false;
            txtbox_500_1600.IsChecked = false;
            txtbox_1600_3200.IsChecked = false;

            cmd_Select.IsSelected = true;
        }

        public void display(MVCEntity data)
        {
            Clear_all();
            PrideOctave po = (PrideOctave)data;
            txt_PrideOctaveID.Text = po.octave_id.ToString();

            if (po.band == Bands.L) cmd_L.IsSelected = true;
            if (po.band == Bands.A) cmd_A.IsSelected = true;
            if (po.band == Bands.B) cmd_B.IsSelected = true;
            if (po.band == Bands.C) cmd_C.IsSelected = true;
            if (po.band == Bands.D) cmd_D.IsSelected = true;


            if (po.octave_range[0].octave_min == 50) txtbox_50_100.IsChecked = true;
            if (po.octave_range[1].octave_min == 100) txtbox_100_200.IsChecked = true;
            if (po.octave_range[2].octave_min == 200) txtbox_200_400.IsChecked = true;
            if (po.octave_range[3].octave_min == 400) txtbox_400_500.IsChecked = true;
            if (po.octave_range[4].octave_min == 500) txtbox_500_1600.IsChecked = true;
            if (po.octave_range[5].octave_min == 1600) txtbox_1600_3200.IsChecked = true;

        }

        public void Enable()
        {
            txt_PrideOctaveID.IsEnabled = true;
            txtbox_50_100.IsEnabled = true;
            txtbox_100_200.IsEnabled = true;
            txtbox_200_400.IsEnabled = true;
            txtbox_400_500.IsEnabled = true;
            txtbox_500_1600.IsEnabled = true;
            txtbox_1600_3200.IsEnabled = true;
        }

        public MVCEntity get_user_input_entity()
        {
            if (this.txt_PrideOctaveID.Text == "")
            {
                return new PrideOctave(this);
            }

            PrideOctave po = new PrideOctave(this);
            po.octave_id = Convert.ToInt32(txt_PrideOctaveID.Text);

            if (cmd_L.IsSelected == true) po.band = Bands.L;
            if (cmd_A.IsSelected == true) po.band = Bands.A;
            if (cmd_B.IsSelected == true) po.band = Bands.B;
            if (cmd_C.IsSelected == true) po.band = Bands.C;
            if (cmd_D.IsSelected == true) po.band = Bands.D;

            int i = 0;
            if (txtbox_50_100.IsChecked == true) { po.octave_range[i].octave_min = 50; po.octave_range[i].octave_max = 100; i++; }
            if (txtbox_100_200.IsChecked == true) { po.octave_range[i].octave_min = 100; po.octave_range[i].octave_max = 200; i++; }
            if (txtbox_200_400.IsChecked == true) { po.octave_range[i].octave_min = 200; po.octave_range[i].octave_max = 400; i++; }
            if (txtbox_400_500.IsChecked == true) { po.octave_range[i].octave_min = 400; po.octave_range[i].octave_max = 500; i++; }
            if (txtbox_500_1600.IsChecked == true) { po.octave_range[i].octave_min = 500; po.octave_range[i].octave_max = 1600; i++; }
            if (txtbox_1600_3200.IsChecked == true) { po.octave_range[i].octave_min = 1600; po.octave_range[i].octave_max = 3200; }

            return po;
        }
    }
}
