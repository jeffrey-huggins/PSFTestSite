using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Home.Models
{
    public class SSISJobsContext : DbContext
    {
        public SSISJobsContext(string connString) : base(connString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<JobActivity>().
            base.OnModelCreating(modelBuilder);
        }

    }
}
