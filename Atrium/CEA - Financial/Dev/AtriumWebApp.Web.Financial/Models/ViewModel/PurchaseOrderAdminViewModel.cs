using System.Collections.Generic;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class PurchaseOrderAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public IEnumerable<PurchaseOrderType> PurcahseOrderTypes { get; set; }
        public IEnumerable<PurchaseOrderVendor> Vendors { get; set; }
        public IEnumerable<PurchaseOrderVendorClass> VendorClasses { get; set; }
        public IEnumerable<PurchaseOrderAssetClass> AssetClasses { get; set; }
        public IEnumerable<PurchaseOrderApprover> Approvers { get; set; }
        public IEnumerable<string> StateCodes { get; set; }
        public PurchaseOrderType NewType { get { return new PurchaseOrderType(); } }
        public PurchaseOrderVendorClass NewVendorClass { get { return new PurchaseOrderVendorClass(); } }
        public PurchaseOrderAssetClass NewAssetClass { get { return new PurchaseOrderAssetClass(); } }
        public PurchaseOrderApprover NewApprover { get { return new PurchaseOrderApprover(); } }
    }
}