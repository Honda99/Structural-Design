using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class DovRebarMat
    {
        public string Name{get;set;}
        public float UnitWt { get; set; }
        public float Fyk { get; set; }
        public float PoissonRatio { get; set; }
        // In MPa
        // private float modulElas;
        public float E { get ; set ; }
        public float St { get;  set; }=0.00207f;
        public bool Actevated { get; set; }

        public DovRebarMat(string name, float unitWt,float fyk,float poissonRatio,float moduElas){
              Name=name;
              UnitWt=unitWt;
              Fyk=fyk;
              PoissonRatio=poissonRatio;
              E = moduElas;
        }
        public DovRebarMat(){}
        
    }

}