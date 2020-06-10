 using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class DovDsgnPreference
    {
        public string DesignCode { get; set; } = "Euro Code";
        public float SaftyFacConc { get; set; } = 1.5f;
        public float SaftyFacRebar { get; set; } = 1.25f;
        public float Alpha { get; set; } = 0.85f;
        public DovDsgnPreference( string designCode,float saftyFacConc,float saftyFacRebar,float alpha){
              DesignCode=designCode;
              SaftyFacConc=saftyFacConc;
              SaftyFacRebar=saftyFacRebar;
              Alpha=alpha;
        }
        public DovDsgnPreference()
        {

        }
    }

}      
        
