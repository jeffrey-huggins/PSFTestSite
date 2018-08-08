using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public class QuarterlySystemsReviewContext : SharedContext
    {
        public QuarterlySystemsReviewContext(string connString) : base(connString)
        {
            connectionString = connString;
        }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StandardsOfCareMeasure> StandardsOfCareMeasures { get; set; }
        public DbSet<StandardsOfCareSection> StandardsOfCareSections { get; set; }
        public DbSet<StandardsOfCareSample> StandardsOfCareSamples { get; set; }
        public DbSet<StandardsOfCareQuestion> StandardsOfCareQuestions { get; set; }
        public DbSet<StandardsOfCareAnswer> StandardsOfCareAnswers { get; set; }
        public DbSet<GeneralMeasure> GeneralMeasures { get; set; }
        public DbSet<GeneralSection> GeneralSections { get; set; }
        public DbSet<GeneralSample> GeneralSamples { get; set; }
        public DbSet<GeneralQuestion> GeneralQuestions { get; set; }
        public DbSet<GeneralPatientAnswer> GeneralPatientAnswers { get; set; }
        public DbSet<GeneralAnswer> GeneralAnswers { get; set; }
        public DbSet<AdditionalReviewQuestion> AdditionalQuestions { get; set; }
        public DbSet<AdditionalReviewAnswer> AdditionalAnswers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Review>().ToTable("QuarterlySystemsReview");
            modelBuilder.Entity<Review>().Property(e => e.Id).HasColumnName("QuarterlySystemsReviewId");
            modelBuilder.Entity<Review>().Property(e => e.ReviewDate).HasColumnType("date");
            modelBuilder.Entity<Review>().Property(e => e.BeginSampleDate).HasColumnType("date");
            modelBuilder.Entity<Review>().Property(e => e.EndSampleDate).HasColumnType("date");
            modelBuilder.Entity<Review>().Property(e => e.IsClosedByNurse).HasColumnName("NursingClosedFlg");
            modelBuilder.Entity<Review>().Property(e => e.ClosedByNurseDate).HasColumnName("NursingClosedDate").HasColumnType("date");
            modelBuilder.Entity<Review>().Property(e => e.ClosedByNurseSignature).HasColumnName("NursingClosedName");
            modelBuilder.Entity<Review>().Property(e => e.IsClosedByDietitian).HasColumnName("DietaryClosedFlg");
            modelBuilder.Entity<Review>().Property(e => e.ClosedByDietitianDate).HasColumnName("DietaryClosedDate").HasColumnType("date");
            modelBuilder.Entity<Review>().Property(e => e.ClosedByDietitianSignature).HasColumnName("DietaryClosedName");
            modelBuilder.Ignore<BaseReviewMeasure>();
            modelBuilder.Ignore<BaseReviewSection>();
            modelBuilder.Ignore<BaseReviewSample>();
            modelBuilder.Ignore<BaseReviewQuestion>();
            modelBuilder.Ignore<BaseReviewAnswer>();
            modelBuilder.Entity<StandardsOfCareMeasure>().ToTable("SystemQSRSOCMeasure");
            modelBuilder.Entity<StandardsOfCareMeasure>().Property(e => e.Id).HasColumnName("QSRSOCMeasureId");
            modelBuilder.Entity<StandardsOfCareMeasure>().Property(e => e.Name).HasColumnName("QSRSOCMeasureName");
            modelBuilder.Entity<StandardsOfCareMeasure>().Property(e => e.Code).HasColumnName("QSRSOCMeasureCode");
            modelBuilder.Entity<StandardsOfCareMeasure>().Property(e => e.ThresholdBonusScore).HasColumnName("ThresholdBonusScoreNb");
            modelBuilder.Entity<StandardsOfCareSection>().ToTable("QSRSOCSection");
            modelBuilder.Entity<StandardsOfCareSection>().Property(e => e.Id).HasColumnName("QSRSOCSectionId");
            modelBuilder.Entity<StandardsOfCareSection>().Property(e => e.ReviewId).HasColumnName("QuarterlySystemsReviewId");
            modelBuilder.Entity<StandardsOfCareSection>().Property(e => e.ReviewMeasureId).HasColumnName("QSRSOCMeasureId");
            modelBuilder.Entity<StandardsOfCareSample>().ToTable("QSRSOCPatientSample");
            modelBuilder.Entity<StandardsOfCareSample>().Property(e => e.Id).HasColumnName("QSRSOCPatientSampleId");
            modelBuilder.Entity<StandardsOfCareSample>().Property(e => e.ReviewSectionId).HasColumnName("QSRSOCSectionId");
            modelBuilder.Entity<StandardsOfCareSample>().Property(e => e.ResidentId).HasColumnName("PatientId");
            modelBuilder.Entity<StandardsOfCareSample>().HasRequired(e => e.Resident).WithMany().HasForeignKey(e => e.ResidentId);
            modelBuilder.Entity<StandardsOfCareQuestion>().ToTable("SystemQSRSOCQuestion");
            modelBuilder.Entity<StandardsOfCareQuestion>().Property(e => e.Id).HasColumnName("QSRSOCQuestionId");
            modelBuilder.Entity<StandardsOfCareQuestion>().Property(e => e.ReviewMeasureId).HasColumnName("QSRSOCMeasureId");
            modelBuilder.Entity<StandardsOfCareQuestion>().Property(e => e.Text).HasColumnName("QSRSOCQuestionText");
            modelBuilder.Entity<StandardsOfCareQuestion>().Property(e => e.MaxScore).HasColumnName("MaxScoreNb");
            modelBuilder.Entity<StandardsOfCareAnswer>().ToTable("QSRSOCAnswer");
            modelBuilder.Entity<StandardsOfCareAnswer>().Property(e => e.Id).HasColumnName("QSRSOCAnswerId");
            modelBuilder.Entity<StandardsOfCareAnswer>().Property(e => e.ReviewSampleId).HasColumnName("QSRSOCPatientSampleId");
            modelBuilder.Entity<StandardsOfCareAnswer>().Property(e => e.ReviewQuestionId).HasColumnName("QSRSOCQuestionId");
            modelBuilder.Entity<StandardsOfCareAnswer>().Property(e => e.IsCompliant).HasColumnName("CompliantFlg");
            modelBuilder.Entity<StandardsOfCareAnswer>().Property(e => e.EarnedScore).HasColumnName("EarnedScoreNb");
            modelBuilder.Entity<GeneralMeasure>().ToTable("SystemQSROtherReview");
            modelBuilder.Entity<GeneralMeasure>().Property(e => e.Id).HasColumnName("QSROtherReviewId");
            modelBuilder.Entity<GeneralMeasure>().Property(e => e.Name).HasColumnName("QSROtherName");
            modelBuilder.Entity<GeneralMeasure>().Property(e => e.Code).HasColumnName("QSROtherCode");
            modelBuilder.Entity<GeneralMeasure>().Property(e => e.RequiresPatientSample).HasColumnName("PatientListFlg");
            modelBuilder.Entity<GeneralSection>().ToTable("QSROtherSection");
            modelBuilder.Entity<GeneralSection>().Property(e => e.Id).HasColumnName("QSROtherSectionId");
            modelBuilder.Entity<GeneralSection>().Property(e => e.ReviewId).HasColumnName("QuarterlySystemsReviewId");
            modelBuilder.Entity<GeneralSection>().Property(e => e.ReviewMeasureId).HasColumnName("QSROtherReviewId");
            modelBuilder.Entity<GeneralSample>().ToTable("QSROtherPatientSample");
            modelBuilder.Entity<GeneralSample>().Property(e => e.Id).HasColumnName("QSROtherPatientSampleId");
            modelBuilder.Entity<GeneralSample>().Property(e => e.ReviewSectionId).HasColumnName("QSROtherSectionId");
            modelBuilder.Entity<GeneralSample>().Property(e => e.ResidentId).HasColumnName("PatientId");
            modelBuilder.Entity<GeneralSample>().HasRequired(e => e.Resident).WithMany().HasForeignKey(e => e.ResidentId);
            modelBuilder.Entity<GeneralQuestion>().ToTable("SystemQSROtherQuestion");
            modelBuilder.Entity<GeneralQuestion>().Property(e => e.Id).HasColumnName("QSROtherQuestionId");
            modelBuilder.Entity<GeneralQuestion>().Property(e => e.ReviewMeasureId).HasColumnName("QSROtherReviewId");
            modelBuilder.Entity<GeneralQuestion>().Property(e => e.Text).HasColumnName("QSROtherQuestionText");
            modelBuilder.Entity<GeneralQuestion>().Property(e => e.MaxScore).HasColumnName("MaxScoreNb");
            modelBuilder.Entity<GeneralAnswer>().ToTable("QSROtherAnswer");
            modelBuilder.Entity<GeneralAnswer>().Property(e => e.Id).HasColumnName("QSROtherAnswerId");
            modelBuilder.Entity<GeneralAnswer>().Property(e => e.ReviewSectionId).HasColumnName("QSROtherSectionId");
            modelBuilder.Entity<GeneralAnswer>().Property(e => e.ReviewQuestionId).HasColumnName("QSROtherQuestionId");
            modelBuilder.Entity<GeneralAnswer>().Property(e => e.IsCompliant).HasColumnName("CompliantFlg");
            modelBuilder.Entity<GeneralAnswer>().Property(e => e.EarnedScore).HasColumnName("EarnedScoreNb");
            modelBuilder.Entity<GeneralPatientAnswer>().ToTable("QSROtherPatientAnswer");
            modelBuilder.Entity<GeneralPatientAnswer>().Property(e => e.Id).HasColumnName("QSROtherPatientAnswerId");
            modelBuilder.Entity<GeneralPatientAnswer>().Property(e => e.ReviewSampleId).HasColumnName("QSROtherPatientSampleId");
            modelBuilder.Entity<GeneralPatientAnswer>().Property(e => e.ReviewQuestionId).HasColumnName("QSROtherQuestionId");
            modelBuilder.Entity<GeneralPatientAnswer>().Property(e => e.IsCompliant).HasColumnName("CompliantFlg");
            modelBuilder.Entity<GeneralPatientAnswer>().Property(e => e.EarnedScore).HasColumnName("EarnedScoreNb");
            modelBuilder.Entity<AdditionalReviewQuestion>().ToTable("SystemQSRAdditionalQuestion");
            modelBuilder.Entity<AdditionalReviewQuestion>().Property(e => e.Id).HasColumnName("QSRAdditionalQuestionId");
            modelBuilder.Entity<AdditionalReviewQuestion>().Property(e => e.Text).HasColumnName("QSRAdditionalQuestionText");
            modelBuilder.Entity<AdditionalReviewQuestion>().Property(e => e.Score).HasColumnName("ScoreNb");
            modelBuilder.Entity<AdditionalReviewAnswer>().ToTable("QSRAdditionalAnswer");
            modelBuilder.Entity<AdditionalReviewAnswer>().Property(e => e.Id).HasColumnName("QSRAdditionalAnswerId");
            modelBuilder.Entity<AdditionalReviewAnswer>().Property(e => e.ReviewId).HasColumnName("QuarterlySystemsReviewId");
            modelBuilder.Entity<AdditionalReviewAnswer>().Property(e => e.ReviewQuestionId).HasColumnName("QSRAdditionalQuestionId");
            modelBuilder.Entity<AdditionalReviewAnswer>().Property(e => e.IsCompliant).HasColumnName("CompliantFlg");
            modelBuilder.Entity<AdditionalReviewAnswer>().Property(e => e.EarnedScore).HasColumnName("EarnedScoreNb");
        }
    }
}