using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCLimits
    {
        [Key]
        public int ID { get; set; }
        public bool SymptomRequiredFlg { get; set; }
        public int SymptomMax { get; set; }
        public bool DiagnosisRequiredFlg { get; set; }
        public int DiagnosisMax { get; set; }
        public bool OrganismRequiredFlg { get; set; }
        public int OrganismMax { get; set; }        
        public bool AntibioticRequiredFlg { get; set; }
        public int AntibioticMax { get; set; }
        public bool TypeOfPrecautionRequiredFlg { get; set; }
        public int TypeOfPrecautionMax { get; set; }
    }
}