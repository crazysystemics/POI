using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace PFMG
{
    /// <summary>
    /// Interaction logic for UC_Book.xaml
    /// </summary>
    public partial class UC_Book : UserControl, Entity_UC
    {
        public UC_Book()
        {
            InitializeComponent();
        }

        public MVCEntity get_entity()
        {
           if(txt_id.Text == "")
            {
                return new Book(this);
            }
            return new Book(this, Convert.ToInt32(txt_id.Text), txt_name.Text);
        }

        public void display(MVCEntity e)
        {
            txt_id.Text = Convert.ToString(((Book)e).id);
            txt_name.Text = ((Book)e).name;
        }
    }
}
