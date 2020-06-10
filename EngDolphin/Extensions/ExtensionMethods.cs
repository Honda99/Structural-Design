using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Extensions
{
    public static class ExtensionMethods
    {
        public static int RoundDown(this int n)
        {
            return (int)Math.Floor(n / 10.0d) * 10;
        }
    }
}
