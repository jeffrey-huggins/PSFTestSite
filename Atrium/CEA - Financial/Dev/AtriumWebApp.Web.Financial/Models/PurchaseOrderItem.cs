using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.Financial.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int OrderQuantity { get; set; }
        [StringLength(16)]
        public string StockNumber { get; set; }
        [StringLength(512)]
        [Required]
        public string ItemDescription { get; set; }
        [StringLength(16)]
        public string AccountNumber { get; set; }
        public decimal EstimatedItemCost { get; set; }
        [StringLength(16)]
        public string Unit { get; set; }
        public decimal TotalCost
        {
            get
            {
                return EstimatedItemCost * OrderQuantity;
            }
        }
    }
}