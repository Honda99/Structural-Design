using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using EngDolphin.Client.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using EngDolphin.Client.Extensions;
using EngDolphin.Canvas;
using static EngDolphin.Canvas.CanvasEnum;

namespace EngDolphin.Pages
{
    public class IsolatedFootingDsgnComp : ComponentBase
    {
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        protected ElementReference FootingCanvas { get; set; }
        protected ElementReference DiagramCanvas { get; set; }
        protected ElementReference DetailingCanvas { get; set; }
        protected bool doveShowMaterial = false;
        protected bool showFootingDimension = false;
        protected bool showAnalysisDesign = false;
        protected bool showDetailing = false;
        protected string displayConcMat = "dovOff";
        protected string displayRebarMat = "dovOff";
        protected bool dispDsgnPref = false;
        protected bool dispDsgnResult = false;
        protected bool continueDesign = false;
        protected bool continueDetailing = false;
        protected bool checkWidth = false;
        protected bool checkColLoc = false;
        protected string RunorPref = "Preference";
        protected string UniorBiAxial = "Uniaxial Chart";
        protected string dimError = "";
        protected string locationError = "";
        protected float test1;
        protected float test2;
        protected float test3;
        protected float test4;
        // Drawings 
        protected string footingBackColor { get; set; } = "white";
        protected string DsgnBackColor { get; set; } = "white";
        protected string DetailBackColor { get; set; } = "white";
        public long footingCanvWidth { get; set; } = 400;
        public long footingCanvHeight { get; set; } = 400;
        public long DiagramCanvWidth { get; set; } = 700;
        public long DiagramCanvHeight { get; set; } = 700;
        public long DetailingCanvWidth { get; set; } = 360;
        public long DetailingCanvHeight { get; set; } = 600;
        public float X { get; set; }
        public float Y { get; set; }
        protected List<DovConcMat> dovConcMatLibrary = new List<DovConcMat>();
        protected List<DovRebarMat> dovRebarMatLibrary = new List<DovRebarMat>();
        protected DovConcMat concMat = new DovConcMat();
        protected DovRebarMat rebarMat = new DovRebarMat();
        protected FootingData footingData = new FootingData();
        protected IsoFootingDesign fDesign = new IsoFootingDesign();
        protected DovDsgnPreference DsgnPref = new DovDsgnPreference();
        public DovDrawings FootingGraphics { get; set; }
        public DovDrawings DiagramGraphics { get; set; }
        public DovDrawings DetailingGraphics { get; set; }
        private CanvasContext diagramContext;
        private CanvasContext footingContext;
        private CanvasContext detailingContext;
        private ChartStyle footingChart;
        private ChartStyle diagramChart;
        private ChartStyle detailingChart;
        protected override void OnInitialized()
        {
            concMat = new DovConcMat() { Name = "25MPa", UnitWt = 25, Fck = 25, PoissonRatio = 0.2f, E = 29000 };
            dovConcMatLibrary.Add(concMat);
            concMat = new DovConcMat() { Name = "15MPa", UnitWt = 25, Fck = 15, PoissonRatio = 0.2f, E = 29000 };
            dovConcMatLibrary.Add(concMat);

            rebarMat = new DovRebarMat() { Name = "355MPa", UnitWt = 78.5f, Fyk = 355, PoissonRatio = 0.3f, E = 200000 };
            dovRebarMatLibrary.Add(rebarMat);
            rebarMat = new DovRebarMat() { Name = "400MPa", UnitWt = 78.5f, Fyk = 400, PoissonRatio = 0.3f, E = 200000 };
            dovRebarMatLibrary.Add(rebarMat);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            this.diagramContext = new CanvasContext(DiagramCanvas, JsRuntime);
            DiagramGraphics = new DovDrawings(diagramContext);
            //
            this.footingContext = new CanvasContext(FootingCanvas, JsRuntime);
            FootingGraphics = new DovDrawings(footingContext);
            this.detailingContext = new CanvasContext(DetailingCanvas, JsRuntime);
            DetailingGraphics = new DovDrawings(detailingContext);
            footingChart = new ChartStyle();
            diagramChart = new ChartStyle();
            detailingChart = new ChartStyle();
        }
        protected void ShowConcMatProp(ChangeEventArgs e)
        {
            foreach (var mat in dovConcMatLibrary)
            {
                if (mat.Name == e.Value.ToString())
                {
                    concMat = new DovConcMat() {Name=mat.Name,UnitWt=mat.UnitWt,Fck=mat.Fck,PoissonRatio=mat.PoissonRatio,E=mat.E,St=mat.St};
                }
            }
            displayConcMat = "dovShow";
        }

