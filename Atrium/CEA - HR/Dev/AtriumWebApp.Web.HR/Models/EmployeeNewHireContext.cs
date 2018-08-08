using AtriumWebApp.Models;
using System.Data.Entity;

namespace AtriumWebApp.Web.HR.Models
{
    public class EmployeeNewHireContext : SharedContext 
    {

        public EmployeeNewHireContext(string connString) : base(connString)
        {
            connectionString = connString;
        }
		public DbSet<MasterNewHireChecklist> MasterNewHireChecklist { get; set; }
		public DbSet<EmployeeNewHire> EmployeeNewHire { get; set; }
		public DbSet<EmployeeNewHireChecklist> EmployeeNewHireChecklist { get; set; }
		public DbSet<EmployeeNewHireChecklistDocument> EmployeeNewHireChecklistDocument { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeeNewHire>().ToTable("EmployeeNewHire");
			modelBuilder.Entity<EmployeeNewHireChecklist>().ToTable("EmployeeNewHireChecklist");
			modelBuilder.Entity<EmployeeNewHireChecklistDocument>().ToTable("EmployeeNewHireChecklistDocument");
			modelBuilder.Entity<MasterNewHireChecklist>().ToTable("MasterNewHireChecklist");
		}
    }
}
