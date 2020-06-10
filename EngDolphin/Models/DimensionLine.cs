using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class DimensionLine
    {
        public enum DimensionDirection { left, right }
        public enum DimensionLocation { start, end }
        public DovDrawings Graphic { get; set; }
        public DimensionDirection DimDir { get; set; } = DimensionDirection.left;
        public DimensionLocation DimLoc { get; set; } = DimensionLocation.start;
        public float DimOffset { get; set; } = 5;
        public float DimExtInner { get; set; } = 6;
        public float DimExtOuter { get; set; } = 3;
        public float DimExtParallel { get; set; } = 5;
        public enum TextLoc { oneThird, middle, twoThird }
        public TextLoc TextPosition = TextLoc.middle;
        public DimensionLine()
        {

        }
        public void DrawDimLineAligned(PointF pt1, PointF pt2, string dimen)
        {
            PointF p1 = pt1;
            PointF p2 = pt2;
            float adjucent = pt2.X - pt1.X;
            float oppos = pt2.Y - pt1.Y;
            float length = (float)Math.Sqrt((adjucent * adjucent) + (oppos * oppos));
            float angle = (float)Math.Asin(oppos / length);
            //Draw Text if Exist
            string drawText = string.IsNullOrEmpty(dimen) ? Math.Round(length, 3).ToString() : dimen;
            PointF dimPt1;
            PointF dimPt2;
            PointF innerExt1;
            PointF innerExt2;
            PointF outerExt1;
            PointF outerExt2;
            PointF parallelExt1;
            PointF parallelExt2;

            float inOffsetX = 0;
            float inOffsetY = 0;
            float extInnerX = 0;
            float extInnerY = 0;
            float extOuterX = 0;
            float extOuterY = 0;
            float parExtX = 0;
            float parExtY = 0;
            switch (DimDir)
            {

                case DimensionDirection.left:
                    inOffsetX = (DimOffset + DimExtInner) * (float)Math.Sin(angle);
                    inOffsetY = (DimOffset + DimExtInner) * (float)Math.Cos(angle);
                    if (Math.Sin(angle) == 0 || Math.Round(Math.Cos(angle), 6) == 0)
                    {
                        if (Math.Sin(angle) == 0)
                        {
                            extInnerX = 0;
                            extInnerY = DimExtInner * (float)Math.Cos(angle);
                            extOuterX = 0;
                            extOuterY = DimExtOuter * (float)Math.Cos(angle);

                        }
                        if (Math.Round(Math.Cos(angle), 6) == 0)
                        {
                            extInnerX = DimExtInner * (float)Math.Sin(angle);
                            extInnerY = 0;
                            extOuterX = DimExtOuter * (float)Math.Sin(angle);
                            extOuterY = 0;

                        }
                    }
                    else
                    {
                        extInnerX = DimExtInner * (float)Math.Cos(angle);
                        extInnerY = DimExtInner * (float)Math.Sin(angle);
                        extOuterX = DimExtOuter * (float)Math.Cos(angle);
                        extOuterY = DimExtOuter * (float)Math.Sin(angle);

                    }

                    parExtX = DimExtParallel * (float)Math.Cos(angle);
                    parExtY = DimExtParallel * (float)Math.Sin(angle);
                    dimPt1 = new PointF(p1.X - inOffsetX, pt1.Y + inOffsetY);
                    dimPt2 = new PointF(p2.X - inOffsetX, pt2.Y + inOffsetY);
                    innerExt1 = new PointF(dimPt1.X + extInnerX, dimPt1.Y - extInnerY);
                    innerExt2 = new PointF(dimPt2.X + extInnerX, dimPt2.Y - extInnerY);
                    outerExt1 = new PointF(dimPt1.X - extOuterX, dimPt1.Y + extOuterY);
                    outerExt2 = new PointF(dimPt2.X - extOuterX, dimPt2.Y + extOuterY);
                    parallelExt1 = new PointF(dimPt1.X - parExtX, dimPt1.Y - parExtY);
                    parallelExt2 = new PointF(dimPt2.X + parExtX, dimPt2.Y + parExtY);
                     Graphic.DrawLine(dimPt1, dimPt2);
                     Graphic.DrawLine(dimPt1, innerExt1);
                     Graphic.DrawLine(dimPt1, outerExt1);
                     Graphic.DrawLine(dimPt1, parallelExt1);
                     Graphic.DrawLine(dimPt2, innerExt2);
                     Graphic.DrawLine(dimPt2, outerExt2);
                    Graphic.DrawLine(dimPt2, parallelExt2);
                     DrawDimeValue(dimPt1, dimPt2, drawText);
                    break;
                case DimensionDirection.right:
                    inOffsetX = (DimOffset + DimExtInner) * (float)Math.Sin(angle);
                    inOffsetY = (DimOffset + DimExtInner) * (float)Math.Cos(angle);
                    if (Math.Sin(angle) == 0 || Math.Round(Math.Cos(angle), 6) == 0)
                    {
                        if (Math.Sin(angle) == 0)
                        {
                            extInnerX = 0;
                            extInnerY = DimExtInner * (float)Math.Cos(angle);
                            extOuterX = 0;
                            extOuterY = DimExtOuter * (float)Math.Cos(angle);

                        }
                        if (Math.Round(Math.Cos(angle), 6) == 0)
                        {
                            extInnerX = DimExtInner * (float)Math.Sin(angle);
                            extInnerY = 0;
                            extOuterX = DimExtOuter * (float)Math.Sin(angle);
                            extOuterY = 0;

                        }
                    }
                    else
                    {
                        extInnerX = DimExtInner * (float)Math.Cos(angle);
                        extInnerY = DimExtInner * (float)Math.Sin(angle);
                        extOuterX = DimExtOuter * (float)Math.Cos(angle);
                        extOuterY = DimExtOuter * (float)Math.Sin(angle);

                    }
                    parExtX = DimExtParallel * (float)Math.Cos(angle);
                    parExtY = DimExtParallel * (float)Math.Sin(angle);

                    dimPt1 = new PointF(p1.X + inOffsetX, pt1.Y - inOffsetY);
                    dimPt2 = new PointF(p2.X + inOffsetX, pt2.Y - inOffsetY);
                    innerExt1 = new PointF(dimPt1.X - extInnerX, dimPt1.Y + extInnerY);
                    innerExt2 = new PointF(dimPt2.X - extInnerX, dimPt2.Y + extInnerY);
                    outerExt1 = new PointF(dimPt1.X + extOuterX, dimPt1.Y - extOuterY);
                    outerExt2 = new PointF(dimPt2.X + extOuterX, dimPt2.Y - extOuterY);
                    parallelExt1 = new PointF(dimPt1.X - parExtX, dimPt1.Y - parExtY);
                    parallelExt2 = new PointF(dimPt2.X + parExtX, dimPt2.Y + parExtY);
                    Graphic.DrawLine(dimPt1, dimPt2);
                     Graphic.DrawLine(dimPt1, innerExt1);
                     Graphic.DrawLine(dimPt1, outerExt1);
                     Graphic.DrawLine(dimPt1, parallelExt1);
                     Graphic.DrawLine(dimPt2, innerExt2);
                    Graphic.DrawLine(dimPt2, outerExt2);
                     Graphic.DrawLine(dimPt2, parallelExt2);
                     DrawDimeValue(dimPt1, dimPt2, drawText);
                    break;
                default: break;
            }
        }
        private void DrawDimeValue(PointF pt1, PointF pt2, string value)
        {
            Matrix m;
            float adju = pt2.X - pt1.X;
            float opp = pt2.Y - pt1.Y;
            float txtloc = (pt2.X + pt1.X);
            float slope = 0;
            float txtY = 0;
            float length = (float)Math.Sqrt((adju * adju) + (opp * opp));
            float angle = (float)Math.Asin(opp / length);
            PointF pt;
            if (adju != 0)
            {
                slope = opp / adju;
            }


            switch (TextPosition)
            {
                case TextLoc.oneThird:
                    txtY = slope * 0.3f * txtloc - slope * pt1.X + pt1.Y;
                    pt = new PointF(0.3f * txtloc, txtY);
                    m = Matrix.Rotate(-angle * 180 / (float)Math.PI);
                    float[] reverse1 = m.VectorMultiply(new float[] { pt.X, pt.Y, 1 });
                    pt = new PointF(reverse1[0], reverse1[1]);
                     Graphic.Save();
                     Graphic.Rotate(angle);
                     Graphic.DrawText(value, pt);
                     Graphic.Restore();

                    break;
                case TextLoc.middle:
                    txtY = slope * 0.5f * txtloc - slope * pt1.X + pt1.Y;
                    if (adju == 0)
                    {
                        txtY = 0.5f * (pt2.Y + pt1.Y);
                    }
                    pt = new PointF(0.5f * txtloc, txtY);
                    m = Matrix.Rotate(-angle * 180 / (float)Math.PI);
                    float[] reverse = m.VectorMultiply(new float[] { pt.X, pt.Y, 1 });
                    pt = new PointF(reverse[0], reverse[1]);
                     Graphic.Save();
                     Graphic.Rotate(angle);
                     Graphic.DrawText(value, pt);
                     Graphic.Restore();

                    break;
                case TextLoc.twoThird:
                    txtY = slope * 0.7f * txtloc - slope * pt1.X + pt1.Y;
                    pt = new PointF(0.7f * txtloc, txtY);
                    m = Matrix.Rotate(-angle * 180 / (float)Math.PI);
                    float[] reverse3 = m.VectorMultiply(new float[] { pt.X, pt.Y, 1 });
                    pt = new PointF(reverse3[0], reverse3[1]);
                    Graphic.Save();
                     Graphic.Rotate(angle);
                     Graphic.DrawText(value, pt);
                    Graphic.Restore();
                    break;
                default: break;
            }
        }

        public void DrawAdjDimLine(PointF pt1, PointF pt2, string value)
        {
            float startY;
            PointF start, end;
            switch (DimLoc)
            {
                case DimensionLocation.start:
                    if (pt1.Y < pt2.Y)
                    {
                        startY = pt1.Y;
                    }
                    else
                    {
                        startY = pt2.Y;
                    }
                    start = new PointF(pt1.X, startY);
                    end = new PointF(pt2.X, startY);
                     DrawDimLineAligned(start, end, value);

                    break;
                case DimensionLocation.end:
                    if (pt1.Y > pt2.Y)
                    {
                        startY = pt1.Y;
                    }
                    else
                    {
                        startY = pt2.Y;
                    }
                    start = new PointF(pt1.X, startY);
                    end = new PointF(pt2.X, startY);
                     DrawDimLineAligned(start, end, value);
                    break;
                default: break;

            }
        }
        public void DrawOppoDimLine(PointF pt1, PointF pt2, string value)
        {
            float startX;
            PointF start, end;
            switch (DimLoc)
            {
                case DimensionLocation.start:
                    if (pt1.X < pt2.X)
                    {
                        startX = pt1.X;
                    }
                    else
                    {
                        startX = pt2.X;
                    }
                    start = new PointF(startX, pt1.Y);
                    end = new PointF(startX, pt2.Y);
                     DrawDimLineAligned(start, end, value);

                    break;
                case DimensionLocation.end:
                    if (pt1.X > pt2.X)
                    {
                        startX = pt1.X;
                    }
                    else
                    {
                        startX = pt2.X;
                    }
                    start = new PointF(startX, pt1.Y);
                    end = new PointF(startX, pt2.Y);
                     DrawDimLineAligned(start, end, value);
                    break;
                default: break;

            }
        }
        public void DrawTextAtAngle(PointF pt1, PointF pt2, string text)
        {
            Matrix m;
            float adju = pt2.X - pt1.X;
            float opp = pt2.Y - pt1.Y;
            float txtloc = (pt2.X + pt1.X);

            float length = (float)Math.Sqrt((adju * adju) + (opp * opp));
            float angle = (float)Math.Asin(opp / length);
            PointF pt;
            pt = pt1;
            m = Matrix.Rotate(-angle * 180 / (float)Math.PI);
            float[] reverse1 = m.VectorMultiply(new float[] { pt.X, pt.Y, 1 });
            pt = new PointF(reverse1[0], reverse1[1]);
             Graphic.Save();
             Graphic.Rotate(angle);
             Graphic.DrawText(text, pt);
            Graphic.Restore();
        }
        public void DrawTextAtAngleMid(PointF pt1, PointF pt2, string text)
        {
            Matrix m;
            float adju = pt2.X - pt1.X;
            float opp = pt2.Y - pt1.Y;
            float txtloc = (pt2.X + pt1.X);

            float length = (float)Math.Sqrt((adju * adju) + (opp * opp));
            float angle = (float)Math.Asin(opp / length);
            PointF pt=new PointF(0.5f*(pt1.X+pt2.X),0.5f*(pt1.Y+pt2.Y));
         
            m = Matrix.Rotate(-angle * 180 / (float)Math.PI);
            float[] reverse1 = m.VectorMultiply(new float[] { pt.X, pt.Y, 1 });
            pt = new PointF(reverse1[0], reverse1[1]);
            Graphic.Save();
            Graphic.Rotate(angle);
             Graphic.DrawText(text, pt);
             Graphic.Restore();
        }
        // angle in degree
        public void DrawTextAtAngle(PointF pt1, float ang, string text)
        {
            float angle = ang;
            PointF pt;
            pt = pt1;
            Matrix m = Matrix.Rotate(-angle);
            float[] reverse1 = m.VectorMultiply(new float[] { pt.X, pt.Y, 1 });
            pt = new PointF(reverse1[0], reverse1[1]);
             Graphic.Save();
             Graphic.Rotate(angle*(float)Math.PI/180);
            Graphic.DrawText(text, pt);
             Graphic.Restore();
        }
    }
}
