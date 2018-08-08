using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractManagementContext : SharedContext
    {
        public ContractManagementContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public virtual IDbSet<ContractProvider> Providers { get; set; }
        public virtual IDbSet<ProviderDocument> Documents { get; set; }
        public virtual IDbSet<ProviderAddress> Addresses { get; set; }
        public virtual IDbSet<ProviderContact> Contacts { get; set; }
        public virtual IDbSet<ProviderAddressType> AddressTypes { get; set; }
        public virtual IDbSet<ProviderContactType> ContactTypes { get; set; }
        public virtual IDbSet<Contract> Contracts { get; set; }
        public virtual IDbSet<ContractDocument> ContractDocuments { get; set; }
        public virtual IDbSet<ContractCategory> ContractCategories { get; set; }
        public virtual IDbSet<ContractSubCategory> ContractSubCategories { get; set; }
        public virtual IDbSet<ContractInfoCommunity> ContractCommunities { get; set; }
        public virtual IDbSet<ContractTemplate> Templates { get; set; }
        public virtual IDbSet<ContractPaymentTerm> ContractPaymentTerms { get; set; }
        public virtual IDbSet<ContractRenewal> ContractRenewals { get; set; }
        public virtual IDbSet<ContractTerminationNotice> ContractTerminationNotices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            MapContractProvider(modelBuilder);
            MapProviderDocument(modelBuilder);
            MapProviderAddress(modelBuilder);
            MapProviderContact(modelBuilder);
            MapProviderAddressType(modelBuilder);
            MapProviderContactType(modelBuilder);
            MapContract(modelBuilder);
            MapContractCategory(modelBuilder);
            MapContractSubCategory(modelBuilder);
            MapContractDocument(modelBuilder);
            MapContractTemplate(modelBuilder);
            MapContractInfoCommunity(modelBuilder);
            MapContractPaymentTerms(modelBuilder);
            MapContractRenewalTypes(modelBuilder);
            MapContractTerminationNotices(modelBuilder);
        }

        private void MapContractPaymentTerms(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractPaymentTerm>().ToTable("MasterContractPaymentTerm");
            modelBuilder.Entity<ContractPaymentTerm>().Property(p => p.Id).HasColumnName("ContractPaymentTermId");
            modelBuilder.Entity<ContractPaymentTerm>().Property(p => p.Name).HasColumnName("ContractPaymentTermDesc");
            modelBuilder.Entity<ContractPaymentTerm>().Property(p => p.IsDataEntry).HasColumnName("DataEntryFlg");
            modelBuilder.Entity<ContractPaymentTerm>().Property(p => p.IsReportable).HasColumnName("ReportFlg");
            modelBuilder.Entity<ContractPaymentTerm>().Property(p => p.SortOrder).HasColumnName("SortOrder");
        }

        private void MapContractRenewalTypes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractRenewal>().ToTable("MasterContractRenewal");
            modelBuilder.Entity<ContractRenewal>().Property(p => p.Id).HasColumnName("ContractRenewalId");
            modelBuilder.Entity<ContractRenewal>().Property(p => p.Name).HasColumnName("ContractRenewalDesc");
            modelBuilder.Entity<ContractRenewal>().Property(p => p.IsDataEntry).HasColumnName("DataEntryFlg");
            modelBuilder.Entity<ContractRenewal>().Property(p => p.IsReportable).HasColumnName("ReportFlg");
            modelBuilder.Entity<ContractRenewal>().Property(p => p.SortOrder).HasColumnName("SortOrder");
        }

        private void MapContractTerminationNotices(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractTerminationNotice>().ToTable("MasterContractTerminationNotice");
            modelBuilder.Entity<ContractTerminationNotice>().Property(p => p.Id).HasColumnName("ContractTerminationNoticeId");
            modelBuilder.Entity<ContractTerminationNotice>().Property(p => p.Name).HasColumnName("ContractTerminationNoticeDesc");
            modelBuilder.Entity<ContractTerminationNotice>().Property(p => p.IsDataEntry).HasColumnName("DataEntryFlg");
            modelBuilder.Entity<ContractTerminationNotice>().Property(p => p.IsReportable).HasColumnName("ReportFlg");
            modelBuilder.Entity<ContractTerminationNotice>().Property(p => p.SortOrder).HasColumnName("SortOrder");
        }

        private void MapContractInfoCommunity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractInfoCommunity>().ToTable("ContractInfoCommunity");

            modelBuilder.Entity<ContractInfoCommunity>()
                .HasKey(pk => new { pk.ContractInfoId, pk.CommunityId });

            modelBuilder.Entity<ContractInfoCommunity>().Property(p => p.ArchiveFlg).HasColumnName("ArchiveFlg");
            modelBuilder.Entity<ContractInfoCommunity>().Property(p => p.ArchiveDate).HasColumnName("ArchiveDate");

            modelBuilder.Entity<ContractInfoCommunity>().Property(p => p.CredentialingDate).HasColumnType("date");
            modelBuilder.Entity<ContractInfoCommunity>().Property(p => p.StartDate).HasColumnType("date");
            modelBuilder.Entity<ContractInfoCommunity>().Property(p => p.EndDate).HasColumnType("date");
        }

        private static void MapContractSubCategory(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractSubCategory>().ToTable("MasterContractSubCategory");
            modelBuilder.Entity<ContractSubCategory>().Property(p => p.Id).HasColumnName("ContractSubCategoryId");
            modelBuilder.Entity<ContractSubCategory>().Property(p => p.Name).HasColumnName("ContractSubCategoryName");
        }

        private static void MapContractCategory(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractCategory>().ToTable("MasterContractCategory");
            modelBuilder.Entity<ContractCategory>().Property(p => p.Id).HasColumnName("ContractCategoryId");
            modelBuilder.Entity<ContractCategory>().HasKey(p => p.Id);
            modelBuilder.Entity<ContractCategory>().Property(p => p.Name).HasColumnName("ContractCategoryName");
            modelBuilder.Entity<ContractCategory>().Property(p => p.IsProvider).HasColumnName("ProviderFlg");
        }

        private void MapContractTemplate(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractTemplate>().ToTable("ContractTemplate");

            modelBuilder.Entity<ContractTemplate>().Property(p => p.Id).HasColumnName("ContractTemplateId");
            modelBuilder.Entity<ContractTemplate>().Property(p => p.Description).HasColumnName("DocumentDesc");
            modelBuilder.Entity<ContractTemplate>().Property(p => p.SavedDate).HasColumnType("date");
        }

        private static void MapContract(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>().ToTable("ContractInfo");

            modelBuilder.Entity<Contract>().Property(p => p.Id).HasColumnName("ContractInfoId");
            modelBuilder.Entity<Contract>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Contract>().HasMany(coms => coms.ContractCommunities).WithRequired().HasForeignKey(fk => fk.ContractInfoId);
            modelBuilder.Entity<Contract>().Property(p => p.ArchiveFlg).HasColumnName("ArchiveFlg");
            modelBuilder.Entity<Contract>().Property(p => p.ArchivedDate).HasColumnName("ArchiveDate").HasColumnType("date");
            modelBuilder.Entity<Contract>().Property(p => p.ContractCategoryId).HasColumnName("ContractCategoryId");

            // http://www.entityframeworktutorial.net/code-first/configure-one-to-one-relationship-in-code-first.aspx

            //// Configure ContractCategoryId as FK for ContractCategory
            //modelBuilder.Entity<Contract>()
            //            .HasOptional(p => p.Category)
            //            .WithRequired(cc => cc.Contract); // Create inverse relationship
            //// Configure ContractSubCategoryId as FK for ContractSubCategory
            //modelBuilder.Entity<Contract>()
            //            .HasOptional(p => p.SubCategory)
            //            .WithRequired(cc => cc.Contract); // Create inverse relationship
        }

        private static void MapContractDocument(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractDocument>().ToTable("ContractDocument");
            modelBuilder.Entity<ContractDocument>().Property(p => p.Id).HasColumnName("ContractDocumentId");
            modelBuilder.Entity<ContractDocument>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ContractDocument>().Property(p => p.ArchiveFlg).HasColumnName("ArchiveFlg");
            modelBuilder.Entity<ContractDocument>().Property(p => p.ArchivedDate).HasColumnName("ArchiveDate").HasColumnType("date");
            modelBuilder.Entity<ContractDocument>().Property(p => p.ContractId).HasColumnName("ContractInfoId");

            modelBuilder.Entity<ContractDocument>().Property(p => p.FileName).HasColumnName("DocumentFileName");
        }

        private static void MapProviderContactType(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderContactType>().ToTable("MasterContractContactType");
            modelBuilder.Entity<ProviderContactType>().Property(p => p.Id).HasColumnName("ContractContactTypeId");
            modelBuilder.Entity<ProviderContactType>().Property(p => p.Name).HasColumnName("ContractContactTypeName");
        }

        private static void MapProviderAddressType(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderAddressType>().ToTable("MasterContractAddressType");
            modelBuilder.Entity<ProviderAddressType>().Property(p => p.Id).HasColumnName("ContractAddressTypeId");
            modelBuilder.Entity<ProviderAddressType>().Property(p => p.Name).HasColumnName("ContractAddressTypeName");
        }

        private static void MapProviderContact(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderContact>().ToTable("ContractProviderContact");
            modelBuilder.Entity<ProviderContact>().Property(p => p.Id).HasColumnName("ContractProviderContactId");
            modelBuilder.Entity<ProviderContact>().Property(p => p.ProviderContactTypeId).HasColumnName("ContractContactTypeId");
            modelBuilder.Entity<ProviderContact>().Property(p => p.Email).HasColumnName("eMail");
        }

        private static void MapProviderAddress(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderAddress>().ToTable("ContractProviderAddress");
            modelBuilder.Entity<ProviderAddress>().Property(p => p.Id).HasColumnName("ContractProviderAddressId");
            modelBuilder.Entity<ProviderAddress>().Property(p => p.ProviderAddressTypeId)
                .HasColumnName("ContractAddressTypeId");
            modelBuilder.Entity<ProviderAddress>().Property(p => p.AddressLineOne)
                .HasColumnName("Address1");
            modelBuilder.Entity<ProviderAddress>().Property(p => p.AddressLineTwo).HasColumnName("Address2");
            modelBuilder.Entity<ProviderAddress>().Property(p => p.StateId).HasColumnName("StateCd");
            modelBuilder.Entity<ProviderAddress>().HasOptional(p => p.State).WithMany().HasForeignKey(p => p.StateId);
        }

        private static void MapProviderDocument(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderDocument>().ToTable("ContractProviderDocument");

            modelBuilder.Entity<ProviderDocument>().Property(p => p.Id)
                .HasColumnName("ContractProviderDocumentId");

            modelBuilder.Entity<ProviderDocument>().Property(p => p.Description)
                .HasColumnName("DocumentDesc");

            modelBuilder.Entity<ProviderDocument>()
                .Property(p => p.SavedDate)
                .HasColumnType("date");
        }

        private static void MapContractProvider(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractProvider>().ToTable("ContractProvider");

            modelBuilder.Entity<ContractProvider>().Property(p => p.Id)
                .HasColumnName("ContractProviderId");

            modelBuilder.Entity<ContractProvider>().Property(p => p.IsActive)
                .HasColumnName("ActiveFlg");

            modelBuilder.Entity<ContractProvider>().Property(p => p.LicenseExpirationDate)
                .HasColumnName("LicenseExpDate")
                .HasColumnType("date");

            modelBuilder.Entity<ContractProvider>().Property(p => p.InsuranceExpirationDate)
                .HasColumnName("InsuranceExpDate")
                .HasColumnType("date");
        }
    }
}