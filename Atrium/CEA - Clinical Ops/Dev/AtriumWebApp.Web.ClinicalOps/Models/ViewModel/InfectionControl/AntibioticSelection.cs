using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class AntibioticSelection
    {
        public bool Selected { set; get; }
        public PatientIFCAntibiotic Type { get; set; }
    }
}
