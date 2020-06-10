using EngDolphin.Canvas;
using EngDolphin.Client.Extensions;
using EngDolphin.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static EngDolphin.Canvas.CanvasEnum;

namespace EngDolphin.Pages
{
    public class CombinedFootingComp : ComponentBase
    {
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        UnitConv u = new UnitConv();
        protected ElementReference FootingCanvas { get; set; }
        protected ElementReference DiagramCanvas { get; set; }
        protected ElementReference DetailCanvas { get; set; }
        protected bool doveShowMaterial = false;
        protected bool showFootingDimension = false;
        protected bool showAnalysisDesign = false;
        protected bool showDetailing = false;
        protected string displayConcMat = "dovOff";
        protected string displayRebarMat = "dovOff";
        protected bool dispDsgnResult = false;
        protected bool continueDesign = false;
        protected bool continueDetailing = false;
        protected bool dispDsgnPref = false;
        protected bool dispDsgnRslt = false;
        protected bool dispChartTable = false;
        protected int LoadInputNo = 0;
        protected string RunorPref = "Preference";
        protected string UniorBiAxial = "Uniaxial Chart";
        protected string LoadName = "First Column Load";
        
        protected float [] dt = new float[3];

        // Drawings 
        protected string footingBackColor { get; set; } = "white";
        protected string DsgnBackColor { get; set; } = "white";
        protected string DetailBackColor { get; set; } = "white";
        protected string warning { get; set; } = "red";
        public long footingCanvWidth { get; set; } = 500;
        public long footingCanvHeight { get; set; } = 250;
        public long DiagramCanvWidth { get; set; } = 500;
        public long DiagramCanvHeight { get; set; } = 750;
        public long DetailWidth { get; set; } = 600;
        public long DetailHeight { get; set; } = 800;
        protected string dimError = "";
        protected string locationError = "";
        public float X { get; set; }
        public float Y { get; set; }
        protected List<DovConcMat> dovConcMatLibrary = new List<DovConcMat>();
        protected List<DovRebarMat> dovRebarMatLibrary = new List<DovRebarMat>();
        protected DovConcMat concMat = new DovConcMat();
        protected DovRebarMat rebarMat = new DovRebarMat();
        protected CombFootingData fData = new CombFootingData();
        protected CombFootingDesign fDesign = new CombFootingDesign();
        protected DovDsgnPreference DsgnPref = new DovDsgnPreference();
        public DovDrawings FGraphics { get; set; }
        public DovDrawings dGraphics { get; set; }
        public DovDrawings detGraphics { get; set; }
        private CanvasContext dContext;
        private CanvasContext fContext;
        private CanvasContext detContext;
        private ChartStyle fChart;
        private ChartStyle dChart;
        private ChartStyle detChart;
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

            this.dContext = new CanvasContext(DiagramCanvas, JsRuntime);
            dGraphics = new DovDrawings(dContext);
            //
            this.fContext = new CanvasContext(FootingCanvas, JsRuntime);
            FGraphics = new DovDrawings(fContext);
            this.detContext = new CanvasContext(DetailCanvas, JsRuntime);
            detGraphics = new DovDrawings(detContext);
            fChart = new ChartStyle();
            dChart = new ChartStyle();
            detChart = new ChartStyle();
        }
        protected void ShowConcMatProp(ChangeEventArgs e)
        {
            foreach (var mat in dovConcMatLibrary)
            {
                if (mat.Name == e.Value.ToString())
                {
                    concMat = new DovConcMat(mat.Name, mat.UnitWt, mat.Fck, mat.PoissonRatio, mat.E, mat.St);
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
                    rebarMat = new DovRebarMat(rebar.Name, rebar.UnitWt, rebar.Fyk, rebar.PoissonRatio, rebar.E);
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
                    concMat = new DovConcMat(mat.Name, mat.UnitWt, mat.Fck, mat.PoissonRatio, mat.E, mat.St);
                }
            }
        }

