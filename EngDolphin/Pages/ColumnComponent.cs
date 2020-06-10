using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using EngDolphin.Client.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using EngDolphin.Canvas;
using static EngDolphin.Canvas.CanvasEnum;

namespace EngDolphin.Pages
{
    public class ColumnComponent : ComponentBase
    {
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        protected int value;
        protected ElementReference SectionCanvas { get; set; }
        protected ElementReference DiagramCanvas { get; set; }
        protected ElementReference BiaxialDiagramCanvas { get; set; }
        protected ElementReference ColumnNeutralAxisCanvas { get; set; }
        protected DovRebarLocation[] re = new DovRebarLocation[] { };
        protected bool doveShowMaterial = false;
        protected bool doveShowSection = false;
        protected bool doveShowDesign = false;
        protected bool continueDesign = true;
        protected string displayConcMat = "dovOff";
        protected string displayRebarMat = "dovOff";
      
        protected int rebarActiveRow = 0;
        protected int rebarMaxRow = 2;
        protected bool dispDsgnPref = false;
        protected bool dispDsgnRslt = false;
        protected bool dispChartTable = false;
        protected bool dispUniAxial = true;
        protected bool dispBiAxial = false;
        protected string RebarLocationMessage = "Enter Location of rebars";
        protected string RebarLocMsgColor = "";
        protected string RunorPref = "Preference";
        protected string UniorBiAxial = "Uniaxial Chart";
        protected float ColumnNeutralAxisAngle = 0;
        protected float test1;
        protected float test2;
        protected float test3;
        protected float test4;
        protected bool test6;
        protected Point3[] test5 = new Point3[1];
        // Drawings 
        protected string SecBackColor { get; set; } = "black";
        protected string DsgnBackColor { get; set; } = "black";
        public long SecCanvWidth { get; set; } = 300;
        public long SecCanvHeight { get; set; } = 300;
        public long DiagramCanvWidth { get; set; } = 700;
        public long DiagramCanvHeight { get; set; } = 700;
        public long BiaxialCanvWidth { get; set; } = 700;
        public long BiaxialCanvHeight { get; set; } = 700;
        public long ClmnNeutralAxisWidth { get; set; } = 200;
        public long ClmnNeutralAxisHeight { get; set; } = 200;
        public float X { get; set; }
        public float Y { get; set; }
        protected float rebarLowestLocation { get; set; } = 0;
        protected List<DovConcMat> dovConcMatLibrary = new List<DovConcMat>();
        protected List<DovRebarMat> dovRebarMatLibrary = new List<DovRebarMat>();
        protected DovConcMat concMat = new DovConcMat();
        protected DovRebarMat rebarMat = new DovRebarMat();
        protected DovSectionRec secRec = new DovSectionRec(200, 400);
        protected List<DovRebarLocation> rebarLocLibrary = new List<DovRebarLocation>();
        protected DovDsgnPreference DsgnPref = new DovDsgnPreference();
        protected IDictionary<float, Stack<DovRebarLocation>> RebarLocSorted = new Dictionary<float, Stack<DovRebarLocation>>();
        protected DovColumnDesign ColumnInter = new DovColumnDesign();
        public DovDrawings SecGraphics { get; set; }
        public DovDrawings DiagramGraphics { get; set; }
        public DovDrawings BiaxialGraphics { get; set; }
        public DovDrawings NeutralAxisGraphics { get; set; }
        private CanvasContext diagramContext;
        private CanvasContext biaxialContext;
        private CanvasContext sectionContext;
        private CanvasContext clmnNeutralAxisContext;
        private ChartStyle sectionChart;
        private ChartStyle diagramChart;
        private ChartStyle biaxialChart;
        private ChartStyle neutralAxisChart;
        protected override void OnInitialized()
        {
            Point3 pt = new Point3();
            test5[0] = pt;
            concMat = new DovConcMat() { Name = "25MPa", UnitWt = 25, Fck = 25, PoissonRatio = 0.2f, E = 29000 };
            dovConcMatLibrary.Add(concMat);
            concMat = new DovConcMat() { Name = "15MPa", UnitWt = 25, Fck = 15, PoissonRatio = 0.2f, E = 29000 };
            dovConcMatLibrary.Add(concMat);

            rebarMat = new DovRebarMat() { Name = "355MPa", UnitWt = 78.5f, Fyk = 355, PoissonRatio = 0.3f, E = 200000 };
            dovRebarMatLibrary.Add(rebarMat);
            rebarMat = new DovRebarMat() { Name = "400MPa", UnitWt = 78.5f, Fyk = 400, PoissonRatio = 0.3f, E = 200000 };
            dovRebarMatLibrary.Add(rebarMat);

            DovRebarLocation rLoc = new DovRebarLocation(1, 16, -67, -167);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(2, 16, 67, -167);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(3, 16, 67, -117);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(4, 16, -67, -117);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(5, 16, 67, -67);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(6, 16, -67, -67);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(7, 16, 67, 0);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(8, 16, -67, 0);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(9, 16, 67, 167);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(10, 16, -67, 167);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(11, 16, 67, 117);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(12, 16, -67, 117);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(13, 16, 67, 67);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(14, 16, -67, 67);
            rebarLocLibrary.Add(rLoc);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            this.diagramContext = new CanvasContext(DiagramCanvas, JsRuntime);
            DiagramGraphics = new DovDrawings(diagramContext);
            this.biaxialContext = new CanvasContext(BiaxialDiagramCanvas, JsRuntime);
            //
            this.sectionContext = new CanvasContext(SectionCanvas, JsRuntime);
            SecGraphics = new DovDrawings(sectionContext);
            this.clmnNeutralAxisContext = new CanvasContext(ColumnNeutralAxisCanvas, JsRuntime);
            NeutralAxisGraphics = new DovDrawings(clmnNeutralAxisContext);

            sectionChart = new ChartStyle();
            diagramChart = new ChartStyle();
            biaxialChart = new ChartStyle();
            neutralAxisChart = new ChartStyle();
        }
        protected void ShowConcMatProp(ChangeEventArgs e)
        {
            foreach (var mat in dovConcMatLibrary)
            {
                if (mat.Name == e.Value.ToString())
                {
                    concMat = new DovConcMat() {Name=mat.Name,UnitWt=mat.UnitWt,Fck=mat.Fck,PoissonRatio=mat.PoissonRatio,E=mat.E,St=mat.St };
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
                    concMat = new DovConcMat() { Name=mat.Name,UnitWt=mat.UnitWt,Fck=mat.Fck,PoissonRatio=mat.PoissonRatio,E=mat.E,St=mat.St};
                }
            }
        }
        protected void SelectedRebarMat(ChangeEventArgs e)
        {
            foreach (var rebar in dovRebarMatLibrary)
            {
                if (rebar.Name == e.Value.ToString())
                {
                    rebarMat = new DovRebarMat() {Name=rebar.Name,UnitWt=rebar.UnitWt,Fyk=rebar.Fyk,PoissonRatio=rebar.PoissonRatio,E=rebar.E };
                }
            }
        }

