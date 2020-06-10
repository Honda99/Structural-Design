using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class DovSectionRec
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public DovSectionRec(float width,float height){
            Width=width;
            Height=height;
        }
        public DovSectionRec(){
            
        }
    }
}      
        
