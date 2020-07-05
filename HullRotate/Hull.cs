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
    public class Hull
    {
        public int numChines { get; private set; }
        public int numBulkheads { get ; private set; }

        private double[][,] m_chines;           // [chine][index, axis]
        private double[][,] m_bulkheads;        // [bulkhead][chine, axis]
        private double[][,] m_drawnBulkheads;   // [bulkhead][chine, axis]
        private const int POINTS_PER_CHINE = 50;

        private double m_rotate_x, m_rotate_y, m_rotate_z;
        private double m_translate_x, m_translate_y, m_translate_z;
        private double m_scale;

        public Hull() { }

        public string UnitCube()
        {
            numBulkheads = 2;
            numChines = 4;

            m_bulkheads = new double[numBulkheads][,];
            for (int bulkhead = 0; bulkhead < numBulkheads; bulkhead++)
            {
                m_bulkheads[bulkhead] = new double[numChines, 3];
            }

            for (int bulkhead=0; bulkhead<numBulkheads; bulkhead++)
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
            int num_chines = numChines;

            if (! int.TryParse(lines[0], out num_chines)) return "Invalid file format 1";
            numChines = num_chines;
            numBulkheads = 5;
            m_bulkheads = new double[numBulkheads][,];
            for (int bulkhead=0; bulkhead<numBulkheads; bulkhead++)
            {
                m_bulkheads[bulkhead] = new double[numChines, 3];
            }

            if (lines.Length < numBulkheads * numChines * 3 + 1) return "Invalid file format 2";

            int index = 1;
            for (int bulkhead=0; bulkhead<numBulkheads; bulkhead++)
            {
                for (int chine=0; chine<numChines; chine++)
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
            // Get size
            double min_x = double.MaxValue;
            double min_y = double.MaxValue;
            double max_x = double.MinValue;
            double max_y = double.MinValue;

            for (int bulkhead = 0; bulkhead < numBulkheads; bulkhead++)
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

            m_scale = scale1;
            if (scale2 < m_scale) m_scale = scale2;
            m_scale = 0.9 * m_scale;

            Console.WriteLine("Scale: {0}", m_scale);

            for (int bulkhead = 0; bulkhead < numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_drawnBulkheads[bulkhead].GetLength(0); chine++)
                {
                    m_drawnBulkheads[bulkhead][chine, 0] *= m_scale;
                    m_drawnBulkheads[bulkhead][chine, 1] *= m_scale;
                    m_drawnBulkheads[bulkhead][chine, 2] *= m_scale;
                }
            }

            for (int chine = 0; chine < numChines*2; chine++)
            {
                for (int point = 0; point < POINTS_PER_CHINE; point++)
                {
                    m_chines[chine][point, 0] *= m_scale;
                    m_chines[chine][point, 1] *= m_scale;
                    m_chines[chine][point, 2] *= m_scale;
                }
            }

            CenterTo(canvas.ActualWidth/2, canvas.ActualHeight/2, 0);

            for (int bulkhead = 0; bulkhead < numBulkheads; bulkhead++)
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
            //    for (int bulkhead = 0; bulkhead < numBulkheads - 1; bulkhead++)
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

            for (int chine = 0; chine < numChines*2; chine++)
            {
                for (int point = 0; point < POINTS_PER_CHINE - 2; point++)
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
        protected void PrepareDrawing()
        {
            m_drawnBulkheads = new double[m_bulkheads.GetLength(0)][,];
            int centerChine = m_bulkheads[0].GetLength(0);

            for (int bulkhead = 0; bulkhead < numBulkheads; bulkhead++)
            {
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

            m_chines = new double[numChines * 2][,];
            double[,] chine_data = new double[numBulkheads, 3];
            for (int chine = 0; chine<numChines * 2; chine++)
            {
                int actual_chine = chine;
                if (chine >= numChines) actual_chine = chine - numChines;

                m_chines[chine] = new double[POINTS_PER_CHINE, 3];
                for (int bulkhead=0; bulkhead<numBulkheads; bulkhead++)
                {
                    for (int axis=0; axis<3; axis++)
                    {
                        chine_data[bulkhead, axis] = m_bulkheads[bulkhead][actual_chine, axis];
                    }

                    if (chine >= numChines) chine_data[bulkhead, 0] *= -1;
                }
                Splines spline = new Splines(numBulkheads, Splines.RELAXED, chine_data);
                spline.GetPoints(m_chines[chine]);
            }
        }

        protected void RotateDrawing_X(double angle)
        {
            double[,] rotate = new double[3, 3];

            m_rotate_x = angle;

            angle = angle * Math.PI / 180.0;

            rotate[0, 0] = 1.0;
            rotate[1, 1] = Math.Cos(angle);
            rotate[2, 2] = Math.Cos(angle);
            rotate[1, 2] = Math.Sin(angle);
            rotate[2, 1] = -Math.Sin(angle);

            CenterTo(0,0,0);

            for (int ii = 0; ii < numBulkheads; ii++)
            {
                Matrix.Multiply(m_drawnBulkheads[ii], rotate, m_drawnBulkheads[ii]);
            }

            for (int ii = 0; ii < numChines * 2; ii++)
            {
                Matrix.Multiply(m_chines[ii], rotate, m_chines[ii]);
            }
        }

        protected void RotateDrawing_Y(double angle)
        {
            double[,] rotate = new double[3, 3];

            m_rotate_y = angle;

            angle = angle * Math.PI / 180.0;

            rotate[1, 1] = 1.0;
            rotate[0, 0] = Math.Cos(angle);
            rotate[2, 2] = Math.Cos(angle);
            rotate[2, 0] = Math.Sin(angle);
            rotate[0, 2] = -Math.Sin(angle);

            CenterTo(0, 0, 0);

            for (int ii = 0; ii < numBulkheads; ii++)
            {
                Matrix.Multiply(m_drawnBulkheads[ii], rotate, m_drawnBulkheads[ii]);
            }

            for (int ii = 0; ii < numChines * 2; ii++)
            {
                Matrix.Multiply(m_chines[ii], rotate, m_chines[ii]);
            }
        }

        protected void RotateDrawing_Z(double angle)
        {
            double[,] rotate = new double[3, 3];

            m_rotate_z = angle;

            angle = angle * Math.PI / 180.0;

            rotate[2, 2] = 1.0;
            rotate[0, 0] = Math.Cos(angle);
            rotate[1, 1] = Math.Cos(angle);
            rotate[0, 1] = Math.Sin(angle);
            rotate[1, 0] = -Math.Sin(angle);

            CenterTo(0, 0, 0);

            for (int ii = 0; ii < numBulkheads; ii++)
            {
                Matrix.Multiply(m_drawnBulkheads[ii], rotate, m_drawnBulkheads[ii]);
            }

            for (int ii=0; ii<numChines * 2; ii++)
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

            for (int bulkhead = 0; bulkhead < numBulkheads; bulkhead++)
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

            m_translate_x = centerX - (max_x + min_x) / 2;
            m_translate_y = centerY - (max_y + min_y) / 2;
            m_translate_z = centerZ - (max_z + min_z) / 2;

            Console.WriteLine("CenterTo ({0},{1},{2}) Shift ({3},{4},{5})",
                centerX, centerY, centerZ, m_translate_x, m_translate_y, m_translate_z);
            TranslateTo(m_translate_x, m_translate_y, m_translate_z);
        }

        private void TranslateTo(double shiftX, double shiftY, double shiftZ)
        {
            for (int bulkhead = 0; bulkhead < numBulkheads; bulkhead++)
            {
                for (int chine = 0; chine < m_drawnBulkheads[bulkhead].GetLength(0); chine++)
                {
                    m_drawnBulkheads[bulkhead][chine, 0] += shiftX;
                    m_drawnBulkheads[bulkhead][chine, 1] += shiftY;
                    m_drawnBulkheads[bulkhead][chine, 2] += shiftZ;
                }
            }

            for (int ii = 0; ii < numChines * 2; ii++)
            {
                for (int point=0; point<POINTS_PER_CHINE; point++)
                {
                    m_chines[ii][point, 0] += shiftX;
                    m_chines[ii][point, 1] += shiftY;
                    m_chines[ii][point, 2] += shiftZ;
                }
            }
        }

        public void GetBulkheadPoints(int bulkhead, double[,] points)
        {
            for (int ii=0; ii<numChines; ii++)
            {
                points[ii, 0] = m_drawnBulkheads[bulkhead][ii, 0];
                points[ii, 1] = m_drawnBulkheads[bulkhead][ii, 1];
            }
        }

        public void SetBulkheadPoint(int bulkhead, int chine, double x, double y, double z)
        {
            m_bulkheads[bulkhead][chine, 0] = x;
            m_bulkheads[bulkhead][chine, 1] = y;
            //m_bulkheads[bulkhead][chine, 2] = z;
            PrepareDrawing();
        }
    }
}
