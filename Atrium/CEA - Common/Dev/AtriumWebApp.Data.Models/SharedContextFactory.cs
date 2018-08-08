using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SharedContextFactory : IDbContextFactory<SharedContext>
    {
        public SharedContext Create()
        {
            if (string.IsNullOrEmpty(SharedContext.connectionString))
            {
                //command line tools will use the test db
                return new SharedContext("Server=ironman,22866;Initial Catalog=CorporateEnterpriseApplication_TST;User Id=CEAUser;Password=C0rp@ccess;");
            }
            else
            {
                return new SharedContext(SharedContext.connectionString);
            }
        }
    }
}
