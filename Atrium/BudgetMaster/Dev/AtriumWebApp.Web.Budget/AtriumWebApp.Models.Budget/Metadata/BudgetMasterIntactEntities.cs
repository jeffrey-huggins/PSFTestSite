using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget
{
    public partial class BudgetMasterIntactEntities : DbContext
    {
        public static string connection;
        public BudgetMasterIntactEntities(string connectionString)
        {
            connection = connectionString;
            Database.Connection.ConnectionString = connectionString;
        }

    }
}
