using System.Collections.Generic;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public InstructionsViewModel InstructionsViewModel { get; set; }
        public IList<FederalDeficiency> FederalDeficiencies { get; set; }
        public IList<SafetyDeficiency> SafetyDeficiencies { get; set; }
        public IList<DeficiencyGroup> DeficiencyGroups { get; set; }
    }
}