        protected void RebarActivateRow(MouseEventArgs e, int n)
        {
            rebarActiveRow = n;
            foreach (var rebars in rebarLocLibrary.ToList())
            {
                if (rebars.No == n)
                {
                    rebars.Activated = true;
                }
                else
                {
                    rebars.Activated = false;
                }
            }
        }

        protected void DeleteRebarRow()
        {
            foreach (var rebars in rebarLocLibrary.ToList())
            {
                if (rebars.No == rebarActiveRow && rebars.No > 2)
                {
                    rebarLocLibrary.Remove(rebars);
                }
            }
        }

        protected void InsertRebarRow()
        {
            rebarMaxRow++;
            DovRebarLocation reb = new DovRebarLocation(rebarMaxRow, 12, 100, 50);
            int n = 0;
            foreach (var rebar in rebarLocLibrary.ToList())
            {
                if (rebar.No == rebarActiveRow)
                {
                    rebarLocLibrary.Insert(n + 1, reb);
                }
                n++;
            }
        }
        protected void DoveDisplay(string page)
        {
            switch (page)
            {
                case "section":
                    doveShowSection = !doveShowSection;
                    doveShowMaterial = false;
                    doveShowDesign = false;
                    break;
                case "material":
                    doveShowMaterial = !doveShowMaterial;
                    doveShowSection = false;
                    doveShowDesign = false;
                    break;
                case "design":
                    doveShowDesign = !doveShowDesign;
                    doveShowSection = false;
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
                    dispChartTable = false;
                    RunorPref = "Run"; break;
                case "Run":
                    dispDsgnPref = false;
                    dispChartTable = true;
                    if (UniorBiAxial == "Uniaxial Chart")
                    {
                        dispUniAxial = true;
                        dispBiAxial = false;
                        CalcUniaxialInteraction();
                        DrawDiagramGraphics();
                       DrawUniInterChart();
                    }
                    else
                    {
                        dispUniAxial = false;
                        dispBiAxial = true;
                        CalcBiaxialInteraction();
                       DrawBiaxialGraphics();
                    }
                     DrawNeutralAxisLoc();
                    RunorPref = "Preference";
                    break;
                default: break;
            }
        }
        protected void SectionGraphics()
        {

             SecGraphics.FillColor(0, 0, SecCanvWidth, SecCanvHeight, SecBackColor);
            DiagramGraphics.FillColor(0, 0, DiagramCanvWidth, DiagramCanvHeight, DsgnBackColor);
            SetRecSecGraphicLayout(SecCanvWidth, SecCanvHeight, 0, sectionChart);
            sectionChart.XTick = 0.2f * SecCanvWidth;
            sectionChart.YTick = 0.2f * SecCanvWidth;
             sectionChart.DrawGrid(SecGraphics);
            sectionChart.DrawTickMark(SecGraphics);
             DrawRecSection(SecGraphics);
            DrawRebar(SecGraphics, sectionChart);
           DrawAxis(SecGraphics, sectionChart);
        }
        protected void DrawRecSection(DovDrawings graphic)
        {
            graphic.DrwColor = "white";
            graphic.FillColorOpt = "#15179c";
            PointF pt1 = new PointF(-secRec.Width * 0.5f, -secRec.Height * 0.5f);
            graphic.DrawRecSection(pt1, secRec.Width, secRec.Height, sectionChart);
        }
        protected void SetRecSecGraphicLayout(float canvaWidth, float canvaHeight, float horiStartPt, ChartStyle graphic)
        {
            float offset = canvaWidth * 0.2f;
            RectangleF plot = new RectangleF(0.5f * offset + horiStartPt, 0.5f * offset, canvaWidth - offset, canvaHeight - offset);
            graphic.MarginTop = 0.5f * offset;
            graphic.MarginBottom = 0.5f * offset;
            graphic.MarginLef = 0.5f * offset + horiStartPt;
            graphic.MarginRight = 0.5f * offset;
            graphic.PlotArea = plot;
            if (secRec.Width >= secRec.Height)
            {
                graphic.XMin = -secRec.Width * 0.5f;
                graphic.XMax = secRec.Width * 0.5f;
                graphic.YMin = -secRec.Width * 0.5f;
                graphic.YMax = secRec.Width * 0.5f;
            }
            else
            {
                graphic.XMin = -secRec.Height * 0.5f;
                graphic.XMax = secRec.Height * 0.5f;
                graphic.YMin = -secRec.Height * 0.5f;
                graphic.YMax = secRec.Height * 0.5f;
            }
        }
        protected void SetGraphicLayout(float canvaWidth, float canvaHeight, float horiStartPt, ChartStyle graphic, params float[] limits)
        {
            float offset = canvaWidth * 0.3f;
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
        protected void DrawRebar(DovDrawings graphic, ChartStyle cs)
        {
            float x;
            float y;
            float radius;
            float limitXNeg = -secRec.Width * 0.5f;
            float limitXPos = secRec.Width * 0.5f;
            float limitYNeg = -secRec.Height * 0.5f;
            float limitYPos = secRec.Height * 0.5f;
            PointF loc;
            rebarLocLibrary = rebarLocLibrary.OrderBy(reb => reb.LocY).ToList();
            DovRebarLocation lo = new DovRebarLocation();
            lo = rebarLocLibrary.First();
            rebarLowestLocation = lo.LocY;
            float locY = 0;
            bool start = true;
            RebarLocSorted.Clear();
            foreach (DovRebarLocation bars in rebarLocLibrary)
            {
                if (start)
                {
                    locY = bars.LocY;
                    start = false;
                }
                if (locY == bars.LocY)
                {
                    if (!RebarLocSorted.ContainsKey(locY))
                    {
                        Stack<DovRebarLocation> bar = new Stack<DovRebarLocation>();
                        bar.Push(bars);
                        RebarLocSorted[locY] = bar;
                    }
                    else
                    {
                        RebarLocSorted[locY].Push(bars);
                    }

                }
                else
                {
                    locY = bars.LocY;
                    Stack<DovRebarLocation> bar = new Stack<DovRebarLocation>();
                    bar.Push(bars);
                    RebarLocSorted[locY] = bar;
                }
                x = bars.LocX;
                y = bars.LocY;
                if ((x < limitXNeg || x > limitXPos) || (y < limitYNeg || y > limitYPos))
                {
                    RebarLocationMessage = "Error: Location is not within the boundary of the section.";
                    RebarLocMsgColor = "red";
                    continueDesign = false;
                    break;
                }
                else
                {
                    graphic.DrwColor = "white";
                    graphic.LineThickness = 2;
                    graphic.FillColorOpt = "red";
                    RebarLocMsgColor = "";
                    RebarLocationMessage = "Enter Location of rebars";
                    radius = bars.Diameter * 0.5f;
                    radius = cs.ScaleX(radius);
                    loc = cs.Point2D(new PointF(x, y));
                     graphic.DrawArc(loc.X, loc.Y, radius, 0, 2 * Math.PI);
                    continueDesign = true;
                }

                foreach (KeyValuePair<float, Stack<DovRebarLocation>> rebar in RebarLocSorted)
                {
                    re = rebar.Value.ToArray();
                    for (int i = 0; i < re.Length - 1; i++)
                    {
                        float xl = re[i].LocX;
                        for (int j = i+1; j < re.Length; j++)
                        {
                            if (xl == re[j].LocX)
                            {
                                RebarLocationMessage = "Warning:There are dublicate of bars one of them will be used.";
                                RebarLocMsgColor = "red";
                            }
                        }
                    }
                }
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
        //Draw 2D Grids and Tickmarks
        protected void DrawDiagramGraphics()
        {
            float[] resultantMoment = new float[] { };
            float[] axialForce = new float[] { };
            List<float[]> data = ColumnInter.BiaxialTableatAngle;
            List<float> bendingx = data[0].ToList();
            List<float> bendingy = data[1].ToList();
            List<float> axial = data[2].ToList();
            resultantMoment = new float[bendingx.Count];
            axialForce = new float[bendingx.Count];
            for (int i = 0; i < bendingx.Count; i++)
            {
                resultantMoment[i] = (float)Math.Sqrt((bendingx[i] * bendingx[i]) + (bendingy[i] * bendingy[i]));
            }
            axialForce = axial.ToArray();
            List<float> sort = axialForce.ToList();
            sort = sort.OrderBy(o => o).ToList();
            float minForce = sort.First();
            float maxForce = sort.Last();
            sort = resultantMoment.ToList();
            sort = sort.OrderBy(o => o).ToList();
            float minBending = sort.First();
            float maxBending = sort.Last();
            sort.Clear();
            float[] scaleLimits = new float[] { minBending, maxBending, minForce, maxForce };
            DiagramGraphics.DrwColor = "black";
            DiagramGraphics.FillColorOpt = "black";
            PointF pt1 = new PointF(0, 0);
            PointF pt2 = new PointF(DiagramCanvWidth, DiagramCanvHeight);
             DiagramGraphics.FillColor(pt1, pt2);
            //Why is this not working 
            ///
            ///await DiagramGraphics.ClearAsync(0, 0, DiagramCanvWidth, DiagramCanvHeight);
            ///
            SetGraphicLayout(DiagramCanvWidth, DiagramCanvHeight, 0, diagramChart, scaleLimits);
            diagramChart.XTick = (maxBending - minBending) / 10;
            diagramChart.YTick = (maxForce - minForce) / 10;
             diagramChart.DrawGrid(DiagramGraphics);
            SetGraphicLayout(DiagramCanvWidth, DiagramCanvHeight, 0, diagramChart, scaleLimits);
            diagramChart.XTick = (maxBending - minBending) / 10;
            diagramChart.YTick = (maxForce - minForce) / 10;
             diagramChart.DrawTickMark(DiagramGraphics);
          
        }
        protected void DrawAxis(DovDrawings graphic, ChartStyle cs)
        {
            float axisH = 0.5f * secRec.Height * 0.8f;
            float axisV = 0.5f * secRec.Width * 0.8f;
            graphic.DrwColor = "white";
            graphic.TextBase = TextBaseline.Bottom;
            PointF origin = cs.Point2D(new PointF(0, 0));
            PointF xEnd = cs.Point2D(new PointF(axisH, 0));
            PointF yEnd = cs.Point2D(new PointF(0, axisV));
            graphic.DrawLine(origin, xEnd, true);
             graphic.DrawLine(origin, yEnd, true, true);
             graphic.DrawText("X", xEnd);
             graphic.DrawText("Y", yEnd);
        }
        public void CalcUniaxialInteraction()
        {
            ColumnInter = new DovColumnDesign(concMat, rebarMat, secRec, DsgnPref);
            ColumnInter.lowestBar = rebarLowestLocation;
            ColumnInter.DsgnColumnBiaxialInter(RebarLocSorted, ColumnNeutralAxisAngle);
        }
        public void CalcBiaxialInteraction()
        {
            ColumnInter = new DovColumnDesign(concMat, rebarMat, secRec, DsgnPref);
            ColumnInter.DsgnColumnBiaxialInter(RebarLocSorted, ColumnNeutralAxisAngle);
            ColumnInter.DsgnColumnBiaxialInter(RebarLocSorted);
        }
        public void DrawUniInterChart()
        {
            float[] resultantMoment = new float[] { };
            float[] axialForce = new float[] { };
            List<float[]> data = ColumnInter.BiaxialTableatAngle;
            List<float> bendingx = data[0].ToList();
            List<float> bendingy = data[1].ToList();
            List<float> axial = data[2].ToList();
            resultantMoment = new float[bendingx.Count];
            axialForce = new float[bendingx.Count];
            for (int i = 0; i < bendingx.Count; i++)
            {
                resultantMoment[i] = (float)Math.Sqrt((bendingx[i] * bendingx[i]) + (bendingy[i] * bendingy[i]));
            }
            axialForce = axial.ToArray();
            PointF[] diagramPoints = new PointF[resultantMoment.Length];
            for (int i = 0; i < resultantMoment.Length; i++)
            {
                diagramPoints[i] = diagramChart.Point2D(new PointF(resultantMoment[i], axialForce[i]));
            }
            DiagramGraphics.DrwColor = "white";
            DiagramGraphics.LineThickness = 2;
             DiagramGraphics.DrawPolyLine(diagramPoints);
        }
        //#region
        //public async Task BiaxialGraphicsAsync()
        //{
        //    DiagramGraphics = new DovDrawings(diagramContext);
        //    DiagramGraphics.DrwColor = "white";
        //    DiagramGraphics.LineThickness = 2;
        //    diagramChart.ChartArea = new RectangleF(0, 0, DiagramCanvWidth, DiagramCanvHeight);
        //    bool start = true;
        //    float axialMax = 5;
        //    float axialMin = -5;
        //    float bendxMax = 6;
        //    float bendxMin = -6;
        //    float bendyMax = 4;
        //    float bendyMin = -4;
        //    foreach (KeyValuePair<float, List<float[]>> chartPoint in ColumnInter.BiaxialChartTable)
        //    {
        //        List<float[]> data = chartPoint.Value;
        //        List<float> bendingx = data[0].ToList();
        //        bendingx = bendingx.OrderBy(o => o).ToList();
        //        List<float> bendingy = data[1].ToList();
        //        bendingy = bendingy.OrderBy(o => o).ToList();
        //        List<float> axial = data[2].ToList();
        //        axial = axial.OrderBy(o => o).ToList();
        //        if (start)
        //        {
        //            axialMin = axial.First();
        //            axialMax = axial.Last();
        //            bendxMin = bendingx.First();
        //            bendxMax = bendingx.Last();
        //            bendyMin = bendingy.First();
        //            bendyMax = bendingy.Last();
        //            start = false;
        //        }
        //        else
        //        {
        //            if (axialMin > axial.First()) axialMin = axial.First();
        //            if (axialMax < axial.Last()) axialMax = axial.Last();
        //            if (bendxMin > bendingx.First()) bendxMin = bendingx.First();
        //            if (bendxMax < bendingx.Last()) bendxMax = bendingx.Last();
        //            if (bendyMin > bendingy.First()) bendyMin = bendingy.First();
        //            if (bendyMax < bendingy.Last()) bendyMax = bendingy.Last();

        //        }

        //    }
        //    diagramChart.XTick = (bendxMax - bendxMin) / 10;
        //    diagramChart.YTick = (bendyMax - bendyMin) / 10;
        //    diagramChart.ZTick = (axialMax - axialMin) / 10;
        //    float[] scaleLimits = new float[] { bendxMin, bendxMax, bendyMin, bendyMax, axialMin, axialMax };
        //    diagramChart.Azimuth = -125;
        //    diagramChart.Elevation = 35;
        //    ///
        //    //DiagramGraphics.DrwColor = "black";
        //    //PointF pt1 = new PointF(0, 0);
        //    //PointF pt2 = new PointF(DiagramCanvWidth, DiagramCanvHeight);
        //    //await DiagramGraphics.DrawRecAsync(pt1, pt2);
        //    ///
        //    SetGraphicLayout(DiagramCanvWidth, DiagramCanvHeight, 0, diagramChart, scaleLimits);
        //    await diagramChart.AddChartStyle(DiagramGraphics);
        //    DiagramGraphics.DrwColor = "red";
        //    diagramChart.ChartArea = new RectangleF(0, 0, DiagramCanvWidth, DiagramCanvHeight);
        //    SetGraphicLayout(DiagramCanvWidth, DiagramCanvHeight, 0, diagramChart, scaleLimits);
        //    diagramChart.Azimuth = -125;
        //    diagramChart.Elevation = 35;
        //    Matrix3 m = new Matrix3();
        //    m = Matrix3.AzimuthElevation(diagramChart.Elevation, diagramChart.Azimuth);
        //    int n = 0;
        //    foreach (KeyValuePair<float, List<float[]>> chartPoint in ColumnInter.BiaxialChartTable)
        //    {
        //        List<float[]> data = chartPoint.Value;
        //        List<float> bendingx = data[0].ToList();
        //        List<float> bendingy = data[1].ToList();
        //        List<float> axial = data[2].ToList();
        //        Point3[] pts = new Point3[bendingx.Count];
        //        for (int i = 0; i < bendingx.Count; i++)
        //        {
        //            pts[i] = new Point3(bendingx[i], bendingy[i], axial[i], 1);
        //            pts[i].Transform(m, diagramChart);
        //        }

        //        for (int j = 0; j < pts.Length - 1; j++)
        //        {
        //            await DiagramGraphics.DrawLineAsync(pts[j].X, pts[j].Y, pts[j + 1].X, pts[j + 1].Y);
        //        }
        //        n++;
        //    }
        //}
        //#endregion
        public void DrawBiaxialGraphics()
        {
            BiaxialGraphics = new DovDrawings(biaxialContext);
            BiaxialGraphics.DrwColor = "white";
            BiaxialGraphics.LineThickness = 3f;
            biaxialChart.ChartArea = new RectangleF(0, 0, BiaxialCanvWidth, BiaxialCanvHeight);
            bool start = true;
            float axialMax = 5;
            float axialMin = -5;
            float bendxMax = 6;
            float bendxMin = -6;
            float bendyMax = 4;
            float bendyMin = -4;
            foreach (KeyValuePair<float, List<float[]>> chartPoint in ColumnInter.BiaxialChartTable)
            {
                List<float[]> data = chartPoint.Value;
                List<float> bendingx = data[0].ToList();
                bendingx = bendingx.OrderBy(o => o).ToList();
                List<float> bendingy = data[1].ToList();
                bendingy = bendingy.OrderBy(o => o).ToList();
                List<float> axial = data[2].ToList();
                axial = axial.OrderBy(o => o).ToList();
                if (start)
                {
                    axialMin = axial.First();
                    axialMax = axial.Last();
                    bendxMin = bendingx.First();
                    bendxMax = bendingx.Last();
                    bendyMin = bendingy.First();
                    bendyMax = bendingy.Last();
                    start = false;
                }
                else
                {
                    if (axialMin > axial.First()) axialMin = axial.First();
                    if (axialMax < axial.Last()) axialMax = axial.Last();
                    if (bendxMin > bendingx.First()) bendxMin = bendingx.First();
                    if (bendxMax < bendingx.Last()) bendxMax = bendingx.Last();
                    if (bendyMin > bendingy.First()) bendyMin = bendingy.First();
                    if (bendyMax < bendingy.Last()) bendyMax = bendingy.Last();

                }

            }
            biaxialChart.XTick = (bendxMax - bendxMin) / 10;
            biaxialChart.YTick = (bendyMax - bendyMin) / 10;
            biaxialChart.ZTick = (axialMax - axialMin) / 10;
            float[] scaleLimits = new float[] { bendxMin, bendxMax, bendyMin, bendyMax, axialMin, axialMax };
            biaxialChart.Azimuth = -125;
            biaxialChart.Elevation = 35;
            ///
            SetGraphicLayout(BiaxialCanvWidth, BiaxialCanvHeight, 0, biaxialChart, scaleLimits);
             biaxialChart.AddChartStyle(BiaxialGraphics);
            BiaxialGraphics.DrwColor = "red";
            BiaxialGraphics.LineThickness = 3f;
            biaxialChart.ChartArea = new RectangleF(0, 0, BiaxialCanvWidth, BiaxialCanvHeight);
            SetGraphicLayout(BiaxialCanvWidth, BiaxialCanvHeight, 0, biaxialChart, scaleLimits);
            biaxialChart.Azimuth = -125;
            biaxialChart.Elevation = 35;
            Matrix3 m = new Matrix3();
            m = Matrix3.AzimuthElevation(biaxialChart.Elevation, biaxialChart.Azimuth);
            foreach (KeyValuePair<float, List<float[]>> chartPoint in ColumnInter.BiaxialChartTable)
            {
                List<float[]> data = chartPoint.Value;
                List<float> bendingx = data[0].ToList();
                List<float> bendingy = data[1].ToList();
                List<float> axial = data[2].ToList();
                Point3[] pts = new Point3[bendingx.Count];
                for (int i = 0; i < bendingx.Count; i++)
                {
                    pts[i] = new Point3(bendingx[i], bendingy[i], axial[i], 1);
                    pts[i].Transform(m, biaxialChart);
                }
                for (int j = 0; j < pts.Length - 1; j++)
                {
                    BiaxialGraphics.DrawLine(pts[j].X, pts[j].Y, pts[j + 1].X, pts[j + 1].Y);
                }
            }
        }
        public void DrawNeutralAxisLoc()
        {
            NeutralAxisGraphics.DrwColor = "white";
            NeutralAxisGraphics.LineThickness = 2;
            neutralAxisChart.ChartArea = new RectangleF(0, 0, ClmnNeutralAxisWidth, ClmnNeutralAxisHeight);
            float[] scaleLimits = new float[] { -0.5f *secRec.Width,0.5f*secRec.Width,
                -0.5f*secRec.Height, 0.5f*secRec.Height };
            SetGraphicLayout(ClmnNeutralAxisWidth, ClmnNeutralAxisHeight, 0, neutralAxisChart, scaleLimits);
            float x1, x2, x3, x4, y1, y2, y3, y4;
            float angleRad = ((float)Math.PI * ColumnNeutralAxisAngle) / 180;
            float x, y;
            float x0, y0;
            float limitAngle = (float)Math.Atan(secRec.Height / secRec.Width);
            x0 = 0;
            y0 = 0;
            x = 0.5f * secRec.Width;
            y = 0.5f * secRec.Height;
            y1 = (float)Math.Tan(angleRad) * (-x - x0) + y0;
            x1 = -x;
            x2 = x;
            y2 = (float)Math.Tan(angleRad) * (x - x0) + y0;
            x3 = x;
            y3 = y;
            x4 = -x;
            y4 = y;
            if (angleRad > limitAngle)
            {
                x0 = 0;
                y0 = 0;
                x = 0.5f * secRec.Width;
                y = 0.5f * secRec.Height;
                y1 = -y;
                x1 = ((y1 - y0) / (float)Math.Tan(angleRad)) + x0;

                y2 = y;
                x2 = ((y2 - y0) / (float)Math.Tan(angleRad)) + x0;
                x3 = -x;
                y3 = y;
                x4 = -x;
                y4 = -y;
            }
            PointF pt1 = neutralAxisChart.Point2D(new PointF(x1, y1));
            PointF pt2 = neutralAxisChart.Point2D(new PointF(x2, y2));
            PointF[] pts = new PointF[] {neutralAxisChart.Point2D(new PointF(x1,y1)),
            neutralAxisChart.Point2D(new PointF(x2,y2)),
            neutralAxisChart.Point2D(new PointF(x3,y3)),
            neutralAxisChart.Point2D(new PointF(x4,y4)) };
            NeutralAxisGraphics.DrwColor = "black";
            NeutralAxisGraphics.FillColorOpt = "red";

            NeutralAxisGraphics.FillColor(neutralAxisChart.Point2D(new PointF(x, y)), neutralAxisChart.Point2D(new PointF(-x, -y)));
            NeutralAxisGraphics.DrawLine(neutralAxisChart.Point2D(new PointF(x1, y1)), neutralAxisChart.Point2D(new PointF(x2, y2)));
            NeutralAxisGraphics.FillColorOpt = "yellow";
             NeutralAxisGraphics.FillPolygone(pts);
             NeutralAxisGraphics.DrawLine(neutralAxisChart.Point2D(new PointF(0, 0)), neutralAxisChart.Point2D(new PointF(x, 0)));
            PointF start = neutralAxisChart.Point2D(new PointF(0, 0));
            float r = 30f;
            float adj = (float)Math.Round(pt2.X - pt1.X, 3);// this one has to be revised;
            float opp = pt2.Y - pt1.Y;
            float scaledAngle = 90 * (float)Math.PI / 180;
            // this is a quation
            test1 = (float)Math.Tan(scaledAngle);
            if (adj != 0)
            {
                scaledAngle = (float)Math.Atan(opp / adj);
                if (scaledAngle != 0)
                {
                    PointF orig = neutralAxisChart.Point2D(new PointF(0, 0));
                    float xa = (r * (float)Math.Cos(scaledAngle));
                    float xr = orig.X + xa;
                    float yr = orig.Y + (r * (float)Math.Sin(scaledAngle));
                    float incr = (r - xa) / 6;

                    float xc = xr;
                    float yc = yr;
                    for (float i = xr + incr; i <= orig.X + r; i += incr)
                    {
                        float rsq = r * r;
                        float xsq = (i - orig.X) * (i - orig.X);
                        float xnext = i;
                        float ynext = -(float)Math.Sqrt(rsq - xsq) + orig.Y;
                         NeutralAxisGraphics.DrawLine(xc, yc, xnext, ynext);
                        xc = xnext;
                        yc = ynext;
                    }
                }
            }
        }
    }
}

