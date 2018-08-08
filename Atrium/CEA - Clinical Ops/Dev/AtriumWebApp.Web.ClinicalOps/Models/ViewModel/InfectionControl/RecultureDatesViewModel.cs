using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class RecultureDatesViewModel
    {
        public bool DeleteDate { get; set; }
        public PatientIFCEventReCulture Reculture { get; set; }
    }
}
