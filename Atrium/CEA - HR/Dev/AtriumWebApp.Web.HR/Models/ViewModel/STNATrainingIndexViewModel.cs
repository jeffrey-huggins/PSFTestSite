using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class STNATrainingIndexViewModel
    {
        public int CurrentTab { get; set; }
        public IEnumerable<STNATrainingActionItem> TrainingActionItems { get; set; }

        //public IEnumerable<STNATrainingFacilityInteractionViewModel> TrainingFacilityInteractions { get; set; }
        public IEnumerable<STNATrainingClass> TrainingClasses { get; set; }
        public IEnumerable<STNATrainingFacilityNote> TrainingFacilityNotes { get; set; }
    }
}