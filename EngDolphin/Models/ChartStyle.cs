using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class ChartStyle
    {
        public RectangleF ChartArea { get; set; }
        public RectangleF PlotArea { get; set; }
        public string ChartBackColor { get; set; }
        public string ChartBorderColor { get; set; }
        public string PlotBackColor { get; set; } = "white";
        public string PlotBorderColor { get; set; } = "black";
        public float XMin { get; set; } = -150;
        public float XMax { get; set; } = 150;
        public float YMin { get; set; } = -200;
        public float YMax { get; set; } = 200;
        public float ZMin { get; set; } = -4;
        public float ZMax { get; set; } = 4;
        public float MarginLef { get; set; } = 10;
        public float MarginBottom { get; set; } = 10;
        public float MarginRight { get; set; } = 10;
        public float MarginTop { get; set; } = 10;
        //private DashStyle gridPattern = DashStyle.Solid;
        public string GridColor { get; set; } = "gray";
        public float gridLineThickness { get; set; } = 1.0f;
        public bool IsXGrid { get; set; } = true;
        public bool IsYGrid { get; set; } = true;
        public bool IsZGrid { get; set; } = true;
        public string XLabel { get; set; } = "Mx (kNm)";
        public string YLabel { get; set; } = "My (kNm)";
        public string ZLabel { get; set; } = "Pn (kNm)";
        public string STitle { get; set; } = "Title";
        public string LabelFont { get; set; } = "8pt Arial";

        public string LabelFontColor { get; set; } = "black";
        public string TitleFont { get; set; } = "12pt Arial";
        public string Title { get; set; } = "Column Interaction Chart";
        public bool IsColorBar { get; set; } = false;

        public string TitleFontColor { get; set; } = "black";
        public float XTick { get; set; } = 1f;
        public float YTick { get; set; } = 1f;
        public float ZTick { get; set; } = 1f;
        public float TickLength { get; set; } = 10f;
        public string TickFont { get; set; }
        public string TickFontColor { get; set; } = "white";
        public float Elevation { get; set; } = 35;
        public float Azimuth { get; set; } = 45;
        public ChartStyle(float xMin, float xMax, float yMin, float yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
        }
        public ChartStyle()
        {

        }
        public PointF Point2D(PointF ptf)
        {
            PointF aPoint = new PointF();
            if (ptf.X < XMin || ptf.X > XMax || ptf.Y < YMin || ptf.Y > YMax)
            {
                ptf.X = Single.NaN; ptf.Y = Single.NaN;
            }
            aPoint.X = MarginLef + (ptf.X - XMin) * PlotArea.Width / (XMax - XMin);
            aPoint.Y = MarginTop + PlotArea.Height - (ptf.Y - YMin) * PlotArea.Height / (YMax - YMin);
            return aPoint;
        }
        public float ScaleX(float x)
        {
            float hor ;
            if (PlotArea.Width >= PlotArea.Height)
            {
                decimal fac = (decimal)((PlotArea.Width / (XMax - XMin)));
                hor = (float)Math.Abs(fac) * x;

            }
            else
            {
                decimal fac = (decimal)(PlotArea.Height / (YMax - YMin));
                hor = (float)Math.Abs(fac) * x;
            }
            return hor;
        }
        public void DrawGrid(DovDrawings g)
        {
            float xlimMin = XMin;
            float xlimMax = XMax;
            float ylimMin = YMin;
            float ylimMax = YMax;
            float start = XMin;
            float end = XMax;
            g.DrwColor = "green";
            g.LineThickness = 0.7f;
            //List<Task> gridx = new List<Task>();
            for (float i = start; i <= end; i += XTick)
            {
               g.DrawLine(Point2D(new PointF(i, ylimMin)), Point2D(new PointF(i, ylimMax)));
            }
            //await Task.WhenAll(gridx);
            g.DrwColor = "#0cd5f0";
            g.LineThickness = 0.3f;
            start = YMin;
            end = YMax;
            //List<Task> gridy = new List<Task>();
            for (float j = start; j <= end; j += YTick)
            {
                 
              g.DrawLine(Point2D(new PointF(xlimMin, j)), Point2D(new PointF(xlimMax, j)));
            }
            //await Task.WhenAll(gridy);
        }
        public void DrawTickMark(DovDrawings g)
        {
            float xlimMin = XMin;
            float xlimMax = XMax;
            float ylimMin = YMin;
            float ylimMax = YMax;
            float start = XMin;
            float end = XMax;
            g.DrwColor = "green";
            g.LineThickness = 0.7f;

            //List<Task> gridx = new List<Task>();
            for (float i = start; i <= end; i += XTick)
            {
                PointF xPoint = Point2D(new PointF(i, ylimMin));
                 g.DrawLine(new PointF(xPoint.X, xPoint.Y + TickLength), Point2D(new PointF(i, ylimMin)));
                g.TextFont = TickFont;
                g.FillColorOpt = TickFontColor;
                g.TextBase = Canvas.CanvasEnum.TextBaseline.Top;
                g.TextAlignment = Canvas.CanvasEnum.TextAlign.Center;
                g.DrawText(Math.Round(i,2).ToString(), new PointF(xPoint.X, xPoint.Y + TickLength));
            }
            g.DrwColor = "#0cd5f0";
            g.LineThickness = 0.3f;
            start = YMin;
            end = YMax;
            for (float j = start; j <= end; j += YTick)
            {
                PointF yPoint = Point2D(new PointF(xlimMin, j));
                 g.DrawLine(new PointF(yPoint.X - TickLength, yPoint.Y), Point2D(new PointF(xlimMin, j)));
                g.TextFont = TickFont;
                g.FillColorOpt = TickFontColor;
                g.TextBase = Canvas.CanvasEnum.TextBaseline.Middle;
                g.TextAlignment = Canvas.CanvasEnum.TextAlign.Right;
                g.DrawText(Math.Round(j,2).ToString(), new PointF(yPoint.X - TickLength, yPoint.Y));
            }
        }
        public void AddChartStyle(DovDrawings g) {
           
             Add3DAxes(g);
             Add3DGrids(g);
             Add3DTicks(g);
             Add3DLabels(g);
        }
        private Point3[] CoordinatesOfChartBox()
        {
            // Create coordinate of the axes:
            Point3[] pta = new Point3[8];
            pta[0] = new Point3(XMax, YMin, ZMin, 1);
            pta[1] = new Point3(XMin, YMin, ZMin, 1);
            pta[2] = new Point3(XMin, YMax, ZMin, 1);
            pta[3] = new Point3(XMin, YMax, ZMax, 1);
            pta[4] = new Point3(XMin, YMin, ZMax, 1);
            pta[5] = new Point3(XMax, YMin, ZMax, 1);
            pta[6] = new Point3(XMax, YMax, ZMax, 1);
            pta[7] = new Point3(XMax, YMax, ZMin, 1);
            Point3[] pts = new Point3[4];
            int[] npts = new int[4] { 0, 1, 2, 3 };
            if (Elevation >= 0)
            {
                if (Azimuth >= -180 && Azimuth < -90)
                    npts = new int[4] { 1, 2, 7, 6 };
                else if (Azimuth >= -90 && Azimuth < 0)
                    npts = new int[4] { 0, 1, 2, 3 };
                else if (Azimuth >= 0 && Azimuth < 90)
                    npts = new int[4] { 7, 0, 1, 4 };
                else if (Azimuth >= 90 && Azimuth <= 180)
                    npts = new int[4] { 2, 7, 0, 5 };
            }
            else if (Elevation < 0)
            {
                if (Azimuth >= -180 && Azimuth < -90)
                    npts = new int[4] { 1, 0, 7, 6 };
                else if (Azimuth >= -90 && Azimuth < 0)
                    npts = new int[4] { 0, 7, 2, 3 };
                else if (Azimuth >= 0 && Azimuth < 90)
                    npts = new int[4] { 7, 2, 1, 4 };
                else if (Azimuth >= 90 && Azimuth <= 180)
                    npts = new int[4] { 2, 1, 0, 5 };
            }
            for (int i = 0; i < 4; i++)
                pts[i] = pta[npts[i]];
            return pts;
        }
        private void Add3DAxes(DovDrawings g)
        {
            Matrix3 m = Matrix3.AzimuthElevation(Elevation, Azimuth);
            Point3[] pts = CoordinatesOfChartBox();
            g.DrwColor = "red";
            g.LineThickness = 2;
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i].Transform(m, this);
            }
            g.DrawLine(pts[0].X, pts[0].Y, pts[1].X, pts[1].Y);
             g.DrawLine(pts[1].X, pts[1].Y, pts[2].X, pts[2].Y);
             g.DrawLine(pts[2].X, pts[2].Y, pts[3].X, pts[3].Y);
        }
        private void Add3DTicks(DovDrawings g)
        {
            Matrix3 m = Matrix3.AzimuthElevation(Elevation, Azimuth);
            Point3[] pta = new Point3[2];
            Point3[] pts = CoordinatesOfChartBox();
            g.DrwColor = "green";
            // Add x ticks:
            float offset = (YMax - YMin) / 20.0f;
            float ticklength = offset;
            for (float x = XMin; x <= XMax; x = x + XTick)
            {
                if (Elevation >= 0)
                {
                    if (Azimuth >= -90 && Azimuth < 90)
                        ticklength = -offset;
                }
                else if (Elevation < 0)
                {
                    if ((Azimuth >= -180 && Azimuth < -90) ||
                    Azimuth >= 90 && Azimuth <= 180)
                        ticklength = -(YMax - YMin) / 30;
                }
                pta[0] = new Point3(x, pts[1].Y + ticklength,
                pts[1].Z, pts[1].W);
                pta[1] = new Point3(x, pts[1].Y, pts[1].Z, pts[1].W);
                for (int i = 0; i < pta.Length; i++)
                {
                    pta[i].Transform(m, this);
                }
                 g.DrawLine(pta[0].X, pta[0].Y, pta[1].X, pta[1].Y);
            }
            // Add y ticks:
            offset = (XMax - XMin) / 20.0f;
            ticklength = offset;
            for (float y = YMin; y <= YMax; y = y + YTick)
            {
                pts = CoordinatesOfChartBox();
                if (Elevation >= 0)
                {
                    if (Azimuth >= -180 && Azimuth < 0)
                        ticklength = -offset;
                }
                else if (Elevation < 0)
                {
                    if (Azimuth >= 0 && Azimuth < 180)
                        ticklength = -offset;
                }
                pta[0] = new Point3(pts[1].X + ticklength, y,
                pts[1].Z, pts[1].W);
                pta[1] = new Point3(pts[1].X, y, pts[1].Z, pts[1].W);
                for (int i = 0; i < pta.Length; i++)
                {
                    pta[i].Transform(m, this);
                }
                 g.DrawLine(pta[0].X, pta[0].Y, pta[1].X, pta[1].Y);
            }
            // Add z ticks:
            float xoffset = (XMax - XMin) / 30.0f;
            float yoffset = (YMax - YMin) / 30.0f;
            float xticklength = xoffset;
            float yticklength = yoffset;
            for (float z = ZMin; z <= ZMax; z = z + ZTick)
            {
                if (Elevation >= 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        xticklength = 0;
                        yticklength = yoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        xticklength = xoffset;
                        yticklength = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        xticklength = 0;
                        yticklength = -yoffset;
                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        xticklength = -xoffset;
                        yticklength = 0;
                    }
                }
                else if (Elevation < 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        yticklength = 0;
                        xticklength = xoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        yticklength = -yoffset;
                        xticklength = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        yticklength = 0;
                        xticklength = -xoffset;
                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        yticklength = yoffset;
                        xticklength = 0;
                    }
                }
                pta[0] = new Point3(pts[2].X, pts[2].Y, z, pts[2].W);
                pta[1] = new Point3(pts[2].X + yticklength,
                pts[2].Y + xticklength, z, pts[2].W);
                for (int i = 0; i < pta.Length; i++)
                {
                    pta[i].Transform(m, this);
                }
                g.DrawLine(pta[0].X, pta[0].Y, pta[1].X, pta[1].Y);
            }
        }
        private void Add3DGrids(DovDrawings g)
        {
            Matrix3 m = Matrix3.AzimuthElevation(Elevation, Azimuth);
            Point3[] pta = new Point3[3];
            Point3[] pts;
            g.DrwColor = "white";
            g.LineThickness = 0.5f;
            // Draw x gridlines:
            if (IsXGrid)
            {
                for (float x = XMin; x <= XMax; x = x + XTick)
                {
                    pts = CoordinatesOfChartBox();
                    pta[0] = new Point3(x, pts[1].Y, pts[1].Z, pts[1].W);
                    if (Elevation >= 0)
                    {
                        if ((Azimuth >= -180 && Azimuth < -90) ||
                        (Azimuth >= 0 && Azimuth < 90))
                        {
                            pta[1] = new Point3(x, pts[0].Y,
                            pts[1].Z, pts[1].W);
                            pta[2] = new Point3(x, pts[0].Y,
                            pts[3].Z, pts[1].W);
                        }
                        else
                        {
                            pta[1] = new Point3(x, pts[2].Y,
                            pts[1].Z, pts[1].W);
                            pta[2] = new Point3(x, pts[2].Y,
                            pts[3].Z, pts[1].W);
                        }
                    }
                    else if (Elevation < 0)
                    {
                        if ((Azimuth >= -180 && Azimuth < -90) ||
                        (Azimuth >= 0 && Azimuth < 90))
                        {
                            pta[1] = new Point3(x, pts[2].Y,
                            pts[1].Z, pts[1].W);
                            pta[2] = new Point3(x, pts[2].Y,
                            pts[3].Z, pts[1].W);
                        }
                        else
                        {
                            pta[1] = new Point3(x, pts[0].Y,
                            pts[1].Z, pts[1].W);
                            pta[2] = new Point3(x, pts[0].Y,
                            pts[3].Z, pts[1].W);
                        }
                    }
                    for (int i = 0; i < pta.Length; i++)
                    {
                        pta[i].Transform(m, this);
                    }
                     g.DrawLine(pta[0].X, pta[0].Y, pta[1].X, pta[1].Y);
                     g.DrawLine(pta[1].X, pta[1].Y, pta[2].X, pta[2].Y);
                }
            }
                // Draw y gridlines:
                if (IsYGrid)
                {
                    for (float y = YMin; y <= YMax; y = y + YTick)
                    {
                        pts = CoordinatesOfChartBox();
                        pta[0] = new Point3(pts[1].X, y, pts[1].Z, pts[1].W);
                        if (Elevation >= 0)
                        {
                            if ((Azimuth >= -180 && Azimuth < -90) ||
                            (Azimuth >= 0 && Azimuth < 90))
                            {
                                pta[1] = new Point3(pts[2].X, y,
                                pts[1].Z, pts[1].W);
                                pta[2] = new Point3(pts[2].X, y,
                                pts[3].Z, pts[1].W);
                            }
                            else
                            {
                                pta[1] = new Point3(pts[0].X, y,
                                pts[1].Z, pts[1].W);
                                pta[2] = new Point3(pts[0].X, y,
                                pts[3].Z, pts[1].W);
                            }
                        }
                        if (Elevation < 0)
                        {
                            if ((Azimuth >= -180 && Azimuth < -90) ||
                            (Azimuth >= 0 && Azimuth < 90))
                            {
                                pta[1] = new Point3(pts[0].X, y,
                                pts[1].Z, pts[1].W);
                                pta[2] = new Point3(pts[0].X, y,
                                pts[3].Z, pts[1].W);
                            }
                            else
                            {
                                pta[1] = new Point3(pts[2].X, y,
                                pts[1].Z, pts[1].W);
                                pta[2] = new Point3(pts[2].X, y,
                                pts[3].Z, pts[1].W);
                            }
                        }
                        for (int i = 0; i < pta.Length; i++)
                        {
                            pta[i].Transform(m, this);
                        }
                        g.DrawLine(pta[0].X, pta[0].Y,
                          pta[1].X, pta[1].Y);
                        g.DrawLine(pta[1].X, pta[1].Y,
                          pta[2].X, pta[2].Y);
                    }
                }
            //Draw Z gridlines:
            if (IsZGrid)
            {
                for (float z = ZMin; z <= ZMax; z = z + ZTick)
                {
                    pts = CoordinatesOfChartBox();
                    pta[0] = new Point3(pts[2].X, pts[2].Y, z, pts[2].W);
                    if (Elevation >= 0)
                    {
                        if ((Azimuth >= -180 && Azimuth < -90) ||
                        (Azimuth >= 0 && Azimuth < 90))
                        {
                            pta[1] = new Point3(pts[2].X,
                            pts[0].Y, z, pts[1].W);
                            pta[2] = new Point3(pts[0].X,
                            pts[0].Y, z, pts[1].W);
                        }
                        else
                        {
                            pta[1] = new Point3(pts[0].X,
                            pts[2].Y, z, pts[1].W);
                            pta[2] = new Point3(pts[0].X,
                            pts[1].Y, z, pts[1].W);
                        }
                    }
                    if (Elevation < 0)
                    {
                        if ((Azimuth >= -180 && Azimuth < -90) ||
                        (Azimuth >= 0 && Azimuth < 90))
                        {
                            pta[1] = new Point3(pts[0].X,
                            pts[2].Y, z, pts[1].W);
                            pta[2] = new Point3(pts[0].X,
                            pts[0].Y, z, pts[1].W);
                        }
                        else
                        {
                            pta[1] = new Point3(pts[2].X,
                            pts[0].Y, z, pts[1].W);
                            pta[2] = new Point3(pts[0].X,
                            pts[0].Y, z, pts[1].W);
                        }
                    }
                    for (int i = 0; i < pta.Length; i++)
                    {
                        pta[i].Transform(m, this);
                    }
                    g.DrawLine(pta[0].X, pta[0].Y, pta[1].X, pta[1].Y);
                    g.DrawLine(pta[1].X, pta[1].Y, pta[2].X, pta[2].Y);
                }
            }
        }
        private void Add3DLabels(DovDrawings g)
        {
            Matrix3 m = Matrix3.AzimuthElevation(Elevation, Azimuth);
            Point3 pt = new Point3();
            Point3[] pts = CoordinatesOfChartBox();
            // Add x tick labels:
            float offset = (YMax - YMin) / 20;
            float labelSpace = offset;
            g.FillColorOpt = "white";
            g.TextFont = LabelFont;
            g.TextAlignment = Canvas.CanvasEnum.TextAlign.Right;
            for (float x = XMin + XTick; x < XMax; x = x + XTick)
            {
                if (Elevation >= 0)
                {
                    if (Azimuth >= -90 && Azimuth < 90)
                        labelSpace = -offset;
                }
                else if (Elevation < 0)
                {
                    if ((Azimuth >= -180 && Azimuth < -90) ||
                    Azimuth >= 90 && Azimuth <= 180)
                        labelSpace = -offset;
                }
                pt = new Point3(x, pts[1].Y + labelSpace, pts[1].Z, pts[1].W);
                pt.Transform(m, this);
               
               g.DrawText(Math.Round(x,2).ToString(), new PointF(pt.X, pt.Y));

            }
            // Add y tick labels:
            offset = (XMax - XMin) / 20;
            labelSpace = offset;
            g.TextAlignment = Canvas.CanvasEnum.TextAlign.Left;
            for (float y = YMin + YTick; y < YMax; y = y + YTick)
            {
                pts = CoordinatesOfChartBox();
                if (Elevation >= 0)
                {
                    if (Azimuth >= -180 && Azimuth < 0)
                        labelSpace = -offset;
                }
                else if (Elevation < 0)
                {
                    if (Azimuth >= 0 && Azimuth < 180)
                        labelSpace = -offset;
                }
                pt = new Point3(pts[1].X + labelSpace, y, pts[1].Z, pts[1].W);
                pt.Transform(m, this);
                g.DrawText(Math.Round(y,2).ToString(), new PointF(pt.X, pt.Y));

            }
            // Add z tick labels:
            float xoffset = (XMax - XMin) / 30.0f;
            float yoffset = (YMax - YMin) / 15.0f;
            float xlabelSpace = xoffset;
            float ylabelSpace = yoffset;
            float txtHeight = 8;
            string font = "pt Verenda";
            font = txtHeight + font;
            LabelFont = font;
            g.TextFont =LabelFont;
           
            g.TextAlignment = Canvas.CanvasEnum.TextAlign.Right;
            for (float z = ZMin; z <= ZMax; z = z + ZTick)
            {
              
                pts = CoordinatesOfChartBox();
                if (Elevation >= 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        xlabelSpace = 0;
                        ylabelSpace = yoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        xlabelSpace = xoffset;
                        ylabelSpace = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        xlabelSpace = 0;
                        ylabelSpace = -yoffset;
                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        xlabelSpace = -xoffset;
                        ylabelSpace = 0;
                    }
                }
                else if (Elevation < 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        ylabelSpace = 0;
                        xlabelSpace = xoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        ylabelSpace = -yoffset;
                        xlabelSpace = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        ylabelSpace = 0;
                        xlabelSpace = -xoffset;
                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        ylabelSpace = yoffset;
                        xlabelSpace = 0;
                    }
                }
                pt = new Point3(pts[2].X + ylabelSpace,
                pts[2].Y + xlabelSpace, z, pts[2].W);
                pt.Transform(m, this);
                g.TextBase = Canvas.CanvasEnum.TextBaseline.Middle;
                g.TextAlignment = Canvas.CanvasEnum.TextAlign.Right;
                g.DrawText(Math.Round(z,2).ToString(), new PointF(pt.X - labelSpace, pt.Y - txtHeight / 2));

            }
            // Add Title:

            if (Title != "No Title")
            {
                g.TextFont = TitleFont;
                 g.DrawText(Title, new PointF(ChartArea.Width / 2, ChartArea.Height / 30));
            }
            // Add x axis label:
            offset = (YMax - YMin) / 3;
            labelSpace = offset;
            
            float offset1 = (XMax - XMin) / 10;
            float xc = offset1;
            if (Elevation >= 0)
            {
                if (Azimuth >= -90 && Azimuth < 90)
                    labelSpace = -offset;
                if (Azimuth >= 0 && Azimuth <= 180)
                    xc = -offset1;
            }
            else if (Elevation < 0)
            {
                if ((Azimuth >= -180 && Azimuth < -90) ||
                Azimuth >= 90 && Azimuth <= 180)
                    labelSpace = -offset;
                if (Azimuth >= -180 && Azimuth <= 0)
                    xc = -offset1;
            }
            Point3[] pta = new Point3[2];
            pta[0] = new Point3(XMin, pts[1].Y + labelSpace,
            pts[1].Z, pts[1].W);
            pta[1] = new Point3((XMin + XMax) / 2 - xc,
            pts[1].Y + labelSpace, pts[1].Z, pts[1].W);
            pta[0].Transform(m,  this);
            pta[1].Transform(m,  this);
            float theta = (float)Math.Atan((pta[1].Y - pta[0].Y) /
            (pta[1].X - pta[0].X));
          
             g.Save();
            g.TextBase = Canvas.CanvasEnum.TextBaseline.Middle;
            g.Translate(pta[1].X, pta[1].Y);
             g.Rotate(theta);
            g.DrawText(XLabel, new PointF( 0,0));

            g.Restore();
            // Add y axis label:
            offset = (XMax - XMin) / 3;
            offset1 = (YMax - YMin) / 5;
            labelSpace = offset;
            float yc = YTick;
            if (Elevation >= 0)
            {
                if (Azimuth >= -180 && Azimuth < 0)
                    labelSpace = -offset;
                if (Azimuth >= -90 && Azimuth <= 90)
                    yc = -offset1;
            }
            else if (Elevation < 0)
            {
                yc = -offset1;
                if (Azimuth >= 0 && Azimuth < 180)
                    labelSpace = -offset;
                if (Azimuth >= -90 && Azimuth <= 90)
                    yc = offset1;
            }
            pta[0] = new Point3(pts[1].X + labelSpace, YMin, pts[1].Z, pts[1].W);

            pta[1] = new Point3(pts[1].X + labelSpace, (YMin + YMax) / 2 + yc, pts[1].Z, pts[1].W);

            pta[0].Transform(m,  this);
            pta[1].Transform(m,  this);
            theta = (float)Math.Atan((pta[1].Y - pta[0].Y) /
            (pta[1].X - pta[0].X));
            
            g.Save();
            g.Translate(pta[1].X, pta[1].Y);
            g.Rotate(theta);
            g.DrawText(YLabel, new PointF(0, 0));

            g.Restore();
            // Add z axis labels:
            float zticklength = 10;
            labelSpace = -1.3f * offset;
            offset1 = (ZMax - ZMin) / 8;
            float zc = -offset1;
            for (float z = ZMin; z < ZMax; z = z + ZTick)
            {

                float size = 30f;/*await g.MeasureTextWidthAsync(z.ToString());*/
                if (zticklength < size)
                    zticklength = size;
            }
            float zlength = -zticklength;
            if (Elevation >= 0)
            {
                if (Azimuth >= -180 && Azimuth < -90)
                {
                    zlength = -zticklength;
                    labelSpace = -1.3f * offset;
                    zc = -offset1;
                }
                else if (Azimuth >= -90 && Azimuth < 0)
                {
                    zlength = zticklength;
                    labelSpace = 2 * offset / 3;
                    zc = offset1;
                }
                else if (Azimuth >= 0 && Azimuth < 90)
                {
                    zlength = zticklength;
                    labelSpace = 2 * offset / 3;
                    zc = -offset1;
                }
                else if (Azimuth >= 90 && Azimuth <= 180)
                {
                    zlength = -zticklength;
                    labelSpace = -1.3f * offset;
                    zc = offset1;
                }
            }
            else if (Elevation < 0)
            {
                if (Azimuth >= -180 && Azimuth < -90)
                {
                    zlength = -zticklength;
                    labelSpace = -1.3f * offset;
                    zc = offset1;
                }
                else if (Azimuth >= -90 && Azimuth < 0)
                {
                    zlength = zticklength;
                    labelSpace = 2 * offset / 3;
                    zc = -offset1;
                }
                else if (Azimuth >= 0 && Azimuth < 90)
                {
                    zlength = zticklength;
                    labelSpace = 2 * offset / 3;
                    zc = offset1;
                }
                else if (Azimuth >= 90 && Azimuth <= 180)
                {
                    zlength = -zticklength;
                    labelSpace = -1.3f * offset;
                    zc = -offset1;
                }
            }
            pta[0] = new Point3(pts[2].X - 1.6f*labelSpace, pts[2].Y,
            (ZMin + ZMax) / 2 + zc, pts[2].W);
            pta[0].Transform(m, this);
            /// rotate Graphic
             g.Save();
            g.TextBase = Canvas.CanvasEnum.TextBaseline.Middle;
            g.Translate(pta[0].X - zlength, pta[0].Y);
             g.Rotate(270*(float)Math.PI/180);
             g.DrawText(ZLabel, new PointF(0, 0));
            g.Restore();
        }
    }
}



