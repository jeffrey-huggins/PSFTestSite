using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class DischargeReason
    {
        [Key]
        public int DischargeReasonId { get; set; }
        [MaxLength(128)]
        [Required]
        public string DischargeReasonDesc { get; set; }
        public bool ERDataEntryFlg { get; set; }
        public bool HospitalDataEntryFlg { get; set; }
        public bool ReportFlg { get; set; }
        public int SortOrder { get; set; }
        public bool Top_N { get; set; }
    }
}