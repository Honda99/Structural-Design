using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using EngDolphin.Canvas;
using EngDolphin.Client.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static EngDolphin.Canvas.CanvasEnum;
using Microsoft.AspNetCore.Components.Web;

namespace EngDolphin.Pages
{
    public class BeamComponent : ComponentBase
    {
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        protected ElementReference SectionCanvas { get; set; }
        protected ElementReference DesignCanvas { get; set; }
        //Total rebar information
        protected DovRebarLocation[] re=new DovRebarLocation[] { };
        protected bool doveShowMaterial = false;
        protected bool doveShowSection = false;
        protected bool doveShowDesign = false;
        protected bool continueDesign = false;
        protected string displayConcMat = "dovOff";
        protected string displayRebarMat = "dovOff";
        //Table Editing
        protected int rebarActiveRow = 0;
        protected int rebarMaxRow = 2;
        protected bool dispDsgnPref = false;
        protected bool dispDsgnRslt = false;
        protected string RebarLocationMessage = "Enter Location of rebars";
        protected string RebarLocMsgColor = "";
        protected int testInt;
        protected int testString;
        protected float testFloat;
        // Drawings 
        protected string SecBackColor { get; set; } = "black";
        protected string DsgnBackColor { get; set; } = "black";
        public long SecCanvWidth { get; set; } = 300;
        public long SecCanvHeight { get; set; } = 300;
        public long DsgnCanvWidth { get; set; } = 600;
        public long DsgnCanvHeight { get; set; } = 300;
        // Total Created Materials
        protected List<DovConcMat> dovConcMatLibrary = new List<DovConcMat>();
        protected List<DovRebarMat> dovRebarMatLibrary = new List<DovRebarMat>();
        protected DovConcMat concMat = new DovConcMat();
        protected DovRebarMat rebarMat = new DovRebarMat();
        protected DovSectionRec secRec = new DovSectionRec(200, 400);
        protected List<DovRebarLocation> rebarLocLibrary = new List<DovRebarLocation>();
        protected DovDsgnPreference DsgnPref = new DovDsgnPreference();
        protected DovBeamDesign BeamDsgnRslt = new DovBeamDesign();
        protected IDictionary<float, Stack<DovRebarLocation>> RebarLocSorted = new Dictionary<float, Stack<DovRebarLocation>>();
        //Graphics
        public DovDrawings SecGraphics { get; set; }
        public DovDrawings DsgnGraphics { get; set; }
        private CanvasContext designContext;
        private CanvasContext sectionContext;
        private ChartStyle sectionChart;
        private ChartStyle designChart;
        protected override void OnInitialized ()
        {
             concMat = new DovConcMat() { Name = "25MPa", UnitWt = 25, Fck = 25, PoissonRatio = 0.2f,E=29000 };
            dovConcMatLibrary.Add(concMat);
            concMat = new DovConcMat() { Name = "15MPa", UnitWt = 25, Fck = 15, PoissonRatio = 0.2f,E=29000 };
            dovConcMatLibrary.Add(concMat);

           rebarMat = new DovRebarMat() { Name = "355MPa", UnitWt = 78.5f, Fyk = 355, PoissonRatio = 0.3f,E=200000 };
            dovRebarMatLibrary.Add(rebarMat);
            rebarMat = new DovRebarMat() { Name = "400MPa", UnitWt = 78.5f, Fyk = 400, PoissonRatio = 0.3f,E=200000  };
            dovRebarMatLibrary.Add(rebarMat);
            DovRebarLocation rLoc = new DovRebarLocation(1, 16, -67, -167);
            rebarLocLibrary.Add(rLoc);
            rLoc = new DovRebarLocation(2, 16, 67, -167);
            rebarLocLibrary.Add(rLoc);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            this.designContext = new CanvasContext(DesignCanvas, JsRuntime);
            DsgnGraphics = new DovDrawings(designContext);
            //
            this.sectionContext = new CanvasContext(SectionCanvas, JsRuntime);
            SecGraphics = new DovDrawings(sectionContext);
            sectionChart = new ChartStyle();
            designChart = new ChartStyle();
        }
       
