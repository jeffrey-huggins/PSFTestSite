using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class SymptomSelection
    {
        public bool Selected { set; get; }
        public PatientIFCSymptom Type { get; set; }
    }
}
