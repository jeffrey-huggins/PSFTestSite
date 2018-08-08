using System.Collections.Generic;
using System.ComponentModel;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class ScheduleAdminAreaRoomsViewModel
    {
        public bool IsAdministrator { get; set; }
        public AdminViewModel AdminViewModel { get; set; }
        public IEnumerable<MasterAtriumPatientGroup> AreaRooms { get; set; }
        public int CommunityId { get; set; }
        public string CommunityShortName { get; set; }
    }
}