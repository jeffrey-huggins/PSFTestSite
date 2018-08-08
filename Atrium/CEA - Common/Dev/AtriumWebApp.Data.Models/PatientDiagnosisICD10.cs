using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class PatientDiagnosisICD10
    {
        [Key]
        public int PatientDiagnosisICD10Id { get; set; }
        public string SrcSystemDiagnosisICD10Id { get; set; }
        public string SrcSystemName { get; set; }
        public int ICD10Id { get; set; }
        public int PatientId { get; set; }
        public bool IsAdmissionFlg { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

		[ForeignKey("ICD10Id")]
		public virtual ICD10 ICD10 { get; set; }
    }
}