        protected void SelectedRebarMat(ChangeEventArgs e)
        {
            foreach (var rebar in dovRebarMatLibrary)
            {
                if (rebar.Name == e.Value.ToString())
                {
                    rebarMat = new DovRebarMat(rebar.Name, rebar.UnitWt, rebar.Fyk, rebar.PoissonRatio, rebar.E);
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
                    //await DrawCombFootPlanAsync();
                    CombFootAnalysisDesign();
                     DrawDiagram();
                    RunorPref = "Preference";
                    break;
                default: break;
            }
        }

        protected void UpdateLoadInput()
        {
            switch (LoadName)
            {
                case "First Column Load":
                    LoadInputNo = 0;
                    LoadName = "Second Column Load"; break;
                case "Second Column Load":
                    LoadName = "First Column Load";
                    LoadInputNo = 1;
                    break;
                default: break;
            }
        }





        protected void SetGraphicLayout(float canvaWidth, float canvaHeight, float horiStartPt, ChartStyle graphic, params float[] limits)
        {
            float offset = (canvaWidth>canvaHeight?canvaHeight:canvaWidth) * 0.3f;
            RectangleF plot = new RectangleF(0.5f * offset + horiStartPt, 0.5f * offset, canvaWidth - offset, canvaHeight - offset);
            graphic.MarginTop = 0.5f * offset;
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
        protected  void DrawCombFoot()
        {
            DimensionLine dim = new DimensionLine();
            FGraphics = new DovDrawings(fContext);
            fChart = new ChartStyle();
            fChart.ChartArea = new RectangleF(0, 0, footingCanvWidth, footingCanvHeight);
            float startX = 0;
            float startY = 0;
            FGraphics.Clear(startX, startY, footingCanvWidth, footingCanvHeight);
            if (fData.X1 < (0.5f * fData.Cw1) || fData.X3 < (0.5f * fData.Cw2))
            {
                locationError = "Column is outside of footing";
                //return false;
            }
            else
            {
                locationError = "";
            }
           
           
            float ylimit = (fData.W1 > fData.W2 ? fData.W1 : fData.W2);
          
            float xMax = fData.X1+fData.X2+fData.X3;
            float xMin = 0;
            float yMax =0.5f* ylimit;
            float yMin =-0.5f*ylimit;
            float offset = 0.3f*(footingCanvHeight> footingCanvWidth ? footingCanvWidth : footingCanvHeight);
            float marginWidth = footingCanvWidth - offset;
            float marginHeight = footingCanvHeight - offset;
            float hratio =marginWidth/ (xMax-xMin) ;
            float vratio =  (yMax-yMin)/marginHeight;
           
            float cw1 =fData.Cw1;
            float cw2 = fData.Cw2;
            float cl1 = vratio * hratio * fData.Cl1;
            float cl2 = vratio * hratio * fData.Cl2;
            SetGraphicLayout(footingCanvWidth, footingCanvHeight, startX, fChart, new float[] { xMin, xMax, yMin, yMax });
            PointF fPt1 = new PointF(0, -0.5f*fData.W1);
            PointF fPt2 = new PointF(0, 0.5f*fData.W1);
            PointF fPt3 = new PointF(xMax, 0.5f*fData.W2);
            PointF fPt4 = new PointF(xMax, -0.5f*fData.W2);
            fPt1 = fChart.Point2D(fPt1);
            fPt2 = fChart.Point2D(fPt2);
            fPt3 = fChart.Point2D(fPt3);
            fPt4 = fChart.Point2D(fPt4);
          
            FGraphics.FillColorOpt = "lightslategrey";
            FGraphics.DrwColor = "dimgrey";
             FGraphics.FillPolygone(new PointF[] { fPt1, fPt2, fPt3, fPt4 });
            SetGraphicLayout(footingCanvWidth, footingCanvHeight, startX, fChart, new float[] { xMin, xMax, yMin, yMax });
            PointF cPt1 = new PointF(fData.X1 - 0.5f * cw1, -0.5f *cl1);
            PointF cPt2 = new PointF(fData.X1 + 0.5f * cw1, 0.5f * cl1);
            cPt1 = fChart.Point2D(cPt1);
            cPt2 = fChart.Point2D(cPt2);
            FGraphics.FillColorOpt = "darkslategrey";
            FGraphics.FillColor(cPt1, cPt2);
            FGraphics.DrwColor = "dimgrey";
             FGraphics.DrawRec(cPt1, cPt2);
             cPt1 = new PointF(fData.X1+fData.X2 - 0.5f * cw2, -0.5f * cl2);
             cPt2 = new PointF(fData.X1+fData.X2 + 0.5f * cw2, 0.5f * cl2);
            cPt1 = fChart.Point2D(cPt1);
            cPt2 = fChart.Point2D(cPt2);
            FGraphics.FillColorOpt = "darkslategrey";
             FGraphics.FillColor(cPt1, cPt2);
            FGraphics.DrwColor = "dimgrey";
            FGraphics.DrawRec(cPt1, cPt2);
           FGraphics.DrwColor = "cadetblue";
            dim.Graphic = FGraphics;
            dim.DimLoc = DimensionLine.DimensionLocation.end;
            dim.DimDir = DimensionLine.DimensionDirection.right;
            FGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawDimLineAligned(fPt1, fPt2, string.Format("W1={0}", fData.W1));
             dim.DrawDimLineAligned(fPt3, fPt4, string.Format("W2={0}", fData.W2));
            dim.DimDir = DimensionLine.DimensionDirection.left;
            FGraphics.TextBase = TextBaseline.Hanging;
            float lower = (fData.W1>fData.W2? -0.5f* fData.W1:-0.5f*fData.W2);
            PointF cPt0 = new PointF(0, lower);
            cPt0 = fChart.Point2D(cPt0);
            cPt1 = new PointF(fData.X1, lower);
            cPt1 = fChart.Point2D(cPt1);
            cPt2 = new PointF(fData.X1 + fData.X2, lower);
            cPt2 = fChart.Point2D(cPt2);
            PointF cPt3 = new PointF(fData.X1 + fData.X2 + fData.X3, lower);
            cPt3 = fChart.Point2D(cPt3);
            dim.DrawDimLineAligned(cPt0, cPt1, string.Format("X1={0}", fData.X1));
             dim.DrawDimLineAligned(cPt1, cPt2, string.Format("X2={0}", fData.X2));
            dim.DrawDimLineAligned(cPt2, cPt3, string.Format("X3={0}", fData.X3));
            Symbols symb = new Symbols();
            symb.Graphic = FGraphics;
            float arLength = 60;
            float aw = 5f;
            float ah = 10f;
            float off = 40f;
            float angle = 0;
            PointF cpt1 = new PointF(fData.X1, 0);
        
            PointF mpt1x = new PointF(cpt1.X + arLength, 0);
            PointF mpt1mx = new PointF(cpt1.X + 0.5f * arLength, 0);
            PointF mpt1y = new PointF(cpt1.X, -arLength);
            PointF mpt1my = new PointF(cpt1.X, -0.5f * arLength);
            cpt1 = fChart.Point2D(cpt1);
            mpt1x = fChart.Point2D(mpt1x);
            mpt1mx = fChart.Point2D(mpt1mx);
            FGraphics.DrwColor = "red";
             FGraphics.DrawLine(cpt1, mpt1x);
            angle = CalcAngle(cpt1, mpt1x);
            if (fData.Loads[0].Mx > 0)
            {
                symb.DrawArrow(mpt1x, aw, ah, angle);
                symb.DrawArrow(mpt1mx, aw, ah, angle);
            }
            else
            {
                angle += 180;
                 symb.DrawArrow(cpt1, aw,ah, angle);
                symb.DrawArrow(mpt1mx, aw, ah, angle);
            }
            FGraphics.FillColorOpt = "white";
             dim.DrawTextAtAngle(mpt1x, angle, string.Format("Mx={0}", fData.Loads[0].Mx));
            mpt1y = fChart.Point2D(mpt1y);
            mpt1my = fChart.Point2D(mpt1my);
            FGraphics.DrwColor = "red";
            FGraphics.DrawLine(cpt1, mpt1y);
            angle = -CalcAngle(cpt1, mpt1y);
            if (fData.Loads[0].My > 0)
            {
                 symb.DrawArrow(cpt1, aw, ah, angle);
                symb.DrawArrow(mpt1my, aw,ah, angle);
            }
            else
            {
                angle = -angle;
               symb.DrawArrow(mpt1y, aw, ah, angle);
               symb.DrawArrow(mpt1my, aw, ah, angle);
            }
         
            FGraphics.FillColorOpt = "white";
            dim.DrawTextAtAngle(mpt1y, angle, string.Format("My={0}", fData.Loads[0].My));
            cpt1 = new PointF(fData.X1, off);
            cpt1 = fChart.Point2D(cpt1);
            FGraphics.FillColorOpt = "white";
           dim.DrawTextAtAngle(cpt1, 0, string.Format("Pn={0}", fData.Loads[0].Pn));
            PointF cpt2 = new PointF(fData.X1 + fData.X2, 0);
            mpt1x = new PointF(cpt2.X - arLength, 0);
            mpt1mx = new PointF(cpt2.X - 0.5f * arLength, 0);
            mpt1y = new PointF(cpt2.X, -arLength);
            mpt1my = new PointF(cpt2.X, -0.5f * arLength);
            cpt2 = fChart.Point2D(cpt2);
            mpt1x = fChart.Point2D(mpt1x);
            mpt1mx = fChart.Point2D(mpt1mx);
            FGraphics.DrwColor = "red";
            FGraphics.DrawLine(cpt2, mpt1x);
            angle = CalcAngle(cpt2, mpt1x);
            if (fData.Loads[1].Mx > 0)
            {
                 symb.DrawArrow(cpt2, aw, ah, angle);
                symb.DrawArrow(mpt1mx, aw, ah, angle);
            }
            else
            {
                angle+=180;
                symb.DrawArrow(mpt1x, aw, ah, angle);
                symb.DrawArrow(mpt1mx, aw, ah, angle);
            }
          
            FGraphics.FillColorOpt = "white";
             dim.DrawTextAtAngle(mpt1x, angle, string.Format("Mx={0}", fData.Loads[1].Mx));
            mpt1y = fChart.Point2D(mpt1y);
            mpt1my = fChart.Point2D(mpt1my);
            FGraphics.DrwColor = "red";
             FGraphics.DrawLine(cpt2, mpt1y);
            angle = -CalcAngle(cpt2, mpt1y);
            if (fData.Loads[1].My > 0)
            {
                symb.DrawArrow(cpt2, aw, ah, angle);
                symb.DrawArrow(mpt1my, aw, ah, angle);
            }
            else
            {
                angle += 180;
                symb.DrawArrow(mpt1y, aw, ah, angle);
                symb.DrawArrow(mpt1my, aw, ah, angle);
            }
            FGraphics.FillColorOpt = "white";
            dim.DrawTextAtAngle(mpt1y, angle, string.Format("My={0}", fData.Loads[1].My));
            cpt2 = new PointF(fData.X1 + fData.X2, off);
            cpt2 = fChart.Point2D(cpt2);
             dim.DrawTextAtAngle(cpt2, 0, string.Format("Pn={0}", fData.Loads[1].Pn));
            //return true;
        }
        public float CalcAngle(PointF pt1, PointF pt2)
        {
            float adju = pt2.X - pt1.X;
            float opp = pt2.Y - pt1.Y;
            float length = (float)Math.Sqrt((adju * adju) + (opp * opp));
            float angle = (float)Math.Asin(opp / length) * 180 / (float)Math.PI;
            return angle;
        }
        protected void CombFootAnalysisDesign()
        {
            fDesign.ConcMat = concMat;
            fDesign.RebarMat = rebarMat;
            fDesign.DsgnPref = DsgnPref;
            fDesign.fData = fData;
            fDesign.CheckBearingStress();
            fDesign.Analysis();
            
           
            if (fDesign.CheckPunching() && fDesign.CheckWideBeamShear())
            {
                continueDetailing = true;
            }
            else
            {
                continueDetailing = false;
            }
        }
        protected void DrawDiagram()
        {
            DimensionLine dim = new DimensionLine();
            dGraphics = new DovDrawings(dContext);
            dChart = new ChartStyle();
            float footW = DiagramCanvWidth;
            float footH = 0.3333f * DiagramCanvHeight;
            dChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0f;
            float startY = footH;
            dGraphics.Clear(startX, startY, footW, footH);
            dGraphics.DrwColor = "black";
            SetGraphicLayout(footW, footH, startX,startY, dChart, new float[] { 0, 10, 0, 10 });
           fDesign.DrawShearForceDiagram(dGraphics, dChart);
             footW = DiagramCanvWidth;
            footH = 0.3333f * DiagramCanvHeight;
            dChart.ChartArea = new RectangleF(0, 0, footW, footH);
             startX = 0f;
            startY =2* footH;
             dGraphics.Clear(startX, startY, footW, footH);
            dGraphics.DrwColor = "black";
            SetGraphicLayout(footW, footH, startX,startY, dChart, new float[] { 0, 10, 0, 10 });
            fDesign.DrawBendingMomentDiagram(dGraphics, dChart);
            
        }
        protected void DrawCombFootPlan()
        {
            DimensionLine dim = new DimensionLine();
            dGraphics = new DovDrawings(dContext);
            dChart = new ChartStyle();
            float footW = DiagramCanvWidth;
            float footH = 0.3333f * DiagramCanvHeight;
            dChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0f;
            float startY = 0f;
             dGraphics.Clear(startX, startY, footW, footH);
           
            dGraphics.DrwColor = "black";
          
            float ylimit = (fData.W1 > fData.W2 ? fData.W1 : fData.W2);

            float xMax = fData.X1 + fData.X2 + fData.X3;
            float xMin = 0;
            float yMax = 0.5f * ylimit;
            float yMin = -0.5f * ylimit;
            float offset = 0.3f * (footH > footW ? footW :footH);
            float marginWidth = footW- offset;
            float marginHeight = footH - offset;
            float hratio = marginWidth / (xMax - xMin);
            float vratio = (yMax - yMin) / marginHeight;

            float cw1 = fData.Cw1;
            float cw2 = fData.Cw2;
            float cl1 = vratio * hratio * fData.Cl1;
            float cl2 = vratio * hratio * fData.Cl2;
            SetGraphicLayout(footW, footH, startX, dChart, new float[] { xMin, xMax, yMin, yMax });
            PointF fPt1 = new PointF(0, -0.5f * fData.W1);
            PointF fPt2 = new PointF(0, 0.5f * fData.W1);
            PointF fPt3 = new PointF(xMax, 0.5f * fData.W2);
            PointF fPt4 = new PointF(xMax, -0.5f * fData.W2);
            fPt1 = dChart.Point2D(fPt1);
            fPt2 = dChart.Point2D(fPt2);
            fPt3 = dChart.Point2D(fPt3);
            fPt4 = dChart.Point2D(fPt4);

            dGraphics.FillColorOpt = "lightslategrey";
            dGraphics.DrwColor = "dimgrey";
            dGraphics.FillPolygone(new PointF[] { fPt1, fPt2, fPt3, fPt4 });
        
            SetGraphicLayout(footW, footH, startX, dChart, new float[] { xMin, xMax, yMin, yMax });
            PointF cPt1 = new PointF(fData.X1 - 0.5f * cw1, -0.5f * cl1);
            PointF cPt2 = new PointF(fData.X1 + 0.5f * cw1, 0.5f * cl1);
            cPt1 = dChart.Point2D(cPt1);
            cPt2 = dChart.Point2D(cPt2);
            dGraphics.FillColorOpt = "darkslategrey";
           dGraphics.FillColor(cPt1, cPt2);
            dGraphics.DrwColor = "dimgrey";
             dGraphics.DrawRec(cPt1, cPt2);
            cPt1 = new PointF(fData.X1 + fData.X2 - 0.5f * cw2, -0.5f * cl2);
            cPt2 = new PointF(fData.X1 + fData.X2 + 0.5f * cw2, 0.5f * cl2);
            cPt1 = dChart.Point2D(cPt1);
            cPt2 = dChart.Point2D(cPt2);
            dGraphics.FillColorOpt = "darkslategrey";
             dGraphics.FillColor(cPt1, cPt2);
            dGraphics.DrwColor = "dimgrey";
            dGraphics.DrawRec(cPt1, cPt2);
            dGraphics.DrwColor = "cadetblue";
            dim.Graphic = dGraphics;
            dim.DimLoc = DimensionLine.DimensionLocation.end;
            dim.DimDir = DimensionLine.DimensionDirection.right;
            dGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawDimLineAligned(fPt1, fPt2, string.Format("W1={0}", fData.W1));
             dim.DrawDimLineAligned(fPt3, fPt4, string.Format("W2={0}", fData.W2));
            dim.DimDir = DimensionLine.DimensionDirection.left;
            dGraphics.TextBase = TextBaseline.Hanging;
            float lower = (fData.W1 > fData.W2 ? -0.5f * fData.W1 : -0.5f * fData.W2);
            PointF cPt0 = new PointF(0, lower);
            cPt0 = dChart.Point2D(cPt0);
            cPt1 = new PointF(fData.X1, lower);
            cPt1 = dChart.Point2D(cPt1);
            cPt2 = new PointF(fData.X1 + fData.X2, lower);
            cPt2 = dChart.Point2D(cPt2);
            PointF cPt3 = new PointF(fData.X1 + fData.X2 + fData.X3, lower);
            cPt3 = dChart.Point2D(cPt3);
             dim.DrawDimLineAligned(cPt0, cPt1, string.Format("X1={0}", fData.X1));
            dim.DrawDimLineAligned(cPt1, cPt2, string.Format("X2={0}", fData.X2));
             dim.DrawDimLineAligned(cPt2, cPt3, string.Format("X3={0}", fData.X3));
            Symbols symb = new Symbols();
            symb.Graphic = dGraphics;
            float arLength = 60;
            float aw = 5f;
            float ah = 10f;
            float off = 40f;
            float angle = 0;
            PointF cpt1 = new PointF(fData.X1, 0);

            PointF mpt1x = new PointF(cpt1.X + arLength, 0);
            PointF mpt1mx = new PointF(cpt1.X + 0.5f * arLength, 0);
            PointF mpt1y = new PointF(cpt1.X, -arLength);
            PointF mpt1my = new PointF(cpt1.X, -0.5f * arLength);
            cpt1 = dChart.Point2D(cpt1);
            mpt1x = dChart.Point2D(mpt1x);
            mpt1mx = dChart.Point2D(mpt1mx);
            dGraphics.DrwColor = "red";
            dGraphics.DrawLine(cpt1, mpt1x);
            angle = CalcAngle(cpt1, mpt1x);
            if (fData.Loads[0].Mx > 0)
            {
                 symb.DrawArrow(mpt1x, aw, ah, angle);
                 symb.DrawArrow(mpt1mx, aw, ah, angle);
            }
            else
            {
                angle += 180;
                 symb.DrawArrow(cpt1, aw, ah, angle);
                 symb.DrawArrow(mpt1mx, aw, ah, angle);
            }
            dGraphics.FillColorOpt = "white";
            dim.DrawTextAtAngle(mpt1x, angle, string.Format("Mx={0}", fData.Loads[0].Mx));
            mpt1y = dChart.Point2D(mpt1y);
            mpt1my = dChart.Point2D(mpt1my);
            dGraphics.DrwColor = "red";
            dGraphics.DrawLine(cpt1, mpt1y);
            angle = -CalcAngle(cpt1, mpt1y);
            if (fData.Loads[0].My > 0)
            {
                symb.DrawArrow(cpt1, aw, ah, angle);
                 symb.DrawArrow(mpt1my, aw, ah, angle);
            }
            else
            {
                angle = -angle;
                symb.DrawArrow(mpt1y, aw, ah, angle);
                symb.DrawArrow(mpt1my, aw, ah, angle);
            }

            dGraphics.FillColorOpt = "white";
           dim.DrawTextAtAngle(mpt1y, angle, string.Format("My={0}", fData.Loads[0].My));
            cpt1 = new PointF(fData.X1, off);
            cpt1 = dChart.Point2D(cpt1);
            dGraphics.FillColorOpt = "white";
             dim.DrawTextAtAngle(cpt1, 0, string.Format("Pn={0}", fData.Loads[0].Pn));
            PointF cpt2 = new PointF(fData.X1 + fData.X2, 0);
            mpt1x = new PointF(cpt2.X - arLength, 0);
            mpt1mx = new PointF(cpt2.X - 0.5f * arLength, 0);
            mpt1y = new PointF(cpt2.X, -arLength);
            mpt1my = new PointF(cpt2.X, -0.5f * arLength);
            cpt2 = dChart.Point2D(cpt2);
            mpt1x = dChart.Point2D(mpt1x);
            mpt1mx = dChart.Point2D(mpt1mx);
            dGraphics.DrwColor = "red";
            dGraphics.DrawLine(cpt2, mpt1x);
            angle = CalcAngle(cpt2, mpt1x);
            if (fData.Loads[1].Mx > 0)
            {
                 symb.DrawArrow(cpt2, aw, ah, angle);
                 symb.DrawArrow(mpt1mx, aw, ah, angle);
            }
            else
            {
                angle += 180;
               symb.DrawArrow(mpt1x, aw, ah, angle);
                symb.DrawArrow(mpt1mx, aw, ah, angle);
            }

            dGraphics.FillColorOpt = "white";
             dim.DrawTextAtAngle(mpt1x, angle, string.Format("Mx={0}", fData.Loads[1].Mx));
            mpt1y = dChart.Point2D(mpt1y);
            mpt1my = dChart.Point2D(mpt1my);
            dGraphics.DrwColor = "red";
             dGraphics.DrawLine(cpt2, mpt1y);
            angle = -CalcAngle(cpt2, mpt1y);
            if (fData.Loads[1].My > 0)
            {
                symb.DrawArrow(cpt2, aw, ah, angle);
                 symb.DrawArrow(mpt1my, aw, ah, angle);
            }
            else
            {
                angle += 180;
                 symb.DrawArrow(mpt1y, aw, ah, angle);
               symb.DrawArrow(mpt1my, aw, ah, angle);
            }
            dGraphics.FillColorOpt = "white";
            dim.DrawTextAtAngle(mpt1y, angle, string.Format("My={0}", fData.Loads[1].My));
            cpt2 = new PointF(fData.X1 + fData.X2, off);
            cpt2 = dChart.Point2D(cpt2);
             dim.DrawTextAtAngle(cpt2, 0, string.Format("Pn={0}", fData.Loads[1].Pn));
          
        }
        protected void CombFootDetailing()
        {
            if (continueDetailing)
            {
                DrawPlanDetail();
                DrawElevDetail();
                DrawDetail();
            }
        }
        protected void DrawPlanDetail()
        {
            DimensionLine dim = new DimensionLine();
            detGraphics = new DovDrawings(detContext);
            detChart = new ChartStyle();
            float footW =DetailWidth;
            float footH = 0.4f *DetailHeight;
            detChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0f;
            float startY = 0f;
             detGraphics.Clear(startX, startY, footW, footH);

            detGraphics.DrwColor = "black";

            float ylimit = (fData.W1 > fData.W2 ? fData.W1 : fData.W2);
            float l = fData.X1 + fData.X2 + fData.X3;
            float xMax = l;
            float xMin = 0;
            float yMax = 0.5f * ylimit;
            float yMin = -0.5f * ylimit;
            float offset = 0.3f * (footH > footW ? footW : footH);
            float marginWidth = footW - offset;
            float marginHeight = footH - offset;
            float hratio = marginWidth / (xMax - xMin);
            float vratio = (yMax - yMin) / marginHeight;

            float cw1 = fData.Cw1;
            float cw2 = fData.Cw2;
            float cl1 = vratio * hratio * fData.Cl1;
            float cl2 = vratio * hratio * fData.Cl2;
            float c = fData.Cover * u.deci;//concrete cover in centimeter
            SetGraphicLayout(footW, footH, startX, detChart, new float[] { xMin, xMax, yMin, yMax });
            PointF fPt1 = new PointF(0, -0.5f * fData.W1);
            PointF fPt2 = new PointF(0, 0.5f * fData.W1);
            PointF fPt3 = new PointF(xMax, 0.5f * fData.W2);
            PointF fPt4 = new PointF(xMax, -0.5f * fData.W2);
            fPt1 = detChart.Point2D(fPt1);
            fPt2 = detChart.Point2D(fPt2);
            fPt3 = detChart.Point2D(fPt3);
            fPt4 = detChart.Point2D(fPt4);

            detGraphics.FillColorOpt = "lightslategrey";
            detGraphics.DrwColor = "dimgrey";
             detGraphics.FillPolygone(new PointF[] { fPt1, fPt2, fPt3, fPt4 });

            SetGraphicLayout(footW, footH, startX, detChart, new float[] { xMin, xMax, yMin, yMax });
            PointF cPt1 = new PointF(fData.X1 - 0.5f * cw1, -0.5f * cl1);
            PointF cPt2 = new PointF(fData.X1 + 0.5f * cw1, 0.5f * cl1);
            cPt1 = detChart.Point2D(cPt1);
            cPt2 = detChart.Point2D(cPt2);
            detGraphics.FillColorOpt = "darkslategrey";
            detGraphics.FillColor(cPt1, cPt2);
            detGraphics.DrwColor = "dimgrey";
            detGraphics.DrawRec(cPt1, cPt2);
            cPt1 = new PointF(fData.X1 + fData.X2 - 0.5f * cw2, -0.5f * cl2);
            cPt2 = new PointF(fData.X1 + fData.X2 + 0.5f * cw2, 0.5f * cl2);
            cPt1 = detChart.Point2D(cPt1);
            cPt2 = detChart.Point2D(cPt2);
            detGraphics.FillColorOpt = "darkslategrey";
             detGraphics.FillColor(cPt1, cPt2);
            detGraphics.DrwColor = "dimgrey";
             detGraphics.DrawRec(cPt1, cPt2);
            detGraphics.DrwColor = "cadetblue";
            dim.Graphic = detGraphics;
            dim.DimLoc = DimensionLine.DimensionLocation.end;
            dim.DimDir = DimensionLine.DimensionDirection.right;
            detGraphics.TextBase = TextBaseline.Bottom;
             dim.DrawDimLineAligned(fPt1, fPt2, string.Format("W1={0}", fData.W1));
             dim.DrawDimLineAligned(fPt3, fPt4, string.Format("W2={0}", fData.W2));
            dim.DimDir = DimensionLine.DimensionDirection.left;
            detGraphics.TextBase = TextBaseline.Hanging;
            float lower = (fData.W1 > fData.W2 ? -0.5f * fData.W1 : -0.5f * fData.W2);
            float higher = (fData.W1 > fData.W2 ? -0.5f * fData.W2 : -0.5f * fData.W1);
            PointF cPt0 = new PointF(0, lower);
            cPt0 = detChart.Point2D(cPt0);
            cPt1 = new PointF(fData.X1, lower);
            cPt1 = detChart.Point2D(cPt1);
            cPt2 = new PointF(fData.X1 + fData.X2, lower);
            cPt2 = detChart.Point2D(cPt2);
            PointF cPt3 = new PointF(fData.X1 + fData.X2 + fData.X3, lower);
            cPt3 = detChart.Point2D(cPt3);
            dim.DrawDimLineAligned(cPt0, cPt1, string.Format("X1={0}", fData.X1));
            dim.DrawDimLineAligned(cPt1, cPt2, string.Format("X2={0}", fData.X2));
            dim.DrawDimLineAligned(cPt2, cPt3, string.Format("X3={0}", fData.X3));
            //Rebar Detailings 
            detGraphics.DrwColor = "red";
            detGraphics.FillColorOpt = "yellow";
            float dia = fData.Dia;
            float barArea = (float)Math.PI * dia * dia / 4;
            int nbar = (int)Math.Ceiling(fDesign.Asx / barArea);
            int barSpacingBottom =(int) fDesign.Wspan / nbar;
            barSpacingBottom = barSpacingBottom.RoundDown() ;
            nbar = (int)Math.Ceiling(fDesign.Asy / barArea);
            int barSpacingBottomTrans = 1000 / nbar;
            barSpacingBottomTrans = barSpacingBottomTrans.RoundDown() ;
            int ns1 = (int)Math.Ceiling(fDesign.AsxSup1 / barArea);
            int ns2 = (int)Math.Ceiling(fDesign.AsxSup2 / barArea);
            
            nbar = ns1 > ns2 ? ns1 : ns2;
            int barSpacingTop =(int) (fDesign.Wsup1>fDesign.Wsup2?fDesign.Wsup2:fDesign.Wsup1) / nbar;
            barSpacingTop = barSpacingTop.RoundDown();

            float barLocV1 = higher + 3 * c;
            float barLocH1 = c;
            float thick = fData.Thickness;
            PointF pt1 = new PointF(barLocH1, barLocV1);
            PointF pt2 = new PointF(l - c, barLocV1);
            PointF pt3 = new PointF(barLocH1, barLocV1 + thick - c);
            PointF pt4 = new PointF(l - c, barLocV1 + thick - c);
            pt1 = detChart.Point2D(pt1);
            pt2 = detChart.Point2D(pt2);
            pt3 = detChart.Point2D(pt3);
            pt4 = detChart.Point2D(pt4);
           detGraphics.DrawLine(pt1, pt2);
            detGraphics.DrawLine(pt1, pt3);
            detGraphics.DrawLine(pt2, pt4);
            pt1 = new PointF(0.5f * l, barLocV1);
            pt1 = detChart.Point2D(pt1);
            dim.DimDir = DimensionLine.DimensionDirection.left;
            detGraphics.TextAlignment = TextAlign.Left;
            detGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawTextAtAngle(pt1, 0, string.Format("{0} c/c {1} mm", dia, barSpacingBottom.ToString()));
             lower = (fData.W1 > fData.W2 ? 0.5f * fData.W2 : 0.5f * fData.W1);
            barLocV1 = lower - 3 * c;
            barLocH1 = c;
            pt1 = new PointF(barLocH1, barLocV1);
            pt2 = new PointF(l - c, barLocV1);
            pt3 = new PointF(barLocH1, barLocV1 - thick + c);
            pt4 = new PointF(l - c, barLocV1 - thick + c);
            pt1 = detChart.Point2D(pt1);
            pt2 = detChart.Point2D(pt2);
            pt3 = detChart.Point2D(pt3);
            pt4 = detChart.Point2D(pt4);
             detGraphics.DrawLine(pt1, pt2);
             detGraphics.DrawLine(pt1, pt3);
            detGraphics.DrawLine(pt2, pt4);
            pt1 = new PointF(0.5f * l, barLocV1);
            pt1 = detChart.Point2D(pt1);
            dim.DimDir = DimensionLine.DimensionDirection.left;
            detGraphics.TextAlignment = TextAlign.Left;
            detGraphics.TextBase = TextBaseline.Hanging;
             dim.DrawTextAtAngle(pt1, 0, string.Format("{0} c/c {1} mm", dia, barSpacingTop.ToString()));
            // transverse bar 
            float b1 = fData.W1; float b2 = fData.W2;
            float avb = 0.5f * (b1 + b2);
            pt1 = new PointF(0.5f * l, -0.5f * avb+c);
            pt2 = new PointF(0.5f * l, 0.5f * avb - c);
            pt3 = new PointF(0.5f * l-thick+c, -0.5f*avb+c);
            pt4 = new PointF(0.5f * l - thick + c, 0.5f * avb - c);
            pt1 = detChart.Point2D(pt1);
            pt2 = detChart.Point2D(pt2);
            pt3 = detChart.Point2D(pt3);
            pt4 = detChart.Point2D(pt4);
             detGraphics.DrawLine(pt1, pt2);
             detGraphics.DrawLine(pt1, pt3);
            detGraphics.DrawLine(pt2, pt4);
            pt1 = new PointF(0.5f * l, 0);
            pt1 = detChart.Point2D(pt1);
            dim.DimDir = DimensionLine.DimensionDirection.left;
            detGraphics.TextAlignment = TextAlign.Center;
            detGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawTextAtAngle(pt1,pt2, string.Format("{0} c/c {1} mm", dia, barSpacingBottomTrans.ToString()));
        }
        protected void DrawElevDetail()
        {
            DimensionLine dim = new DimensionLine();
            detGraphics = new DovDrawings(detContext);
            detChart = new ChartStyle();
            float footW = DetailWidth;
            float footH = 0.3f * DetailHeight;
            detChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0f;
            float startY = 0.4f * DetailHeight;
             detGraphics.Clear(startX, startY, footW, footH);
            detGraphics.DrwColor = "black";
            float x1 = fData.X1;
            float x2 = fData.X2;
            float x3 = fData.X3;
            float thick = fData.Thickness;
            float l = x1 + x2 + x3;
            float ylimit = 2 * thick ;
            float xMax = x1+x2+x3;
            float xMin = 0;
            float yMax = ylimit;
            float yMin = 0;
            float cw1 = fData.Cw1;
            float cw2 = fData.Cw2;
            float cl1 = fData.Cl1;
            float cl2 =  fData.Cl2;
            float l1 = x1 + x2 + 0.5f * cw2;
            float l2 = x1 + x2 - 0.5f * cw2;
            float l3 = x1 + 0.5f * cw1;
            float l4 = x1 - 0.5f * cw1;
            SetGraphicLayout(footW, footH, startX,startY, detChart, new float[] { xMin, xMax, yMin, yMax });
            PointF pt1 = new PointF(0, 0);
            PointF pt2 = new PointF(l, 0);
            PointF pt3 = new PointF(l,thick);
            PointF pt4 = new PointF(l1,thick);
            PointF pt5 = new PointF(l1, 2*thick);
            PointF pt6 = new PointF(l2, 2*thick);
            PointF pt7 = new PointF(l2, thick);
            PointF pt8 = new PointF(l3, thick);
            PointF pt9 = new PointF(l3, 2*thick);
            PointF pt10= new PointF(l4, 2*thick);
            PointF pt11 = new PointF(l4, thick);
            PointF pt12 = new PointF(0, thick);
            PointF[] pts = new PointF[] { pt1, pt2, pt3, pt4, pt5, pt6, pt7, pt8, pt9, pt10, pt11, pt12 };
            for(int i = 0; i < pts.Length; i++)
            {
                pts[i] = detChart.Point2D(pts[i]);
            }

            detGraphics.FillColorOpt = "lightslategrey";
            detGraphics.DrwColor = "dimgrey";
             detGraphics.FillPolygone(pts);
            detGraphics.DrwColor = "cadetblue";
            dim.Graphic = detGraphics;
            dim.DimLoc = DimensionLine.DimensionLocation.end;
            dim.DimDir = DimensionLine.DimensionDirection.left;
            detGraphics.TextBase = TextBaseline.Hanging;
            pt1 = detChart.Point2D(pt1);
            pt2 = detChart.Point2D(pt2);
             dim.DrawDimLineAligned(pt1, pt2, string.Format("L={0}", l));
            pt12 = detChart.Point2D(pt12);
            pt11 = detChart.Point2D(pt11);
            dim.DimDir = DimensionLine.DimensionDirection.right;
            detGraphics.TextBase = TextBaseline.Bottom;
             dim.DrawDimLineAligned(pt12, pt11, string.Format("{0}", l4));
            pt8 = detChart.Point2D(pt8);
            pt7 = detChart.Point2D(pt7);
            dim.DimDir = DimensionLine.DimensionDirection.right;
            detGraphics.TextBase = TextBaseline.Bottom;
             dim.DrawDimLineAligned(pt8, pt7, string.Format("{0}", x2-0.5f*cw1-0.5f*cw2));
            pt4 = detChart.Point2D(pt4);
            pt3 = detChart.Point2D(pt3);
            dim.DimDir = DimensionLine.DimensionDirection.right;
            detGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawDimLineAligned(pt4, pt3, string.Format("{0}", x3-0.5f*cw2));
            float c = fData.Cover*u.deci;//centimeter
            PointF pd1 = new PointF(c , c );//centimeter
            PointF pd2 = new PointF(l - c, c);
            PointF pd3 = new PointF(c, thick -  c);
            PointF pd4 = new PointF(l - c, thick - c);
            detGraphics.DrwColor = "red";
            pd1 = detChart.Point2D(pd1);
            pd2 = detChart.Point2D(pd2);
            pd3 = detChart.Point2D(pd3);
            pd4 = detChart.Point2D(pd4);
            detGraphics.DrawLine(pd1, pd2);
            detGraphics.DrawLine(pd1, pd3);
            detGraphics.DrawLine(pd2, pd4);
            pd1 = new PointF(c, thick-c);//centimeter
            pd2 = new PointF(l - c, thick-c);
            pd3 = new PointF(c,  c);
            pd4 = new PointF(l - c,  c);
            pd1 = detChart.Point2D(pd1);
            pd2 = detChart.Point2D(pd2);
            pd3 = detChart.Point2D(pd3);
            pd4 = detChart.Point2D(pd4);
             detGraphics.DrawLine(pd1, pd2);
            detGraphics.DrawLine(pd1, pd3);
            detGraphics.DrawLine(pd2, pd4);
            //scale out diameter for visibility
            float dia = 3f * fData.Dia*u.deci;//centimeter
            float devLength = 47f * fData.Dia * u.deci;//centimeter
            float cantiLength1 = x1 - 0.5f * cw1-c;
            float cantiLength2 = x3 - 0.5f * cw2-c;
            float colBarLengthBottom = cantiLength1 > (devLength - thick - c) ? cantiLength1 : devLength - thick-c;
            float colBarLengthBottom1 = cantiLength2 > (devLength - thick - c) ? cantiLength2 : devLength - thick-c;
            //Drawing Column Rebars First Column
            PointF pd5= new PointF(l4+c, 2 * thick-c);
            PointF pd6 = new PointF(l4 + c, c + dia);
            pd5 = detChart.Point2D(pd5);
            pd6 = detChart.Point2D(pd6);
            PointF pd7 = new PointF(l4 -colBarLengthBottom+c , c + dia);
            pd7 = detChart.Point2D(pd7);
            detGraphics.DrawLine(pd5, pd6);
             detGraphics.DrawLine(pd6, pd7);
            //column right side bar
            pd5= new PointF(l3-c, 2 * thick-c);
            pd6 = new PointF(l3 - c, c + dia);
            pd7 = new PointF(l3  + devLength-thick+c, c + dia);
            pd5 = detChart.Point2D(pd5);
            pd6 = detChart.Point2D(pd6);
            pd7 = detChart.Point2D(pd7);
             detGraphics.DrawLine(pd5, pd6);
             detGraphics.DrawLine(pd6, pd7);
            // second column rebar leftside
            pd5 = new PointF(l2+c, 2 * thick-c);
            pd6 = new PointF(l2 + c, c + dia);
            pd7 = new PointF(l2 - devLength + thick, c + dia);
            pd5 = detChart.Point2D(pd5);
            pd6 = detChart.Point2D(pd6);
            pd7 = detChart.Point2D(pd7);
             detGraphics.DrawLine(pd5, pd6);
            detGraphics.DrawLine(pd6, pd7);
            //right side rebar
            pd5 = new PointF(l1-c, 2 * thick-c);
            pd6 = new PointF(l1 - c, c + dia);
            pd7 = new PointF(l1 + colBarLengthBottom1-c, c + dia);
            pd5 = detChart.Point2D(pd5);
            pd6 = detChart.Point2D(pd6);
            pd7 = detChart.Point2D(pd7);
            detGraphics.DrawLine(pd5, pd6);
            detGraphics.DrawLine(pd6, pd7);
            dia = fData.Dia;
            float barArea = (float)Math.PI * dia * dia / 4;
            int nbar = (int)Math.Ceiling(fDesign.Asy / barArea);
            int barSpacing = 1000 / nbar;
            barSpacing = barSpacing.RoundDown() / 10;
            for (int i = (int)(2 * c); i < (l - 2 * c); i += barSpacing)
            {
             
                float y =  c + 1.5f * dia *u.deci;
                float x =  i;
                PointF pt = new PointF(x, y);
                pt = detChart.Point2D(pt);
                detGraphics.DrawArc(pt.X, pt.Y, 1.5f * dia * 0.1f, 0, 2 * Math.PI);
            }
            barSpacing = 30;//minimum apacing in cm; transverse top reinforcement;
            for (int i = (int)(2 * c); i < (l - 2 * c); i += barSpacing)
            {

                float y =thick- c - 1.5f * dia * u.deci;
                float x = i;
                PointF pt = new PointF(x, y);
                pt = detChart.Point2D(pt);
                 detGraphics.DrawArc(pt.X, pt.Y, 1.5f * dia * 0.1f, 0, 2 * Math.PI);
            }
        }
        protected void DrawDetail()
        {
            DimensionLine dim = new DimensionLine();
            dim.Graphic = detGraphics;
            detGraphics = new DovDrawings(detContext);
            detChart = new ChartStyle();
            float footW = DetailWidth;
            float footH = 0.33f * DetailHeight;
            detChart.ChartArea = new RectangleF(0, 0, footW, footH);
            float startX = 0f;
            float startY = 2*0.33f * DetailHeight;
            detGraphics.Clear(startX, startY, footW, footH);
            detGraphics.DrwColor = "black";
            float x1 = fData.X1;
            float x2 = fData.X2;
            float x3 = fData.X3;
            float c = fData.Cover * u.deci;//centimeter
            float thick = fData.Thickness;
            float l = x1 + x2 + x3;
            float ylimit = 2 * thick+3*c;
            float xMax = x1 + x2 + x3;
            float xMin = 0;
            float yMax = ylimit;
            float yMin = 0;
            float cw1 = fData.Cw1;
            float cw2 = fData.Cw2;
            float cl1 = fData.Cl1;
            float cl2 = fData.Cl2;
           
            SetGraphicLayout(footW, footH, startX, startY, detChart, new float[] { xMin, xMax, yMin, yMax });
            dim.DimDir = DimensionLine.DimensionDirection.right;
            detGraphics.TextBase = TextBaseline.Bottom;
           
            PointF pd1 = new PointF(c, c);//centimeter
            PointF pd2 = new PointF(l - c, c);
            PointF pd3 = new PointF(c, thick - c);
            PointF pd4 = new PointF(l - c, thick - c);
            detGraphics.DrwColor = "red";
            pd1 = detChart.Point2D(pd1);
            pd2 = detChart.Point2D(pd2);
            pd3 = detChart.Point2D(pd3);
            pd4 = detChart.Point2D(pd4);
             detGraphics.DrawLine(pd1, pd2);
             detGraphics.DrawLine(pd1, pd3);
             detGraphics.DrawLine(pd2, pd4);
            detGraphics.FillColorOpt = "lightslategrey";
            detGraphics.DrwColor = "dimgrey";
            dim.Graphic = detGraphics;
            dim.DimDir = DimensionLine.DimensionDirection.left;
            detGraphics.TextBase = TextBaseline.Hanging;
             dim.DrawDimLineAligned(pd1, pd2,(l-2*c).ToString());
            dim.DimDir = DimensionLine.DimensionDirection.right;
            detGraphics.TextBase = TextBaseline.Bottom;
             dim.DrawDimLineAligned(pd1, pd3, (thick-c).ToString());
            pd1 = new PointF(c, 2*thick+c );//centimeter
            pd2 = new PointF(l - c, 2*thick+c);
            pd3 = new PointF(c, thick+2*c);
            pd4 = new PointF(l - c, thick+2*c);
            pd1 = detChart.Point2D(pd1);
            pd2 = detChart.Point2D(pd2);
            pd3 = detChart.Point2D(pd3);
            pd4 = detChart.Point2D(pd4);
            detGraphics.DrwColor = "red";
             detGraphics.DrawLine(pd1, pd2);
             detGraphics.DrawLine(pd1, pd3);
             detGraphics.DrawLine(pd2, pd4);
            detGraphics.FillColorOpt = "lightslategrey";
            detGraphics.DrwColor = "dimgrey";
            dim.Graphic = detGraphics;
            dim.DimDir = DimensionLine.DimensionDirection.right;
            detGraphics.TextBase = TextBaseline.Bottom;
            dim.DrawDimLineAligned(pd1, pd2, (l-2*c).ToString());
            dim.DimDir = DimensionLine.DimensionDirection.left;
            detGraphics.TextBase = TextBaseline.Hanging;
            dim.DrawDimLineAligned(pd1, pd3, (thick-c).ToString());
        }
        /// Rectangular Beam Design Graphics
        /// 
    }

}

