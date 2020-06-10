using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class CombFootAnalysis : UnitConv
    {
        public CombFootingData Data { get; set; }
        public DovConcMat ConcMat { get; set; }
        public DovRebarMat RebMat { get; set; }
        //number of members
        private int M { get; set; } = 16;
        //Number of restraints
        private int Nr { get; set; } = 2;
        //Member end actions
        private int Md { get; set; } = 4;
        //band width
        public int Nb { get; set; } = 4;
        // average stress in kN/m2;
        public float AveStress { get; set; } = 160;
        //number of restrained joints
        private int Nrj { get; set; } = 2;
        //modulus of elasticity
        private float E { get; set; }
        //numbers of joints
        private int Nj { get; set; }
        
        private int Nd { get; set; }
        //number of degree freedoms
        public int N { get; set; }
        public int nel { get; set; } = 3;
        public int ner { get; set; } = 3;
        //element length
        public float[] El;
        // member inertia
        public float[] Zi;
        // joint restraint lists
        public int[] Jrl;
        // joint index number
        public int[] Id;
        // stiffness matrix
        public float[,] Sff;
        //Loads 
        // number of loaded joints
        private int Nlj { get; set; } = 0;
        // number of loaded members
        private int Nlm { get; set; } = 16;
        // joint actions
        private float[] Aj;
        // fixed end actions
        public float[,] Aml;
        //loaded members
        public int[] Lml { get; set; }
        // end actions
        public float[] Ae;
        // total actions
        public float[] Ac;
        // free end joint displacement
        public float[] Dff { get; set; }
        // joint displacement
        public float[] Dj;
        // joint reaction
        public float[] Ar { get; set; }
        // member end actions
        private float[] Am;
        public float [,] AmAllMemeAction{ get; set; }
        // free end actions
        public float[] Acf;
        // Loads
        public CombFootAnalysis():base()
        {
            //members
            Nj = M + 1; Nd = 2 * Nj; N = Nd - Nr;
            Zi = new float[M];
            El = new float[M];
            Jrl = new int[Nd];
        }
        public void Analyze()
        {
            MemberInfo();
            Jrl = JointRestList();
            Id = JointDisIndex();
            MemberAndJointAction();
            EquivaJointLoad();
            CombJointLoad();
             Sff= StiffMatrix();
            Acf = FillAcf();
            BanFac(N,Nb,ref Sff);
            Dff = new float[N];
            BanSol(N,Nb,Sff,Acf,Dff);
            JointDisplacement();
            MemberEndAction();
        }
        public float TransverseBending()
        {
            float l = Data.W1 > Data.W2 ? 0.5f * Data.W1 * centi : 0.5f * Data.W2 * centi;
           // average stress in kN/m2; l in m; per unit width;
            return AveStress * l * l * 0.5f;
        }
        public void MemberInfo()
        {
            // in cm
            float leftCantiLength = 0;
            //in cm
            float rightCantiLength = 0;
            //in cm
            float midLength = 0;
            float w1 = Data.W1;
            float w2 = Data.W2;
            float l = Data.X1 + Data.X2 + Data.X3;
            if (Data.X1 > 0)
            {
                leftCantiLength = Data.X1 / nel;
            }
            if (Data.X3 > 0)
            {
                rightCantiLength = Data.X3 / ner;
            }
            midLength = Data.X2 / (M - nel-ner);

            // Inertia Unit in meter3
            float In = micro * (float)Math.Pow(Data.Thickness, 3) / 12;

            for (int i = 0; i < M; i++)
            {
                float x = 0;
                float bx = 1;
                if (i < nel)
                {
                    x = (i+1) * leftCantiLength;
                    bx = w1 + (x * (w2 - w1) / l);
                    In=micro*centi*bx* (float)Math.Pow(Data.Thickness, 3) / 12;
                    Zi[i] = In;
                    El[i] = leftCantiLength*centi;
                }
               else if (i >= M - ner)
                {
                    float loc2 = Data.X1 + Data.X2;
                    x = loc2+(i-M-ner + 1) * rightCantiLength;
                    bx = w1 + (x * (w2 - w1) / l);
                    In = micro * centi * bx * (float)Math.Pow(Data.Thickness, 3) / 12;
                    Zi[i] = In;
                    El[i] = rightCantiLength*centi;
                }
                else
                {
                    float loc1 = Data.X1 ;
                    x = loc1 + (i - nel + 1) * midLength;
                    bx = w1 + (x * (w2 - w1) / l);
                    In = micro * centi * bx * (float)Math.Pow(Data.Thickness, 3) / 12;
                    Zi[i] = In;
                    El[i] = midLength*centi;
                }
              
            }
            E = ConcMat.E*kilo;
        }
        public void MemberInfoTest()
        {
            El[0] = 100;
            El[1] = 100;
            El[2] = 200;
            Zi[0] = 1000;
            Zi[1] = 2000;
            Zi[2] = 2000;
            E = 10000;
        }
        public int[] JointRestList()
        {
            int[] jrl = new int[Nd];
            //starts from one
            // from joint numbering
            int k = nel;
            int k1 = 2 * k ;
            int k2 = 2 * k+1;
            jrl[k1] = 1;
            jrl[k2] = 0;
            k = M-ner;
            k1 = 2 * k ;
            k2 = 2 * k+1;
            jrl[k1] = 1;
            jrl[k2] = 0;
            return jrl;
        }
        public int[] JointRestListTest()
        {
            int[] jrl = new int[Nd];
            //starts from one
            int k = 0;
            int k1 = 2 * k;
            int k2 = 2 * k + 1;
            jrl[k1] = 1;
            jrl[k2] = 1;
            k = 2;
            k1 = 2 * k;
            k2 = 2 * k + 1;
            jrl[k1] = 1;
            jrl[k2] = 0;
            k = 3;
            k1 = 2 * k;
            k2 = 2 * k + 1;
            jrl[k1] = 1;
            jrl[k2] = 1;
            return jrl;
        }
        public int[] JointDisIndex()
        {
          
            int[] id = new int[Nd];
            int n1 = 0;
            for (int j = 0; j < Nd; j++)
            {
                n1 = n1 + Jrl[j];
                if (Jrl[j] > 0)
                {
                    id[j] = N + n1-1;
                }
                else
                {
                    //marked for error 
                    // modified for i starts from 0;
                    id[j] = j - n1;
                }
            }
            return id;
        }
        public void MemberAndJointAction()
        {
            
            Aj = new float[Nd];
            Aml = new float[4,M];
            Lml = new int[M];

            // in cm
            float leftCantiLength = 0;
            //in cm
            float rightCantiLength = 0;
            //in cm
            float midLength = 0;
            float w1 = Data.W1;
            float w2 = Data.W2;
            float l = Data.X1 + Data.X2 + Data.X3;
            if (Data.X1 > 0)
            {
                leftCantiLength = Data.X1 / nel;
            }
            if (Data.X3 > 0)
            {
                rightCantiLength = Data.X3 / ner;
            }
            midLength = Data.X2 / (M - nel - ner);
            // Bearing Stress in KN/m2;
            float bearStress = AveStress;
            // left fixed end moment
          
            
           
            for (int j = 0; j < M ; j++)
            {
                float wld1 =0;
                float wld2 = 0;
                float s1 = 0;
                float s2 = 0;
                float s3 = 0;

                float x = 0;
                float bx = 1;
                float[] actions = new float[4] ;
                if (j < nel)
                {
                    x = (j ) * leftCantiLength;// cm
                    bx = w1 + (x * (w2 - w1) / l);// cm
                    wld1 = centi * bx * bearStress;//kN/m
                    x = (j+1) * leftCantiLength;// cm
                    bx = w1 + (x * (w2 - w1) / l);// cm
                    wld2 = centi * bx * bearStress;//kN/m
                    s2 =  leftCantiLength*centi;
                    actions = TpzLdEndAct(wld1, wld2, s1, s2, s3);
                    Aml[0, j] = actions[0]; Aml[1, j] = actions[1]; Aml[2, j] = actions[2]; Aml[3, j] = actions[3];
                    Lml[j] = 1;

                }
                else if (j >= M - ner)
                {
                    float loc2 = Data.X1 + Data.X2;//cm
                    x = loc2 + (j - M - ner) * rightCantiLength;//cm
                    bx = w1 + (x * (w2 - w1) / l);
                    wld1 = centi * bx * bearStress;//kN/m
                    x = loc2+(j - M - ner + 1) * rightCantiLength;// cm
                    bx = w1 + (x * (w2 - w1) / l);// cm
                    wld2 = centi * bx * bearStress;//kN/m
                    s2 = rightCantiLength * centi;
                    actions = TpzLdEndAct(wld1, wld2, s1, s2, s3);
                    Aml[0, j] = actions[0]; Aml[1, j] = actions[1]; Aml[2, j] = actions[2]; Aml[3, j] = actions[3];
                    Lml[j] = 1;
                }
                else
                {
                    float loc1 = Data.X1;
                    x = loc1 + (j - nel) * midLength;
                    bx = w1 + (x * (w2 - w1) / l);
                    wld1 = centi * bx * bearStress;//kN/m
                    x = loc1 + (j - nel + 1) * midLength;// cm
                    bx = w1 + (x * (w2 - w1) / l);// cm
                    wld2 = centi * bx * bearStress;//kN/m
                    s2 = midLength * centi;
                    actions = TpzLdEndAct(wld1, wld2, s1, s2, s3);
                    Aml[0, j] = actions[0]; Aml[1, j] = actions[1]; Aml[2, j] = actions[2]; Aml[3, j] = actions[3];
                    Lml[j] = 1;
                }
               
            }

        }
        //Trapizoidal load end actions of fixed beam ; a,b,c: s1,s2,s3; w1,w2 first and second load value;
        //reference from eng-tips.com/
        private float[] TpzLdEndAct(float w1,float w2,float s1,float s2,float s3)
        {
            float mr1 = (-1f / 60f) * w2 * s2 * ((3 * s2 * s2 * s2 + 15 * s2 * s2 * s3 + 10 * s1 * s1 * s2 + 30 * s1 * s1 * s3 + 10 * s2 * s2 * s1 + 40 * s2 * s1 * s3) / ((s1 + s2 + s3) * (s1 + s2 + s3)));
            float mr2 = (-1f / 60f) * w1 * s2 * ((2 * s2 * s2 * s2 + 5 * s2 * s2 * s3 + 20 * s1 * s1 * s2 + 30 * s1 * s1 * s3 + 10 * s2 * s2 * s1 + 20 * s2 * s1 * s3) / ((s1 + s2 + s3) * (s1 + s2 + s3)));
            float mr = mr1 + mr2;
            float vr1 = (1f / 20f) * w1 * s2 * (3 * s2 * s2 * s2 + 5 * s2 * s2 * s3 + 10 * s1 * s1 * s1 + 30 * s1 * s1 * s2 + 30 * s1 * s1 * s3 + 15 * s2 * s2 * s1 + 20 * s1 * s2 * s3) / ((s1 + s2 + s3)*(s1 + s2 + s3) * (s1 + s2 + s3));
            float vr2 = (1f / 20f) * w2 * s2 * (7 * s2 * s2 * s2 + 15 * s2 * s2 * s3 + 10 * s1 * s1 * s1 + 30 * s1 * s1 * s2 + 30 * s1 * s1 * s3 + 25 * s2 * s2 * s1 + 40 * s1 * s2 * s3) / ((s1 + s2 + s3) * (s1 + s2 + s3)* (s1 + s2 + s3));
            float vr = vr1 + vr2;
            float ml1= (1f / 60f) * w1 * s2 * ((3 * s2 * s2 * s2 + 15 * s2 * s2 * s1 + 10 * s3 * s3 * s2 + 30 * s3 * s3 * s1 + 10 * s2 * s2 * s3 + 40 * s2 * s3 * s1) / ((s1 + s2 + s3) * (s1 + s2 + s3)));
            float ml2 = (1f / 60f) * w2 * s2 * ((2 * s2 * s2 * s2 + 5 * s2 * s2 * s1 + 20 * s3 * s3 * s2 + 30 * s3 * s3 * s1 + 10 * s2 * s2 * s3 + 20 * s2 * s3 * s1) / ((s1 + s2 + s3) * (s1 + s2 + s3)));
            float ml = ml1 + ml2;
            float vl1 = (1f / 20f) * w2 * s2 * (3 * s2 * s2 * s2 + 5 * s2 * s2 * s1 + 10 * s3 * s3 * s3 + 30 * s3 * s3 * s2 + 30 * s3 * s3 * s1 + 15 * s2 * s2 * s3 + 20 * s1 * s2 * s3) / ((s1 + s2 + s3) * (s1 + s2 + s3) * (s1 + s2 + s3));
            float vl2 = (1f / 20f) * w1 * s2 * (7 * s2 * s2 * s2 + 15 * s2 * s2 * s1 + 10 * s3 * s3 * s3 + 30 * s3 * s3 * s2 + 30 * s3 * s3 * s1 + 25 * s2 * s2 * s3 + 40 * s1 * s2 * s3) / ((s1 + s2 + s3) * (s1 + s2 + s3) * (s1 + s2 + s3));
            float vl = vl1 + vl2;
            return new float[] { vl, ml, vr, mr };
        }
        public void MemberAndJointActionTest()
        {
            // Bearing Stress in KN/m2;
            Aj = new float[Nd];
            Aml = new float[4, M];
            Lml = new int[M];
           
            Aml[0, 0] = 10; Aml[1, 0] = 250; Aml[2, 0] = 10; Aml[3, 0] = -250;
            Lml[0] = 1;
            Aml[0, 1] = 10; Aml[1, 1] = 250; Aml[2, 1] = 10; Aml[3, 1] = -250;
            Lml[1] = 1;
            Aml[0,2] = 10; Aml[1, 2] = 333.333f; Aml[2, 2] = 10; Aml[3, 2] = -333.333f;
            Lml[2] = 1;
            Aj[2] = -10;Aj[3] = 1000;
            Aj[4] = -10;
           
           

        }
        public void EquivaJointLoad()
        {
            Ae = new float[Nd];
            for (int i = 0; i < M; i++)
            {
                if (Lml[i] == 0)
                {
                    continue;
                }
                else
                {
                    Ae[2 * i] -= Aml[0, i];
                    Ae[2 * i + 1] -= Aml[1, i];
                    Ae[2 * i + 2] -= Aml[2, i];
                    Ae[2 * i + 3] -= Aml[3, i];
                }
            }
        }
        public void CombJointLoad()
        {
            Ac = new float[Nd];
            for (int j = 0; j < Nd; j++)
            {
                //suspected
                int Jr = Id[j] ;
                Ac[Jr] = Ae[j] + Aj[j];
            }
        }
        public float[,] StiffMatrix()
        {

            int j, k, i, i1, i2, ir, ic, item;
            float scm1, scm2, scm3;
            int[] im = new int[Md];
            float[,] Sm = new float[Md, Nb];
            float[,] sff = new float[N, Nb];
            for (i = 0; i < M; i++)
            {
                scm1 = (4 * E * Zi[i]) / El[i];
                scm2 = (1.5f * scm1) / El[i];
                scm3 = (2 * scm2) / El[i];

                Sm[0, 0] = scm3; Sm[0, 1] = scm2;
                Sm[0, 2] = -scm3; Sm[0, 3] = scm2;
                Sm[1, 1] = scm1; Sm[1, 2] = -scm2;
                Sm[1, 3] = scm1 / 2; Sm[2, 2] = scm3;
                Sm[2, 3] = -scm2; Sm[3, 3] = scm1;
                im[0] = 2 * i; im[1] = 2 * i + 1;
                im[2] = 2 * i + 2; im[3] = 2 * i + 3;
                for (j = 0; j < Md; j++)
                {
                    i1 = im[j];
                    if (Jrl[i1] > 0)
                    {
                        continue;
                    }
                    else
                    {
                        for (k = j; k < Md; k++)
                        {
                            i2 = im[k];
                            if (Jrl[i2] > 0)
                            {
                                continue;
                            }
                            else
                            {
                                //suspected
                                ir = Id[i1] ; ic = Id[i2] ;
                                if (ir < ic)
                                {
                                    ic = ic - ir ;

                                }
                                else
                                {
                                    item = ir;
                                    ir = ic;
                                    ic = item;
                                    ic = ic - ir ;
                                }
                            }
                            sff[ir, ic] += Sm[j, k];
                        }
                    }
                }
            }
            return sff;

        }
        public float[] FillAcf()
        {
            float[] acf = new float[N];
            for (int j = 0; j < N; j++)
            {
                acf[j] = (float)Ac[j];
            }
            return acf;
        }
        public void JointDisplacement()
        {
            //initialize
            Dj = new float[Nd];
            int j, k, je;
            j = N ;
            for (k = 0; k < Nd; k++)
            {
                //changed
                je = Nd - k - 1;
                if (Jrl[je] == 0)
                {
                    // Dff not initialized;
                    j -= 1; Dj[je] = Dff[j];
                }
                else
                {
                    Dj[je] = 0;
                }
            }
        }
        public void MemberEndAction()
        {
            int i = 0, j =0, k = 0, i1 = 0,
            i2 = 0; float scm1, scm2, scm3; float[] Amc = new float[Md];
            int[] im = new int[Md]; float[,] Sm = new float[Md, Nb];
            Am = new float[Md]; float[] Dm = new float[Md]; Ar = new float[Nd];
            float[] Amd = new float[Md];
            AmAllMemeAction = new float[M, Md];
            for (i = 0; i < M; i++)
            {
                scm1 = (4 * E * (float)Zi[i]) / (float)El[i];
                scm2 = (1.5f * scm1) / (float)El[i];
                scm3 = (2 * scm2) / (float)El[i];
                Sm[0, 0] = scm3; Sm[0, 1] = scm2;
                Sm[0, 2] = -scm3; Sm[0, 3] = scm2;
                Sm[1, 1] = scm1; Sm[1, 2] = -scm2;
                Sm[1, 3] = scm1 / 2; Sm[2, 2] = scm3;
                Sm[2, 3] = -scm2; Sm[3, 3] = scm1;
                Sm[1, 0] = scm2; Sm[2, 0] = -scm3;
                Sm[3, 0] = scm2; Sm[2, 1] = -scm2;
                Sm[3, 1] = scm1 / 2; Sm[3, 2] = -scm2;
                im[0] = 2 * i; im[1] = 2 * i + 1;
                im[2] = 2 * i + 2; im[3] = 2 * i + 3;
                for (j = 0; j < Md; j++)
                {
                    Amd[j] = 0;
                    for (k = 0; k < Md; k++)
                    {
                        i1 = im[k]; Dm[k] = Dj[i1];
                        Amd[j] += (Sm[j, k] * Dm[k]);
                    }
                    Am[j] =(float)Aml[j,i]+ Amd[j];
                    AmAllMemeAction[i, j] = Am[j];
                    i2 = im[j];
                    if (Jrl[i2] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        Ar[i2] += Amd[j];
                    }
                }

            }
            for (j = 0; j < Nd; j++)
            {
                if (Jrl[j] == 0)
                {
                    continue;
                }
                else
                {
                    Ar[j] = Ar[j]-(float)Aj[j] - (float)Ae[j];
                }
            }
        }
        public void BanFac(int n, int nb, ref float[,] A)
        {
            int j1 = 0, j2 = 0, j = 0, k = 0, i = 0, i1 = 0; float SUM = 0, TEMP = 0;
            if (A[0, 0] <= 0)
            {
                throw new Exception("matrix can not be factorized.");
            }
            for (j = 1; j < N; j++)
            {
                j1 = j - 1; j2 = j - nb + 1;
                if (j2 < 0) j2 = 0;
                if (j1 == 0) goto next;
                for (i = 1; i <= j1; i++)
                {
                    i1 = i - 1;
                    if (i1 < j2) continue;
                    //to be checked
                    SUM = A[i, j - i];
                    for (k = j2; k <= i1; k++)
                    {
                        SUM = SUM - A[k, i - k ] * A[k, j - k ];
                    }
                    A[i, j - i ] = SUM;
                }
            next: SUM = A[j, 0];
                for (k = j2; k <= j1; k++)
                {
                    TEMP = A[k, j - k ] / A[k, 0];
                    SUM = SUM - (TEMP * A[k, j - k ]);
                    A[k, j - k ] = TEMP;
                }
                if (SUM <= 0) return; A[j, 0] = SUM;
            }
        }
        public void BanSol(int n,int nb, float[,] U, float[] B, float[] X)
        {
            int i = 0, i1 = 0, j = 0, k = 0, k1 = 0, k2 = 0;float SUM = 0;
            for (i = 0; i < N; i++)
            {
                j= i - nb + 1;
                if(i < nb) j = 0;
                SUM = B[i]; k1 = i - 1;
                if (j > k1)
                {
                    X[i] = SUM; continue;
                }
                else
                {
                    for(k = j; k <= k1; k++)
                    {
                        SUM = SUM - U[k, i - k ] * X[k];
                    }
                    X[i] = SUM;

                }
            }
            for(i = 0; i <n; i++)
            {
                X[i] = X[i] / U[i, 0];
            }
            for(i1 = 0; i1 < n; i1++)
            {
                i = n - i1-1 ; j = i + nb - 1;
                if(j >= n) j = n-1;
                SUM = X[i]; k2 = i + 1;
                if (k2 > j)
                {
                    X[i] = SUM;continue;
                }
                else
                {
                    for (k = k2; k <= j; k++)
                    {
                        SUM = SUM - U[i, k - i] * X[k];
                        X[i] = SUM;
                    }
                }
              
            }
        }
    }
}
