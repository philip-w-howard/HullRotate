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
                    m_xAngle = 10;
                    m_yAngle = 30;
                    m_zAngle = 190;
                    UpdateDrawings();
                }
            }
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {

        }

        private void XClick(object sender, RoutedEventArgs e)
        {
            m_xAngle += 5;
            myHull.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            myHull.Draw(Perspective);
        }

        private void YClick(object sender, RoutedEventArgs e)
        {
            m_yAngle += 5;
            myHull.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            myHull.Draw(Perspective);
        }

        private void ZClick(object sender, RoutedEventArgs e)
        {
            m_zAngle += 5;
            myHull.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            myHull.Draw(Perspective);
        }

        private void cubeClick(object sender, RoutedEventArgs e)
        {
            myHull.UnitCube();
            myHull.Draw(Perspective);
        }

        private void FrontClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                Console.WriteLine("Front double clock");

                if (m_frontHull == null)
                {
                    m_frontHull = new HullWindow(myHull, 0, 0, 180, "Front", this);
                    m_frontHull.Show();
                }

                m_frontHull.Display();
            }
            else
            {
                m_xAngle = 0;
                m_yAngle = 0;
                m_zAngle = 180;
                UpdateDrawings();
            }
        }

        private void TopClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (m_topHull == null)
                {
                    m_topHull = new HullWindow(myHull, 0, 90, 90, "Top", this);
                    m_topHull.Show();
                }

                m_topHull.Display();
            }
            else
            {
                m_xAngle = 0;
                m_yAngle = 90;
                m_zAngle = 90;
                UpdateDrawings();
            }
        }

        private void SideClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (m_sideHull == null)
                {
                    m_sideHull = new HullWindow(myHull, 0, 90, 180, "Side", this);
                    m_sideHull.Show();
                }

                m_sideHull.Display();
            }
            else
            {
                m_xAngle = 0;
                m_yAngle = 90;
                m_zAngle = 180;
                UpdateDrawings();
            }
        }

        private void UpdateDrawings()
        {
            myHull.RotateTo(0, 0, 180);
            myHull.Draw(FrontCanvas);

            myHull.RotateTo(0, 90, 180);
            myHull.Draw(SideCanvas);

            myHull.RotateTo(0, 90, 90);
            myHull.Draw(TopCanvas);

            myHull.RotateTo(m_xAngle, m_yAngle, m_zAngle);
            myHull.Draw(Perspective);
        }

        public void ChildClosing(string name)
        {
            if (name == "Top")
                m_topHull = null;
            else if (name == "Front")
                m_frontHull = null;
            else if (name == "Side")
                m_sideHull = null;
        }

        private Hull myHull;
        protected double m_xAngle, m_yAngle, m_zAngle;
        protected HullWindow m_frontHull;
        protected HullWindow m_sideHull;
        protected HullWindow m_topHull;
    }
}
