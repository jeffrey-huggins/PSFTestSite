using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.PayrollTransfer.Models.ViewModel
{
    public class ContractorAssociationViewModel
    {
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public bool IsAssociated { get; set; }
        public bool CanDisassociate { get; set; }
    }
}
