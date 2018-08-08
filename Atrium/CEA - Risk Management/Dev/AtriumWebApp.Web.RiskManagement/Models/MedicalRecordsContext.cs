using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class MedicalRecordsContext : SharedContext
    {
        public MedicalRecordsContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<MedicalRecordsRequest> RecordRequests { get; set; }
        public DbSet<MedicalRecordsRequestNotes> MedicalRecordsRequestNotes { get; set; }
        public DbSet<MedicalRecordsRequestDocument> MedicalRequestDocuments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MedicalRecordsRequest>().ToTable("MedicalRecordsRequest");
            modelBuilder.Entity<MedicalRecordsRequestNotes>().ToTable("MedicalRecordsRequestNotes");
            modelBuilder.Entity<MedicalRecordsRequestDocument>().ToTable("MedicalRecordsRequestDocument");
        }
    }
}