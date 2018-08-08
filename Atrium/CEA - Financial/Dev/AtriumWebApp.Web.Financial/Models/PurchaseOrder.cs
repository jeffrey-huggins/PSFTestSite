using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Financial.Models
{
    public class PurchaseOrder : IValidatableObject
    {
        public PurchaseOrder()
        {
            Items = new List<PurchaseOrderItem>();
            Documents = new List<PurchaseOrderDocument>();
        }

        public int Id { get; set; }

        [StringLength(16)]
        public string CustomPurchaseOrderNumber { get; set; }

        [Required]
        public int CommunityId { get; set; }

        public int? VendorId { get; set; }

        [Required]
        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(256)]
        public string Comments { get; set; }

        [StringLength(64)]
        public string Terms { get; set; }

        public bool IsPurchaseOrder { get; set; }

        public bool IsCapitalExpense { get; set; }

        public bool IsEmergency { get; set; }

        public bool IsSpecialProject { get; set; }

        public DateTime EnterDate { get; set; }

        public DateTime PurchaseDate { get; set; }

        [StringLength(16)]
        public string FreightOnBoard { get; set; }

        [StringLength(16)]
        public string ShipVia { get; set; }

        public DateTime? StartShipping { get; set; }

        public DateTime? CompleteShipping { get; set; }

        public int PurchaseOrderTypeId { get; set; }
        public PurchaseOrderType PurchaseOrderType { get; set; }

        public int PurchaseOrderAssetClassId { get; set; }
        public PurchaseOrderAssetClass PurchaseOrderAssetClass { get; set; }

        [RangeIf(typeof(decimal), "0", "0", "IsTaxIncluded", true)]
        [RangeIf(typeof(decimal), ".01", "79228162514264337593543950335", "IsTaxIncluded", false)]
        public decimal EstimatedTaxCost { get; set; }

        public bool IsTaxIncluded { get; set; }

        [RangeIf(typeof(decimal), "0", "0", "IsFreightIncluded", true)]
        [RangeIf(typeof(decimal), ".01", "79228162514264337593543950335", "IsFreightIncluded", false)]
        public decimal EstimatedFreightCost { get; set; }

        public bool IsFreightIncluded { get; set; }

        [StringLength(256)]
        [Required]
        public string Level1ApproverName { get; set; }

        [StringLength(256)]
        [Required]
        public string Level1ApproverADName { get; set; }

        public DateTime Level1ApprovalDate { get; set; }

        [StringLength(256)]
        [RequiredIf("IsLevel2Approved")]
        public string Level2ApproverName { get; set; }

        [StringLength(256)]
        [RequiredIf("IsLevel2Approved")]
        public string Level2ApproverADName { get; set; }

        [RequiredIf("IsLevel2Approved")]
        public DateTime? Level2ApprovalDate { get; set; }

        public bool IsLevel2Approved
        {
            get { return !string.IsNullOrEmpty(Level2ApproverName) || Level2ApprovalDate.HasValue; }
        }

        [StringLength(256)]
        [RequiredIf("IsLevel3Approved")]
        public string Level3ApproverName { get; set; }

        [StringLength(256)]
        [RequiredIf("IsLevel3Approved")]
        public string Level3ApproverADName { get; set; }

        [RequiredIf("IsLevel3Approved")]
        public DateTime? Level3ApprovalDate { get; set; }

        public bool IsLevel3Approved
        {
            get { return !string.IsNullOrEmpty(Level3ApproverName) || Level3ApprovalDate.HasValue; }
        }

        [StringLength(256)]
        [RequiredIf("IsLevel4Approved")]
        public string Level4ApproverName { get; set; }

        [StringLength(256)]
        [RequiredIf("IsLevel4Approved")]
        public string Level4ApproverADName { get; set; }

        [RequiredIf("IsLevel4Approved")]
        public DateTime? Level4ApprovalDate { get; set; }

        public bool IsLevel4Approved
        {
            get { return !string.IsNullOrEmpty(Level4ApproverName) || Level4ApprovalDate.HasValue; }
        }

        [StringLength(256)]
        [RequiredIf("HasFinalApproval")]
        public string FinalApproverName { get; set; }

        [StringLength(256)]
        [RequiredIf("HasFinalApproval")]
        public string FinalApproverADName { get; set; }

        [RequiredIf("HasFinalApproval")]
        public DateTime? FinalApprovalDate { get; set; }

        public bool HasFinalApproval
        {
            get { return !string.IsNullOrEmpty(FinalApproverName) || FinalApprovalDate.HasValue; }
        }

        [StringLength(256)]
        [RequiredIf("IsDenied")]
        public string DenierName { get; set; }

        [StringLength(256)]
        [RequiredIf("IsDenied")]
        public string DenierADName { get; set; }

        [RequiredIf("IsDenied")]
        public DateTime? DenialDate { get; set; }

        public bool IsDenied
        {
            get { return !string.IsNullOrEmpty(DenierName) || DenialDate.HasValue; }
        }

        [StringLength(256)]
        public string DenialComments { get; set; }

        public decimal? ActualInvoiceCost { get; set; }

        [RequiredElements]
        public ICollection<PurchaseOrderItem> Items { get; set; }

        public ICollection<PurchaseOrderDocument> Documents { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((IsCapitalExpense && IsPurchaseOrder) || (!IsCapitalExpense && !IsPurchaseOrder))
            {
                yield return new ValidationResult("IsPurchaseOrder or IsCapitalExpense must be true, but not both.", new[] { "IsPurchaseOrder", "IsCapitalExpense" });
            }
        }
    }
}