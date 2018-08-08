using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class STNAFacilityAssociationViewModel
    {
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public bool IsAssociated { get; set; }
        public bool CanDisassociate { get; set; }
    }
}
