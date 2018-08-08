using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.ViewModel
{
    public class StandardsOfCareAdminViewModel
    {
        public List<Measure> Measures { get; set; }
        public AdminViewModel AdminViewModel { get; set; }

        #region Fall
        public List<SOCFallLocation> SOCFallLocation { get; set; }
        public List<SOCFallInjuryType> SOCFallInjuryType { get; set; }
        public List<SOCFallTreatment> SOCFallTreatment { get; set; }
        public List<SOCFallIntervention> SOCFallIntervention { get; set; }
        public List<SOCFallType> SOCFallType { get; set; }
        #endregion

        #region Wounds
        public List<PressureWoundStage> PressureWoundStages { get; set; }
        public List<CompositeWoundDescribe> CompositeWoundDescribe { get; set; }
        #endregion

        public List<SOCAntiPsychoticDiagnosis> SOCAintiPsychoticDiagnosis { get; set; }
        public List<SOCCatheterType> CatheterTypes { get; set; }
        public List<SOCRestraint> Restraints { get; set; }
        public List<SOCAntiPsychoticMedication> Medications { get; set; }

        #region NewItems
        public Measure NewMeasure { get { return new Measure(); } }
        public SOCAntiPsychoticDiagnosis NewAntipsychotic { get { return new SOCAntiPsychoticDiagnosis(); } }
        public SOCCatheterType NewCatheter { get { return new SOCCatheterType(); } }
        public SOCFallLocation NewFallLocation { get { return new SOCFallLocation(); } }
        public SOCFallInjuryType NewFallInjury { get { return new SOCFallInjuryType(); } }
        public SOCFallTreatment NewFallTreatment { get { return new SOCFallTreatment(); } }
        public SOCFallIntervention NewFallIntervention { get { return new SOCFallIntervention(); } }
        public SOCFallType NewFallType { get { return new SOCFallType(); } }
        public SOCRestraint NewRestraint { get { return new SOCRestraint(); } }
        public PressureWoundStage NewPressureWound { get { return new PressureWoundStage(); } }
        public CompositeWoundDescribe NewCompositeWound { get { return new CompositeWoundDescribe(); } }
        public SOCAntiPsychoticMedication NewAntipsychoticMed { get { return new SOCAntiPsychoticMedication(); } }
        #endregion




        public List<AtriumPayerGroup> PayerGroups { get; set; }
        public List<ApplicationCommunityAtriumPayerGroupInfo> CommunityPayerGroupInfo { get; set; }
    }
}
