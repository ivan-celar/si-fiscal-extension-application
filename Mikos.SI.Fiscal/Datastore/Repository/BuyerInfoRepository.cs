using Mikos.SI.Fiscal.Datastore.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Datastore.Repository
{
    public class BuyerInfoRepository : Repository<BuyerInfo>, IBuyerInfoRepository
    {
        public BuyerInfoRepository(FiscalContext context) : base(context) { }

        public BuyerInfo GetByVatId(string vatId)
        {
            return _context.BuyerInfos.FirstOrDefault(bi => bi.vatId.Equals(vatId));
        }
    }
}
