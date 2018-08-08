using System.Data.Entity;

namespace AtriumWebApp.Web.Survey.Models
{
    public class MockSurveyContext : BaseSurveyContext
    {
        public MockSurveyContext(string connString) : base(connString)
        {
            connectionString = connString;
        }
        public DbSet<MockSurvey> MockSurveys { get; set; }
        public DbSet<MockFederalCitation> MockFederalCitations { get; set; }
        public DbSet<MockSafetyCitation> MockSafetyCitations { get; set; }
        public DbSet<MockSurveyNotificationRecipient> NotificationRecipients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<BaseMockCitation>();
            modelBuilder.Entity<MockSurvey>().ToTable("MockSurvey");
            modelBuilder.Entity<MockSurvey>().Property(e => e.Id).HasColumnName("MockSurveyId");
            modelBuilder.Entity<MockSurvey>().Property(e => e.MockSurveyDate).HasColumnType("date");
            modelBuilder.Entity<MockSurvey>().Property(e => e.FollowUpDate).HasColumnType("date");
            modelBuilder.Entity<MockSurvey>().Property(e => e.PlanOfCorrectionCompleteDate).HasColumnName("POCCompleteDate");
            modelBuilder.Entity<MockSurvey>().Property(e => e.PlanOfCorrectionCompleteDate).HasColumnType("date");
            modelBuilder.Entity<MockSurvey>().Property(e => e.FollowUpCompleteDate).HasColumnName("CompliantDate");
            modelBuilder.Entity<MockSurvey>().Property(e => e.FollowUpCompleteDate).HasColumnType("date");
            modelBuilder.Entity<MockSurvey>().Property(e => e.ClosedSignature).HasColumnName("ClosedName");
            modelBuilder.Entity<MockFederalCitation>().ToTable("MockFederalCitation");
            modelBuilder.Entity<MockFederalCitation>().Property(e => e.Id).HasColumnName("MockFederalCitationId");
            modelBuilder.Entity<MockFederalCitation>().Property(e => e.DeficiencyId).HasColumnName("FederalDeficiencyId");
            modelBuilder.Entity<MockFederalCitation>().Property(e => e.IsCompliant).HasColumnName("CompliantFlg");
            modelBuilder.Entity<MockSafetyCitation>().ToTable("MockSafetyCitation");
            modelBuilder.Entity<MockSafetyCitation>().Property(e => e.Id).HasColumnName("MockSafetyCitationId");
            modelBuilder.Entity<MockSafetyCitation>().Property(e => e.DeficiencyId).HasColumnName("SafetyDeficiencyId");
            modelBuilder.Entity<MockSafetyCitation>().Property(e => e.IsCompliant).HasColumnName("CompliantFlg");
            modelBuilder.Entity<MockSurveyNotificationRecipient>().ToTable("MockSurveyNotification");
            modelBuilder.Entity<MockSurveyNotificationRecipient>().Property(e => e.Id).HasColumnName("MockSurveyNotificationId");
        }
    }
}