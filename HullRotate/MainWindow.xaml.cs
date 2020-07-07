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
        private Hull myHull;
        protected double m_xAngle, m_yAngle, m_zAngle;
        protected HullWindow m_frontHull;
        protected HullWindow m_sideHull;
        protected HullWindow m_topHull;

        protected DisplayHull m_FrontDisplay, m_SideDisplay, m_TopDisplay, m_PerspectiveDisplay;

        HullEditor m_hullEditor;

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
                    currBulkhead.Items.Clear();
                    
                    for (int ii = 1; ii < myHull.numBulkheads + 1; ii++)
                    {
                        currBulkhead.Items.Add(ii);
                    }

                    //m_hullEditor = new HullEditor(myHull, 0, 0, 0, Perspective);

                    m_xAngle = 10;
                    m_yAngle = 30;
                    m_zAngle = 190;

                    m_FrontDisplay = new DisplayHull(myHull, FrontCanvas);
                    m_SideDisplay = new DisplayHull(myHull, SideCanvas);
                    m_TopDisplay = new DisplayHull(myHull, TopCanvas);
                    m_PerspectiveDisplay = new DisplayHull(myHull, Perspective);

                    m_hullEditor = new HullEditor(m_PerspectiveDisplay);

                    m_hullEditor.IsEditable = false;
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
            m_PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);

            m_hullEditor.IsEditable = false;
            m_hullEditor.Draw();
        }

        private void YClick(object sender, RoutedEventArgs e)
        {
            m_yAngle += 5;
            m_PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);

            m_hullEditor.IsEditable = false;
            m_hullEditor.Draw();
        }

        private void ZClick(object sender, RoutedEventArgs e)
        {
            m_zAngle += 5;
            m_PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);

            //m_hullEditor.IsEditable = false;
            m_hullEditor.Draw();
        }

        private void FrontClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                Console.WriteLine("Front double click");

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

                m_hullEditor.IsEditable = true;

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

                m_hullEditor.IsEditable = true;

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

                m_hullEditor.IsEditable = true;

                UpdateDrawings();
            }
        }

        private void UpdateDrawings()
        {
            if (m_FrontDisplay != null)
            {
                m_FrontDisplay.RotateTo(0, 0, 180);
                m_FrontDisplay.Draw();

                m_SideDisplay.RotateTo(0, 90, 180);
                m_SideDisplay.Draw();

                m_TopDisplay.RotateTo(0, 90, 90);
                m_TopDisplay.Draw();

                m_PerspectiveDisplay.RotateTo(m_xAngle, m_yAngle, m_zAngle);
                m_hullEditor.Draw();
            }
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_hullEditor != null) m_hullEditor.Bulkhead_SelectionChanged(sender, e);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateDrawings();
        }

        private void Perspective_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("PriviewMouseDown: " + e);
            if (m_hullEditor != null) m_hullEditor.PreviewMouseDown(sender, e);
        }

        private void Perspective_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_hullEditor != null) m_hullEditor.PreviewMouseMove(sender, e);
        }
    }
}
