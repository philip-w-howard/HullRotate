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
                    myHull.RotateDrawing_Z(180);

                    myHull.Draw(Front);

                    myHull.RotateDrawing_Y(90);
                    myHull.Draw(Side);

                    myHull.RotateDrawing_X(90);
                    myHull.Draw(Top);

                    myHull.RotateDrawing_X(-30);
                    myHull.RotateDrawing_Y(-30);
                    myHull.Draw(Perspective);
                }
            }
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {

        }

        private void XClick(object sender, RoutedEventArgs e)
        {
            myHull.RotateDrawing_X(5);
            myHull.Draw(Perspective);
        }

        private void YClick(object sender, RoutedEventArgs e)
        {
            myHull.RotateDrawing_Y(5);
            myHull.Draw(Perspective);
        }

        private void ZClick(object sender, RoutedEventArgs e)
        {
            myHull.RotateDrawing_Z(5);
            myHull.Draw(Perspective);
        }

        private void cubeClick(object sender, RoutedEventArgs e)
        {
            myHull.UnitCube();
            myHull.Draw(Perspective);
        }

        private Hull myHull;

        private void FrontClick(object sender, MouseButtonEventArgs e)
        {
            myHull.PrepareDrawing();
            myHull.RotateDrawing_Z(180);

            myHull.Draw(Front);

            myHull.RotateDrawing_Y(90);
            myHull.Draw(Side);

            myHull.RotateDrawing_X(90);
            myHull.Draw(Top);


            myHull.RotateDrawing_X(-90);
            myHull.RotateDrawing_Y(-90);
            myHull.Draw(Perspective);
        }

        private void TopClick(object sender, MouseButtonEventArgs e)
        {
            myHull.PrepareDrawing();
            myHull.RotateDrawing_Z(180);

            myHull.Draw(Front);

            myHull.RotateDrawing_Y(90);
            myHull.Draw(Side);

            myHull.RotateDrawing_X(90);
            myHull.Draw(Top);
            myHull.Draw(Perspective);
        }

        private void SideClick(object sender, MouseButtonEventArgs e)
        {
            myHull.PrepareDrawing();
            myHull.RotateDrawing_Z(180);

            myHull.Draw(Front);

            myHull.RotateDrawing_Y(90);
            myHull.Draw(Side);

            myHull.RotateDrawing_X(90);
            myHull.Draw(Top);

            myHull.RotateDrawing_X(-90);
            myHull.Draw(Perspective);
        }
    }
}
