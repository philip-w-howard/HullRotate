using Microsoft.Win32;
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

namespace HullRotate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            myHull = new Hull();
        }

        private void openClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Hull files (*.hul)|*.hul|All files (*.*)|*.*";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                string result = myHull.LoadFromHullFile(openFileDialog.FileName);

                if (result != "")
                    Console.WriteLine(result);
                else
                {
                    myHull.Draw(myCanvas);
                }
            }
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {

        }

        private void XClick(object sender, RoutedEventArgs e)
        {
            myHull.RotateDrawing_X(5 * Math.PI / 180.0);
            myHull.Draw(myCanvas);
        }

        private void YClick(object sender, RoutedEventArgs e)
        {
            myHull.RotateDrawing_Y(5 * Math.PI / 180.0);
            myHull.Draw(myCanvas);
        }

        private void ZClick(object sender, RoutedEventArgs e)
        {
            myHull.RotateDrawing_Z(5 * Math.PI / 180.0);
            myHull.Draw(myCanvas);
        }

        private void cubeClick(object sender, RoutedEventArgs e)
        {
            myHull.UnitCube();
            myHull.Draw(myCanvas);
        }

        private Hull myHull;
    }
}
