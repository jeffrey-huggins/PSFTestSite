using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PELIContext : SharedContext
    {
        public PELIContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<PatientPELILog> PatientPELILogs { get; set; }
        public DbSet<PELIType> PELITypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientPELILog>().ToTable("PatientPELILog");
            modelBuilder.Entity<PatientPELILog>().Property(p => p.PELILogId).HasColumnName("PatientPELILogId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<PatientPELILog>().HasKey(p => p.PELILogId);
            modelBuilder.Entity<PatientPELILog>().Property(p => p.PatientId).HasColumnName("PatientId");
            modelBuilder.Entity<PatientPELILog>().Property(p => p.AdmitDate).HasColumnName("AdmitDate");
            modelBuilder.Entity<PatientPELILog>().Property(p => p.CompletedDate).HasColumnName("CompletedDate");
            modelBuilder.Entity<PatientPELILog>().Property(p => p.PELITypeId).HasColumnName("MasterPELIIncompleteReasonId");
            modelBuilder.Entity<PatientPELILog>().Property(p => p.DeletedFlg).HasColumnName("DeletedFlg");
            //modelBuilder.Entity<PatientPELILog>().HasMany(p => p.PELIType).WithRequired().Map(p => p.MapKey("MasterPELIIncompleteReasonId"));
            //modelBuilder.Entity<PatientPELILog>().HasMany(p => p.PELIType).WithOptional().HasForeignKey(k => k.PayPeriodScheduleId);

            //HasMany(p => p.PELIType);

            modelBuilder.Entity<PELIType>().ToTable("MasterPELIIncompleteReason");
            modelBuilder.Entity<PELIType>().Property(p => p.Id).HasColumnName("MasterPELIIncompleteReasonId");
            modelBuilder.Entity<PELIType>().HasKey(p => p.Id);
            modelBuilder.Entity<PELIType>().Property(p => p.Description).HasColumnName("PELIIncompleteReasonDesc");
            modelBuilder.Entity<PELIType>().Property(p => p.DataEntryFlg).HasColumnName("DataEntryFlg");
            modelBuilder.Entity<PELIType>().Property(p => p.SortOrder).HasColumnName("SortOrder");
        }
    }
}