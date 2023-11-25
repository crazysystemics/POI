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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace RWR_POC_GUI
{
    /// <summary>
    /// Interaction logic for RWRDisplay.xaml
    /// </summary>
    public partial class RWRDisplay : Window
    {
        System.Windows.Controls.Label[] labels;
        int centerX;
        int centerY;
        int maxRange;
        bool rangeFound = false;
        Label label = new Label();

        public RWRDisplay()
        {
            InitializeComponent();
            centerX = 400;
            centerY = 200;
            Init();
        }
        public void Init()
        {
            double currentX = 400;
            double currentY = 200;
            label.Content = "+";
            label.Foreground = Brushes.White;
            Canvas.SetLeft(label, currentX - label.ActualWidth / 2);
            Canvas.SetTop(label, currentY - label.ActualHeight / 2);
            canvas.Children.Add(label);
        }

        public void DisplaySymbols(List<RadarSignature> signatures)
        {
            if (labels != null)
            {
                foreach (Label label in labels)

                    canvas.Children.Remove(label);

            }

            labels = new Label[signatures.Count];

            int i = 0;

            double currentX;

            double currentY;

            double angleRadians;

            //canvas.Children.Clear();

            foreach (RadarSignature rs in signatures)
            {
                int r = rs.r;
                if (!rangeFound)
                {
                    maxRange = r;
                    rangeFound = true;
                }
                angleRadians = rs.theta;// * (Math.PI / 180);
                if (angleRadians < 0)
                {
                    angleRadians += (Math.PI * 2);
                }
                labels[i] = new Label();
                labels[i].Content = rs.symbol;
                labels[i].Foreground = Brushes.Red;
                //currentX = centerX + (rs.r * Math.Cos(-angleRadians));
                //currentY = centerY + (rs.r * Math.Sin(-angleRadians));

                currentX = centerX + ((rs.r * 200) / maxRange) * Math.Cos(-angleRadians);
                currentY = centerY + ((rs.r * 200) / maxRange) * Math.Sin(angleRadians);

                //currentX = 400;
                //currentY = 0;

                //Console.WriteLine($"DU : {currentX}, {currentY}\n");
                //  currentX = currentX / 2;
                //  currentY = currentY / 2;
                Canvas.SetLeft(labels[i], currentX - labels[i].ActualWidth / 2);
                Canvas.SetTop(labels[i], currentY - labels[i].ActualHeight / 2);
                canvas.Children.Add(labels[i]);

                i++;

            }
        }
    }
}