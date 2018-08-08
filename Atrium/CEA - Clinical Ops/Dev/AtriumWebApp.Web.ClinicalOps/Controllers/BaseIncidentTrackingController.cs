using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "ITR")]
    public abstract class BaseIncidentTrackingController : BaseController
    {
        protected const string AppCode = "ITR";

        public BaseIncidentTrackingController(IOptions<AppSettingsConfig> config, IncidentTrackingContext context) : base(config, context)
        {
        }

        protected new IncidentTrackingContext Context
        {
            get { return (IncidentTrackingContext)base.Context; }
        }

        protected void SetIncidentTypeDropDown()
        {
            if (!Session.Contains("TypeIncident") || !Session.Contains("Locations"))
            {
                var incidentList = (from inc in Context.PatientIncidentTypes
                                    where inc.DataEntryFlg
                                    orderby inc.SortOrder, inc.PatientIncidentName
                                    select inc).ToList();
                Session.SetItem("TypeIncident",incidentList);
                var locationList = (from loc in Context.IncidentLocations
                                    where loc.DataEntryFlg
                                    orderby loc.SortOrder, loc.PatientIncidentLocationName
                                    select loc).ToList();
                Session.SetItem("Locations", locationList);
            }
            var AMPMList = new List<string> { "AM", "PM" };
            ViewData["AMPM"] = new SelectList(AMPMList);
        }

        protected void SetRegionalNursesDropDown()
        {
            if (Session.Contains("ITRCurrentFacility"))
            {
                int facility;
                Session.TryGetObject("ITRCurrentFacility",out facility);
                var nurseAndAdminList = (from rnci in Context.RegionalNurses
                                         where rnci.CommunityId == facility
                                         select rnci.RegionalNurseEmployeeId).ToList().Union(
                                             from ciac in Context.CloseAllCommunitiesEmployees
                                             select ciac.EmployeeId).ToList();
                var eligibleEmployeeList = (from employee in Context.Employees
                                            join naa in nurseAndAdminList on employee.EmployeeId equals naa
                                            select employee).OrderBy(e => e.LastName).ToList();
                Session.SetItem("RegionalNurseList", eligibleEmployeeList);
            }
            else
            {
                Session.Remove("RegionalNurseList");
            }
        }
    }
}