using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class Measure
    {
        [Key]
        public int SOCMeasureId { get; set; }
        [Required]
        [MaxLength(60)]
        public string SOCMeasureName { get; set; }
        public bool ResolveRequiredFlg { get; set; }
        public bool DataEntryFlg { get; set; }
        public bool ReportFlg { get; set; }
        public bool PatientCalcFlg { get; set; }
        public int SortOrder { get; set; }
        [MaxLength(10)]
        public string SubAppCode { get; set; }
        public bool CalcMeasureFlg { get; set; }
    }
}