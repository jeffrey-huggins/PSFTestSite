using System.Collections.Generic;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class InfectionControlAdminViewModel
    {
        public PatientIFCLimits Limits { get; set; }
        public List<PatientIFCSite> Sites { get; set; }
        public List<PatientIFCSymptom> Symptoms { get; set; }
        public List<PatientIFCDiagnosis> Diagnoses { get; set; }
        public List<PatientIFCTypeOfPrecaution> Precautions { get; set; }
        public List<PatientIFCOrganism> Organisms { get; set; }
        public List<PatientIFCAntibiotic> Antibiotics { get; set; }

        public PatientIFCSite NewSite { get { return new PatientIFCSite(); } }
        public PatientIFCSymptom NewSymptom { get { return new PatientIFCSymptom(); } }
        public PatientIFCDiagnosis NewDiagnosis { get { return new PatientIFCDiagnosis(); } }
        public PatientIFCTypeOfPrecaution NewPrecaution { get { return new PatientIFCTypeOfPrecaution(); } }
        public PatientIFCOrganism NewOrganism { get { return new PatientIFCOrganism(); } }
        public PatientIFCAntibiotic NewAntibiotic { get { return new PatientIFCAntibiotic(); } }

        public AdminViewModel AdminViewModel { get; set; }

        public List<AtriumPayerGroup> PayerGroups { get; set; }
        public List<ApplicationCommunityAtriumPayerGroupInfo> CommunityPayerGroupInfo { get; set; }
    }
}