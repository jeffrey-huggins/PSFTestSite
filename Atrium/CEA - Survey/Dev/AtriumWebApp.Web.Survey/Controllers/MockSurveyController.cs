using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Survey.Models;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "MSU")]
    public class MockSurveyController : BaseController
    {
        private const string AppCode = "MSU";

        public MockSurveyController(IOptions<AppSettingsConfig> config, MockSurveyContext context) : base(config, context)
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
                    FollowUpId = null;
                }
                else
                {
                    Session.Remove(AppCode + "EditingId");
                }
            }
        }
        public int? FollowUpId
        {
            get
            {
                Session.TryGetObject(AppCode + "FollowUpId", out int? id);
                return id;
            }
            set
            {

                if (value != null)
                {
                    Session.SetItem(AppCode + "FollowUpId", value);
                    EditingId = null;
                }
                else
                {
                    Session.Remove(AppCode + "FollowUpId");
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
            var viewModel = new MockSurveyIndexViewModel
            {
                Items = Context.MockSurveys.Where(m => m.CommunityId == currentCommunity
                                                    && m.MockSurveyDate >= fromDate
                                                    && m.MockSurveyDate <= toDate).ToList(),
                CurrentCommunity = currentCommunity,
                Communities = FacilityList[AppCode],
                OccurredRangeFrom = fromDate,
                OccurredRangeTo = toDate,
                CanDeleteClosed = DetermineObjectAccess("0002", currentCommunity, AppCode),
                CanEditClosed = DetermineObjectAccess("0001", currentCommunity, AppCode)
            };
            if (EditingId.HasValue)
            {
                viewModel.Current = ((PartialViewResult)Edit(EditingId.Value)).Model as MockSurveyViewModel;
            }
            if (FollowUpId.HasValue)
            {
                viewModel.Current = ((PartialViewResult)FollowUp(FollowUpId.Value)).Model as MockSurveyPlanOfCorrectionViewModel;
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(string id)
        {
            var community = FindCommunityId(id);
            if (community == 0)
            {
                return NotFound("Community could not be found.");
            }
            var mockSurvey = new MockSurvey();
            mockSurvey.CommunityId = community;
            Context.MockSurveys.Add(mockSurvey);
            Context.SaveChanges();
            EditingId = mockSurvey.Id;
            CurrentFacility[AppCode] = community;
            CurrentFacilityName[AppCode] = id;
            var viewModel = new MockSurveyViewModel
            {
                MockSurveyId = mockSurvey.Id,
                CommunityId = community,
                MockSurveyDate = mockSurvey.MockSurveyDate,
                FederalGroups = Context.DeficiencyGroups.OrderBy(e => e.SortOrder).ThenBy(e => e.Description).Where(e => e.GroupType == "Federal").Select(e => new MockSurveyGroupViewModel
                {
                    CommunityShortName = id,
                    MockSurveyId = mockSurvey.Id,
                    GroupId = e.Id,
                    GroupType = e.GroupType,
                    Description = e.Description
                }).ToList(),
                SafetyGroups = Context.DeficiencyGroups.OrderBy(e => e.SortOrder).ThenBy(e => e.Description).Where(e => e.GroupType == "Safety").Select(e => new MockSurveyGroupViewModel
                {
                    CommunityShortName = id,
                    MockSurveyId = mockSurvey.Id,
                    GroupId = e.Id,
                    GroupType = e.GroupType,
                    Description = e.Description
                }).ToList(),
                TeamMembers = mockSurvey.TeamMembers,
                CanEditClosed = DetermineObjectAccess("0003", community, AppCode)
            };
            return PartialView("_Dashboard", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var mockSurvey = Context.MockSurveys.Include(m => m.Community).SingleOrDefault(m => m.Id == id);
            if (mockSurvey == null)
            {
                return NotFound("Community could not be found.");
            }
            if (mockSurvey.IsClosed && !DetermineObjectAccess("0001", mockSurvey.CommunityId, AppCode))
            {
                return new UnauthorizedResult();
                //return new HttpUnauthorizedResult("Cannot edit closed mock survey.");
            }
            var viewModel = new MockSurveyViewModel
            {
                MockSurveyId = mockSurvey.Id,
                CommunityId = mockSurvey.CommunityId,
                ClosedDate = mockSurvey.ClosedDate,
                Close = mockSurvey.ClosedDate.HasValue,
                CloseSignature = mockSurvey.ClosedSignature,
                FollowUpDate = mockSurvey.FollowUpDate,
                MockSurveyDate = mockSurvey.MockSurveyDate,
                FederalGroups = Context.DeficiencyGroups.OrderBy(e => e.SortOrder).ThenBy(e => e.Description).Where(e => e.GroupType == "Federal").Select(e => new MockSurveyGroupViewModel
                {
                    CommunityShortName = mockSurvey.Community.CommunityShortName,
                    MockSurveyId = mockSurvey.Id,
                    GroupId = e.Id,
                    GroupType = e.GroupType,
                    Description = e.Description
                }).ToList(),
                SafetyGroups = Context.DeficiencyGroups.OrderBy(e => e.SortOrder).ThenBy(e => e.Description).Where(e => e.GroupType == "Safety").Select(e => new MockSurveyGroupViewModel
                {
                    CommunityShortName = mockSurvey.Community.CommunityShortName,
                    MockSurveyId = mockSurvey.Id,
                    GroupId = e.Id,
                    GroupType = e.GroupType,
                    Description = e.Description
                }).ToList(),
                TeamMembers = mockSurvey.TeamMembers,
                CanEditClosed = DetermineObjectAccess("0003", mockSurvey.CommunityId, AppCode)
            };
            EditingId = mockSurvey.Id;
            return PartialView("_Dashboard", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(MockSurveyViewModel viewModel)
        {
            var mockSurvey = Context.MockSurveys.Include(e => e.Community).SingleOrDefault(e => e.Id == viewModel.MockSurveyId);
            if (mockSurvey == null)
            {
                return NotFound("Could not find mock survey with the specified Id.");
            }
            if (mockSurvey.IsClosed && !DetermineObjectAccess("0001", mockSurvey.CommunityId, AppCode))
            {
                return new UnauthorizedResult();
                //return new HttpUnauthorizedResult("Cannot edit closed mock survey.");
            }
            if (ModelState.IsValid)
            {
                mockSurvey.MockSurveyDate = viewModel.MockSurveyDate;
                mockSurvey.FollowUpDate = viewModel.FollowUpDate;
                mockSurvey.TeamMembers = viewModel.TeamMembers;
                if (viewModel.Close && !mockSurvey.ClosedDate.HasValue)
                {
                    var notificationRecipients = Context.NotificationRecipients.Where(r => r.CommunityId == mockSurvey.CommunityId).Select(r => r.EmailAddress).ToList();
                    if (notificationRecipients.Count > 0)
                    {
                        var messageSubject = string.Format("[TEST] Plan of Correction Required for {0} [TEST]", mockSurvey.Community.CommunityShortName);
                        var messageBody = string.Format(@"This email is to inform you that a Customer Service and Compliance Survey has been completed for your facility.  The follow-up date is tentatively scheduled for {0}.  

Please log into Atrium’s Survey application and complete your plan of correction within 14 days.", mockSurvey.FollowUpDate.Value.ToShortDateString());
                        var sendMail = new Thread(() =>
                        {
                            try
                            {
                                MailHelper.SendEmailToListOfEmails(notificationRecipients, messageSubject, messageBody, true);
                            }
                            catch (AggregateException e)
                            {
                                Trace.TraceError("Failed sending Mock Survey notification email.\r\n{0}", e.ToString());
                            }
                        });
                        sendMail.Start();
                    }

                    mockSurvey.ClosedDate = DateTime.Now;
                    mockSurvey.ClosedSignature = viewModel.CloseSignature;
                }
                else if (!viewModel.Close && mockSurvey.ClosedDate.HasValue && DetermineObjectAccess("0003", mockSurvey.CommunityId, AppCode))
                {
                    mockSurvey.ClosedDate = null;
                    mockSurvey.ClosedSignature = null;
                }
                Context.SaveChanges();
                EditingId = null;
                return Json(new MockSurveySaveResultViewModel
                {
                    Success = true,
                    IsClosed = mockSurvey.IsClosed,
                    FormattedClosedDate = mockSurvey.IsClosed ? mockSurvey.ClosedDate.Value.ToShortDateString() : string.Empty,
                    CanEditCompleted = DetermineObjectAccess("0001", mockSurvey.CommunityId, "MSUPOC")
                });
            }
            return Edit(viewModel.MockSurveyId);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var mockSurvey = Context.MockSurveys.SingleOrDefault(r => r.Id == id);
            if (mockSurvey == null)
            {
                return NotFound("Could not find mock survey with the specified Id.");
            }
            if (mockSurvey.IsClosed && !DetermineObjectAccess("0002", mockSurvey.CommunityId, AppCode))
            {
                return new UnauthorizedResult();
                //return new HttpUnauthorizedResult("Cannot delete closed mock survey.");
            }
            Context.MockSurveys.Remove(mockSurvey);
            Context.SaveChanges();
            if (EditingId.HasValue && EditingId.Value == id)
            {
                EditingId = null;
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public ActionResult Group(int id, int groupId)
        {
            var group = Context.DeficiencyGroups
                               .Include(e => e.FederalDeficiencies)
                               .Include(e => e.SafetyDeficiencies)
                               .SingleOrDefault(e => e.Id == groupId);
            if (group == null)
            {
                return NotFound("Could not find deficiency group to conduct mock survey on.");
            }
            IEnumerable<BaseMockCitation> citations;
            IEnumerable<BaseDeficiency> deficiencies;
            if (group.GroupType == "Federal")
            {
                citations = Context.MockFederalCitations
                                   .Include(e => e.MockSurvey)
                                   .Where(e => e.MockSurveyId == id && e.Deficiency.DeficiencyGroupId == groupId);
                deficiencies = group.FederalDeficiencies;
            }
            else
            {
                citations = Context.MockSafetyCitations
                                   .Include(e => e.MockSurvey)
                                   .Where(e => e.MockSurveyId == id && e.Deficiency.DeficiencyGroupId == groupId);
                deficiencies = group.SafetyDeficiencies;
            }
            var viewModel = new MockSurveyGroupViewModel
            {
                CommunityShortName = Context.MockSurveys.Where(e => e.Id == id).Select(e => e.Community.CommunityShortName).Single(),
                MockSurveyId = id,
                GroupId = groupId,
                GroupType = group.GroupType,
                Description = group.Description,
                Deficiencies = deficiencies.Select(d => new MockSurveyDeficiencyViewModel
                {
                    DeficiencyId = d.Id,
                    Tag = d.TagCode,
                    Description = d.Description,
                    Instructions = d.Instructions,
                    Citation = citations.Where(c => c.DeficiencyId == d.Id).Select(c => new CitationViewModel
                    {
                        CitationId = c.Id,
                        GroupType = group.GroupType,
                        DeficiencyId = d.Id,
                        FindingDetails = c.CitationComments,
                        MockSeverity = c.MockSeverity,
                        MockSurveyId = c.MockSurveyId
                    }).SingleOrDefault()
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateCitation(CitationViewModel viewModel)
        {
            BaseMockCitation citation;
            BaseDeficiency deficiency;
            if (ModelState.IsValid)
            {
                if (viewModel.GroupType == "Federal")
                {
                    var federalCitation = new MockFederalCitation
                    {
                        DeficiencyId = viewModel.DeficiencyId.Value
                    };
                    deficiency = Context.FederalDeficiencies.Find(viewModel.DeficiencyId);
                    Context.MockFederalCitations.Add(federalCitation);
                    citation = federalCitation;
                }
                else
                {
                    var safetyCitation = new MockSafetyCitation
                    {
                        DeficiencyId = viewModel.DeficiencyId.Value
                    };
                    deficiency = Context.SafetyDeficiencies.Find(viewModel.DeficiencyId);
                    Context.MockSafetyCitations.Add(safetyCitation);
                    citation = safetyCitation;
                }
                if (deficiency == null)
                {
                    return NotFound("Could not find deficiency with the specified Id.");
                }
                citation.MockSeverity = viewModel.MockSeverity;
                citation.MockSurveyId = viewModel.MockSurveyId;
                citation.CitationComments = viewModel.FindingDetails;
                Context.SaveChanges();
                return Json(new SaveResultWithIdViewModel
                {
                    Success = true,
                    Id = citation.Id
                });
            }
            else
            {
                return Json(new SaveResultViewModel { Success = false });
            }
        }

        [HttpPost]
        public ActionResult EditCitation(CitationViewModel viewModel)
        {
            BaseMockCitation citation;
            if (ModelState.IsValid)
            {
                if (viewModel.GroupType == "Federal")
                {
                    var federalCitation = Context.MockFederalCitations.Find(viewModel.CitationId);
                    citation = federalCitation;
                }
                else
                {
                    var safetyCitation = Context.MockSafetyCitations.Find(viewModel.CitationId);
                    citation = safetyCitation;
                }
                citation.MockSeverity = viewModel.MockSeverity;
                citation.CitationComments = viewModel.FindingDetails;
                Context.SaveChanges();
                return Json(new SaveResultWithIdViewModel
                {
                    Success = true,
                    Id = citation.Id
                });
            }
            else
            {
                return Json(new SaveResultViewModel { Success = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteCitation(CitationIdentityViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.GroupType == "Federal")
                {
                    var citation = Context.MockFederalCitations.Find(viewModel.CitationId);
                    Context.MockFederalCitations.Remove(citation);
                }
                else
                {
                    var citation = Context.MockSafetyCitations.Find(viewModel.CitationId);
                    Context.MockSafetyCitations.Remove(citation);
                }
                Context.SaveChanges();
                return Json(new SaveResultViewModel { Success = true });
            }
            return Json(new SaveResultViewModel { Success = false });
        }

        public ActionResult FollowUp(int id)
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
            var viewModel = new MockSurveyPlanOfCorrectionViewModel
            {
                MockSurveyId = id,
                ClosedDate = mockSurvey.ClosedDate,
                FollowUpDate = mockSurvey.FollowUpDate,
                MockSurveyDate = mockSurvey.MockSurveyDate,
                Close = mockSurvey.IsFollowUpComplete,
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
            FollowUpId = id;
            return PartialView("_FollowUpDashboard", viewModel);
        }

        [HttpPost]
        public ActionResult FollowUp(MockSurveyPlanOfCorrectionViewModel viewModel)
        {
            var mockSurvey = Context.MockSurveys.Find(viewModel.MockSurveyId);
            if (mockSurvey == null)
            {
                return NotFound("Could not find mock survey with the specified Id.");
            }
            if (mockSurvey.IsFollowUpComplete && !DetermineObjectAccess("0001", mockSurvey.CommunityId, "MSUPOC"))
            {
                return new UnauthorizedResult();
                //return new HttpUnauthorizedResult("Cannot edit close flag of follow-up for mock survey.");
            }
            if (viewModel.Close && !mockSurvey.FollowUpCompleteDate.HasValue)
            {
                mockSurvey.FollowUpCompleteDate = DateTime.Now;
            }
            else if (!viewModel.Close && mockSurvey.FollowUpCompleteDate.HasValue && DetermineObjectAccess("0001", mockSurvey.CommunityId, "MSUPOC"))
            {
                mockSurvey.FollowUpCompleteDate = null;
            }
            Context.SaveChanges();
            FollowUpId = null;
            return Json(new MockSurveySaveResultViewModel
            {
                Success = true,
                IsClosed = mockSurvey.IsFollowUpComplete,
                FormattedClosedDate = mockSurvey.IsFollowUpComplete ? mockSurvey.FollowUpCompleteDate.Value.ToShortDateString() : string.Empty,
                CanEditCompleted = DetermineObjectAccess("0001", mockSurvey.CommunityId, "MSUPOC")
            });
        }

        public ActionResult FollowUpGroup(int id, int groupId)
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
            var viewModel = new MockSurveyFollowUpGroupViewModel
            {
                CommunityShortName = Context.MockSurveys.Where(e => e.Id == id).Select(e => e.Community.CommunityShortName).Single(),
                MockSurveyId = id,
                GroupId = groupId,
                GroupType = group.GroupType,
                Description = group.Description,
                Citations = citations.Select(e => new CitationFollowUpViewModel
                {
                    CitationId = e.Id,
                    DeficiencyDescription = e.BaseDeficiency.Description,
                    DeficiencyInstructions = e.BaseDeficiency.Instructions,
                    FindingDetails = e.CitationComments,
                    MockSeverity = e.MockSeverity,
                    PlanOfCorrection = e.PlanOfCorrection,
                    FollowUpDetails = e.FollowUpComments,
                    IsCompliant = e.IsCompliant
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult FollowUpGroup(MockSurveyFollowUpGroupViewModel viewModel)
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
                    citation.FollowUpComments = citationViewModel.FollowUpDetails;
                    citation.IsCompliant = citationViewModel.IsCompliant;
                }
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return FollowUpGroup(viewModel.MockSurveyId, viewModel.GroupId);
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
            FollowUpId = null;
            return RedirectToAction("Index");
        }
    }
}