using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompClaimNotes
    {
        [Key]
        public int ClaimNoteId { get; set; }
        [StringLength(256)]
        public string ClaimId { get; set; }
        [Required]
        [StringLength(4000)]
        public string Notes { get; set; }
        [Required]
        [StringLength(256)]
        public string UserName { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime InsertedDate { get; set; }
    }
}