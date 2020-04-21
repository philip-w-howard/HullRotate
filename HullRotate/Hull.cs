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

        public string UnitCube()
        {
            m_numBulkheads = 2;
            m_numChines = 4;

            m_bulkheads = new double[m_numBulkheads][,];
            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                m_bulkheads[bulkhead] = new double[m_numChines, 3];
            }

            for (int bulkhead=0; bulkhead<m_numBulkheads; bulkhead++)
            {
                m_bulkheads[bulkhead][0, 0] = 0;
                m_bulkheads[bulkhead][0, 1] = 0;
                m_bulkheads[bulkhead][0, 2] = bulkhead;

                m_bulkheads[bulkhead][1, 0] = 1;
                m_bulkheads[bulkhead][1, 1] = 0;
                m_bulkheads[bulkhead][1, 2] = bulkhead;

                m_bulkheads[bulkhead][2, 0] = 1;
                m_bulkheads[bulkhead][2, 1] = 1;
                m_bulkheads[bulkhead][2, 2] = bulkhead;

                m_bulkheads[bulkhead][3, 0] = 0;
                m_bulkheads[bulkhead][3, 1] = 1;
                m_bulkheads[bulkhead][3, 2] = bulkhead;
            }

            PrepareDrawing();

            return "";
        }

        public string LoadFromHullFile(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            if (lines.Length < 1) return "Invalid file format";
            if (! int.TryParse(lines[0], out m_numChines)) return "Invalid file format 1";
            m_numBulkheads = 5;
            m_bulkheads = new double[m_numBulkheads][,];
            for (int bulkhead=0; bulkhead<m_numBulkheads; bulkhead++)
            {
                m_bulkheads[bulkhead] = new double[m_numChines, 3];
            }

            if (lines.Length < m_numBulkheads * m_numChines * 3 + 1) return "Invalid file format 2";

            int index = 1;
            for (int bulkhead=0; bulkhead<m_numBulkheads; bulkhead++)
            {
                for (int chine=0; chine<m_numChines; chine++)
                {
                    for (int axis=0; axis<3; axis++)
                    {
                        if (!double.TryParse(lines[index], out m_bulkheads[bulkhead][chine, axis]))
                            return "Invalid file format on line " + index;
                        index++;
                    }
                }
            }

            PrepareDrawing();

            return "";
        }

        public void Draw(Canvas canvas)
        {
            canvas.Children.Clear();

            // Get size
            double min_x = double.MaxValue;
            double min_y = double.MaxValue;
            double max_x = double.MinValue;
            double max_y = double.MinValue;

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_drawnBulkheads[bulkhead].GetLength(0); chine++)
                {
                    double x = m_drawnBulkheads[bulkhead][chine, 0];
                    double y = m_drawnBulkheads[bulkhead][chine, 1];
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
            scale = 0.8 * scale;

            Console.WriteLine("Scale: {0}", scale);

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_drawnBulkheads[bulkhead].GetLength(0); chine++)
                {
                    m_drawnBulkheads[bulkhead][chine, 0] *= scale;
                    m_drawnBulkheads[bulkhead][chine, 1] *= scale;
                    m_drawnBulkheads[bulkhead][chine, 2] *= scale;
                }
            }

            for (int chine = 0; chine < m_numChines; chine++)
            {
                for (int point = 0; point < POINTS_PER_CHINE; point++)
                {
                    m_chines[chine][point, 0] *= scale;
                    m_chines[chine][point, 1] *= scale;
                    m_chines[chine][point, 2] *= scale;
                }
            }

            CenterTo(canvas.ActualWidth/2, canvas.ActualHeight/2, 0);

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_drawnBulkheads[bulkhead].GetLength(0) - 1; chine++)
                {
                    if (chine != m_drawnBulkheads[bulkhead].GetLength(0) / 2 - 1)
                    {
                        Line myLine = new Line();

                        myLine.Stroke = System.Windows.Media.Brushes.Black;

                        myLine.X1 = m_drawnBulkheads[bulkhead][chine, 0];
                        myLine.X2 = m_drawnBulkheads[bulkhead][chine + 1, 0];
                        myLine.Y1 = m_drawnBulkheads[bulkhead][chine, 1];
                        myLine.Y2 = m_drawnBulkheads[bulkhead][chine + 1, 1];

                        myLine.StrokeThickness = 1;

                        canvas.Children.Add(myLine);
                    }
                }
            }

            // NOTE: Won't work if the array is ragged
            //for (int chine = 0; chine < m_drawnBulkheads[0].GetLength(0); chine++)
            //{
            //    for (int bulkhead = 0; bulkhead < m_numBulkheads - 1; bulkhead++)
            //    {
            //        Line myLine = new Line();

            //        myLine.Stroke = System.Windows.Media.Brushes.Gray;

            //        myLine.X1 = m_drawnBulkheads[bulkhead][chine, 0];
            //        myLine.X2 = m_drawnBulkheads[bulkhead + 1][chine, 0];
            //        myLine.Y1 = m_drawnBulkheads[bulkhead][chine, 1];
            //        myLine.Y2 = m_drawnBulkheads[bulkhead + 1][chine, 1];

            //        myLine.StrokeThickness = 1;

            //        canvas.Children.Add(myLine);
            //    }
            //}

            for (int chine = 0; chine < m_numChines; chine++)
            {
                for (int point = 0; point < POINTS_PER_CHINE - 1; point++)
                {
                    Line myLine = new Line();

                    myLine.Stroke = System.Windows.Media.Brushes.Gray;

                    myLine.X1 = m_chines[chine][point, 0];
                    myLine.X2 = m_chines[chine][point+1, 0];
                    myLine.Y1 = m_chines[chine][point, 1];
                    myLine.Y2 = m_chines[chine][point+1, 1];

                    myLine.StrokeThickness = 1;

                    canvas.Children.Add(myLine);
                }
            }

        }
        // public Splines(int numPoints, int endCondition, double[,] points);
        // public void GetPoints(double[,] points);
        protected void PrepareDrawing()
        {

            m_drawnBulkheads = new double[m_bulkheads.GetLength(0)][,];

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                int centerChine = m_bulkheads[bulkhead].GetLength(0);
                m_drawnBulkheads[bulkhead] = new double[m_bulkheads[bulkhead].GetLength(0)*2,3];
                for (int chine=0; chine< m_bulkheads[bulkhead].GetLength(0); chine++)
                {
                    for (int axis=0; axis<3; axis++)
                    {
                        m_drawnBulkheads[bulkhead][chine, axis] = m_bulkheads[bulkhead][chine, axis];
                        m_drawnBulkheads[bulkhead][chine + centerChine, axis] = m_bulkheads[bulkhead][chine, axis];
                    }

                    // mirror the X
                    m_drawnBulkheads[bulkhead][chine + centerChine, 0] *= -1;
                }
            }

            m_chines = new double[m_numChines * 2][,];
            double[,] chine_data = new double[m_numBulkheads, 3];
            for (int chine = 0; chine<m_numChines; chine++)
            {
                m_chines[chine] = new double[POINTS_PER_CHINE, 3];
                for (int bulkhead=0; bulkhead<m_numBulkheads; bulkhead++)
                {
                    for (int axis=0; axis<3; axis++)
                    {
                        chine_data[bulkhead, axis] = m_bulkheads[bulkhead][chine, axis];
                    }
                }
                Splines spline = new Splines(m_numBulkheads, Splines.RELAXED, chine_data);
                spline.GetPoints(m_chines[chine]);
            }
        }

        protected void RotateDrawing_X(double angle)
        {
            double[,] rotate = new double[3, 3];

            angle = angle * Math.PI / 180.0;

            rotate[0, 0] = 1.0;
            rotate[1, 1] = Math.Cos(angle);
            rotate[2, 2] = Math.Cos(angle);
            rotate[1, 2] = Math.Sin(angle);
            rotate[2, 1] = -Math.Sin(angle);

            CenterTo(0,0,0);

            for (int ii = 0; ii < m_numBulkheads; ii++)
            {
                Matrix.Multiply(m_drawnBulkheads[ii], rotate, m_drawnBulkheads[ii]);
            }

            for (int ii = 0; ii < m_numChines; ii++)
            {
                Matrix.Multiply(m_chines[ii], rotate, m_chines[ii]);
            }
        }

        protected void RotateDrawing_Y(double angle)
        {
            double[,] rotate = new double[3, 3];

            angle = angle * Math.PI / 180.0;

            rotate[1, 1] = 1.0;
            rotate[0, 0] = Math.Cos(angle);
            rotate[2, 2] = Math.Cos(angle);
            rotate[2, 0] = Math.Sin(angle);
            rotate[0, 2] = -Math.Sin(angle);

            CenterTo(0, 0, 0);

            for (int ii = 0; ii < m_numBulkheads; ii++)
            {
                Matrix.Multiply(m_drawnBulkheads[ii], rotate, m_drawnBulkheads[ii]);
            }

            for (int ii = 0; ii < m_numChines; ii++)
            {
                Matrix.Multiply(m_chines[ii], rotate, m_chines[ii]);
            }
        }

        protected void RotateDrawing_Z(double angle)
        {
            double[,] rotate = new double[3, 3];

            angle = angle * Math.PI / 180.0;

            rotate[2, 2] = 1.0;
            rotate[0, 0] = Math.Cos(angle);
            rotate[1, 1] = Math.Cos(angle);
            rotate[0, 1] = Math.Sin(angle);
            rotate[1, 0] = -Math.Sin(angle);

            CenterTo(0, 0, 0);

            for (int ii = 0; ii < m_numBulkheads; ii++)
            {
                Matrix.Multiply(m_drawnBulkheads[ii], rotate, m_drawnBulkheads[ii]);
            }

            for (int ii=0; ii<m_numChines; ii++)
            {
                Matrix.Multiply(m_chines[ii], rotate, m_chines[ii]);
            }
        }

        public void RotateTo(double x, double y, double z)
        {
            // NOTE: Could optimize by multiplying the three rotation matrices before rotating the points
            PrepareDrawing();
            RotateDrawing_Z(z);
            RotateDrawing_X(x);
            RotateDrawing_Y(y);
        }

        private void CenterTo(double centerX, double centerY, double centerZ)
        {
            // Get size
            double min_x = double.MaxValue;
            double min_y = double.MaxValue;
            double min_z = double.MaxValue;
            double max_x = double.MinValue;
            double max_y = double.MinValue;
            double max_z = double.MinValue;

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_drawnBulkheads[bulkhead].GetLength(0); chine++)
                {
                    double x = m_drawnBulkheads[bulkhead][chine, 0];
                    double y = m_drawnBulkheads[bulkhead][chine, 1];
                    double z = m_drawnBulkheads[bulkhead][chine, 2];
                    if (x > max_x) max_x = x;
                    if (y > max_y) max_y = y;
                    if (z > max_z) max_z = z;
                    if (x < min_x) min_x = x;
                    if (y < min_y) min_y = y;
                    if (z < min_z) min_z = z;
                }
            }

            double shift_x = centerX - (max_x + min_x) / 2;
            double shift_y = centerY - (max_y + min_y) / 2;
            double shift_z = centerZ - (max_z + min_z) / 2;

            Console.WriteLine("CenterTo ({0},{1},{2}) Shift ({3},{4},{5})",
                centerX, centerY, centerZ, shift_x, shift_y, shift_z);

            for (int bulkhead = 0; bulkhead < m_numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_drawnBulkheads[bulkhead].GetLength(0); chine++)
                {
                    m_drawnBulkheads[bulkhead][chine, 0] += shift_x;
                    m_drawnBulkheads[bulkhead][chine, 1] += shift_y;
                    m_drawnBulkheads[bulkhead][chine, 2] += shift_z;
                }
            }

            for (int ii = 0; ii < m_numChines; ii++)
            {
                for (int point=0; point<POINTS_PER_CHINE; point++)
                {
                    m_chines[ii][point, 0] += shift_x;
                    m_chines[ii][point, 1] += shift_y;
                    m_chines[ii][point, 2] += shift_z;
                }
            }
        }

        private int m_numChines;
        private int m_numBulkheads;
        private double[][,] m_chines;           // [chine][index, axis]
        private double[][,] m_bulkheads;        // [bulkhead][chine, axis]
        private double[][,] m_drawnBulkheads;   // [bulkhead][chine, axis]
        private const int POINTS_PER_CHINE = 50;
    }
}
