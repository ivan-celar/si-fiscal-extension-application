using Mikos.SI.Fiscal.Datastore.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Datastore.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(long id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(long id);
        void Save();
    }

    public interface IFiscalInvoiceRepository : IRepository<FiscalInvoice>
    {
        FiscalInvoice GetByZOI(string zki);
        FiscalInvoice GetByEOR(string jir);
        FiscalInvoice GetByCheckGuid(string checkGuid);
        FiscalInvoice GetLastCheckByCheckNumber(int checkNumber);
    }

    public interface IBuyerInfoRepository : IRepository<BuyerInfo>
    {
        BuyerInfo GetByVatId(string vatId);
    }
}
