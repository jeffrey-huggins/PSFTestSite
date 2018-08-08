using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Schedule.Models;
using AtriumWebApp.Web.Schedule.Models.ViewModel;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace AtriumWebApp.Web.Schedule.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SCH", Admin = true)]
    public class ScheduleAdminController : BaseAdminController
    {
        private const string AppCode = "SCH";
        private const string PayerGroupCode = "SNF";
        private IDictionary<string, bool> _AdminAccess;

        public ScheduleAdminController(IOptions<AppSettingsConfig> config, ScheduleAdminContext context) : base(config, context)
        {
        }

        protected new ScheduleAdminContext Context
        {
            get { return (ScheduleAdminContext)base.Context; }
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

        public int? EditingId
        {
            get
            {
                Session.TryGetObject(AppCode + "EditingId", out int? id);
                return id;
            }
            private set { Session.SetItem(AppCode + "EditingId", value); }
        }


        public ActionResult Index()
        {
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
                return redirectToAction;

            var viewModel = new ScheduleAdminViewModel
            {
                AdminViewModel = CreateAdminViewModel(AppCode),
                AreaRooms = GetAllAreaRooms(),
                IsAdministrator = this.IsAdministrator
            };
            return View(viewModel);
        }

        private IList<MasterAtriumPatientGroup> GetAllAreaRooms()
        {
            return (from rooms in Context.AreaRoom
                    select rooms).OrderBy(g => g.AtriumPatientGroupName)
                    .ToList<MasterAtriumPatientGroup>();
        }

        public ActionResult GetAreaRooms(int id)
        {
            var communityShortName = FindCommunityShortName(id);
            if (communityShortName == null)
            {
                return NotFound("Could not find Community with the specified Id.");
            }
            var viewModel = new ScheduleAdminAreaRoomsViewModel
            {
                IsAdministrator = this.IsAdministrator,
                AreaRooms = Context.AreaRoom.Where(e => e.CommunityId == id).ToList(),
                AdminViewModel = CreateAdminViewModel(AppCode)
            };
            return PartialView(viewModel);
        }

        [HttpPost]
        public JsonResult DeleteAreaRoom(int id)
        {
            var record = Context.AreaRoom.Find(id);
            if (record == null)
                return Json(new SaveResultViewModel { Success = false });
            try
            {
                Context.AreaRoom.Remove(record);
                Context.SaveChanges();
                return Json(new SaveResultViewModel { Success = true });
            }
            catch (System.Exception e)
            {
                if (e != null && e.InnerException != null &&
                    e.InnerException.InnerException != null &&
                    e.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                    return Json(new { Success = false, Msg = "The Hall/Unit is in use and may not be removed." });
                return Json(new SaveResultViewModel { Success = false });
            }
        }

        [HttpPost]
        public JsonResult EditAreaRoom(int id, string name) //, int communityId
        {
            var record = Context.AreaRoom.Find(id);
            if (record == null)
                record = new MasterAtriumPatientGroup();
            //return Json(new SaveResultViewModel { Success = false });
            record.AtriumPatientGroupName = name;
            //record.CommunityId = communityId;
            Context.Entry(record).State = EntityState.Modified;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        // ORIGINALS

        [HttpPost]
        public JsonResult AddRoomArea(string newRoomArea)
        {
            MasterAtriumPatientGroup newRecord =
                new MasterAtriumPatientGroup { AtriumPatientGroupName = newRoomArea, CommunityId = CurrentFacility[AppCode] };
            Context.AreaRoom.Add(newRecord);
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            int areaRoomId = Context.Entry(newRecord).Property(p => p.Id).CurrentValue;
            return Json(new { Success = true, id = areaRoomId });
        }

        [HttpPost]
        public JsonResult DeleteRoomArea(int id)
        {
            MasterAtriumPatientGroup roomArea = Context.AreaRoom.Find(id);
            if (roomArea != null)
            {
                Context.AreaRoom.Remove(roomArea);
                Context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        //[HttpGet, ActionName("EditRoomAreaForCommunity")]
        public ActionResult GetAreaRoomsForCommunity(int id)
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return PartialView(redirectToAction);
            }
            SetCommunity(id);
            var communityShortName = FindCommunityShortName(id);
            if (communityShortName == null)
            {
                return NotFound("Could not find Community with the specified Id.");
            }
            return PartialView(CreateAreaRoomsViewModel(id));
        }

        //[HttpPost]
        //public PartialViewResult EditRoomAreaForCommunity(int? communityId)
        //{
        //    var redirectToAction = DetermineWebpageAccess(AppCode);
        //    if (redirectToAction != null)
        //    {
        //        return PartialView(redirectToAction);
        //    }
        //    SetCommunity(communityId);
        //    return PartialView(CreateAreaRoomsViewModel(communityId));
        //}

        private Community SetCommunity(int? communityId)
        {
            CurrentFacility[AppCode] = communityId ?? 0;
            Community facility = Context.Facilities.Single(fac => fac.CommunityId == (communityId ?? 0));
            return facility;
        }

        private ScheduleAdminAreaRoomsViewModel CreateAreaRoomsViewModel(int? currentCommunity)
        {
            List<MasterAtriumPatientGroup> items = new List<MasterAtriumPatientGroup>();
            if (currentCommunity != null && currentCommunity > -1)
                items = (from rooms in Context.AreaRoom
                         where rooms.CommunityId == currentCommunity //CurrentFacility[AppCode]
                         select rooms).OrderBy(g => g.AtriumPatientGroupName)
                        .ToList<MasterAtriumPatientGroup>();
            return new ScheduleAdminAreaRoomsViewModel
            {
                AdminViewModel = CreateAdminViewModel(AppCode),
                AreaRooms = items,
                CommunityId = currentCommunity ?? 0,
                CommunityShortName = FindCommunityShortName(currentCommunity ?? 0)
            };
        }

        /*
        public ActionResult SlotNbr()
        {
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode); 
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            int? currentCommunity = null;
            try
            {
                currentCommunity = CurrentFacility[AppCode] as int?;
            } catch(Exception) {}
            return View(CreateViewModel(currentCommunity ?? null));
            //return View(CreateViewModel(null));
        }

        private ScheduleAdminViewModel CreateViewModel(int? currentCommunity)
        {
            List<MasterAtriumPatientGroup> items = new List<MasterAtriumPatientGroup>();
            if (currentCommunity != null && currentCommunity > -1)
                items = (from rooms in Context.AreaRoom
                         where rooms.CommunityId == currentCommunity //CurrentFacility[AppCode]
                        select rooms).OrderBy(g => g.AtriumPatientGroupName)
                        .ToList<MasterAtriumPatientGroup>();

            // Limit Community List by Active Directory.
            List<Community> communities = FacilityList[AppCode].ToList();
            List<SelectListItem> communitiesList =
                communities.Select(x => new SelectListItem() { Value = x.CommunityId.ToString(), Text = x.CommunityShortName })
                            .ToList<SelectListItem>();
            if (communitiesList != null && communitiesList.Count > 0)
                communitiesList[0].Selected = true;

            return new ScheduleAdminViewModel
            {
                CurrentCommunity = currentCommunity,
                Communities = communitiesList, //controller.GetCommunitiesListItems(),
                AdminViewModel = CreateAdminViewModel(AppCode),
                AreaRooms = items
            };
        }

         * private Community SetCommunity(string currentCommunity)
        {
            var facilityId = 0;
            if (!Int32.TryParse(currentCommunity, out facilityId))
            {
                throw new System.NullReferenceException("SetCommunity: input currentCommunity does not parse to integer.");
            }
            return SetCommunity(facilityId);
        }

        [HttpPost]
        public ActionResult UpdateCurrentCommunity(string currentCommunity, string returnUrl)
        {
            Community facility = null;
            try
            {
                facility = SetCommunity(currentCommunity);
            }
            catch (Exception e)
            {
                Redirect(returnUrl);
            }
            if (facility != null)
                CurrentFacilityName[AppCode] = facility.CommunityShortName;
            EditingId = null;
            return Redirect(returnUrl);
        }
         
         * */

        private IList<MasterAtriumPatientGroup> GetAreaRoomsForCommunity(int? commnityId)
        {
            List<MasterAtriumPatientGroup> items = new List<MasterAtriumPatientGroup>();
            if (commnityId != null && commnityId > -1)
                items = (from rooms in Context.AreaRoom
                         where rooms.CommunityId == commnityId //CurrentFacility[AppCode]
                         select rooms).OrderBy(g => g.AtriumPatientGroupName)
                        .ToList<MasterAtriumPatientGroup>();
            return items;
        }

    }
}
