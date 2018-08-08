using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Web.Survey.Models
{
    public class SurveyContext : BaseSurveyContext
    {
        public SurveyContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public virtual IDbSet<CommunitySurvey> CommunitySurveys { get; set; }
        public virtual IDbSet<CommunitySurveyType> CommunitySurveyTypes { get; set; }
        public virtual IDbSet<FederalCitation> FederalCitations { get; set; }
        public virtual IDbSet<SafetyCitation> SafetyCitations { get; set; }
        public virtual IDbSet<StateCitation> StateCitations { get; set; }
        public virtual IDbSet<CivilMonetaryPenalty> CivilMonetaryPenalties { get; set; }
        public virtual IDbSet<ScopeAndSeverity> ScopeAndSeverities { get; set; }
        public virtual IDbSet<SurveyDocument> SurveyDocuments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CommunitySurvey>().ToTable("CommunitySurvey");
            modelBuilder.Entity<CommunitySurvey>().Property(s => s.SurveyCycleId).HasColumnOrder(0);
            modelBuilder.Entity<CommunitySurvey>().Property(s => s.SurveyId).HasColumnOrder(1);
            modelBuilder.Entity<CommunitySurveyType>().ToTable("MasterCommunitySurveyType");
            modelBuilder.Entity<FederalCitation>().ToTable("FederalCitation");
            modelBuilder.Entity<FederalCitation>().Property(e => e.Id).HasColumnName("FederalCitationId");
            modelBuilder.Entity<SafetyCitation>().ToTable("SafetyCitation");
            modelBuilder.Entity<SafetyCitation>().Property(e => e.Id).HasColumnName("SafetyCitationId");
            modelBuilder.Entity<StateCitation>().ToTable("StateCitation");
            modelBuilder.Entity<StateCitation>().Property(e => e.Id).HasColumnName("StateCitationId");
            modelBuilder.Entity<CivilMonetaryPenalty>().ToTable("CivilMonetaryPenalty");
            modelBuilder.Entity<ScopeAndSeverity>().ToTable("MasterScopeAndSeverity");
            MapSurveytDocument(modelBuilder);
        }

        private static void MapSurveytDocument(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SurveyDocument>().ToTable("CommunitySurveyDocument");
            modelBuilder.Entity<SurveyDocument>().Property(p => p.Id).HasColumnName("CommunitySurveyDocumentId");
            modelBuilder.Entity<SurveyDocument>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SurveyDocument>().Property(p => p.SurveyCycleId).HasColumnName("SurveyCycleId");
            modelBuilder.Entity<SurveyDocument>().Property(p => p.SurveyId).HasColumnName("SurveyId");
            modelBuilder.Entity<SurveyDocument>().Property(p => p.FileName).HasColumnName("DocumentFileName");
            modelBuilder.Entity<SurveyDocument>().Property(p => p.ContentType).HasColumnName("ContentType");
            modelBuilder.Entity<SurveyDocument>().Property(p => p.Document).HasColumnName("Document");

            //modelBuilder.Entity<SurveyDocument>().Property(p => p.ArchiveFlg).HasColumnName("ArchiveFlg");
            //modelBuilder.Entity<SurveyDocument>().Property(p => p.ArchivedDate).HasColumnName("ArchiveDate").HasColumnType("date");
        }
    }
}