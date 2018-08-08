using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PASRRContext : SharedContext
    {
        public PASRRContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<PatientPASRRLog> PatientPASRRLogs { get; set; }
        public DbSet<PASRRType> PASRRTypes { get; set; }
        public DbSet<PASRRTypeStateCode> PASRRTypeStateCodes { get; set; }
        public DbSet<PASRRSigChangeType> SigChangeTypes { get; set; }
        public DbSet<PASRRSigChangeTypeStateCode> SigChangeTypeStateCodes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientPASRRLog>().ToTable("PatientPASRRLog");
            modelBuilder.Entity<PatientPASRRLog>().Property(p => p.PASRRLogId).HasColumnName("PatientPASRRLogId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<PatientPASRRLog>().Property(p => p.CompleteDate).HasColumnName("PASRRCompleteDate");
            modelBuilder.Entity<PatientPASRRLog>().Property(p => p.HospitalExemption).HasColumnName("HospitalExemptFlg");
            modelBuilder.Entity<PatientPASRRLog>().Property(p => p.HospitalExemptionExpirationDate).HasColumnName("HospitalExpirationDate");
            modelBuilder.Entity<PatientPASRRLog>().Property(p => p.StayGreaterThan30Days).HasColumnName("AnticipatedGreaterThan30Flg");
            modelBuilder.Entity<PatientPASRRLog>().Property(p => p.DementiaExemption).HasColumnName("DementiaExemptFlg");
            modelBuilder.Entity<PatientPASRRLog>().Property(p => p.LevelIINeeded).HasColumnName("LevelIINeededFlg");

            modelBuilder.Entity<PASRRType>().ToTable("SystemPASRRType");
            modelBuilder.Entity<PASRRType>().HasMany(p => p.StateCodes);
            
            modelBuilder.Entity<PASRRTypeStateCode>().ToTable("SystemPASRRTypeState");
            modelBuilder.Entity<PASRRTypeStateCode>().Property(p => p.PASRRTypeId).HasColumnOrder(0);
            modelBuilder.Entity<PASRRTypeStateCode>().Property(p => p.StateCode).HasColumnName("StateCd").HasColumnOrder(1);

            modelBuilder.Entity<PASRRSigChangeType>().ToTable("SystemSigChangeType");
            modelBuilder.Entity<PASRRSigChangeType>().HasMany(p => p.StateCodes);
            
            modelBuilder.Entity<PASRRSigChangeTypeStateCode>().ToTable("SystemSigChangeTypeState");
            modelBuilder.Entity<PASRRSigChangeTypeStateCode>().Property(p => p.SigChangeTypeId).HasColumnOrder(0);
            modelBuilder.Entity<PASRRSigChangeTypeStateCode>().Property(p => p.StateCode).HasColumnName("StateCd").HasColumnOrder(1);
        }
    }
}