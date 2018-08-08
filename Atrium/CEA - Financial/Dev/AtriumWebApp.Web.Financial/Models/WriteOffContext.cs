using AtriumWebApp.Models;
using System.Data.Entity;

namespace AtriumWebApp.Web.Financial.Models
{
    public class WriteOffContext : SharedContext
    {
        public WriteOffContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<WriteOff> WriteOffs { get; set; }
        public DbSet<CommunityPayerRelationship> CommunityPayerRelationships { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WriteOff>().ToTable("WriteOff");
            
            modelBuilder.Entity<CommunityPayerRelationship>().ToTable("CommunityCommunityPayerInfo");
            //modelBuilder.Entity<CommunityPayerRelationship>().Map(r => {
            //    r.ToTable("MasterCommunity");
            //    r.Properties(p => p.Community);
            //});
            //modelBuilder.Entity<CommunityPayerRelationship>().Map(r =>
            //{
            //    r.ToTable("MasterCommunityPayers");
            //    r.Properties(p => p.CommunityPayer);
            //});
        }
    }
}