        protected void ShowRebarMatProp(ChangeEventArgs e)
        {
            foreach (var rebar in dovRebarMatLibrary)
            {
                if (rebar.Name == e.Value.ToString())
                {
                    rebarMat = new DovRebarMat() {Name=rebar.Name,UnitWt=rebar.UnitWt,Fyk=rebar.Fyk,PoissonRatio=rebar.PoissonRatio,E=rebar.E };
                }
            }
            displayRebarMat = "dovShow";
        }
        protected void ChangeConcMatProp()
        {
            if (concMat != null)
            {

                foreach (DovConcMat conc in dovConcMatLibrary.ToList())
                {
                    if (string.Equals(conc, concMat))
                    {
                        break;
                    }
                    else if (conc.Name == concMat.Name)
                    {
                        dovConcMatLibrary.Remove(conc);
                    }
                    else if (!dovConcMatLibrary.Contains(concMat))
                    {
                        dovConcMatLibrary.Add(concMat);
                    }
                    else
                    {

                    }
                }
                displayConcMat = "dovOff";
            }
        }
        protected void ChangeRebarMatProp()
        {
            if (rebarMat != null)
            {

                foreach (DovRebarMat rebar in dovRebarMatLibrary.ToList())
                {
                    if (string.Equals(rebar, rebarMat))
                    {
                        break;
                    }
                    else if (rebar.Name == rebarMat.Name)
                    {
                        dovRebarMatLibrary.Remove(rebar);
                    }
                    else if (!dovRebarMatLibrary.Contains(rebarMat))
                    {
                        dovRebarMatLibrary.Add(rebarMat);
                    }
                    else
                    {

                    }
                }
                displayRebarMat = "dovOff";
            }
        }
        protected void SelectedConcMat(ChangeEventArgs e)
        {
            foreach (var mat in dovConcMatLibrary)
            {
                if (mat.Name == e.Value.ToString())
                {
                    concMat = new DovConcMat() {Name=mat.Name,UnitWt=mat.UnitWt,Fck=mat.Fck,PoissonRatio=mat.PoissonRatio,E=mat.E,St=mat.St };
                }
            }
        }
        protected void SelectedRebarMat(ChangeEventArgs e)
        {
            foreach (var rebar in dovRebarMatLibrary)
            {
                if (rebar.Name == e.Value.ToString())
                {
                    rebarMat = new DovRebarMat() { Name=rebar.Name,UnitWt=rebar.UnitWt,Fyk=rebar.Fyk,PoissonRatio=rebar.PoissonRatio,E=rebar.E};
                }
            }
        }
        protected void DoveDisplay(string page)
        {
            switch (page)
            {
                case "footingdimension":
                    showFootingDimension = !showFootingDimension;
                    doveShowMaterial = false;
                    showAnalysisDesign = false;
                    showDetailing = false;
                    break;
                case "material":
                    doveShowMaterial = !doveShowMaterial;
                    showFootingDimension = false;
                    showAnalysisDesign = false;
                    showDetailing = false;
                    break;
                case "analysisdesign":
                    showAnalysisDesign = !showAnalysisDesign;
                    showFootingDimension = false;
                    doveShowMaterial = false;
                    showDetailing = false;
                    break;
                case "detailing":
                    showDetailing = !showDetailing;
                    showAnalysisDesign = false;
                    showFootingDimension = false;
                    doveShowMaterial = false;
                    break;
                default: break;
            }
        }
        protected void DisplayDsgnOpt()
        {
            switch (RunorPref)
            {
                case "Preference":
                    dispDsgnPref = true;
                    dispDsgnResult = false;
                    RunorPref = "Run"; break;
                case "Run":
                    dispDsgnPref = false;
                    dispDsgnResult = true;
                  IsoFootAnalysis();
                    RunorPref = "Preference";
                    break;
                default: break;
            }
        }
        protected void SetGraphicLayout(float canvaWidth, float canvaHeight, float horiStartPt, ChartStyle graphic, params float[] limits)
        {
            float offset = canvaWidth * 0.3f;
            RectangleF plot = new RectangleF(0.5f * offset + horiStartPt, 0.5f * offset, canvaWidth - offset, canvaHeight - offset);
            graphic.MarginTop = 0.6f * offset;
            graphic.MarginBottom = 0.5f * offset;
            graphic.MarginLef = 0.5f * offset + horiStartPt;
            graphic.MarginRight = 0.5f * offset;
            graphic.PlotArea = plot;
            graphic.XMin = limits[0];
            graphic.XMax = limits[1];
            graphic.YMin = limits[2];
            graphic.YMax = limits[3];
            if (limits.Length > 4)
            {
                graphic.ZMin = limits[4];
                graphic.ZMax = limits[5];
            }
        }
        protected void SetGraphicLayout(float canvaWidth, float canvaHeight, float hStartPt, float vStartPt, ChartStyle graphic, params float[] limits)
        {
            float offset = canvaWidth * 0.2f;
            RectangleF plot = new RectangleF(0.5f * offset + hStartPt, 0.5f * offset + vStartPt, canvaWidth - offset, canvaHeight - offset);
            graphic.MarginTop = 0.5f * offset + vStartPt;
            graphic.MarginBottom = 0.5f * offset;
            graphic.MarginLef = 0.5f * offset + hStartPt;
            graphic.MarginRight = 0.5f * offset;
            graphic.PlotArea = plot;
            graphic.XMin = limits[0];
            graphic.XMax = limits[1];
            graphic.YMin = limits[2];
            graphic.YMax = limits[3];
            if (limits.Length > 4)
            {
                graphic.ZMin = limits[4];
                graphic.ZMax = limits[5];
            }
        }

