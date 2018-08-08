using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class HospitalDischargeContext : SharedContext
    {
        public HospitalDischargeContext() : base()
        {
        }
        public HospitalDischargeContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<HospitalDischarge> Discharges { get; set; }
        public DbSet<DischargeReason> DischargeReasons { get; set; }
        public DbSet<DidNotReturnReason> DidNotReturnReasons { get; set; }
        public DbSet<PatientAdmits> PatientAdmits { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        //public DbSet<CommunityPayers> CommunityPayers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HospitalDischarge>().ToTable("HospitalDischarge");
            modelBuilder.Entity<HospitalDischarge>().Property(hd => hd.PatientId).HasColumnOrder(0);
            modelBuilder.Entity<HospitalDischarge>().Property(hd => hd.CensusDate).HasColumnOrder(1);
            modelBuilder.Entity<DischargeReason>().ToTable("MasterDischargeReason");
            modelBuilder.Entity<DidNotReturnReason>().ToTable("MasterDidNotReturnReason");
            modelBuilder.Entity<PatientAdmits>().ToTable("PatientAdmits");
            modelBuilder.Entity<PatientAdmits>().Property(p => p.PatientId).HasColumnOrder(0);
            modelBuilder.Entity<PatientAdmits>().Property(p => p.AdmitDateTime).HasColumnOrder(1);
            modelBuilder.Entity<Hospital>().ToTable("MasterHospital");
            modelBuilder.Entity<Hospital>().Property(p => p.Id).HasColumnName("HospitalId");
            modelBuilder.Entity<Hospital>().Property(p => p.Name).HasColumnName("HospitalName");
            modelBuilder.Entity<Hospital>().Property(p => p.AllowDataEntry).HasColumnName("DataEntryFlg");
            modelBuilder.Entity<Hospital>().Property(p => p.AllowReporting).HasColumnName("ReportFlg");
            modelBuilder.Entity<Hospital>().HasMany(p => p.Communities).WithMany().Map(p => 
            {
                p.ToTable("HospitalCommunityInfo");
                p.MapLeftKey("HospitalId");
                p.MapRightKey("CommunityId");
            });
            modelBuilder.Entity<CommunityPayers>().ToTable("MasterCommunityPayers");
        }
    }
}
