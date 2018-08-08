using AtriumWebApp.Web.Schedule.Models;
using System;

namespace AtriumWebApp.Web.Schedule.Managers
{
    public class ScheduleLogManager
    {
        ScheduleContext Context { get; set; }

        public ScheduleLogManager(ScheduleContext context)
        {
            Context = context;
        }

        public ScheduleLogManager() : this(new ScheduleContext()) { }

        public void LogTemplateEntry(int communityId, string userName, bool ScheduleExists)
        {
            if (ScheduleExists)
            {
                //Log Edit
                Context.ScheduleLog.Add(new SystemScheduleLogItem
                {
                    CommunityId = communityId,
                    ScheduleType = "master",
                    PayPeriodBeginDate = DateTime.Now,//ScheduleViewModel.ParseFirstDateofWeek(payPeriod),
                    Action = "EDIT",
                    ADName = userName
                });
                Context.SaveChanges();
            }
            else
            {
                //Log Create
                Context.ScheduleLog.Add(new SystemScheduleLogItem
                {
                    CommunityId = communityId,
                    ScheduleType = "master",
                    PayPeriodBeginDate = DateTime.Now,//ScheduleViewModel.ParseFirstDateofWeek(payPeriod),
                    Action = "CREATE",
                    ADName = userName
                });
                Context.SaveChanges();
            }
        }

        public void LogTemplateDelete(int commId, string userName)
        {
            Context.ScheduleLog.Add(new SystemScheduleLogItem
            {
                CommunityId = commId,
                ScheduleType = "master",
                PayPeriodBeginDate = DateTime.Now,
                Action = "DELETE",
                ADName = userName
            });
            Context.SaveChanges();
        }



        public void LogScheduleEntry(int communityId, DateTime payPeriod, string userName, bool ScheduleExists)
        {
            if (ScheduleExists)
            {
                //Log Edit
                Context.ScheduleLog.Add(new SystemScheduleLogItem
                {
                    CommunityId = communityId,
                    ScheduleType = "manage",
                    PayPeriodBeginDate = payPeriod,//ScheduleViewModel.ParseFirstDateofWeek(payPeriod),
                    Action = "EDIT",
                    ADName = userName
                });
                Context.SaveChanges();
            }
            else
            {
                //Log Create
                Context.ScheduleLog.Add(new SystemScheduleLogItem
                {
                    CommunityId = communityId,
                    ScheduleType = "manage",
                    PayPeriodBeginDate = payPeriod,//ScheduleViewModel.ParseFirstDateofWeek(payPeriod),
                    Action = "CREATE",
                    ADName = userName
                });
                Context.SaveChanges();
            }
        }

        public void LogScheduleDelete(int commId, DateTime payPeriod, string userName)
        {
            Context.ScheduleLog.Add(new SystemScheduleLogItem
            {
                CommunityId = commId,
                ScheduleType = "manage",
                PayPeriodBeginDate = payPeriod,
                Action = "DELETE",
                ADName = userName
            });
            Context.SaveChanges();
        }
    }
}