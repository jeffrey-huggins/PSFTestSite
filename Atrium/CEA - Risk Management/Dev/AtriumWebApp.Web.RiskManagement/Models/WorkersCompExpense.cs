using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompExpense
    {
        [Key]
        [Required]
        [StringLength(256)]
        public string ClaimId { get; set; }
        [Key]
        [Required]
        [StringLength(20)]
        public string ExpenseType { get; set; }
        [Key]
        public long ExpenseKey { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal ExpenseAmt { get; set; }
    }
}