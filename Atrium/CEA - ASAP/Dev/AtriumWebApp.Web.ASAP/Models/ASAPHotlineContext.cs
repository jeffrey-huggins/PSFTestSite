using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;


namespace AtriumWebApp.Web.ASAP.Models
{
    public class ASAPHotlineContext : SharedContext
    {
        public ASAPHotlineContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<ASAPCallDocument> ASAPCallDocuments { get; set; }
        public DbSet<ASAPContact> Contacts { get; set; }
        public DbSet<ASAPCall> AsapCalls { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ASAPCallDocument>().ToTable("ASAPCallDocument");
            modelBuilder.Entity<ASAPCall>().ToTable("ASAPCall");
            modelBuilder.Entity<ASAPContact>().ToTable("MasterASAPContact");


        }
    }
}
