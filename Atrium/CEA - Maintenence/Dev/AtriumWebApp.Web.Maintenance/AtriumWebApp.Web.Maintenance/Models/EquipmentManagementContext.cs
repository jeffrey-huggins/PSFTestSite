using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Maintenance.Models
{
    public class EquipmentManagementContext : SharedContext
    {
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<EquipmentInspection> Inspections { get; set; }
        public DbSet<EquipmentInspectionDocument> InspectionDocuments { get; set; }
        public DbSet<EquipmentMaintenancePlan> MaintenancePlans { get; set; }
        public DbSet<EquipmentRepair> Repairs { get; set; }
        public DbSet<EquipmentRepairDocument> RepairDocuments { get; set; }
        public DbSet<PurchaseOrderVendor> Vendors { get; set; }

        public EquipmentManagementContext() : base()
        {

        }

        public EquipmentManagementContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Equipment>().ToTable("Equipment");
            modelBuilder.Entity<EquipmentInspection>().ToTable("EquipmentInspection");
            modelBuilder.Entity<EquipmentInspectionDocument>().ToTable("EquipmentInspectionDocument");
            modelBuilder.Entity<EquipmentMaintenancePlan>().ToTable("EquipmentMaintenancePlan");
            modelBuilder.Entity<EquipmentRepair>().ToTable("EquipmentRepair");
            modelBuilder.Entity<EquipmentRepairDocument>().ToTable("EquipmentRepairDocument");
            modelBuilder.Entity<PurchaseOrderVendor>().ToTable("MasterPOVendor");
        }
    }
}
