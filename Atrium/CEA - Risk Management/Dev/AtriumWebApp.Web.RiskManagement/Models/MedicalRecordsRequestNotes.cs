using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class MedicalRecordsRequestNotes
    {
        [Key]
        public int RequestNoteId { get; set; }
        public string Requestid { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public DateTime InsertedDate { get; set; }

        [ForeignKey("Requestid")]
        public virtual MedicalRecordsRequest Request { get; set; }
    }
}