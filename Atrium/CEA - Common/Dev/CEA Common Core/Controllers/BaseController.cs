using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics;

namespace AtriumWebApp.Web.Controllers
{

    [ResponseCache(NoStore = true,Duration =0)]
    public class BaseController : Controller
    {

        protected static string ADDomain;
        protected static string ADUser;
        protected static string ADPassword;
        protected PrincipalContext PrincipalContext;
        protected static IOptions<AppSettingsConfig> Config;
        public UserPrincipal UserPrincipal;
        protected string UserName = "defaultUser";
        protected virtual SharedContext Context { get; set; }
        private Lazy<AppCodeSessionProperty<IList<Community>>> _FacilityListSessionProperty;
        private Lazy<AppCodeSessionProperty<int>> _CurrentFacilitySessionProperty;
        private Lazy<AppCodeSessionProperty<string>> _CurrentFacilityNameSessionProperty;
        private Lazy<AppCodeSessionProperty<string>> _OccurredRangeFromSessionProperty;
        private Lazy<AppCodeSessionProperty<string>> _OccurredRangeToSessionProperty;
        protected ISession Session;
        const string PASRR = "PASRR";
        private IOptions<AppSettingsConfig> config;
        private SharedContext context;

        #region init
        public BaseController(IOptions<AppSettingsConfig> config, SharedContext context)
        {
            Config = config;
            ADDomain = config.Value.LDAP.ADDomain;
            ADUser = config.Value.LDAP.ADUser;
            ADPassword = config.Value.LDAP.ADPassword;
            MailHelper.AppEmailAddress = config.Value.SMTP.ApplicationEmailAddress;
            MailHelper.SMTPServer = config.Value.SMTP.SMTPServer;
            MailHelper.SMTPLogin = config.Value.SMTP.SMTPLogin;
            MailHelper.SMTPPassword = config.Value.SMTP.SMTPassword;
            MailHelper.SMTPPort = config.Value.SMTP.SMTPort;
            Context = context;
        }

        protected AppCodeSessionProperty<IList<Community>> FacilityList
        {
            get { return _FacilityListSessionProperty.Value; }
        }

        protected AppCodeSessionProperty<int> CurrentFacility
        {
            get { return _CurrentFacilitySessionProperty.Value; }
        }
        
        protected AppCodeSessionProperty<string> CurrentFacilityName
        {
            get { return _CurrentFacilityNameSessionProperty.Value; }
        }

        protected AppCodeSessionProperty<string> OccurredRangeFrom
        {
            get { return _OccurredRangeFromSessionProperty.Value; }
        }

        protected AppCodeSessionProperty<string> OccurredRangeTo
        {
            get { return _OccurredRangeToSessionProperty.Value; }
        }

        protected Community FindCommunity(int communityId)
        {
            return Context.Facilities.Where(f => f.CommunityId == communityId).SingleOrDefault();
        }

        protected Community FindCommunity(string communityShortName)
        {
            return Context.Facilities.Where(f => f.CommunityShortName == communityShortName).SingleOrDefault();
        }

        protected int FindCommunityId(string communityShortName)
        {
            return Context.Facilities.Where(f => f.CommunityShortName == communityShortName).Select(f => f.CommunityId).SingleOrDefault();
        }

        protected string FindCommunityShortName(int communityId)
        {
            return Context.Facilities.Where(f => f.CommunityId == communityId).Select(f => f.CommunityShortName).SingleOrDefault();
        }

