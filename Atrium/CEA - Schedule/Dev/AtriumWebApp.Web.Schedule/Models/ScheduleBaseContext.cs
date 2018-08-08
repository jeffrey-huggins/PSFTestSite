using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using AtriumWebApp.Models;
namespace AtriumWebApp.Web.Schedule.Models
{
    public class ScheduleBaseContext : SharedContext
    {
        //public DbSet<SystemWorkShift> Shifts { get; set; }
        public DbSet<SystemPayPeriod> PayPeriod { get; set; }
        public DbSet<SystemSchdlHourAlt> AltHours { get; set; }
        public DbSet<SystemSchdlSlotAlt> AltSlots { get; set; }
        //public DbSet<SystemMonthlyLaborBudget> MonthlyLaborBudget { get; set; }

        public ScheduleBaseContext() : base() { }

        public ScheduleBaseContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public new ScheduleBaseContext Context
        {
            get { return new ScheduleBaseContext(); }

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SystemPayPeriod>().ToTable("SystemPayPeriod");
            modelBuilder.Entity<SystemPayPeriod>().Property(p => p.Id).HasColumnName("PayPeriodId");
            modelBuilder.Entity<SystemPayPeriod>().HasKey(k => k.Id);
            modelBuilder.Entity<SystemPayPeriod>().Property(p => p.PayPeriodBeginDate);
            modelBuilder.Entity<SystemPayPeriod>().Property(p => p.PayPeriodEndDate);
            modelBuilder.Entity<SystemPayPeriod>().Property(p => p.CheckDate);
            modelBuilder.Entity<SystemPayPeriod>().Property(p => p.PayPeriodYear);
            modelBuilder.Entity<SystemPayPeriod>().Property(p => p.PayPeriod);
            modelBuilder.Entity<SystemPayPeriod>().Property(p => p.PayPeriodTotal);

            modelBuilder.Entity<SystemSchdlSlotAlt>().ToTable("SystemSchdlSlotAlt");
            modelBuilder.Entity<SystemSchdlSlotAlt>().Property(p => p.Id).HasColumnName("SchdlSlotAltId");
            modelBuilder.Entity<SystemSchdlSlotAlt>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SystemSchdlSlotAlt>().Property(p => p.SlotAltCode).HasMaxLength(8);
            modelBuilder.Entity<SystemSchdlSlotAlt>().Property(p => p.SlotAltDesc).HasMaxLength(32);
            modelBuilder.Entity<SystemSchdlSlotAlt>().Property(p => p.SortOrder);
            modelBuilder.Entity<SystemSchdlSlotAlt>().Property(p => p.SchdlCalcFlg);

            modelBuilder.Entity<SystemSchdlHourAlt>().ToTable("SystemSchdlHourAlt");
            modelBuilder.Entity<SystemSchdlHourAlt>().Property(p => p.Id).HasColumnName("SchdlHourAltId");
            modelBuilder.Entity<SystemSchdlHourAlt>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SystemSchdlHourAlt>().Property(p => p.HourAltCode).HasMaxLength(8);
            modelBuilder.Entity<SystemSchdlHourAlt>().Property(p => p.HourAltDesc).HasMaxLength(16);
            modelBuilder.Entity<SystemSchdlHourAlt>().Property(p => p.SortOrder);

            modelBuilder.Entity<EmployeeContact>().ToTable("MasterEmployeeContact");
            modelBuilder.Entity<EmployeeContact>().Property(p => p.Id).HasColumnName("EmployeeId");
            modelBuilder.Entity<EmployeeContact>().HasKey(k => k.Id);
            modelBuilder.Entity<EmployeeContact>().Property(p => p.EmployeeContactTypeId).HasColumnName("EmployeeContactTypeId");
            modelBuilder.Entity<EmployeeContact>().Property(p => p.Contactinformation).HasColumnName("Contactinformation");

            modelBuilder.Entity<SystemScheduleLogItem>().ToTable("SystemScheduleLog");

            modelBuilder.Entity<MasterAtriumPatientGroup>().ToTable("MasterAtriumPatientGroup");
            modelBuilder.Entity<MasterAtriumPatientGroup>().Property(p => p.Id).HasColumnName("AtriumPatientGroupId");
            modelBuilder.Entity<MasterAtriumPatientGroup>().HasKey(k => k.Id);
            modelBuilder.Entity<MasterAtriumPatientGroup>().Property(p => p.AtriumPatientGroupName).HasColumnName("AtriumPatientGroupName");
            modelBuilder.Entity<MasterAtriumPatientGroup>().Property(p => p.CommunityId).HasColumnName("CommunityId");

        }
    }
}