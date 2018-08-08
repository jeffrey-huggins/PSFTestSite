using System.Collections.Generic;

namespace AtriumWebApp.Web.RiskManagement.Models.ViewModel
{
    public class RMWorkerCompViewModel
    {
        public WorkersCompClaim CompClaim { get; set; }
        public List<WorkersCompDiagnosisType> DiagnosisTypes { get; set; }
        public List<WorkersCompDiagnosis> DiagnosesForClaim { get; set; }
        public List<WorkersCompExpense> Expenses { get; set; }
        public List<WorkersCompClaimNotes> Notes { get; set; }
    }
}