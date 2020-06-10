using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngDolphin.Client.Models
{
    public class FootingLoads
    {
        public float sMx { get; set; } = 20;
        public float sMy { get; set; } = 25;
        public float sPn { get; set; } = 550;
        public float Mx { get; set; } = 30;
        public float My { get; set; } = 40;
        public float Pn { get; set; } = 700;
        public string sMxName { get; set; } = "Mx 1";
        public string sMyName { get; set; } = "My 1";
        public string sPnName { get; set; } = "Pn 1";
        public string MxName { get; set; } = "Mx 1";
        public string MyName { get; set; } = "My 1";
        public string PnName { get; set; } = "Pn 1";
        public FootingLoads()
        {

        }
    }
}
