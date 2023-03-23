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

namespace PFMG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MVCEntity entity;
        Entity_UC UC;
        CRUDList crudList;
        public MainWindow()
        {
            InitializeComponent();
            crudList = new CRUDList();
            entity = UC_B.get_entity();
            crudList.MVCEntity_List = crudList.Read_From_File(entity);
            UC = UC_B;
        }

        private void btn_Create_Click(object sender, RoutedEventArgs e)
        {
            entity = UC.get_entity();
            crudList.OnCreate(entity);
            MessageBox.Show(entity.ToString() + " Created Successfully");
        }

        private void btn_Read_Click(object sender, RoutedEventArgs e)
        {
            entity = UC.get_entity();
            entity = crudList.OnRead(entity);
            if(entity == null)
            {
                MessageBox.Show("Does Not Exists");
            }
            else
            {
                UC.display(entity);
                MessageBox.Show(entity.ToString() + " Read Successfully");
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            entity = UC.get_entity();
            entity = crudList.OnUpdate(entity);
            if (entity == null)
            {
                MessageBox.Show("Does Not Exists");
            }
            else
            {
                UC.display(entity);
                MessageBox.Show(entity.ToString() + " Update Successfully");
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            entity = UC.get_entity();
            entity = crudList.OnDelete(entity);
            if (entity == null)
            {
                MessageBox.Show("Does Not Exists");
            }
            else
            {
                UC.display(entity);
                MessageBox.Show(entity.ToString() + " Delete Successfully");
            }
        }

        private void btn_Store_to_db_Click(object sender, RoutedEventArgs e)
        {
            crudList.OnFlush_DB();
            crudList.Write_To_File(crudList.MVCEntity_List);
            MessageBox.Show("Database Updated Successfully");
        }
    }
}
