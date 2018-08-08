using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class PatientIFCEventViewModel
    {
        public PatientIFCEvent Event { get; set; }
        public List<AntibioticSelection> Antibiotics { get; set; }
        public List<OrganismSelection> Organisms { get; set; }
        public List<SymptomSelection> Symptoms { get; set; }
        public List<DiagnosisSelection> Diagnosis { get; set; }
        public List<PrecautionSelection> Precautions { get; set; }
        public List<RecultureDatesViewModel> RecultureDates { get; set; }
        public PatientIFCLimits Limits { get; set; }

        public string GetLimitDisplay(string type)
        {
            switch (type)
            {
                case ("Organism"):
                    return GetLimits(Limits.OrganismRequiredFlg, Limits.OrganismMax, Organisms.Count);
                case ("Symptom"):
                    return GetLimits(Limits.SymptomRequiredFlg, Limits.SymptomMax, Symptoms.Count);
                case ("Diagnosis"):
                    return GetLimits(Limits.DiagnosisRequiredFlg, Limits.DiagnosisMax, Diagnosis.Count);
                case ("Precaution"):
                    return GetLimits(Limits.TypeOfPrecautionRequiredFlg, Limits.TypeOfPrecautionMax, Precautions.Count);
                case ("Antibiotic"):
                    return GetLimits(Limits.AntibioticRequiredFlg, Limits.AntibioticMax, Antibiotics.Count);
            }
            return string.Empty;
        }
        private string GetLimits(bool required, int max, int count)
        {
            string displayString = "( " + (required ? "1" : "0");

            if (required && max == 1)
            {
                
            }
            else if (max > count)
            {
                displayString += " - Unlimited";
            }
            else
            {
                displayString += " - " + max.ToString();
            }

            displayString += " )";
            return displayString;
        }
        //PatientIFCEventViewModel
    }
}
