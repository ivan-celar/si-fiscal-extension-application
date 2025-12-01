using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Model
{
    internal class BuyerInfoEntryBoxData
    {
        public string BuyerName { get; set; }
        public string VatId { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string ZIP { get; set; }
        public string Country { get; set; }
    }
}
