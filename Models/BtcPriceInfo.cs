using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace problem4.Models
{
    public class BtcPriceInfo
    {
        public DateTime UpdatedTime { get; set; }
        public string ChartName { get; set; }
        public float USD_Rate { get; set; }
        public float USD_RATE_FLOAT { get; set; }
    }
}
