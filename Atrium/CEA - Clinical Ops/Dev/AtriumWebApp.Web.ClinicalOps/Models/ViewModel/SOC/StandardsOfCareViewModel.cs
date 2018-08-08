using AtriumWebApp.Models.ViewModel;
using System.Collections.Generic;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class StandardsOfCareViewModel
    {
		public string OccuredTo { get; set; }
		public string OccuredFrom { get; set; }
        //public List<SOCEvent> EventsForResident { get; set; }
        //public List<SOCEventFall> EventFallsForResident { get; set; }
        //public List<Measure> MeasuresForEvent { get; set; }
        //public List<CompositeWoundDescribe> CompositeWoundDescriptions { get; set; }
        //public List<PressureWoundStage> PressureWoundStages { get; set; }

        //public SOCEvent SOCEvent { get; set; }

        //public SOCEventWound SOCEventWound { get; set; }
        //public List<SOCEventWoundNoted> SOCEventWoundNotes { get; set; }
        //public List<SOCPressureWoundDocument> SOCEventWoundDocuments { get; set; }
        //public int? WoundHighestSeverityLevel { get; set; }

        //public List<SOCFallInjuryType> FallInjuryType { get; set; }
        //public List<SOCFallTreatment> FallTreatment { get; set; }
        //public List<SOCFallIntervention> FallIntervention { get; set; }
        //public List<SOCFallType> FallType { get; set; }

        //public SOCEventFall SOCEventFall { get; set; }
        //public string SOCEventFallAMPM { get; set; }
        //public List<SOCEventFallInjuryType> SOCEventFallInjuryTypes { get; set; }
        //public List<SOCEventFallTreatment> SOCEventFallTreatments { get; set; }
        //public List<SOCEventFallIntervention> SOCEventFallInterventions { get; set; }
        //public List<SOCEventFallType> SOCEventFallTypes { get; set; }

        //public SOCEventAntiPsychotic SOCEventAntiPsychotic { get; set; }
        //public List<SOCEventAntiPsychoticNoted> SOCEventAntiPsychoticNoted { get; set; }

        //public SOCEventCatheter SOCEventCatheter { get; set; }

        //public List<SOCEventRestraintNoted> SOCEventRestraintNoted { get; set; }
        //public List<SOCRestraint> SOCRestraint { get; set; }

        //public List<SOCEventWound> EventWoundsForResident { get; set; }
        //public List<SOCEventRestraintNoted> EventRestraintsForResident { get; set; }
        //public List<SOCEventCatheter> EventCathetersForResident { get; set; }
        //public List<SOCEventAntiPsychotic> EventAntiPsychoticsForResident { get; set; }

        //public List<SOCRestraint> RestraintTypes { get; set; }
        //public List<SOCCatheterType> CatheterTypes { get; set; }
        //public List<SOCAntiPsychoticDiagnosis> AntiPsychoticDiagnoses { get; set; }
        //public List<SOCAntiPsychoticMedication> AntipsychoticTypes { get; set; }

		public SideBarViewModel SideBar { get; set; }

        public int PatientId { get; set; }
        public int RoomId { get; set; }
        public int? PatientGroupPayerId { get; set; }
    }
}