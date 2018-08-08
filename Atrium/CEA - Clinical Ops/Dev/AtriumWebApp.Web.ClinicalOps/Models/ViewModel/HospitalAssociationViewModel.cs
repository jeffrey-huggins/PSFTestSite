using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class HospitalAssociationViewModel
    {
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public bool IsAssociated { get; set; }
    }
}
