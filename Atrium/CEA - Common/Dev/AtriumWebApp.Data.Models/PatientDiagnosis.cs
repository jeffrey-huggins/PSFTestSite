using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class PatientDiagnosis
    {
        [Key]
        public int PatientDiagnosisId { get; set; }
        public string SrcSystemDiagnosisId { get; set; }
        public string SrcSystemName { get; set; }
        public int ICD9Id { get; set; }
        public int PatientId { get; set; }
        public bool IsAdmissionFlg { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

		[ForeignKey("ICD9Id")]
		public virtual ICD9 ICD9 { get; set; }
    }
}