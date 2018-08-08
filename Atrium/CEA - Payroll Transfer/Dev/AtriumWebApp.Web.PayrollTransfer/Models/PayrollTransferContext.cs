using AtriumWebApp.Models;
using System.Data.Entity;

namespace AtriumWebApp.Web.PayrollTransfer.Models
{
    public class PayrollTransferContext : SharedContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<AtriumWebApp.Web.PayrollTransfer.Models.PayrollTransferContext>());
        public PayrollTransferContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<EmployeePayrollTransfer> EmployeePayrollTransfers { get; set; }
        public DbSet<ContractorPayrollTransfer> ContractorPayrollTransfers { get; set; }
        public DbSet<PTContractor> PTContractors { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeePayrollTransfer>().ToTable("EmployeePayrollTransfer");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.Id).HasColumnName("EmployeePayrollTransferId");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.EmployeeId).HasColumnName("EmployeeId");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.SourceCommunityId).HasColumnName("SourceCommunityId");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.SourceGeneralLedgerId).HasColumnName("SourceGeneralLedgerId");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.TransferDate).HasColumnName("TransferDate");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.DestinationCommunityId).HasColumnName("DestinationCommunityId");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.DestinationGeneralLedgerId).HasColumnName("DestinationGeneralLedgerId");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.HourCnt).HasColumnName("HourCnt");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.PayAmt).HasColumnName("PayAmt");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.PayType).HasColumnName("PayType");
            modelBuilder.Entity<EmployeePayrollTransfer>().Property(p => p.PBJOnlyFlg).HasColumnName("PBJOnlyFlg");

            modelBuilder.Entity<PTContractor>().ToTable("MasterPTContractor");
            modelBuilder.Entity<PTContractor>().HasMany(p => p.Communities).WithMany().Map(p => {
                p.ToTable("PTContractorCommunityInfo");
                p.MapLeftKey("PTContractorId");
                p.MapRightKey("CommunityId");
            });

            modelBuilder.Entity<ContractorPayrollTransfer>().ToTable("ContractorPayrollTransfer");

            //modelBuilder.Entity<PayrollEmployee>().ToTable("MasterEmployee");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.EmployeeId).HasColumnName("EmployeeId");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.SrcSystemEmployeeId).HasColumnName("SrcSystemEmployeeId");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.SrcSystemCompanyId).HasColumnName("SrcSystemCompanyId");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.SrcSystemName).HasColumnName("SrcSystemName");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.FirstName).HasColumnName("FirstName");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.LastName).HasColumnName("LastName");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.SocialSecurityNumber).HasColumnName("SocialSecurityNumber");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.CommunityId).HasColumnName("CommunityId");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.BirthDate).HasColumnName("BirthDate");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.Gender).HasColumnName("Gender");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.EmployeeStatus).HasColumnName("EmployeeStatus");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.HireDate).HasColumnName("HireDate");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.TerminationDate).HasColumnName("TerminationDate");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.TerminationType).HasColumnName("TerminationType");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.JobClass).HasColumnName("JobClass");
            //modelBuilder.Entity<PayrollEmployee>().Property(p => p.JobClassGeneralLedgerId).HasColumnName("JobClassGeneralLedgerId");
        }
    }
}
