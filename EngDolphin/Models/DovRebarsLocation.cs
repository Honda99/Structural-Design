using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class DovRebarLocation:ICloneable
    {
        public int No { get; set; }
        public float Diameter { get; set; }
        public float LocX { get; set; }
        public float LocY { get; set; }
        public bool Activated{get;set;}
        public DovRebarLocation( int no,float diameter,float locX,float locY){
               No=no;
               Diameter=diameter;
               LocX=locX;
               LocY=locY;
        }
        public DovRebarLocation( ){
           
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

}      
        