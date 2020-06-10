using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{

    public class IsoFootingDesign:UnitConv
    {
        private float aveStr=0;// kN/m2
        private float lCanti=0;
        private float rCanti=0;
        private float bCanti = 0;
        private float tCanti = 0;
        public string DesignCondition { get; set; } = "";
        public int noPointX { get; set; } = 10;
        public int noPointY { get; set; } = 10;
        public float t1 { get; set; }
        //
        public float VRdcWidebeam { get; set; }
        public float VRdcPunching { get; set; }
        public float VEdPunching { get; set; }
        public float VEdWideBeam { get; set; }
        public float Asx { get; set; }
        public float Asy { get; set; }
        //
        private FootingData fData { get; set; }
        public DovDsgnPreference DsgnPref { get; set; }
        public DovConcMat ConcMat { get; set; }
        private const float lamda = 0.8f;
        public DovRebarMat RebarMat { get; set; }
        private List<ArrayList> lbm;
        private List<ArrayList> rbm;
        private List<ArrayList> bbm;
        private List<ArrayList> tbm;
        //Shear force data
       private List<ArrayList> lsf ;
       private List<ArrayList> rsf;
       private List<ArrayList> bsf ;
       private List<ArrayList> tsf ;
        public IsoFootingDesign( FootingData fd,DovConcMat conc,DovRebarMat reb)
        {
            fData = fd;
            ConcMat = conc;
            RebarMat =reb;
        }
        public IsoFootingDesign() {
           
        }
        public void CheckBearingStress()
        {
            float w = fData.Width;
            float l = fData.Length;
            float qult = fData.UltBearingCap;
            float mx =fData.Mx;
            float my = fData.My;
            float pn = fData.Pn;
            float colx = fData.ColLocX;
            float coly = fData.ColLocY;
            float qa = qult / fData.SaftyFactor;
            float smx = fData.sMx;
            float smy = fData.sMy;
            float spn = fData.sPn;
            float st1, st2, st3, st4;
            float minst, maxst;
            float ix, iy, a,x,y;
            float ex, ey;
            float exc, eyc;
            exc = (colx - 0.5f * w)*centi;
            eyc = (coly - 0.5f * l)*centi;
            ex = exc + (smy / spn);
            ey = eyc + (smx / spn);
           
            a = w * l*tetra;
            ix = w * l * l * l*octa / 12;
            iy = l * w * w * w*octa / 12;
            x = 0.5f * w * centi;
            y = 0.5f * l * centi;
            st1 = (spn / a) + (spn*ey * y / ix) + (spn*ex * x / iy);
            st2= (spn / a) + (spn*ey * -y / ix) + (spn*ex * -x / iy);
            st3= (spn / a) + (spn * ey * y / ix) + (spn * ex * -x / iy);
            st4= (spn / a) + (spn * ey * -y / ix) + (spn * ex * x / iy);
            minst = Min(new[] { st1, st2, st3, st4 });
            maxst = Max(new[] { st1, st2, st3, st4 });
            if (minst < 0) {
                DesignCondition = "Warning: Minimum stress is negative," +
                       " change footing dimension or column location.";
            }
            else
            {
                DesignCondition = "";
            }
            if (maxst > qa)
            {
                DesignCondition = "Warning: Maximum stress is greater than soil bearing capcity," +
                      " change footing dimension or column location.";
            }
           
            //Design Stress
            ex = exc + (mx / pn);
            ey = eyc + (my / pn);
            st1 = (pn / a) + (pn * ex * y / ix) + (pn * ey * x / iy);
            st2 = (pn / a) + (pn * ex * -y / ix) + (pn * ey * -x / iy);
            st3 = (pn / a) + (pn * ex * -y / ix) + (pn * ey * x / iy);
            st4 = (pn / a) + (pn * ex * y / ix) + (pn * ey * -x / iy);
            minst = Min(new[] { st1, st2, st3, st4 });
            maxst = Max(new[] { st1, st2, st3, st4 });
             aveStr = (minst + maxst) * 0.5f;
             lCanti =colx - fData.ColumnWidth*0.5f;
             rCanti = w - colx - fData.ColumnWidth * 0.5f;
             bCanti = coly - fData.ColumnLength * 0.5f;
             tCanti = l - coly - fData.ColumnLength * 0.5f;
            //Bending moment data
             lbm = new List<ArrayList>();
             rbm = new List<ArrayList>();
             bbm = new List<ArrayList>();
            tbm = new List<ArrayList>();
            //Shear force data
             lsf = new List<ArrayList>();
             rsf = new List<ArrayList>();
             bsf = new List<ArrayList>();
             tsf = new List<ArrayList>();
            float incrx = w / noPointX;
            float incry = l / noPointY;
           
          
            ArrayList hori = new ArrayList();
            ArrayList vertiSf = new ArrayList();
            ArrayList vertiBm = new ArrayList();

            int n =(int) Math.Round(lCanti / incrx, 0);
            float inc = lCanti / n;
            for(int i = 0; i <= n;i++)
            {
                // shear force diagram
                hori.Add( i*inc);
                vertiSf.Add( -aveStr * i*inc*centi);
                //bending moment diagram
                vertiBm.Add( -0.5f * aveStr * i*inc * i*inc*tetra);
            }
            lsf.Add(hori);
            lsf.Add(vertiSf);
            lbm.Add(hori);
            lbm.Add(vertiBm);
           ArrayList hori1 = new ArrayList();
            ArrayList vertiSf1 = new ArrayList();
            ArrayList vertiBm1 = new ArrayList();
            n = (int)Math.Round(rCanti / incrx, 0);
             inc = rCanti / n;
            for (int i = 0; i <= n; i ++)
            {
                // shear force diagram
                hori1.Add(i*inc);
                vertiSf1.Add((aveStr*rCanti*centi)-aveStr * i*inc*centi);
                //bending moment diagram
                vertiBm1.Add((aveStr*rCanti*i*inc*tetra) -(0.5f * aveStr * i*inc * i*inc*tetra)- (0.5f * aveStr * rCanti * rCanti*tetra));
            }
            rsf.Add(hori1);
            rsf.Add(vertiSf1);
            rbm.Add(hori1);
            rbm.Add(vertiBm1);

            // top and bottom
            ArrayList hori2 = new ArrayList();
            ArrayList vertiSf2 = new ArrayList();
            ArrayList vertiBm2 = new ArrayList();
            n = (int)Math.Round(bCanti / incry, 0);
            inc = bCanti / n;
            for (int i = 0; i <= n; i ++)
            {
                // shear force diagram
                hori2.Add(i*inc);
                vertiSf2.Add( -aveStr * i*inc*centi);
                //bending moment diagram
                vertiBm2.Add( -0.5f * aveStr * i*inc * i*inc*tetra);
            }
            bsf.Add(hori2);
            bsf.Add(vertiSf2);
            bbm.Add(hori2);
            bbm.Add(vertiBm2);

           ArrayList hori3 = new ArrayList();
            ArrayList vertiSf3 = new ArrayList();
            ArrayList vertiBm3 = new ArrayList();
            n = (int)Math.Round(tCanti / incry, 0);
            inc = tCanti / n;
            for (int i = 0; i <= n; i ++)
            {
                // shear force diagram
                hori3.Add( i*inc);
                vertiSf3.Add( ( aveStr * tCanti*centi) - aveStr * i*inc*centi);
                //bending moment diagram
                vertiBm3.Add(( aveStr * tCanti * i*inc*tetra) - (0.5f * aveStr * i*inc * i*inc*tetra) - (0.5f * aveStr * tCanti * tCanti*tetra));
            }
            tsf.Add(hori3);
            tsf.Add(vertiSf3);
            tbm.Add(hori3);
            tbm.Add(vertiBm3);
        }
        public void DrawSfx( DovDrawings g,ChartStyle cs)
        {
            PointF pt1;
            PointF pt2;
            PointF pt3;
            PointF pt4;
            cs.XMin = 0;
            cs.XMax = fData.Width;
            float miny1 = Min(lsf[1].ToArray()) ;
            float miny2 = Min(rsf[1].ToArray());
            float maxy1 = Max(lsf[1].ToArray());
            float maxy2 = Max(rsf[1].ToArray());
            cs.YMin = (miny1 < miny2 ? miny1 : miny2);
            cs.YMax = (maxy1 > maxy2 ? maxy1 : maxy2);
            //await g.DrawRecAsync(cs.Point2D(new PointF(0, 0)),cs.Point2D( new PointF(50, 10)));
            for (int i = 0; i < lsf[0].Count-1;i++)
            {
                 pt1 = new PointF((float)lsf[0][i], (float)lsf[1][i]);
                 pt2= new PointF((float)lsf[0][i+1], (float)lsf[1][i+1]);
              
                 pt3 = new PointF(pt1.X, 0);
                 pt4 = new PointF(pt2.X, 0);
                g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
                g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
                g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
                if (i == (lsf[0].Count - 2))
                {
                    string va = string.Format("{0}kN", Math.Round(pt2.Y,2));
                    g.FillColorOpt = "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Top;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Right;
                    g.DrawText(va,cs.Point2D(pt2));
                }
            }
            for (int i = 0; i < rsf[0].Count - 1; i++)
            {
                pt1 = new PointF((float)rsf[0][i]+fData.ColLocX+0.5f*fData.ColumnWidth, (float)rsf[1][i]);
                pt2 = new PointF((float)rsf[0][i + 1]+ fData.ColLocX + 0.5f * fData.ColumnWidth, (float) rsf[1][i + 1]);
                pt3 = new PointF(pt1.X, 0);
                pt4 = new PointF(pt2.X, 0);
                if (i == 0)
                {
                     g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt3));
                    //value
                    g.FillColorOpt= "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Bottom;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Left;
                    string va = string.Format("{0}kN",Math.Round( pt1.Y,2));

                    g.DrawText(va, cs.Point2D(pt1));
                }
                 g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
               if(pt2.Y!=pt4.Y)  g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
                 g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
             
            }
        }
        public void DrawSfy(DovDrawings g, ChartStyle cs)
        {
            PointF pt1;
            PointF pt2;
            PointF pt3;
            PointF pt4;
            cs.YMin = 0;
            float minx1 = Min(bsf[1].ToArray());
            float minx2 = Min(tsf[1].ToArray());
            float maxx1 = Max(bsf[1].ToArray());
            float maxx2 = Max(tsf[1].ToArray());
            cs.XMin = (minx1 < minx2 ? minx1 : minx2);
            cs.XMax = (maxx1 > maxx2 ? maxx1 : maxx2);
            cs.YMax = fData.Length;
            for (int i = 0; i < bsf[0].Count - 1; i++)
            {
                pt1 = new PointF((float)bsf[1][i], (float)bsf[0][i]);
                pt2 = new PointF((float)bsf[1][i + 1],(float) bsf[0][i + 1]);
                pt3 = new PointF(0, pt1.Y);
                pt4 = new PointF(0, pt2.Y);
                 g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
                g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
                 g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
                if (i == (bsf[0].Count - 2))
                {
                    string va = string.Format("{0}kN",Math.Round( pt2.X,2));
                    g.FillColorOpt = "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Bottom;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Left;
                    g.DrawText(va, cs.Point2D(pt2));
                }
            }
            for (int i = 0; i < tsf[0].Count - 1; i++)
            {
                pt1 = new PointF((float)tsf[1][i] , (float)tsf[0][i] + fData.ColLocY+0.5f*fData.ColumnLength);
                pt2 = new PointF((float)tsf[1][i + 1] ,(float) tsf[0][i + 1] +fData.ColLocY+0.5f* fData.ColumnLength);
                pt3 = new PointF(0, pt1.Y);
                pt4 = new PointF(0, pt2.Y);
                if (i == 0)
                {
                    g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt3));
                    //value
                    string va = string.Format("{0}kN", Math.Round(pt1.X,2));
                    g.FillColorOpt = "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Top;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Right;
                    g.DrawText(va, cs.Point2D(pt1));
                }
                 g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
                if (pt2.X != pt4.X)  g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
                 g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
            }
        }
        public void DrawBmx(DovDrawings g, ChartStyle cs)
        {
            PointF pt1;
            PointF pt2;
            PointF pt3;
            PointF pt4;
            cs.XMin = 0;
            cs.XMax = fData.Width;
            float miny1 = Min(lbm[1].ToArray());
            float miny2 = Min(rbm[1].ToArray());
            float maxy1 = Max(lbm[1].ToArray());
            float maxy2 = Max(rbm[1].ToArray());
            cs.YMin = (miny1 < miny2 ? miny1 : miny2);
            cs.YMax = (maxy1 > maxy2 ? maxy1 : maxy2);
            for (int i = 0; i < lbm[0].Count - 1; i++)
            {
                pt1 = new PointF((float)lbm[0][i], (float)lbm[1][i]);
                pt2 = new PointF((float)lbm[0][i + 1], (float)lbm[1][i + 1]);
                pt3 = new PointF(pt1.X, 0);
                pt4 = new PointF(pt2.X, 0);
                g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
                g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
                g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
                if (i == (lbm[0].Count - 2))
                {
                    string va = string.Format("{0}kNm",Math.Round( pt2.Y,2));
                    g.FillColorOpt = "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Top;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Right;
                    g.DrawText(va, cs.Point2D(pt2));
                }
            }
            for (int i = 0; i < rbm[0].Count - 1; i++)
            {
                pt1 = new PointF((float)rbm[0][i] +fData.ColLocX+ 0.5f*fData.ColumnWidth, (float)rbm[1][i]);
                pt2 = new PointF((float)rbm[0][i + 1] +fData.ColLocX+ 0.5f*fData.ColumnWidth, (float)rbm[1][i + 1]);
                pt3 = new PointF(pt1.X, 0);
                pt4 = new PointF(pt2.X, 0);
                if (i == 0)
                {
                     g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt3));
                    string va = string.Format("{0}kNm", Math.Round(pt1.Y,2));
                    g.FillColorOpt = "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Top;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Left;
                     g.DrawText(va, cs.Point2D(pt1));
                }
                g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
                if (pt2.Y != pt4.Y) g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
                g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
            }
        }
        public void DrawBmy(DovDrawings g, ChartStyle cs)
        {
            PointF pt1;
            PointF pt2;
            PointF pt3;
            PointF pt4;
            cs.YMin = 0;
            float minx1 = Min(bbm[1].ToArray());
            float minx2 = Min(tbm[1].ToArray());
            float maxx1 = Max(bbm[1].ToArray());
            float maxx2 = Max(tbm[1].ToArray());
            cs.XMin = (minx1 < minx2 ? minx1 : minx2);
            cs.XMax = (maxx1 > maxx2 ? maxx1 : maxx2);
            cs.YMax = fData.Length;
            for (int i = 0; i < bbm[0].Count - 1; i++)
            {
                pt1 = new PointF((float)bbm[1][i], (float)bbm[0][i]);
                pt2 = new PointF((float)bbm[1][i + 1], (float)bbm[0][i + 1]);
                pt3 = new PointF(0, pt1.Y);
                pt4 = new PointF(0, pt2.Y);
                g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
                g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
               g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
                if (i == (bbm[0].Count - 2))
                {
                    string va = string.Format("{0}kNm",Math.Round( pt2.X,2));
                    g.FillColorOpt = "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Bottom;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Left;
                     g.DrawText(va, cs.Point2D(pt2));
                }
            }
            for (int i = 0; i < tbm[0].Count - 1; i++)
            {
                pt1 = new PointF((float)tbm[1][i],(float) tbm[0][i] + fData.ColLocY+0.5f*fData.ColumnLength);
                pt2 = new PointF((float)tbm[1][i + 1], (float)tbm[0][i + 1] +fData.ColLocY+ 0.5f*fData.ColumnLength);
                pt3 = new PointF(0, pt1.Y);
                pt4 = new PointF(0, pt2.Y);
                if (i == 0)
                {
                    g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt3));
                    //value
                    string va = string.Format("{0}kNm",Math.Round( pt1.X,2));
                    g.FillColorOpt = "black";
                    g.TextBase = Canvas.CanvasEnum.TextBaseline.Top;
                    g.TextAlignment = Canvas.CanvasEnum.TextAlign.Left;
                     g.DrawText(va, cs.Point2D(pt1));
                }
                 g.DrawLine(cs.Point2D(pt1), cs.Point2D(pt2));
                if (pt2.X != pt4.X)  g.DrawLine(cs.Point2D(pt2), cs.Point2D(pt4));
                g.DrawLine(cs.Point2D(pt4), cs.Point2D(pt3));
            }
        }
        public float Min(float[] data)
        {
            float min = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (min > data[i])
                {
                    min = data[i];
                }
            }
            return min;
        }
        public float Min(object[] data)
        {
            float min = (float)data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (min >(float) data[i])
                {
                    min = (float)data[i];
                }
            }
            return min;
        }
        public float Max(float[] data)
        {
            float max = data[0]; ; ;
            for (int i = 1; i < data.Length; i++)
            {
                if (max < data[i])
                {
                    max = data[i];
                }
            }
            return max;
        }
        public float Max(object[] data)
        {
            float max = (float)data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (max < (float)data[i])
                {
                    max = (float)data[i];
                }
            }
            return max;
        }
        public float AbsMax(object[] data)
        {
            float max = (float)data[0] ;
            for (int i = 1; i < data.Length; i++)
            {
                if (Math.Abs(max) < Math.Abs((float)data[i]))
                {
                    max = (float)data[i];
                }
            }
            return max;
        }
        public bool CheckPunching()
        {
            switch (FootLocation())
            {
                case 0:return CheckInterPunching();
                case 1:return CheckEdgePunching();
                case 2:return CheckCornerPunching();
                case 4:return PunchPerimeterOutside();
                default:return true;
            }
        }
       private bool PunchPerimeterOutside()
        {
            float d = EffAv();
            // check max shear at face of column a=0;
            float reaction = aveStr * fData.ColumnWidth * fData.ColumnLength * hecto * micro;// kN
            float Vreduced = fData.Pn - reaction;
            VRdcPunching = 1000f;
            float ved =  (Vreduced * kilo / (Perimeter(0f) * d));//MPa
            float vrdmax = VRdmax();
            if (vrdmax < ved) return false;
            VEdPunching = 500f;
            float as1 = AsSingle(CalcMaxMomX(), d, 1000);
            float as2 = AsSingle(CalcMaxMomY(), d, 1000);
            Asx = as1;
            Asy = as2;
            return true;
        }
       
        private bool CheckInterPunching()
        {
            float c1 = fData.ColumnWidth*deca;
            float c2 = fData.ColumnLength*deca;
            float d = EffAv();
            float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
            float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
            float eper = exc + (fData.My / fData.Pn);
            float epar = eyc + (fData.Mx / fData.Pn);
            float beta = 1;
            if (eper != 0 && epar != 0)
            {
                float bx = c1 + 4 * d;
                float by = c2 + 4 * d;
                beta = CalcBiBeta(eper, epar, bx*milli, by*milli);
            }
            else if (eper != 0 && epar == 0)
            {
                beta = CalcUniBeta(eper * fData.Pn, fData.Pn, c1, c2, d);
            }
            else if (eper == 0 && epar != 0)
            {
                beta = CalcUniBeta(epar * fData.Pn, fData.Pn, c2, c1, d);
            }
            else
            {
                beta = 1;
            }
            // check max shear at face of column a=0;
            float reaction = aveStr * fData.ColumnWidth*fData.ColumnLength* hecto*micro;// kN
            float Vreduced = fData.Pn - reaction;
            float ved = beta * (Vreduced*kilo / (Perimeter(0f) * d));//MPa
            float vrdmax = VRdmax();
            if (vrdmax < ved) return false;
            // check punching shear at the perimater
            reaction = aveStr * CritArea(2 * d)*micro;
            Vreduced = fData.Pn - reaction;
            ved= beta * (Vreduced * kilo / (Perimeter(2*d) * d));//MPa
            float as1 = AsSingle(CalcMaxMomX(), d, 1000);
            float as2 = AsSingle(CalcMaxMomY(), d, 1000);
            Asx = as1;
            Asy = as2;
            float Vrdc = VRdcPunch(d, as1, as2, 1000, 1000 * fData.Thickness*deca);
            VEdPunching = ved;
            VRdcPunching = Vrdc;
            if (Vrdc < ved) { return false; } else { return true; }
        }
        private bool CheckEdgePunching()
        {
            float d = EffAv();
            int[,] pType = PType(2*d);
            float beta = 1;
            if (pType[0, 0] == 0)
            {
                float c1 = fData.ColumnWidth*deca;
                float c2 = fData.ColumnLength*deca;
                float rp = REdgePer(c1, c2, d);
                float per = Perimeter(2*d);
               float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
               float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
                float eper = exc + (fData.My / fData.Pn);
               float epar = eyc + (fData.Mx / fData.Pn);
                bool both = (eper != 0 && epar!= 0);
                bool inward = (eper > 0);
                bool exist = eper!= 0;
                 beta = EdgeBeta(inward, both, exist, eper, epar, d, per, rp, c1, c2);
            }
            else if (pType[0, 1] == 0)
            {
                float c2 = fData.ColumnWidth*deca;
                float c1 = fData.ColumnLength*deca;
                float rp = REdgePer(c1, c2, d);
                float per = Perimeter(2*d);
                float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
                float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
                float epar = exc + (fData.My / fData.Pn);
                float eper = eyc + (fData.Mx / fData.Pn);
                bool both = (eper != 0 && epar != 0);
                bool inward = (eper < 0);
                bool exist = eper != 0;
                beta = EdgeBeta(inward, both, exist, eper, epar, d, per, rp, c1, c2);
            }
            else if (pType[1, 1]==0)
            {
                float c1 = fData.ColumnWidth*deca;
                float c2 = fData.ColumnLength*deca;
                float rp = REdgePer(c1, c2, d);
                float per = Perimeter(2*d);
                float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
                float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
                float eper = exc + (fData.My / fData.Pn);
                float epar = eyc + (fData.Mx / fData.Pn);
                bool both = (eper != 0 && epar != 0);
                bool inward = (eper < 0);
                bool exist = eper != 0;
                 beta = EdgeBeta(inward, both, exist, eper, epar, d, per, rp, c1, c2);
            }
            else 
            {
                float c2 = fData.ColumnWidth*deca;
                float c1 = fData.ColumnLength*deca;
                float rp = REdgePer(c1, c2, d);
                float per = Perimeter(2*d);
                float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
                float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
                float epar = exc + (fData.My / fData.Pn);
                float eper = eyc + (fData.Mx / fData.Pn);
                bool both = (eper != 0 && epar != 0);
                bool inward = (eper > 0);
                bool exist = eper != 0;
                 beta= EdgeBeta(inward, both, exist, eper, epar, d, per, rp, c1, c2);

            }
            float reaction = aveStr * fData.ColumnWidth * fData.ColumnLength * hecto * micro;// kN
            float Vreduced = fData.Pn - reaction;
            float ved = beta * (Vreduced * kilo / (Perimeter(0f) * d));//MPa
            float vrdmax = VRdmax();
            if (vrdmax < ved) return false;
            // check punching shear at the perimater
            reaction = aveStr * CritArea(2 * EffAv()) * micro;
            Vreduced = fData.Pn - reaction;
            ved = beta * (Vreduced * kilo / (Perimeter(2 * d) * d));//MPa
            float as1 = AsSingle(CalcMaxMomX(), d, 1000);
            float as2 = AsSingle(CalcMaxMomY(), d, 1000);
            Asx = as1;
            Asy = as2;
            float Vrdc = VRdcPunch(d, as1, as2, 1000, 1000 * fData.Thickness * deca);
            VEdPunching = ved;
            VRdcPunching = Vrdc;
            if (Vrdc < ved) { return false; } else { return true; }
        }
        
        private bool CheckCornerPunching()
        {
            float d = EffAv();
            int[,] pType = PType(2*d);
            float beta = 1;
            if (pType[0, 0] == 0&& pType[0, 1] == 0)
            {
                float c1 = fData.ColumnWidth*deca;
                float c2 = fData.ColumnLength*deca;
                float rp = RCornerPer(c1, c2, d);
                float per = Perimeter(2*d);
                float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
                float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
                float eper = exc + (fData.My / fData.Pn);
                float epar = eyc + (fData.Mx / fData.Pn);
                int both = -1;
                if (eper != 0 && epar != 0) { both = 1; } else if (eper != 0 || epar != 0) { both = 0; } else { both = -1; }
                int inward1 = -1;
                if (eper < 0) { inward1 = 0; }else if(eper > 0) { inward1 = 1; } else { inward1 = -1; }
                int inward2 = -1;
                if (epar > 0) { inward2 = 0; } else if (epar < 0) { inward2 = 1; } else { inward2 = -1; }
                beta = CornerBeta(inward1, inward2,both, eper, epar, d, per, rp, c1, c2);
            }
            else if (pType[0, 1] == 0&& pType[1, 1] == 0)
            {
                float c2 = fData.ColumnWidth*deca;
                float c1 = fData.ColumnLength*deca;
                float rp = RCornerPer(c1, c2, d);
                float per = Perimeter(2*d);
                float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
                float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
                float epar = exc + (fData.My / fData.Pn);
                float eper = eyc + (fData.Mx / fData.Pn);
                int both = -1;
                if (eper != 0 && epar != 0) { both = 1; } else if (eper != 0 || epar != 0) { both = 0; } else { both = -1; }
                int inward1 = -1;
                if (eper > 0) { inward1 = 0; } else if (eper < 0) { inward1 = 1; } else { inward1 = -1; }
                int inward2 = -1;
                if (epar > 0) { inward2 = 0; } else if (epar < 0) { inward2 = 1; } else { inward2 = -1; }
                 beta = CornerBeta(inward1, inward2, both, eper, epar, d, per, rp, c1, c2);
            }
            else if (pType[1, 1] == 0 && pType[1, 0] == 0)
            {
                float c1 = fData.ColumnWidth*deca;
                float c2 = fData.ColumnLength*deca;
                float rp = RCornerPer(c1, c2, d);
                float per = Perimeter(2*d);
                float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;
                float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;
                float eper = exc + (fData.My / fData.Pn);
                float epar = eyc + (fData.Mx / fData.Pn);
                int both = -1;
                if (eper != 0 && epar != 0) { both = 1; } else if (eper != 0 || epar != 0) { both = 0; } else { both = -1; }

                int inward1 = -1;
                if (eper > 0) { inward1 = 0; } else if (eper < 0) { inward1 = 1; } else { inward1 = -1; }
                int inward2 = -1;
                if (epar < 0) { inward2 = 0; } else if (epar > 0) { inward2 = 1; } else { inward2 = -1; }

                beta = CornerBeta(inward1, inward2, both, eper, epar, d, per, rp, c1, c2);
            }
            else
            {
                float c2 = fData.ColumnWidth*deca;
                float c1 = fData.ColumnLength*deca;
                float rp = RCornerPer(c1, c2, d);
                float per = Perimeter(2*d);
                float exc = (fData.ColLocX - 0.5f * fData.Width) * centi;//m
                float eyc = (fData.ColLocY - 0.5f * fData.Length) * centi;//m
                float epar = exc + (fData.My / fData.Pn);//m
                float eper = eyc + (fData.Mx / fData.Pn);//m
                int both = -1;
            
                if ( eper != 0 && epar != 0) { both = 1; } else if (eper != 0 || epar != 0) { both = 0; } else { both = -1; }
                int inward1 = -1;
                if (eper < 0) { inward1 = 0; } else if (eper > 0) { inward1 = 1; } else { inward1 = -1; }
                int inward2 = -1;
                if (epar < 0) { inward2 = 0; } else if (epar > 0) { inward2 = 1; } else { inward2 = -1; }
              
                beta = CornerBeta(inward1, inward2, both, eper, epar, d, per, rp, c1, c2);

            }
            float reaction = aveStr * fData.ColumnWidth * fData.ColumnLength * hecto * micro;// kN
            float Vreduced = fData.Pn - reaction;
            float ved = beta * (Vreduced * kilo / (Perimeter(0f) * d));//MPa
            float vrdmax = VRdmax();
            if (vrdmax < ved) return false;
            // check punching shear at the perimater
            reaction = aveStr * CritArea(2 * EffAv()) * micro;
            Vreduced = fData.Pn - reaction;
            ved = beta * (Vreduced * kilo / (Perimeter(2 * d) * d));//MPa
            float as1 = AsSingle(CalcMaxMomX(), d, 1000);
            float as2 = AsSingle(CalcMaxMomY(), d, 1000);
            Asx = as1;
            Asy = as2;
            float Vrdc = VRdcPunch(d, as1, as2, 1000, 1000 * fData.Thickness *deca);
            VEdPunching = ved;
            VRdcPunching = Vrdc;
            if (Vrdc < ved) { return false; } else { return true; }
        }
        private float CalcMaxMomX()
        {
            float lmax = AbsMax(lbm[1].ToArray());
            float rmax = AbsMax(rbm[1].ToArray());
            float hDsgnMom = (Math.Abs(lmax) > Math.Abs(rmax) ? Math.Abs(lmax) : Math.Abs(rmax));
            return hDsgnMom;
        }
        private float CalcMaxMomY()
        {
            float bmax = AbsMax(bbm[1].ToArray());
            float tmax = AbsMax(tbm[1].ToArray());
            float vDsgnMom = (Math.Abs(bmax) > Math.Abs(tmax) ? Math.Abs(bmax) : Math.Abs(tmax));
            return vDsgnMom;
        }
        public bool CheckWideBeamShear()
        {
            float d = EffAv();
            int[,] pType = PType(d);
           
            float as1 = AsSingle(CalcMaxMomX(), d, 1000);
            float as2 = AsSingle(CalcMaxMomY(), d, 1000);
            float Vrdc = VRdcBeam(d, as1, 1000, 1000 * fData.Thickness * deca);
            float shear1 = pType[0, 0] * (lCanti * deca - d) * aveStr * fData.Length * deca * micro;
            float shear2 = pType[1, 0] * (rCanti * deca - d) * aveStr * fData.Length * deca * micro;
            VEdWideBeam = Math.Max(shear1, shear2);
            VRdcWidebeam = Vrdc;
            if (shear1 > Vrdc || shear2 > Vrdc) { return false; }
            float shear3 = pType[0, 1] * (tCanti * deca - d) * aveStr * fData.Width * deca * micro;
            float shear4 = pType[1, 1] * (bCanti * deca - d) * aveStr * fData.Width * deca * micro;
            Vrdc = VRdcBeam(d, as2, 1000, 1000 * fData.Thickness * deca);
            VRdcWidebeam = Vrdc;

            VEdWideBeam = Math.Max(Math.Max(shear3, shear4), VEdWideBeam);
            if (shear3 > Vrdc || shear4 > Vrdc) { return false; } else { return true; }
        }
        float EffAv()
        {
            // fData.Thickness in cm
            float d1 = fData.Thickness * deca - fData.Cover - 0.5f * fData.Dia;
            float d2 = fData.Thickness * deca - fData.Cover - (3f / 2f) * fData.Dia;
            return (d1 + d2) * 0.5f;//mm
        }
        private float EdgeBeta(bool inward, bool both, bool exist, float eper,
           float epar, float d, float per, float rp, float c1, float c2)
        {
            float beta = 1;
            if (inward && both)
            {
                beta = (per / rp) + CalcK(c1, 2 * c2) * epar;
                return beta;
            }
            else if (inward && !both)
            {
                beta = per / rp;
                return beta;
            }
            else if (!inward && both)
            {
                beta = CalcBiBeta(eper, epar, (c1 + 2 * d), (c2 + 4 * d));
                return beta;
            }
            else if (!inward && exist)
            {
                beta = CalcEdgeUniBeta(eper * fData.Pn, fData.Pn, c1, c2, d);
                return beta;
            }
            else if (epar != 0)
            {
                beta = CalcEdgeUniBeta(epar * fData.Pn, fData.Pn, c1, c2, d);
                return beta;
            }
            else
            {
                return beta;
            }
        }
        private float CornerBeta(int inward1,int inward2, int both, float eper,
          float epar, float d, float per, float rp, float c1, float c2)
        {
            float beta = 1;
            // 0 outward; 1 inward; -1 none
            if (inward1==1 || inward2==1 && both==1)
            {
                beta = (per / rp);
                return beta;
            }
            else if (inward1==0 && inward2==0 &&both==1)
            {
                beta = CalcBiBeta(eper, epar, (c1 + 2 * d), (c2 + 2 * d));
                return beta;
            }
            else if (inward1 == 1 ||inward2 == 1 && both == 0)
            {
                beta = per/rp;
                return beta;
            }
            else if (inward1 == 0 || inward2 == 0 && both == 0)
            {
                float e = (eper != 0 ? eper : epar);
               
                beta = CalcUniBeta( e* fData.Pn, fData.Pn, c1, c2, d);
                return beta;
            }
            else
            {
                return beta;
            }
        }
        private float CalcK(float c1,float c2)
        {
            float[,] value = new float[,] { { 0.5f, 1, 2, 3 }, { 0.45f, 0.6f, 0.7f, 0.8f } };
            float r = c1 / c2;
            t1 = r;
            if (r == value[0, 0]) return 0.45f;
            if (r == value[0, 3]) return 0.8f;
            
            for(int i = 1; i < 4-1; i++)
            {
                if(value[0,i-1]<=r&& value[0, i] >= r)
                {
                    float x1 = value[0, i - 1];
                    float x2 = value[0, i];
                    float y1 = value[1, i - 1];
                    float y2 = value[1, i];
                   
                    return (y1 + ((y2 - y1) / (x2 - x1)) * (r - x1));
                }
            }
            return 0.6f;
        }
     
        private float CalcW1(float c1,float c2,float d)
        {
            return (0.5f * c1 * c1 + c1 * c2 + 4 * c2 * d + 16 * d * d + 2 * (float)Math.PI * d * c1);
        }
        private float CalcEdgeW1(float c1, float c2, float d)
        {
            return (0.5f * c2 * c2 + c1 * c2 + 4 * c1 * d + 8 * d * d +  (float)Math.PI * d * c1);
        }
        private float CalcUniBeta(float med,float ved,float c1,float c2,float d)
        {
           
            return (1 + (CalcK(c1, c2) * (med / ved)) * (Perimeter(2*d)*milli / CalcW1(c1*milli, c2*milli, d*milli)));
        }
        private float CalcEdgeUniBeta(float med, float ved, float c1, float c2, float d)
        {

            return (1 + (CalcK(c1, c2) * (med / ved)) * (Perimeter(2 * d)*milli / CalcEdgeW1(c1*milli, c2*milli, d*milli)));
        }
        private float CalcBiBeta(float ex, float ey , float bx, float by)
        {
            return (1+(1.8f*(float)Math.Sqrt(Math.Pow(ex/by,2)+Math.Pow(ey/bx,2))));
        }
        private float Perimeter(float a)
        {
            //checked
            // all are in mm
           
            int[,] pType = PType(a);
            float arc1 = pType[0, 0] * pType[0, 1] * (float)Math.PI  * a/2;
            float arc2 = pType[0, 1] * pType[1, 1] * (float)Math.PI *a/2;
            float arc3 = pType[0, 0] * pType[1, 0] * (float)Math.PI * a/2;
            float arc4 = pType[1, 0] * pType[1, 1] * (float)Math.PI * a/2;
            float edge1 = pType[0, 0] * fData.ColumnLength*deca;//mm
            float edge2 = pType[0, 1] * fData.ColumnWidth*deca;
            float edge3 = pType[1, 1] * fData.ColumnLength*deca;
            float edge4 = pType[1, 0] * fData.ColumnWidth*deca;
            return (arc1 + arc2 + arc3 + arc4 + edge1 + edge2 + edge3 + edge4);
        }
        private float CritArea(float a)
        {
            // all are in mm
            //checked
            int[,] pType = PType(a);
            float arc1 = pType[0, 0] * pType[0, 1] * (float)Math.PI *a* a/4;
            float arc2 = pType[0, 1] * pType[1, 1] * (float)Math.PI *a* a/4;
            float arc3 = pType[0, 0] * pType[1, 0] * (float)Math.PI *a* a/4;
            float arc4 = pType[1, 0] * pType[1, 1] * (float)Math.PI *a* a/4;
            float edge1 = pType[0, 0] * fData.ColumnLength*deca*a;
            float edge2 = pType[0, 1] * fData.ColumnWidth*deca*a;
            float edge3 = pType[1, 1] * fData.ColumnLength*deca*a;
            float edge4 = pType[1, 0] * fData.ColumnWidth*deca*a;
            return (arc1 + arc2 + arc3 + arc4 + edge1 + edge2 + edge3 + edge4); //mm2
        }
      
        private float REdgePer(float c1,float c2,float d )
        {
            float c = (0.5f * c1 < 1.5f*d ? 0.5f * c1 : 1.5f*d);
            return (2 * c+c2 + 2 * (float)Math.PI * d);//mm

        }
        private float RCornerPer(float c1, float c2, float d)
        {
            float c = 0.5f * c1 < 1.5f * d ? 0.5f * c1 : 1.5f * d;
            float b = 0.5f * c2 < 1.5f * d ? 0.5f * c2 : 1.5f * d;
            return c + b +(float)Math.PI * d;//mm
        }
        private int[,] PType(float dav)
        {
         
            int[,] pType = new int[2, 2];
            pType[0, 0] = (lCanti  >  dav*deci ? 1 : 0);
            pType[0, 1] = (tCanti  > dav*deci ? 1 : 0);
            pType[1, 0] = (rCanti  >  dav*deci ? 1 : 0);
            pType[1, 1] = (bCanti >  dav*deci ? 1 : 0);
            return pType;
        }
        private int FootLocation()
        {
            int n = 0;
            int[,] pType = PType(2*EffAv());
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    if (pType[i, j] == 0)
                    {
                        n++;
                    }
                }
            }
         
           
            return n;
        }
        private int [] MContain(int[,]data,int n)
        {
           
                for(int j = 0; j < data.GetLength(1); j++)
                {
                  if (data[0, j] == n) return new int[] { 1, j };
                }
            return new int[] { -1, -1 };
        }
        private float AsSingle(float med, float d,float bw)
        {
            // d in mm; bw in mm; med kNm; As in mm2
            // Fck = MPa;
            float mrd = 0.167f * ConcMat.Fck * bw * d * d * micro;//kNm
            if (mrd < med) return 0;
            float k0 = med / (ConcMat.Fck * bw * d * d) * mega;
            float z = d * (0.5f + (float)Math.Sqrt(0.25f - ((3 * k0) / 3.4)));
            float As = med*mega / (RebarMat.Fyk * z / DsgnPref.SaftyFacRebar);//mm2
            float fctm = 0.3f *(float) Math.Pow(ConcMat.Fck, 0.67f);
            float Asmin1 = 0.26f * (fctm / RebarMat.Fyk) * bw * d;
            float Asmin2 = 0.0013f * bw * d;
            float AminMax = (float)Math.Max(Asmin1, Asmin2);
            return (float)Math.Max(AminMax,As);//mm2
        }
        private float VRdcPunch(float d,float As1,float As2, float bw, float Ag, float ?ned1=null,float ?ned2=null)
        {
            // d in mm;As in mm2; bw in mm; Ag in mm ;ned in kN
            float CRdc = 1.8f / DsgnPref.SaftyFacConc;
            float k =Math.Min( 1+(float)Math.Sqrt(200f/d),2);
            float ro1 = As1 / (bw*d);
            float ro2 =As2 / (bw * d);
            float ro = Math.Min((float)Math.Sqrt(ro1* ro2),0.02f);
            float k1 = 0.1f;
            float scp = 0.5f * ((ned1 != null ? ((float)ned1*kilo / Ag):0) + (ned2 != null ? ((float)ned2*kilo / Ag):0));
            float vmin = 0.035f * (float)Math.Sqrt(ConcMat.Fck) * (float) Math.Pow(k, 3f / 2f);
            float vrdc1 = (CRdc * k * (float)Math.Pow(100 * ro1 * ConcMat.Fck, 1f / 3f) + k1 * scp) ;//MPa
            float vrdc2 = (vmin + k1 * scp);
            // return in MPa
            return Math.Max(vrdc1, vrdc2);

        }
        // all are in mm and mm2;ned1 is in kN;
        private float VRdcBeam(float d, float As1,  float bw, float Ag, float? ned1 = null)
        {
            // d in mm;As in mm2; bw in mm; Ag in mm ;ned in kN
            float CRdc = 1.8f / DsgnPref.SaftyFacConc;
            float k = Math.Min(1 + (float)Math.Sqrt(200f / d), 2);
            float ro1 = As1 / (bw * d);
            float k1 = 0.15f;
            float scp = (ned1 != null ? ((float)ned1 * kilo / Ag) : 0);
            float vmin = 0.035f * (float)Math.Sqrt(ConcMat.Fck) * (float)Math.Pow(k, 3f / 2f);
            float vrdc1 = (CRdc * k * (float)Math.Pow(100 * ro1 * ConcMat.Fck, 1f / 3f) + k1 * scp) * bw * d * milli;//kN
            float vrdc2 = (vmin + k1 * scp) * bw * d * milli;
            // return in kN
            return Math.Max(vrdc1, vrdc2);

        }
        private float VRdmax()
        {
            // fck in MPa
            float v = 0.6f * (1 - (ConcMat.Fck / 250f));
            float vrdmax = 0.5f * v * DsgnPref.SaftyFacConc * DsgnPref.Alpha * ConcMat.Fck;
            // return in MPa
            return vrdmax;
        }
    }
}
