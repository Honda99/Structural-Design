using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class CombFootingData
    {
        public float W1 { get; set; } = 200;
        public float W2 { get; set; } = 200;
        public float X1 { get; set; } = 100;
        public float X2 { get; set; } = 500;
        public float X3 { get; set; } = 150;
        public float Cw1 { get; set; } = 30;
        public float Cl1 { get; set; } = 30;
        public float UltBearingCap { get; set; }
        public float SaftyFactor { get; set; }
        public float Thickness { get; set; }
        public FootingLoads[] Loads {get;set ;}
        public float Cw2 { get; set; } = 30;
        public float Cl2 { get; set; } = 30;
        public float Cover { get; set; } = 50;
        public float Dia { get; set; } = 16;
       
        public CombFootingData(){
            Loads = new FootingLoads[2];
            Loads[0] = new FootingLoads();
            Loads[1] = new FootingLoads() { MxName = "Mx 2", MyName = "My 2", PnName = "Pn 2", sMxName = "Mx 2", sMyName = "My 2", sPnName = "Pn 2" };
            Thickness = 50;
            UltBearingCap = 250;
            SaftyFactor = 2.3f;
        }
    }
}      
        