        protected void DesignChartType()
        {
            switch (UniorBiAxial)
            {
                case "Uniaxial Chart":
                    UniorBiAxial = "Biaxial Chart"; break;
                case "Biaxial Chart":
                    UniorBiAxial = "Uniaxial Chart"; break;
                default: break;
            }
        }
        public void DrawIsolatedFooting()
        {
             FootingGraphics.Clear(-50, -50, footingCanvWidth + 50, footingCanvHeight + 50);
            FootingGraphics.DrwColor = "dimgray";
            FootingGraphics.FillColorOpt = "gray";
            FootingGraphics.LineThickness = 2;
            footingChart.ChartArea = new RectangleF(0, 0, footingCanvWidth, footingCanvHeight);
            DimensionLine dimLine = new DimensionLine();
            dimLine.DimDir = DimensionLine.DimensionDirection.left;
            dimLine.Graphic = FootingGraphics;
            float limit = (footingData.Width > footingData.Length ? footingData.Width : footingData.Length);
            float xMax = limit * 0.5f;
            float xMin = -limit * 0.5f;
            float yMax = limit * 0.5f;
            float yMin = -limit * 0.5f;
            float zMax = 1.2f * footingData.Thickness;
            float zMin = -1.2f * footingData.Thickness;
            float[] scaleLimits = new float[] { xMin, xMax, yMin, yMax, zMin, zMax };
            //float[] scaleLimits = new float[] { -5, 5, -4, 4,-6, 6 };
            SetGraphicLayout(footingCanvWidth, footingCanvHeight, 0, footingChart, scaleLimits);
            Matrix3 m = new Matrix3();
            m = Matrix3.AzimuthElevation(footingChart.Elevation, footingChart.Azimuth);
            Point3[] pts = FootingCoordinate(footingData.Width, footingData.Length, footingData.Thickness);
            PointF[] pta = new PointF[4];

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i].Transform(m, footingChart);
            }
            int[] i0, i1;
            i0 = new int[4] { 1, 2, 7, 6 };
            i1 = new int[4] { 2, 3, 4, 7 };
            float elevation = footingChart.Elevation;
            float azimuth = footingChart.Azimuth;
            if (elevation >= 0)
            {
                if (azimuth >= -180 && azimuth < -90)
                {
                    i0 = new int[4] { 4, 5, 6, 3 };
                    i1 = new int[4] { 5, 0, 7, 6 };
                }
                else if (azimuth >= -90 && azimuth < 0)
                {
                    i0 = new int[4] { 0, 7, 2, 1 };
                    i1 = new int[4] { 5, 0, 7, 6 };
                }
                else if (azimuth >= 0 && azimuth < 90)
                {
                    i0 = new int[4] { 0, 7, 2, 1 };
                    i1 = new int[4] { 1, 4, 3, 2 };
                }
                else if (azimuth >= 90 && azimuth <= 180)
                {
                    i0 = new int[4] { 4, 5, 6, 3 };
                    i1 = new int[4] { 1, 4, 3, 2 };
                }
            }
            else if (elevation < 0)
            {
                if (azimuth >= -180 && azimuth < -90)
                {
                    i0 = new int[4] { 1, 4, 3, 2 };
                    i1 = new int[4] { 1, 0, 7, 2 };
                }
                else if (azimuth >= -90 && azimuth < 0)
                {
                    i0 = new int[4] { 4, 5, 6, 3 };
                    i1 = new int[4] { 1, 4, 3, 2 };
                }
                else if (azimuth >= 0 && azimuth < 90)
                {
                    i0 = new int[4] { 5, 0, 7, 6 };
                    i1 = new int[4] { 4, 5, 6, 3 };
                }
                else if (azimuth >= 90 && azimuth <= 180)
                {
                    i0 = new int[4] { 5, 0, 7, 6 };
                    i1 = new int[4] { 1, 0, 7, 2 };
                }
            }
            pta[0] = new PointF(pts[i0[0]].X, pts[i0[0]].Y);
            pta[1] = new PointF(pts[i0[1]].X, pts[i0[1]].Y);
            pta[2] = new PointF(pts[i0[2]].X, pts[i0[2]].Y);
            pta[3] = new PointF(pts[i0[3]].X, pts[i0[3]].Y);
            FootingGraphics.DrwColor = "dimgray";
            FootingGraphics.FillColorOpt = "gray";
             FootingGraphics.FillPolygone(pta);
            dimLine.DrawDimLineAligned(pta[0], pta[3], string.Format("W={0}", footingData.Width));
            pta[0] = new PointF(pts[i1[0]].X, pts[i1[0]].Y);
            pta[1] = new PointF(pts[i1[1]].X, pts[i1[1]].Y);
            pta[2] = new PointF(pts[i1[2]].X, pts[i1[2]].Y);
            pta[3] = new PointF(pts[i1[3]].X, pts[i1[3]].Y);
            FootingGraphics.DrwColor = "dimgray";
            FootingGraphics.FillColorOpt = "gray";
             FootingGraphics.FillPolygone(pta);
             dimLine.DrawDimLineAligned(pta[0], pta[1], string.Format("L={0}", footingData.Length));
             dimLine.DrawDimLineAligned(pta[1], pta[2], footingData.Thickness.ToString());
            pta[0] = new PointF(pts[2].X, pts[2].Y);
            pta[1] = new PointF(pts[3].X, pts[3].Y);
            pta[2] = new PointF(pts[6].X, pts[6].Y);
            pta[3] = new PointF(pts[7].X, pts[7].Y);
            FootingGraphics.DrwColor = "dimgray";
            FootingGraphics.FillColorOpt = "gray";
            FootingGraphics.FillPolygone(pta);
            //Problem Around Here including the column graphics doesnt work well
             DrawColumn();
            SetGraphicLayout(footingCanvWidth, footingCanvHeight, 0, footingChart, scaleLimits);
            Point3 pto = FootingCoordinate(footingData.Width, footingData.Length, footingData.Thickness)[0];
            float x = footingData.ColLocX - (footingData.Width * 0.5f);
            float y = footingData.ColLocY - (footingData.Length * 0.5f);
            Point3 ptd = new Point3(pto.X, pto.Y + footingData.ColLocY, 0, 1);
            Point3 ptc = new Point3(pto.X + footingData.ColLocX, pto.Y, 0, 1);
            Point3 ptn = new Point3(x, y, 0, 1);
            ptd.Transform(m, footingChart);
            ptc.Transform(m, footingChart);
            ptn.Transform(m, footingChart);

            FootingGraphics.DrwColor = "white";
            FootingGraphics.FillColorOpt = "yellow";
            FootingGraphics.TextFont = "10 arial";
            FootingGraphics.LineThickness = 0.5f;
             FootingGraphics.DrawLine(new PointF(ptd.X, ptd.Y), new PointF(ptn.X, ptn.Y));
            FootingGraphics.DrawLine(new PointF(ptc.X, ptc.Y), new PointF(ptn.X, ptn.Y));
            PointF ptt = new PointF(0.5f * (ptc.X + ptn.X), 0.5f * (ptc.Y + ptn.Y));
            float ang = CalcAngle(new PointF(ptc.X, ptc.Y), new PointF(ptn.X, ptn.Y));
            dimLine.DrawTextAtAngle(ptt, ang, string.Format("x={0}", footingData.ColLocX));
            PointF ptm = new PointF(0.5f * (ptd.X + ptn.X), 0.5f * (ptd.Y + ptn.Y));
            ang = CalcAngle(new PointF(ptd.X, ptd.Y), new PointF(ptn.X, ptn.Y));
            dimLine.DrawTextAtAngle(ptm, ang, string.Format("y={0}", footingData.ColLocY));
            FootingGraphics.DrwColor = "green";
            Symbols symb = new Symbols();
            symb.Graphic = FootingGraphics;
            //Loading Assignment
            Point3 loadPt = ColumnCoordinate(x, y, footingData.ColumnWidth, footingData.ColumnLength, footingData.Thickness)[0];
            loadPt = new Point3(loadPt.X + 0.5f * footingData.ColumnWidth, loadPt.Y + footingData.ColumnLength * 0.5f, footingData.Thickness, 1);
            Point3 axialPt = new Point3(loadPt.X, loadPt.Y, loadPt.Z + 40, 1);
            Point3 mxPt = new Point3(loadPt.X + 80, loadPt.Y, loadPt.Z, 1);
            Point3 mxmPt = new Point3(loadPt.X + 40, loadPt.Y, loadPt.Z, 1);
            Point3 myPt = new Point3(loadPt.X, loadPt.Y - 80, loadPt.Z, 1);
            Point3 mymPt = new Point3(loadPt.X, loadPt.Y - 40, loadPt.Z, 1);
            loadPt.Transform(m, footingChart);
            axialPt.Transform(m, footingChart);
            mxPt.Transform(m, footingChart);
            myPt.Transform(m, footingChart);
            mxmPt.Transform(m, footingChart);
            mymPt.Transform(m, footingChart);
             FootingGraphics.DrawLine(loadPt.X, loadPt.Y, axialPt.X, axialPt.Y);
            FootingGraphics.DrawLine(loadPt.X, loadPt.Y, mxPt.X, mxPt.Y);
             FootingGraphics.DrawLine(loadPt.X, loadPt.Y, myPt.X, myPt.Y);
            PointF ptb = new PointF(loadPt.X, loadPt.Y);
            PointF pte = new PointF(axialPt.X, axialPt.Y);
            float angle = -CalcAngle(ptb, pte);
            if (footingData.Pn < 0)
            {
                symb.DrawArrow(new PointF(axialPt.X, axialPt.Y), 5, 10, -angle);
            }
            else
            {
                symb.DrawArrow(new PointF(loadPt.X, loadPt.Y), 5, 10, angle);
            }
          
