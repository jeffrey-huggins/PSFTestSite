using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text;

namespace AtriumWebApp.Models
{
    public class SharedContext : DbContext
    {
        public static string connectionString;
        public DbSet<SystemObjectPermission> ObjectPermission { get; set; }
        public DbSet<SystemObjectPermissionRef> ObjectUserPermission { get; set; }
        public DbSet<SystemLogUsage> SystemLogs { get; set; }
        public DbSet<ApplicationInfo> Applications { get; set; }
        public DbSet<ApplicationCommunityInfo> ApplicationCommunityInfos { get; set; }
        public DbSet<Region> MasterRegion { get; set; }
        public DbSet<Community> Facilities { get; set; }
        public DbSet<ResidentGroup> ResidentGroups { get; set; }
        public DbSet<Room> MasterRoom { get; set; }
        public DbSet<Patient> Residents { get; set; }
        public DbSet<PatientDiagnosis> PatientDiagnoses { get; set; }
        public DbSet<PatientDiagnosisICD10> PatientDiagnosesICD10 { get; set; }
        public DbSet<ICD9> MasterICD9 { get; set; }
        public DbSet<ICD10> MasterICD10 { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AtriumPayerGroup> AtriumPayerGroups { get; set; }
        public DbSet<ApplicationCommunityAtriumPayerGroupInfo> PayerGroupInfos { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<MasterApplicationGroup> MasterApplicationGroup { get; set; }
        public DbSet<ApplicationCommunityBusinessUserInfo> ApplicationUserAccess { get; set; }
        public DbSet<MasterBusinessUser> MasterBusinessUsers { get; set; }
        public DbSet<SystemAppAdmin> SystemAppAdmin { get; set; }
        public DbSet<SystemSysAdmin> SystemAdmin { get; set; }

        public DbSet<CommunityPayers> CommunityPayers { get; set; }
        public DbSet<EmployeeJobClass> EmployeeJobClasses { get; set; }
        public DbSet<MasterJobClass> MasterJobClasses { get; set; }
        public DbSet<GeneralLedgerAccount> GLAccounts { get; set; }

        public SharedContext Context
        {
            get { return new SharedContext(); }
        }

        public SharedContext() : base(connectionString)
        {

        }

        public SharedContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SystemLogUsage>().ToTable("SystemLogUsage");
            modelBuilder.Entity<ApplicationInfo>().ToTable("MasterApplication");
            modelBuilder.Entity<ApplicationCommunityInfo>().ToTable("ApplicationCommunityInfo");
            modelBuilder.Entity<ApplicationCommunityInfo>().Property(aci => aci.ApplicationId).HasColumnOrder(0);
            modelBuilder.Entity<ApplicationCommunityInfo>().Property(aci => aci.CommunityId).HasColumnOrder(1);
            modelBuilder.Entity<Region>().ToTable("MasterRegion");
            modelBuilder.Entity<Community>().ToTable("MasterCommunity");
            modelBuilder.Entity<ResidentGroup>().ToTable("MasterPatientGroup");
            modelBuilder.Entity<ResidentGroup>().Property(g => g.Id).HasColumnName("PatientGroupId");
            modelBuilder.Entity<ResidentGroup>().Property(g => g.CommunityId).HasColumnName("CommunityId");
            modelBuilder.Entity<ResidentGroup>().Property(g => g.Name).HasColumnName("PatientGroupName");
            modelBuilder.Entity<Room>().ToTable("MasterRoom");
            modelBuilder.Entity<Room>().Property(r => r.ResidentGroupId).HasColumnName("PatientGroupId");
            modelBuilder.Entity<Patient>().ToTable("MasterPatient");
            modelBuilder.Entity<Patient>().Property(p => p.LastCensusDate).HasColumnType("date");
            modelBuilder.Entity<PatientDiagnosis>().ToTable("PatientDiagnosis");
            modelBuilder.Entity<PatientDiagnosisICD10>().ToTable("PatientDiagnosisICD10");
            modelBuilder.Entity<ICD9>().ToTable("MasterICD9");
            modelBuilder.Entity<ICD10>().ToTable("MasterICD10");
            modelBuilder.Entity<Employee>().ToTable("MasterEmployee");
            modelBuilder.Entity<AtriumPayerGroup>().ToTable("MasterAtriumPayerGroup");
            modelBuilder.Entity<AtriumPayerGroup>().Property(p => p.IsCommunitySurveyEligible).HasColumnName("CommunitySurveyFlg");
            modelBuilder.Entity<ApplicationCommunityAtriumPayerGroupInfo>().ToTable("ApplicationCommunityAtriumPayerGroupInfo");
            modelBuilder.Entity<ApplicationCommunityAtriumPayerGroupInfo>().Property(pgi => pgi.ApplicationId).HasColumnOrder(0);
            modelBuilder.Entity<ApplicationCommunityAtriumPayerGroupInfo>().Property(pgi => pgi.CommunityId).HasColumnOrder(1);
            modelBuilder.Entity<ApplicationCommunityAtriumPayerGroupInfo>().Property(pgi => pgi.AtriumPayerGroupCode).HasColumnOrder(2);
            modelBuilder.Entity<State>().ToTable("MasterState");

            modelBuilder.Entity<CommunityPayers>().ToTable("MasterCommunityPayers");
            modelBuilder.Entity<MasterApplicationGroup>().ToTable("MasterApplicationGroup");

            modelBuilder.Entity<ApplicationCommunityBusinessUserInfo>().ToTable("ApplicationCommunityBusinessUserInfo");
            modelBuilder.Entity<MasterBusinessUser>().ToTable("MasterBusinessUser");
            modelBuilder.Entity<SystemAppAdmin>().ToTable("SystemAppAdmin");

            modelBuilder.Entity<SystemObjectPermission>().ToTable("SystemObjectPermission");
            modelBuilder.Entity<SystemObjectPermissionRef>().ToTable("SystemObjectPermissionReference");

            modelBuilder.Entity<SystemSysAdmin>().ToTable("SystemSysAdmin");

            modelBuilder.Entity<EmployeeJobClass>().ToTable("EmployeeJobClass");

            modelBuilder.Entity<MasterJobClass>().ToTable("MasterJobClass");

            modelBuilder.Entity<GeneralLedgerAccount>().ToTable("SystemGeneralLedger");
            modelBuilder.Entity<GeneralLedgerAccount>().Property(p => p.GeneralLedgerId).HasColumnName("GeneralLedgerId");
            modelBuilder.Entity<GeneralLedgerAccount>().Property(p => p.AccountNbr).HasColumnName("AccountNbr");
            modelBuilder.Entity<GeneralLedgerAccount>().Property(p => p.AccountName).HasColumnName("AccountName");
            modelBuilder.Entity<GeneralLedgerAccount>().Property(p => p.AtriumPayerGroupCode).HasColumnName("AtriumPayerGroupCode");
            modelBuilder.Entity<GeneralLedgerAccount>().Property(p => p.GLAccountTypeID).HasColumnName("GLAccountTypeID");

        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                TraceValidationException(e);
                throw;
            }
        }

        [Conditional("TRACE")]
        private void TraceValidationException(DbEntityValidationException e)
        {
            var message = new StringBuilder("Validation Exceptions on SaveChanges:");
            foreach (var entityError in e.EntityValidationErrors)
            {
                if (!entityError.IsValid)
                {
                    message.AppendLine("\r\n-------------------------------");
                    message.AppendFormat("Entity {0} ({1})", entityError.Entry.Entity.GetType(), entityError.Entry.State);
                    foreach (var error in entityError.ValidationErrors)
                    {
                        message.AppendFormat("\r\nProperty: {0}\r\nError Message:{1}", error.PropertyName, error.ErrorMessage);
                    }
                }
            }
            Trace.WriteLine(message.ToString().TrimEnd(), this.GetType().Name);
        }
    }
}