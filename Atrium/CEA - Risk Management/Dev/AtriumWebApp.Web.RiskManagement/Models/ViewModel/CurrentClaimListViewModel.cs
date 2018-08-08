using System.Collections.Generic;

namespace AtriumWebApp.Web.RiskManagement.Models.ViewModel
{
    public class CurrentClaimListViewModel
    {
        public List<CurrentClaimViewModel> Claims { get; set; }

        public int CommunityId { get; set; }
    }
}