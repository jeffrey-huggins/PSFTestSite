using System;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.RiskManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    // TODO: Make this type abstract, and change the views to use the inheriting controller instead of this one directly.
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "RMU,RMW,RMM")]
    public class BaseRiskManagementController : BaseController
    {
        public BaseRiskManagementController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }
        #region Getters
        internal void GetCommunitiesForEmployee(string appCode)
        {
            ViewData["Communities"] = new SelectList(Context.Facilities.ToList().OrderBy(c => c.CommunityShortName), "CommunityId", "CommunityShortName");
            var firstCommunity = Context.Facilities.First();
            var employeeList = (from emp in Context.Employees
                                orderby emp.LastName ascending
                                where emp.CommunityId == firstCommunity.CommunityId
                                select emp).ToList();
            Session.SetItem(appCode + "Employees", employeeList);
            CurrentFacility[appCode] = firstCommunity.CommunityId;
        }

        public void GetAllEmployeeInformation(string appDate, int employeeId, string claimId, string appCode)
        {
            //Get patient based on submit
            var claimExists = !String.IsNullOrEmpty(claimId);
            var appDateExists = !String.IsNullOrEmpty(appDate);
            if (appDateExists)
            {
                employeeId = Int32.Parse(appDate);
            }
            var employee = (from emp in Context.Employees
                            where emp.EmployeeId == employeeId
                            select emp).Single();
            //Get community to extract info
            var com = (from community in Context.Facilities
                       where community.CommunityId == employee.CommunityId
                       select community).Single();
            string lookbackString;
            Session.TryGetObject(appCode + "LookbackDate", out lookbackString);
            var lookbackDate = DateTime.Parse(lookbackString);
            var employeeListFull = (from emp in Context.Employees
                                    orderby emp.LastName ascending
                                    where emp.CommunityId == com.CommunityId &&
                                          ((emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0)
                                          || emp.EmployeeStatus == "Active" || emp.EmployeeStatus == "Leave of Absence")
                                    select emp).ToList();
            var employeeListTerm = (from emp in Context.Employees
                                    orderby emp.LastName ascending
                                    where emp.CommunityId == com.CommunityId &&
                                          ((emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0))
                                    select emp).ToList();
            Session.SetItem(appCode + "Employees", employeeListFull);
            Session.SetItem(appCode + "EmployeesTerm", employeeListTerm);
            //Set Session Variables
            //if (appDateExists)
            //{
            //    Session[appCode + "CurrentEmployeeMulti"] = appDate;
            //    CurrentFacility[appCode] = com.CommunityId;
            //}
            //else
            //{
            //    Session[appCode + "CurrentEmployeeMulti"] = "";
            //}
            //if (claimExists)
            //{
            //    Session[appCode + "CurrentEmployeeMultiClaim"] = claimId;
            //    Session[appCode + "CurrentEmployeeClaimId"] = claimId;
            //}
            //else
            //{
            //    Session[appCode + "CurrentEmployeeMultiClaim"] = "";
            //}
            Session.SetItem(appCode + "CurrentEmployeeId", employeeId);
            Session.SetItem(appCode + "CurrentEmployeeIdString", employeeId.ToString());
            Session.SetItem(appCode + "CurrentEmployeeIdStringTerm", employeeId.ToString());
            Session.SetItem(appCode + "CurrentEmployeeName", employee.LastName + ", " + employee.FirstName);
            Session.SetItem(appCode + "CurrentEmployeeFacility", com.CommunityShortName);
            Session.SetItem(appCode + "CurrentEmployeeDateHire", employee.HireDate.ToString("d"));
            Session.SetItem(appCode + "CurrentEmployeeDateTerm", string.Format("{0:d}", employee.TerminationDate));
            Session.SetItem(appCode + "CurrentEmployeeSSN", "XXX-XX-" + employee.SocialSecurityNumber.Substring(5));
        }

        public ActionResult GetEmployeeListForCommunity(int facilityId, string appCode)
        {
            string lookbackString;
            Session.TryGetObject(appCode + "LookbackDate", out lookbackString);
            var lookbackDate = DateTime.Parse(lookbackString);
            Session.TryGetObject(appCode + "ActiveShow", out string activeShow);
            if (Session.Contains(appCode + "ActiveShow") && activeShow.Equals("true"))
            {
                var employeeListFull = (from emp in Context.Employees
                                        orderby emp.LastName ascending
                                        where emp.CommunityId == facilityId &&
                                              ((emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0)
                                               || emp.EmployeeStatus == "Active" || emp.EmployeeStatus == "Leave of Absence")
                                        select emp).ToList();
                var employees = employeeListFull.Select(a => new SelectListItem()
                {
                    Text = a.LastName + ", " + a.FirstName,
                    Value = a.EmployeeId.ToString()
                });
                //var employeeSelect = new SelectList(employeeListFull.Select(a => new SelectListItem()
                //    {
                //        Text = a.LastName + ", " + a.FirstName,
                //        Value = a.EmployeeId.ToString()
                //    }), "Value", "Text");
                Session.SetItem(appCode + "Employees", employeeListFull);
                CurrentFacility[appCode] = facilityId;
                return Json(employees);
            }
            else
            {
                var employeeListTerm = (from emp in Context.Employees
                                        orderby emp.LastName ascending
                                        where emp.CommunityId == facilityId &&
                                              (emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0)
                                        select emp).ToList();
                var employees = employeeListTerm.Select(a => new SelectListItem()
                {
                    Text = a.LastName + ", " + a.FirstName,
                    Value = a.EmployeeId.ToString()
                });
                //var employeeSelect = new SelectList(employeeListTerm.Select(a => new SelectListItem()
                //    {
                //        Text = a.LastName + ", " + a.FirstName,
                //        Value = a.EmployeeId.ToString()
                //    }), "Value", "Text");
                Session.SetItem(appCode + "EmployeesTerm", employeeListTerm);
                CurrentFacility[appCode] = facilityId;
                return Json(employees);
            }
        }
        #endregion

        #region Setters
        public void ChangeCommunityForEmployee(string appCode, int facilityId)
        {
            string lookbackString;
            Session.TryGetObject(appCode + "LookbackDate", out lookbackString);
            var lookbackDate = DateTime.Parse(lookbackString);
            var employeeListFull = (from emp in Context.Employees
                                    orderby emp.LastName ascending
                                    where emp.CommunityId == facilityId &&
                                            ((emp.TerminationDate != null &&
                                            lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0)
                                            || emp.EmployeeStatus == "Active" || emp.EmployeeStatus == "Leave of Absence")
                                    select emp).ToList();
            Session.SetItem(appCode + "Employees", employeeListFull);
            var employeeListTerm = (from emp in Context.Employees
                                    orderby emp.LastName ascending
                                    where emp.CommunityId == facilityId &&
                                            (emp.TerminationDate != null &&
                                            lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0)
                                    select emp).ToList();
            Session.SetItem(appCode + "EmployeesTerm", employeeListTerm);
            CurrentFacility[appCode] = facilityId;
            Session.Remove(appCode + "CurrentEmployeeId");
            Session.Remove(appCode + "CurrentEmployeeIdString");
            Session.Remove(appCode + "CurrentEmployeeIdStringTerm");
            Session.Remove(appCode + "CurrentEmployeeName");
            Session.Remove(appCode + "CurrentEmployeeFacility");
            Session.Remove(appCode + "CurrentEmployeeDateHire");
            Session.Remove(appCode + "CurrentEmployeeDateTerm");
            Session.Remove(appCode + "CurrentEmployeeSSN");
            switch (appCode)
            {
                case "RMU":
                    Session.Remove(appCode + "CurrentEmployeeClaimId");
                    break;
            }
        }

        public void SetStateDropDown(string appCode)
        {
            var stateList = Context.States.ToList();
            ViewData["States"] = new SelectList(stateList, "PaymentPeriodCd", "StateName");
        }


        #endregion

        #region Ajax Calls
        public JsonResult DeleteNote(int rowId, string appCode)
        {
            try
            {
                switch (appCode)
                {
                    case "RMU":
                        using (var context = new UnemploymentContext(Context.Database.Connection.ConnectionString))
                        {
                            context.UnemploymentClaimNotes.Remove(context.UnemploymentClaimNotes.Find(rowId));
                            context.SaveChanges();
                        }
                        break;
                    case "RMM":
                        using (var context = new MedicalRecordsContext(Context.Database.Connection.ConnectionString))
                        {
                            context.MedicalRecordsRequestNotes.Remove(context.MedicalRecordsRequestNotes.Find(rowId));
                            context.SaveChanges();
                        }
                        break;
                    case "RMW":
                        using (var context = new WorkersCompContext())
                        {
                            context.WorkersCompClaimNotes.Remove(context.WorkersCompClaimNotes.Find(rowId));
                            context.SaveChanges();
                        }
                        break;
                }
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion
    }
}