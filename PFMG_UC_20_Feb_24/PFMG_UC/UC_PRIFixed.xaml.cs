﻿using System;
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
    /// Interaction logic for UC_PRIFixed.xaml
    ///</summary>
    public partial class UC_PRIFixed : UserControl, Entity_UC
    {
        public UC_PRIFixed()
        {
            InitializeComponent();
        }

        public void Clear_all()
        {
            txt_PRImin.Clear();
            txt_PRImax.Clear();
            txt_Tolerance.Clear();
        }

        public void display(MVCEntity data)
        {
            this.Visibility = Visibility.Visible;
            Radar r = (Radar)data;
            txt_PRImin.Text = Convert.ToString(r.pri_min);
            txt_PRImax.Text = Convert.ToString(r.pri_max);
            txt_Tolerance.Text = Convert.ToString(r.tolerance);
        }

        public void Enable()
        {
            txt_PRImin.IsEnabled = true;
            txt_PRImax.IsEnabled = true;
            txt_Tolerance.IsEnabled = true;
        }

        public MVCEntity get_user_input_entity()
        {
            // Radar radar = ;
            if (this.txt_PRImin.Text == "")
            {
                return new Radar(new UC_Radar_Info());
            }
            Radar radar = new Radar(new UC_Radar_Info(), 0, Side.BLUE, new Fixed(), null, Convert.ToDouble(txt_PRImin.Text), Convert.ToDouble(txt_PRImax.Text),
                                 0.0, 0.0, 0.0, 0.0, Convert.ToDouble(txt_Tolerance.Text));
            //radar.pri_agility = new Fixed();
            radar.priagility = PRI_Agility.FIXED;
            return radar;
        }
    }
}
