using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class DovColumnDesign : DovDsgnResult
    {
        public DovConcMat ConcMat { get; set; }
        public DovRebarMat RebarMat { get; set; }
        public DovSectionRec RecSec { get; set; }
        public DovDsgnPreference DsgnPref { get; set; }
        public int NumberOfPoints { get; set; } = 20;
        public const float lamda = 0.8f;
        private readonly float micro = (float)Math.Pow(10, -6);
        private readonly float milli = (float)Math.Pow(10, -3);
        public float[] ColumnAxialCapacity { get; set; }
        public float[] ColumnBendingCapacity { get; set; }
        public Dictionary<float, List<float[]>> BiaxialChartTable { get; set; }
        public List<float[]> BiaxialTableatAngle { get; set; }
        public float lowestBar { get; set; }
        public DovColumnDesign(DovConcMat conc, DovRebarMat reb, DovSectionRec rec, DovDsgnPreference pref)
        {
            ConcMat = conc;
            RebarMat = reb;
            RecSec = rec;
            DsgnPref = pref;
            BiaxialTableatAngle = new List<float[]>();
        }
        public DovColumnDesign()
        {
            ColumnAxialCapacity = new float[NumberOfPoints];
            ColumnBendingCapacity = new float[NumberOfPoints];
        }
        public void DsgnColumnUniaxialInter(IDictionary<float, Stack<DovRebarLocation>> rebarData)
        {
            float h = RecSec.Height;
            float b = RecSec.Width;
            float Ast = 0;
            float yieldStrain = RebarMat.Fyk / (RebarMat.E * DsgnPref.SaftyFacRebar);
            float compAxialCapacity = 0;
            float tensAxialCapacity = 0;
            float[] NumberOfPoint = new float[] { 0.5f * yieldStrain, 0.25f * yieldStrain, 0 * yieldStrain, -1 * yieldStrain, -1.75f * yieldStrain, -2.1875f * yieldStrain, -4 * yieldStrain, -6 * yieldStrain };
            ColumnAxialCapacity = new float[NumberOfPoint.Length + 2];
            ColumnBendingCapacity = new float[NumberOfPoint.Length + 2];
            // First estimate assuming tensile strain is yielded
            float c;
            IDictionary<float, Stack<DovRebarLocation>> rebars = rebarData;
            // new neutral axis from equilibrium of the forces
            // this.TensStrainX=c;

            for (int k = 0; k < NumberOfPoint.Length; k++)
            {
                float[] RebarForce = new float[2];
                float[] BendingMoment = new float[2];
                c = Math.Abs((ConcMat.St / (ConcMat.St - NumberOfPoint[k])) * (Math.Abs(lowestBar) + 0.5f * h));
                foreach (KeyValuePair<float, Stack<DovRebarLocation>> reb in rebarData)
                {
                    DovRebarLocation[] rebar = DeleteRepeatedRebar(reb.Value.ToArray());

                    for (int i = 0; i < rebar.Length; i++)
                    {

                        if (k == 0) Ast += (float)Math.PI * (float)Math.Pow((float)rebar[i].Diameter, 2) / 4;
                        float[] moment = CalcInterBarMoment(rebar[i].LocY, rebar[i].Diameter, c, h);
                        BendingMoment[0] += moment[0];//bottom
                        BendingMoment[1] += moment[1];//top
                        float[] force = CalcInterBarForce(rebar[i].LocY, rebar[i].Diameter, c, h);
                        RebarForce[0] += force[0];
                        RebarForce[1] += force[1];
                    }
                }
                ColumnAxialCapacity[k + 1] = RebarForce[1] - RebarForce[0] + CalcInterConcForce(c, b, h);
                ColumnBendingCapacity[k + 1] = BendingMoment[0] + BendingMoment[1] + CalcInterConcMoment(c, b, h);
            }
            compAxialCapacity = (milli * DsgnPref.Alpha * ConcMat.Fck * (b * h - Ast) / DsgnPref.SaftyFacConc) + (milli * RebarMat.Fyk * Ast / DsgnPref.SaftyFacRebar);
            tensAxialCapacity = -(milli * RebarMat.Fyk * Ast / DsgnPref.SaftyFacRebar);
            ColumnAxialCapacity[0] = compAxialCapacity;
            ColumnAxialCapacity[NumberOfPoint.Length + 1] = tensAxialCapacity;
        }
        public void DsgnColumnBiaxialInter(IDictionary<float, Stack<DovRebarLocation>> rebarData, float? atAngle = null)
        {
            BiaxialChartTable = new Dictionary<float, List<float[]>>();
            float h = RecSec.Height;
            float b = RecSec.Width;
            float limitAngle = 180 * (float)Math.Atan(h / b) / (float)Math.PI;
            float compAxialCapacity = 0;
            float tensAxialCapacity = 0;
            float[] NumberOfPoint = new float[] { 1, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f };
            float cincl;
            float ac;
            float d1;
            double[] angle = new double[] { 0, 15, 30, 45, 60, 65, 75, 90 };
            if (atAngle.HasValue) angle = new double[] { (double)atAngle };
            IDictionary<float, Stack<DovRebarLocation>> rebars = rebarData;
            float Ast = 0;
            foreach (KeyValuePair<float, Stack<DovRebarLocation>> reb in rebarData)
            {
                DovRebarLocation[] rebar =DeleteRepeatedRebar( reb.Value.ToArray());
                for (int i = 0; i < rebar.Length; i++)
                {
                    Ast += (float)Math.PI * (float)Math.Pow((float)rebar[i].Diameter, 2) / 4;
                }
            }
            for (int m = 0; m < angle.Length; m++)
            {

                float[] ColumnBiaxialCapacity = new float[NumberOfPoint.Length + 2];
                float[] ColumnBendingCapacityX = new float[NumberOfPoint.Length + 2];
                float[] ColumnBendingCapacityY = new float[NumberOfPoint.Length + 2];
                for (int k = 0; k < NumberOfPoint.Length; k++)
                {
                    float RebarForce = 0;
                    float BendingX = 0;
                    float BendingY = 0;
                    d1 = NumberOfPoint[k] * h;
                    if (angle[m] >= limitAngle) d1 = NumberOfPoint[k] * b;
                    float angleRad = (float)(angle[m] * Math.PI / 180);
                    this.NeutralAxisY = angleRad;
                    cincl = d1 * (float)Math.Cos(angleRad);
                    if (angle[m] >= limitAngle) cincl = d1 * (float)Math.Sin(angleRad);
                    float x0 = -0.5f * b;
                    if (angle[m] >= limitAngle) x0 = d1 - (0.5f * b);
                    float x = 0;
                    float y0 = 0.5f * h - d1;
                    if (angle[m] >= limitAngle) y0 = 0.5f * h;
                    float yna = ((float)Math.Tan(angleRad) * (x - x0)) + y0;
                    ac = lamda * cincl;
                    float d3 = ac / (float)Math.Cos(angleRad);
                    if (angle[m] >= limitAngle) d3 = ac / (float)Math.Sin(angleRad);
                    x0 = -0.5f * b;
                    if (angle[m] >= limitAngle) x0 = d3 - (0.5f * b);
                    y0 = 0.5f * h - d3;
                    if (angle[m] >= limitAngle) y0 = 0.5f * h;
                    x = 0.5f * b;
                    float y = 0;
                    if (angle[m] >= limitAngle) y = -0.5f * h;
                    float y3 = (float)Math.Tan(angleRad) * (x - x0) + y0;
                    if (angle[m] >= limitAngle) y3 = x0 + ((y - y0) / (float)Math.Tan(angleRad));
                    float d4 = 0.5f * h - y3;
                    if (angle[m] >= limitAngle) d4 = 0.5f * b + y3;
                    this.My = yna;
                    this.MyConc = d3;
                    this.MyBottomRebar = d4;
                    this.MyTopRebar = y3;
                    this.NeutralAxisX = x0;
                    foreach (KeyValuePair<float, Stack<DovRebarLocation>> reb in rebarData)
                    {
                        DovRebarLocation[] rebar = DeleteRepeatedRebar(reb.Value.ToArray());

                        for (int i = 0; i < rebar.Length; i++)
                        {
                            float[][] moment = CalcBiaxialBarMoment(rebar[i].LocX, rebar[i].LocY, yna, rebar[i].Diameter, cincl, angleRad);
                            BendingY += (moment[0][1] + moment[1][1]);
                            BendingX += (moment[0][0] + moment[1][0]);
                            float[] force = CalcBiaxialBarForce(rebar[i].LocX, rebar[i].LocY, yna, rebar[i].Diameter, cincl, angleRad);
                            RebarForce += force[0] + force[1];
                        }
                    }
                    ColumnBiaxialCapacity[k + 1] = RebarForce + (angle[m] < limitAngle ? CalcBiaxialConcForce(y0, y3, d3, d4, b) : CalcBiaxialConcForce(x0, y3, d3, d4, h));
                    ColumnBendingCapacityX[k + 1] = BendingX + (angle[m] < limitAngle ? CalcBiaxialConcMoment(y0, y3, d3, d4, b, angle[m] > limitAngle)[0]
                    : CalcBiaxialConcMoment(x0, y3, d3, d4, h, angle[m] > limitAngle)[1]);
                    ColumnBendingCapacityY[k + 1] = BendingY + (angle[m] < limitAngle ? CalcBiaxialConcMoment(y0, y3, d3, d4, b, angle[m] > limitAngle)[1]
                        : CalcBiaxialConcMoment(x0, y3, d3, d4, h, angle[m] > limitAngle)[0]);

                }
                compAxialCapacity = (milli * DsgnPref.Alpha * ConcMat.Fck * (b * h - Ast) / DsgnPref.SaftyFacConc) + (milli * RebarMat.Fyk * Ast / DsgnPref.SaftyFacRebar);
                tensAxialCapacity = -(milli * RebarMat.Fyk * Ast / DsgnPref.SaftyFacRebar);
                ColumnBiaxialCapacity[0] = compAxialCapacity;
                ColumnBiaxialCapacity[NumberOfPoint.Length + 1] = tensAxialCapacity;
                List<float[]> data = new List<float[]>();
                data.Add(ColumnBendingCapacityX);
                data.Add(ColumnBendingCapacityY);
                data.Add(ColumnBiaxialCapacity);
                if (atAngle.HasValue)
                {
                    BiaxialTableatAngle = data;
                }
                else
                {
                    BiaxialChartTable[m] = data;
                }
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
        // For moment at neutral axis using effective depth in kNm
        public float[] CalcInterBarMoment(float y, float diam, float c, float h)
        {
            float N = (h * 0.5f - c);
            float area = (float)(Math.PI * Math.Pow((double)diam * 0.5f, 2));
            float[] moments = new float[2];
            if (y < N)
            {
                float strain = ((N - y) / c) * ConcMat.St;
                float ey = RebarMat.Fyk / (RebarMat.E * DsgnPref.SaftyFacRebar);
                if (strain > ey) strain = ey;
                // bottom
                if (y > 0)
                {

                    moments[0] = -Math.Abs(micro * strain * RebarMat.E * area * y);
                }
                else
                {
                    moments[0] = Math.Abs(micro * strain * RebarMat.E * area * y);
                }

                return moments;
            }
            else
            {
                float fsi = ((y - N) / c) * ConcMat.St * RebarMat.E / DsgnPref.SaftyFacRebar;
                if (fsi > (RebarMat.Fyk / DsgnPref.SaftyFacRebar)) fsi = RebarMat.Fyk / DsgnPref.SaftyFacRebar;
                if ((0.5f * h - y) < lamda * c) fsi -= DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;
                // top
                if (y < 0)
                {
                    moments[1] = -Math.Abs(micro * fsi * area * y);
                }
                else
                {
                    moments[1] = Math.Abs(micro * fsi * area * y);
                }

                return moments;
            }
        }
        // For Strain Compatibility in kN
        public float[] CalcInterBarForce(float y, float diam, float c, float h)
        {
            float[] forces = new float[2];
            float N = (h * 0.5f - c);
            float area = (float)(Math.PI * Math.Pow((double)diam * 0.5f, 2));
            if (y < N)
            {
                float strain = ((N - y) / c) * ConcMat.St;
                float ey = RebarMat.Fyk / (RebarMat.E * DsgnPref.SaftyFacRebar);
                if (strain > ey) strain = ey;
                //bottom
                forces[0] = Math.Abs(milli * strain * RebarMat.E * area);
                return forces;
            }
            else
            {
                float fsi = ((y - N) / c) * ConcMat.St * RebarMat.E / DsgnPref.SaftyFacRebar;
                if (fsi > (RebarMat.Fyk / DsgnPref.SaftyFacRebar)) fsi = RebarMat.Fyk / DsgnPref.SaftyFacRebar;
                if ((0.5f * h - y) < lamda * c) fsi -= DsgnPref.Alpha * ConcMat.Fck;
                forces[1] = Math.Abs(milli * fsi * area / DsgnPref.SaftyFacRebar);
                return forces;
            }
        }
        // For Strain Compatibility in kNm
        public float CalcInterConcMoment(float c, float b, float h)
        {
            if (c >= h)
            {
                return 0;
            }
            return Math.Abs(micro * (DsgnPref.Alpha * ConcMat.Fck * lamda * c * b) * (0.5f * h - 0.5f * lamda * c) / DsgnPref.SaftyFacConc);
        }
        // Force due to Conc with out c in kN

        public float CalcInterConcForce(float c, float b, float h)
        {
            if (c > h)
            {
                return milli * b * h * DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;
            }
            else
            {
                return Math.Abs(milli * (DsgnPref.Alpha * ConcMat.Fck * lamda * b * c) / DsgnPref.SaftyFacConc);
            }
        }
        public float[] CalcBiaxialConcMoment(float y0, float y3, float d3, float d4, float b, bool top)
        {
            float[] moments = new float[2];
            if (d4 <= 0)
            {
                float d44 = Math.Abs(d4);
                float x = (d44 * b) / (d3 + d44);
                float area = (b - x) * d3 * 0.5f;
                float force = milli * area * DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;

                float bendy = milli * force * ((2 * d3 / 3) + y0);
                if (top) bendy = -milli * force * ((2 * d3 / 3) - y0);
                float bendx = milli * force * (-(b + (2 * x)) / 6);
                if (top) bendx = milli * force * ((b + (2 * x)) / 6);
                moments[0] = bendx;
                moments[1] = bendy;
                return moments;
            }
            else
            {
                float area = b * d4;

                float force = milli * area * DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;
                float bendy = milli * force * ((0.5f * d4) + y3);
                if (top) bendy = -milli * force * ((0.5f * d4) - y3);
                float bendx = 0;
                area = 0.5f * (d3 - d4) * b;
                force = milli * area * DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;


                if (top)
                {
                    bendy -= milli * force * (((2 * (d3 - d4)) / 3) - y0);
                }
                else
                {
                    bendy += milli * force * (((2 * (d3 - d4)) / 3) + y0);
                }

                if (top)
                {
                    bendx += milli * force * (b / 6);
                }
                else
                {
                    bendx += milli * force * (-b / 6);
                }
                moments[0] = bendx;
                moments[1] = bendy;
                return moments;
            }
        }
        public float CalcBiaxialConcForce(float y0, float y3, float d3, float d4, float b)
        {
            if (d4 <= 0)
            {
                float d44 = Math.Abs(d4);
                float x = (d44 * b) / (d3 + d44);
                float area = (b - x) * d3 * 0.5f;
                float force = milli * area * DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;
                return force;
            }
            else
            {
                float area = b * d4;
                float force = milli * area * DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;
                area = 0.5f * (d3 - d4) * b;
                force += milli * area * DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc;
                return force;
            }
        }
        public float[] CalcBiaxialBarForce(float x, float y, float yna, float diam, float cinc, float angle)
        {
            float[] forces = new float[2];
            float pi = (y - yna) * (float)Math.Cos(angle) - x * (float)Math.Sin(angle);
            float strain = pi * ConcMat.St / cinc;
            float yieldStrain = RebarMat.Fyk / (RebarMat.E * DsgnPref.SaftyFacRebar);
            if (Math.Abs(strain) > yieldStrain) strain = (strain / Math.Abs(strain)) * yieldStrain;
            float area = (float)Math.PI * (float)Math.Pow(0.5f * diam, 2);

            if (pi > 0)
            {

                forces[0] = milli * strain * RebarMat.E * area;
                if (pi > (1 - lamda) * cinc) forces[0] = milli * (strain * RebarMat.E - (DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc)) * area;
                return forces;
            }
            else
            {
                forces[1] = milli * strain * RebarMat.E * area;
                return forces;
            }
        }
        public float[][] CalcBiaxialBarMoment(float x, float y, float yna, float diam, float cinc, float angle)
        {
            float[][] moment = new float[2][];
            moment[0] = new float[2];
            moment[1] = new float[2];
            float pi = (y - yna) * (float)Math.Cos(angle) - x * (float)Math.Sin(angle);
            float strain = pi * ConcMat.St / cinc;
            float yieldStrain = RebarMat.Fyk / (RebarMat.E * DsgnPref.SaftyFacRebar);
            if (Math.Abs(strain) > yieldStrain) strain = (strain / Math.Abs(strain)) * yieldStrain;
            float area = (float)Math.PI * (float)Math.Pow(0.5f * diam, 2);
            if (pi > 0)
            {
                float force;
                force = milli * strain * RebarMat.E * area;
                if (pi > (1 - lamda) * cinc) force = milli * ((strain * RebarMat.E) - (DsgnPref.Alpha * ConcMat.Fck / DsgnPref.SaftyFacConc)) * area;
                moment[0][0] = milli * force * x;
                moment[0][1] = milli * force * y;
                return moment;
            }
            else
            {
                float force;
                force = milli * strain * RebarMat.E * area;
                moment[1][0] = milli * force * x;
                moment[1][1] = milli * force * y;
                return moment;
            }
        }
    }
}
