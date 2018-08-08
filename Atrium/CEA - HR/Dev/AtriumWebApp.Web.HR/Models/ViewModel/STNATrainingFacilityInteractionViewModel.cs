using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class STNATrainingFacilityInteractionViewModel
    {
        public int STNATrainingFacilityInteractionId { get; set; }
        public int STNATrainingFacilityId { get; set; }
        public int STNATrainingActionItemId { get; set; }
        public string STNATrainingActionItemDesc { get; set; }
        public DateTime InteractionDate { get; set; }
    }
}
