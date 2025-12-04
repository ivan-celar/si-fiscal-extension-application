using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Dtos
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class FiscalResponseData
    {
        public string FiscalFolioNo { get; set; }
        public FiscalOutputs FiscalOutputs { get; set; }
        public StatusMessages StatusMessages { get; set; }
    }

    public class FiscalOutputs
    {
        public List<Output> Output { get; set; }
    }

    public class Message
    {
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class Output
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class StatusMessages
    {
        public List<Message> Messages { get; set; }
    }


}
