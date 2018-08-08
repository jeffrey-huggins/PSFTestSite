using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class PurchaseOrderItemViewModel
    {
        //public bool IsAdministrator { get; set; }
        public bool CanManageBudget { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public int Id { get; set; }

        [DisplayName("Order Quantity")]
        public int OrderQuantity { get; set; }
        [StringLength(16)]
        [DisplayName("Stock Number")]
        public string StockNumber { get; set; }
        [StringLength(256)]
        [Required]
        [DisplayName("Item Description")]
        public string ItemDescription { get; set; }
        [StringLength(16)]
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        [StringLength(16)]
        public string Unit { get; set; }
        public decimal TotalCost
        {
            get
            {
                if (ActualCost.HasValue)
                {
                    return ActualCost.Value * OrderQuantity;
                }

                return EstimatedCost * OrderQuantity;
            }
        }
    }
}