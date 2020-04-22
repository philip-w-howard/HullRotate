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
using System.Windows.Shapes;

namespace HullRotate
{
    /// <summary>
    /// Interaction logic for HullWindow.xaml
    /// </summary>
    public partial class HullWindow : Window
    {
        public HullWindow(Hull hull, double x, double y, double z, string name, MainWindow parent)
        {
            InitializeComponent();
            m_hull = hull;
            m_rotate_x = x;
            m_rotate_y = y;
            m_rotate_z = z;

            Title = name;

            m_parent = parent;
        }

        public void SetHull(Hull hull)
        {
            m_hull = hull;
        }

        public void SetRotation(double x, double y, double z)
        {
            m_rotate_x = x;
            m_rotate_y = y;
            m_rotate_z = z;
        }

        public void Display()
        {
            m_hull.RotateTo(m_rotate_x, m_rotate_y, m_rotate_z);
            m_hull.Draw(HullCanvas);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_hull != null) m_hull.Draw(HullCanvas);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Window closing");
            m_parent.ChildClosing(Title);
        }

        Hull m_hull;
        MainWindow m_parent;
        double m_rotate_x;
        double m_rotate_y;
        double m_rotate_z;
        bool m_editing = false;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            m_editing = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int bulkhead;
            double[,] points = new double[m_hull.numChines, 2];
            if (m_editing)
            {
                bulkhead = currBulkhead.SelectedIndex;
                Console.WriteLine("Selected " + bulkhead);

                HullCanvas.Children.Clear();
                m_hull.RotateTo(m_rotate_x, m_rotate_y, m_rotate_z);
                m_hull.Draw(HullCanvas);

                m_hull.GetBulkheadPoints(bulkhead, points);

                m_handle = new Button[m_hull.numChines];

                for (int ii=0; ii<m_hull.numChines; ii++)
                {
                    m_handle[ii] = new Button();
                    Rectangle rect = new Rectangle();
                    rect.Height = 8;
                    rect.Width = 8;
                    rect.Stroke = new SolidColorBrush(Colors.Black); ;
                    rect.StrokeThickness = 1;
                    Canvas.SetTop(rect, points[ii, 1] - 4);
                    Canvas.SetLeft(rect, points[ii, 0] - 4);
                    HullCanvas.Children.Add(rect);
                }
            }
        }

        protected Button[] m_handle;
    }
}
