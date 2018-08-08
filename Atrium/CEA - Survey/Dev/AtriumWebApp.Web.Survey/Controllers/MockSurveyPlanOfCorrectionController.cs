using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Survey.Models;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "MSUPOC")]
    public class MockSurveyPlanOfCorrectionController : BaseController
    {
        private const string AppCode = "MSUPOC";

        public MockSurveyPlanOfCorrectionController(IOptions<AppSettingsConfig> config, MockSurveyContext context) : base(config, context)
        {
        }

        protected new MockSurveyContext Context
        {
            get { return (MockSurveyContext)base.Context; }
        }

        private bool IsAdministrator
        {
            get
            {
                var adminDictionary = DetermineAdminAccess(PrincipalContext, UserPrincipal);
                bool isAdministrator;
                if (adminDictionary.TryGetValue(AppCode, out isAdministrator))
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
            set
            {

                if (value != null)
                {
                    Session.SetItem(AppCode + "EditingId", value);
                }
                else
                {
                    Session.Remove(AppCode + "EditingId");
                }
            }
        }

        public ActionResult Index()
        {
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesDropDownWithFilter(AppCode);
            var currentCommunity = CurrentFacility[AppCode];
            var fromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var toDate = DateTime.Parse(OccurredRangeTo[AppCode]);
            var viewModel = new MockSurveyPlanOfCorrectionIndexViewModel
            {
                Items = Context.MockSurveys.Where(m => m.CommunityId == currentCommunity
                                                    && m.MockSurveyDate >= fromDate
                                                    && m.MockSurveyDate <= toDate && m.ClosedDate.HasValue).ToList(),
                CurrentCommunity = currentCommunity,
                Communities = FacilityList[AppCode],
                OccurredRangeFrom = fromDate,
                OccurredRangeTo = toDate,
                IsAdministrator = IsAdministrator
            };
            if (EditingId.HasValue)
            {
                viewModel.PlanOfCorrection = ((PartialViewResult)PlanCorrections(EditingId.Value)).Model as MockSurveyPlanOfCorrectionViewModel;
            }
            return View(viewModel);
        }

        
        public ActionResult PlanCorrections(int id)
        {
            var mockSurvey = Context.MockSurveys
                                    .Include(e => e.Community)
                                    .Include("MockFederalCitations.Deficiency.DeficiencyGroup")
                                    .Include("MockSafetyCitations.Deficiency.DeficiencyGroup")
                                    .SingleOrDefault(e => e.Id == id);
            if (mockSurvey == null)
            {
                return NotFound("Could not find mock survey with the specified Id.");
            }
            if (mockSurvey.IsPlanOfCorrectionComplete && !IsAdministrator)
            {
                return new UnauthorizedResult();
                //return new HttpUnauthorizedResult("Cannot edit completed plan of correction for mock survey.");
            }
            var viewModel = new MockSurveyPlanOfCorrectionViewModel
            {
                MockSurveyId = id,
                ClosedDate = mockSurvey.ClosedDate,
                FollowUpDate = mockSurvey.FollowUpDate,
                MockSurveyDate = mockSurvey.MockSurveyDate,
                Close = mockSurvey.IsPlanOfCorrectionComplete,
                FederalGroups = mockSurvey.MockFederalCitations.Select(e => e.Deficiency.DeficiencyGroup).Distinct().Select(e => new MockSurveyPlanOfCorrectionGroupViewModel
                {
                    CommunityShortName = mockSurvey.Community.CommunityShortName,
                    MockSurveyId = mockSurvey.Id,
                    GroupId = e.Id,
                    GroupType = e.GroupType,
                    Description = e.Description
                }).ToList(),
                SafetyGroups = mockSurvey.MockSafetyCitations.Select(e => e.Deficiency.DeficiencyGroup).Distinct().Select(e => new MockSurveyPlanOfCorrectionGroupViewModel
                {
                    CommunityShortName = mockSurvey.Community.CommunityShortName,
                    MockSurveyId = mockSurvey.Id,
                    GroupId = e.Id,
                    GroupType = e.GroupType,
                    Description = e.Description
                }).Distinct().ToList()
            };
            EditingId = id;
            return PartialView("_Dashboard", viewModel);
        }

        [HttpPost]
        public ActionResult PlanCorrections(MockSurveyPlanOfCorrectionViewModel viewModel)
        {
            var mockSurvey = Context.MockSurveys.Find(viewModel.MockSurveyId);
            if (mockSurvey == null)
            {
                return NotFound("Could not find mock survey with the specified Id.");
            }
            if (mockSurvey.IsPlanOfCorrectionComplete && !IsAdministrator)
            {
                return new UnauthorizedResult();
                //return new HttpUnauthorizedResult("Cannot edit close flag of plan of correction for mock survey.");
            }
            if (viewModel.Close && !mockSurvey.PlanOfCorrectionCompleteDate.HasValue)
            {
                mockSurvey.PlanOfCorrectionCompleteDate = DateTime.Now;
            }
            else if (!viewModel.Close && mockSurvey.PlanOfCorrectionCompleteDate.HasValue && IsAdministrator)
            {
                mockSurvey.PlanOfCorrectionCompleteDate = null;
            }
            Context.SaveChanges();
            EditingId = null;
            return Json(new MockSurveySaveResultViewModel
            {
                Success = true,
                IsClosed = mockSurvey.IsPlanOfCorrectionComplete,
                FormattedClosedDate = mockSurvey.IsPlanOfCorrectionComplete ? mockSurvey.PlanOfCorrectionCompleteDate.Value.ToShortDateString() : string.Empty
            });
        }

        
        public ActionResult Group(int id, int groupId)
        {
            var group = Context.DeficiencyGroups.SingleOrDefault(e => e.Id == groupId);
            if (group == null)
            {
                return NotFound("Could not find deficiency group to plan corrections on.");
            }
            IEnumerable<BaseMockCitation> citations;
            if (group.GroupType == "Federal")
            {
                citations = Context.MockFederalCitations
                                   .Include(e => e.Deficiency.DeficiencyGroup)
                                   .Where(e => e.MockSurveyId == id && e.Deficiency.DeficiencyGroupId == groupId);
            }
            else
            {
                citations = Context.MockSafetyCitations
                                   .Include(e => e.Deficiency.DeficiencyGroup)
                                   .Where(e => e.MockSurveyId == id && e.Deficiency.DeficiencyGroupId == groupId);
            }
            var viewModel = new MockSurveyPlanOfCorrectionGroupViewModel
            {
                CommunityShortName = Context.MockSurveys.Where(e => e.Id == id).Select(e => e.Community.CommunityShortName).Single(),
                MockSurveyId = id,
                GroupId = groupId,
                GroupType = group.GroupType,
                Description = group.Description,
                Citations = citations.Select(e => new CitationPlanOfCorrectionViewModel
                {
                    CitationId = e.Id,
                    DeficiencyDescription = e.BaseDeficiency.Description,
                    DeficiencyInstructions = e.BaseDeficiency.Instructions,
                    FindingDetails = e.CitationComments,
                    MockSeverity = e.MockSeverity,
                    PlanOfCorrection = e.PlanOfCorrection
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Group(MockSurveyPlanOfCorrectionGroupViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var group = Context.DeficiencyGroups.SingleOrDefault(e => e.Id == viewModel.GroupId);
                if (group == null)
                {
                    return NotFound("Could not find deficiency group to plan corrections on.");
                }
                IEnumerable<BaseMockCitation> citations;
                var citationIds = viewModel.Citations.Select(c => c.CitationId);
                if (group.GroupType == "Federal")
                {
                    citations = Context.MockFederalCitations.Where(e => e.MockSurveyId == viewModel.MockSurveyId
                                                                     && e.Deficiency.DeficiencyGroup.Id == viewModel.GroupId
                                                                     && citationIds.Contains(e.Id)).ToList();
                }
                else
                {
                    citations = Context.MockSafetyCitations.Where(e => e.MockSurveyId == viewModel.MockSurveyId
                                                                    && e.Deficiency.DeficiencyGroup.Id == viewModel.GroupId
                                                                    && citationIds.Contains(e.Id)).ToList();
                }
                foreach (var citationViewModel in viewModel.Citations)
                {
                    var citation = citations.Single(e => e.Id == citationViewModel.CitationId);
                    citation.PlanOfCorrection = citationViewModel.PlanOfCorrection;
                }
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return Group(viewModel.MockSurveyId, viewModel.GroupId);
        }

        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        {
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }

        [HttpPost]
        public RedirectToActionResult UpdateCurrentCommunity(int currentCommunity)
        {
            CurrentFacility[AppCode] = currentCommunity;
            EditingId = null;
            return RedirectToAction("Index");
        }
    }
}