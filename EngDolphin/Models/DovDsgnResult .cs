 using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class DovDsgnResult
    {
        public float My { get; set; }
        public float MyConc { get; set; }
        public float MyBottomRebar { get; set; }
        public float MyTopRebar{ get; set; }
        public float CompStress { get; set; }
        public float ConcForce { get; set; }
        public float RebarTesnsileForce { get; set; }
        public float RebarCompForce { get; set; }
        public float NeutralAxisY { get; set; }
        public float NeutralAxisX { get; set; }
        public float TensStrainY { get; set; }
        public float TensStrainX { get; set; }
        public float CompStrain { get; set; }
        public float TensEffDepth { get; set; }
        public float CompEffDepth { get; set; }
        public string DesignInfo { get; set; }
    }

}      
        
