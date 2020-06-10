using System;
using System.Drawing;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class Symbols
    {
        public enum SymbolType {arrow,diamond,rect,horiLine,vertLine,inclinedLine}
        public enum SymbDirec { left, right,top, bottom, vertLine, inclinedLine }
        public DovDrawings Graphic { get; set; }
       public Symbols()
        {

        }
         public void DrawArrow(PointF pt,float w,float h,float angle) {

            PointF pt0 = pt;
            PointF pt1 = new PointF(pt0.X - h, pt0.Y + w * 0.5f);
            PointF pt2 = new PointF(pt0.X - h, pt0.Y - w * 0.5f);
            Matrix m= Matrix.RotateAt(angle, pt); ;
            float [] trans= m.VectorMultiply(new[] { pt.X, pt.Y, 1 });
            pt0 = new PointF(trans[0], trans[1]);
            trans = m.VectorMultiply(new[] { pt1.X, pt1.Y, 1 });
            pt1= new PointF(trans[0], trans[1]);
            trans = m.VectorMultiply(new[] { pt2.X, pt2.Y, 1 });
            pt2 = new PointF(trans[0], trans[1]);
             Graphic.DrawLine(pt0, pt1);
            Graphic.DrawLine(pt0, pt2);
             Graphic.DrawLine(pt1, pt2);
        }
        public void DrawArrow(PointF pts, PointF pte, float w, float h)
        {
            float adju = pte.X - pts.X;
            float opp = pte.Y - pts.Y;
           
            float length = (float)Math.Sqrt((adju * adju) + (opp * opp));
            float angle = (float)Math.Asin(opp / length)*180/(float)Math.PI;
            PointF pt0 = pts;
            PointF pt1 = new PointF(pt0.X - h, pt0.Y + w * 0.5f);
            PointF pt2 = new PointF(pt0.X - h, pt0.Y - w * 0.5f);
            Matrix m = new Matrix();
            m = Matrix.RotateAt(angle, pt0);
            float[] trans = m.VectorMultiply(new[] { pt0.X, pt0.Y, 1 });
            pt0 = new PointF(trans[0], trans[1]);
            trans = m.VectorMultiply(new[] { pt1.X, pt1.Y, 1 });
            pt1 = new PointF(trans[0], trans[1]);
            trans = m.VectorMultiply(new[] { pt2.X, pt2.Y, 1 });
            pt2 = new PointF(trans[0], trans[1]);
            Graphic.DrawLine(pt0, pt1);
             Graphic.DrawLine(pt0, pt2);
             Graphic.DrawLine(pt1, pt2);
        }
    }
}
