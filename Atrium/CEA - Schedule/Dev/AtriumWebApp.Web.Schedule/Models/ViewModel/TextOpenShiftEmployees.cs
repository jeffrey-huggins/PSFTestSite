using System.Collections.Generic;


namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class TextOpenShiftEmployees
    {
        public bool IsAdministrator { get; set; }
        public IEnumerable<TextOpenShiftEmployee> textOpenShiftEmployees { get; set; }
    }
}