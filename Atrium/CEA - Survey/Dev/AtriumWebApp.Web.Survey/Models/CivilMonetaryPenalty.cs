using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class CivilMonetaryPenalty
    {
        [Key]
        public int CMPId { get; set; }
        [StringLength(256)]
        public string SurveyCycleId { get; set; }
        public int SurveyId { get; set; }
        public DateTime? CMPStartDate { get; set; }
        public DateTime? CMPEndDate { get; set; }
        public DateTime? CMPInstanceDate { get; set; }
        public decimal CMPAmount { get; set; }
        public bool DailyFlg { get; set; }
        public bool InstanceFlg { get; set; }
        public bool DiscountFlg { get; set; }
    }
}