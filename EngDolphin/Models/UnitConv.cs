using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class UnitConv
    {
        public readonly float kilo = (float)Math.Pow(10, 3);
        public readonly float hecto = (float)Math.Pow(10, 2);
        public readonly float deca = 10f;
        public readonly float mega = (float)Math.Pow(10, 6);
        public readonly float giga = (float)Math.Pow(10, 9);
        public readonly float tera = (float)Math.Pow(10, 12);
        public readonly float micro = (float)Math.Pow(10, -6);
        public readonly float milli = (float)Math.Pow(10, -3);
        public readonly float pico = (float)Math.Pow(10, -12);
        public readonly float nano = (float)Math.Pow(10, -9);
        public readonly float centi = (float)Math.Pow(10, -2);
        public readonly float tetra = (float)Math.Pow(10, -4);
        public readonly float octa = (float)Math.Pow(10, -8);
        public readonly float deci = (float)Math.Pow(10, -1);
        public UnitConv()
        {

        }
    }
}