            FootingGraphics.FillColorOpt = "black";
            dimLine.DrawTextAtAngle(pte, -angle, string.Format("Pn={0}", footingData.Pn));
            ptb = new PointF(loadPt.X, loadPt.Y);
            pte = new PointF(mxPt.X, mxPt.Y);
            angle = CalcAngle(ptb, pte);
            if (footingData.Mx < 0)
            {
                symb.DrawArrow(new PointF(mxPt.X, mxPt.Y), 5f, 10, angle);
                symb.DrawArrow(new PointF(mxmPt.X, mxmPt.Y), 5f, 10, angle);
            }
            else
            {
                angle += 180;
                 symb.DrawArrow(new PointF(loadPt.X, loadPt.Y), 5f, 10, angle);
                 symb.DrawArrow(new PointF(mxmPt.X, mxmPt.Y), 5f, 10, angle);
            }
           
            dimLine.DrawTextAtAngle(pte, angle, string.Format("Mx={0}", footingData.Mx));
            ptb = new PointF(loadPt.X, loadPt.Y);
            pte = new PointF(myPt.X, myPt.Y);
            angle = -CalcAngle(ptb, pte);
            if (footingData.My < 0)
            {
                angle += 180;
                symb.DrawArrow(new PointF(myPt.X, myPt.Y), 5f, 10, angle);
                symb.DrawArrow(new PointF(mymPt.X, mymPt.Y), 5f, 10, angle);
            }
            else
            {
                 symb.DrawArrow(new PointF(loadPt.X, loadPt.Y), 5f, 10, angle);
                symb.DrawArrow(new PointF(mymPt.X, mymPt.Y), 5f, 10, angle);
            }
           
