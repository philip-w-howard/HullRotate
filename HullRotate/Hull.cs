//******************************************************
// Class to represent a hull as a sequence of bulkheads

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HullRotate
{
    class Hull
    {
        public Hull() { }

        public string LoadFromHullFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            if (lines.Length < 1) return "Invalid file format";
            if (! int.TryParse(lines[0], out m_numChines)) return "Invalid file format 1";
            m_numBulkheads = 5;
            m_bulkheads = new double[m_numBulkheads, m_numChines, 3];

            if (lines.Length < m_numBulkheads * m_numChines * 3 + 1) return "Invalid file format 2";

            int index = 1;
            for (int bulkhead=0; bulkhead<m_numBulkheads; bulkhead++)
            {
                for (int chine=0; chine<m_numChines; chine++)
                {
                    for (int axis=0; axis<3; axis++)
                    {
                        if (!double.TryParse(lines[index], out m_bulkheads[bulkhead, chine, axis]))
                            return "Invalid file format on line " + index;
                        index++;
                    }
                }
            }

            return "";
        }

        public void Draw(Canvas canvas, double x_rotate, double y_rotate, double z_rotate)
        {
            double[,,] bulkheads = m_bulkheads;

            // Do rotations

            // Do projection

            // Get size
            double min_x = double.MaxValue;
            double min_y = double.MaxValue;
            double max_x = double.MinValue;
            double max_y = double.MinValue;

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_numChines; chine++)
                {
                    double x = bulkheads[bulkhead, chine, 0];
                    double y = bulkheads[bulkhead, chine, 1];
                    if (x > max_x) max_x = x;
                    if (y > max_y) max_y = y;
                    if (x < min_x) min_x = x;
                    if (y < min_y) min_y = y;
                }
            }

            // Scale all the points to fit in the canvas
            double scale1 = canvas.ActualWidth / (max_x - min_x);
            double scale2 = canvas.ActualHeight / (max_y - min_y);

            double scale = scale1;
            if (scale2 < scale) scale = scale2;
            double shift_x = -min_x + 0.1 * scale;
            double shift_y = -min_y + 0.1 * scale;
            scale = 0.8 * scale;

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_numChines; chine++)
                {
                    bulkheads[bulkhead, chine, 0] += shift_x;
                    bulkheads[bulkhead, chine, 1] += shift_y;

                    bulkheads[bulkhead, chine, 0] *= scale;
                    bulkheads[bulkhead, chine, 1] *= scale;

                    // Flip in Y direction
                    bulkheads[bulkhead, chine, 1] = canvas.ActualHeight - bulkheads[bulkhead, chine, 1];
                }
            }

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_numChines-1; chine++)
                {
                    Line myLine = new Line();

                    myLine.Stroke = System.Windows.Media.Brushes.Black;

                    myLine.X1 = bulkheads[bulkhead, chine, 0];
                    myLine.X2 = bulkheads[bulkhead, chine + 1, 0];
                    myLine.Y1 = bulkheads[bulkhead, chine, 1];
                    myLine.Y2 = bulkheads[bulkhead, chine + 1, 1];

                    myLine.StrokeThickness = 1;

                    canvas.Children.Add(myLine);

                }
            }

            for (int chine = 0; chine < m_numChines; chine++)
            {
                for (int bulkhead = 0; bulkhead < m_numBulkheads-1; bulkhead++)
                {
                    Line myLine = new Line();

                    myLine.Stroke = System.Windows.Media.Brushes.Gray;

                    myLine.X1 = bulkheads[bulkhead, chine, 0];
                    myLine.X2 = bulkheads[bulkhead + 1, chine, 0];
                    myLine.Y1 = bulkheads[bulkhead, chine, 1];
                    myLine.Y2 = bulkheads[bulkhead + 1, chine, 1];

                    myLine.StrokeThickness = 1;

                    canvas.Children.Add(myLine);

                }
            }

        }

        private int m_numChines;
        private int m_numBulkheads;
        private double[,,] m_bulkheads; // [bulkhead, chine, axis]
    }
}
