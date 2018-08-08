using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class MedicalRecordsRequestDocument
    {
        [Key]
        public int MedicalRecordsRequestDocumentId { get; set; }
        public string Requestid { get; set; }
        [Required]
        [MaxLength(256)]
        public string DocumentFileName { get; set; }
        [Required]
        [MaxLength(256)]
        public string ContentType { get; set; }
        public byte[] Document { get; set; }
        [DataType(DataType.Date)]
        public DateTime SentDate { get; set; }
        [ForeignKey("Requestid")]
        public virtual MedicalRecordsRequest Request { get; set; }
    }
}
