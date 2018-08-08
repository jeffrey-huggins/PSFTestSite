using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Enumerations;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SOC,ITR,HOD,IFC,VAC,EIFC,EVAC", Admin = true)]
    public class JSONController : BaseController
    {
        public JSONController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }
        #region All Admin Pages
        public JsonResult EditRowNameOrder(int rowId, string description, int order, string appCode, IFCCode? contextId, SOCCode? socCode, ITRCode? itrCode, HODCode? hodCode)
        {
            try
            {
                switch (appCode)
                {
                    case "SOC":
                        using (var context = new SOCContext())
                        {
                            switch (socCode)
                            {
                                case SOCCode.Measure:
                                    var measureSelected = context.MasterSOCMeasure.Find(rowId);
                                    measureSelected.SOCMeasureName = description;
                                    measureSelected.SortOrder = order;
                                    break;
                                case SOCCode.Pressure:
                                    var pressureSelected = context.PressureWoundStages.Find(rowId);
                                    pressureSelected.PressureWoundStageName = description;
                                    pressureSelected.SortOrder = order;
                                    break;
                                case SOCCode.Composite:
                                    var compositeSelected = context.CompositeWoundDescribes.Find(rowId);
                                    compositeSelected.CompositeWoundDescribeName = description;
                                    compositeSelected.SortOrder = order;
                                    break;
                                case SOCCode.FallLocation:
                                    var locationSelected = context.FallLocations.Find(rowId);
                                    locationSelected.SOCFallLocationName = description;
                                    locationSelected.SortOrder = order;
                                    break;
                                case SOCCode.FallInjuryType:
                                    var injurySelected = context.FallInjuryTypes.Find(rowId);
                                    injurySelected.SOCFallInjuryTypeName = description;
                                    injurySelected.SortOrder = order;
                                    break;
                                case SOCCode.FallTreatment:
                                    var treatmentSelected = context.FallTreatmentTypes.Find(rowId);
                                    treatmentSelected.SOCFallTreatmentName = description;
                                    treatmentSelected.SortOrder = order;
                                    break;
                                case SOCCode.FallIntervention:
                                    var interventionSelected = context.FallInterventionTypes.Find(rowId);
                                    interventionSelected.SOCFallInterventionName = description;
                                    interventionSelected.SortOrder = order;
                                    break;
                                case SOCCode.FallType:
                                    var typeSelected = context.FallTypes.Find(rowId);
                                    typeSelected.SOCFallTypeName = description;
                                    typeSelected.SortOrder = order;
                                    break;
                                case SOCCode.AntiPsychotic:
                                    var antiSelected = context.AntiPsychoticDiagnoses.Find(rowId);
                                    antiSelected.AntiPsychoticDiagnosisDesc = description;
                                    antiSelected.SortOrder = order;
                                    break;
                                case SOCCode.Catheter:
                                    var catheterSelected = context.CatheterTypes.Find(rowId);
                                    catheterSelected.SOCCatheterTypeName = description;
                                    catheterSelected.SortOrder = order;
                                    break;
                                case SOCCode.Restraint:
                                    var restraintSelected = context.Restraints.Find(rowId);
                                    restraintSelected.SOCRestraintName = description;
                                    restraintSelected.SortOrder = order;
                                    break;
                                case SOCCode.AntiPsychoticMed:
                                    var medSelected = context.AntiPsychoticMedications.Find(rowId);
                                    medSelected.AntiPsychoticMedicationName = description;
                                    medSelected.SortOrder = order;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "ITR":
                        using (var context = new IncidentTrackingContext())
                        {
                            switch (itrCode)
                            {
                                case ITRCode.Incident:
                                    var incidentSelected = context.PatientIncidentTypes.Find(rowId);
                                    incidentSelected.PatientIncidentName = description;
                                    incidentSelected.SortOrder = order;
                                    break;
                                case ITRCode.Location:
                                    var locationSelected = context.IncidentLocations.Find(rowId);
                                    locationSelected.PatientIncidentLocationName = description;
                                    locationSelected.SortOrder = order;
                                    break;
                                case ITRCode.Intervention:
                                    var interventionSelected = context.IncidentInterventions.Find(rowId);
                                    interventionSelected.PatientIncidentInterventionName = description;
                                    interventionSelected.SortOrder = order;
                                    break;
                                case ITRCode.Treatment:
                                    var treatmentSelected = context.IncidentTreatments.Find(rowId);
                                    treatmentSelected.PatientIncidentTreatmentName = description;
                                    treatmentSelected.SortOrder = order;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "HOD":
                        using (var context = new HospitalDischargeContext())
                        {
                            switch (hodCode)
                            {
                                case HODCode.ERDischarge:
                                case HODCode.HospitalDischarge:
                                    var dischargeSelected = context.DischargeReasons.Find(rowId);
                                    dischargeSelected.DischargeReasonDesc = description;
                                    dischargeSelected.SortOrder = order;
                                    break;
                                case HODCode.DNRR:
                                    var dnrrSelected = context.DidNotReturnReasons.Find(rowId);
                                    dnrrSelected.DidNotReturnReasonDesc = description;
                                    dnrrSelected.SortOrder = order;
                                    break;
                                case HODCode.Hospital:
                                    var hospitalSelected = context.Hospitals.Find(rowId);
                                    hospitalSelected.Name = description;
                                    hospitalSelected.SortOrder = order;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "IFC":
                        using (var context = new InfectionControlContext())
                        {
                            switch (contextId)
                            {
                                case IFCCode.Site:
                                    var siteSelected = context.Sites.Find(rowId);
                                    siteSelected.PatientIFCSiteName = description;
                                    siteSelected.SortOrder = order;
                                    break;
                                case IFCCode.Symptom:
                                    var symptomSelected = context.Symptoms.Find(rowId);
                                    symptomSelected.PatientIFCSymptomName = description;
                                    symptomSelected.SortOrder = order;
                                    break;
                                case IFCCode.Diagnosis:
                                    var diagnosisSelected = context.Diagnoses.Find(rowId);
                                    diagnosisSelected.PatientIFCDiagnosisName = description;
                                    diagnosisSelected.SortOrder = order;
                                    break;
                                case IFCCode.Precaution:
                                    var precautionSelected = context.Precautions.Find(rowId);
                                    precautionSelected.PatientIFCTypeOfPrecautionName = description;
                                    precautionSelected.SortOrder = order;
                                    break;
                                case IFCCode.Organism:
                                    var organismSelected = context.Organisms.Find(rowId);
                                    organismSelected.PatientIFCOrganismName = description;
                                    organismSelected.SortOrder = order;
                                    break;
                                case IFCCode.Antibiotic:
                                    var antibioticSelected = context.Antibiotics.Find(rowId);
                                    antibioticSelected.PatientIFCAntibioticName = description;
                                    antibioticSelected.SortOrder = order;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                }
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangeDataFlgCom(string community, bool dFlag, string appCode)
        {
            using (var sharedContext = new SharedContext())
            {
                var appInfo = (from app in sharedContext.Applications
                               where app.ApplicationCode == appCode
                               select app).First();
                var facility = (from fac in sharedContext.Facilities
                                where fac.CommunityShortName == community
                                select fac).First();
                var appSelected = (from app in sharedContext.ApplicationCommunityInfos
                                   where app.ApplicationId == appInfo.ApplicationId && app.CommunityId == facility.CommunityId
                                   select app).Single();
                appSelected.DataEntryFlg = dFlag;
                try
                {
                    sharedContext.SaveChanges();
                }
                catch
                {
                    return Json(new SaveResultViewModel { Success = false });
                }
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangeDataFlg(string description, bool dFlag, string appCode, IFCCode? contextId, SOCCode? socCode, ITRCode? itrCode, HODCode? hodCode)
        {
            try
            {
                switch (appCode)
                {
                    case "SOC":
                        using (var context = new SOCContext())
                        {
                            switch (socCode)
                            {
                                case SOCCode.Measure:
                                    var socSelected = context.MasterSOCMeasure.Single(s => s.SOCMeasureName == description);
                                    socSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.Pressure:
                                    var pressureSelected = context.PressureWoundStages.Single(p => p.PressureWoundStageName == description);
                                    pressureSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.Composite:
                                    var compositeSelected = context.CompositeWoundDescribes.Single(c => c.CompositeWoundDescribeName == description);
                                    compositeSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.FallLocation:
                                    var locationSelected = context.FallLocations.Single(l => l.SOCFallLocationName == description);
                                    locationSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.FallInjuryType:
                                    var injurySelected = context.FallInjuryTypes.Single(i => i.SOCFallInjuryTypeName == description);
                                    injurySelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.FallTreatment:
                                    var treatmentSelected = context.FallTreatmentTypes.Single(t => t.SOCFallTreatmentName == description);
                                    treatmentSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.FallIntervention:
                                    var interventionSelected = context.FallInterventionTypes.Single(i => i.SOCFallInterventionName == description);
                                    interventionSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.FallType:
                                    var typeSelected = context.FallTypes.Single(t => t.SOCFallTypeName == description);
                                    typeSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.AntiPsychotic:
                                    var antiSelected = context.AntiPsychoticDiagnoses.Single(a => a.AntiPsychoticDiagnosisDesc == description);
                                    antiSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.Catheter:
                                    var catheterSelected = context.CatheterTypes.Single(c => c.SOCCatheterTypeName == description);
                                    catheterSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.Restraint:
                                    var restraintSelected = context.Restraints.Single(r => r.SOCRestraintName == description);
                                    restraintSelected.DataEntryFlg = dFlag;
                                    break;
                                case SOCCode.AntiPsychoticMed:
                                    var medSelected = context.AntiPsychoticMedications.Single(r => r.AntiPsychoticMedicationName == description);
                                    medSelected.DataEntryFlg = dFlag;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "ITR":
                        using (var context = new IncidentTrackingContext())
                        {
                            switch (itrCode)
                            {
                                case ITRCode.Incident:
                                    var incidentSelected = context.PatientIncidentTypes.Single(i => i.PatientIncidentName == description);
                                    incidentSelected.DataEntryFlg = dFlag;
                                    break;
                                case ITRCode.Location:
                                    var locationSelected = context.IncidentLocations.Single(l => l.PatientIncidentLocationName == description);
                                    locationSelected.DataEntryFlg = dFlag;
                                    break;
                                case ITRCode.Intervention:
                                    var interventionSelected = context.IncidentInterventions.Single(i => i.PatientIncidentInterventionName == description);
                                    interventionSelected.DataEntryFlg = dFlag;
                                    break;
                                case ITRCode.Treatment:
                                    var treatmentSelected = context.IncidentTreatments.Single(t => t.PatientIncidentTreatmentName == description);
                                    treatmentSelected.DataEntryFlg = dFlag;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "IFC":
                        using (var context = new InfectionControlContext())
                        {
                            switch (contextId)
                            {
                                case IFCCode.Diagnosis:
                                    var diagnosisSelected = context.Diagnoses.Single(d => d.PatientIFCDiagnosisName == description);
                                    diagnosisSelected.DataEntryFlg = dFlag;
                                    break;
                                case IFCCode.Site:
                                    var siteSelected = context.Sites.Single(s => s.PatientIFCSiteName == description);
                                    siteSelected.DataEntryFlg = dFlag;
                                    break;
                                case IFCCode.Symptom:
                                    var symptomSelected = context.Symptoms.Single(s => s.PatientIFCSymptomName == description);
                                    symptomSelected.DataEntryFlg = dFlag;
                                    break;
                                case IFCCode.Precaution:
                                    var precautionSelected = context.Precautions.Single(p => p.PatientIFCTypeOfPrecautionName == description);
                                    precautionSelected.DataEntryFlg = dFlag;
                                    break;
                                case IFCCode.Organism:
                                    var organismSelected = context.Organisms.Single(o => o.PatientIFCOrganismName == description);
                                    organismSelected.DataEntryFlg = dFlag;
                                    break;
                                case IFCCode.Antibiotic:
                                    var antibioticSelected = context.Antibiotics.Single(a => a.PatientIFCAntibioticName == description);
                                    antibioticSelected.DataEntryFlg = dFlag;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "HOD":
                        using (var context = new HospitalDischargeContext())
                        {
                            switch (hodCode)
                            {
                                case HODCode.ERDischarge:
                                    var erDischargeSelected = context.DischargeReasons.Single(d => d.DischargeReasonDesc == description);
                                    erDischargeSelected.ERDataEntryFlg = dFlag;
                                    break;
                                case HODCode.HospitalDischarge:
                                    var hospitalDischargeSelected = context.DischargeReasons.Single(d => d.DischargeReasonDesc == description);
                                    hospitalDischargeSelected.HospitalDataEntryFlg = dFlag;
                                    break;
                                case HODCode.DNRR:
                                    var dnrrSelected = context.DidNotReturnReasons.Single(d => d.DidNotReturnReasonDesc == description);
                                    dnrrSelected.DataEntryFlg = dFlag;
                                    break;
                                case HODCode.Hospital:
                                    var hospitalSelected = context.Hospitals.Single(d => d.Name == description);
                                    hospitalSelected.AllowReporting = dFlag;
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                }
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangeDataFlgReport(string description, bool dFlag, string appCode, IFCCode? contextId, SOCCode? socCode, HODCode? hodCode)
        {
            try
            {
                switch (appCode)
                {
                    case "SOC":
                        using (var context = new SOCContext())
                        {
                            switch (socCode)
                            {
                                case SOCCode.Measure:
                                    var socSelected = context.MasterSOCMeasure.Single(s => s.SOCMeasureName == description);
                                    socSelected.ReportFlg = dFlag;
                                    break;
                                case SOCCode.Pressure:
                                    var pressureSelected = context.PressureWoundStages.Single(p => p.PressureWoundStageName == description);
                                    pressureSelected.ReportFlg = dFlag;
                                    break;
                                case SOCCode.Composite:
                                    var compositeSelected = context.CompositeWoundDescribes.Single(c => c.CompositeWoundDescribeName == description);
                                    compositeSelected.ReportFlg = dFlag;
                                    break;
                                //case SOCCode.FallLocation:
                                //    var locationContext = new SOCFallLocationContext();
                                //    var locationSelected = (from l in locationContext.FallLocations
                                //                            where l.SOCFallLocationName == description
                                //                            select l).Single();
                                //    locationSelected.ReportFlg = dFlag;
                                //    locationContext.SaveChanges();
                                //    break;
                                //case SOCCode.FallInjuryType:
                                //    var injuryContext = new SOCFallInjuryTypeContext();
                                //    var injurySelected = (from i in injuryContext.FallInjuryTypes
                                //                          where i.SOCFallInjuryTypeName == description
                                //                          select i).Single();
                                //    injurySelected.ReportFlg = dFlag;
                                //    injuryContext.SaveChanges();
                                //    break;
                                //case SOCCode.FallTreatment:
                                //    var treatmentContext = new SOCFallTreatmentContext();
                                //    var treatmentSelected = (from t in treatmentContext.FallTreatmentTypes
                                //                             where t.SOCFallTreatmentName == description
                                //                             select t).Single();
                                //    treatmentSelected.ReportFlg = dFlag;
                                //    treatmentContext.SaveChanges();
                                //    break;
                                //case SOCCode.FallIntervention:
                                //    var interventionContext = new SOCFallInterventionContext();
                                //    var interventionSelected = (from i in interventionContext.FallInterventionTypes
                                //                                where i.SOCFallInterventionName == description
                                //                                select i).Single();
                                //    interventionSelected.ReportFlg = dFlag;
                                //    interventionContext.SaveChanges();
                                //    break;
                                //case SOCCode.FallType:
                                //    var typeContext = new SOCFallTypeContext();
                                //    var typeSelected = (from t in typeContext.FallTypes
                                //                        where t.SOCFallTypeName == description
                                //                        select t).Single();
                                //    typeSelected.ReportFlg = dFlag;
                                //    typeContext.SaveChanges();
                                //    break;
                                //case SOCCode.AntiPsychotic:
                                //    var antiContext = new SOCAntiPsychoticMedicationContext();
                                //    var antiSelected = (from a in antiContext.AntiPsychoticMedications
                                //                        where a.AntiPsychoticMedicationName == description
                                //                        select a).Single();
                                //    antiSelected.ReportFlg = dFlag;
                                //    antiContext.SaveChanges();
                                //    break;
                                //case SOCCode.Catheter:
                                //    var catheterContext = new SOCCatheterTypeContext();
                                //    var catheterSelected = (from c in catheterContext.CatheterTypes
                                //                            where c.SOCCatheterTypeName == description
                                //                            select c).Single();
                                //    catheterSelected.ReportFlg = dFlag;
                                //    catheterContext.SaveChanges();
                                //    break;
                                //case SOCCode.Restraint:
                                //    var restraintContext = new SOCRestraintContext();
                                //    var restraintSelected = (from r in restraintContext.Restraints
                                //                             where r.SOCRestraintName == description
                                //                             select r).Single();
                                //    restraintSelected.ReportFlg = dFlag;
                                //    restraintContext.SaveChanges();
                                //    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "ITR":
                        var incidentContext = new IncidentTrackingContext();
                        var incidentSelected = incidentContext.PatientIncidentTypes.Single(i => i.PatientIncidentName == description);
                        incidentSelected.ReportFlg = dFlag;
                        incidentContext.SaveChanges();
                        break;
                    case "IFC": //IFC does not have ReportFlgs
                        //switch (contextId)
                        //{
                        //    case IFCCode.Diagnosis:
                        //        var diagnosisContext = new PatientIFCDiagnosisContext();
                        //        var diagnosisSelected = (from d in diagnosisContext.Diagnoses
                        //                                 where d.PatientIFCDiagnosisName == description
                        //                                 select d).Single();
                        //        diagnosisSelected.ReportFlg = dFlag;
                        //        diagnosisContext.SaveChanges();
                        //        break;
                        //    case IFCCode.Site:
                        //        var siteContext = new PatientIFCSiteContext();
                        //        var siteSelected = (from s in siteContext.Sites
                        //                            where s.PatientIFCSiteName == description
                        //                            select s).Single();
                        //        siteSelected.ReportFlg = dFlag;
                        //        siteContext.SaveChanges();
                        //        break;
                        //    case IFCCode.Symptom:
                        //        var symptomContext = new PatientIFCSymptomContext();
                        //        var symptomSelected = (from s in symptomContext.Symptoms
                        //                               where s.PatientIFCSymptomName == description
                        //                               select s).Single();
                        //        symptomSelected.ReportFlg = dFlag;
                        //        symptomContext.SaveChanges();
                        //        break;
                        //    case IFCCode.Precaution:
                        //        var precautionContext = new PatientIFCTypeOfPrecautionContext();
                        //        var precautionSelected = (from p in precautionContext.Precautions
                        //                                  where p.PatientIFCTypeOfPrecautionName == description
                        //                                  select p).Single();
                        //        precautionSelected.ReportFlg = dFlag;
                        //        precautionContext.SaveChanges();
                        //        break;
                        //    case IFCCode.Vaccine:
                        //        var vaccineContext = new VaccineTypeContext();
                        //        var vaccineSelected = (from v in vaccineContext.VaccineTypes
                        //                               where v.VaccineTypeName == description
                        //                               select v).Single();
                        //        vaccineSelected.ReportFlg = dFlag;
                        //        vaccineContext.SaveChanges();
                        //        break;
                        //}
                        break;
                    case "HOD":
                        using (var dischargeContext = new HospitalDischargeContext())
                        {
                            if (hodCode.Value == HODCode.ERDischarge || hodCode.Value == HODCode.HospitalDischarge)
                            {
                                var dischargeSelected = dischargeContext.DischargeReasons.Single(d => d.DischargeReasonDesc == description);
                                dischargeSelected.ReportFlg = dFlag;
                            }
                            else if (hodCode.Value == HODCode.Hospital)
                            {
                                var hospitalSelected = dischargeContext.Hospitals.Single(d => d.Name == description);
                                hospitalSelected.AllowReporting = dFlag;
                            }
                            dischargeContext.SaveChanges();
                        }
                        break;
                }
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangeDataFlgWound(string description, bool dFlag, string appCode, SOCCode? socCode, SOCWoundFlag socWoundFlg)
        {
            try
            {
                if (appCode == "SOC")
                {
                    if (socCode == SOCCode.Pressure)
                    {
                        using (var pressureWoundContext = new SOCContext())
                        {
                            var pressureWoundStage = pressureWoundContext.PressureWoundStages.Single(p => p.PressureWoundStageName == description);
                            switch (socWoundFlg)
                            {
                                case SOCWoundFlag.Threshold:
                                    pressureWoundStage.IncludeInThresholdFlg = dFlag;
                                    break;
                                case SOCWoundFlag.Length:
                                    pressureWoundStage.LengthFlg = dFlag;
                                    break;
                                case SOCWoundFlag.Width:
                                    pressureWoundStage.WidthFlg = dFlag;
                                    break;
                                case SOCWoundFlag.Depth:
                                    pressureWoundStage.DepthFlg = dFlag;
                                    break;
                            }

                            pressureWoundContext.SaveChanges();
                        }
                    }
                }
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult UpdateStageSeverity(string id, string severityLevel)
        {
            try
            {
                int stageId;
                int severity;
                stageId = int.Parse(id);

                using (var pressureWoundContext = new SOCContext())
                {
                    var stage = pressureWoundContext.PressureWoundStages.FirstOrDefault(s => s.PressureWoundStageId == stageId);

                    if (!int.TryParse(severityLevel, out severity))
                    {
                        stage.SeverityLevelNbr = null;
                    }
                    else
                    {
                        stage.SeverityLevelNbr = severity;
                    }
                    pressureWoundContext.SaveChanges();

                }

            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangeThresholdFlg(string description, bool dFlag, string appCode, IFCCode? contextId, SOCCode? socCode)
        {
            try
            {
                if (appCode == "SOC")
                {
                    using (var context = new SOCContext())
                    {
                        switch (socCode)
                        {
                            case SOCCode.FallInjuryType:
                                var fallInjuryTypeSelected = context.FallInjuryTypes.Single(f => f.SOCFallInjuryTypeName == description);
                                fallInjuryTypeSelected.IncludeInThresholdFlg = dFlag;
                                break;
                            case SOCCode.FallTreatment:
                                var fallTreatmentSelected = context.FallTreatmentTypes.Single(f => f.SOCFallTreatmentName == description);
                                fallTreatmentSelected.IncludeInThresholdFlg = dFlag;
                                break;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowAdmin(int rowId, string appCode, IFCCode? contextId, SOCCode? socCode, ITRCode? itrCode, HODCode? hodCode)
        {
            try
            {
                switch (appCode)
                {
                    case "SOC":
                        using (var context = new SOCContext())
                        {
                            switch (socCode)
                            {
                                case SOCCode.Measure:
                                    context.MasterSOCMeasure.Remove(context.MasterSOCMeasure.Find(rowId));
                                    break;
                                case SOCCode.Pressure:
                                    context.PressureWoundStages.Remove(context.PressureWoundStages.Find(rowId));
                                    break;
                                case SOCCode.Composite:
                                    context.CompositeWoundDescribes.Remove(context.CompositeWoundDescribes.Find(rowId));
                                    break;
                                case SOCCode.FallLocation:
                                    context.FallLocations.Remove(context.FallLocations.Find(rowId));
                                    break;
                                case SOCCode.FallInjuryType:
                                    context.FallInjuryTypes.Remove(context.FallInjuryTypes.Find(rowId));
                                    break;
                                case SOCCode.FallTreatment:
                                    context.FallTreatmentTypes.Remove(context.FallTreatmentTypes.Find(rowId));
                                    break;
                                case SOCCode.FallIntervention:
                                    context.FallInterventionTypes.Remove(context.FallInterventionTypes.Find(rowId));
                                    break;
                                case SOCCode.FallType:
                                    context.FallTypes.Remove(context.FallTypes.Find(rowId));
                                    break;
                                case SOCCode.AntiPsychotic:
                                    context.AntiPsychoticDiagnoses.Remove(context.AntiPsychoticDiagnoses.Find(rowId));
                                    break;
                                case SOCCode.Catheter:
                                    context.CatheterTypes.Remove(context.CatheterTypes.Find(rowId));
                                    break;
                                case SOCCode.Restraint:
                                    context.Restraints.Remove(context.Restraints.Find(rowId));
                                    break;
                                case SOCCode.AntiPsychoticMed:
                                    context.AntiPsychoticMedications.Remove(context.AntiPsychoticMedications.Find(rowId));
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "ITR":
                        using (var context = new IncidentTrackingContext())
                        {
                            switch (itrCode)
                            {
                                case ITRCode.Incident:
                                    context.PatientIncidentTypes.Remove(context.PatientIncidentTypes.Find(rowId));
                                    break;
                                case ITRCode.Location:
                                    context.IncidentLocations.Remove(context.IncidentLocations.Find(rowId));
                                    break;
                                case ITRCode.Intervention:
                                    context.IncidentInterventions.Remove(context.IncidentInterventions.Find(rowId));
                                    break;
                                case ITRCode.Treatment:
                                    context.IncidentTreatments.Remove(context.IncidentTreatments.Find(rowId));
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "HOD":
                        using (var context = new HospitalDischargeContext())
                        {
                            switch (hodCode)
                            {
                                case HODCode.ERDischarge:
                                case HODCode.HospitalDischarge:
                                    context.DischargeReasons.Remove(context.DischargeReasons.Find(rowId));
                                    break;
                                case HODCode.DNRR:
                                    context.DidNotReturnReasons.Remove(context.DidNotReturnReasons.Find(rowId));
                                    break;
                                case HODCode.Hospital:
                                    context.Hospitals.Remove(context.Hospitals.Find(rowId));
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                    case "IFC":
                        using (var context = new InfectionControlContext())
                        {
                            switch (contextId)
                            {
                                case IFCCode.Site:
                                    context.Sites.Remove(context.Sites.Find(rowId));
                                    break;
                                case IFCCode.Symptom:
                                    context.Symptoms.Remove(context.Symptoms.Find(rowId));
                                    break;
                                case IFCCode.Diagnosis:
                                    context.Diagnoses.Remove(context.Diagnoses.Find(rowId));
                                    break;
                                case IFCCode.Precaution:
                                    context.Precautions.Remove(context.Precautions.Find(rowId));
                                    break;
                                case IFCCode.Organism:
                                    context.Organisms.Remove(context.Organisms.Find(rowId));
                                    break;
                                case IFCCode.Antibiotic:
                                    context.Antibiotics.Remove(context.Antibiotics.Find(rowId));
                                    break;
                            }
                            context.SaveChanges();
                        }
                        break;
                }
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion

        #region Standards of Care Admin Page
        //Removed until names have been changed
        //public JsonResult ChangeDataFlgOccurrance(string description, bool dFlag)
        //{
        //    var measureContext = new MeasureContext();
        //    var measureSelected = (from m in measureContext.MasterSOCMeasure
        //                             where m.SOCMeasureName == description
        //                             select m).Single();

        //    measureSelected.OccuranceCalcFlg = dFlag;
        //    measureSelected.PatientCalcFlg = !dFlag;
        //    try
        //    {
        //        measureContext.SaveChanges();
        //    }
        //    catch
        //    {
        //        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult ChangeDataFlgPatient(string description, bool dFlag)
        //{
        //    var measureContext = new MeasureContext();
        //    var measureSelected = (from m in measureContext.MasterSOCMeasure
        //                           where m.SOCMeasureName == description
        //                           select m).Single();

        //    measureSelected.OccuranceCalcFlg = !dFlag;
        //    measureSelected.PatientCalcFlg = dFlag;
        //    try
        //    {
        //        measureContext.SaveChanges();
        //    }
        //    catch
        //    {
        //        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Hospital Discharge Admin Page
        [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "HOD", Admin = true)]
        public JsonResult ChangeDataFlgTopN(string description, bool dFlag)
        {
            using (var dischargeContext = new HospitalDischargeContext())
            {
                var dischargeSelected = (from d in dischargeContext.DischargeReasons
                                         where d.DischargeReasonDesc == description
                                         select d).Single();
                dischargeSelected.Top_N = dFlag;
                try
                {
                    dischargeContext.SaveChanges();
                }
                catch
                {
                    return Json(new SaveResultViewModel { Success = false });
                }
            }
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion

        #region Incident Tracking Admin Page
        [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "ITR", Admin = true)]
        public JsonResult ChangeRegionalNurse(int regionalNurseId, string communityName)
        {
            using (var context = new IncidentTrackingContext())
            {
                var facility = (from fac in context.Facilities
                                where fac.CommunityShortName.Equals(communityName)
                                select fac).First();
                var rnci = (from r in context.RegionalNurses
                            where r.CommunityId == facility.CommunityId
                            select r).First();
                rnci.RegionalNurseEmployeeId = regionalNurseId;
                try
                {
                    context.SaveChanges();
                }
                catch
                {
                    return Json(new SaveResultViewModel { Success = false });
                }
            }
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion

        #region Infection Control Admin Page
        [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "IFC", Admin = true)]
        public void UpdateLimit(int max, IFCCode contextId)
        {
            using (var ifcLimitsContext = new InfectionControlContext())
            {
                var ifcLimits = ifcLimitsContext.PatientIFCLimits.First();
                switch (contextId)
                {
                    case IFCCode.Symptom:
                        ifcLimits.SymptomMax = max;
                        break;
                    case IFCCode.Diagnosis:
                        ifcLimits.DiagnosisMax = max;
                        break;
                    case IFCCode.Precaution:
                        ifcLimits.TypeOfPrecautionMax = max;
                        break;
                    case IFCCode.Antibiotic:
                        ifcLimits.AntibioticMax = max;
                        break;
                    //DB - Per Rick 2015/06/15 - Use DB values for Organisms.
                    case IFCCode.Organism:
                        ifcLimits.OrganismMax = max;
                        break;
                }
                ifcLimitsContext.SaveChanges();
            }
        }
        [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "IFC", Admin = true)]
        public void ChangeRequiredFlgIFC(bool dFlag, IFCCode contextId)
        {
            using (var ifcLimitsContext = new InfectionControlContext())
            {
                var ifcLimits = ifcLimitsContext.PatientIFCLimits.First();
                switch (contextId)
                {
                    case IFCCode.Symptom:
                        ifcLimits.SymptomRequiredFlg = dFlag;
                        break;
                    case IFCCode.Diagnosis:
                        ifcLimits.DiagnosisRequiredFlg = dFlag;
                        break;
                    case IFCCode.Precaution:
                        ifcLimits.TypeOfPrecautionRequiredFlg = dFlag;
                        break;
                    case IFCCode.Antibiotic:
                        ifcLimits.AntibioticRequiredFlg = dFlag;
                        break;
                    case IFCCode.Organism:
                        ifcLimits.OrganismRequiredFlg = dFlag;
                        break;
                }
                ifcLimitsContext.SaveChanges();
            }
        }
        #endregion
    }
}