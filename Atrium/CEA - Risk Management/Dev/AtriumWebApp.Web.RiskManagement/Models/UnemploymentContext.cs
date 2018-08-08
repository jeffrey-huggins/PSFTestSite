using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class UnemploymentContext : SharedContext
    {
        public UnemploymentContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<UnemploymentClaim> UnemploymentClaims { get; set; }
        public DbSet<UnemploymentBenefit> UnemploymentBenefits { get; set; }
        public DbSet<UnemploymentClaimNotes> UnemploymentClaimNotes { get; set; }
        public DbSet<UnemploymentPayoutSummary> UnemploymentPayoutSummaries { get; set; }
        public DbSet<UnemploymentClaimReason> UnemploymentClaimReasons { get; set; }
        public DbSet<UnemploymentClaimPaymentPeriod> PaymentPeriods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UnemploymentClaim>().ToTable("UnemploymentClaim");
            modelBuilder.Entity<UnemploymentBenefit>().ToTable("UnemploymentBenefit");
            modelBuilder.Entity<UnemploymentBenefit>().Property(ub => ub.ClaimId).HasColumnOrder(0);
            modelBuilder.Entity<UnemploymentBenefit>().Property(ub => ub.BenefitKey).HasColumnOrder(1);
            modelBuilder.Entity<UnemploymentClaimNotes>().ToTable("UnemploymentClaimNotes");
            modelBuilder.Entity<UnemploymentPayoutSummary>().ToTable("UnemploymentPayoutSummary");
            modelBuilder.Entity<UnemploymentClaimReason>().ToTable("SystemUnemploymentClaimReason");
            modelBuilder.Entity<UnemploymentClaimPaymentPeriod>().ToTable("SystemUnemploymentClaimPaymentPeriod");
        }
    }
}