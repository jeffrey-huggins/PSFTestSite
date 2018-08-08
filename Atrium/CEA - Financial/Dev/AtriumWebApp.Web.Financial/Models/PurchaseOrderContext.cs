using System.Data.Entity;
using AtriumWebApp.Models;
using System.Collections;
using System.Collections.Generic;

namespace AtriumWebApp.Web.Financial.Models
{
    public class PurchaseOrderContext : SharedContext
    {
        public PurchaseOrderContext(string connString) : base(connString)
        {
            connectionString = connString;
        }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderLineItems { get; set; }
        public DbSet<PurchaseOrderDocument> PurchaseOrderDocuments { get; set; }
        public DbSet<PurchaseOrderType> PurchaseOrderTypes { get; set; }
        public DbSet<PurchaseOrderVendor> PurchaseOrderVendors { get; set; }
        public DbSet<PurchaseOrderVendorClass> PurchaseOrderVendorClasses { get; set; }
        public DbSet<PurchaseOrderAssetClass> PurchaseOrderAssetClasses { get; set; }
        public DbSet<PurchaseOrderApprover> PurchaseOrderApprovers { get; set; }
        public DbSet<BudgetItem> CapitalBudgetItems { get; set; }
        public DbSet<CapitalBudget> CapitalBudgets { get; set; }
        public DbSet<EmergencyBudget> EmergencyBudgets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PurchaseOrder>().ToTable("PurchaseOrder");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Id).HasColumnName("PurchaseOrderId");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.VendorId).HasColumnName("POVendorId");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.CustomPurchaseOrderNumber).HasColumnName("CustomerPurchaseOrderNum");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Description).HasColumnName("PODesc");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Comments).HasColumnName("Comments");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Terms).HasColumnName("Terms");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.IsPurchaseOrder).HasColumnName("PurchaseOrderFlg");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.IsCapitalExpense).HasColumnName("CapitalExpenseFlg");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.IsEmergency).HasColumnName("EmergencyFlg");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.IsSpecialProject).HasColumnName("SpecialProjectFlg");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.EnterDate).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.PurchaseDate).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.StartShipping).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.CompleteShipping).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.FreightOnBoard).HasColumnName("FOB");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.FinalApprovalDate).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.FinalApproverADName).HasColumnName("FinalApproverADDomainName");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.DenialDate).HasColumnName("DenyDate").HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.DenierName).HasColumnName("DeniedByName");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.DenierADName).HasColumnName("DeniedByADDomainName");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.DenialComments).HasColumnName("DenyComments");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.PurchaseOrderTypeId).HasColumnName("POTypeId");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.PurchaseOrderAssetClassId).HasColumnName("POAssetClassId");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.EstimatedTaxCost).HasColumnName("EstTaxCost").HasColumnType("money");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.IsTaxIncluded).HasColumnName("TaxIncludedFlg");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.EstimatedFreightCost).HasColumnName("EstFreightCost").HasColumnType("money");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.IsFreightIncluded).HasColumnName("FreightIncludedFlg");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level1ApprovalDate).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level2ApprovalDate).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level3ApprovalDate).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level4ApprovalDate).HasColumnType("date");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level1ApproverADName).HasColumnName("Level1ApproverADDomainName");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level2ApproverADName).HasColumnName("Level2ApproverADDomainName");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level3ApproverADName).HasColumnName("Level3ApproverADDomainName");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Level4ApproverADName).HasColumnName("Level4ApproverADDomainName");
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.ActualInvoiceCost).HasColumnName("ActualInvoiceCost").HasColumnType("money");

            modelBuilder.Entity<PurchaseOrder>().HasMany(p => p.Items).WithRequired().Map(p => p.MapKey("PurchaseOrderId"));
            modelBuilder.Entity<PurchaseOrder>().HasMany(p => p.Items).WithOptional().HasForeignKey(k => k.PurchaseOrderId);
            modelBuilder.Entity<PurchaseOrder>().HasMany(p => p.Documents).WithRequired().Map(p => p.MapKey("PurchaseOrderId"));

            modelBuilder.Entity<PurchaseOrderDocument>().ToTable("PurchaseOrderDocument");
            modelBuilder.Entity<PurchaseOrderDocument>().Property(p => p.Id).HasColumnName("PurchaseOrderDocumentId");
            modelBuilder.Entity<PurchaseOrderDocument>().Property(p => p.FileName).HasColumnName("DocumentFileName");

            modelBuilder.Entity<PurchaseOrderItem>().ToTable("PurchaseOrderLine");
            modelBuilder.Entity<PurchaseOrderItem>().Property(p => p.Id).HasColumnName("PurchaseOrderLineId");
            //modelBuilder.Entity<PurchaseOrderItem>().HasKey(p => p.PurchaseOrderId);
            modelBuilder.Entity<PurchaseOrderItem>().Property(p => p.PurchaseOrderId).HasColumnName("PurchaseOrderId");
            modelBuilder.Entity<PurchaseOrderItem>().Property(p => p.OrderQuantity).HasColumnName("OrderQty");
            modelBuilder.Entity<PurchaseOrderItem>().Property(p => p.ItemDescription).HasColumnName("ItemDesc");
            modelBuilder.Entity<PurchaseOrderItem>().Property(p => p.EstimatedItemCost).HasColumnType("money");
            modelBuilder.Entity<PurchaseOrderItem>().Ignore(p => p.TotalCost);

            modelBuilder.Entity<PurchaseOrderType>().ToTable("MasterPOType");
            modelBuilder.Entity<PurchaseOrderType>().Property(p => p.Id).HasColumnName("POTypeId");
            modelBuilder.Entity<PurchaseOrderType>().Property(p => p.Name).HasColumnName("POTypeName");
            modelBuilder.Entity<PurchaseOrderType>().Property(p => p.AllowDataEntry).HasColumnName("DataEntryFlg");

            modelBuilder.Entity<PurchaseOrderVendor>().ToTable("MasterPOVendor");
            modelBuilder.Entity<PurchaseOrderVendor>().Property(p => p.Id).HasColumnName("POVendorId");
            modelBuilder.Entity<PurchaseOrderVendor>().Property(p => p.VendorClassId).HasColumnName("POVendorClassId");
            modelBuilder.Entity<PurchaseOrderVendor>().Property(p => p.Email).HasColumnName("eMail");
            modelBuilder.Entity<PurchaseOrderVendor>().Property(p => p.AllowDataEntry).HasColumnName("DataEntryFlg");

            modelBuilder.Entity<PurchaseOrderVendorClass>().ToTable("MasterPOVendorClass");
            modelBuilder.Entity<PurchaseOrderVendorClass>().Property(p => p.Id).HasColumnName("POVendorClassId");
            modelBuilder.Entity<PurchaseOrderVendorClass>().Property(p => p.Name).HasColumnName("POVendorClassName");
            modelBuilder.Entity<PurchaseOrderVendorClass>().Property(p => p.AllowDataEntry).HasColumnName("DataEntryFlg");

            modelBuilder.Entity<PurchaseOrderAssetClass>().ToTable("MasterPOAssetClass");
            modelBuilder.Entity<PurchaseOrderAssetClass>().Property(p => p.Id).HasColumnName("POAssetClassId");
            modelBuilder.Entity<PurchaseOrderAssetClass>().Property(p => p.Name).HasColumnName("POAssetClassName");
            modelBuilder.Entity<PurchaseOrderAssetClass>().Property(p => p.AllowDataEntry).HasColumnName("DataEntryFlg");

            modelBuilder.Entity<PurchaseOrderApprover>().ToTable("MasterPOApprover");
            modelBuilder.Entity<PurchaseOrderApprover>().Property(p => p.Id).HasColumnName("POApproverId");
            modelBuilder.Entity<PurchaseOrderApprover>().Property(p => p.Name).HasColumnName("ADName");

            modelBuilder.Entity<BudgetItem>().ToTable("CapitalBudgetItem");
            modelBuilder.Entity<BudgetItem>().Property(p => p.Id).HasColumnName("CapitalBudgetItemId");
            modelBuilder.Entity<BudgetItem>().Property(p => p.CommunityId).HasColumnName("CommunityId");
            modelBuilder.Entity<BudgetItem>().Property(p => p.BudgetYear).HasColumnName("BudgetYear");
            modelBuilder.Entity<BudgetItem>().Property(p => p.BudgetQtr).HasColumnName("BudgetQtr");
            modelBuilder.Entity<BudgetItem>().Property(p => p.Description).HasColumnName("ItemDesc");
            modelBuilder.Entity<BudgetItem>().Property(p => p.Comments).HasColumnName("Comments");
            modelBuilder.Entity<BudgetItem>().Property(p => p.BudgetAmt).HasColumnName("BudgetAmt");
            modelBuilder.Entity<BudgetItem>().Property(p => p.IsSpecialProject).HasColumnName("SpecialProjectFlg");

            modelBuilder.Entity<CapitalBudget>().ToTable("CapitalBudgetBasis");
            modelBuilder.Entity<CapitalBudget>().Property(p => p.Id).HasColumnName("CapitalBudgetBasisId");
            modelBuilder.Entity<CapitalBudget>().Property(p => p.CommunityId).HasColumnName("CommunityId");
            modelBuilder.Entity<CapitalBudget>().Property(p => p.BudgetYear).HasColumnName("BudgetYear");
            modelBuilder.Entity<CapitalBudget>().Property(p => p.BedCount).HasColumnName("BudgetBedCnt");
            modelBuilder.Entity<CapitalBudget>().Property(p => p.AmtPerBed).HasColumnName("PerBedAmt");

            modelBuilder.Entity<EmergencyBudget>().ToTable("EmergencyBudget");
            modelBuilder.Entity<EmergencyBudget>().Property(p => p.Id).HasColumnName("EmergencyBudgetId");
            modelBuilder.Entity<EmergencyBudget>().Property(p => p.BudgetYear).HasColumnName("BudgetYear");
            modelBuilder.Entity<EmergencyBudget>().Property(p => p.BudgetAmt).HasColumnName("BudgetAmt");
            //modelBuilder.Entity<CapitalBudget>().Property(p => p.EmergencyBudgetAmt).HasColumnName("EmergencyBudgetAmt");
        }

        public IEnumerable<string> GetBudgetYears(int startYear, int endYear)
        {
            return Context.Database.SqlQuery<string>("SELECT DISTINCT CalendarYear FROM [CorporateEnterpriseApplication].[dbo].[SystemDate] WHERE CalendarYear >= '" + startYear + "' AND CalendarYear <= '" + endYear + "' ORDER BY CalendarYear");
        }

        public IEnumerable<string> GetBudgetQuarters()
        {
            return Context.Database.SqlQuery<string>("SELECT DISTINCT CalendarQuarter FROM [CorporateEnterpriseApplication].[dbo].[SystemDate] ORDER BY CalendarQuarter");
        }
    }
}