        protected void ShowConcMatProp(ChangeEventArgs e)
        {
            foreach(var mat in dovConcMatLibrary)
            {
                if (mat.Name == e.Value.ToString())
                {
                    concMat = new DovConcMat() { Name=mat.Name,UnitWt=mat.UnitWt,Fck=mat.Fck,PoissonRatio=mat.PoissonRatio,E=mat.E,St=mat.St};
                }
            }
            displayConcMat = "dovShow";
        }

        protected void ShowRebarMatProp(ChangeEventArgs e)
        {
            foreach(var rebar in dovRebarMatLibrary)
            {
                if (rebar.Name == e.Value.ToString())
                {
                    rebarMat = new DovRebarMat() {Name=rebar.Name,UnitWt=rebar.UnitWt,Fyk=rebar.Fyk,PoissonRatio=rebar.PoissonRatio,E=rebar.E,St=rebar.St };
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
                    concMat = new DovConcMat() { Name = mat.Name,UnitWt=mat.UnitWt,Fck=mat.Fck,PoissonRatio=mat.PoissonRatio,St=mat.St,E=mat.E };
                }
            }
        }
        protected void SelectedRebarMat(ChangeEventArgs e)
        {
            foreach (var rebar in dovRebarMatLibrary)
            {
                if (rebar.Name == e.Value.ToString())
                {
                    rebarMat = new DovRebarMat(rebar.Name, rebar.UnitWt, rebar.Fyk, rebar.PoissonRatio, rebar.E) {Name=rebar.Name,UnitWt=rebar.UnitWt,Fyk=rebar.Fyk,PoissonRatio=rebar.PoissonRatio,St=rebar.St };
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
            DovRebarLocation reb = new DovRebarLocation(rebarMaxRow, 12, 0, 0);
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
        protected void DisplayDsgnOpt(ChangeEventArgs e)
        {
            switch (e.Value.ToString())
            {
                case "pref":
                    dispDsgnPref = true;
                    dispDsgnRslt = false; break;
                case "run":
                    dispDsgnPref = false;
                    dispDsgnRslt = true;
                    DesignBeam();
                    BmDsgnGraphics();
                    break;
                default: break;
            }
        }
        protected void SectionGraphics()
        {
             SecGraphics.FillColor(0, 0, SecCanvWidth, SecCanvHeight, SecBackColor);
             DsgnGraphics.FillColor(0, 0, DsgnCanvWidth, DsgnCanvHeight, DsgnBackColor);
            SetRecSecGraphicLayout(SecCanvWidth, SecCanvHeight, 0, sectionChart);
            sectionChart.XTick = 0.2f * SecCanvWidth;
            sectionChart.YTick = 0.2f * SecCanvWidth;
             sectionChart.DrawGrid(SecGraphics);
             sectionChart.DrawTickMark(SecGraphics);
            SetRecSecGraphicLayout(SecCanvWidth, SecCanvHeight, 0, sectionChart);
            DrawRecSection(SecGraphics,sectionChart);
            DrawRebar(SecGraphics,sectionChart);
             DrawAxis(SecGraphics,sectionChart);
        }

        protected void DrawRecSection(DovDrawings graphic,ChartStyle cs)
        {
            graphic.DrwColor = "white";
            graphic.FillColorOpt = "#15179c";
            PointF pt1 = new PointF(-secRec.Width * 0.5f, -secRec.Height * 0.5f);
            graphic.DrawRecSection(pt1,secRec.Width, secRec.Height,cs);
        }
        protected void SetRecSecGraphicLayout( float canvaWidth,float canvaHeight,float horiStartPt, ChartStyle graphic )
        {
            float offset = canvaWidth * 0.2f;
            RectangleF plot = new RectangleF(0,0, canvaWidth - offset, canvaHeight - offset);
            graphic.MarginTop = 0.5f * offset;
            graphic.MarginBottom = 0.5f * offset;
            graphic.MarginLef = 0.5f * offset+horiStartPt;
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
        protected void SetGraphicLayout(float canvaWidth, float canvaHeight,float [] limits, float horiStartPt, ChartStyle graphic)
        {
                float offset = canvaWidth * 0.2f;
                RectangleF plot = new RectangleF(0.5f * offset + horiStartPt, 0.5f * offset, canvaWidth - offset, canvaHeight - offset);
                graphic.PlotArea = plot;
                graphic.XMin =limits[0];
                graphic.XMax = limits[1];
                graphic.YMin = limits[2];
                graphic.YMax = limits[3];
        }
        protected void DrawRebar(DovDrawings graphic,ChartStyle cs)
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
                    RebarLocationMessage = "Error: location is not within the boundary of the section";
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
                    for (int i = 0; i < re.Length-1; i++)
                    {
                        float xl = re[i].LocX;
                        for (int j = i+1; j < re.Length; j++)
                        {
                            if (xl == re[j].LocX)
                            {
                                RebarLocationMessage = "Warning: There are dublicate of bars, one of them will be used.";
                                RebarLocMsgColor = "red";
                            }
                        }
                    }
                }
            }
        }

        protected void DrawAxis( DovDrawings graphic,ChartStyle cs)
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
            graphic.DrawText("X",xEnd);
             graphic.DrawText( "Y",yEnd);
        }
    
