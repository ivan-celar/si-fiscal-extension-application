using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Model
{
    public class BuyerInfoInput
    {
        public string name { get; set; }
        public string vatId { get; set; }
        public string address { get; set; }
        public string town { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public bool active { get; set; }
    }
}
