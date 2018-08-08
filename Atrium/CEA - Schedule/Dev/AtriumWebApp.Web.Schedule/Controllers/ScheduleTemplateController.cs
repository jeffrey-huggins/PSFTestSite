using System;
using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Schedule.Managers;
using AtriumWebApp.Web.Schedule.Models;
using AtriumWebApp.Web.Schedule.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Data.Entity;

namespace AtriumWebApp.Web.Schedule.Controllers
{
    /// <summary>
    /// Template for populating a NEW Schedule's default Slots.
    /// </summary>
    /// 
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SCH")]
    public class ScheduleTemplateController : ScheduleBaseController
    {
        private const string AppCode = "SCH";
        private const string PayerGroupCode = "SNF";
        private const string NurseCodes = "RN,LPN,CNA";
        private ScheduleLogManager LogManager = new ScheduleLogManager();

        private IDictionary<string, bool> _AdminAccess;

        public ScheduleTemplateController(IOptions<AppSettingsConfig> config, ScheduleContext context) : base(config, context)
        {
        }

        protected new ScheduleContext Context
        {
            get { return (ScheduleContext)base.Context; }
        }

        public int? EditingId
        {
            get
            {
                Session.TryGetObject(AppCode + "EditingId", out int? id);
                return id;
            }
            private set { Session.SetItem(AppCode + "EditingId", value); }
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

        public string CurrentYear()
        {
            return DateTime.Now.Year.ToString();
        }

        public ActionResult Index()
        {
            if (!DetermineObjectAccess("0002", null, "SCH"))
            {
                string redirectUrl = "/" + Url.Content("~").Split('/')[1] + "/Unauthorized";
                return Redirect(redirectUrl);
            }
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);

            TemplatePayPeriodScheduleViewModel viewModel = new TemplatePayPeriodScheduleViewModel();
            viewModel.CurrentCommunity = null;
            viewModel.Communities = GetCommunitiesListItems();

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult DoesScheduleForCommunityExist(int communityId)
        {
            if (communityId < 1)
                return new JsonResult(new { });
            if (Context.TemplatePayPeriod.Where(a => a.CommunityId == communityId).Count() > 0)
                LogManager.LogTemplateEntry(communityId, UserPrincipal.Name, true);
            return new JsonResult(new {
                Success = true,
                ScheduleWeekExist = Convert.ToString(Context.TemplatePayPeriod.Where(a => a.CommunityId == communityId).Count()).ToLower() });

        }

        [HttpPost]
        public ActionResult DeleteMasterTemplate(int payPeriodId)
        {
            TemplateSchdlPayPeriod payPeriod = Context.TemplatePayPeriod.Find(payPeriodId);
            LogManager.LogTemplateDelete(payPeriod.CommunityId, UserPrincipal.Name);
            Context.TemplatePayPeriod.Remove(payPeriod);
            Context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult GetPayPeriodScheduleTemplate(int communityId)
        {
            Context.TemplatePayPeriod.Include(a => a.PayerGroups.Select(
                b => b.ScheduleLedger.Select(
                    c => c.Slots.Select(
                        d => d.Employee.JobClasses.Select(
                            e => e.GLAccount)))))
                .Where(a => a.CommunityId == communityId).FirstOrDefault().PayerGroups.ToList()
                .ForEach(a => a.ScheduleLedger.ToList()
                .ForEach(b => b.Slots.Where(c => (c.Employee != null ? c.Employee.EmployeeStatus != "Active" : false) || (c.Employee != null ? !CodeIsValid(c.Employee.JobClasses.ToList(), b.GeneralLedger,DateTime.Now) :false)
                ).ToList()
                .ForEach(c =>
            {
                c.EmployeeId = null;
                c.SchdlSlotAltId = Context.AltSlots.Where(d => d.SlotAltCode == "Unass").FirstOrDefault().Id;
            })));

            Context.SaveChanges();
            TemplateSchdlPayPeriod weeklySchedule = Context.TemplatePayPeriod.Where(a => a.CommunityId == communityId).FirstOrDefault();

            TemplatePayPeriodScheduleViewModel viewModel = new TemplatePayPeriodScheduleViewModel()
            {
                CurrentCommunity = communityId,
                PayPeriod = weeklySchedule,
                PayerGroup = weeklySchedule.PayerGroups.ToList<TemplateSchdlPayerGroup>(),
                Hours = new List<TotalHoursSummary>(),
                PPDSummary = new List<PPDSummary>()
            };

            //Pay Period Group
            //Employee totals groupped by account number
            //Totals groupped by account number
            foreach (TemplateSchdlPayerGroup group in viewModel.PayerGroup)
            {
                decimal dailyCensus = group.AvgDailyCensusCnt;
                decimal census = dailyCensus * 14;
                foreach (TemplateSchdlGeneralLedger ledger in group.ScheduleLedger)
                {
                    decimal budget = ledger.HourPPDCnt;
                    decimal budgetedWeeklyHours = (census * budget) / 2;

                    decimal totalPPDHours = ledger.Slots.Where(a => a.EmployeeId.HasValue ? true : a.SchdlSlotAltId.HasValue
                        ? a.SchdlSlotAlt.PPDCalcFlg : false).Sum(b => b.Days.Sum(c => c.HourCnt.HasValue ? c.HourCnt.Value : 0));

                    decimal totalEmployeeHours = ledger.Slots.Where(a => a.EmployeeId.HasValue ? true : a.SchdlSlotAltId.HasValue
                        ? a.SchdlSlotAlt.EmployeeCalcFlg : false).Sum(b => b.Days.Sum(c => c.HourCnt.HasValue ? c.HourCnt.Value : 0));

                    decimal scheduledHours = 0;

                    if (census != 0)
                    {
                        scheduledHours = totalPPDHours / census;
                    }

                    string jobType = MapCodes(ledger.GeneralLedger.AccountNbr)[0];

                    PPDSummary ppd = new PPDSummary()
                    {
                        PPD = jobType,
                        Budgeted = budget,
                        Schedule = scheduledHours,
                        BudgetedHoursPerWeek = budgetedWeeklyHours,
                        TotalPPDHours = totalPPDHours,
                        TotalEmpHours = totalEmployeeHours
                    };
                    viewModel.PPDSummary.Add(ppd);

                    Dictionary<Employee, decimal> employeeHourCount = new Dictionary<Employee, decimal>();
                    foreach (TemplateSchdlSlot slot in ledger.Slots.Where(a => a.Employee != null))
                    {
                        if (!employeeHourCount.ContainsKey(slot.Employee))
                        {
                            employeeHourCount.Add(slot.Employee, 0);
                        }
                        foreach (TemplateSchdlSlotDay day in slot.Days.Where(a => a.HourCnt != null && a.SchdlHourAltId == null))
                        {
                            employeeHourCount[slot.Employee] += day.HourCnt.Value;
                        }
                    }

                    TotalHoursSummary summary = new TotalHoursSummary()
                    {
                        Key = MapCodes(ledger.GeneralLedger.AccountNbr)[0],
                        EmployeeTotals = employeeHourCount,
                        PayerGroupCode = group.PayerGroup.AtriumPayerGroupCode
                    };
                    viewModel.Hours.Add(summary);
                }
            }
            return PartialView("TemplatePartial", viewModel);
        }

        [HttpPost]
        public JsonResult CopyExistingSchedule(int communityId, string payPeriodDate)
        {
            //Check if a schedule already exists with that id
            if (Context.TemplatePayPeriod.Where(a => a.CommunityId == communityId).Count() > 0)
            {
                return new JsonResult(new { });
            }
            LogManager.LogTemplateEntry(communityId, UserPrincipal.Name, false);
            DateTime startDate = PayPeriodScheduleViewModel.ParseFirstDateofWeek(payPeriodDate);

            SchdlPayPeriod weeklySchedule =
            (from week in Context.Week
             where week.PayPeriodBeginDate == startDate
                 && week.CommunityId == communityId
             select week).FirstOrDefault();
            TemplateSchdlPayPeriod payPeriod = new TemplateSchdlPayPeriod
            {
                CommunityId = communityId,
                PayerGroups = new List<TemplateSchdlPayerGroup>()
            };
            foreach (SchdlPayerGroup group in weeklySchedule.PayerGroups)
            {
                TemplateSchdlPayerGroup payerGroup = new TemplateSchdlPayerGroup
                {
                    TemplatePayPeriod = payPeriod,
                    AtriumPayerGroupCode = group.AtriumPayerGroupCode,
                    AvgDailyCensusCnt = group.AvgDailyCensusCnt,
                    ScheduleLedger = new List<TemplateSchdlGeneralLedger>()
                };
                payPeriod.PayerGroups.Add(payerGroup);
                foreach (SchdlGeneralLedger ledger in group.ScheduleLedger)
                {
                    TemplateSchdlGeneralLedger templateLedger = new TemplateSchdlGeneralLedger
                    {
                        TemplatePayerGroup = payerGroup,
                        GeneralLedgerId = ledger.GeneralLedgerId,
                        HourPPDCnt = ledger.HourPPDCnt,
                        Slots = new List<TemplateSchdlSlot>()

                    };
                    payerGroup.ScheduleLedger.Add(templateLedger);
                    foreach (SchdlSlot slot in ledger.Slots)
                    {
                        TemplateSchdlSlot templateSlot = new TemplateSchdlSlot
                        {
                            Ledger = templateLedger,
                            WorkShiftId = slot.WorkShiftId,
                            SlotNbr = slot.SlotNbr,
                            EmployeeId = slot.EmployeeId,
                            SchdlSlotAltId = slot.SchdlSlotAltId,
                            Days = new List<TemplateSchdlSlotDay>()
                        };
                        templateLedger.Slots.Add(templateSlot);
                        int i = 1;
                        foreach (SchdlSlotDay day in slot.Days.OrderBy(a => a.WorkDate))
                        {
                            TemplateSchdlSlotDay templateDay = new TemplateSchdlSlotDay
                            {
                                Slot = templateSlot,
                                PayPeriodDayNbr = i,
                                AtriumPatientGroupId = day.AtriumPatientGroupId,
                                ShiftStartTime = day.ShiftStartTime,
                                ShiftEndTime = day.ShiftEndTime,
                                HourCnt = day.HourCnt,
                                SchdlHourAltId = day.SchdlHourAltId
                            };
                            templateSlot.Days.Add(templateDay);
                            i++;

                        }

                    }
                }
            }
            Context.TemplatePayPeriod.Add(payPeriod);
            Context.SaveChanges();

            JsonResult json = new JsonResult(new { Success = true });
            return json;

            //Context.Refresh(RefreshMode.StoreWins, refreshableObjects);
            //Context = new ScheduleTemplateContext();

            //return GetPayPeriodScheduleTemplate(communityId);
        }

        [HttpGet, ActionName("ExistingPayPeriodDates")]
        public JsonResult PayPeriodDatesForExistingSchedules(int communityId)
        {
            if (communityId < 1)
                return new JsonResult(new { });
            var schedules = String.Join(",", FindExistingPayPeriodSchedules(
                            communityId).Where(a => a.Date < DateTime.Now.AddMonths(1)).Select(dt => dt.ToString("yyyyMMdd")).ToArray());
            JsonResult json = new JsonResult(new { Success = true, ExistingPayPeriodBeginDates = schedules });

            return json;
        }


        [HttpPost, ActionName("RemoveSlot")]
        public JsonResult RemoveSlot(string slotId)
        {
            int id;
            if (!int.TryParse(slotId, out id))
            {
                return new JsonResult(new { });
            }

            TemplateSchdlSlot slot = Context.TemplateSlot.Find(id);
            int communityID = slot.Ledger.TemplatePayerGroup.TemplatePayPeriod.CommunityId;
            Context.TemplateSlot.Remove(slot);
            Context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost, ActionName("AddSlot")]
        public ActionResult AddSlot(string shift, string generalLedgerId)
        {
            int id;
            if (!int.TryParse(generalLedgerId, out id))
            {
                return View();
            }
            int workShift;
            if (!int.TryParse(shift, out workShift))
            {
                return View();
            }
            TemplateSchdlGeneralLedger generalLedger = Context.TemplateLedger.Find(id);

            int slotNumber = 1;
            if (generalLedger.Slots.Count > 0)
            {
                slotNumber = generalLedger.Slots.OrderByDescending(a => a.SlotNbr).FirstOrDefault().SlotNbr + 1;
            }
            SystemSchdlSlotAlt alt = Context.AltSlots.FirstOrDefault(a => a.SlotAltCode == "OpenPos");
            int? altId = null;
            if (alt != null)
            {
                altId = alt.Id;
            }
            TemplateSchdlSlot slot = new TemplateSchdlSlot
            {
                TemplateSchdlGeneralLedgerId = id,
                WorkShiftId = workShift,
                SlotNbr = slotNumber,
                SchdlSlotAltId = altId
            };
            Context.TemplateSlot.Add(slot);
            Context.SaveChanges();
            //DateTime startDate = generalLedger.PayerGroup.PayPeriod.PayPeriodBeginDate;
            for (int i = 1; i <= 14; i++)
            {
                TemplateSchdlSlotDay day = new TemplateSchdlSlotDay
                {
                    TemplateSchdlSlotId = slot.Id,
                    PayPeriodDayNbr = i
                };
                Context.TemplateSlotDay.Add(day);
            }
            Context.SaveChanges();
            return EditSlot(slot.Id.ToString());
        }

        [HttpPost]
        public ActionResult CreateEmpty(int communityId)
        {
            LogManager.LogTemplateEntry(communityId, UserPrincipal.Name, false);
            TemplateSchdlPayPeriod payPeriod = new TemplateSchdlPayPeriod
            {
                CommunityId = communityId,
                PayerGroups = new List<TemplateSchdlPayerGroup>()
            };
            TemplateSchdlPayerGroup payerGroup = new TemplateSchdlPayerGroup()
            {
                TemplatePayPeriod = payPeriod,
                AtriumPayerGroupCode = PayerGroupCode,
                AvgDailyCensusCnt = 0.0m,
                ScheduleLedger = new List<TemplateSchdlGeneralLedger>()
            };
            payPeriod.PayerGroups.Add(payerGroup);
            foreach (string nurseCode in NurseCodes.Split(','))
            {
                string nurseNmbr = MapCodes(nurseCode)[0];
                TemplateSchdlGeneralLedger templateLedger = new TemplateSchdlGeneralLedger
                {
                    TemplatePayerGroup = payerGroup,
                    GeneralLedgerId = Context.GLAccounts.First(a => a.AccountNbr == nurseNmbr).GeneralLedgerId,
                    HourPPDCnt = 0,
                    Slots = new List<TemplateSchdlSlot>()
                };
                payerGroup.ScheduleLedger.Add(templateLedger);
            }
            Context.TemplatePayPeriod.Add(payPeriod);
            Context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult EditSlot(string slotId)
        {
            int id;
            if (!int.TryParse(slotId, out id))
            {
                return View();
            }

            TemplateSchdlSlot slot = Context.TemplateSlot.Find(id);

            int communityID = slot.Ledger.TemplatePayerGroup.TemplatePayPeriod.CommunityId;
            var jobTypes = MapCodes(slot.Ledger.GeneralLedger.AccountNbr, true);
            var employees = Context.EmployeeJobClasses.Where(a => (jobTypes.Contains(a.GLAccount.AccountNbr))
                    && (a.Employee.CommunityId == communityID
                    && a.StartDate <= DateTime.Now && a.StopDate >= DateTime.Now
                        && a.Employee.EmployeeStatus == "Active"))
                        .Select(a => a.Employee).ToList();
            var altDaySlots = Context.AltSlots.Where(a => a.Id > 0);
            IList<SelectListItem> dropdownList = GetScheduleDropdownValues(altDaySlots.ToList(), employees.Distinct().ToList());
            if (slot.Employee != null)
            {
                dropdownList.FirstOrDefault(a => a.Value == slot.EmployeeId.ToString()).Selected = true;
            }
            else if (slot.SchdlSlotAlt != null)
            {
                dropdownList.FirstOrDefault(a => a.Value == (slot.SchdlSlotAltId * -1).ToString()).Selected = true;
            }
            Dictionary<int, IList<SelectListItem>> startTime = new Dictionary<int, IList<SelectListItem>>();
            Dictionary<int, IList<SelectListItem>> endTime = new Dictionary<int, IList<SelectListItem>>();
            Dictionary<int, IList<SelectListItem>> roomId = new Dictionary<int, IList<SelectListItem>>();
            List<MasterAtriumPatientGroup> rooms = Context.AreaRoom.Where(a => a.CommunityId == slot.Ledger.TemplatePayerGroup.TemplatePayPeriod.CommunityId).ToList();
            var altHourSlots = Context.AltHours.Where(a => a.Id > 0);
            foreach (TemplateSchdlSlotDay day in slot.Days)
            {
                IList<SelectListItem> room = GetCommunityRooms(rooms);
                IList<SelectListItem> shiftStart = GetHourDropdownValues(altHourSlots.ToList(), true);
                IList<SelectListItem> shiftEnd = GetHourDropdownValues(altHourSlots.ToList(), false);
                if (day.ShiftStartTime.HasValue)
                {
                    string timeValue = day.ShiftStartTime.Value.ToString("HHmm").ToLower();
                    shiftStart.FirstOrDefault(a => a.Value == timeValue).Selected = true;
                }
                if (day.ShiftEndTime.HasValue)
                {
                    string timeValue = day.ShiftEndTime.Value.ToString("HHmm").ToLower();
                    shiftEnd.FirstOrDefault(a => a.Value == timeValue).Selected = true;
                }
                if (day.SchdlHourAltId.HasValue)
                {
                    shiftStart.FirstOrDefault(a => a.Value == (-1 * day.SchdlHourAltId.Value).ToString()).Selected = true;
                }
                if (day.AtriumPatientGroupId.HasValue)
                {
                    room.FirstOrDefault(a => a.Value == day.AtriumPatientGroupId.Value.ToString()).Selected = true;
                }
                startTime.Add(day.PayPeriodDayNbr, shiftStart);
                endTime.Add(day.PayPeriodDayNbr, shiftEnd);
                roomId.Add(day.PayPeriodDayNbr, room);
            }

            TemplateSchdlSlotEditViewModel editSlot = new TemplateSchdlSlotEditViewModel
            {
                Title = MapCodes(slot.Ledger.GeneralLedger.AccountNbr)[0] + " Edit Employees:",
                Slot = slot,
                EmployeeList = dropdownList,
                StartTime = startTime,
                EndTime = endTime,
                RoomId = roomId,
                HourAltMap = GetAltHourMapping()
            };
            return PartialView("EditSlot", editSlot);

        }

        [HttpPost, ActionName("SaveSlotEdit")]
        public JsonResult SaveSlotEdit(string slotId, string employeeId, string startShifts, string endShifts, string rooms, string hourCount)
        {
            int id;
            if (!int.TryParse(slotId, out id))
            {
                return new JsonResult(new { });
            }
            TemplateSchdlSlot slot = Context.TemplateSlot.Find(id);
            slot.EmployeeId = null;
            slot.SchdlSlotAltId = null;
            int communityID = slot.Ledger.TemplatePayerGroup.TemplatePayPeriod.CommunityId;
            int employee;
            if (!int.TryParse(employeeId, out employee))
            {
                return new JsonResult(new { });
            }
            if (employee > 0)
            {
                slot.EmployeeId = employee;
            }
            else
            {
                slot.SchdlSlotAltId = employee * -1;
            }
            Dictionary<int, string> startShiftValues = GetValuesFromString(startShifts);
            Dictionary<int, string> endShiftValues = GetValuesFromString(endShifts);
            Dictionary<int, string> roomValues = GetValuesFromString(rooms);
            Dictionary<int, string> hourValues = GetValuesFromString(hourCount);
            for (int i = 0; i < slot.Days.Count; i++)
            {
                TemplateSchdlSlotDay day = slot.Days[i];
                day.ShiftStartTime = null;
                day.ShiftEndTime = null;
                day.SchdlHourAltId = null;
                day.HourCnt = null;
                day.AtriumPatientGroupId = null;

                DateTime date = DateTime.Now;
                string startShift = startShiftValues[day.PayPeriodDayNbr];
                int shiftTime;
                if (int.TryParse(startShift, out shiftTime))
                {
                    if (shiftTime < 0)
                    {
                        day.SchdlHourAltId = shiftTime * -1;

                    }
                    else
                    {
                        int hour = (int)shiftTime / 100;
                        int minute = shiftTime - (hour * 100);
                        DateTime startTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                        day.ShiftStartTime = startTime;
                    }
                }
                string endShift = endShiftValues[day.PayPeriodDayNbr];
                if (int.TryParse(endShift, out shiftTime))
                {
                    int hour = (int)shiftTime / 100;
                    int minute = shiftTime - (hour * 100);
                    DateTime endTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                    day.ShiftEndTime = endTime;
                }
                else
                {
                    day.ShiftStartTime = null;
                }

                string room = roomValues[day.PayPeriodDayNbr];
                int AtriumPatientGroupId;
                if (int.TryParse(room, out AtriumPatientGroupId))
                {
                    day.AtriumPatientGroupId = AtriumPatientGroupId;
                }
                string count = hourValues[day.PayPeriodDayNbr];
                decimal shiftLength;
                if (decimal.TryParse(count, out shiftLength))
                {
                    if (shiftLength == 0.0m)
                    {
                        day.HourCnt = null;
                    }
                    else
                    {
                        day.HourCnt = shiftLength;
                    }
                }
            }
            Context.SaveChanges();
            return Json(new { success = true });
        }

        public Dictionary<int, string> GetValuesFromString(string input)
        {
            Dictionary<int, string> values = new Dictionary<int, string>();
            foreach (string value in input.TrimEnd(',').Split(','))
            {
                int date = int.Parse(value.Split(';')[0]);
                values.Add(date, value.Split(';')[1]);
            }
            return values;
        }

        [HttpPost]
        public JsonResult GetUpdatedSummary(int payPeriodId)
        {
            TemplateSchdlPayPeriod payPeriod = Context.TemplatePayPeriod.Find(payPeriodId);
            foreach (var payerGroup in payPeriod.PayerGroups)
            {

            }

            return new JsonResult(new { });
        }

        [HttpPost, ActionName("SaveTemplateSchedule")]
        public JsonResult SaveSchedule(int payerGroupId, string dailyCensus, string ppdInfo)
        {
            if (ppdInfo == null || dailyCensus == null)
            {
                return new JsonResult(new { });
            }
            decimal census;
            if (!decimal.TryParse(dailyCensus, out census))
            {
                return new JsonResult(new { });
            }
            ppdInfo = ppdInfo.TrimEnd(',');
            TemplateSchdlPayerGroup payerGroup = Context.TemplatePayerGroup.Find(payerGroupId);
            foreach (string ppdGroup in ppdInfo.Split(','))
            {
                decimal ppdCount;
                if (!decimal.TryParse(ppdGroup.Split(';')[0], out ppdCount))
                {
                    return new JsonResult(new { });
                }
                string code = MapCodes(ppdGroup.Split(';')[1])[0];
                payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == code).FirstOrDefault().HourPPDCnt = ppdCount;

            }
            payerGroup.AvgDailyCensusCnt = census;
            Context.SaveChanges();
            return Json(new { success = true });
        }

        private void LogCreateEditSchedule(int communityId)
        {
            bool ScheduleExists = (Context.TemplatePayPeriod.Count(a => a.CommunityId == communityId) != 0);
            LogManager.LogTemplateEntry(communityId, UserPrincipal.Name, ScheduleExists);
        }

        /// <summary>
        ///  Finds FindExistingPayPeriodSchedules
        ///     This works within the context of DateTime.now prev THREE months, and NEXT SIX months.
        /// </summary>
        /// <returns></returns>
        private IList<DateTime> FindExistingPayPeriodSchedules(int communityId)
        {
            List<DateTime> range = PayPeriodBeginDates(communityId);

            List<DateTime> existingPayPeriodBeginDates =
                (from week in Context.Week
                 where 1 == 1
                     && week.CommunityId == communityId
                     && week.PayPeriodBeginDate >= range.Min()
                     && week.PayPeriodBeginDate <= range.Max()
                 select week.PayPeriodBeginDate).ToList<DateTime>();
            return existingPayPeriodBeginDates;
        }

        public IActionResult GetEmployeeJobs(int communityId, DateTime? lookupDate)
        {
            ViewBag.rnList = MapCodes("RN");
            ViewBag.lpnList = MapCodes("LPN");
            ViewBag.cnaList = MapCodes("CNA");
            DateTime date = lookupDate ?? DateTime.Now;
            var employees = Context.EmployeeJobClasses.Where(a => (a.Employee.CommunityId == communityId
                && a.StartDate <= DateTime.Now && a.StopDate >= DateTime.Now
            && a.Employee.EmployeeStatus == "Active"))
            .Select(a => a.Employee).ToList();
            return PartialView("DisplayTemplates/EmployeeJobList", employees);
        }
    }

}