        public void DesignBeam()
        {
            BeamDsgnRslt.ConcMat = concMat;
            BeamDsgnRslt.RebarMat = rebarMat;
            BeamDsgnRslt.DsgnPref = DsgnPref;
            BeamDsgnRslt.RecSec = secRec;
            BeamDsgnRslt.DsgnBeamRecSec(RebarLocSorted);
        }
    /// Rectangular Beam Design Graphics
    /// 

        protected void BmDsgnGraphics()
        {
            float secWidth = 0.4f * DsgnCanvWidth;
            float strainWidth = 0.3f * DsgnCanvWidth;
            float stressWidth = 0.3f * DsgnCanvWidth;
            float x=0;
            SetRecSecGraphicLayout(secWidth, DsgnCanvHeight, x, designChart);
            DrawRecSection(DsgnGraphics, designChart);
            SetRecSecGraphicLayout(secWidth, DsgnCanvHeight, x, designChart);
            DrawRebar(DsgnGraphics,designChart);
            //Strain
            x = 0.4f * DsgnCanvWidth;
            SetRecSecGraphicLayout(strainWidth, DsgnCanvHeight, x,designChart);
            DrawRecSecStrain();
            //Stress
            x = 0.7f * DsgnCanvWidth;
            SetRecSecGraphicLayout(stressWidth, DsgnCanvHeight, x, designChart);
            DrawRecSecStress();
        }
         protected void DrawRecSecStrain()
        {
            float dsgnStrW = 0.3f * DsgnCanvWidth;
            
            float tensStr =(float)Math.Round( BeamDsgnRslt.TensStrainY,6);
            float compStr = (float)Math.Round(BeamDsgnRslt.CompStrain,6);
            float h = secRec.Height;
            float c = BeamDsgnRslt.NeutralAxisY;
            float d = BeamDsgnRslt.TensEffDepth;
            float dComp = BeamDsgnRslt.CompEffDepth;
            float txtWidth = 30.56f;
            float txtOffset = 5;
            float txtHeight = 10;
            float cb = h - c;
            designChart.XMin = 0;
            designChart.XMax = BeamDsgnRslt.CompStrain + BeamDsgnRslt.TensStrainY+(txtWidth* (compStr + tensStr) / dsgnStrW);
            designChart.YMin = 0;
            designChart.YMax = h+txtHeight;
            DsgnGraphics.DrwColor = "red";
            DsgnGraphics.LineThickness = 2;
           
            PointF pt0 = designChart.Point2D(new PointF(0, h-d));
            PointF pt1 = designChart.Point2D(new PointF(tensStr, h-d));
            PointF pt2 = designChart.Point2D(new PointF(tensStr, h));
            PointF pt3 = designChart.Point2D(new PointF(tensStr + compStr, h));
            PointF pt4 = designChart.Point2D(new PointF(tensStr, cb));
            PointF pt5 = designChart.Point2D(new PointF(0, h-d));
            PointF pt6 = designChart.Point2D(new PointF(tensStr, 0));
            DsgnGraphics.DrawPolyLine(new[] { pt0, pt1, pt2, pt3, pt4, pt5 });
            DsgnGraphics.DrawLine(pt1, pt6);
            if (dComp > 0)
            {
                float stcomp = (compStr * (c - dComp)) / c;
                PointF pt7 = designChart.Point2D(new PointF(tensStr, h - dComp));
                PointF pt8 = designChart.Point2D(new PointF(tensStr+stcomp, h - dComp));
                DsgnGraphics.DrawLine(pt7, pt8);
            }
            //Draw texts
           
            string font = "pt Verenda";
            font = txtHeight + font;
            string txtTensStr = Math.Round(tensStr, 6).ToString();
            string txtCompStr = Math.Round(compStr, 5).ToString();
            DsgnGraphics.TextFont = font;
            DsgnGraphics.TextBase = TextBaseline.Bottom;
            DsgnGraphics.TextAlignment = TextAlign.Right;
            PointF txtpt = designChart.Point2D(new PointF(tensStr+compStr,h+txtOffset));
             DsgnGraphics.DrawText(compStr.ToString(), txtpt);
             txtpt = designChart.Point2D(new PointF(tensStr , h-d - txtOffset));
            DsgnGraphics.TextBase = TextBaseline.Top;
            DsgnGraphics.DrawText(tensStr.ToString(), txtpt);
            DimensionLine dim = new DimensionLine();
            ///
            DsgnGraphics.TextBase = TextBaseline.Top;
            DsgnGraphics.TextAlignment = TextAlign.Center;
            DsgnGraphics.LineThickness = 1;
            DsgnGraphics.DrwColor = "hotpink";
            dim.Graphic = DsgnGraphics;
            dim.DimDir = DimensionLine.DimensionDirection.left;
             dim.DrawDimLineAligned(pt1, pt4,Math.Round(cb,1).ToString() );
            DsgnGraphics.TextBase = TextBaseline.Bottom;
            dim.DimDir = DimensionLine.DimensionDirection.right;
             dim.DrawDimLineAligned(pt4, pt2,Math.Round( c,1).ToString());
             dim.DrawOppoDimLine(pt0, pt2,Math.Round( d,1).ToString());
        }
        protected void DrawRecSecStress()
        {
            float compStress =(float)Math.Round( BeamDsgnRslt.CompStress,3);
            float concForce =(float)Math.Round( BeamDsgnRslt.ConcForce,3);
            float rebarCompForce =(float)Math.Round( BeamDsgnRslt.RebarCompForce,3);
            float rebarTensForce =(float)Math.Round( BeamDsgnRslt.RebarTesnsileForce,3);
            float txtOffset = 5;
            float txtHeight = 10;
            string font = "pt Verenda";
            font = txtHeight + font;
            Symbols arrow = new Symbols();
            IDictionary<string, float> maxValue = new Dictionary<string, float>();
            maxValue["compStress"] = compStress;
            maxValue["concForce"] = concForce;
            maxValue["rebarCompForce"] = rebarCompForce;
            maxValue["rebarTensForce"] = rebarTensForce;
            var order = from o in maxValue orderby o.Value descending select o;
            float maximumForce = order.First().Value;
            maxValue = order.ToDictionary(o => o.Key, o => o.Value);
            float prevValue=0;
            bool start = true;
            foreach(KeyValuePair<string,float>value in order)
            {
                if (start)
                {
                    prevValue = value.Value;
                    start = false;
                    continue;
                }
                maxValue[value.Key]= value.Value +Math.Abs( (prevValue -value.Value))*0.7f;
                prevValue = value.Value;
            }
            float lamda = 0.8f;
            float h = secRec.Height;
            float c =(float)Math.Round( BeamDsgnRslt.NeutralAxisY,3);
            float d =(float)Math.Round( BeamDsgnRslt.TensEffDepth,3);
            float dComp =(float)Math.Round( BeamDsgnRslt.CompEffDepth,3);
            designChart.XMin = 0;
            designChart.XMax = maximumForce;
            designChart.YMin = 0;
            designChart.YMax = secRec.Height+txtHeight+txtOffset;
            DsgnGraphics.DrwColor = "magenta";
            DsgnGraphics.LineThickness = 2;

            PointF pt0 = designChart.Point2D(new PointF(0, 0));
            PointF pt1 = designChart.Point2D(new PointF(0, h));
             DsgnGraphics.DrawLine(pt0, pt1);
            //Compression Stress
            PointF pt2 = designChart.Point2D(new PointF(maxValue["compStress"], h));
            DsgnGraphics.DrawLine(pt1, pt2);
            PointF pt3 = designChart.Point2D(new PointF(maxValue["compStress"], h-c*lamda));
            DsgnGraphics.DrawLine(pt2, pt3);
            PointF pt4 = designChart.Point2D(new PointF(0, h - c * lamda));
             DsgnGraphics.DrawLine(pt3, pt4);

            DsgnGraphics.FillColorOpt = "slategray";
           DsgnGraphics.FillColor(pt4, pt2);
            // Rebar compression Reinforcement
            if (dComp > 0)
            {
                PointF pt5 = designChart.Point2D(new PointF(0, h - dComp));
                PointF pt6 = designChart.Point2D(new PointF(maxValue["rebarCompForce"], h - dComp));
                DsgnGraphics.DrawLine(pt5, pt6);
                DsgnGraphics.DrwColor = "royalblue";
                DsgnGraphics.LineThickness = 1;
                arrow.Graphic = DsgnGraphics;
                arrow.DrawArrow(pt5, 5, 20, 180);
                //Rebar Comp Force
                DsgnGraphics.TextFont = font;
                DsgnGraphics.TextBase = TextBaseline.Bottom;
                DsgnGraphics.TextAlignment = TextAlign.Right;
                DsgnGraphics.DrwColor = "magenta";
                 DsgnGraphics.DrawText(string.Format("{0}kN",rebarCompForce), pt6);
            }
            //Draw Tensile Force
            PointF pt9= designChart.Point2D(new PointF(0, h - d));
            PointF  pt10 = designChart.Point2D(new PointF(maxValue["rebarTensForce"], h - d));
           DsgnGraphics.DrawLine(pt9, pt10);
            //Concrete Force
            PointF pt7 = designChart.Point2D(new PointF(0, h - c *0.5f* lamda));
            PointF pt8 = designChart.Point2D(new PointF(maxValue["concForce"], h - c * 0.5f * lamda));
             DsgnGraphics.DrawLine(pt7, pt8);
            DsgnGraphics.DrwColor = "royalblue";
            DsgnGraphics.LineThickness = 1;
            arrow.Graphic = DsgnGraphics;
            //Compression Stress
           arrow.DrawArrow(pt1, 5, 20, 180);
            arrow.DrawArrow(pt4, 5, 20, 180);
            //Rebar Compression Force
            //Rebar Tension Force
           arrow.DrawArrow(pt10, 5, 20, 0);
            //Concrete Force
           arrow.DrawArrow(pt7, 5, 20,180);
            // Draw Text 
            DsgnGraphics.TextFont = font;
            DsgnGraphics.TextBase = TextBaseline.Bottom;
            DsgnGraphics.TextAlignment = TextAlign.Right;
            DsgnGraphics.DrwColor = "yellow";
            DsgnGraphics.FillColorOpt = "yellow";
            //compression Stress
            DsgnGraphics.DrawText(string.Format("{0}kN/m",compStress),pt2);
          
            //Conc Comp Force
            DsgnGraphics.DrawText(string.Format("{0}kN",concForce),pt8);
          
            //Rebar Tensile Force
             DsgnGraphics.DrawText(string.Format("{0}kN",rebarTensForce), pt10);
            //Draw Dimension Line
            DimensionLine dim = new DimensionLine();
            ///
            DsgnGraphics.TextBase = TextBaseline.Bottom;
            DsgnGraphics.TextAlignment = TextAlign.Center;
            DsgnGraphics.LineThickness = 1;
            DsgnGraphics.DrwColor = "hotpink";
            dim.Graphic = DsgnGraphics;
            dim.DimDir = DimensionLine.DimensionDirection.right;
            
            dim.DrawDimLineAligned(pt0, pt1, h.ToString());
            float v =(float)Math.Round( d - c * lamda,1);
            dim.DimDir = DimensionLine.DimensionDirection.left;
            dim.DimLoc = DimensionLine.DimensionLocation.end;
            DsgnGraphics.TextBase = TextBaseline.Top;
            dim.DrawOppoDimLine(pt9, pt3, v.ToString());
        }
    }
}

