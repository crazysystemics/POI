using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace RWR_POC_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        DiscreteTimeSimulationEngine DTSE = new DiscreteTimeSimulationEngine();
        double[] currentX = new double[10];
        double[] currentY = new double[10];
        System.Windows.Controls.Label[] label = new Label[10];
        public RWRDisplay rwrDisplay;
        //  public AcquisitionRadarDisplay acquisitionRadar;
        // public FCRDisplay fcrDisplay;

        int resolution = -160;
        double multiplier = 6.5;
        double yMultiplier = 4.5;

        public MainWindow()
        {
            InitializeComponent();

            Globals.mainWindow = this;
            Globals.timer = new DispatcherTimer();
            rwrDisplay = new RWRDisplay();
            // acquisitionRadar = new AcquisitionRadarDisplay();
            // fcrDisplay new FCRDisplay();
            DTSE.Init();
        }

        private void Btn_start_tick_Click(object sender, RoutedEventArgs e)
        {
            btn_start_tick.Background = Brushes.Green;
            btn_pause_tick.Background = Brushes.LightBlue;
            btn_next_tick.Background = Brushes.LightBlue;
            Globals.timer = new DispatcherTimer();
            DTSE.RunSimulationEngine();
            Globals.timer.Start();
        }

        private void Btn_pause_tick_Click(object sender, RoutedEventArgs e)
        {
            btn_pause_tick.Background = Brushes.Red;
            btn_start_tick.Background = Brushes.LightBlue;
            btn_next_tick.Background = Brushes.LightBlue;
            Globals.timer.Stop();
        }

        private void Btn_next_tick_Click(object sender, RoutedEventArgs e)
        {
            btn_pause_tick.Background = Brushes.LightBlue;
            btn_next_tick.Background = Brushes.Yellow;
            Globals.timer = new DispatcherTimer();
            DTSE.RunSimulationEngine();
            Globals.timer.Start();
        }
        public void DisplayPosition(object obj)
        {
            if (obj is Aircraft)
            {
                Line line = new Line();
                int index = ((Aircraft)obj).id;
                line.X1 = currentX[index];
                line.Y1 = currentY[index];

                currentX[index] = multiplier * ((Aircraft)obj).currentWaypoint.x + resolution;

                currentY[index] = 300 - yMultiplier * ((Aircraft)obj).currentWaypoint.y - resolution;

                line.X2 = currentX[index];
                line.Y2 = currentY[index];
                if (Globals.tick == 0)
                {
                    label[index] = new Label();
                    line.X1 = currentX[((Aircraft)obj).id];
                    line.Y1 = currentY[((Aircraft)obj).id];
                    //label[index].FontFamily = new FontFamily("Wingdings");
                    label[index].Content = "AIR";
                    label[index].Foreground = Brushes.Black;
                    label[index].FontWeight = FontWeights.Bold;
                    label[index].FontSize = 16;
                    double X = currentX[index] - 7;
                    double Y = currentY[index] - 7;

                    Canvas.SetLeft(label[index], X - label[index].ActualWidth / 2);
                    Canvas.SetTop(label[index], Y - label[index].ActualHeight / 2);
                    canvas.Children.Add(label[index]);
                }
                else
                {
                    label[index].Content = "AIR";
                    Canvas.SetLeft(label[index], currentX[index]);
                    Canvas.SetTop(label[index], currentY[index]);
                    label[index].Visibility = Visibility.Visible;
                    label[index].FontSize = 16;
                }
                line.Stroke = Brushes.Black;
                canvas.Children.Add(line);
            }
            if (obj is Radar)
            {
                if (Globals.tick == 0)
                {
                    Label radarLabel = new Label();
                    double currentX = multiplier * ((Radar)obj).position.x + resolution;
                    double currentY = 300 - yMultiplier * ((Radar)obj).position.y - resolution;
                    //radarLabel.FontFamily = new FontFamily("Symbol");
                    radarLabel.Content = (char)(((Radar)obj).id + 64) + $" ({((Radar)obj).position.x}, {((Radar)obj).position.y})";
                    radarLabel.Foreground = Brushes.Black;
                    radarLabel.FontWeight = FontWeights.Bold;
                    radarLabel.FontSize = 14;
                    double X = currentX - 7;
                    double Y = currentY - 7;

                    Canvas.SetLeft(radarLabel, X - radarLabel.ActualWidth / 2);
                    Canvas.SetTop(radarLabel, Y - radarLabel.ActualHeight / 2);
                    canvas.Children.Add(radarLabel);
                }
            }
        }
          
        public void DisplayFlightPath(object obj)
        {

            if (obj is Aircraft)
            {
                for (int i = 0; i < ((Aircraft)obj).waypoints.Count - 1; i++)
                {
                    Line line = new Line();

                    line.X1 = multiplier * ((Aircraft)obj).waypoints[i].x + resolution;

                    line.Y1 = 300 - yMultiplier * ((Aircraft)obj).waypoints[i].y - resolution;

                    line.X2 = multiplier * ((Aircraft)obj).waypoints[i + 1].x + resolution;

                    line.Y2 = 300 - yMultiplier * ((Aircraft)obj).waypoints[i + 1].y - resolution;
                    line.Stroke = Brushes.Orange;
                    canvas.Children.Add(line);

                    Label label = new Label();

                    label.Content = "wp" + i + $"({((Aircraft)obj).waypoints[i].x} , {((Aircraft)obj).waypoints[i].y})";

                    label.Foreground = Brushes.Black; //label.Fontweight Fontweights.Bold;

                    label.FontSize = 14;

                    double X = line.X1 - 7;

                    double Y = line.Y1 - 7;

                    Canvas.SetLeft(label, X - label.ActualWidth / 2);
                    Canvas.SetTop(label, Y - label.ActualHeight / 2);

                    canvas.Children.Add(label);
                }
            }
        }
        private void Menu_rdr_display_Click(object sender, RoutedEventArgs e)
        {
            // acquisitionRadar.Visibility = Visibility.Visible;
        }
        private void Menu_rwr_display_Click(object sender, RoutedEventArgs e)
        {
            rwrDisplay.Visibility = Visibility.Visible;
        }
        private void Menu_fcr_display_Click(object sender, RoutedEventArgs e)
        {
            // ferDisplayVisibility = Visibility.Visible;
        }
        //public partial class MainWindow : Window
        //{
        //    DiscreteTimeSimulationEngine DTSE;
        //    public MainWindow()
        //    {
        //        InitializeComponent();
        //        DTSE = new DiscreteTimeSimulationEngine();
        //        DTSE.Init();
        //        RunSimulation();
        //    }

        //    public void RunSimulation()
        //    {
        //        while (true)
        //        {
        //            DTSE.RunSimulationEngine();
        //            {
        //                Console.WriteLine("\n----------------\n\nPress Enter/Return to display next tick");
        //                Console.ReadLine();
        //            }
        //        }
        //    }

        //    private void menu_rwr_display_Click(object sender, RoutedEventArgs e)
        //    {

        //    }
        //}
    }
}