using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Dtos
{
    public class ReferenceRequest
    {
        public string propertyTaxNumber { get; set; }
        public string businessPremiseId { get; set; }
        public string workstationId { get; set; }
        public string reference {  get; set; }
    }
}
