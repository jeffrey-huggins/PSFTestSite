using System;
using System.Collections.Generic;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.RiskManagement.Models.ViewModel
{
    public class RMUnemploymentViewModel
    {
        public Employee Employee { get; set; }
        public List<Community> Communities { get; set; }

        public UnemploymentClaim Event { get; set; }
        public List<UnemploymentClaim> Claims { get; set; }
        public List<UnemploymentClaimNotes> Notes { get; set; }
        public List<UnemploymentBenefit> PayoutSummaries { get; set; }
        public List<UnemploymentClaimReason> ClaimReasons { get; set; }
        public UnemploymentClaimPaymentPeriod PayPeriod { get; set; }
        public DateTime? NextPaySuggestion { get; set; }
    }
}