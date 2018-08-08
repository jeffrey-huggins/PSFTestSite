using System.Data.Entity;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class InfectionControlContext : SharedContext
    {
        public static string infectionConnectionString;
        public InfectionControlContext() : base(infectionConnectionString)
        {
        }
        public InfectionControlContext(string connString) : base(connString)
        {
            infectionConnectionString = connString;
        }

        public DbSet<PatientIFCEvent> PatientIFCEvents { get; set; }
        public DbSet<EmployeeIFCEvent> EmployeeIFCEvents { get; set; }
        public DbSet<PatientIFCLimits> PatientIFCLimits { get; set; }
        public DbSet<PatientIFCSymptom> Symptoms { get; set; }
        public DbSet<PatientIFCSite> Sites { get; set; }
        public DbSet<PatientIFCEventTypeOfPrecaution> TypeOfPrecautions { get; set; }
        public DbSet<PatientIFCTypeOfPrecaution> Precautions { get; set; }
        public DbSet<PatientIFCEventSymptom> EventSymptoms { get; set; }
        public DbSet<PatientIFCEventReCulture> ReCultureDates { get; set; }
        public DbSet<PatientIFCEventOrganism> EventOrganisms { get; set; }
        public DbSet<PatientIFCEventDiagnosis> EventDiagnoses { get; set; }
        public DbSet<PatientIFCEventAntibiotic> EventAntibiotics { get; set; }
        public DbSet<PatientIFCDiagnosis> Diagnoses { get; set; }
        public DbSet<PatientIFCAntibiotic> Antibiotics { get; set; }
        public DbSet<PatientIFCOrganism> Organisms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientIFCEvent>().ToTable("PatientIFCEvent");
            modelBuilder.Entity<PatientIFCEvent>().Property(p => p.PatientIFCSiteId).HasColumnName("IFCSiteId");
            modelBuilder.Entity<EmployeeIFCEvent>().ToTable("EmployeeIFCEvent");
            modelBuilder.Entity<EmployeeIFCEvent>().Property(p => p.PatientIFCSiteId).HasColumnName("IFCSiteId");
            modelBuilder.Entity<PatientIFCLimits>().ToTable("MasterPatientIFCLimits");
            modelBuilder.Entity<PatientIFCSymptom>().ToTable("MasterPatientIFCSymptom");
            modelBuilder.Entity<PatientIFCSite>().ToTable("MasterIFCSite");
            modelBuilder.Entity<PatientIFCSite>().Property(p => p.PatientIFCSiteId).HasColumnName("IFCSiteId");
            modelBuilder.Entity<PatientIFCSite>().Property(p => p.PatientIFCSiteName).HasColumnName("IFCSiteName");
            modelBuilder.Entity<PatientIFCEventTypeOfPrecaution>().ToTable("PatientIFCEventTypeOfPrecaution");
            modelBuilder.Entity<PatientIFCEventTypeOfPrecaution>().Property(ifcS => ifcS.PatientIFCEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIFCEventTypeOfPrecaution>().Property(ifcS => ifcS.PatientIFCTypeOfPrecautionId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIFCTypeOfPrecaution>().ToTable("MasterPatientIFCTypeOfPrecaution");
            modelBuilder.Entity<PatientIFCEventSymptom>().ToTable("PatientIFCEventSymptom");
            modelBuilder.Entity<PatientIFCEventSymptom>().Property(ifcS => ifcS.PatientIFCEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIFCEventSymptom>().Property(ifcS => ifcS.PatientIFCSymptomId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIFCEventReCulture>().ToTable("PatientIFCEventReCulture");
            modelBuilder.Entity<PatientIFCEventReCulture>().Property(p => p.PatientIFCEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIFCEventReCulture>().Property(p => p.ReCultureId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIFCEventOrganism>().ToTable("PatientIFCEventOrganism");
            modelBuilder.Entity<PatientIFCEventOrganism>().Property(ifcS => ifcS.PatientIFCEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIFCEventOrganism>().Property(ifcS => ifcS.PatientIFCOrganismId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIFCEventDiagnosis>().ToTable("PatientIFCEventDiagnosis");
            modelBuilder.Entity<PatientIFCEventDiagnosis>().Property(ifcS => ifcS.PatientIFCEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIFCEventDiagnosis>().Property(ifcS => ifcS.PatientIFCDiagnosisId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIFCEventAntibiotic>().ToTable("PatientIFCEventAntibiotic");
            modelBuilder.Entity<PatientIFCEventAntibiotic>().Property(ifcS => ifcS.PatientIFCEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIFCEventAntibiotic>().Property(ifcS => ifcS.PatientIFCAntibioticId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIFCDiagnosis>().ToTable("MasterPatientIFCDiagnosis");
            modelBuilder.Entity<PatientIFCAntibiotic>().ToTable("MasterPatientIFCAntibiotic");
            modelBuilder.Entity<PatientIFCOrganism>().ToTable("MasterPatientIFCOrganism");
        }
    }
}