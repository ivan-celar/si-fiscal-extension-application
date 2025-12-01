using Mikos.SI.Fiscal.Datastore.Dao;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikos.SI.Fiscal.Datastore.Configuration
{
    internal sealed class SiConfiguration : DbMigrationsConfiguration<FiscalContext>
    {
        public SiConfiguration()
        {
            base.AutomaticMigrationsEnabled = true;
            base.AutomaticMigrationDataLossAllowed = false;
            base.ContextKey = "Mikos.Si.Fiscal.Datastore.Configuration";
        }

        protected override void Seed(FiscalContext context)
        {
        }
    }
}
