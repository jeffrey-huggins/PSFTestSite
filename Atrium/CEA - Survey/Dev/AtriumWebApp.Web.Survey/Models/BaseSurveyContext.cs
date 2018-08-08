using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseSurveyContext : SharedContext
    {
        public BaseSurveyContext(string connString) : base(connString)
        {
            connectionString = connString;
        }
        public DbSet<DeficiencyGroup> DeficiencyGroups { get; set; }
        public DbSet<FederalDeficiency> FederalDeficiencies { get; set; }
        public DbSet<SafetyDeficiency> SafetyDeficiencies { get; set; }
        public DbSet<StateDeficiency> StateDeficiencies { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<BaseDeficiency>();
            modelBuilder.Entity<DeficiencyGroup>().ToTable("MasterDeficiencyGroup");
            modelBuilder.Entity<DeficiencyGroup>().Property(e => e.Id).HasColumnName("DeficiencyGroupId");
            modelBuilder.Entity<DeficiencyGroup>().Property(e => e.GroupType).HasColumnName("DeficiencyGroupType");
            modelBuilder.Entity<DeficiencyGroup>().Property(e => e.Description).HasColumnName("DeficiencyGroupDesc");
            modelBuilder.Entity<FederalDeficiency>().ToTable("MasterFederalDeficiency");
            modelBuilder.Entity<FederalDeficiency>().Property(e => e.Id).HasColumnName("FederalDeficiencyId");
            modelBuilder.Entity<FederalDeficiency>().Property(e => e.TagCode).HasColumnName("FederalTagCode");
            modelBuilder.Entity<FederalDeficiency>().Property(e => e.Description).HasColumnName("FederalTagDesc");
            modelBuilder.Entity<SafetyDeficiency>().ToTable("MasterSafetyDeficiency");
            modelBuilder.Entity<SafetyDeficiency>().Property(e => e.Id).HasColumnName("SafetyDeficiencyId");
            modelBuilder.Entity<SafetyDeficiency>().Property(e => e.TagCode).HasColumnName("SafetyTagCode");
            modelBuilder.Entity<SafetyDeficiency>().Property(e => e.Description).HasColumnName("SafetyTagDesc");
            modelBuilder.Entity<StateDeficiency>().ToTable("MasterStateDeficiency");
            modelBuilder.Entity<StateDeficiency>().Property(e => e.Id).HasColumnName("StateDeficiencyId");
            modelBuilder.Entity<StateDeficiency>().Property(e => e.TagCode).HasColumnName("StateTagCode");
            modelBuilder.Entity<StateDeficiency>().Property(e => e.Description).HasColumnName("StateTagDesc");
            modelBuilder.Entity<StateDeficiency>().Property(e => e.StateCode).HasColumnName("StateCd");
            modelBuilder.Entity<StateDeficiency>().HasRequired(e => e.State).WithMany().HasForeignKey(e => e.StateCode);
            modelBuilder.Entity<StateDeficiency>().Ignore(e => e.Instructions);
        }
    }
}