using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class DovConcMat
    {
        public string Name { get; set; }
        public float UnitWt { get; set; }
        public float Fck { get; set; }
        public float PoissonRatio { get; set; }
        //In MPa
        public float E { get; set; } = 29000;
        public bool Actevated { get; set; }
        public float St { get; set; } = 0.0035f;
        public DovConcMat( string name,float unitWt,float fck,float poissonRatio,float moduElas,float strain){
              Name=name;
              UnitWt=unitWt;
              Fck=fck;
              PoissonRatio=poissonRatio;
              St = strain;
              E = moduElas;
        }
        public DovConcMat(){

        }
    }

}
