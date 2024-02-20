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
    /// Interaction logic for UC_Header.xaml
    ///</summary>
    public partial class UC_Header : UserControl
    {
        public UC_Header()
        {
            InitializeComponent();
        }

        public void display(MVCEntity data, int count)
        {
            if (data is Radar) lab_details.Content = "Radar Details";
            if (data is SearchRegime) lab_details.Content = "SearchRegime Details";
            if (data is PrideOctave) lab_details.Content = "PrideOctave Details";
            if (data is EmitterRecord) lab_details.Content = "EmitterRecord Details";
            if (data is Record) lab_details.Content = "Record Details";
            this.txt_count.Text = Convert.ToString(count);
        }
    }
}
