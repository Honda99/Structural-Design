using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EngDolphin.Canvas
{
    public class CanvasEnum
    {
        public enum TextAlign
        {
            Start,
            End,
            Left,
            Right,
            Center
        }

        public enum TextBaseline
        {
            Alphabetic,
            Top,
            Hanging,
            Middle,
            Ideographic,
            Bottom
        }

        public enum TextDirection
        {
            Inherit,
            LTR,
            RTL
        }

        public enum LineCap
        {
            Butt,
            Round,
            Square
        }

        public enum LineJoin
        {
            Miter,
            Round,
            Bevel
        }
    }
}
