using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HullRotate
{
    class HullEditor
    {
        protected Hull m_hull;
        protected double m_rotate_x, m_rotate_y, m_rotate_z;
        protected Canvas m_canvas;

        protected Rectangle[] m_handle;
        protected bool m_Dragging;
        protected int m_DraggingHandle;
        protected double m_dragX;
        protected double m_dragY;
        public int currBulkhead { get; set; }
        public bool IsEditable { get; set; }
        const int RECT_SIZE = 8;

        public HullEditor(Hull hull, double x, double y, double z, Canvas canvas)
        {
            m_hull = hull;
            m_rotate_x = x;
            m_rotate_y = y;
            m_rotate_z = z;

            m_canvas = canvas;
            IsEditable = false;

            m_handle = new Rectangle[m_hull.numChines];
        }

        public void SetHull(Hull hull)
        {
            m_hull = hull;

            m_handle = new Rectangle[m_hull.numChines];
        }

        public void SetRotation(double x, double y, double z)
        {
            m_rotate_x = x;
            m_rotate_y = y;
            m_rotate_z = z;
        }

        public void Display()
        {
            //m_hull.RotateTo(m_rotate_x, m_rotate_y, m_rotate_z);
            m_canvas.Children.Clear();
            //m_hull.Draw(m_canvas);

            if (IsEditable) DrawHandles();
        }

        public void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point loc = e.GetPosition(m_canvas);

            Console.WriteLine("Preview Mouse Down {0},{1}", loc.X, loc.Y);
            if (m_handle == null || m_handle[0] == null) return;

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (m_Dragging)
                {
                    m_dragX = loc.X;
                    m_dragY = loc.Y;

                    m_handle[m_DraggingHandle].SetValue(Canvas.TopProperty, m_dragY - RECT_SIZE / 2);
                    m_handle[m_DraggingHandle].SetValue(Canvas.LeftProperty, m_dragX - RECT_SIZE / 2);

                    //m_canvas.Children.Remove(m_handle[m_DraggingHandle]);

                    //Rectangle rect = new Rectangle();
                    //rect.Height = 8;
                    //rect.Width = 8;
                    //rect.Stroke = new SolidColorBrush(Colors.Black); ;
                    //rect.StrokeThickness = 1;
                    //Canvas.SetTop(rect, m_dragY - 4);
                    //Canvas.SetLeft(rect, m_dragX - 4);
                    //m_canvas.Children.Add(rect);

                    //m_handle[m_DraggingHandle] = rect;
                }
                else
                {
                    m_dragX = loc.X;
                    m_dragY = loc.Y;

                    Console.WriteLine("Checking for MouseOver");
                    for (int ii = 0; ii < m_handle.Length; ii++)
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
                        if (x <= loc.X && x + RECT_SIZE >= loc.X &&
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
                    //m_hull.GetBulkheadPoints(currBulkhead, points);
                    m_hull.SetBulkheadPoint(currBulkhead, m_DraggingHandle, m_dragX, m_dragY, 0);
                    Display();
                }
                else
                {
                    Console.WriteLine("non-dragging MouseUP");
                    
                }
            }

        }

        public void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_Dragging)
            {
                Point loc = e.GetPosition(m_canvas);
                m_dragX = loc.X;
                m_dragY = loc.Y;

                m_handle[m_DraggingHandle].SetValue(Canvas.TopProperty, m_dragY - RECT_SIZE / 2);
                m_handle[m_DraggingHandle].SetValue(Canvas.LeftProperty, m_dragX - RECT_SIZE / 2);

                Console.WriteLine("Moved {0} to {1},{2}", m_DraggingHandle, loc.X, loc.Y);
            }
        }
        public void Bulkhead_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsEditable)
            {
                currBulkhead = ((ComboBox)sender).SelectedIndex;
                Console.WriteLine("Selected " + currBulkhead);

                Display();
             }

        }

        protected void DrawHandles()
        {
            double[,] points = new double[m_hull.numChines, 2];

           // m_hull.GetBulkheadPoints(currBulkhead, points);

            m_handle = new Rectangle[m_hull.numChines];

            for (int ii = 0; ii < m_hull.numChines; ii++)
            {
                Rectangle rect = new Rectangle();
                rect.Height = RECT_SIZE;
                rect.Width = RECT_SIZE;
                rect.Stroke = new SolidColorBrush(Colors.Red); ;
                rect.StrokeThickness = 1;
                Canvas.SetTop(rect, points[ii, 1] - RECT_SIZE / 2);
                Canvas.SetLeft(rect, points[ii, 0] - RECT_SIZE / 2);
                m_canvas.Children.Add(rect);

                m_handle[ii] = rect;
            }
        }
    }
}
