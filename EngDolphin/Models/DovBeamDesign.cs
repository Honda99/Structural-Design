using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class DovBeamDesign : DovDsgnResult
    {
        public DovConcMat ConcMat { get; set; }
        public DovRebarMat RebarMat { get; set; }
        public DovSectionRec RecSec { get; set; }
        public DovDsgnPreference DsgnPref { get; set; }
        public const float lamda = 0.8f;
        private readonly float micro = (float)Math.Pow(10, -6);
        private readonly float milli = (float)Math.Pow(10, -3);
        public void DsgnBeamRecSec(IDictionary<float, Stack<DovRebarLocation>> rebarData)
        {
            float st = ConcMat.St;
            float sty = RebarMat.Fyk / RebarMat.E;
            float h = RecSec.Height;
            float b = RecSec.Width;
            this.MyConc = 0;
            this.MyBottomRebar = 0;
            this.MyTopRebar = 0;
            // First estimate by assuming tensile strain is yielded
            float fac = 0.8f;//assumed depth of neutral axis
            //depth of neutral axis
            float c = fac * 0.5f * h;// this one should be updated.
            // new neutral axis from equilibrium of the forces
            IDictionary<float, Stack<DovRebarLocation>> rebars = rebarData;
            // 
            float[] getForce = new float[2];
            float concCompForce = CalcConcForce(b);
            foreach (KeyValuePair<float, Stack<DovRebarLocation>> reb in rebars)
            {
                DovRebarLocation[] rebar = DeleteRepeatedRebar(reb.Value.ToArray());

                for (int i = 0; i < rebar.Length; i++)
                {
                    float[] force = CalcBarForce(rebar[i].LocY, rebar[i].Diameter, c, h);
                    getForce[0] += force[0];
                    getForce[1] += force[1];
                }
            }
            float cforce;
            cforce = (getForce[0] - getForce[1]) / concCompForce;
            float[] effDepth = CalcEffecDepth(rebars, cforce, h);
            this.TensEffDepth = (float)Math.Round(effDepth[0], 3);
            this.CompEffDepth = (float)Math.Round(effDepth[1], 3);
            this.MyConc = (float)Math.Round(CalcConcMoment(cforce, b), 3);
            this.TensStrainY = (float)Math.Round(CalcTensileStrain(cforce, effDepth[0]), 3);
            this.CompStrain = (float)Math.Round(ConcMat.St, 3);

            foreach (KeyValuePair<float, Stack<DovRebarLocation>> reb in rebarData)
            {
                DovRebarLocation[] rebar = DeleteRepeatedRebar(reb.Value.ToArray());

                for (int i = 0; i < rebar.Length; i++)
                {

                    float[] moment = CalcBarMoment(rebar[i].LocY, rebar[i].Diameter, cforce, h, effDepth[0], effDepth[1]);
                    this.MyBottomRebar += moment[0];
                    this.MyTopRebar += moment[1];

                }
            }
            this.My = (float)Math.Round(this.MyConc + this.MyTopRebar + this.MyBottomRebar, 3);
            float[] RebarForce = new float[2];
            foreach (KeyValuePair<float, Stack<DovRebarLocation>> reb in rebars)
            {
                DovRebarLocation[] rebar = DeleteRepeatedRebar(reb.Value.ToArray());

                for (int i = 0; i < rebar.Length; i++)
                {
                    float[] force = CalcBarForce(rebar[i].LocY, rebar[i].Diameter, c, h);
                    RebarForce[0] += force[0];
                    RebarForce[1] += force[1];
                }
            }
            this.RebarTesnsileForce = RebarForce[0];
            this.RebarCompForce = RebarForce[1];
            this.ConcForce = CalcConcForce(cforce, b);
            this.CompStress = CalcConcStress();
            this.NeutralAxisY = (float)Math.Round(cforce, 3);
            if (TensStrainY < sty)
            {
                this.DesignInfo = $"Warning:The tensile strain does not yeilded, yeild strain is {sty} less than design strain {TensStrainY}";
            }
            else
            {
                this.DesignInfo = "Ok";
            }
        }
        private DovRebarLocation[] DeleteRepeatedRebar(DovRebarLocation[] reb)
        {
            DovRebarLocation[] reba = new DovRebarLocation[1];
            int n = 0;
            for (int i = 0; i < reb.Length - 1; i++)
            {
                float x1 = reb[i].LocX;
                bool similar = false;
                for (int j = i + 1; j < reb.Length; j++)
                {
                    float x2 = reb[j].LocX;
                    if (x1 == x2)
                    {
                        similar = true;
                    }
                }
                if (!similar)
                {
                    n++;
                    Array.Resize(ref reba, n);
                    reba[n - 1] = reb[i];
                }

            }
            n++;
            Array.Resize(ref reba, n);
            reba[n - 1] = reb[reb.Length - 1];
            return reba;
        }

        // For Strain Compatibility in kNm
        public float CalcBarMoment(float y, float diam, float c, float h, float d)
        {
            float N = (h * 0.5f - c);
            float area = (float)(Math.PI * Math.Pow((double)diam * 0.5f, 2));
            if (y < N)
            {
                // bottom
                return Math.Abs(micro * ((N - y) * ((N - y) / (d - c)) * ConcMat.St * RebarMat.E * area) / DsgnPref.SaftyFacRebar);
            }
            else
            {

                return Math.Abs(micro * (((y - N) / c) * RebarMat.St * RebarMat.E * area * (y - N)) / DsgnPref.SaftyFacRebar);
            }
        }
        // For moment at neutral axis using effective depth in kNm
        public float[] CalcBarMoment(float y, float diam, float c, float h, float dbot, float dtop)
        {
            float N = (h * 0.5f - c);
            float area = (float)(Math.PI * Math.Pow((double)diam * 0.5f, 2));
            float[] moments = new float[2];
            if (y < N)
            {
                // bottom
                moments[0] = Math.Abs(micro * ((dbot - c) * RebarMat.Fyk * area / DsgnPref.SaftyFacRebar));
                return moments;
            }
            else
            {

                // top
                moments[1] = Math.Abs(micro * ((c - dtop) * ((c - dtop) / c) * ConcMat.St * RebarMat.E * area / DsgnPref.SaftyFacRebar));
                return moments;
            }
        }
        // For Strain Compatibility in kN
        public float[] CalcBarForce(float y, float diam, float c, float h)
        {
            float[] forces = new float[2];
            float N = (h * 0.5f - c);
            float area = (float)(Math.PI * Math.Pow((double)diam * 0.5f, 2));
            if (y < N)
            {
                //bottom
                forces[0] = Math.Abs(milli * RebarMat.Fyk * area / DsgnPref.SaftyFacRebar);
                return forces;
            }
            else
            {
                forces[1] = Math.Abs(milli * RebarMat.Fyk * area / DsgnPref.SaftyFacRebar);
                return forces;
            }
        }
        public float[] CalcEffecDepth(IDictionary<float, Stack<DovRebarLocation>> rebarData, float c, float h)
        {
            float AsyTot = 0;
            float AsTot = 0;
            float AsyTotTop = 0;
            float AsTotTop = 0;
            float dtop = 0;
            float dbottom = 0;
            float N = (h * 0.5f - c);

            foreach (KeyValuePair<float, Stack<DovRebarLocation>> rebars in rebarData)
            {
                DovRebarLocation[] rebar = DeleteRepeatedRebar(rebars.Value.ToArray());

                for (int i = 0; i < rebar.Length; i++)
                {
                    float area = (float)(Math.PI * Math.Pow((double)rebar[i].Diameter * 0.5, 2));
                    if (rebar[i].LocY < N)
                    {
                        // bottom
                        AsTot += area;
                        AsyTot += Math.Abs(area * rebar[i].LocY);
                    }
                    else
                    {
                        // top 
                        AsTotTop += area;
                        AsyTotTop += Math.Abs(area * rebar[i].LocY);
                    }
                }
            }
            float[] effecDepth = new float[2];
            if (AsTot > 0)
            {
                dbottom = AsyTot / AsTot;
                effecDepth[0] = dbottom + h * 0.5f;
            }
            if (AsTotTop > 0)
            {
                dtop = AsyTotTop / AsTotTop;
                effecDepth[1] = h * 0.5f - dtop;
            }
            return effecDepth;
        }
        // For Strain Compatibility in kNm
        public float CalcConcMoment(float c, float b)
        {
            return Math.Abs(micro * (DsgnPref.Alpha * ConcMat.Fck * lamda * c * b) * (c - 0.5f * lamda * c) / DsgnPref.SaftyFacConc);
        }
        // Force due to Conc with out c in kN
        public float CalcConcForce(float b)
        {
            return Math.Abs(milli * (DsgnPref.Alpha * ConcMat.Fck * lamda * b) / DsgnPref.SaftyFacConc);
        }
        public float CalcConcForce(float c, float b)
        {
            return Math.Abs(milli * (DsgnPref.Alpha * ConcMat.Fck * lamda * b * c) / DsgnPref.SaftyFacConc);
        }
        public float CalcConcStress()
        {
            return Math.Abs((DsgnPref.Alpha * ConcMat.Fck * lamda) / DsgnPref.SaftyFacConc);
        }
        public float CalcTensileStrain(float c, float d)
        {
            return (ConcMat.St * (d - c) / c);
        }
    }
}

