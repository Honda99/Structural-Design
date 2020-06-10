using System;
using System.Collections.Generic;


namespace EngDolphin.Client.Models
{
    public class FootingData
    {
        public float Width { get; set; }
        public float Length { get; set; }
        public float ColumnWidth { get; set; }
        public float ColumnLength { get; set; }
        public float UltBearingCap { get; set; }
        public float SaftyFactor { get; set; }
        public float Thickness { get; set; }
        public float sMx { get; set; }
        public float sMy { get; set; }
        public float sPn { get; set; }
        public float Mx { get; set; }
        public float My { get; set; }
        public float Pn { get; set; }
        public float ColLocX { get; set; }
        public float ColLocY { get; set; }
        public float Cover { get; set; } = 50;
        public float Dia { get; set; } = 16;
        public FootingData(float width,float height,float thickness){
            Width=width;
            Length=height;
            Thickness = thickness;
        }
        public FootingData(){
            Mx = 10;
            My = 20;
            Pn = 100;
            sMx = 5;
            sMy = 13;
            sPn = 60;
            Width = 300;
            Length = 300;
            ColLocX = 150;
            ColLocY = 150;
            Thickness = 50;
            ColumnWidth = 30;
            ColumnLength = 30;
            UltBearingCap = 250;
            SaftyFactor = 2.3f;
        }
    }
}      
        
