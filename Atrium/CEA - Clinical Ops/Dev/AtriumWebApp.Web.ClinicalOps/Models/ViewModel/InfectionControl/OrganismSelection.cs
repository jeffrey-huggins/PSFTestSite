using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class OrganismSelection
    {
        public bool Selected { set; get; }
        public PatientIFCOrganism Type { get; set; }
    }
}
