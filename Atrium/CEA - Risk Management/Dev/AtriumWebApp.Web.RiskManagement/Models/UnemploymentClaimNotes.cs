using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class UnemploymentClaimNotes
    {
        [Key]
        public int ClaimNoteId { get; set; }
        [StringLength(256)]
        public string ClaimId { get; set; }
        [StringLength(4000)]
        [Required]
        public string Notes { get; set; }
        [StringLength(256)]
        public string UserName { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime InsertedDate { get; set; }
    }
}