using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Survey.Models;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "MSU")]
    public class MockSurveyAdminController : BaseAdminController
    {
        private const string AppCode = "MSU";

        public MockSurveyAdminController(IOptions<AppSettingsConfig> config, MockSurveyContext context) : base(config, context)
        {
        }

        protected new MockSurveyContext Context
        {
            get { return (MockSurveyContext)base.Context; }
        }


        public ActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            var viewModel = new MockSurveyAdminViewModel
            {
                FederalDeficiencies = Context.FederalDeficiencies.ToList(),
                SafetyDeficiencies = Context.SafetyDeficiencies.ToList(),
                DeficiencyGroups = Context.DeficiencyGroups.ToList(),
                AdminViewModel = CreateAdminViewModel(AppCode)
            };
            return View(viewModel);
        }

        //public PartialViewResult CreateDeficiencyGroup()
        //{
        //    return PartialView(new DeficiencyGroupViewModel());
        //}

        [HttpPost]
        public ActionResult CreateDeficiencyGroup(DeficiencyGroupViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newGroup = new DeficiencyGroup
                {
                    Description = viewModel.Description,
                    GroupType = viewModel.GroupType,
                    SortOrder = 0
                };
                Context.DeficiencyGroups.Add(newGroup);
                Context.SaveChanges();
                return Json(new SaveResultWithIdViewModel { Success = true, Id = newGroup.Id });
            }
            return PartialView(viewModel);
        }

        
        public PartialViewResult Instructions()
        {
            return PartialView(new InstructionsViewModel());
        }

        [HttpPost]
        public ActionResult Instructions(InstructionsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                BaseDeficiency record;
                if (viewModel.RecordType == InstructionsViewModel.DeficiencyRecordType.Federal)
                {
                    record = Context.FederalDeficiencies.Find(viewModel.Id);
                }
                else if (viewModel.RecordType == InstructionsViewModel.DeficiencyRecordType.Safety)
                {
                    record = Context.SafetyDeficiencies.Find(viewModel.Id);
                }
                else
                {
                    return NotFound("Unrecognized record type.");
                }
                if (record == null)
                {
                    return NotFound("Could not find record with specified Id and type.");
                }
                record.Instructions = viewModel.Instructions;
                Context.SaveChanges();
                return Json(new SaveResultViewModel { Success = true });
            }
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditDeficiencyGroup(EditDeficiencyGroupViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var toEdit = Context.DeficiencyGroups.Find(viewModel.Id);
                if (toEdit == null)
                {
                    return NotFound("Could not find Deficiency Group with the specified Id.");
                }
                toEdit.Description = viewModel.Description;
                toEdit.SortOrder = viewModel.SortOrder;
                Context.SaveChanges();
                return Json(new SaveResultViewModel { Success = true });
            }
            return Json(new SaveResultViewModel { Success = false });
        }

        [HttpPost]
        public ActionResult DeleteDeficiencyGroup(int id)
        {
            var toDelete = Context.DeficiencyGroups.Find(id);
            if (toDelete == null)
            {
                return NotFound("Could not find Deficiency Group with the specified Id.");
            }
            Context.DeficiencyGroups.Remove(toDelete);
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult ChangeFederalDeficiencyGroup(int id, int? groupId)
        {
            var federalDeficiency = Context.FederalDeficiencies.Find(id);
            if (federalDeficiency == null)
            {
                return NotFound("Could not find federal deficiency with the specified Id.");
            }
            federalDeficiency.DeficiencyGroupId = groupId;
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

        [HttpPost]
        public ActionResult ChangeSafetyDeficiencyGroup(int id, int? groupId)
        {
            var safetyDeficiency = Context.SafetyDeficiencies.Find(id);
            if (safetyDeficiency == null)
            {
                return NotFound("Could not find safety deficiency with the specified Id.");
            }
            safetyDeficiency.DeficiencyGroupId = groupId;
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

        public ActionResult GetNotificationRecipients(int id)
        {
            var communityShortName = FindCommunityShortName(id);
            if (communityShortName == null)
            {
                return NotFound("Could not find Community with the specified Id.");
            }
            var viewModel = new MockSurveyNotificationRecipientListViewModel
            {
                CommunityId = id,
                CommunityShortName = communityShortName,
                Items = Context.NotificationRecipients.Where(e => e.CommunityId == id).ToList()
            };
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult CreateNotificationRecipient(NotificationRecipientViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (FindCommunityShortName(viewModel.CommunityId) == null)
                {
                    return NotFound("Could not create notification recipient because Community with the specified Id could not be found.");
                }
                var record = new MockSurveyNotificationRecipient
                {
                    CommunityId = viewModel.CommunityId,
                    EmailAddress = viewModel.EmailAddress
                };
                Context.NotificationRecipients.Add(record);
                Context.SaveChanges();
                return Json(new SaveResultWithIdViewModel { Success = true, Id = record.Id });
            }
            return Json(new SaveResultViewModel { Success = false });
        }

        [HttpPost]
        public ActionResult DeleteNotificationRecipient(int id)
        {
            var record = Context.NotificationRecipients.Find(id);
            if (record == null)
            {
                return NotFound("Could not find Notification Recipient with the specified Id.");
            }
            Context.NotificationRecipients.Remove(record);
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }
    }
}