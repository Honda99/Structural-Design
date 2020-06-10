using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using static EngDolphin.Canvas.CanvasEnum;

namespace EngDolphin.Client.Models
{
    public class CombFootingDesign:UnitConv
    {
        private float aveStr=0;// kN/m2
        public string DesignCondition { get; set; } = "";
        //
        public float VRdcWidebeam { get; set; }
        public float VRdcPunching { get; set; }
        public float VEdPunching { get; set; }
        public float VEdWideBeam { get; set; }
        public float Asx { get; set; }
        public float Asy { get; set; }
        public float AsxSup1 { get; set; }
        public float AsxSup2 { get; set; }
        public float Wsup1 { get; set; }
        public float Wsup2 { get; set; }
        public float Wspan { get; set; }
        public CombFootingData fData { get; set; }
        public DovDsgnPreference DsgnPref { get; set; }
        public DovConcMat ConcMat { get; set; }
        private const float lamda = 0.8f;
        public DovRebarMat RebarMat { get; set; }
        private CombFootAnalysis ca;
        int nel = 3;
        int ner = 3;
        public CombFootingDesign( CombFootingData fd,DovConcMat conc,DovRebarMat reb)
        {
            fData = fd;
            ConcMat = conc;
            RebarMat =reb;
        }
        public CombFootingDesign():base()
        {
        }
        public void CheckBearingStress()
        {
            float x1 = fData.X1*centi;
            float x2 = fData.X2*centi;
            float x3 = fData.X3*centi;
            float b1 = fData.W1*centi;
            float b2 = fData.W2*centi;
            float l = x1+x2+x3;
            float qult = fData.UltBearingCap;
            float mx1 =fData.Loads[0].Mx;
            float my1 = fData.Loads[0].My;
            float pn1 = fData.Loads[0].Pn;
            float mx2 = fData.Loads[1].Mx;
            float my2 = fData.Loads[1].My;
            float pn2 = fData.Loads[1].Pn;
            float qa = qult / fData.SaftyFactor;
            float smx1 = fData.Loads[0].sMx;
            float smy1 = fData.Loads[0].sMy;
            float spn1 = fData.Loads[0].sPn;
            float smx2 = fData.Loads[1].sMx;
            float smy2 = fData.Loads[1].sMy;
            float spn2 = fData.Loads[1].sPn;
            float st1, st2, st3, st4;
            float minst, maxst;
            float ix=0, iy=0;
            float ex;
            float area=0;
            float cx=0, cy=0;
            PointF[] polPt=new PointF[5];
            PointF pt1 = new PointF(0,-0.5f*b1);
            PointF pt2 = new PointF(l, -0.5f * b2);
            PointF pt3 = new PointF(l, 0.5f * b2);
            PointF pt4 = new PointF(0, 0.5f * b1);
            polPt[0] = pt1;
            polPt[1] = pt2;
            polPt[2] = pt3;
            polPt[3] = pt4;
            polPt[4] = pt1;
            for(int i = 0; i < polPt.Length - 1; i++)
            {
                area += 0.5f * ((polPt[i].X * polPt[i + 1].Y) - (polPt[i + 1].X * polPt[i].Y));
            }
            for(int i = 0; i < polPt.Length-1; i++)
            {
                cx += (1 / (6 * area)) * (polPt[i].X + polPt[i + 1].X) * (polPt[i].X * polPt[i + 1].Y - polPt[i + 1].X * polPt[i].Y);
                cy+= (1 /( 6 * area)) * (polPt[i].Y + polPt[i + 1].Y) * (polPt[i].X * polPt[i + 1].Y - polPt[i + 1].X * polPt[i].Y);
                iy += (1f / 12f) * (polPt[i].X * polPt[i + 1].Y - polPt[i + 1].X * polPt[i].Y) * (polPt[i].X * polPt[i].X + polPt[i].X * polPt[i + 1].X + polPt[i + 1].X * polPt[i + 1].X);
                ix += (1f / 12f) * (polPt[i].X * polPt[i + 1].Y - polPt[i + 1].X * polPt[i].Y) * (polPt[i].Y * polPt[i].Y + polPt[i].Y * polPt[i + 1].Y + polPt[i + 1].Y * polPt[i + 1].Y);
            }
            iy -= area * cx * cx;
            float R = spn1 + spn2;
            float xr = (spn2 * x2+smy1+smy2) / R;
            ex = x1 + xr - cx;
            float myn = R * ex;
            float my = myn ;
            float mx = smx1 + smx2;
            float xn1 = cx;
            float xn2 = x1 + x2 + x3 - cx;
            float yn1 = 0.5f * b1;
            float yn2 = 0.5f * b2;
            float yn3 = -0.5f * b2;
            float yn4 = -0.5f * b1;
            st1 = (R / area) + (mx * yn1 / ix) + (my* -xn1 / iy);
            st2 = (R / area) + (mx * yn2 / ix) + (my* xn2 / iy);
            st3 = (R / area) + (mx *yn3  / ix) + (my* xn2 / iy);
            st4 = (R / area) + (mx *yn4 / ix) + (my*-xn1 / iy);
            minst = Min(new[] { st1, st2, st3, st4 });
            maxst = Max(new[] { st1, st2, st3, st4 });
            if (minst < 0)
            {
                DesignCondition = "Warning: Minimum stress is negative," +
                       "change footing dimension or column location.";
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
            R = pn1 + pn2;
            xr = (pn2 * x2+my1+my2) / R;
            ex = x1 + xr - cx;
             myn = R * ex;
             my = myn ;
             mx = smx1 + smx2;
             xn1 = cx;
             xn2 = x1 + x2 + x3 - cx;
             yn1 = 0.5f * b1;
             yn2 = 0.5f * b2;
             yn3 = -0.5f * b2;
            yn4 = -0.5f * b1;
            st1 = (R / area) + (mx * yn1 / ix) + (my * -xn1 / iy);
            st2 = (R / area) + (mx * yn2 / ix) + (my * xn2 / iy);
            st3 = (R / area) + (mx * yn3 / ix) + (my * xn2 / iy);
            st4 = (R / area) + (mx * yn4 / ix) + (my * -xn1 / iy);
            minst = Min(new[] { st1, st2, st3, st4 });
            maxst = Max(new[] { st1, st2, st3, st4 });
            //stress in KN/m2
            aveStr = (minst + maxst) * 0.5f;
            //visual check
        }
       public void Analysis()
        {
            ca = new CombFootAnalysis();
            ca.ConcMat = ConcMat;
            ca.RebMat = RebarMat;
            ca.Data = fData;
            ca.nel = nel;
            ca.ner = ner;
            ca.AveStress = aveStr;
            ca.Analyze();
        }
      
        public bool CheckPunching()
        {
            // all in meter
            float b1 = fData.W1*centi;
            float b2 = fData.W2*centi;
            float cw1 = fData.Cw1*centi;
            float cl1 = fData.Cl1*centi;
            float cw2 = fData.Cw2*centi;
            float cl2 = fData.Cl2*centi;
            float l1 = fData.X1*centi;
            float l2 = fData.X2*centi;
            float l3 = fData.X3*centi;
            float l = l1 - 0.5f * cw1;
            float w = 0.5f * b1 - 0.5f * cl1;

            //moment in kNm, pn in kN;
            float mx1 = fData.Loads[0].Mx;
            float my1= fData.Loads[0].My;
            float pn1= fData.Loads[0].Pn;
            float mx2 = fData.Loads[1].Mx;
            float my2 = fData.Loads[1].My;
            float pn2 = fData.Loads[1].Pn;
            int[] ptype1 = CombPType(EffAv(), l, w);
            l = l3 - 0.5f * cw2;
            w = 0.5f * b2 - 0.5f * cl2;
            float d = EffAv();
            int[] ptype2 = CombPType(EffAv(), l, w);
            float supM1 = FindMaxMoments()[0];
            float supM2 = FindMaxMoments()[2];
            float spanM = FindMaxMoments()[1];
            float tl = l1 + l2 + l3;
            Wsup1 = (((b1 - b2) / tl) * (tl - l1) + b2)*kilo;
            Wsup2 =( ((b1 - b2) / tl) * (tl - l1-l2) + b2)*kilo;
            AsxSup1 = AsSingle(supM1,d,Wsup1);
            AsxSup2 = AsSingle(supM2, d, Wsup2 );
            //span longitudinal reinforcement 
            Wspan= (((b1 - b2) / tl) * (tl - locOfNegatMoment()) + b2)*kilo;
            Asx = AsSingle(spanM, d, Wspan);
            //span transverse reinforcement
            float transM = ca.TransverseBending();
            Asy = AsSingle(transM, d, 1000f);
            return CombPunching(cw1, cl1, mx1, my1, pn1, ptype1) && CombPunching(cw2, cl2, mx2, my2, pn2, ptype2);
        }
        private float locOfNegatMoment()
        {
            List<float> mom = new List<float>();
            float[,] Am = ca.AmAllMemeAction;
            float[] El = ca.El;
            float l2=0;
            for (int i = 0; i < Am.GetLength(0); i++)
            {
                float ml = Am[i, 1]; float mr = -Am[i, 3];
                mom.Add(ml);
                mom.Add(mr);
            }
            float[] locAndMin = MinAndLocation(mom.ToArray());
            int elno = FindElement((int)locAndMin[0]);
            for (int j = 0; j <= elno; j++)
            {
                l2 += El[j];
            }
            return l2;
        }
        private bool CombPunching(float cw1,float cl1,float mx1,float my1,float pn1,int[]ptype1)
        {
            bool state = true;
            if (ptype1[0] == 0 && ptype1[1] == 0)
            {
               return state = true;
            }
            else if (ptype1[0] != 0 && ptype1[1] != 0)
            {
               return state = CheckInterPunching(cw1, cl1, mx1, my1, pn1);
            }
            else if (ptype1[0] != 0 && ptype1[1] == 0)
            {
               return  state = true;
            }
            else if (ptype1[0] == 0 && ptype1[1] != 0)
            {
              return  state = CheckEdgePunching(cw1, cl1, mx1, -my1, pn1);
            }
            else
            {
              return  state = true;
            }
        }
       
       // cw,cl in meter; mx ,my in kNm;pn in kN
        private bool CheckInterPunching(float cw,float cl,float mx,float my, float pn )
        {
            float c1 = cw*kilo;// in mm
            float c2 =cl*kilo;
            float d = EffAv(); // in mm
            float eper =  my / pn;
            float epar =  mx / pn;
            float beta = 1;
            if (eper != 0 && epar != 0)
            {
                float bx = c1 + 4 * d;//mm
                float by = c2 + 4 * d;//mm
                beta = CalcBiBeta(eper, epar, bx, by);
            }
            else if (eper != 0 && epar == 0)
            {
                //c1 in mm,c2 in mm,d in mm;
                beta = CalcUniBeta(eper * pn, pn, c1, c2, d, CombIntPerimeter(c1, c2, 2 * d));
            }
            else if (eper == 0 && epar != 0)
            {
                beta = CalcUniBeta(epar * pn, pn, c2, c1, d, CombIntPerimeter(c1, c2, 2 * d));
            }
            else
            {
                beta = 1;
            }
            // check max shear at face of column a=0;
            float reaction = aveStr * c1*c2*micro;// kN
            float Vreduced = pn - reaction;
            float ved = beta * (Vreduced*kilo / (CombIntPerimeter(c1,c2,0f) * d));//MPa
            float vrdmax = VRdmax();
            if (vrdmax < ved) return false;
            // check punching shear at the perimater,,
            reaction = aveStr * CombIntCritArea(c1,c2,2 * d)*micro;
            Vreduced = pn - reaction;
            ved= beta * (Vreduced * kilo / (CombIntPerimeter(c1, c2, 2*d) * d));//MPa
            float hDsgnMom = FindMaxMoments()[1];
            float vDsgnMom = ca.TransverseBending();
            //this is to be edited
            float b = 0.5f * (fData.W1 + fData.W2) * deca;
            float as1 = AsSingle(hDsgnMom, d, b);
            float as2 = AsSingle(vDsgnMom, d, 1000);
            Asx = as1;
            Asy = as2;
            float Vrdc = VRdcPunch(d, as1, as2, 1000, 1000 * fData.Thickness*deca);
            VEdPunching = ved;
            VRdcPunching = Vrdc;
            if (Vrdc < ved) { return false; } else { return true; }
        }
        // cw,cl in meter; mx ,my in kNm;pn in kN
        private bool CheckEdgePunching(float cw,float cl,float mx,float my,float pn)
        {
            float d = EffAv();
            float beta = 1;
                float c1 = cw*kilo;
                float c2 = cl*kilo;
                float rp = REdgePer(c1, c2, d);
                float per =CombEdgePerimeter(c1,c2,2*d);
                float eper =  my / pn;
               float epar =  mx / pn;
                bool both = (eper != 0 && epar!= 0);
                bool inward = (eper > 0);
                bool exist = eper!= 0;
                 beta = EdgeBeta(inward, both, exist, eper, epar, d, per, rp, c1, c2,pn);
            float reaction = aveStr * c1*c2*micro;// kN
            float Vreduced = pn - reaction;
            float ved = beta * (Vreduced * kilo / (CombEdgePerimeter(c1,c2,0f) * d));//MPa
            float vrdmax = VRdmax();
            if (vrdmax < ved) return false;
            // check punching shear at the perimater
            reaction = aveStr *CombEdgeCritArea(c1,c2,2 * d) * micro;
            Vreduced = pn - reaction;
            ved = beta * (Vreduced * kilo / (CombEdgePerimeter(c1,c2,2 * d) * d));//MPa
            // to be edited
            float hDsgnMom = FindMaxMoments()[1];
            float vDsgnMom = ca.TransverseBending();
            float b = 0.5f * (fData.W1 + fData.W2) * deca;
            float as1 = AsSingle(hDsgnMom, d, b);
            float as2 = AsSingle(vDsgnMom, d, 1000f);
            Asx = as1;
            Asy = as2;
            float Vrdc = VRdcPunch(d, as1, as2, 1000f, 1000f * fData.Thickness *deca);
            VEdPunching = ved;
            VRdcPunching = Vrdc;
            if (Vrdc < ved) { return false; } else { return true; }
        }
        public bool CheckWideBeamShear()
        {
            float d = EffAv();
            float hDsgnMom = FindMaxMoments()[1];
            float vDsgnMom = ca.TransverseBending();
            float b = 0.5f * (fData.W1 + fData.W2) * deca;
            float as1 = AsSingle(hDsgnMom, d, b);
            float as2 = AsSingle(vDsgnMom, d, 1000f);
            float Vrdc = VRdcBeam(d, as1, b, 1000 * fData.Thickness * deca);
            // to be edited
            VEdWideBeam = FindDesignShear();
            VRdcWidebeam = Vrdc;
            if (VEdWideBeam < Vrdc) { return true; } else { return false; }

        }
        float EffAv()
        {
            // fData.Thickness in cm
            float d1 = fData.Thickness * deca - fData.Cover - 0.5f * fData.Dia;
            float d2 = fData.Thickness * deca - fData.Cover - (3f / 2f) * fData.Dia;
            return (d1 + d2) * 0.5f;//mm
        }
        private float EdgeBeta(bool inward, bool both, bool exist, float eper,
           float epar, float d, float per, float rp, float c1, float c2,float pn)
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
                beta = CalcEdgeUniBeta(eper * pn, pn, c1, c2, d);
                return beta;
            }
            else if (epar != 0)
            {
                beta = CalcEdgeUniBeta(epar * pn, pn, c1, c2, d);
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
        private float CalcUniBeta(float med,float ved,float c1,float c2,float d,float perm)
        {
           
            return (1 + (CalcK(c1, c2) * (med / ved)) * (perm*milli / CalcW1(c1*milli, c2*milli, d*milli)));
        }
        private float CalcEdgeUniBeta(float med, float ved, float c1, float c2, float d)
        {

            return (1 + (CalcK(c1, c2) * (med / ved)) * (CombEdgePerimeter(c1,c2,2*d)*milli / CalcEdgeW1(c1*milli, c2*milli, d*milli)));
        }
        private float CalcBiBeta(float ex, float ey , float bx, float by)
        {
            return (1+(1.8f*(float)Math.Sqrt(Math.Pow(ex/(by*milli),2)+Math.Pow(ey/(bx*milli),2))));
        }
        // cw, cl in millimeter
        // a is distance from the face of the column
        private float CombIntPerimeter(float cw,float cl,float a)
        {
            //checked
            // all are in mm
            float arc1 =  (float)Math.PI  * a/2;
            float arc2 =  (float)Math.PI *a/2;
            float arc3 = (float)Math.PI * a/2;
            float arc4 = (float)Math.PI * a/2;
            float edge1 =  cw;//mm
            float edge2 = cw;
            float edge3 =cl;
            float edge4 =cl;
            return (arc1 + arc2 + arc3 + arc4 + edge1 + edge2 + edge3 + edge4);
        }
        // cw, cl in millimeter
        // a is distance from the face of the column
        private float CombEdgePerimeter(float cw, float cl, float a)
        {
            //checked
            // all are in mm
            float arc3 = (float)Math.PI * a / 2;
            float arc4 = (float)Math.PI * a / 2;
            float edge1 = cw;//mm
            float edge2 = cw;
            float edge3 = cl;
            return ( arc3 + arc4 + edge1 + edge2 + edge3 );
        }
        // cw, cl in millimeter
        // a is distance from the face of the column
        private float CombIntCritArea(float cw,float cl,float a)
        {
            // all are in mm
            //checked
            float arc1 =  4*(float)Math.PI *a* a/4;
            float edge1 = 2*cw*a;
            float edge3 = 2 * cl *a;
            return (arc1 +  edge1 + edge3 ); //mm2
        }
        // cw, cl in millimeter
        // a is distance from the face of the column
        private float CombEdgeCritArea(float cw, float cl, float a)
        {
            // all are in mm
            //checked
            float arc1 = 2 * (float)Math.PI * a * a / 4;
            float edge1 = 2 * cw * a;
            float edge3 =  cl * a;
            return (arc1 + edge1 + edge3); //mm2
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
      
        //in leng and bw meter; dav in mm
        private int[] CombPType(float dav,float leng,float bw)
        {
            int[] pType = new int[2];
            pType[0] = (leng > dav * milli ? 1 : 0);
            pType[1] = (bw > dav * milli ? 1 : 0);
            return pType;
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
            float vrdc1 = (CRdc * k * (float)Math.Pow(100 * ro1 * ConcMat.Fck, 1f / 3f) + k1 * scp);//kN
            float vrdc2 = (vmin + k1 * scp) ;
            // return in kN
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
      
        private float [] FindMaxMoments()
        {
            float[] res = new float[3];
            float[,] m=ca.AmAllMemeAction;
            int nelm = m.GetLength(0);
            List<float> negMoment = new List<float>();
            for(int i = nel; i < nelm-ner; i++)
            {
                float m1 = m[i, 1]; float m2 = -m[i, 3];
                if (m1 < 0) negMoment.Add(m1);
                if (m2 < 0) negMoment.Add(m2);
            }
            float maxNegMoment =Math.Abs( AbsMax(negMoment.ToArray()));
            float supM1 = Max(new float[] { -m[nel - 1, 3], m[nel, 1] });
            float supM2 = Max(new float[] { -m[nelm - ner - 1, 3], m[nelm - ner, 1] });
            return new float[] { supM1, maxNegMoment, supM2 };
        }
        private float FindDesignShear()
        {
            float[,] m = ca.AmAllMemeAction;
            int nelm = m.GetLength(0);
            float v1 = ca.AmAllMemeAction[nel, 0];
            float v2 = -ca.AmAllMemeAction[nelm - ner - 1, 2];
            float x = 0.5f*fData.Cw1*centi+EffAv()*milli;
            float vdl = v1 + x * (v2 - v1) / (fData.X2*centi);
            x = fData.X2*centi - 0.5f * fData.Cw2 * centi-EffAv()*milli;
            float vdr=Math.Abs( v1 + x * (v2 - v1) / (fData.X2 * centi));
            return Math.Max(vdl,vdr);
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
        public float[] MinAndLocation(float[] data)
        {
            float min = data[0];
            int loc = 0;
            for (int i = 1; i < data.Length; i++)
            {
                if (min > data[i])
                {
                    min = data[i];
                    loc = i;
                }
            }

            //location of maximum value then Maximum Value;
            return new float[] {loc,min };
        }
        public float Min(object[] data)
        {
            float min = (float)data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (min > (float)data[i])
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
            float max = (float)data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (Math.Abs(max) < Math.Abs((float)data[i]))
                {
                    max = (float)data[i];
                }
            }
            return max;
        }
        public float AbsMax(float[] data)
        {
            float max = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                if (Math.Abs(max) < Math.Abs(data[i]))
                {
                    max = data[i];
                }
            }
            return max;
        }
        public void DrawShearForceDiagram(DovDrawings g, ChartStyle cs)
        {
            DimensionLine dim = new DimensionLine();
            dim.Graphic = g;
            g.TextBase = TextBaseline.Hanging;
            g.TextAlignment = TextAlign.Left;
            g.TextFont = "12px Arial";
            float[,] Am = ca.AmAllMemeAction;
            float[] El = ca.El;
            float maxShear = 0;
            float minShear = 0;
            List<float> shear = new List<float>();
            float length = 0;
            for (int i = 0; i < Am.GetLength(0); i++)
            {
                float vl = Am[i, 0]; float vr = -Am[i, 2];
                shear.Add(vl);
                shear.Add(vr);
                length += El[i];
            }
            maxShear =Max(shear.ToArray());
            minShear =Min(shear.ToArray());
            cs.YMax = maxShear;
            cs.YMin = minShear;
            cs.XMin = 0;
            cs.XMax = length;
            float preLength = 0;
            float v1 = 0;
            float v2 = 0;
            float vnext = 0;
            PointF pt1 = new PointF();
            PointF pt2 = new PointF();
            PointF pt3 = new PointF();
            for(int i = 0; i < Am.GetLength(0);i++)
            {
                v1 = Am[i, 0];v2 = -Am[i, 2];
                pt1 = new PointF(preLength, v1);
                preLength += El[i];
                pt2 = new PointF(preLength, v2);
                pt1 = cs.Point2D(pt1);
                pt2 = cs.Point2D(pt2);
                g.DrawLine(pt1, pt2);
                if (i < Am.GetLength(0) - 1)
                {
                    vnext = Am[i + 1, 0];
                    if (vnext == v2)
                    {
                        continue;
                    }
                    else
                    {
                        pt3 = new PointF(preLength, vnext);
                        pt3 = cs.Point2D(pt3);
                        g.DrawLine(pt2, pt3);
                    }
                }
            }
            PointF pt4 = new PointF(0, 0);
            PointF pt5 = new PointF(length, 0);
            pt4 = cs.Point2D(pt4);
            pt5 = cs.Point2D(pt5);
            g.DrawLine(pt4, pt5);
            float l1 = 0;
            for (int j = 0; j < nel; j++)
            {
                l1 += El[j];
            }
            float vl1 = -Am[nel - 1, 2];
            float vl2=Am[nel, 0];
            g.FillColorOpt = "black";
            PointF pt6 = new PointF(l1, vl1);
            PointF pt7 = new PointF(l1, vl2);
            pt6 = cs.Point2D(pt6);
            dim.DrawTextAtAngle(pt6, 0f,string.Format("{0} kNm", Math.Round(vl1,2).ToString()));
            pt7 = cs.Point2D(pt7);
            g.TextBase = TextBaseline.Bottom;
            g.TextAlignment = TextAlign.Left;
             dim.DrawTextAtAngle(pt7, 0f, string.Format("{0} kNm",Math.Round( vl2,2).ToString()));
            int nelem = Am.GetLength(0);
            float vr1 = -Am[nelem - ner - 1, 2];
            float vr2 = Am[nelem - ner, 0];
            float l2 = 0;
            for (int j = 0; j < nelem - ner; j++)
            {
                l2 += El[j];
            }
            pt6 = new PointF(l2, vr1);
            pt7 = new PointF(l2, vr2);
            pt6 = cs.Point2D(pt6);
            pt7 = cs.Point2D(pt7);
            g.TextBase = TextBaseline.Hanging;
            g.TextAlignment = TextAlign.Left;
             dim.DrawTextAtAngle(pt6, 0f, string.Format("{0} kNm", Math.Round( vr1,2).ToString()));
            g.TextBase = TextBaseline.Bottom;
            g.TextAlignment = TextAlign.Left;
            dim.DrawTextAtAngle(pt7, 0f, string.Format("{0} kNm",Math.Round( vr2,2).ToString()));

        }
        public void DrawBendingMomentDiagram(DovDrawings g, ChartStyle cs)
        {
            DimensionLine dim = new DimensionLine();
            dim.Graphic = g;
            g.TextBase = TextBaseline.Bottom;
            g.TextAlignment = TextAlign.Left;
            g.TextFont = "12px Arial";
            float[,] Am = ca.AmAllMemeAction;
            float[] El = ca.El;
            float maxMom ;
            float minMom ;
            List<float> mom=new List<float>();
            float length = 0;
            for(int i = 0; i < Am.GetLength(0); i++)
            {
                float ml =Am[i, 1]; float mr = -Am[i, 3];
                mom.Add(ml);
                mom.Add(mr);
                length += El[i];
            }
            maxMom = Max(mom.ToArray());
            minMom = Min(mom.ToArray());
            cs.YMax = maxMom;
            cs.YMin = minMom;
            cs.XMax = length;
            cs.XMin = 0;
            float preLength = 0;
            float m1;
            float m2 ;
            float mnext ;
            PointF pt1 ;
            PointF pt2 ;
            PointF pt3 ;
            for (int i = 0; i < Am.GetLength(0); i++)
            {
                m1 = Am[i, 1]; m2 = -Am[i, 3];
                pt1 = new PointF(preLength, m1);
                preLength += El[i];
                pt2 = new PointF(preLength, m2);
                pt1 = cs.Point2D(pt1);
                pt2 = cs.Point2D(pt2);
                g.DrawLine(pt1, pt2);
                if (i < Am.GetLength(0) - 1)
                {
                    mnext = Am[i + 1, 1];
                    if (mnext == m2)
                    {
                        continue;
                    }
                    else
                    {
                        pt3 = new PointF(preLength, mnext);
                        pt3 = cs.Point2D(pt3);
                         g.DrawLine(pt2,pt3);
                    }
                }
            }
            PointF pt4 = new PointF(0, 0);
            PointF pt5 = new PointF(length, 0);
            pt4 = cs.Point2D(pt4);
            pt5 = cs.Point2D(pt5);
             g.DrawLine(pt4, pt5);
            float l1 = 0;
            for (int j = 0; j < nel; j++)
            {
                l1 += El[j];
            }
            float ml1 = -Am[nel - 1, 3];
            g.FillColorOpt = "black";
            PointF pt6 = new PointF(l1, ml1);
            pt6 = cs.Point2D(pt6);
             dim.DrawTextAtAngle(pt6, 0f,string.Format("{0} kNm",Math.Round( ml1,2).ToString()));
            int nelem = Am.GetLength(0);
            float mr1 = -Am[nelem - ner - 1, 3];
            float l2 = 0;
            for (int j = 0; j < nelem - ner; j++)
            {
                l2 += El[j];
            }
            pt6 = new PointF(l2, mr1);
            pt6 = cs.Point2D(pt6);
            g.TextBase = TextBaseline.Bottom;
            g.TextAlignment = TextAlign.Left;
             dim.DrawTextAtAngle(pt6, 0f,string.Format("{0} kNm",Math.Round( mr1,2).ToString()));
            float[] locAndMin = MinAndLocation(mom.ToArray());
            int elno = FindElement((int)locAndMin[0]);
            float m = locAndMin[1];
            l2 = 0;
            for (int j = 0; j <= elno; j++)
            {
                l2 += El[j];
            }
            g.TextBase = TextBaseline.Hanging;
            g.TextAlignment = TextAlign.Right;
            pt6 = new PointF(l2, m);
            pt6 = cs.Point2D(pt6);
             dim.DrawTextAtAngle(pt6, 0f, string.Format("{0} kNm",Math.Round( m,2).ToString()));
        }
        private int FindElement(int n)
        {
            if (EvenOrOdd(n))
            {
                return n / 2;
            }
            else
            {
                return (n - 1) / 2;
            }
        }
        //return true if it is even 
        private bool EvenOrOdd(int n)
        {
            if (n % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
