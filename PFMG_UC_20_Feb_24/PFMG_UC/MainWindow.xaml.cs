using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
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
    /// Interaction logic for MainWindow.xaml
    ///</summary>
    public partial class MainWindow : Window
    {
        List<MVCEntity> mvc_entitylist;
        CRUDList[] crudlists;

        CRUDList cl;
        MVCEntity mvc_entity;
        Entity_UC UC_curr;

        public MainWindow()
        {
            InitializeComponent();

            // Connecting UC with their entities

            mvc_entitylist = new List<MVCEntity>();
            mvc_entitylist.Add(UC_R.get_user_input_entity());
            mvc_entitylist.Add(UC_SR.get_user_input_entity());
            mvc_entitylist.Add(UC_PO.get_user_input_entity());
            mvc_entitylist.Add(UC_ER.get_user_input_entity());
            mvc_entitylist.Add(UC_REC.get_user_input_entity());

            // To read from database paticular entity (example Book or Student) and store it as a list

            crudlists = new CRUDList[mvc_entitylist.Count()];
            for (int i = 0; i < mvc_entitylist.Count(); i++)
            {
                crudlists[i] = new CRUDList();
            }

            int j = 0;
            foreach (CRUDList CL in crudlists)
            {
                CL.MVCEntityList = CL.Read_From_File(mvc_entitylist[j].UC.get_user_input_entity());
                CL.Get_TOC(mvc_entitylist[j]);
                j++;
            }

            // Hide all the UserControls
            foreach (MVCEntity entity in mvc_entitylist)
            {
                ((UserControl)entity.UC).Visibility = Visibility.Hidden;
            }

            UC_H.Visibility = Visibility.Hidden;
        }

        private void Btn_Create_Click(object sender, RoutedEventArgs e)
        {
            ((UserControl)mvc_entity.UC).Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Hidden;

            cl.OnCreate(mvc_entity);
            MessageBox.Show("Enter the following details and Click on OK");
            btn_Read.IsEnabled = true;
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            ((UserControl)mvc_entity.UC).Visibility = Visibility.Visible;

            mvc_entity = UC_curr.get_user_input_entity();
            List<MVCEntity> mvc_list = cl.MVCEntityList;
            cl.MVCEntityList = cl.OnSave(mvc_entity);

            if (cl.MVCEntityList == null)
            {
                MessageBox.Show("Data with this ID already exists");
                cl.MVCEntityList = mvc_list;
                UC_curr.Clear_all();
            }

            else
            {
                MessageBox.Show(mvc_entity.ToString() + " Created Successfully");
                UC_curr.Clear_all();
            }

            foreach (CRUDList CL in crudlists)
            {
                if (CL.MVCEntityList[0].GetType() != mvc_entity.GetType() && (cl.toc.offset < CL.toc.offset))
                    CL.toc.offset += 1;
            }

            btn_Read.IsEnabled = true;
        }

        private void Btn_Read_Click(object sender, RoutedEventArgs e)
        {
            mvc_entity = UC_curr.get_user_input_entity();
            mvc_entity = cl.OnRead(mvc_entity);
            if (mvc_entity == null)
            {
                MessageBox.Show(" Does not exist");
            }
            else
            {
                UC_curr.display(mvc_entity);

                MessageBox.Show(mvc_entity.ToString() + " Read Successfully");

                btn_Update.IsEnabled = true;
                btn_Delete.IsEnabled = true;
            }
        }

        private void Btn_Update_Click(object sender, RoutedEventArgs e)
        {
            mvc_entity = UC_curr.get_user_input_entity();
            cl.MVCEntityList = cl.OnUpdate(ref mvc_entity);

            if (mvc_entity == null)
            {
                MessageBox.Show(" Does not exists");
            }
            else
            {
                UC_curr.display(mvc_entity);
                MessageBox.Show(mvc_entity.ToString() + " Updated Successfully");
            }
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            mvc_entity = UC_curr.get_user_input_entity();
            cl.MVCEntityList = cl.OnDelete(ref mvc_entity);

            if (cl.MVCEntityList == null)
            {
                MessageBox.Show(" Does not exists");
            }
            else
            {
                foreach (CRUDList CL in crudlists)
                {
                    if (CL.MVCEntityList[0].GetType() != mvc_entity.GetType() && (cl.toc.offset < CL.toc.offset))
                        CL.toc.offset -= 1;
                }
                MessageBox.Show(mvc_entity.ToString() + " Deleted Successfully");
            }
            UC_curr.Clear_all();
        }

        //______________________________________Database_Related_Buttons___________________________________________________________________

        private void Btn_store_db_Click(object sender, RoutedEventArgs e)
        {
            cl.Flush_DB();

            using (StreamWriter writer = new StreamWriter("Input.csv", true))
            {
                writer.WriteLine("Table of contents");
                foreach (CRUDList CL in crudlists)
                {
                    writer.WriteLine(CL.toc.ToString());
                }
                writer.WriteLine();
            }

            for (int i = 0; i < mvc_entitylist.Count(); i++)
            {
                if (mvc_entity.get_Identifier().id == mvc_entitylist[i].get_Identifier().id)
                    crudlists[i].MVCEntityList = cl.MVCEntityList;
            }

            int j = 0;
            foreach (CRUDList CL in crudlists)
            {
                CL.Write_To_File(CL.MVCEntityList);
                j++;
            }
            MessageBox.Show("Database updated successfully");
        }

        private void Btn_load_db_Click(object sender, RoutedEventArgs e)
        {
            int j = 0;
            foreach (CRUDList CL in crudlists)
            {
                CL.MVCEntityList = CL.Read_From_File(mvc_entitylist[j].UC.get_user_input_entity());
                j++;
            }
            MessageBox.Show("Database Loaded successfully");
        }

        //_______________________________Iterator________________________________________________________________________

        private void Btn_first_Click(object sender, RoutedEventArgs e)
        {
            ((UserControl)mvc_entity.UC).Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Hidden;

            cl.OnFirst();
        }

        private void Btn_previous_Click(object sender, RoutedEventArgs e)
        {
            ((UserControl)mvc_entity.UC).Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Hidden;

            if (cl.OnPrev() == null)
            {
                MessageBox.Show("Current Position is out of bound");
            }
        }

        private void Btn_next_Click(object sender, RoutedEventArgs e)
        {
            ((UserControl)mvc_entity.UC).Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Hidden;

            if (cl.OnNext() == null)
            {
                MessageBox.Show("Current Position is out of bound");
            }
        }

        private void Btn_last_Click(object sender, RoutedEventArgs e)
        {
            ((UserControl)mvc_entity.UC).Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Hidden;

            cl.OnLast();
        }

        private void RADAR_Selected(object sender, RoutedEventArgs e)
        {
            foreach (MVCEntity entity in mvc_entitylist)
            {
                ((UserControl)entity.UC).Visibility = Visibility.Hidden;
            }

            UC_curr = (Entity_UC)UC_R;
            //UC_R.Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Visible;
            mvc_entity = mvc_entitylist[0];
            cl = crudlists[0];
            UC_H.display(mvc_entity, cl.MVCEntityList.Count());
        }

        private void SearchRegime_Selected(object sender, RoutedEventArgs e)
        {
            foreach (MVCEntity entity in mvc_entitylist)
            {
                ((UserControl)entity.UC).Visibility = Visibility.Hidden;
            }

            UC_curr = (Entity_UC)UC_SR;
            //UC_R.Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Visible;
            mvc_entity = mvc_entitylist[1];
            cl = crudlists[1];
            UC_H.display(mvc_entity, cl.MVCEntityList.Count());
        }

        private void PrideOctave_Selected(object sender, RoutedEventArgs e)
        {
            foreach (MVCEntity entity in mvc_entitylist)
            {
                ((UserControl)entity.UC).Visibility = Visibility.Hidden;
            }

            UC_curr = (Entity_UC)UC_PO;
            //UC_R.Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Visible;
            mvc_entity = mvc_entitylist[2];
            cl = crudlists[2];
            UC_H.display(mvc_entity, cl.MVCEntityList.Count());
        }

        private void EmitterRecord_Selected(object sender, RoutedEventArgs e)
        {
            foreach (MVCEntity entity in mvc_entitylist)
            {
                ((UserControl)entity.UC).Visibility = Visibility.Hidden;
            }

            UC_ER.mvc_list = crudlists[0].MVCEntityList;
            UC_curr = (Entity_UC)UC_ER;
            //UC_R.Visibility = Visibility.Visible;
            UC_H.Visibility = Visibility.Visible;
            mvc_entity = mvc_entitylist[3];
            cl = crudlists[3];
            UC_H.display(mvc_entity, cl.MVCEntityList.Count());
        }

        private void Record_Selected(object sender, RoutedEventArgs e)
        {
            foreach (MVCEntity entity in mvc_entitylist)
            {
                ((UserControl)entity.UC).Visibility = Visibility.Hidden;
            }

            UC_curr = (Entity_UC)UC_REC;
            UC_H.Visibility = Visibility.Visible;
            mvc_entity = mvc_entitylist[4];
            cl = crudlists[4];
            UC_H.display(mvc_entity, cl.MVCEntityList.Count());
        }

        private void Emitter_Tracks_Selected(object sender, RoutedEventArgs e)
        {
            ViewEmitterTracks viewEmitterTracks = new ViewEmitterTracks();
            viewEmitterTracks.Visibility = Visibility.Visible;
        }
    }
}
