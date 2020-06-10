
using EngDolphin.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static EngDolphin.Canvas.CanvasEnum;

namespace EngDolphin.Client.Models
{
    public class DovDrawings
    {
        public string DrwColor { get; set; } = "green";
        public string FillColorOpt { get; set; } = "yellow";
        public float LineThickness { get; set; } = 2f;
        public LineCap DovLineCap { get; set; } = LineCap.Butt;
        public LineJoin DovLineJoin { get; set; } = LineJoin.Miter;
        public string TextFont { get; set; } = "15px Arial";
        public TextAlign TextAlignment { get; set; } = TextAlign.Center;
        public TextBaseline TextBase { get; set; } = TextBaseline.Top;
        public TextDirection TextDire { get; set; } = TextDirection.LTR;
        public CanvasContext Graphic { get; set; }
       
        public DovDrawings(CanvasContext cont)
        {
            Graphic = cont;
        }
        public void DrawRec(PointF pt1, PointF pt2)
        {
            float xmin = Math.Min(pt1.X, pt2.X);
            float ymin = Math.Min(pt1.Y, pt2.Y);
            float width= Math.Abs(pt1.X- pt2.X);
            float height = Math.Abs(pt1.Y - pt2.Y);
          
             Graphic.Stroke();
            //await Graphic.SetFillStyleAsync(FillColor);
             Graphic.SetStrokeStyle(DrwColor);
            Graphic.StrokeRect(xmin, ymin,width, height);
            Graphic.Stroke();
        }
        public void DrawRec(PointF pt1, float width, float height)
        {
             Graphic.Stroke();
             Graphic.SetFillStyle(FillColorOpt);
             Graphic.StrokeRect(pt1.X, pt1.Y, width, height);
             Graphic.Stroke();
        }

        public void DrawLine(PointF pt1, PointF pt2, bool? lineCap = null, bool? lineJoin = null)
        {
            Graphic.Stroke();
             Graphic.SetLineWidth(LineThickness);
            Graphic.SetStrokeStyle(DrwColor);
             Graphic.BeginPath();
            Graphic.MoveTo(pt1.X, pt1.Y);
            Graphic.LineTo(pt2.X, pt2.Y);
            Graphic.SetLineCap(DovLineCap);
             Graphic.SetLineJoin(DovLineJoin);
             Graphic.Stroke();
        }
        private void DrawLineTo(PointF pt1)
        {
            Graphic.LineTo(pt1.X, pt1.Y);

        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            Graphic.Stroke();
            Graphic.SetLineWidth(LineThickness);
             Graphic.SetStrokeStyle(DrwColor);
           Graphic.BeginPath();
             Graphic.SetLineCap(LineCap.Square);
             Graphic.MoveTo(x1, y1);
             Graphic.LineTo(x2, y2);
             Graphic.Stroke();
        }
        public void DrawRecSection(PointF pt, float w, float h,ChartStyle cs)
        {
            float wdth = w;
            float hgt = h;
           
            PointF pt2 = new PointF(wdth * 0.5f, -hgt * 0.5f);
            PointF pt3 = new PointF(wdth * 0.5f, hgt * 0.5f);
            PointF pt4 = new PointF(-wdth * 0.5f, hgt * 0.5f);
            PointF[] polygon = new[] {cs. Point2D(pt2),cs. Point2D(pt3),cs. Point2D(pt4) };
             Graphic.Stroke();
           Graphic.SetLineWidth(LineThickness);
            Graphic.SetStrokeStyle(DrwColor);
            Graphic.BeginPath();
            Graphic.MoveTo(cs.Point2D(pt).X,cs. Point2D(pt).Y);
            //List<Task> secTask = new List<Task>();
            for (int i = 0; i < polygon.Length; i++)
            {
                 DrawLineTo(polygon[i]);
            }
            //await Task.WhenAll(secTask);
            Graphic.SetFillStyle(FillColorOpt);
             Graphic.ClosePath();
            Graphic.Fill();

        }
        public void Clear(double x, double y, double w, double h)
        {
            Graphic.ClearRect(x, y, w, h);
        }
        public void FillColor(double x, double y, double w, double h, string background)
        {
             Graphic.SetFillStyle(background);
            Graphic.FillRect(x, y, w, h);
        }
        public void FillColor(PointF pt1, PointF pt2)
        {
            Graphic.Stroke();
            Graphic.SetLineWidth(LineThickness);
           Graphic.BeginPath();
            Graphic.MoveTo(pt1.X, pt1.Y);
            Graphic.LineTo(pt2.X, pt1.Y);
             Graphic.LineTo(pt2.X, pt2.Y);
             Graphic.LineTo(pt1.X, pt2.Y);
             Graphic.SetFillStyle(FillColorOpt);
             Graphic.ClosePath();
            Graphic.Fill();
        }
        public void DrawArc(double x, double y, double radius, double startAngle, double endAngle,bool?antiClockwise=null)
        {
           Graphic.SetStrokeStyle(DrwColor);
            Graphic.SetFillStyle(FillColorOpt);
            Graphic.BeginPath();
             Graphic.Arc(x, y, radius, startAngle, endAngle,antiClockwise);
             Graphic.Fill();
             Graphic.Stroke();
        }

        public void DrawPolyLine(PointF[] pt)
        {
             Graphic.Stroke();
             Graphic.SetLineWidth(LineThickness);
            Graphic.SetStrokeStyle(DrwColor);
             Graphic.BeginPath();
             Graphic.MoveTo(pt[0].X, pt[0].Y);
            for (int i = 1; i < pt.Length; i++)
            {

                 Graphic.LineTo(pt[i].X, pt[i].Y);
            }
             Graphic.Stroke();
        }
        public void FillPolygone(PointF[] pt)
        {
            Graphic.Stroke();
             Graphic.SetLineWidth(LineThickness);
            Graphic.SetStrokeStyle(DrwColor);
             Graphic.SetFillStyle(FillColorOpt);
             Graphic.BeginPath();
             Graphic.MoveTo(pt[0].X, pt[0].Y);
            for (int i = 1; i < pt.Length; i++)
            {
                 Graphic.LineTo(pt[i].X, pt[i].Y);
            }
            Graphic.ClosePath();
             Graphic.Fill();
            Graphic.Stroke();
        }
        public void DrawText(string text, PointF pt)
        {

            Graphic.SetStrokeStyle(DrwColor);
            Graphic.SetFillStyle(FillColorOpt);
            Graphic.SetTextAlign(TextAlignment);
            Graphic.SetTextBaseline(TextBase);
            Graphic.SetTextDirection(TextDire);
            Graphic.SetFont(TextFont);
            Graphic.FillText(text, pt.X, pt.Y);
        }
        public void Rotate(float angle)
        {
             Graphic.Rotate(angle);
        }
        public void Save()
        {
            Graphic.Save();
        }
        public void Restore()
        {
            Graphic.Restore();
        }
        //public async Task<float> MeasureTextWidthAsync(string text)
        //{
        //  return  await Graphic.MeasureTextAsync(text);
        //}
        public void Translate(float x,float y)
        {
            Graphic.Translate(x,y);
        }

    }
}
