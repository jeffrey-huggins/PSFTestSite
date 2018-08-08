using System;
using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Controllers;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class ScheduleAdminContext : SharedContext
    {
        public ScheduleAdminContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<MasterAtriumPatientGroup> AreaRoom { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            
            modelBuilder.Entity<MasterAtriumPatientGroup>().ToTable("MasterAtriumPatientGroup");
            modelBuilder.Entity<MasterAtriumPatientGroup>().Property(p => p.Id).HasColumnName("AtriumPatientGroupId");
            modelBuilder.Entity<MasterAtriumPatientGroup>().HasKey(k => k.Id);
            modelBuilder.Entity<MasterAtriumPatientGroup>().Property(p => p.AtriumPatientGroupName).HasColumnName("AtriumPatientGroupName");
            modelBuilder.Entity<MasterAtriumPatientGroup>().Property(p => p.CommunityId).HasColumnName("CommunityId");
        }
    }
}