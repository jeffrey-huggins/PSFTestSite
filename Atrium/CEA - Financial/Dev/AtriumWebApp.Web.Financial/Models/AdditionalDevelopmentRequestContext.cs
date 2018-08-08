using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Financial.Models
{
    public class AdditionalDevelopmentRequestContext : SharedContext
    {
        public AdditionalDevelopmentRequestContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<AdditionalDevelopmentRequest> ADRs { get; set; }
        public DbSet<ADRPayer> ADRPayers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AdditionalDevelopmentRequest>().ToTable("AdditionalDevelopmentRequest");
            modelBuilder.Entity<ADRPayer>().ToTable("MasterADRPayer");
        }
    }
}