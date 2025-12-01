using Mikos.SI.Fiscal.Datastore.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Micros.PosCore.Extensibility.DataStore.DbRecords.DbKey;

namespace Mikos.SI.Fiscal.Datastore.Repository
{
    public class FiscalInvoiceRepository : Repository<FiscalInvoice>, IFiscalInvoiceRepository
    {
        public FiscalInvoiceRepository(FiscalContext context) : base(context)
        {
        }

        public FiscalInvoice GetByZOI(string zoi)
        {
            return _context.FiscalInvoices.FirstOrDefault(invoice => invoice.ZOI == zoi);
        }

        public FiscalInvoice GetByEOR(string eor)
        {
            return _context.FiscalInvoices.FirstOrDefault(invoice => invoice.EOR == eor);
        }

        public FiscalInvoice GetByCheckGuid(string checkGuid)
        {
            return _context.FiscalInvoices.FirstOrDefault(invoice => invoice.ChkGUID.Equals(checkGuid.Trim()));
        }

        public FiscalInvoice GetLastCheckByCheckNumber(int checkNumber)
        {
            return _context.FiscalInvoices.Where(invoice => invoice.CheckNumber == checkNumber)
                   .OrderByDescending(invoice => invoice.SeqNum)
                   .FirstOrDefault();
        }
    }

}
