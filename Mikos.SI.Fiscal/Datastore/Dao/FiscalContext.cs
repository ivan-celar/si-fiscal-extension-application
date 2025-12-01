using Micros.PosCore.Extensibility.DataStore.DbRecords;
using Mikos.SI.Fiscal.Datastore.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Datastore.Dao
{
    public class FiscalContext : DbContext
    {
        public DbSet<FiscalInvoice> FiscalInvoices { get; set; }

        public DbSet<BuyerInfo> BuyerInfos { get; set; }

        public FiscalContext()
            : base("MIKOS_FISCAL_SI")
        {
            var created = base.Database.CreateIfNotExists();
            var exists = base.Database.Exists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<FiscalContext, SiConfiguration>());
        }
    }
}
