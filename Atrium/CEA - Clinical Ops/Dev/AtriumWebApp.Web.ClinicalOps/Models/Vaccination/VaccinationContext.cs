using System.Data.Entity;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class VaccinationContext : SharedContext
    {
        public VaccinationContext() : base(connectionString)
        {

        }
        public VaccinationContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<VaccineType> VaccineTypes { get; set; }
        public DbSet<VaccinePneumoniaType> VaccinePneumoniaTypes { get; set; }
        public DbSet<VaccineTBReactionMeasurement> VaccineTBReactionMeasurements { get; set; }
        public DbSet<VaccineTBSite> VaccineTBSites { get; set; }

        public DbSet<PatientVaccine> PatientVaccineRecords { get; set; }
        public DbSet<PatientVaccineInfluenza> PatientVaccineInfluenzaRecords { get; set; }
        public DbSet<PatientVaccinePneumonia> PatientVaccinePneumoniaRecords { get; set; }
        public DbSet<PatientVaccineTBAnnual> PatientVaccineTBAnnualRecords { get; set; }
        public DbSet<PatientVaccineTBInitial2Step> PatientVaccineTBInitial2StepRecords { get; set; }

        public DbSet<EmployeeVaccineRecord> EmployeeVaccineRecords { get; set; }
        public DbSet<EmployeeVaccineInfluenzaRecord> EmployeeVaccineInfluenzaRecords { get; set; }
        public DbSet<EmployeeVaccineTBAnnualRecord> EmployeeVaccineTBAnnualRecords { get; set; }
        public DbSet<EmployeeVaccineTBInitial2StepRecord> EmployeeVaccineTBInitial2StepRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VaccineType>().ToTable("SystemVaccineType");
            modelBuilder.Entity<VaccinePneumoniaType>().ToTable("SystemVaccinePneumoniaType");
            modelBuilder.Entity<VaccineTBReactionMeasurement>().ToTable("SystemVaccineTBReactionMeasurement");
            modelBuilder.Entity<VaccineTBReactionMeasurement>().Property(p => p.Description).HasColumnName("VaccineTBReactionMeasurementDesc");
            modelBuilder.Entity<VaccineTBSite>().ToTable("SystemVaccineTBSite");
            modelBuilder.Entity<VaccineTBSite>().Property(p => p.SiteName).HasColumnName("VaccineTBSiteName");

            modelBuilder.Entity<PatientVaccine>().ToTable("PatientVaccine");
            modelBuilder.Entity<PatientVaccineInfluenza>().ToTable("PatientVaccineInfluenza");
            modelBuilder.Entity<PatientVaccineInfluenza>().Property(p => p.LotNumber).HasColumnName("LotNbr");
            modelBuilder.Entity<PatientVaccinePneumonia>().ToTable("PatientVaccinePneumonia");
            modelBuilder.Entity<PatientVaccinePneumonia>().Property(p => p.LotNumber).HasColumnName("LotNbr");
            modelBuilder.Entity<PatientVaccineTBAnnual>().ToTable("PatientVaccineTBAnnual");
            modelBuilder.Entity<PatientVaccineTBAnnual>().Property(p => p.PPDLotNumber).HasColumnName("PPDLotNbr");
            modelBuilder.Entity<PatientVaccineTBInitial2Step>().ToTable("PatientVaccineTBInitial2Step");
            modelBuilder.Entity<PatientVaccineTBInitial2Step>().Property(p => p.PPDStep1LotNumber).HasColumnName("PPDStep1LotNbr");
            modelBuilder.Entity<PatientVaccineTBInitial2Step>().Property(p => p.PPDStep2LotNumber).HasColumnName("PPDStep2LotNbr");

            modelBuilder.Entity<EmployeeVaccineRecord>().ToTable("EmployeeVaccine");
            modelBuilder.Entity<EmployeeVaccineRecord>().Property(p => p.VaccineRecordId).HasColumnName("EmployeeVaccineId");
            modelBuilder.Entity<EmployeeVaccineRecord>().Property(p => p.PersonId).HasColumnName("EmployeeId");
            modelBuilder.Entity<EmployeeVaccineRecord>().Property(p => p.LotNumber).HasColumnName("LotNbr");
            modelBuilder.Entity<EmployeeVaccineInfluenzaRecord>().ToTable("EmployeeVaccineInfluenza");
            modelBuilder.Entity<EmployeeVaccineInfluenzaRecord>().Property(p => p.VaccineRecordId).HasColumnName("EmployeeVaccineId");
            modelBuilder.Entity<EmployeeVaccineTBAnnualRecord>().ToTable("EmployeeVaccineTBAnnual");
            modelBuilder.Entity<EmployeeVaccineTBAnnualRecord>().Property(p => p.VaccineRecordId).HasColumnName("EmployeeVaccineId");
            modelBuilder.Entity<EmployeeVaccineTBInitial2StepRecord>().ToTable("EmployeeVaccineTBInitial2Step");
            modelBuilder.Entity<EmployeeVaccineTBInitial2StepRecord>().Property(p => p.VaccineRecordId).HasColumnName("EmployeeVaccineId");
        }
    }
}