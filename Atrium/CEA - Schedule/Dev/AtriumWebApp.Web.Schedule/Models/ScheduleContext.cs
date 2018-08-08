using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class ScheduleContext : ScheduleBaseContext
    {
        public DbSet<SchdlPayPeriod> Week { get; set; }
        public DbSet<SchdlPayerGroup> Group { get; set; }
        public DbSet<SchdlGeneralLedger> SchedLedger { get; set; }
        public DbSet<SchdlSlot> Slots { get; set; }
        public DbSet<SchdlSlotDay> SlotDays { get; set; }
        public DbSet<EmployeeContact> EmployeeContacts { get; set; }
        public DbSet<SystemScheduleLogItem> ScheduleLog { get; set; }
        public DbSet<MasterAtriumPatientGroup> AreaRoom { get; set; }
        public DbSet<TemplateSchdlPayPeriod> TemplatePayPeriod { get; set; }
        public DbSet<TemplateSchdlPayerGroup> TemplatePayerGroup { get; set; }
        public DbSet<TemplateSchdlGeneralLedger> TemplateLedger { get; set; }
        public DbSet<TemplateSchdlSlot> TemplateSlot { get; set; }
        public DbSet<TemplateSchdlSlotDay> TemplateSlotDay { get; set; }

        public ScheduleContext() : base() { }

        public ScheduleContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public new ScheduleContext Context
        {
            get { return new ScheduleContext(); }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SchdlPayPeriod>().ToTable("SchdlPayPeriod");
            modelBuilder.Entity<SchdlPayPeriod>().Property(p => p.Id).HasColumnName("SchdlPayPeriodId");
            modelBuilder.Entity<SchdlPayPeriod>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SchdlPayPeriod>().Property(p => p.CommunityId);
            modelBuilder.Entity<SchdlPayPeriod>().Property(p => p.PayPeriodBeginDate).HasColumnType("date");
            modelBuilder.Entity<SchdlPayPeriod>().HasMany(p => p.PayerGroups).WithRequired(a => a.PayPeriod).HasForeignKey(p => p.SchdlPayPeriodId);

            modelBuilder.Entity<SchdlPayerGroup>().ToTable("SchdlPayerGroup");
            modelBuilder.Entity<SchdlPayerGroup>().Property(p => p.Id).HasColumnName("SchdlPayerGroupId");
            modelBuilder.Entity<SchdlPayerGroup>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SchdlPayerGroup>().Property(p => p.AtriumPayerGroupCode);
            modelBuilder.Entity<SchdlPayerGroup>().Property(p => p.SchdlPayPeriodId);
            modelBuilder.Entity<SchdlPayerGroup>().HasRequired(p => p.PayPeriod);
            modelBuilder.Entity<SchdlPayerGroup>().HasMany(p => p.ScheduleLedger).WithRequired(a => a.PayerGroup);

            modelBuilder.Entity<SchdlGeneralLedger>().ToTable("SchdlGeneralLedger");
            modelBuilder.Entity<SchdlGeneralLedger>().Property(p => p.Id).HasColumnName("SchdlGeneralLedgerId");
            modelBuilder.Entity<SchdlGeneralLedger>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SchdlGeneralLedger>().Property(p => p.HourPPDCnt);
            modelBuilder.Entity<SchdlGeneralLedger>().Property(p => p.GeneralLedgerId);
            modelBuilder.Entity<SchdlGeneralLedger>().Property(p => p.SchdlPayerGroupId);
            modelBuilder.Entity<SchdlGeneralLedger>().HasRequired(p => p.PayerGroup);
            modelBuilder.Entity<SchdlGeneralLedger>().HasRequired(p => p.GeneralLedger);
            modelBuilder.Entity<SchdlGeneralLedger>().HasMany(p => p.Slots).WithRequired(a => a.Ledger).HasForeignKey(p => p.SchdlGeneralLedgerId);

            modelBuilder.Entity<SchdlSlot>().ToTable("SchdlSlot");
            modelBuilder.Entity<SchdlSlot>().Property(p => p.Id).HasColumnName("SchdlSlotId");
            modelBuilder.Entity<SchdlSlot>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SchdlSlot>().Property(p => p.SchdlGeneralLedgerId);
            modelBuilder.Entity<SchdlSlot>().Property(p => p.WorkShiftId);
            modelBuilder.Entity<SchdlSlot>().Property(p => p.SlotNbr);
            modelBuilder.Entity<SchdlSlot>().Property(p => p.EmployeeId);
            modelBuilder.Entity<SchdlSlot>().Property(p => p.SchdlSlotAltId);
            modelBuilder.Entity<SchdlSlot>().HasRequired(p => p.Ledger);
            modelBuilder.Entity<SchdlSlot>().HasOptional(p => p.Employee);
            modelBuilder.Entity<SchdlSlot>().HasOptional(p => p.SchdlSlotAlt);
            modelBuilder.Entity<SchdlSlot>().HasMany(p => p.Days).WithRequired(a => a.Slot).HasForeignKey(p => p.SchdlSlotId);


            modelBuilder.Entity<SchdlSlotDay>().ToTable("SchdlSlotDay");
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.Id).HasColumnName("SchdlSlotDayId");
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.SchdlSlotId);
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.WorkDate);
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.AtriumPatientGroupId);
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.ShiftStartTime).HasColumnType("datetime"); ;
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.ShiftEndTime).HasColumnType("datetime"); ;
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.HourCnt);
            modelBuilder.Entity<SchdlSlotDay>().Property(p => p.SchdlHourAltId);
            modelBuilder.Entity<SchdlSlotDay>().HasRequired(p => p.Slot);
            modelBuilder.Entity<SchdlSlotDay>().HasOptional(p => p.SchdlHourAlt);
            modelBuilder.Entity<SchdlSlotDay>().HasRequired(p => p.AtriumPatientGroup);

            modelBuilder.Entity<TemplateSchdlPayPeriod>().ToTable("TemplateSchdlPayPeriod");
            modelBuilder.Entity<TemplateSchdlPayPeriod>().Property(p => p.Id).HasColumnName("TemplateSchdlPayPeriodId");
            modelBuilder.Entity<TemplateSchdlPayPeriod>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TemplateSchdlPayPeriod>().Property(p => p.CommunityId);
            modelBuilder.Entity<TemplateSchdlPayPeriod>().HasMany(p => p.PayerGroups).WithRequired(a => a.TemplatePayPeriod).HasForeignKey(p => p.TemplateSchdlPayPeriodId);

            modelBuilder.Entity<TemplateSchdlPayerGroup>().ToTable("TemplateSchdlPayerGroup");
            modelBuilder.Entity<TemplateSchdlPayerGroup>().Property(p => p.Id).HasColumnName("TemplateSchdlPayerGroupId");
            modelBuilder.Entity<TemplateSchdlPayerGroup>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TemplateSchdlPayerGroup>().Property(p => p.AtriumPayerGroupCode);
            modelBuilder.Entity<TemplateSchdlPayerGroup>().Property(p => p.TemplateSchdlPayPeriodId);
            modelBuilder.Entity<TemplateSchdlPayerGroup>().HasRequired(p => p.TemplatePayPeriod);
            modelBuilder.Entity<TemplateSchdlPayerGroup>().HasMany(p => p.ScheduleLedger).WithRequired(a => a.TemplatePayerGroup);

            modelBuilder.Entity<TemplateSchdlGeneralLedger>().ToTable("TemplateSchdlGeneralLedger");
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().Property(p => p.Id).HasColumnName("TemplateSchdlGeneralLedgerId");
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().Property(p => p.HourPPDCnt);
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().Property(p => p.GeneralLedgerId);
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().Property(p => p.TemplateSchdlPayerGroupId);
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().HasRequired(p => p.TemplatePayerGroup);
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().HasRequired(p => p.GeneralLedger);
            modelBuilder.Entity<TemplateSchdlGeneralLedger>().HasMany(p => p.Slots).WithRequired(a => a.Ledger).HasForeignKey(p => p.TemplateSchdlGeneralLedgerId);

            modelBuilder.Entity<TemplateSchdlSlot>().ToTable("TemplateSchdlSlot");
            modelBuilder.Entity<TemplateSchdlSlot>().Property(p => p.Id).HasColumnName("TemplateSchdlSlotId");
            modelBuilder.Entity<TemplateSchdlSlot>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TemplateSchdlSlot>().Property(p => p.TemplateSchdlGeneralLedgerId);
            modelBuilder.Entity<TemplateSchdlSlot>().Property(p => p.WorkShiftId);
            modelBuilder.Entity<TemplateSchdlSlot>().Property(p => p.SlotNbr);
            modelBuilder.Entity<TemplateSchdlSlot>().Property(p => p.EmployeeId);
            modelBuilder.Entity<TemplateSchdlSlot>().Property(p => p.SchdlSlotAltId);
            modelBuilder.Entity<TemplateSchdlSlot>().HasRequired(p => p.Ledger);
            modelBuilder.Entity<TemplateSchdlSlot>().HasOptional(p => p.Employee);
            modelBuilder.Entity<TemplateSchdlSlot>().HasOptional(p => p.SchdlSlotAlt);
            modelBuilder.Entity<TemplateSchdlSlot>().HasMany(p => p.Days).WithRequired(a => a.Slot).HasForeignKey(p => p.TemplateSchdlSlotId);

            modelBuilder.Entity<TemplateSchdlSlotDay>().ToTable("TemplateSchdlSlotDay");
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.Id).HasColumnName("TemplateSchdlSlotDayId");
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.TemplateSchdlSlotId);
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.PayPeriodDayNbr);
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.AtriumPatientGroupId);
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.ShiftStartTime).HasColumnType("datetime"); ;
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.ShiftEndTime).HasColumnType("datetime"); ;
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.HourCnt);
            modelBuilder.Entity<TemplateSchdlSlotDay>().Property(p => p.SchdlHourAltId);
            modelBuilder.Entity<TemplateSchdlSlotDay>().HasRequired(p => p.Slot);
            modelBuilder.Entity<TemplateSchdlSlotDay>().HasOptional(p => p.SchdlHourAlt);
            modelBuilder.Entity<TemplateSchdlSlotDay>().HasRequired(p => p.AtriumPatientGroup);
        }
    }
}