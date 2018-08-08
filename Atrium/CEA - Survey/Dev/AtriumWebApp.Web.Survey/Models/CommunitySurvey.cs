using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public class CommunitySurvey
    {
        [Key]
        [Required]
        [StringLength(256)]
        public string SurveyCycleId { get; set; }
        [Key]
        public int SurveyId { get; set; }
        public int CommunityId { get; set; }
        public int SurveyTypeId { get; set; }
        public DateTime EnterDate { get; set; }
        public DateTime? Form2567ReceiveDate { get; set; }
        public DateTime? PotentialDOPDate { get; set; }
        public DateTime ExitDate { get; set; }
        [RequiredIf("RequiresCertainDate")]
        public DateTime? CertainDate { get; set; }
        public DateTime? PotentialTermDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public bool ClosedFlg { get; set; }
        public bool SQCFlg { get; set; }
        public bool ImmediateJeopardyFlg { get; set; }
        public bool DidNotClearFollowUpFlg { get; set; }
        public bool UnsubstantiatedComplaintFlg { get; set; }
        public bool SelfReportFlg { get; set; }
        public bool FederalMonitoringFlg { get; set; }
        public bool DOPFlg { get; set; }
        public DateTime? DOPStartDate { get; set; }
        public DateTime? DOPEndDate { get; set; }
        public decimal? DOPDailyAmount { get; set; }
        public bool StateFineFlg { get; set; }
        public decimal? StateFineAmount { get; set; }

        [Required]
        [StringLength(3)]
        public string AtriumPayerGroupCode { get; set; }
        public AtriumPayerGroup AtriumPayerGroup { get; set; }

        public ICollection<SurveyDocument> SurveyDocuments { get; set; }

        public bool RequiresCertainDate
        {
            get { return ClosedFlg && !UnsubstantiatedComplaintFlg; }
        }
    }
}