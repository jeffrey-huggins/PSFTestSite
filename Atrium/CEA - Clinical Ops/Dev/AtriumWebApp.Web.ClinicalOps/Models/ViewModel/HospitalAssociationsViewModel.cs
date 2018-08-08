using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class HospitalAssociationsViewModel
    {
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public IEnumerable<HospitalAssociationViewModel> Associations { get; set; }
    }
}
