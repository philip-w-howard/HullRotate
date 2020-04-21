﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HullRotate
{
    class Splines
    {
        public const int RELAXED = 1;
        public const int CLAMPED = 2;

        public Splines(int numPoints, int endCondition,
                    double[,] points)
        {
            m_numPoints = numPoints;
            m_endCondition = endCondition;
            m_points = points;

            m_m_matrix = new double[numPoints, numPoints];
            m_b_matrix = new double[numPoints, 3];
            m_chordLength = new double[numPoints - 1];

            //********************************************
            ComputeChords();
            CreateMatrices();
            GaussianElimination();
        }

        private void ComputeChords()
        {
            // Use normalized chords: 0<t<1
            for (int ii = 0; ii < m_numPoints - 1; ii++)
            {
                m_chordLength[ii] = 1;
            }

            // Compute chord length for scaling
            for (int ii = 0; ii < m_numPoints - 1; ii++)
            {
                m_chordLength[ii] = 0;
                for (int axis = 0; axis < 3; axis++)
                {
                    double value;
                    value = (m_points[ii, axis] - m_points[ii + 1, axis]);
                    value *= value;
                    m_chordLength[ii] += value;
                }
                m_chordLength[ii] = Math.Sqrt(m_chordLength[ii]);
                Console.WriteLine("{0} {1}", ii, m_chordLength[ii]);
            }
        }

        private void CreateMatrices()
        {
            m_m_matrix[0, 0] = 1;
            m_m_matrix[0, 1] = .5;

            for (int ii = 1; ii < m_numPoints - 1; ii++)
            {
                m_m_matrix[ii, ii - 1] = m_chordLength[ii];
                m_m_matrix[ii, ii] = 2 * (m_chordLength[ii - 1] + m_chordLength[ii]);
                m_m_matrix[ii, ii + 1] = m_chordLength[ii - 1];
            }

            m_m_matrix[m_numPoints - 1, m_numPoints - 2] = 2;
            m_m_matrix[m_numPoints - 1, m_numPoints - 1] = 4;

            for (int axis = 0; axis < 3; axis++)
            {
                m_b_matrix[0, axis] = 3.0 / 2.0 / m_chordLength[0] * (m_points[1, axis] - m_points[0, axis]);
                m_b_matrix[m_numPoints - 1, axis] = 6.0 / m_chordLength[m_numPoints - 2] * (m_points[m_numPoints - 1, axis] - m_points[m_numPoints - 2, axis]);

                for (int point = 1; point < m_numPoints - 1; point++)
                {
                    double factor = 3 / m_chordLength[point - 1] / m_chordLength[point];
                    double diff1 = m_points[point + 1, axis] - m_points[point, axis];
                    double diff2 = m_points[point, axis] - m_points[point - 1, axis];
                    double scale1 = m_chordLength[point - 1] * m_chordLength[point - 1];
                    double scale2 = m_chordLength[point] * m_chordLength[point];
                    m_b_matrix[point, axis] = factor * (scale1 * diff1 + scale2 * diff2);
                }
            }
        }

        private void GaussianElimination()
        {
            double scale;
            for (int row = 0; row < m_numPoints - 1; row++)
            {
                // Normalize current row
                scale = m_m_matrix[row, row];
                for (int col = row; col < m_numPoints; col++)
                {
                    m_m_matrix[row, col] /= scale;
                }
                for (int axis = 0; axis < 3; axis++)
                {
                    m_b_matrix[row, axis] /= scale;
                }

                // Zero left of next row
                scale = m_m_matrix[row + 1, row];
                for (int col = row; col < m_numPoints; col++)
                {
                    m_m_matrix[row + 1, col] -= scale * m_m_matrix[row, col];
                }
                for (int axis = 0; axis < 3; axis++)
                {
                    m_b_matrix[row + 1, axis] -= scale * m_b_matrix[row, axis];
                }
            }

            // Normalize the last row
            scale = m_m_matrix[m_numPoints - 1, m_numPoints - 1];
            for (int col = m_numPoints - 1; col < m_numPoints; col++)
            {
                m_m_matrix[m_numPoints - 1, col] /= scale;
            }
            for (int axis = 0; axis < 3; axis++)
            {
                m_b_matrix[m_numPoints - 1, axis] /= scale;
            }

            //****************************************
            // We now have a Reduced Row Echelon Form matrix
            // Solve for the unknowns
            // NOTE: this is optimized because we know we started with a tri-diagonal matrix
            for (int row = m_numPoints - 1; row > 0; row--)
            {
                scale = m_m_matrix[row - 1, row];
                m_m_matrix[row - 1, row] = 0;
                for (int axis = 0; axis < 3; axis++)
                {
                    m_b_matrix[row - 1, axis] -= scale * m_b_matrix[row, axis];
                }
            }
        }

        public void GetPoints(double[,] points)
        {
            // B[1-4, segment, axis]
            double[,,] B = new double[4, m_numPoints - 1, 3];

            // Compute the coefficients
            for (int seg = 0; seg < m_numPoints - 1; seg++)
            {
                for (int axis = 0; axis < 3; axis++)
                {
                    double tmax = m_chordLength[seg];
                    B[0, seg, axis] = m_points[seg, axis];
                    B[1, seg, axis] = m_b_matrix[seg, axis];
                    B[2, seg, axis] = 3 / (tmax * tmax) * (m_points[seg + 1, axis] - m_points[seg, axis])
                                      - 2 / tmax * m_b_matrix[seg, axis]
                                      - 1.0 / tmax * m_b_matrix[seg + 1, axis];
                    B[3, seg, axis] = 2 / (tmax * tmax * tmax) * (m_points[seg, axis] - m_points[seg + 1, axis])
                                      + 1.0 / (tmax * tmax) * m_b_matrix[seg, axis]
                                      + 1.0 / (tmax * tmax) * m_b_matrix[seg + 1, axis];
                }
            }

            int pointsPerSegment = points.GetLength(0) / (m_numPoints - 1);

            int index = 0;
            for (int seg = 0; seg < m_numPoints - 1; seg++)
            {
                double tmax = m_chordLength[seg];

                for (int point = 0; point < pointsPerSegment; point++)
                {
                    double t = point * tmax / pointsPerSegment;
                    for (int axis = 0; axis < 3; axis++)
                    {
                        points[index, axis] = B[0, seg, axis] +
                                              B[1, seg, axis] * t +
                                              B[2, seg, axis] * t * t +
                                              B[3, seg, axis] * t * t * t;
                    }
                    index++;
                }
            }

            // Set the end point
            for (int axis = 0; axis < 3; axis++)
            {
                points[index, axis] = m_points[m_numPoints - 1, axis];
            }
        }
        private void PrintM()
        {
            for (int row = 0; row < m_numPoints; row++)
            {
                for (int col = 0; col < m_numPoints; col++)
                {
                    Console.Write("{0,7:F3}", m_m_matrix[row, col]);
                }
                Console.Write("      ");
                for (int axis = 0; axis < 3; axis++)
                {
                    Console.Write("{0,7:F3}", m_b_matrix[row, axis]);
                }
                Console.WriteLine();
            }
        }

        private void PrintM(string msg)
        {
            Console.WriteLine(msg);
            PrintM();
        }

        private int m_numPoints;                // N
        private int m_endCondition;             // C1
        //private int m_pointsPerSegment;         // Z
        private double[,] m_points;             // P [n, axis]
        private double[,] m_m_matrix;           // N [rows, cols]
        private double[,] m_b_matrix;           // B [rows, axis]
        private double[] m_chordLength;         // L [rows]

    }
}
