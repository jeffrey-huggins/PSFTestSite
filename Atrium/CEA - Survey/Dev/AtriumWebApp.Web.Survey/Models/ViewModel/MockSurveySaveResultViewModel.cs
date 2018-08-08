using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveySaveResultViewModel : SaveResultViewModel
    {
        public bool IsClosed { get; set; }
        public string FormattedClosedDate { get; set; }
        public bool CanEditCompleted { get; set; }
    }
}