using AtriumWebApp.Models;
using System.Data.Entity;

namespace AtriumWebApp.Web.HR.Models
{
    public class STNATrainingContext : SharedContext 
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<AtriumWebApp.Web.HR.Models.STNATrainingContext>());

        public STNATrainingContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<STNATrainingFacility> STNATrainingFacilities { get; set; }
        //public DbSet<STNATrainingActionItem> STNATrainingActionItems { get; set; }
        //public DbSet<STNATrainingFacilityInteraction> STNATrainingFacilityInteractions { get; set; }
        public DbSet<STNATrainingClass> STNATrainingClasses { get; set; }
        public DbSet<STNATrainingFacilityNote> STNATrainingFacilityNotes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<STNATrainingFacilityInteraction>().ToTable("STNATrainingFacilityInteraction");
            modelBuilder.Entity<STNATrainingClass>().ToTable("STNATrainingClasses");
            modelBuilder.Entity<STNATrainingFacilityNote>().ToTable("STNATrainingFacilityNote");
            
            modelBuilder.Entity<STNATrainingFacility>().ToTable("MasterSTNATrainingFacility");
            modelBuilder.Entity<STNATrainingFacility>().HasMany(f => f.Communities).WithMany().Map(p =>
            {
                p.ToTable("STNATrainingFacilityCommunityInfo");
                p.MapLeftKey("STNATrainingFacilityId");
                p.MapRightKey("CommunityId");
            });

            //modelBuilder.Entity<STNATrainingActionItem>().ToTable("MasterSTNATrainingActionItem");
        }
    }
}
