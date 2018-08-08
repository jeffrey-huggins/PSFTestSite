using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AtriumWebApp.Web.Schedule.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.Schedule.Models.ViewModel
{
    public class TemplatePayPeriodScheduleViewModel
    {
        public TemplateSchdlPayPeriod PayPeriod { get; set; }
        public List<TemplateSchdlPayerGroup> PayerGroup { get; set; }
        public List<TotalHoursSummary> Hours { get; set; }

        public IEnumerable<TemplateSchdlSlot> GetShiftSlots(string type, int shiftId, TemplateSchdlPayerGroup payerGroup)
        {
            List<TemplateSchdlSlot> shiftSlots = payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault().Slots.Where(a => a.WorkShiftId == shiftId).ToList<TemplateSchdlSlot>();
            return shiftSlots.OrderBy(a => a.SlotNbr);
        }
        public TemplateSchdlGeneralLedger GetGeneralLedger(string type, int shiftId, TemplateSchdlPayerGroup payerGroup)
        {
            return payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault();
        }

        [DisplayName("Community")]
        public int? CurrentCommunity { get; set; }

        public IList<SelectListItem> Communities { get; set; }

        public Dictionary<int, decimal> SummaryHours(string types, int? shiftId, TemplateSchdlPayerGroup payerGroup)
        {
            Dictionary<int, decimal> totals = new Dictionary<int, decimal>();
            if (shiftId != null)
            {

                foreach (string type in types.Split(','))
                {
                    List<TemplateSchdlSlot> slots = payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault().Slots.Where(a => a.WorkShiftId == shiftId).ToList();
                    if (slots.Count != 0)
                    {
                        foreach (TemplateSchdlSlotDay day in slots[0].Days)
                        {
                            if (!totals.ContainsKey(day.PayPeriodDayNbr))
                            {
                                totals[day.PayPeriodDayNbr] = 0;
                            }
                            totals[day.PayPeriodDayNbr] += slots.Where(a => a.SchdlSlotAltId.HasValue ? a.SchdlSlotAlt.SlotAltCode == "PendPos" : true).Sum(a => a.Days.Where(b => b.PayPeriodDayNbr == day.PayPeriodDayNbr && b.HourCnt.HasValue).Sum(c => c.HourCnt)).Value;
                        }
                    }
                }
            }
            else
            {
                foreach (string type in types.Split(','))
                {
                    List<TemplateSchdlSlot> slots = payerGroup.ScheduleLedger.Where(a => a.GeneralLedger.AccountNbr == ScheduleController.MapCodes(type)[0]).FirstOrDefault().Slots.ToList();
                    if (slots.Count != 0)
                    {
                        foreach (TemplateSchdlSlotDay day in slots[0].Days)
                        {
                            if (!totals.ContainsKey(day.PayPeriodDayNbr))
                            {
                                totals[day.PayPeriodDayNbr] = 0;
                            }
                            totals[day.PayPeriodDayNbr] += slots.Where(a => a.SchdlSlotAltId.HasValue ? a.SchdlSlotAlt.SlotAltCode == "PendPos" : true).Sum(a => a.Days.Where(b => b.PayPeriodDayNbr == day.PayPeriodDayNbr && b.HourCnt.HasValue).Sum(c => c.HourCnt)).Value;
                        }
                    }
                }
            }
            return totals;
        }

        public IList<PPDSummary> PPDSummary { get; set; }
        //public class TotalHoursSummary
        //{
        //    public string Key { get; set; }
        //    public string PayerGroupCode { get; set; }
        //    public Dictionary<Employee, decimal> EmployeeTotals { get; set; }
        //}
    }
}