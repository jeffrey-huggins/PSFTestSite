using System.Data.Entity;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class IncidentTrackingContext : SharedContext
    {
        public DbSet<IncidentIntervention> IncidentInterventions { get; set; }
        public DbSet<IncidentTreatment> IncidentTreatments { get; set; }
        public DbSet<PatientIncidentEvent> PatientIncidentEvents { get; set; }
        public DbSet<PatientIncidentEventIntervention> EventInterventions { get; set; }
        public DbSet<PatientIncidentEventTreatment> EventTreatments { get; set; }
        public DbSet<PatientIncidentLocation> IncidentLocations { get; set; }
        public DbSet<PatientIncidentType> PatientIncidentTypes { get; set; }
        public DbSet<RegionalNurseCommunityInfo> RegionalNurses { get; set; }
        public DbSet<PatientIncidentEventDocument> PatientIncidentEventDocuments { get; set; }
        public DbSet<CloseIncidentAllCommunity> CloseAllCommunitiesEmployees { get; set; }

        public IncidentTrackingContext() : base()
        {
        }

        public IncidentTrackingContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IncidentIntervention>().ToTable("MasterPatientIncidentIntervention");
            modelBuilder.Entity<IncidentTreatment>().ToTable("MasterPatientIncidentTreatment");
            modelBuilder.Entity<PatientIncidentEvent>().ToTable("PatientIncidentEvent");
            modelBuilder.Entity<PatientIncidentEventIntervention>().ToTable("PatientIncidentIntervention");
            modelBuilder.Entity<PatientIncidentEventIntervention>().Property(p => p.PatientIncidentEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIncidentEventIntervention>().Property(p => p.PatientIncidentInterventionId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIncidentEventTreatment>().ToTable("PatientIncidentTreatment");
            modelBuilder.Entity<PatientIncidentEventTreatment>().Property(p => p.PatientIncidentEventId).HasColumnOrder(0);
            modelBuilder.Entity<PatientIncidentEventTreatment>().Property(p => p.PatientIncidentTreatmentId).HasColumnOrder(1);
            modelBuilder.Entity<PatientIncidentLocation>().ToTable("MasterPatientIncidentLocation");
            modelBuilder.Entity<PatientIncidentType>().ToTable("MasterPatientIncidentType");
            modelBuilder.Entity<RegionalNurseCommunityInfo>().ToTable("RegionalNurseCommunityInfo");
            modelBuilder.Entity<RegionalNurseCommunityInfo>().Property(aci => aci.RegionalNurseEmployeeId).HasColumnOrder(0);
            modelBuilder.Entity<RegionalNurseCommunityInfo>().Property(aci => aci.CommunityId).HasColumnOrder(1);
            modelBuilder.Entity<CloseIncidentAllCommunity>().ToTable("CloseIncidentAllCommunity");
            modelBuilder.Entity<PatientIncidentEventDocument>().ToTable("PatientIncidentEventDocument");
        }
    }
}