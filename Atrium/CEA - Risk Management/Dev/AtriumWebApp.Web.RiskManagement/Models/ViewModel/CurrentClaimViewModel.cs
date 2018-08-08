using System;

namespace AtriumWebApp.Web.RiskManagement.Models.ViewModel
{
    public class CurrentClaimViewModel
    {
        public string ClaimId { get; set; }
        public int CommunityId { get; set; }
        public DateTime ReportedToCarrierDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MaskedSocialSecurityNumber { get; set; }
        public DateTime BirthDate { get; set; }
    }
}