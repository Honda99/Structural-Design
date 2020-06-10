using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class DovCoordinate
    {
        public float XMin { get; set; } = -150;
        public float XMax { get; set; } = 150;
        public float YMin { get; set; } = -200;
        public float YMax { get; set; } = 200;
        public float GraphicsWidth { get; set; } = 300;
        public float GraphicsHeight { get; set; } = 400;
        public float OriginX { get; set; } = 150;
        public float OriginY { get; set; } = 200;
        public float GraphicsX { get; set; } = 0;
        public float Offset { get; set; } = 10;
       
        public DovCoordinate(float xMin,float xMax, float yMin,float yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;

        }
        public DovCoordinate()
        {

        }
        public PointF Point2D(PointF ptf)
        {
            PointF aPoint = new PointF();
            if (ptf.X < XMin || ptf.X > XMax || ptf.Y < YMin || ptf.Y > YMax)
            {
                ptf.X = Single.NaN; ptf.Y = Single.NaN;
            }
            aPoint.X =Offset*0.5f+ GraphicsX + (ptf.X - XMin) * GraphicsWidth / (XMax - XMin);
            aPoint.Y =Offset*0.5f+ GraphicsHeight - (ptf.Y - YMin) * GraphicsHeight / (YMax - YMin);
            return aPoint;
        }
        public float ScaleX(float x)
        {
            float hor = x;
            if (GraphicsWidth >= GraphicsHeight)
            {
                decimal fac =(decimal)((GraphicsWidth / (XMax - XMin)));
                hor =  (float)Math.Abs(fac)*x;
                
            }
            else
            {
                decimal fac = (decimal)(GraphicsHeight / (YMax - YMin));
                hor = (float)Math.Abs(fac) * x;
            }
            return hor;
        }
    }
}
