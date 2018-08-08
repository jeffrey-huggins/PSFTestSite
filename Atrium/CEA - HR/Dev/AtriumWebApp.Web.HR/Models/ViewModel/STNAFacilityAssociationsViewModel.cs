using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class STNAFacilityAssociationsViewModel
    {
        public int STNATrainingFacilityId { get; set; }
        public string TrainingFacilityName { get; set; }

        public IEnumerable<STNAFacilityAssociationViewModel> Associations { get; set; }
    }
}