            dimLine.DrawTextAtAngle(pte, angle, string.Format("My={0}", footingData.My));
        }
        public float CalcAngle(PointF pt1, PointF pt2)
        {
            float adju = pt2.X - pt1.X;
            float opp = pt2.Y - pt1.Y;
            float length = (float)Math.Sqrt((adju * adju) + (opp * opp));
            float angle = (float)Math.Asin(opp / length) * 180 / (float)Math.PI;
            return angle;
        }
        public void DrawColumn()
        {
            FootingGraphics.DrwColor = "dimgray";
            FootingGraphics.FillColorOpt = "gray";
            FootingGraphics.LineThickness = 2;
            footingChart.ChartArea = new RectangleF(0, 0, footingCanvWidth, footingCanvHeight);
            float limit = (footingData.Width > footingData.Length ? footingData.Width : footingData.Length);
            float xMax = limit * 0.5f;
            float xMin = -limit * 0.5f;
            float yMax = limit * 0.5f;
            float yMin = -limit * 0.5f;
            float zMax = 1.2f * footingData.Thickness;
            float zMin = -1.2f * footingData.Thickness;
            float[] scaleLimits = new float[] { xMin, xMax, yMin, yMax, zMin, zMax };
            SetGraphicLayout(footingCanvWidth, footingCanvHeight, 0, footingChart, scaleLimits);
            Matrix3 m = new Matrix3();
            m = Matrix3.AzimuthElevation(footingChart.Elevation, footingChart.Azimuth);
            PointF[] pta = new PointF[4];
            int[] i0, i1;
            i0 = new int[4] { 1, 2, 7, 6 };
            i1 = new int[4] { 2, 3, 4, 7 };
            float elevation = footingChart.Elevation;
            float azimuth = footingChart.Azimuth;
            if (elevation >= 0)
            {
                if (azimuth >= -180 && azimuth < -90)
                {
                    i0 = new int[4] { 4, 5, 6, 3 };
                    i1 = new int[4] { 5, 0, 7, 6 };
                }
                else if (azimuth >= -90 && azimuth < 0)
                {
                    i0 = new int[4] { 0, 7, 2, 1 };
                    i1 = new int[4] { 5, 0, 7, 6 };
                }
                else if (azimuth >= 0 && azimuth < 90)
                {
                    i0 = new int[4] { 0, 7, 2, 1 };
                    i1 = new int[4] { 1, 4, 3, 2 };
                }
                else if (azimuth >= 90 && azimuth <= 180)
                {
                    i0 = new int[4] { 4, 5, 6, 3 };
                    i1 = new int[4] { 1, 4, 3, 2 };
                }
            }
            else if (elevation < 0)
            {
                if (azimuth >= -180 && azimuth < -90)
                {
                    i0 = new int[4] { 1, 4, 3, 2 };
                    i1 = new int[4] { 1, 0, 7, 2 };
                }
                else if (azimuth >= -90 && azimuth < 0)
                {
                    i0 = new int[4] { 4, 5, 6, 3 };
                    i1 = new int[4] { 1, 4, 3, 2 };
                }
                else if (azimuth >= 0 && azimuth < 90)
                {
                    i0 = new int[4] { 5, 0, 7, 6 };
                    i1 = new int[4] { 4, 5, 6, 3 };
                }
                else if (azimuth >= 90 && azimuth <= 180)
                {
                    i0 = new int[4] { 5, 0, 7, 6 };
                    i1 = new int[4] { 1, 0, 7, 2 };
                }
            }
            float x = footingData.ColLocX - (footingData.Width * 0.5f);
            float y = footingData.ColLocY - (footingData.Length * 0.5f);
            Point3[] ptc = ColumnCoordinate(x, y, footingData.ColumnWidth, footingData.ColumnLength, footingData.Thickness);
            SetGraphicLayout(footingCanvWidth, footingCanvHeight, 0, footingChart, scaleLimits);
            for (int i = 0; i < ptc.Length; i++)
            {
                ptc[i].Transform(m, footingChart);
            }
            pta[0] = new PointF(ptc[i0[0]].X, ptc[i0[0]].Y);
            pta[1] = new PointF(ptc[i0[1]].X, ptc[i0[1]].Y);
            pta[2] = new PointF(ptc[i0[2]].X, ptc[i0[2]].Y);
            pta[3] = new PointF(ptc[i0[3]].X, ptc[i0[3]].Y);
            FootingGraphics.FillColorOpt = "grey";
             FootingGraphics.FillPolygone(pta);
            pta[0] = new PointF(ptc[i1[0]].X, ptc[i1[0]].Y);
            pta[1] = new PointF(ptc[i1[1]].X, ptc[i1[1]].Y);
            pta[2] = new PointF(ptc[i1[2]].X, ptc[i1[2]].Y);
            pta[3] = new PointF(ptc[i1[3]].X, ptc[i1[3]].Y);
            FootingGraphics.FillColorOpt = "grey";
            FootingGraphics.FillPolygone(pta);
            pta[0] = new PointF(ptc[2].X, ptc[2].Y);
            pta[1] = new PointF(ptc[3].X, ptc[3].Y);
            pta[2] = new PointF(ptc[6].X, ptc[6].Y);
            pta[3] = new PointF(ptc[7].X, ptc[7].Y);
            FootingGraphics.FillColorOpt = "grey";
            FootingGraphics.FillPolygone(pta);
        }
        private Point3[] FootingCoordinate(float width, float length, float thickness)
        {
            Point3[] pts = new Point3[8];
            pts[0] = new Point3(-width * 0.5f, -length * 0.5f, -thickness, 1);
            pts[1] = new Point3(width * 0.5f, -length * 0.5f, -thickness, 1);
            pts[2] = new Point3(width * 0.5f, -length * 0.5f, 0, 1);
            pts[3] = new Point3(width * 0.5f, length * 0.5f, 0, 1);
            pts[4] = new Point3(width * 0.5f, length * 0.5f, -thickness, 1);
            pts[5] = new Point3(-width * 0.5f, length * 0.5f, -thickness, 1);
            pts[6] = new Point3(-width * 0.5f, length * 0.5f, 0, 1);
            pts[7] = new Point3(-width * 0.5f, -length * 0.5f, 0, 1);
            return pts;
        }
        private Point3[] ColumnCoordinate(float width, float length, float thickness)
        {
            Point3[] pts = new Point3[8];
            pts[0] = new Point3(-width * 0.5f, -length * 0.5f, 0, 1);
            pts[1] = new Point3(width * 0.5f, -length * 0.5f, 0, 1);
            pts[2] = new Point3(width * 0.5f, -length * 0.5f, thickness, 1);
            pts[3] = new Point3(width * 0.5f, length * 0.5f, thickness, 1);
            pts[4] = new Point3(width * 0.5f, length * 0.5f, 0, 1);
            pts[5] = new Point3(-width * 0.5f, length * 0.5f, 0, 1);
            pts[6] = new Point3(-width * 0.5f, length * 0.5f, thickness, 1);
            pts[7] = new Point3(-width * 0.5f, -length * 0.5f, thickness, 1);
            return pts;
        }
        private Point3[] ColumnCoordinate(float x, float y, float width, float length, float thickness)
        {
            Point3[] pts = new Point3[8];
            pts[0] = new Point3(x - width*0.5f, y-0.5f*length, 0, 1);
            pts[1] = new Point3(x + width * 0.5f, y- 0.5f * length, 0, 1);
            pts[2] = new Point3(x + width * 0.5f, y - 0.5f * length, thickness, 1);
            pts[3] = new Point3(x + width * 0.5f, y + 0.5f * length, thickness, 1);
            pts[4] = new Point3(x + width * 0.5f, y + 0.5f * length, 0, 1);
            pts[5] = new Point3(x -0.5f* width, y + 0.5f*length, 0, 1);
            pts[6] = new Point3(x - 0.5f * width, y + 0.5f * length, thickness, 1);
            pts[7] = new Point3(x - 0.5f * width, y - 0.5f * length, thickness, 1);
            return pts;
        }
        protected void CheckInputData()
        {
            if (footingData.Width > (3 * footingData.Length) || footingData.Length > (3 * footingData.Width))
            {
                continueDesign = false;
                dimError = "Error: width to length ratio is not within limit";
            }
            else
            {
                continueDesign = true;
                dimError = "";
            }
            float x = footingData.ColLocX;
            float y = footingData.ColLocY;
            float cw =0.5f*footingData.ColumnWidth;
            float cl = 0.5f*footingData.ColumnLength;
            if (x < cw || y < cl || x > footingData.Width-cw || y > footingData.Length - cl)
            {
                locationError = "Error: location of column is not within the footing";
                continueDesign = false;
            }
            else
            {
                locationError = "";
            }
            if (continueDesign)
            {
                 DrawIsolatedFooting();
            }
        }
        protected void DrawIsoFoot()
        {
            DimensionLine dim = new DimensionLine();
            DiagramGraphics = new DovDrawings(diagramContext);
            diagramChart = new ChartStyle();
            float footW = 0.4f * DiagramCanvWidth;
            float footH = 0.4f * DiagramCanvHeight;
            diagramChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0;
            float startY = 0;
            DiagramGraphics.Clear(startX, startY, footW, footH);
            float limit = (footingData.Width > footingData.Length ? footingData.Width : footingData.Length);
            float xMax = 0.5f * limit;
            float xMin = -0.5f * limit;
            float yMax = 0.5f * limit;
            float yMin = -0.5f * limit;
            SetGraphicLayout(footW, footH, startX, startY, diagramChart, new float[] { xMin, xMax, yMin, yMax });
            PointF fPt1 = new PointF(-0.5f * footingData.Width, 0.5f * footingData.Length);
            PointF fPt2 = new PointF(0.5f * footingData.Width, -0.5f * footingData.Length);
            fPt1 = diagramChart.Point2D(fPt1);
            fPt2 = diagramChart.Point2D(fPt2);
            DiagramGraphics.FillColorOpt = "lightslategrey";
             DiagramGraphics.FillColor(fPt1, fPt2);
            DiagramGraphics.DrwColor = "dimgrey";
             DiagramGraphics.DrawRec(fPt1, fPt2);
            SetGraphicLayout(footW, footH, startX, startY, diagramChart, new float[] { xMin, xMax, yMin, yMax });
            PointF cPt1 = new PointF(footingData.ColLocX - 0.5f * footingData.Width + footingData.ColumnWidth * 0.5f, footingData.ColLocY - footingData.Length * 0.5f- 0.5f * footingData.ColumnLength);
            PointF cPt2 = new PointF(footingData.ColLocX - 0.5f * footingData.Width - footingData.ColumnWidth*0.5f, footingData.ColLocY - footingData.Length * 0.5f +0.5f* footingData.ColumnLength);
            PointF center = new PointF(footingData.ColLocX - 0.5f * footingData.Width, footingData.ColLocY- footingData.Length * 0.5f);
            cPt1 = diagramChart.Point2D(cPt1);
            cPt2 = diagramChart.Point2D(cPt2);
            center= diagramChart.Point2D(center);
            DiagramGraphics.FillColorOpt = "darkslategrey";
             DiagramGraphics.FillColor(cPt1, cPt2);
            DiagramGraphics.DrwColor = "dimgrey";
            DiagramGraphics.DrawRec(cPt1, cPt2);
            DiagramGraphics.DrwColor = "cadetblue";
            dim.Graphic = DiagramGraphics;
            dim.DimLoc = DimensionLine.DimensionLocation.end;
             dim.DrawAdjDimLine(fPt1, fPt2, string.Format("W={0}", footingData.Width));
            dim.DimDir = DimensionLine.DimensionDirection.right;
            DiagramGraphics.TextBase = TextBaseline.Bottom;
             dim.DrawOppoDimLine(fPt1, fPt2, string.Format("L={0}", footingData.Length));
            dim.DimDir = DimensionLine.DimensionDirection.right;
             dim.DrawOppoDimLine(fPt1, center, string.Format("y={0}", footingData.ColLocY));
            dim.DimLoc = DimensionLine.DimensionLocation.start;
             dim.DrawAdjDimLine(fPt1, center, string.Format("x={0}", footingData.ColLocX));
        }
        protected void IsoFootAnalysis()
        {
            fDesign = new IsoFootingDesign(footingData,concMat,rebarMat);
            fDesign.CheckBearingStress();
            DiagramGraphics = new DovDrawings(diagramContext);
            diagramChart = new ChartStyle();
            float footW = 0.4f * DiagramCanvWidth;
            float footH = 0.2f * DiagramCanvHeight;
            diagramChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0;
            float startY = 0.4f * DiagramCanvHeight;
           
           
             DiagramGraphics.Clear(startX, startY, footW, footH);
            SetGraphicLayout(footW, footH, startX, startY, diagramChart, new float[] { 0, 0, 0, 0 });
            DiagramGraphics.DrwColor = "black";
            //await DiagramGraphics.FillColorAsync(startX, startY, footW, footH,"black");
           fDesign.DrawSfx(DiagramGraphics, diagramChart);
            footW = 0.2f * DiagramCanvWidth;
            footH = 0.4f * DiagramCanvHeight;
            startX = 0.4f * DiagramCanvWidth;
            startY = 0;
            DiagramGraphics.Clear(startX, startY, footW, footH);
            SetGraphicLayout(footW, footH, startX, startY, diagramChart, new float[] { 0, 0, 0, 0 });
             fDesign.DrawSfy(DiagramGraphics, diagramChart);
            footW = 0.4f * DiagramCanvWidth;
            footH = 0.15f * DiagramCanvHeight;
            startX = 0;
            startY = 0.6f* DiagramCanvHeight;
             DiagramGraphics.Clear(startX, startY, footW, footH);
            SetGraphicLayout(footW, footH, startX, startY, diagramChart, new float[] { 0, 0, 0, 0 });
             fDesign.DrawBmx(DiagramGraphics, diagramChart);
            footW = 0.15f * DiagramCanvWidth;
            footH = 0.4f * DiagramCanvHeight;
            startX = 0.6f * DiagramCanvWidth;
            startY = 0;
             DiagramGraphics.Clear(startX, startY, footW, footH);
            SetGraphicLayout(footW, footH, startX, startY, diagramChart, new float[] { 0, 0, 0, 0 });
            fDesign.DrawBmy(DiagramGraphics, diagramChart);
            fDesign.DsgnPref = DsgnPref;
            if(fDesign.CheckPunching()&& fDesign.CheckWideBeamShear())
            {
                continueDetailing = true;
            }
            else
            {
                continueDetailing = false;
            }
            
        }
        protected void IsoFootingDetailing()
        {
            if (continueDetailing)
            {
                IsoFootDetailOnPlan();
                IsoFootDetailOnElev();
            }
        }
        protected void IsoFootDetailOnPlan()
        {
            DimensionLine dim = new DimensionLine();
            DetailingGraphics = new DovDrawings(detailingContext);
            detailingChart = new ChartStyle();
            float footW = DetailingCanvWidth;
            float footH = 0.6f * DetailingCanvHeight;
            detailingChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0;
            float startY = 0;
            DetailingGraphics.Clear(startX, startY, footW, footH);
            float limit = (footingData.Width > footingData.Length ? footingData.Width : footingData.Length);
            float xMax = 0.5f * limit;
            float xMin = -0.5f * limit;
            float yMax = 0.5f * limit;
            float yMin = -0.5f * limit;

            SetGraphicLayout(footW, footH, startX, startY, detailingChart, new float[] { xMin, xMax, yMin, yMax });
            PointF fPt1 = new PointF(-0.5f*footingData.Width, 0.5f*footingData.Length);
            PointF fPt2 = new PointF(0.5f*footingData.Width, -0.5f*footingData.Length);
            fPt1 = detailingChart.Point2D(fPt1);
            fPt2 = detailingChart.Point2D(fPt2);
            DetailingGraphics.FillColorOpt = "lightslategrey";
             DetailingGraphics.FillColor(fPt1, fPt2);
            DetailingGraphics.DrwColor = "dimgrey";
             DetailingGraphics.DrawRec(fPt1, fPt2);
            SetGraphicLayout(footW, footH, startX, startY, detailingChart, new float[] { xMin, xMax, yMin, yMax });
            PointF cPt1 = new PointF(footingData.ColLocX - 0.5f * footingData.Width + footingData.ColumnWidth * 0.5f, footingData.ColLocY - footingData.Length * 0.5f - 0.5f * footingData.ColumnLength);
            PointF cPt2 = new PointF(footingData.ColLocX - 0.5f * footingData.Width - footingData.ColumnWidth * 0.5f, footingData.ColLocY - footingData.Length * 0.5f + 0.5f * footingData.ColumnLength);
            PointF center = new PointF(footingData.ColLocX - 0.5f * footingData.Width, footingData.ColLocY - footingData.Length * 0.5f);
            cPt1 = detailingChart.Point2D(cPt1);
            cPt2 = detailingChart.Point2D(cPt2);
            center = detailingChart.Point2D(center);
          DetailingGraphics.FillColorOpt = "darkslategrey";
            DetailingGraphics.FillColor(cPt1, cPt2);
            DetailingGraphics.DrwColor = "dimgrey";
             DetailingGraphics.DrawRec(cPt1, cPt2);
            DetailingGraphics.DrwColor = "cadetblue";
            dim.Graphic = DetailingGraphics;
            dim.DimLoc = DimensionLine.DimensionLocation.end;
             dim.DrawAdjDimLine(fPt1, fPt2, string.Format("W={0}", footingData.Width));
            dim.DimDir = DimensionLine.DimensionDirection.right;
            DetailingGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawOppoDimLine(fPt1, fPt2, string.Format("L={0}", footingData.Length));
            fPt1 = new PointF(-0.5f * footingData.Width, -0.5f * footingData.Length);
            fPt1 = detailingChart.Point2D(fPt1);
             dim.DrawOppoDimLine(fPt1, center, string.Format("y={0}", footingData.ColLocY));
            ////dim.DimLoc = DimensionLine.DimensionLocation.end;
            //await dim.DrawAdjDimLine(fPt1, center, string.Format("x={0}", footingData.ColLocX));
            // drawing reinforcement
            // horizontal
            float thick = footingData.Thickness;
            float w = footingData.Width;
            float l = footingData.Length;
            float c = footingData.Cover*0.1f;
            float dia = footingData.Dia;
            float barArea = (float)Math.PI * dia * dia / 4;
            int nbar =(int)Math.Ceiling(fDesign.Asy/barArea);
            int barSpacing = 1000/nbar;
            barSpacing= barSpacing.RoundDown();
            float x = footingData.ColLocX - 0.5f * w + 0.5f*footingData.ColumnWidth +thick-2*c;
            float y1 = -(0.5f * l - c);
            float y2 = (0.5f * l -c);
            PointF pt1 = new PointF(x, y1);
            PointF pt2 = new PointF(x, y2);
            PointF pt3 = new PointF(pt1.X - thick + 2 * c, pt1.Y);
            PointF pt4 = new PointF(pt2.X - thick + 2 * c, pt2.Y);
            pt1 = detailingChart.Point2D(pt1);
            pt2 = detailingChart.Point2D(pt2);
            pt3 = detailingChart.Point2D(pt3);
            pt4 = detailingChart.Point2D(pt4);
            DetailingGraphics.DrwColor = "red";
            DetailingGraphics.DrawLine(pt1, pt2);
            string spacing = string.Format("{0} c/c {1} mm", dia, barSpacing);
            DetailingGraphics.TextAlignment = TextAlign.Center;
            DetailingGraphics.TextBase = TextBaseline.Hanging;
            DetailingGraphics.FillColorOpt = "yellow";
           dim.DrawTextAtAngleMid(pt1, pt2, spacing);
            DetailingGraphics.DrawLine(pt1, pt3);
             DetailingGraphics.DrawLine(pt2, pt4);
            //vertical 
            float y3 = footingData.ColLocY - 0.5f * l + 0.5f * footingData.ColumnLength + thick - 2 * c;
            float x1 = -(0.5f * w - c);
            float x2 = (0.5f * w - c);
             nbar = (int)Math.Ceiling(fDesign.Asx / barArea);
             barSpacing = 1000 / nbar;
            barSpacing = barSpacing.RoundDown();
            pt1 = new PointF(x1, y3);
            pt2 = new PointF(x2, y3);
            pt3 = new PointF(pt1.X, pt1.Y - thick + 2 * c);
            pt4 = new PointF(pt2.X, pt2.Y - thick + 2 * c);
            pt1 = detailingChart.Point2D(pt1);
            pt2 = detailingChart.Point2D(pt2);
            pt3 = detailingChart.Point2D(pt3);
            pt4 = detailingChart.Point2D(pt4);
             DetailingGraphics.DrawLine(pt1, pt2);
             spacing = string.Format("{0} c/c {1} mm", dia, barSpacing);
            DetailingGraphics.TextAlignment = TextAlign.Center;
            DetailingGraphics.TextBase = TextBaseline.Bottom;
             dim.DrawTextAtAngleMid(pt1, pt2, spacing);
             DetailingGraphics.DrawLine(pt1, pt3);
            DetailingGraphics.DrawLine(pt2, pt4);
        }
        protected void IsoFootDetailOnElev()
        {
            DimensionLine dim = new DimensionLine();
            DetailingGraphics = new DovDrawings(detailingContext);
            detailingChart = new ChartStyle();
            dim.Graphic = DetailingGraphics;
            float footW = DetailingCanvWidth;
            float footH = 0.4f * DetailingCanvHeight;
            detailingChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0;
            float startY = 0.6f* DetailingCanvHeight;
             DetailingGraphics.Clear(startX, startY, footW, footH);
            float limit = footingData.Width ;
            float xMax = 0.5f * limit;
            float xMin = -0.5f * limit;
            float yMax = 0.5f* footingData.Thickness;
            float yMin = -2*footingData.Thickness;

            SetGraphicLayout(footW, footH, startX, startY, detailingChart, new float[] { xMin, xMax, yMin, yMax });

            float thick = footingData.Thickness;
            float w = footingData.Width;
            float l = footingData.Length;
            float c = footingData.Cover * 0.1f;
            float dia = footingData.Dia;
            float locX = footingData.ColLocX;
            float locY = footingData.ColLocY;
            float cw = footingData.ColumnWidth;
            float cl = footingData.ColumnLength;
            PointF fPt1 = new PointF(-0.5f * footingData.Width, 0f);
            PointF fPt2 = new PointF(0.5f * footingData.Width, - footingData.Thickness);
            fPt1 = detailingChart.Point2D(fPt1);
            fPt2 = detailingChart.Point2D(fPt2);
            DetailingGraphics.FillColorOpt = "lightslategrey";
            DetailingGraphics.FillColor(fPt1, fPt2);
            DetailingGraphics.DrwColor = "dimgrey";
            //await DetailingGraphics.DrawRecAsync(fPt1, fPt2);
            PointF pt1 = new PointF(-0.5f * w, 0f);
            PointF pt2 = new PointF(locX-0.5f*w-0.5f*cw, 0);
            PointF pt3 = new PointF(locX-0.5f*w-0.5f*cw, 0.5f * thick);
            PointF pt4 = new PointF(locX-0.5f*w +0.5f* cw, 0.5f * thick);
            PointF pt5 = new PointF(locX-0.5f*w +0.5f* cw, 0);
            PointF pt6 = new PointF(0.5f * w, 0);
            PointF pt7 = new PointF(0.5f * w, -thick);
            PointF pt8 = new PointF(-0.5f * w, -thick);
            PointF[] pts = new PointF[] { pt1, pt2, pt3, pt4, pt5, pt6, pt7, pt8 };
            for(int i = 0; i < pts.Length; i++)
            {
                pts[i] = detailingChart.Point2D(pts[i]);
            }
            DetailingGraphics.FillPolygone(pts);
             dim.DrawDimLineAligned(pts[0], pts[7], thick.ToString());
             dim.DrawDimLineAligned(pts[6], pts[7], w.ToString());
            dim.DimDir = DimensionLine.DimensionDirection.right;
            DetailingGraphics.TextBase = TextBaseline.Bottom;
             dim.DrawDimLineAligned(pts[0], pts[1], (footingData.ColLocX-0.5f*cw).ToString());
            DetailingGraphics.DrwColor = "red";
            pt1 = new PointF(-0.5f * w+c, -thick+c);
            pt2 = new PointF(0.5f * w-c, -thick+c);
            pt3 = new PointF(pt1.X, pt1.Y + thick - 2*c);
            pt4 = new PointF(pt2.X, pt2.Y + thick - 2*c);
            pt1 = detailingChart.Point2D(pt1);
            pt2 = detailingChart.Point2D(pt2);
             DetailingGraphics.DrawLine(pt1, pt2);
            pt3 = detailingChart.Point2D(pt3);
            pt4 = detailingChart.Point2D(pt4);
             DetailingGraphics.DrawLine(pt1, pt3);
            DetailingGraphics.DrawLine(pt2, pt4);
            //Drawing column bars
            pt5 = new PointF(locX - 0.5f * w - 0.5f * cw+c, -thick+c+3*dia*0.1f);
            pt6= new PointF(locX - 0.5f * w - 0.5f * cw+c, 0.5f * thick);
            pt7= new PointF(locX - 0.5f * w + 0.5f * cw-c, -thick + c + 3*dia * 0.1f);
            pt8 = new PointF(locX - 0.5f * w + 0.5f * cw-c, 0.5f * thick);
            //47 factor for development length
            PointF pt9 = new PointF(pt5.X-47*dia*0.1f, pt5.Y);
            PointF pt10 = new PointF(pt7.X + 47 * dia * 0.1f, pt7.Y);
            pt9 = detailingChart.Point2D(pt9);
            pt10 = detailingChart.Point2D(pt10);
            pt5 = detailingChart.Point2D(pt5);
            pt6 = detailingChart.Point2D(pt6);
            pt7 = detailingChart.Point2D(pt7);
            pt8 = detailingChart.Point2D(pt8);
             DetailingGraphics.DrawLine(pt5, pt6);
             DetailingGraphics.DrawLine(pt7, pt8);
            DetailingGraphics.DrawLine(pt5, pt9);
            DetailingGraphics.DrawLine(pt7, pt10);
            pt1=detailingChart.Point2D( new PointF(-0.5f * w+c, -2*thick));
            pt2 =detailingChart.Point2D( new PointF(0.5f * w-c, -2*thick));
            pt3= detailingChart.Point2D(new PointF(-0.5f * w+c, -thick-2*c));
            pt4 = detailingChart.Point2D(new PointF(0.5f * w-c, -thick - 2 * c));
            DetailingGraphics.DrawLine(pt1, pt2);
            DetailingGraphics.DrwColor = "dimgrey";
            dim.DimDir = DimensionLine.DimensionDirection.left;
            DetailingGraphics.TextBase = TextBaseline.Hanging;
            dim.DrawDimLineAligned(pt1, pt2, (w - 2 * c).ToString());
            DetailingGraphics.DrwColor = "red";
             DetailingGraphics.DrawLine(pt1, pt3);
            dim.DimDir = DimensionLine.DimensionDirection.right;
            DetailingGraphics.DrwColor = "dimgrey";
            DetailingGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawDimLineAligned(pt1, pt3, (thick - 2 * c).ToString());
            DetailingGraphics.DrwColor = "red";
            DetailingGraphics.DrawLine(pt2, pt4);
            float barArea = (float)Math.PI * dia * dia / 4;
            int nbar = (int)Math.Ceiling(fDesign.Asy / barArea);
            int barSpacing = 1000 / nbar;
            barSpacing = barSpacing.RoundDown()/10;
            for(int i = (int)(2*c); i < (w - 2 * c); i += barSpacing)
            {
                float start = -0.5f * (w - 2 * c);
                float y = -(thick - c - 1.5f * dia*0.1f);
                float x = start + i;
                PointF pt = new PointF(x, y);
                pt = detailingChart.Point2D(pt);
                 DetailingGraphics.DrawArc(pt.X, pt.Y, 1.5f * dia * 0.1f, 0, 2 * Math.PI);
            }
        }
    }
}
   


