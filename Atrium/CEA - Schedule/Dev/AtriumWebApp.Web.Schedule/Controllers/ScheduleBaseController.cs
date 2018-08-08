
using System;
using System.Collections.Generic;

using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;

using AtriumWebApp.Web.Schedule.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Schedule.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SCH")]
    public class ScheduleBaseController : BaseController
    {
        private const string AppCode = "SCH";
        private const string PayerGroupCode = "SNF";
        private static IDictionary<int, string> _JOB_TYPES = null;
        private IDictionary<string, bool> _AdminAccess;

        public new ScheduleBaseContext Context
        {
            get { return (ScheduleBaseContext)base.Context; }
        }

        private bool IsAdministrator
        {
            get
            {
                if (_AdminAccess == null)
                {
                    _AdminAccess = DetermineAdminAccess(PrincipalContext, UserPrincipal);
                }
                bool isAdministrator;
                if (_AdminAccess.TryGetValue(AppCode, out isAdministrator))
                {
                    return isAdministrator;
                }
                return false;
            }
        }

        public IDictionary<int, string> JobTypes
        {
            get
            {
                if (_JOB_TYPES == null)
                {
                    _JOB_TYPES = JobTypeMappings();
                }
                return _JOB_TYPES;
            }
        }


        string[] NURSE_GL_CODES = { "5101.00", "5103.00", "5105.00" };

        public ScheduleBaseController(IOptions<AppSettingsConfig> config, ScheduleBaseContext context) : base(config, context)
        {
        }

        /// <summary>
        /// GeneralLedgerId	AccountNbr	AccountName	AtriumPayerGroupCode
        /// 4	5101.00	Registered Nurse	SNF
        /// 5	5102.00	MDS Coordinator	SNF (NOT USED)
        /// 6	5103.00	Licensed Practical Nurse	SNF
        /// 7	5105.00	Certified Nursing Assistant	SNF
        /// </summary>
        /// <returns></returns>
        public IDictionary<int, string> JobTypeMappings()
        {
            string glCodes = String.Join(",", NURSE_GL_CODES);
            IDictionary<int, string> theDictionary = (from jobTypes in Context.GLAccounts
                                                      where glCodes.IndexOf(jobTypes.AccountNbr) > -1
                                                      select jobTypes
                                                      ).OrderBy(t => t.GeneralLedgerId)
                                        .AsEnumerable()
                                        .ToDictionary(t => t.GeneralLedgerId,
                                                t => (String.Join("", t.AccountName.Split(' ')
                                                    .Select(word => word[0]).ToArray())));
            return theDictionary;
        }

        [HttpPost]
        public List<SelectListItem> GetCommunitiesListItems()
        {
            //List<Community> items = (from apps in Context.Applications
            //                         join appCommInfo in Context.ApplicationCommunityInfos
            //                         on apps.ApplicationId equals appCommInfo.ApplicationId
            //                         join communities in Context.Facilities
            //                         on appCommInfo.CommunityId equals communities.CommunityId
            //                         where 1 == 1
            //                            && apps.ApplicationCode == AppCode
            //                            && appCommInfo.DataEntryFlg
            //                            && communities.IsCommunityFlg
            //                         orderby communities.CommunityShortName
            //                         select communities).ToList<Community>();

            // Limit Community List by Active Directory.
            List<Community> items = FacilityList[AppCode].ToList();
            List<SelectListItem> communitiesList =
                items.Select(x => new SelectListItem() { Value = x.CommunityId.ToString(), Text = x.CommunityShortName })
                            .ToList<SelectListItem>();
            //communitiesList.Insert(0, new SelectListItem() { Value = "-1", Text = "(None: Select Community)", Selected = true });
            if (communitiesList != null && communitiesList.Count > 0)
                communitiesList[0].Selected = true;
            return communitiesList;
        }

        // Find DateTime that represents months from now that include the entire Month's First Day and 00.00.00 Start Time.
        private DateTime FindDateTimeForMonthFirstDay(int monthsFromNow)
        {
            DateTime now = DateTime.Today;
            DateTime first = now.AddMonths(monthsFromNow).AddDays(1 - now.Day);
            return first;
        }

        // Find DateTime that represents months from now that include the entire Month's range of Days to end of Month and 23.59.59 End Time.
        private DateTime FindDateTimeForMonthUptoLastDay(int monthsFromNow)
        {
            DateTime now = DateTime.Today;
            DateTime first = FindDateTimeForMonthFirstDay(monthsFromNow);
            DateTime last = first.AddDays(DateTime.DaysInMonth(now.Year, now.AddMonths(monthsFromNow).Month) - 1);
            return last.AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// Setup for ONE PREVIOUS Month and SIX FUTURE MONTHS
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public List<DateTime> PayPeriodBeginDates(int? communityId)
        {
            if(communityId == null || communityId < 1)
                return new List<DateTime>();

            DateTime prevTHREEmonths = FindDateTimeForMonthFirstDay(-1);
            DateTime nextSIXmonths = FindDateTimeForMonthUptoLastDay(6);

            return Context.PayPeriod
                .Where(pp => pp.CommunityId == communityId)
                .Where(pp => pp.PayPeriodBeginDate >= prevTHREEmonths)
                .Where(pp => pp.PayPeriodBeginDate <= nextSIXmonths)
                .Select(pp => pp.PayPeriodBeginDate).ToList();
        }

        //public SystemMonthlyLaborBudget FindMonthlyLaborBudgetForPayPeriod(DateTime payPeriodEndDate, int communityId, int jobType)
        //{
        //    if (communityId < 1)
        //        return null;

        //    return Context.MonthlyLaborBudget
        //        .Where(mlb => mlb.CommunityId == communityId)
        //        .Where(mlb => mlb.MonthBeginDate <= payPeriodEndDate)
        //        .Where(mlb => mlb.MonthEndDate >= payPeriodEndDate)
        //        .Where(mlb => mlb.GeneralLedgerId == jobType)
        //        .FirstOrDefault();
        //}

        public SystemPayPeriod FindCurrentPayPeriod(DateTime payPeriodBeginDate, int communityId)
        {
            if (communityId < 1)
                return null;
            return Context.PayPeriod
                .Where(pp => pp.CommunityId == communityId)
                .Where(pp => pp.PayPeriodBeginDate == payPeriodBeginDate)
                .FirstOrDefault();
        }

        public int FindJobTypeKey(string jobType)
        {
            return FindJobTypeKeyValuePair(jobType).Key;
        }

        public KeyValuePair<int, string> FindJobTypeKeyValuePair(string jobType)
        {
            KeyValuePair<int, string> jobTypeKey = new KeyValuePair<int, string>(0, "");
            try
            {
                jobTypeKey = JobTypeMappings().Single(kvp => jobType.Replace("tabs-", "").Equals(kvp.Value));
            }
            catch { }
            return jobTypeKey;
        }

        public KeyValuePair<int, string> FindJobTypeKeyValuePair(int jobType)
        {
            KeyValuePair<int, string> jobTypeKey = new KeyValuePair<int, string>(0, "");
            try
            {
                jobTypeKey = JobTypes.Single(kvp => jobType.Equals(kvp.Key));
            }
            catch { }
            return jobTypeKey;
        }


        public int MapWorkShiftToViewKey(int workShiftId, string jobType)
        {
            if (jobType.Equals("RN", StringComparison.OrdinalIgnoreCase) ||
                jobType.Equals("LPN", StringComparison.OrdinalIgnoreCase))
            {
                switch (workShiftId)
                {
                    case 0:
                    case 1:
                        return 1;
                    case 2:
                    case 3:
                        return 2;
                    case 4:
                    case 5:
                        return 3;
                    default:
                        return -1;
                }
            } if (jobType.Equals("CNA", StringComparison.OrdinalIgnoreCase))
                return (workShiftId + 1);
            return -1;
        }
        public static string MapEmployeeType(string accountNmbr)
        {
            switch (accountNmbr)
            {
                case "5101.00":
                    return "Registered Nurse";
                case "5103.00":
                    return "Licensed Practical Nurse";
                case "5105.00":
                    return "Certified Nursing Assistant";
                default:
                    return string.Empty;
            }
        }
        public static IList<SelectListItem> GetCommunityRooms(List<MasterAtriumPatientGroup> rooms)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            foreach (MasterAtriumPatientGroup room in rooms)
            {
                items.Add(new SelectListItem
                {
                    Text = room.AtriumPatientGroupName,
                    Value = room.Id.ToString()
                });
            }
            return items;
        }

        public static IList<SelectListItem> GetHourDropdownValues(List<SystemSchdlHourAlt> altHourSlots, bool includeAlts)
        {

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "None",
                Value = null,
                Selected = true
            });
            for (decimal n = 0.0m; n <= 12.5m; n += 0.5m)
            {
                string hourIndicator = "a";
                if (n >= 12)
                {
                    hourIndicator = "p";
                }
                items.Add(new SelectListItem
                {
                    Text = TimeHalfHourDisplay(n) + hourIndicator,
                    Value = TimeHalfHourDisplay(n)
                });
            }
            for (decimal n = 1.0m; n < 12; n += 0.5m)
                items.Add(new SelectListItem
                {
                    Text = TimeHalfHourDisplay(n) + "p",
                    Value = TimeHalfHourDisplay((n + 12))
                });
            if (includeAlts)
            {
                foreach (SystemSchdlHourAlt hourAlt in altHourSlots.OrderBy(a => a.SortOrder).ThenBy(a => a.HourAltDesc))
                {
                    items.Add(new SelectListItem
                    {
                        Text = hourAlt.HourAltDesc + " - " + hourAlt.HourAltCode,
                        Value = (-1 * hourAlt.Id).ToString()
                    });
                }
            }
            return items;
        }

        private static string TimeHalfHourDisplay(decimal n)
        {
            return n.ToString("0#.00").Replace(".5", ".3").Replace(".", "");
        }

        public static IList<SelectListItem> GetScheduleDropdownValues(List<SystemSchdlSlotAlt> altDaySlots, List<Employee> employees)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (SystemSchdlSlotAlt slotAlt in altDaySlots.OrderBy(a => a.SortOrder).ThenBy(a => a.SlotAltDesc))
            {
                SelectListItem item = new SelectListItem();
                item.Value = (slotAlt.Id * -1).ToString();
                item.Text = slotAlt.SlotAltDesc;
                list.Add(item);
            }
            foreach (Employee employee in employees.OrderBy(a => a.LastName))
            {
                SelectListItem item = new SelectListItem();
                item.Value = employee.EmployeeId.ToString();
                item.Text = employee.LastName + ", " + employee.FirstName;
                list.Add(item);
            }
            return list;
        }

        public Dictionary<int, string> GetAltHourMapping()
        {
            Dictionary<int, string> mapping = new Dictionary<int, string>();
            Context.AltHours.ToList().ForEach(a =>
            {
                mapping.Add(a.Id * -1, a.HourAltCode);
            });
            return mapping;
        }

        public static bool CodeIsValid(List<EmployeeJobClass> classes, GeneralLedgerAccount glAccount,DateTime date)
        {
            var list = MapCodes(glAccount.AccountNbr, true);
            var test = classes.Any(a => list.Contains(a.GLAccount.AccountNbr) && (a.StartDate <= date && a.StopDate >= date));
            return test;
        }

        /// <summary>
        /// This will need to be changed in the future.
        /// </summary>
        /// <param name="nurseAcronym"></param>
        /// <returns></returns>
        public static List<string> MapCodes(string nurseAcronym,bool getLedgerList = false)
        {
            List<string> rnLists = new List<string>
            {
                "5101.00",
                "5000.00",
                "5101.90",
                "5101.70"
            };
            if (rnLists.Contains(nurseAcronym))
            {
                if (getLedgerList)
                {
                    return rnLists;
                }
                return new List<string>() { "RN" };
            }
            if(nurseAcronym == "RN")
            {
                return rnLists;
            }
            List<string> lpnList = new List<string>
            {
                "5103.00",
                "5103.90",
                "5103.70"
            };
            if (lpnList.Contains(nurseAcronym))
            {
                if (getLedgerList)
                {
                    return lpnList;
                }
                return new List<string>() { "LPN" };
            }
            if(nurseAcronym == "LPN")
            {
                return lpnList;
            }
            List<string> cnaList = new List<string>
            {
                "5105.00",
                "5105.90",
                "5105.70"
            };
            if (cnaList.Contains(nurseAcronym))
            {
                if (getLedgerList)
                {
                    return cnaList;
                }
                return new List<string>() { "CNA" };
            }
            if(nurseAcronym == "CNA")
            {
                return cnaList;
            }
            return new List<string>();
        }
        public int MapViewKeyWorkShift(int workShiftId123, string jobType)
        {
            if (jobType.Equals("RN", StringComparison.OrdinalIgnoreCase))
            {
                switch (workShiftId123)
                {
                    case 1:
                        return 0;
                    case 2:
                        return 2;
                    case 3:
                        return 4;
                    default:
                        return -1;
                }
            }
            else if (jobType.Equals("LPN", StringComparison.OrdinalIgnoreCase))
            {
                switch (workShiftId123)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 3;
                    case 3:
                        return 5;
                    default:
                        return -1;
                }
            }
            else if (jobType.Equals("CNA", StringComparison.OrdinalIgnoreCase))
                return (workShiftId123 - 1);
            return -1;
        }
    }
}
