using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public struct RMatrix : ICloneable
    {
        private int nRows;
        private int nCols;
        private double[,] matrix;
        public RMatrix(int nRows, int nCols)
        {
            this.nRows = nRows;
            this.nCols = nCols;
            this.matrix = new double[nRows, nCols];

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    matrix[i, j] = 0.0;
                }
            }
        }
        public RMatrix(double[,] matrix)
        {
            this.nRows = matrix.GetLength(0);
            this.nCols = matrix.GetLength(1);
            this.matrix = matrix;
        }
        public RMatrix(RMatrix m)
        {
            nRows = m.GetnRows;
            nCols = m.GetnCols;
            matrix = m.matrix;
        }
        public RMatrix IdentityMatrix()
        {
            RMatrix m = new RMatrix(nRows, nCols);
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if (i == j)
                    {
                        m[i, j] = 1;
                    }
                }
            }
            return m;
        }
        public double this[int m, int n]
        {
            get
            {
                if (m < 0 || m > nRows)
                {
                    throw new Exception("m-th row is out of range!");
                }
                if (n < 0 || n > nCols)
                {
                    throw new Exception("n-th col is out of range!");
                }
                return matrix[m, n];
            }
            set { matrix[m, n] = value; }
        }
        public int GetnRows
        {
            get { return nRows; }
        }
        public int GetnCols
        {
            get { return nCols; }
        }
        public RMatrix Clone()
        {
            RMatrix m = new RMatrix(matrix);
            m.matrix = (double[,])matrix.Clone();
            return m;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
        public override string ToString()
        {
            string strMatrix = "(";
            for (int i = 0; i < nRows; i++)
            {
                string str = "";
                for (int j = 0; j < nCols - 1; j++)
                {
                    str += matrix[i, j].ToString() + ", ";
                }
                str += matrix[i, nCols - 1].ToString();
                if (i != nRows - 1 && i == 0)
                    strMatrix += str + "\n";
                else if (i != nRows - 1 && i != 0)
                    strMatrix += " " + str + "\n";
                else
                    strMatrix += " " + str + ")";
            }
            return strMatrix;
        }
        public override bool Equals(object obj)
        {
            return (obj is RMatrix) && this.Equals((RMatrix)obj);
        }
        public bool Equals(RMatrix m)
        {
            return matrix == m.matrix;
        }
        public override int GetHashCode()
        {
            return matrix.GetHashCode();
        }
        public static bool operator ==(RMatrix m1, RMatrix m2)
        {
            return m1.Equals(m2);
        }
        public static bool operator !=(RMatrix m1, RMatrix m2)
        {
            return !m1.Equals(m2);
        }
        public static RMatrix operator +(RMatrix m)
        {
            return m;
        }
        public static RMatrix operator +(RMatrix m1, RMatrix m2)
        {
            if (!RMatrix.CompareDimension(m1, m2))
            {
                throw new Exception("The dimensions of two matrices must  be the same!");

            }
            RMatrix result = new RMatrix(m1.GetnRows, m1.GetnCols);
            for (int i = 0; i < m1.GetnRows; i++)
            {
                for (int j = 0; j < m1.GetnCols; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }
            return result;
        }
        public static RMatrix operator -(RMatrix m)
        {
            for (int i = 0; i < m.GetnRows; i++)
            {
                for (int j = 0; j < m.GetnCols; j++)
                {
                    m[i, j] = -m[i, j];
                }
            }
            return m;
        }

        public static RMatrix operator -(RMatrix m1, RMatrix m2)
        {
            if (!RMatrix.CompareDimension(m1, m2))
            {
                throw new Exception("The dimensions of two matrices must  be the same!");

            }
            RMatrix result = new RMatrix(m1.GetnRows, m1.GetnCols);
            for (int i = 0; i < m1.GetnRows; i++)
            {
                for (int j = 0; j < m1.GetnCols; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }
            return result;
        }
        public static RMatrix operator *(RMatrix m, double d)
        {
            RMatrix result = new RMatrix(m.GetnRows, m.GetnCols);
            for (int i = 0; i < m.GetnRows; i++)
            {
                for (int j = 0; j < m.GetnCols; j++)
                {
                    result[i, j] = m[i, j] * d;
                }
            }
            return result;
        }
        public static RMatrix operator *(double d, RMatrix m)
        {
            RMatrix result = new RMatrix(m.GetnRows, m.GetnCols);
            for (int i = 0; i < m.GetnRows; i++)
            {
                for (int j = 0; j < m.GetnCols; j++)
                {
                    result[i, j] = m[i, j] * d;
                }
            }
            return result;
        }
        public static RMatrix operator /(RMatrix m, double d)
        {
            RMatrix result = new RMatrix(m.GetnRows, m.GetnCols);
            for (int i = 0; i < m.GetnRows; i++)
            {
                for (int j = 0; j < m.GetnCols; j++)
                {
                    result[i, j] = m[i, j] / d;
                }
            }
            return result;
        }
        public static RMatrix operator /(double d, RMatrix m)
        {
            RMatrix result = new RMatrix(m.GetnRows, m.GetnCols);
            for (int i = 0; i < m.GetnRows; i++)
            {
                for (int j = 0; j < m.GetnCols; j++)
                {
                    result[i, j] = m[i, j] / d;
                }
            }
            return result;
        }
        public static RMatrix operator *(RMatrix m1, RMatrix m2)
        {
            if (m1.GetnCols != m2.GetnRows)
            {
                throw new Exception("The numbers of columns of the" +
                " first matrix must be equal to the number of " +
                " rows of the second matrix!");
            }
            double tmp;
            RMatrix result = new RMatrix(m1.GetnRows, m2.GetnCols);
            for (int i = 0; i < m1.GetnRows; i++)
            {
                for (int j = 0; j < m2.GetnCols; j++)
                {
                    tmp = result[i, j];
                    for (int k = 0; k < result.GetnRows; k++)
                    {
                        tmp += m1[i, k] * m2[k, j];
                    }
                    result[i, j] = tmp;
                }
            }
            return result;
        }
        public RMatrix GetTranspose()
        {
            RMatrix m = this;
            m.Transpose();
            return m;
        }
        public void Transpose()
        {
            RMatrix m = new RMatrix(nCols, nRows);
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    m[j, i] = matrix[i, j];
                }
            }
            this = m;
        }
        public double GetTrace()
        {
            double sum_of_diag = 0.0;
            for (int i = 0; i < nRows; i++)
            {
                if (i < nCols)
                    sum_of_diag += matrix[i, i];
            }
            return sum_of_diag;
        }
        public bool IsSquared()
        {
            if (nRows == nCols)
                return true;
            else
                return false;
        }
        public static bool CompareDimension(RMatrix m1, RMatrix m2)
        {
            if (m1.GetnRows == m2.GetnRows && m1.GetnCols == m2.GetnCols)
                return true;
            else
                return false;
        }
        public RVector GetRowVector(int m)
        {
            if (m < 0 || m > nRows)
            {
                throw new Exception("m-th row is out of range!");
            }
            RVector RowVector = new RVector(nCols);
            for (int i = 0; i < nCols; i++)
            {
                RowVector[i] = matrix[m, i];
            }
            return RowVector;
        }
        public RVector GetColVector(int n)
        {
            if (n < 0 || n > nCols)
            {
                throw new Exception("n-th col is out of range!");
            }
            RVector ColVector = new RVector(nRows);
            for (int i = 0; i < nRows; i++)
            {
                ColVector[i] = matrix[i, n];
            }
            return ColVector;
        }
        public RMatrix ReplaceRow(RVector vec, int m)
        {
            if (m < 0 || m > nRows)
            {
                throw new Exception("m-th row is out of range!");
            }
            if (vec.GetVectorSize != nCols)
            {
                throw new Exception("Vector ndim is out of range!");
            }
            for (int i = 0; i < nCols; i++)
            {
                matrix[m, i] = vec[i];
            }
            return new RMatrix(matrix);
        }
        public RMatrix ReplaceCol(RVector vec, int n)
        {
            if (n < 0 || n > nCols)
            {
                throw new Exception("n-th col is out of range!");
            }
            if (vec.GetVectorSize != nRows)
            {
                throw new Exception("Vector ndim is out of range!");
            }
            for (int i = 0; i < nRows; i++)
            {
                matrix[i, n] = vec[i];
            }
            return new RMatrix(matrix);
        }
        public RMatrix SwapMatrixRow(int m, int n)
        {
            double temp = 0.0;
            for (int i = 0; i < nCols; i++)
            {
                temp = matrix[m, i];
                matrix[m, i] = matrix[n, i];
                matrix[n, i] = temp;
            }
            return new RMatrix(matrix);
        }
        public RMatrix SwapMatrixColumn(int m, int n)
        {
            double temp = 0.0;
            for (int i = 0; i < nRows; i++)
            {
                temp = matrix[i, m];
                matrix[i, m] = matrix[i, n];
                matrix[i, n] = temp;
            }
            return new RMatrix(matrix);
        }
        public static RVector Transform(RMatrix mat, RVector vec)
        {
            RVector result = new RVector(vec.GetVectorSize);
            if (!mat.IsSquared())
            {
                throw new Exception("The matrix must be squared!");
            }
            if (mat.GetnCols != vec.GetVectorSize)
            {
                throw new Exception("The ndim of the vector must be equal"
                + " to the number of cols of the matrix!");
            }
            for (int i = 0; i < mat.GetnRows; i++)
            {
                result[i] = 0.0;
                for (int j = 0; j < mat.GetnCols; j++)
                {
                    result[i] += mat[i, j] * vec[j];
                }
            }
            return result;
        }
        public static RVector Transform(RVector vec, RMatrix mat)
        {
            RVector result = new RVector(vec.GetVectorSize);
            if (!mat.IsSquared())
            {
                throw new Exception("The matrix must be squared!");
            }
            if (mat.GetnRows != vec.GetVectorSize)
            {
                throw new Exception("The ndim of the vector must be equal"
                + " to the number of rows of the matrix!");
            }
            for (int i = 0; i < mat.GetnRows; i++)
            {
                result[i] = 0.0;
                for (int j = 0; j < mat.GetnCols; j++)
                {
                    result[i] += vec[j] * mat[j, i];
                }
            }
            return result;
        }
        public static RMatrix Transform(RVector v1, RVector v2)
        {
            if (v1.GetVectorSize != v2.GetVectorSize)
            {
                throw new Exception("The vectors must have the same ndim!");
            }
            RMatrix result = new RMatrix(v1.GetVectorSize, v1.GetVectorSize);
            for (int i = 0; i < v1.GetVectorSize; i++)
            {
                for (int j = 0; j < v1.GetVectorSize; j++)
                {
                    result[j, i] = v1[i] * v2[j];
                }
            }
            return result;
        }
        public static double Determinant(RMatrix mat)
        {
            double result = 0.0;
            if (!mat.IsSquared())
            {
                throw new Exception("The matrix must be squared!");
            }
            if (mat.GetnRows == 1)
                result = mat[0, 0];
            else
            {
                for (int i = 0; i < mat.GetnRows; i++)
                {
                    result += Math.Pow(-1, i) * mat[0, i] *
                    Determinant(RMatrix.Minor(mat, 0, i));
                }
            }
            return result;
        }
        public static RMatrix Minor(RMatrix mat, int row, int col)
        {
            RMatrix mm = new RMatrix(mat.GetnRows - 1, mat.GetnCols - 1);
            int ii = 0, jj = 0;
            for (int i = 0; i < mat.GetnRows; i++)
            {
                if (i == row)
                    continue;
                jj = 0;
                for (int j = 0; j < mat.GetnCols; j++)
                {
                    if (j == col)
                        continue;
                    mm[ii, jj] = mat[i, j];
                    jj++;
                }
                ii++;
            }
            return mm;
        }
        public static RMatrix Adjoint(RMatrix mat)
        {
            if (!mat.IsSquared())
            {
                throw new Exception("The matrix must be squared!");
            }
            RMatrix ma = new RMatrix(mat.GetnRows, mat.GetnCols);
            for (int i = 0; i < mat.GetnRows; i++)
            {
                for (int j = 0; j < mat.GetnCols; j++)
                {
                    ma[i, j] = Math.Pow(-1, i + j) * (Determinant(Minor(mat, i, j)));
                }
            }
            return ma.GetTranspose();
        }
        public static RMatrix Inverse(RMatrix mat)
        {
            if (Determinant(mat) == 0)
            {
                throw new Exception("Cannot inverse a matrix with a zero  determinant!");

            }
            return (Adjoint(mat) / Determinant(mat));
        }

    }
    }
