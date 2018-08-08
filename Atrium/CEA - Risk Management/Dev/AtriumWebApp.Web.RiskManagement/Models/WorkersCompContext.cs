using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompContext : SharedContext
    {
        public DbSet<WorkersCompClaim> WorkersCompClaims { get; set; }
        public DbSet<WorkersCompClaimNotes> WorkersCompClaimNotes { get; set; }
        public DbSet<WorkersCompDiagnosis> WorkersCompDiagnoses { get; set; }
        public DbSet<WorkersCompExpense> WorkersCompExpenses { get; set; }
        public DbSet<WorkersCompClaimType> CompClaimTypes { get; set; }
        public DbSet<WorkersCompDiagnosisType> WorkersCompDiagnosisTypes { get; set; }
        public DbSet<WorkersCompInsurance> WorkersCompInsurances { get; set; }
        public DbSet<WorkersCompLegalFirm> WorkersCompLegalFirms { get; set; }
        public DbSet<WorkersCompTCM> WorkersCompTCMs { get; set; }
        public DbSet<WorkersCompVOCRehab> WorkersCompVOCRehabs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WorkersCompClaim>().ToTable("WorkersCompClaim");
            modelBuilder.Entity<WorkersCompClaim>().Property(p => p.ReportedToCarrierDateOverrideFlag).HasColumnName("ReportedToCarrierDateOverRideFlg");
            modelBuilder.Entity<WorkersCompClaimNotes>().ToTable("WorkersCompClaimNotes");
            modelBuilder.Entity<WorkersCompDiagnosis>().ToTable("WorkersCompDiagnosis");
            modelBuilder.Entity<WorkersCompDiagnosis>().Property(d => d.ClaimId).HasColumnOrder(0);
            modelBuilder.Entity<WorkersCompDiagnosis>().Property(d => d.DiagnosisId).HasColumnOrder(1);
            modelBuilder.Entity<WorkersCompExpense>().ToTable("WorkersCompExpense");
            modelBuilder.Entity<WorkersCompExpense>().Property(w => w.ClaimId).HasColumnOrder(0);
            modelBuilder.Entity<WorkersCompExpense>().Property(w => w.ExpenseType).HasColumnOrder(1);
            modelBuilder.Entity<WorkersCompExpense>().Property(w => w.ExpenseKey).HasColumnOrder(2);
            modelBuilder.Entity<WorkersCompClaimType>().ToTable("MasterWorkersCompClaimType");
            modelBuilder.Entity<WorkersCompDiagnosisType>().ToTable("MasterWorkersCompDiagnosis");
            modelBuilder.Entity<WorkersCompInsurance>().ToTable("MasterWorkersCompInsurance");
            modelBuilder.Entity<WorkersCompLegalFirm>().ToTable("MasterWorkersCompLegalFirm");
            modelBuilder.Entity<WorkersCompTCM>().ToTable("MasterWorkersCompTCM");
            modelBuilder.Entity<WorkersCompVOCRehab>().ToTable("MasterWorkersCompVOCRehab");
        }
    }
}