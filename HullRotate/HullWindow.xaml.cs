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

            for (int ii = 1; ii < hull.numBulkheads + 1; ii++)
            {
                currBulkhead.Items.Add(ii);
            }

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
           // m_hull.RotateTo(m_rotate_x, m_rotate_y, m_rotate_z);
            HullCanvas.Children.Clear();
            //m_hull.Draw(HullCanvas);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_hull != null)
            {
                Display();
            }
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

                Display();

               // m_hull.GetBulkheadPoints(bulkhead, points);

                m_handle = new Rectangle[m_hull.numChines];

                for (int ii=0; ii<m_hull.numChines; ii++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Height = RECT_SIZE;
                    rect.Width = RECT_SIZE;
                    rect.Stroke = new SolidColorBrush(Colors.Red); ;
                    rect.StrokeThickness = 1;
                    Canvas.SetTop(rect, points[ii, 1] - RECT_SIZE/2);
                    Canvas.SetLeft(rect, points[ii, 0] - RECT_SIZE/2);
                    HullCanvas.Children.Add(rect);

                    m_handle[ii] = rect;
                }
            }
        }

        protected Rectangle[] m_handle;
        protected bool m_Dragging;
        protected int m_DraggingHandle;
        protected double m_dragX;
        protected double m_dragY;
        const int RECT_SIZE = 8;

        private void HullCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point loc = e.GetPosition(HullCanvas);

            Console.WriteLine("Preview Mouse Down {0},{1}", loc.X, loc.Y);
            if (m_handle == null) return;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (m_Dragging)
                {
                    m_dragX = loc.X;
                    m_dragY = loc.Y;

                    m_handle[m_DraggingHandle].SetValue(Canvas.TopProperty, m_dragY - RECT_SIZE/2);
                    m_handle[m_DraggingHandle].SetValue(Canvas.LeftProperty, m_dragX - RECT_SIZE/2);

                    //HullCanvas.Children.Remove(m_handle[m_DraggingHandle]);

                    //Rectangle rect = new Rectangle();
                    //rect.Height = 8;
                    //rect.Width = 8;
                    //rect.Stroke = new SolidColorBrush(Colors.Black); ;
                    //rect.StrokeThickness = 1;
                    //Canvas.SetTop(rect, m_dragY - 4);
                    //Canvas.SetLeft(rect, m_dragX - 4);
                    //HullCanvas.Children.Add(rect);

                    //m_handle[m_DraggingHandle] = rect;
                }
                else
                {
                    m_dragX = loc.X;
                    m_dragY = loc.Y;

                    Console.WriteLine("Checking for MouseOver");
                    for (int ii=0; ii<m_handle.Length; ii++)
                    {
                        Console.WriteLine("Checking {0},{1}",
                            m_handle[ii].GetValue(Canvas.LeftProperty), m_handle[ii].GetValue(Canvas.TopProperty));

                        if (m_handle[ii].IsMouseOver)
                        {
                            m_DraggingHandle = ii;
                            m_Dragging = true;
                            Console.WriteLine("Mouse over {0} {1},{2}", ii, m_dragX, m_dragY);
                            break;
                        }

                        double x = (double)m_handle[ii].GetValue(Canvas.LeftProperty);
                        double y = (double)m_handle[ii].GetValue(Canvas.TopProperty);
                        if (x <= loc.X && x+RECT_SIZE >= loc.X &&
                            y <= loc.Y && y + RECT_SIZE >= loc.Y)
                        {
                            m_DraggingHandle = ii;
                            m_Dragging = true;
                            Console.WriteLine("Found {0} {1},{2}", ii, m_dragX, m_dragY);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (m_Dragging)
                {
                    Console.WriteLine("Dropped at {0} {1}", m_dragX, m_dragY);
                    m_Dragging = false;

                    //zzz TODO: 
                    //      Need to scale and rotate the new point
                    //      Need to place point back in bulkhead plain.
                    //m_hull.GetBulkheadPoints(currBulkhead.SelectedIndex, points);
                    m_hull.SetBulkheadPoint(currBulkhead.SelectedIndex, m_DraggingHandle, m_dragX, m_dragY, 0);
                    Display();
                }
            }

        }

        private void HullCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_Dragging)
            {
                Point loc = e.GetPosition(HullCanvas);
                m_dragX = loc.X;
                m_dragY = loc.Y;

                m_handle[m_DraggingHandle].SetValue(Canvas.TopProperty, m_dragY - RECT_SIZE / 2);
                m_handle[m_DraggingHandle].SetValue(Canvas.LeftProperty, m_dragX - RECT_SIZE / 2);

                Console.WriteLine("Moved {0} to {1},{2}", m_DraggingHandle, loc.X, loc.Y);
            }
        }
    }
}
