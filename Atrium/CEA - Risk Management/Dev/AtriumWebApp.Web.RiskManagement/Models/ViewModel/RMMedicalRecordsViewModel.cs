using System.Collections.Generic;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.RiskManagement.Models.ViewModel
{
    public class RMMedicalRecordsViewModel
    {
        public MedicalRecordsRequest Request { get; set; }
        public List<DocumentViewModel> Documents { get; set; }
        public DocumentViewModel NewDocument { get; set; }
        public List<MedicalRecordsRequestNotes> Notes { get; set; }
        public MedicalRecordsRequestNotes NewNote { get; set; }
    }
}