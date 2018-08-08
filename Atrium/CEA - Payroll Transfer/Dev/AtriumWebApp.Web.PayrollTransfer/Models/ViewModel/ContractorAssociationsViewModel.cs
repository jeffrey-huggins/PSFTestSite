using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.PayrollTransfer.Models.ViewModel
{
    public class ContractorAssociationsViewModel
    {
        public int ContractorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return LastName + ", " + FirstName; } }

        public IEnumerable<ContractorAssociationViewModel> Associations { get; set; }

    }
}