        public void SetSessionVariables()
        {
            Session = HttpContext.Session;
            PrincipalContext = new PrincipalContext(ContextType.Domain, ADDomain, ADUser, ADPassword);
            string userName = HttpContext.User.Identity.Name;
            if (userName == null)
            {
                userName = User.Identity.Name;
            }
            UserPrincipal = UserPrincipal.FindByIdentity(PrincipalContext, userName);
            _FacilityListSessionProperty = new Lazy<AppCodeSessionProperty<IList<Community>>>(() => new AppCodeSessionProperty<IList<Community>>("FacilityList", HttpContext));
            _CurrentFacilitySessionProperty = new Lazy<AppCodeSessionProperty<int>>(() => new AppCodeSessionProperty<int>("CurrentFacility", HttpContext));
            _CurrentFacilityNameSessionProperty = new Lazy<AppCodeSessionProperty<string>>(() => new AppCodeSessionProperty<string>("CurrentFacilityName", HttpContext));
            _OccurredRangeFromSessionProperty = new Lazy<AppCodeSessionProperty<string>>(() => new AppCodeSessionProperty<string>("occurredRangeFrom", HttpContext));
            _OccurredRangeToSessionProperty = new Lazy<AppCodeSessionProperty<string>>(() => new AppCodeSessionProperty<string>("occurredRangeTo", HttpContext));
            
            //Byte[] userGuid;
            if (!Session.Contains("userGuid"))
            {
                Session.SetItem("userGuid", GetUserGuid());
            }
            if (!Session.Contains("IsAdmin"))
            {
                Session.SetItem("isAdmin", IsUserAdmin(PrincipalContext, UserPrincipal));
            }
            PopulateGlobalNav();
            ViewBag.CurrentUserDisplayName = UserPrincipal.DisplayName;
            ViewBag.CurrentDate = DateTime.Now.ToString("dddd,MMMM d, yyyy");
        }
        #endregion
        #region UserAccess
        public bool IsUserAdmin(PrincipalContext principalContext, UserPrincipal userPrincipal)
        {
            var adminGroups = new Dictionary<string, bool>();
            using (var appContext = new SharedContext())
            {
                if (!Session.TryGetObject("userGuid", out byte[] userGuid))
                {
                    Session.SetItem("userGuid", GetUserGuid());
                }
                Session.TryGetObject("userGuid", out userGuid);
                MasterBusinessUser busUser = appContext.MasterBusinessUsers.First(a => a.ADObjectGuid == userGuid);
                List<string> appCodeList = new List<string>();
                appContext.Applications.ToList().ForEach(a => appCodeList.Add(a.ApplicationCode));
                foreach (string app in appCodeList)
                {
                    
                    if (busUser.AdminAccess.Any(a => a.AppInfo.ApplicationCode == app))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Byte[] GetUserGuid()
        {
            if (Session.Contains("userGuid"))
            {
                Session.TryGetObject("userGuid", out byte[] userGuid);
                return userGuid;
            }
            var guid = UserPrincipal.Guid;
            var guidByteArray = guid.Value.ToByteArray();
            return guidByteArray;
        }

        public Dictionary<string, bool> DetermineAdminAccess(PrincipalContext principal, UserPrincipal user)
        {
            var adminGroups = new Dictionary<string, bool>();
            using (var appContext = new SharedContext())
            {
                byte[] userGuid = GetUserGuid();
                MasterBusinessUser busUser = appContext.MasterBusinessUsers.First(a => a.ADObjectGuid == userGuid);
                List<string> appCodeList = new List<string>();
                appContext.Applications.ToList().ForEach(a => appCodeList.Add(a.ApplicationCode));
                foreach (string app in appCodeList)
                {
                    adminGroups.Add(app, busUser.AdminAccess.Where(a => a.AppInfo.ApplicationCode == app)
                        .Any());
                }
            }
            return adminGroups;
        }

        private void PopulateGlobalNav()
        {

            if (Session.Contains("globalNav"))
            {
                return;
            }

            using (var appContext = new SharedContext())
            {
                appContext.Configuration.ProxyCreationEnabled = false;
                appContext.Configuration.AutoDetectChangesEnabled = false;
                Navigation globalNavigation = new Navigation();

                Session.TryGetObject("userGuid", out byte[] userGuid);

                MasterBusinessUser user = appContext.MasterBusinessUsers.Include("UserAccess.ApplicationInfo.MasterApplicationGroup").
                    Include("AdminAccess.AppInfo").
                    Include("SystemAdmin").First(a => a.ADObjectGuid == userGuid);
                var mbuGuidByteArray = user.ADObjectGuid;
                foreach (var access in user.UserAccess.Where(a =>
                    a.AppFlg
                    && a.ApplicationInfo.EnabledFlg))
                {
                    var info = access.ApplicationInfo;
                    info.UserAccessList = null;
                    info.UserAdminList = null;
                    if (globalNavigation.Application.Where(a => a.ApplicationCode == info.ApplicationCode).Count() == 0)
                    {

                        if (!globalNavigation.Groups.Contains(info.MasterApplicationGroup))
                        {
                            info.MasterApplicationGroup.ApplicationInfo = null;
                            globalNavigation.Groups.Add(info.MasterApplicationGroup);
                        }
                        info.MasterApplicationGroup = null;
                        globalNavigation.Application.Add(info);
                    }
                    if (!user.AdminAccess.Any(a => a.ApplicationId == info.ApplicationId))
                    {
                        info.RelativeAdminURL = "";
                    }
                }
                if (user.SystemAdmin.Any())
                {
                    OtherNavTabs adminTab = new OtherNavTabs()
                    {
                        TabName = "System",
                        SortOrder = 0
                    };
                    string content = Url.Content("~");
                    if (string.IsNullOrEmpty(content))
                    {
                        content = "/";
                    }
                    adminTab.NavItems.Add(new OtherNav()
                    {

                        DisplayName = "User Access Admin",
                        Url = "UserAdmin"
                    });
                    adminTab.NavItems.Add(new OtherNav()
                    {
                        DisplayName = "General Access Admin",
                        Url = "AccessAdmin"
                    });
                    adminTab.NavItems.Add(new OtherNav()
                    {
                        DisplayName = "Server Status",
                        Url = "Admin/Status"
                    });
                    globalNavigation.OtherTabs.Add(adminTab);
                }
                OtherNavTabs navTab = new OtherNavTabs()
                {
                    TabName = "External"
                };
                if(HttpContext.Request.Scheme == "https")
                {
                    navTab.NavItems.Add(new OtherNav()
                    {
                        DisplayName = "SharePoint",
                        Url = "https://pete.atriumlivingcenters.com"
                    });
                }
                else
                {
                    navTab.NavItems.Add(new OtherNav()
                    {
                        DisplayName = "SharePoint",
                        Url = "http://wolverine"
                    });
                }

                globalNavigation.OtherTabs.Add(navTab);
                globalNavigation.Groups = globalNavigation.Groups.OrderBy(a => a.SortOrder).ThenBy(a => a.ApplicationGroupName).ToList();
                globalNavigation.Application = globalNavigation.Application.OrderBy(a => a.SortOrder).ThenBy(a => a.ApplicationName).ToList();
                Session.SetItem("globalNav", globalNavigation);
                appContext.Configuration.ProxyCreationEnabled = true;
            }
        }

        public bool DetermineObjectAccess(string objectCode, int? communityId, string appCode)
        {
            string accessCode = appCode + objectCode + (communityId.HasValue ? communityId.Value.ToString() : "");

            bool hasAccess = false;
            if (Session.Contains(accessCode))
            {
                Session.TryGetObject(accessCode, out hasAccess);
                return hasAccess;
            }
            using (var appContext = new SharedContext())
            {
                Session.TryGetObject("userGuid", out byte[] userGuid);
                MasterBusinessUser user = appContext.MasterBusinessUsers.First(a => a.ADObjectGuid == userGuid);
                var access = appContext.ObjectUserPermission.Where(a =>
                a.BusinessUserId == user.BusinessUserId
                && (communityId.HasValue ? a.CommunityId == communityId.Value : true));
                hasAccess = access.Any(a => a.EnabledFlg
                    && a.ObjectPermissions.Application.ApplicationCode == appCode
                    && a.ObjectPermissions.ObjectCode == objectCode);
                Session.SetItem(accessCode, hasAccess);
            }
            return hasAccess;
        }

        public Dictionary<string, bool> DetermineUserAccess(PrincipalContext principal, UserPrincipal userPrincipal, List<string> AppCodes)
        {
            Session.TryGetObject("userGuid", out byte[] userGuid);
            MasterBusinessUser user = Context.MasterBusinessUsers.First(a => a.ADObjectGuid == userGuid);
            var appGroups = new Dictionary<string, bool>();
            foreach (string appCode in AppCodes)
            {
                var userAccess = user.UserAccess.Where(a =>
                    a.ApplicationInfo.ApplicationCode == appCode
                    && a.AppFlg);
                if (!userAccess.Any())
                {
                    appGroups.Add(appCode, false);
                    continue;
                }
                else
                {
                    appGroups.Add(appCode, true);
                }
            }
            return appGroups;
        }

        public ActionResult DetermineWebpageAccess(string AppCode)
        {
            //This used to determin admin access for admin pages.  New security makes this redundant.
            return null;
        }
        #endregion

        #region Getters
        public void GetCommunitiesDropDownWithFilter(string appCode)
        {
            if (FacilityList[appCode] != null)
            {
                return;
            }
            var filteredFacilities = FilterCommunitiesByADGroups(appCode);
            
            FacilityList[appCode] = filteredFacilities;
            if (appCode != "HOD" && appCode != "CSU" && appCode != "MSU" && !Session.Contains(appCode + "ResidentList"))
            {
                var firstCommunity = filteredFacilities.First();
                Session.TryGetObject(appCode + "LookbackDate", out string lookbackString);
                var lookbackDate = DateTime.Parse(lookbackString);
                var patientList = new List<Patient>();
                if (!appCode.Equals(PASRR, StringComparison.OrdinalIgnoreCase))
                {
                    patientList = Context.Residents.Where(a => a.CommunityId == firstCommunity.CommunityId
                    && a.LastCensusDate >= lookbackDate).ToList();
                        //&& a.LastCensusDate.CompareTo(lookbackDate) >= 0).OrderBy(a => a.LastName).ToList();
                    patientList = (from patient in Context.Residents
                                   orderby patient.LastName ascending
                                   where patient.CommunityId == firstCommunity.CommunityId
                                         && patient.LastCensusDate.CompareTo(lookbackDate) >= 0
                                   select patient).ToList();
                }
                else
                {
                    patientList = (from patient in Context.Residents
                                   join commPayers in Context.CommunityPayers on patient.CurrentPayerId equals commPayers.PayerId
                                   orderby patient.LastName ascending
                                   where patient.CommunityId == firstCommunity.CommunityId
                                         && patient.LastCensusDate.CompareTo(lookbackDate) >= 0
                                         && !commPayers.AtriumPayerTypeCode.Contains("RST")
                                         && !commPayers.AtriumPayerTypeCode.Contains("IL")
                                   select patient).ToList();
                }

                Session.SetItem(appCode + "ResidentsList", patientList);
                CurrentFacility[appCode] = firstCommunity.CommunityId;
                ViewData.Add(appCode + "CurrentFacility", firstCommunity.CommunityId);

            }
            else
            {
                CurrentFacility[appCode] = filteredFacilities.First().CommunityId;
                ViewData.Add(appCode + "CurrentFacility", filteredFacilities.First().CommunityId);
            }
            ViewData.Add(appCode + "CurrentFacilityName", filteredFacilities.First().CommunityShortName);
            CurrentFacilityName[appCode] = filteredFacilities.First().CommunityShortName;
        }

        public List<Community> FilterCommunitiesByADGroups(string appCode)
        {
            var userGuid = GetUserGuid();
            var user = Context.MasterBusinessUsers.First(a => a.ADObjectGuid == userGuid);
            //user has access to these communities associated with this app
            var communities = user.UserAccess.Where(a =>
                a.ApplicationInfo.ApplicationCode == appCode
                && a.AppFlg
                && a.ApplicationInfo.CommunityInfo.Any(b =>
                    b.ApplicationId == a.ApplicationId
                    && b.CommunityId == a.CommunityId
                    && b.DataEntryFlg)).Select(a => a.Community).ToList();

            return communities.OrderBy(a => a.CommunityShortName).ToList();
        }

        public void GetCommunitiesForEmployeeDropDownWithFilter(string appCode)
        {
            if (FacilityList[appCode] != null)
            {
                return;
            }
            var filteredFacilities = FilterCommunitiesByADGroups(appCode);
            FacilityList[appCode] = filteredFacilities;
            if (!Session.Contains(appCode + "Employees") && !appCode.Equals("RMU"))
            {
                var firstCommunity = filteredFacilities.First();
                var employeeList = (from emp in Context.Employees
                                    orderby emp.LastName ascending, emp.TerminationDate descending
                                    where emp.CommunityId == firstCommunity.CommunityId
                                    select emp).ToList();
                //var employees = new SelectList(employeeList.Select(a => new SelectListItem
                //{
                //    Text = a.LastName + ", " + a.FirstName,
                //    Value = SqlFunctions.StringConvert((decimal)a.EmployeeId)
                //}), "Value", "Text");
                Session.SetItem(appCode + "Employees", employeeList);
                CurrentFacility[appCode] = firstCommunity.CommunityId;
                ViewData.Add(appCode + "CurrentFacility", firstCommunity.CommunityId);
            }
            else if (!Session.Contains(appCode + "Employees") && appCode.Equals("RMU"))
            {
                var firstCommunity = filteredFacilities.First();
                Session.TryGetObject(appCode + "LookbackDate", out string lookbackString);
                var lookbackDate = DateTime.Parse(lookbackString);
                var employeeListFull = (from emp in Context.Employees
                                        orderby emp.LastName ascending, emp.TerminationDate descending
                                        where emp.CommunityId == firstCommunity.CommunityId &&
                                              ((emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0)
                                               || emp.EmployeeStatus == "Active" || emp.EmployeeStatus == "Leave of Absence")
                                        select emp).ToList();
                var employeeListTerm = (from emp in Context.Employees
                                        orderby emp.LastName ascending, emp.TerminationDate descending
                                        where emp.CommunityId == firstCommunity.CommunityId &&
                                              ((emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0))
                                        select emp).ToList();
                Session.SetItem(appCode + "Employees", employeeListFull);
                Session.SetItem(appCode + "EmployeesTerm", employeeListTerm);
                CurrentFacility[appCode] = firstCommunity.CommunityId;
                ViewData.Add(appCode + "CurrentFacility", firstCommunity.CommunityId);
            }
        }

        public ActionResult GetResidentListForCommunity(int facilityID, string appCode)
        {
            Session.TryGetObject(appCode + "LookbackDate", out string lookbackString);
            var lookbackDate = DateTime.Parse(lookbackString);
            var residentList = new List<Patient>();
            if (!appCode.Equals(PASRR, StringComparison.OrdinalIgnoreCase))
            {
                residentList = (from resident in Context.Residents
                                orderby resident.LastName ascending
                                where resident.CommunityId == facilityID
                                        && resident.LastCensusDate.CompareTo(lookbackDate) >= 0
                                select resident).ToList();
            }
            else
            {
                residentList = (from resident in Context.Residents
                                join commPayers in Context.CommunityPayers on resident.CurrentPayerId equals commPayers.PayerId
                                orderby resident.LastName ascending
                                where resident.CommunityId == facilityID
                                    && resident.LastCensusDate.CompareTo(lookbackDate) >= 0
                                    && !commPayers.AtriumPayerTypeCode.Contains("RST")
                                    && !commPayers.AtriumPayerTypeCode.Contains("IL")
                                select resident).ToList();
            }
            if (!residentList.Any())
            {
                return null;
            }
            var residents = new SelectList(residentList.Select(a => new SelectListItem()
            {
                Text = a.LastName + ", " + a.FirstName,
                Value = a.PatientId.ToString()
            }), "Value", "Text");
            Session.SetItem(appCode + "ResidentsList", residentList);
            CurrentFacility[appCode] = facilityID;
            ViewData.Add(appCode + "CurrentFacility", facilityID);
            return Json(residents);
        }

        public void GetAllResidentInformation(int residentId, string appCode)
        {
            //Get patient based on submit
            var pat = Context.Residents.Single(p => p.PatientId == residentId);
            //Get community to extract info
            var com = Context.Facilities.Single(c => c.CommunityId == pat.CommunityId);
            //Room Information
            var rm = Context.MasterRoom.SingleOrDefault(r => r.RoomId == pat.RoomId);
            //Diagnosis Information; query separate, then union merge
            var patientDiagnoses = (from pd in Context.PatientDiagnoses
                                    where pd.PatientId == pat.PatientId
                                    orderby pd.DiagnosisDate descending
                                    select pd)
                                        .ToList()
                                        .Select(diagnosis =>
                                        new
                                        {
                                            Type = 9,
                                            DiagnosisDate = diagnosis.DiagnosisDate,
                                            ICDId = diagnosis.ICD9Id,
                                            InsertedDate = diagnosis.InsertedDate,
                                            IsAdmissionFlg = diagnosis.IsAdmissionFlg,
                                            LastModifiedDate = diagnosis.LastModifiedDate,
                                            PatientDiagnosisId = diagnosis.PatientDiagnosisId,
                                            PatientId = diagnosis.PatientId,
                                            SrcSystemDiagnosisId = diagnosis.SrcSystemDiagnosisId,
                                            SrcSystemName = diagnosis.SrcSystemName
                                        });

            var patientDiagnosesICD10 = (from pd in Context.PatientDiagnosesICD10
                                         where pd.PatientId == pat.PatientId
                                         orderby pd.DiagnosisDate descending
                                         select pd)
                                        .ToList()
                                        .Select(diagnosis =>
                                        new
                                        {
                                            Type = 10,
                                            DiagnosisDate = diagnosis.DiagnosisDate,
                                            ICDId = diagnosis.ICD10Id,
                                            InsertedDate = diagnosis.InsertedDate,
                                            IsAdmissionFlg = diagnosis.IsAdmissionFlg,
                                            LastModifiedDate = diagnosis.LastModifiedDate,
                                            PatientDiagnosisId = diagnosis.PatientDiagnosisICD10Id,
                                            PatientId = diagnosis.PatientId,
                                            SrcSystemDiagnosisId = diagnosis.SrcSystemDiagnosisICD10Id,
                                            SrcSystemName = diagnosis.SrcSystemName
                                        });

            // http://stackoverflow.com/questions/26134632/how-to-use-union-between-two-different-object-in-linq
            var patientDiagnosesMERGED = patientDiagnoses.Union(patientDiagnosesICD10).OrderByDescending(p => p.DiagnosisDate);

            //Get Latest Admission Diagnosis
            var admissionDiagnosis = (from pd in patientDiagnosesMERGED
                                      where pd.IsAdmissionFlg
                                      orderby pd.DiagnosisDate descending
                                      select pd).FirstOrDefault();
            if (admissionDiagnosis != null && admissionDiagnosis.ICDId > 0)
            {
                string ICDDiseaseName = null;
                if (admissionDiagnosis.Type == 10)
                {
                    if (Context.MasterICD10.Find(admissionDiagnosis.ICDId) != null)
                        ICDDiseaseName = Context.MasterICD10.Find(admissionDiagnosis.ICDId).DiseaseName ?? "";
                }
                else
                {
                    if (Context.MasterICD9.Find(admissionDiagnosis.ICDId) != null)
                        ICDDiseaseName = Context.MasterICD9.Find(admissionDiagnosis.ICDId).DiseaseName ?? "";
                }


                Session.SetItem(appCode + "CurrentResidentAdmissionDiagnosis", admissionDiagnosis == null
                                                                         ? "No Diagnosis"
                                                                         : ICDDiseaseName);
            }
            else
                Session.SetItem(appCode + "CurrentResidentAdmissionDiagnosis", "No Diagnosis");

            //Get first 3 Diagnosis based on DiagnosisDate
            if (patientDiagnosesMERGED != null && patientDiagnosesMERGED.Any())
            {
                var count = 1;
                foreach (var pd in patientDiagnosesMERGED)
                {
                    if (pd.IsAdmissionFlg)
                    {
                        continue;
                    }
                    string icd = "";
                    if (pd.Type == 9)
                        icd = (from i9 in Context.MasterICD9
                               where i9.ICD9Id == pd.ICDId
                               select i9).FirstOrDefault().DiseaseName as String;
                    if (pd.Type == 10 || String.IsNullOrWhiteSpace(icd))
                    {
                        icd = (from i10 in Context.MasterICD10
                               where i10.ICD10Id == pd.ICDId
                               select i10).FirstOrDefault().DiseaseName as String;
                        if (String.IsNullOrWhiteSpace(icd))
                            break;
                    }
                    Session.SetItem(appCode + "CurrentResidentDiagnosis" + count, icd);
                    count++;
                    if (count == 4)
                    {
                        break;
                    }
                }
            }
            //Set Session Variables
            Session.SetItem(appCode + "CurrentResidentId", residentId);
            Session.SetItem(appCode + "CurrentResidentName", pat.LastName + ", " + pat.FirstName);
            Session.SetItem(appCode + "CurrentResidentFacility", com.CommunityShortName);
            Session.SetItem(appCode + "CurrentResidentRoom", rm.RoomName);
            Session.SetItem(appCode + "CurrentResidentRoomId", rm.RoomId);
            Session.SetItem(appCode + "CurrentResidentBirthdate", string.Format("{0:MM/dd/yyyy}", pat.BirthDate));
            Session.SetItem(appCode + "CurrentResidentAdmitDate", pat.AdmitDate.ToString("MM/dd/yyyy"));
            Session.SetItem(appCode + "CurrentPatientStatus", pat.LastStatus);
            Session.SetItem(appCode + "CurrentResidentCensusDate", pat.LastCensusDate.ToString("MM/dd/yyyy"));
            Session.SetItem(appCode + "CurrentResidentPayerId", pat.CurrentPayerId);

        }

        #endregion

        #region Setters

        public void ChangeCommunityForResident(string appCode, int facilityId)
        {
            Session.TryGetObject(appCode + "LookbackDate", out string lookbackString);
            var lookbackDate = DateTime.Parse(lookbackString);

            var residentList = new List<Patient>();
            if (!appCode.Equals(PASRR, StringComparison.OrdinalIgnoreCase))
            {
                residentList = (from resident in Context.Residents
                                orderby resident.LastName ascending
                                where resident.CommunityId == facilityId
                                        && resident.LastCensusDate.CompareTo(lookbackDate) >= 0
                                select resident).ToList();
            }
            else
            {
                residentList = (from resident in Context.Residents
                                join commPayers in Context.CommunityPayers
                                on resident.CurrentPayerId equals commPayers.PayerId
                                where resident.CommunityId == facilityId
                                    && resident.LastCensusDate.CompareTo(lookbackDate) >= 0
                                    && !commPayers.AtriumPayerTypeCode.Equals("RST")
                                    && !commPayers.AtriumPayerTypeCode.Equals("IL")
                                orderby resident.LastName ascending
                                select resident).ToList();
            }

            Session.SetItem(appCode + "ResidentsList", residentList);
            CurrentFacility[appCode] = facilityId;
            ViewData.Add(appCode + "CurrentFacility",facilityId);
            //Clear Resident Information
            if (Session.Contains("CurrentResidentId"))
            {
                Session.Remove(appCode + "CurrentResidentId");
                Session.Remove(appCode + "CurrentResidentName");
                Session.Remove(appCode + "CurrentResidentFacility");
                Session.Remove(appCode + "CurrentResidentRoom");
                Session.Remove(appCode + "CurrentResidentRoomId");
                Session.Remove(appCode + "CurrentResidentBirthdate");
                Session.Remove(appCode + "CurrentResidentAdmitDate");
                Session.Remove(appCode + "CurrentPatientStatus");
                Session.Remove(appCode + "CurrentResidentCensusDate");
                Session.Remove(appCode + "CurrentResidentIdString");
                Session.Remove(appCode + "CurrentResidentAdmissionDiagnosis");
                Session.Remove(appCode + "CurrentResidentDiagnosis1");
                Session.Remove(appCode + "CurrentResidentDiagnosis2");
                Session.Remove(appCode + "CurrentResidentDiagnosis3");
            }
            //Clear Edit Information
            switch (appCode)
            {
                case "IFC":
                    Session.Remove(appCode + "CurrentInfectionId");
                    break;
                case "SOC":
                    Session.Remove(appCode + "CurrentSOCEventId");
                    break;
            }
        }

        public static void SetLookbackDaysAdmin(HttpContext context, string appCode)
        {
            if (context.Session.Contains(appCode + "LookbackDays"))
            {
                return;
            }
            using (var appContext = new SharedContext())
            {
                context.Session.SetItem(appCode + "LookbackDays", appContext.Applications.Single(x => x.ApplicationCode == appCode).LookbackDays);
            }
        }

        public static void SetLookbackDays(HttpContext context, string appCode)
        {
            if (!context.Session.Contains(appCode + "LookbackDate"))
            {
                using (var appContext = new SharedContext())
                {
                    var application = appContext.Applications.Single(app => app.ApplicationCode == appCode);
                    int lookbackDays = application.LookbackDays;
                    context.Session.SetItem(appCode + "LookbackDate", DateTime.Today.AddDays(-lookbackDays).ToString("d"));
                }
            }
        }

        public void SetDateRangeErrorValues()
        {
            if (!Session.Contains("DateRangeChanged"))
            {
                Session.SetItem("DateRangeChanged", false);
            }
            Session.TryGetObject("DateRangeChanged", out bool dateRangeChange);
            if (dateRangeChange)
            {
                Session.SetItem("DateRangeChanged", false);
            }
            else
            {
                Session.SetItem("FromDateRangeInvalid", "0");
                Session.SetItem("ToDateRangeInvalid", "0");
                Session.SetItem("FromDateRangeInFuture", "0");
                Session.SetItem("ToDateRangeInFuture", "0");
                Session.SetItem("FromAfterTo", "0");
            }
        }
        public void SetInitialTableRange(string appCode)
        {
            if (!OccurredRangeTo.TryGet(appCode, out string date) && !OccurredRangeFrom.TryGet(appCode, out date))
            {
                OccurredRangeTo[appCode] = DateTime.Today.ToString("d");
                if (appCode != "VAC")
                {
                    OccurredRangeFrom[appCode] = DateTime.Today.AddMonths(-1).ToString("d");
                }
                else
                {
                    OccurredRangeFrom[appCode] = DateTime.Today.AddYears(-5).ToString("d");
                }
            }
        }
        public void InitializeCensusDateChangedSessionVariable()
        {
            if (!Session.Contains("CensusDateChanged"))
            {
                Session.SetItem("CensusDateChanged", false);
            }
        }
        public void SetInitialTableRangeLookback(string appCode)
        {
            if (!OccurredRangeTo.TryGet(appCode, out string date) && !OccurredRangeFrom.TryGet(appCode, out date))
            {
                var app = Context.Applications.Single(a => a.ApplicationCode == appCode);
                OccurredRangeTo[appCode] = DateTime.Today.ToString("d");
                OccurredRangeFrom[appCode] = DateTime.Today.AddDays(-app.LookbackDays).ToString("d");
            }
        }

        #endregion

        #region Manipulators
        public void ManipulateCensusDate(string appCode)
        {
            if (!Session.Contains("CensusDateChanged"))
            {
                Session.SetItem("CensusDateChanged", false);
            }
            if (!Session.Contains("CensusDateChangedUpdate"))
            {
                Session.SetItem("CensusDateChangedUpdate", false);
            }
            Session.TryGetObject("CensusDateChangedUpdate", out bool censusDateChangedUpdate);
            if (censusDateChangedUpdate)
            {
                //[dbo].[MasterPatient] MP
                //JOIN dbo.CommunityPayers MCP ON MP.CurrentPayerId = MCP.PayerId
                //WHERE 1 = 1
                //  AND MCP.PayerTypeName NOT IN ('RST', 'IL')


                var currentCommunityId = CurrentFacility[appCode];
                Session.TryGetObject(appCode + "LookbackDate", out string lookbackString);
                var lookbackDate = DateTime.Parse(lookbackString);
                var patientList = new List<Patient>();
                if (!appCode.Equals(PASRR, StringComparison.OrdinalIgnoreCase))
                {
                    patientList = (from patient in Context.Residents
                                   orderby patient.LastName ascending
                                   where patient.CommunityId == currentCommunityId
                                         && patient.LastCensusDate.CompareTo(lookbackDate) >= 0
                                   select patient).ToList();

                }
                else
                {
                    patientList = (from patient in Context.Residents
                                   join commPayers in Context.CommunityPayers on patient.CurrentPayerId equals commPayers.PayerId
                                   orderby patient.LastName ascending
                                   where patient.CommunityId == currentCommunityId
                                         && patient.LastCensusDate.CompareTo(lookbackDate) >= 0
                                         && !commPayers.AtriumPayerTypeCode.Contains("RST")
                                         && !commPayers.AtriumPayerTypeCode.Contains("IL")
                                   select patient).ToList();
                }
                Session.SetItem(appCode + "ResidentsList", patientList);
                //var residents = new SelectList(patientList.Select(a => new SelectListItem()
                //{
                //    Text = a.LastName + ", " + a.FirstName,
                //    Value = SqlFunctions.StringConvert((decimal)a.PatientId)
                //}), "Value", "Text");
                //ViewData["Residents"] = residents;
                Session.SetItem("CensusDateChangedUpdate", false);
            }
            Session.TryGetObject("CensusDateChanged", out bool censusDateChanged);
            if (censusDateChanged)
            {
                Session.SetItem("CensusDateChanged", false);
            }
            else
            {
                Session.SetItem("CensusDateInvalid", "0");
                Session.SetItem("CensusDateInFuture", "0");
            }
        }
        public void ManipulateCensusDateForEmployee(string appCode)
        {
            if (!Session.Contains("CensusDateChanged"))
            {
                Session.SetItem("CensusDateChanged", false);
            }
            if (Session.Contains("CensusDateChangedUpdate"))
            {
                Session.SetItem("CensusDateChangedUpdate", false);
            }
            Session.TryGetObject("CensusDateChangedUpdate", out bool censusDateChangedUpdate);
            if (censusDateChangedUpdate)
            {
                var currentCommunityId = CurrentFacility[appCode];
                Session.TryGetObject(appCode + "LookbackDate", out string lookbackString);
                var lookbackDate = DateTime.Parse(lookbackString);
                var employeeListFull = (from emp in Context.Employees
                                        orderby emp.LastName ascending
                                        where emp.CommunityId == currentCommunityId &&
                                              ((emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0)
                                              || emp.EmployeeStatus == "Active" || emp.EmployeeStatus == "Leave of Absence")
                                        select emp).ToList();
                var employeeListTerm = (from emp in Context.Employees
                                        orderby emp.LastName ascending
                                        where emp.CommunityId == currentCommunityId &&
                                              ((emp.TerminationDate != null && lookbackDate.CompareTo((DateTime)emp.TerminationDate) <= 0))
                                        select emp).ToList();
                Session.SetItem(appCode + "Employees", employeeListFull);
                Session.SetItem(appCode + "EmployeesTerm", employeeListTerm);
                Session.SetItem("CensusDateChangedUpdate", false);
            }
            Session.TryGetObject("CensusDateChanged", out bool censusDateChanged);
            if (censusDateChanged)
            {
                Session.SetItem("CensusDateChanged", false);
            }
            else
            {
                Session.SetItem("CensusDateInvalid", "0");
                Session.SetItem("CensusDateInFuture", "0");
            }
        }

        public ActionResult SaveLookbackDate(string lookbackDate, string returnUrl, string CurrentCommunity, string appCode)
        {
            Session.SetItem("CensusDateInvalid", "0");
            Session.SetItem("CensusDateInFuture", "0");
            if (DateTime.TryParse(lookbackDate, out DateTime dummyDate))
            {
                if (dummyDate.CompareTo(DateTime.Today) > 0)
                {
                    Session.SetItem("CensusDateInFuture", "1");
                    ViewBag.CensusDateInFuture = "1";
                }
                else
                {
                    Session.SetItem(appCode + "LookbackDate", lookbackDate);
                    Session.SetItem("CensusDateChangedUpdate", true);
                    ViewData.Add(appCode + "LookbackDate", lookbackDate);
                    ViewBag.CensusDateChangedUpdate = true;
                }
            }
            else
            {
                Session.SetItem("CensusDateInvalid", "1");
                ViewBag.CensusDateInvalid = "1";
            }
            Session.SetItem("CensusDateChanged", true);
            ViewBag.CensusDateChange = true;
            CurrentFacility[appCode] = Int32.Parse(CurrentCommunity);
            ViewData.Add(appCode + "CurrentFacility", Int32.Parse(CurrentCommunity));
            return Redirect(returnUrl);
        }
        public ActionResult UpdateTableRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl, string appCode)
        {
            Session.SetItem("FromDateRangeInvalid", "0");
            Session.SetItem("ToDateRangeInvalid", "0");
            Session.SetItem("FromDateRangeInFuture", "0");
            Session.SetItem("ToDateRangeInFuture", "0");
            Session.SetItem("FromAfterTo", "0");
            var fromPass = false;
            var toPass = false;
            if (DateTime.TryParse(occurredRangeFrom, out DateTime from))
            {
                if (from.CompareTo(DateTime.Today) > 0)
                {
                    Session.SetItem("FromDateRangeInFuture", "1");
                }
                else
                {
                    fromPass = true;
                }
            }
            else
            {
                Session.SetItem("FromDateRangeInvalid", "1");
            }
            if (DateTime.TryParse(occurredRangeTo, out DateTime to))
            {
                if (to.CompareTo(DateTime.Today) > 0)
                {
                    Session.SetItem("ToDateRangeInFuture", "1");
                }
                else
                {
                    toPass = true;
                }
            }
            else
            {
                Session.SetItem("ToDateRangeInvalid", "1");
            }
            if (from.CompareTo(to) > 0)
            {
                Session.SetItem("FromAfterTo", "1");
                fromPass = false;
                toPass = false;
            }
            if (fromPass && toPass)
            {
                OccurredRangeFrom[appCode] = occurredRangeFrom;
                OccurredRangeTo[appCode] = occurredRangeTo;
            }
            Session.SetItem("DateRangeChanged", true);
            return Redirect(returnUrl);
        }

        public void LogSession(string appCode)
        {
            using (var logContext = new SharedContext())
            {
                if (!Session.Contains(appCode + "SessionStarted"))
                {
                    Session.SetItem(appCode + "SessionStarted", true);
                    var application = logContext.Applications.Single(a => a.ApplicationCode == appCode);
                    string userName = HttpContext.User.Identity.Name;
                    if (userName == null)
                    {
                        userName = User.Identity.Name;
                    }
                    var systemLog = new SystemLogUsage
                    {
                        ADDomainName = userName,
                        FirstName = UserPrincipal.GivenName,
                        LastName = UserPrincipal.Surname,
                        ApplicationId = application.ApplicationId
                    };
                    if (string.IsNullOrEmpty(systemLog.FirstName))
                    {
                        systemLog.FirstName = "NA";
                    }
                    if (string.IsNullOrEmpty(systemLog.LastName))
                    {
                        systemLog.FirstName = "NA";
                    }
                    logContext.SystemLogs.Add(systemLog);
                    logContext.SaveChanges();
                }
            }
        }


        #endregion

        #region Ajax Calls
        public JsonResult ChangeDataFlgCom(int community, bool dFlag, string appCode)
        {
            var application = Context.Applications.Single(a => a.ApplicationCode == appCode);
            var appComInfo = Context.ApplicationCommunityInfos.Single(app => app.CommunityId == community && app.ApplicationId == application.ApplicationId);
            appComInfo.DataEntryFlg = dFlag;
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangeReportFlgCom(int community, bool dFlag, string appCode)
        {
            var application = Context.Applications.Single(a => a.ApplicationCode == appCode);
            var appComInfo = Context.ApplicationCommunityInfos.Single(app => app.CommunityId == community && app.ApplicationId == application.ApplicationId);
            appComInfo.ReportFlg = dFlag;
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangePayerIncludeFlg(int community, bool dFlag, string appCode, string payer)
        {
            var appId = Context.Applications.Single(a => a.ApplicationCode == appCode).ApplicationId;
            var appComAtriumPayerGroupInfo = Context.PayerGroupInfos.Single(a => a.CommunityId == community && a.ApplicationId == appId && a.AtriumPayerGroupCode == payer);
            appComAtriumPayerGroupInfo.IncludeFlg = dFlag;
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public IActionResult UpdateEmployeeSideBar(EmployeeSidebarViewModel sideBar)
        {
            Session.SetItem("EmployeeSideBar", SideBarService.SetEmployeeSideBar(sideBar, this, Context));
            return EditorFor(sideBar);
        }

        public IActionResult EmployeeInfo(int employeeId)
        {
            return PartialView("~/Views/Shared/DisplayTemplates/EmployeeViewModel.cshtml", SideBarService.GetEmployee(employeeId, this, Context));
        }

        public IActionResult UpdateSideBar(SideBarViewModel sideBar)
        {
            Session.SetItem("SideBar", SideBarService.SetSideBar(sideBar, this, Context));
            return EditorFor(sideBar);
        }

        public IActionResult PatientInfo(int patientId)
        {
            return PartialView("~/Views/Shared/DisplayTemplates/ResidentViewModel.cshtml", SideBarService.GetPatient(patientId, this, Context));
        }

        #endregion

        public PartialViewResult EditorFor<TModel>(TModel model)
        {
            return PartialView("EditorTemplates/" + typeof(TModel).Name, model);
        }

        public PartialViewResult DisplayFor<TModel>(TModel model)
        {
            return PartialView("DisplayTemplates/" + typeof(TModel).Name, model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}