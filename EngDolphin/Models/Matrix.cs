using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace EngDolphin.Client.Models
{
    public class Matrix
    {
        public float[,] M = new float[3, 3];

        public Matrix() { Identity(); }

        public Matrix(float m00, float m01, float m02, float m03, float m10, float m11,
            float m12, float m13, float m20, float m21, float m22, float m23)
        {

            M[0, 0] = m00; M[0, 1] = m01; M[0, 2] = m02; M[0, 3] = m03;
            M[1, 0] = m10; M[1, 1] = m11; M[1, 2] = m12; M[1, 3] = m13;
            M[2, 0] = m20; M[2, 1] = m21; M[2, 2] = m22; M[2, 3] = m23; 
           
        }
        public Matrix(float m00, float m01, float m10, float m11, float m20, float m21)
        {
            Identity();
            M[0, 0] = m00; M[0, 1] = m01; 
            M[1, 0] = m10; M[1, 1] = m11; 
            M[2, 0] = m20; M[2, 1] = m21; 

        }
        public void Identity()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j)
                    {
                        M[i, j] = 1;
                    }
                    else
                    {
                        M[i, j] = 0;
                    }
                }
            }
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float element = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        element += m1.M[i, k] * m2.M[k, j];
                    }
                    result.M[i, j] = element;
                }
            }
            return result;
        }
        // Apply a transformation to a vector (point):     
        public float[] VectorMultiply(float[] vector)
        {
            float[] result = new float[3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i] += M[i, j] * vector[j];
                }
            }
            return result;
        }

        public static Matrix Translate(float dx, float dy)
        {
            Matrix result = new Matrix();
            result.M[0, 2] = dx;
            result.M[1, 2] = dy;
            
            return result;
        }
      

        // Create a rotation matrix around the z axis:     
        public static Matrix Rotate(float theta)
        {
            theta = theta * (float)Math.PI / 180.0f;
            float sn = (float)Math.Sin(theta);
            float cn = (float)Math.Cos(theta);
            Matrix result = new Matrix();
            result.M[0, 0] = cn;
            result.M[0, 1] = -sn;
            result.M[1, 0] = sn;
            result.M[1, 1] = cn;
            return result;
        }
        public static Matrix RotateAt(float theta,PointF pt )
        {
            theta = theta * (float)Math.PI / 180.0f;
            float sn = (float)Math.Sin(theta);
            float cn = (float)Math.Cos(theta);
            Matrix result = new Matrix();
            result.M[0, 0] = cn;
            result.M[0, 1] = -sn;
            result.M[1, 0] = sn;
            result.M[1, 1] = cn;
            result.M[0, 2] = pt.X*(1-cn)+(pt.Y*sn);
            result.M[1, 2] = pt.Y*(1-cn)-(pt.X*sn);
            return result;
        }
        public static Matrix Scale(float sx, float sy)
        {
            Matrix result = new Matrix();
            result.M[0, 0] = sx;
            result.M[1, 1] = sy;
            return result;
        }

    }
}
