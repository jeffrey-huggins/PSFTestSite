using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class PurchaseOrderViewModel : IValidatableObject
    {
        public PurchaseOrderViewModel()
        {
            Items = new List<PurchaseOrderItemViewModel>();
            Documents = new List<PurchaseOrderDocumentViewModel>();
        }

        //public bool IsAdministrator { get; set; }
        public bool CanManageBudget { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public string CurrentYear { get; set; }

        [DisplayName("Purchase Id")]
        public int PurchaseOrderId { get; set; }

        [DisplayName("Purchase Order")]
        [StringLength(16)]
        public string CustomPurchaseOrderNumber { get; set; }

        [StringLength(256)]
        [RequiredIf("IsEmergency", true, ErrorMessage = "Comments are required for emergency purchase orders.")]
        public string Comments { get; set; }

        [StringLength(64)]
        public string Terms { get; set; }

        [Required]
        [StringLength(512)]
        public string Description { get; set; }

        [Required]
        public int CommunityId { get; set; }

        public Community Community { get; set; }

        //[Required]
        [RequiredIf("IsEmergency", DesiredValue = false)]
        [DisplayName("Vendor")]
        public int? VendorId { get; set; }

        public PurchaseOrderVendor Vendor { get; set; }

        [DisplayName("Purchase Order")]
        public bool IsPurchaseOrder { get; set; }

        [DisplayName("Capital Expense")]
        public bool IsCapitalExpense { get; set; }

        [DisplayName("Emergency")]
        public bool IsEmergency { get; set; }

        [DisplayName("Special Project")]
        public bool IsSpecialProject { get; set; }

        [DisplayName("Enter Date")]
        public DateTime EnterDate { get; set; }

        [DisplayName("Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [StringLength(16)]
        [DisplayName("F.O.B.")]
        public string FreightOnBoard { get; set; }

        [StringLength(16)]
        [DisplayName("Ship Via")]
        public string ShipVia { get; set; }

        [DisplayName("Start Shipping")]
        public DateTime? StartShipping { get; set; }

        [DisplayName("Complete Shipping")]
        public DateTime? CompleteShipping { get; set; }

        [DisplayName("Purchase Type")]
        public int PurchaseOrderTypeId { get; set; }

        [DisplayName("Asset Class")]
        public int PurchaseOrderAssetClassId { get; set; }

        [DisplayName("Sales Tax")]
        [RangeIf(typeof(decimal), "0", "0", "IsTaxIncluded", true, ErrorMessage = "{0} must be zero if it is included in the item cost.")]
        [RangeIf(typeof(decimal), ".01", "79228162514264337593543950335", "IsTaxIncluded", false, ErrorMessage = "{0} must have a value if it is not included in the item cost.")]
        public decimal EstimatedTaxCost { get; set; }

        [DisplayName("Included?")]
        public bool IsTaxIncluded { get; set; }

        [DisplayName("Freight Est.")]
        [RangeIf(typeof(decimal), "0", "0", "IsFreightIncluded", true, ErrorMessage = "{0} must be zero if it is included in the item cost.")]
        [RangeIf(typeof(decimal), ".01", "79228162514264337593543950335", "IsFreightIncluded", false, ErrorMessage = "{0} must have a value if it is not included in the item cost.")]
        public decimal EstimatedFreightCost { get; set; }

        [DisplayName("Included?")]
        public bool IsFreightIncluded { get; set; }

        public decimal EstimatedTotalCost
        {
            get { return EstimatedFreightCost + EstimatedTaxCost + Subtotal; }
        }

        [DisplayName("Administrator Approval")]
        public string Level1Approver { get; set; }

        [DisplayName("Date")]
        public DateTime Level1ApprovalDate { get; set; }

        public string Level2Approver { get; set; }

        [DisplayName("Date")]
        public DateTime? Level2ApprovalDate { get; set; }

        public bool IsLevel2Approved
        {
            get { return !string.IsNullOrEmpty(Level2Approver) && Level2ApprovalDate.HasValue; }
        }

        public bool IsLevel2ApprovalAllowed { get; set; }

        [DisplayName("Home Office/Divisional Approval")]
        public bool IsLevel2MarkedForApproval { get; set; }

        public string Level3Approver { get; set; }

        [DisplayName("Date")]
        public DateTime? Level3ApprovalDate { get; set; }

        public bool IsLevel3Approved
        {
            get { return !string.IsNullOrEmpty(Level3Approver) && Level3ApprovalDate.HasValue; }
        }

        public bool IsLevel3ApprovalAllowed { get; set; }

        [DisplayName("Senior Vice President of Operation")]
        public bool IsLevel3MarkedForApproval { get; set; }

        public string Level4Approver { get; set; }

        [DisplayName("Date")]
        public DateTime? Level4ApprovalDate { get; set; }

        public bool IsLevel4Approved
        {
            get { return !string.IsNullOrEmpty(Level4Approver) && Level4ApprovalDate.HasValue; }
        }

        public bool IsLevel4ApprovalAllowed { get; set; }

        [DisplayName("Chief Financial Officer")]
        public bool IsLevel4MarkedForApproval { get; set; }

        public string FinalApprover { get; set; }

        [DisplayName("Date")]
        public DateTime? FinalApprovalDate { get; set; }

        public bool HasFinalApproval
        {
            get { return !string.IsNullOrEmpty(FinalApprover) && FinalApprovalDate.HasValue; }
        }

        [DisplayName("Final Approval")]
        public bool IsMarkedForFinalApproval { get; set; }

        public string Denier { get; set; }

        [DisplayName("Date")]
        public DateTime? DenialDate { get; set; }

        public bool IsDenied
        {
            get { return !string.IsNullOrEmpty(Denier) && DenialDate.HasValue; }
        }

        [DisplayName("Deny")]
        public bool IsMarkedForDenial { get; set; }

        [DisplayName("Comments")]
        [StringLength(256)]
        public string DenialComments { get; set; }

        //[DisplayName("Purchase Order Invoice Amount")]
        [DisplayName("Final Invoiced Amount")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? ActualInvoiceCost { get; set; }

        [DisplayName("Purchase Order Item")]
        [RequiredElements]
        public ICollection<PurchaseOrderItemViewModel> Items { get; set; }

        public ICollection<PurchaseOrderDocumentViewModel> Documents { get; set; }

        public decimal Subtotal
        {
            get
            {
                if (Items == null)
                {
                    return 0;
                }
                return Items.Sum(i => i.TotalCost);
            }
        }

        public bool CanBeDeleted
        {
            get { return CanDelete || !(IsLevel2Approved || IsLevel3Approved || IsLevel4Approved || HasFinalApproval || IsDenied); }
        }

        public bool CanBeEdited
        {
            get { return CanEdit || !(HasFinalApproval || IsDenied); }
        }

        public IEnumerable<Community> Communities { get; set; }
        public IEnumerable<PurchaseOrderType> PurchaseOrderTypes { get; set; }
        public IEnumerable<PurchaseOrderVendor> Vendors { get; set; }
        public IEnumerable<PurchaseOrderAssetClass> AssetClasses { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((IsCapitalExpense && IsPurchaseOrder) || (!IsCapitalExpense && !IsPurchaseOrder))
            {
                yield return new ValidationResult("Please select either Purchase Order or Capital Expense, but not both.");
            }
        }
    }
}