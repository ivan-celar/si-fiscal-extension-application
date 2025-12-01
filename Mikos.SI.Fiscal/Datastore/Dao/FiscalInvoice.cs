using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Datastore.Dao
{
    public class FiscalInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long SeqNum { get; set; }
        [StringLength(100)]
        public string SpecialId { get; set; }

        [StringLength(11)]
        public string TaxNumber { get; set; }

        public DateTime? ChkClosedDateTime { get; set; }

        [StringLength(20)]
        public string RvcNumber { get; set; }

        [StringLength(20)]
        public string WorkstationNumber { get; set; }

        public long CheckNumber { get; set; }

        [StringLength(11)]
        public string EmployeeVatId { get; set; }

        [StringLength(32)]
        public string ZOI { get; set; }

        [StringLength(36)]
        public string EOR { get; set; }

        [Required]
        [DefaultValue(1)]
        public bool Queued { get; set; } = true;

        public DateTime? SyncDateTime { get; set; }

        [Required]
        [DefaultValue(0)]
        public bool Error { get; set; } = false;

        [StringLength(10)]
        public string ErrorCode { get; set; }

        [StringLength(256)]
        public string ErrorDesc { get; set; }

        [StringLength(1)]
        public string PaymentMethod { get; set; }
        public decimal PaymentSubtotal { get; set; }

        public decimal PaymentTotal { get; set; }

        public string Tax01Name { get; set; }

        public decimal? Tax01Net { get; set; }

        public decimal? Tax01Vat { get; set; }

        public string Tax02Name { get; set; }

        public decimal? Tax02Net { get; set; }

        public decimal? Tax02Vat { get; set; }

        public string Tax03Name { get; set; }

        public decimal? Tax03Net { get; set; }

        public decimal? Tax03Vat { get; set; }

        public string Tax04Name { get; set; }

        public decimal? Tax04Net { get; set; }

        public decimal? Tax04Vat { get; set; }

        public decimal? TaxExemptTtl { get; set; }

        public decimal? NoTaxTtl { get; set; }

        public DateTime? InsertDateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateDateTime { get; set; }

        [StringLength(256)]
        public string VerifyInvoiceCode { get; set; }

        public byte[] QRImage { get; set; }

        [StringLength(100)]
        public string ChkGUID { get; set; }
        [StringLength(256)]
        public string ResponseMessage { get; set; }
    }
}
