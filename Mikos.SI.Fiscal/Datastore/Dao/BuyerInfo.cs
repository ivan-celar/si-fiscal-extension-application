using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Datastore.Dao
{
    public class BuyerInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string name { get; set; }
        public string vatId { get; set; }
        public string address { get; set; }
        public string town { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public bool active { get; set; }
    }
}
