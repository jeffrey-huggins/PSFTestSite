using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "QSR")]
    public class QuarterlySystemsReviewController : BaseController
    {
        private const string AppCode = "QSR";

        public QuarterlySystemsReviewController(IOptions<AppSettingsConfig> config, QuarterlySystemsReviewContext context) : base(config, context)
        {
        }

        protected new QuarterlySystemsReviewContext Context
        {
            get { return (QuarterlySystemsReviewContext)base.Context; }
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
            SetInitialTableRange(AppCode);
            GetCommunitiesDropDownWithFilter(AppCode);
            var currentCommunity = CurrentFacility[AppCode];
            var fromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var toDate = DateTime.Parse(OccurredRangeTo[AppCode]);
            var viewModel = new ReviewIndexViewModel
            {
                Items = Context.Reviews.Where(po => po.CommunityId == currentCommunity
                                                 && po.ReviewDate >= fromDate
                                                 && po.ReviewDate <= toDate).ToList(),
                CurrentCommunity = currentCommunity,
                Communities = FacilityList[AppCode],
                OccurredRangeFrom = fromDate,
                OccurredRangeTo = toDate,
                CanDelete = DetermineObjectAccess("0002", currentCommunity, AppCode),
                CanEdiit = DetermineObjectAccess("0001", currentCommunity, AppCode),
                CanEditDate = DetermineObjectAccess("0003", currentCommunity, AppCode),
                CanCloseDietary = DetermineObjectAccess("0004", currentCommunity, AppCode),
                CanCloseNursing = DetermineObjectAccess("0005", currentCommunity, AppCode)

            };
            if (EditingId.HasValue)
            {
                viewModel.Review = ((PartialViewResult)Edit(EditingId.Value)).Model as ReviewViewModel;
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(string id)
        {
            var community = FindCommunity(id);
            if (community == null)
            {
                return NotFound("Community could not be found.");
            }
            var review = PersistNewReview(community);
            var viewModel = CreateViewModel(review);
            viewModel.CanCloseDietary = DetermineObjectAccess("0004", viewModel.CommunityId, AppCode);
            viewModel.CanCloseNursing = DetermineObjectAccess("0005", viewModel.CommunityId, AppCode);
            viewModel.CanDelete = DetermineObjectAccess("0002", viewModel.CommunityId, AppCode);
            viewModel.CanEdiit = DetermineObjectAccess("0001", viewModel.CommunityId, AppCode);
            viewModel.CanEditDate = DetermineObjectAccess("0003", viewModel.CommunityId, AppCode);
            EditingId = review.Id;
            return PartialView("_Dashboard", viewModel);
        }

        
        public ActionResult Edit(int id)
        {
            var review = Context.Reviews.Include(r => r.Community)
                                        .Include("StandardsOfCareSections.ReviewSamples.ReviewAnswers")
                                        .Include("StandardsOfCareSections.ReviewMeasure.ReviewQuestions")
                                        .Include("GeneralSections.ReviewSamples.ReviewAnswers")
                                        .Include("GeneralSections.ReviewMeasure.ReviewQuestions")
                                        .Include("GeneralSections.ReviewAnswers")
                                        .Include(r => r.AdditionalAnswers)
                                        .SingleOrDefault(r => r.Id == id);
            if (review == null)
            {
                return NotFound("Could not find specified review.");
            }

            if (review.IsClosed && !DetermineObjectAccess("0001", review.CommunityId, AppCode))
            {
                return new UnauthorizedResult();
            }
            var viewModel = CreateViewModel(review);
            viewModel.CanCloseDietary = DetermineObjectAccess("0004", viewModel.CommunityId, AppCode);
            viewModel.CanCloseNursing = DetermineObjectAccess("0005", viewModel.CommunityId, AppCode);
            viewModel.CanDelete = DetermineObjectAccess("0002", viewModel.CommunityId, AppCode);
            viewModel.CanEdiit = DetermineObjectAccess("0001", viewModel.CommunityId, AppCode);
            viewModel.CanEditDate = DetermineObjectAccess("0003", viewModel.CommunityId, AppCode);
            EditingId = id;
            return PartialView("_Dashboard", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ReviewViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var review = Context.Reviews.Include("StandardsOfCareSections")
                                            .Include("GeneralSections")
                                            .SingleOrDefault(r => r.Id == viewModel.ReviewId);
                if (review == null)
                {
                    return NotFound("Could not find review with specified Id.");
                }

                if (review.IsClosed && !DetermineObjectAccess("0001", viewModel.CommunityId, AppCode))
                {
                    return new UnauthorizedResult();
                    //return new HttpUnauthorizedResult("Cannot edit closed review.");
                }
                foreach (var sectionViewModel in viewModel.Sections.Where(s => s.SectionType == ReviewSectionType.StandardsOfCare))
                {
                    var section = review.StandardsOfCareSections.Single(s => s.Id == sectionViewModel.Id);
                    section.Comments = sectionViewModel.Comments;
                }
                foreach (var sectionViewModel in viewModel.Sections.Where(s => s.SectionType == ReviewSectionType.General))
                {
                    var section = review.GeneralSections.Single(s => s.Id == sectionViewModel.Id);
                    section.Comments = sectionViewModel.Comments;
                }
                review.ReviewDate = viewModel.ReviewDate;
                review.BeginSampleDate = viewModel.BeginSampleDate;
                review.EndSampleDate = viewModel.EndSampleDate;
                if (viewModel.CloseNursing && !review.IsClosedByNurse)
                {
                    review.IsClosedByNurse = true;
                    review.ClosedByNurseDate = DateTime.Now;
                    review.ClosedByNurseSignature = viewModel.CloseNursingSignature;
                }
                else if (!viewModel.CloseNursing && review.IsClosedByNurse && DetermineObjectAccess("0005", viewModel.CommunityId, AppCode))
                {
                    review.IsClosedByNurse = false;
                    review.ClosedByNurseDate = null;
                    review.ClosedByNurseSignature = null;
                }
                if (viewModel.CloseDietary && !review.IsClosedByDietitian)
                {
                    review.IsClosedByDietitian = true;
                    review.ClosedByDietitianDate = DateTime.Now;
                    review.ClosedByDietitianSignature = viewModel.CloseDietarySignature;
                }
                else if (!viewModel.CloseDietary && review.IsClosedByDietitian && DetermineObjectAccess("0004", viewModel.CommunityId, AppCode))
                {
                    review.IsClosedByDietitian = false;
                    review.ClosedByDietitianDate = null;
                    review.ClosedByDietitianSignature = null;
                }
                Context.SaveChanges();
                EditingId = null;
                return Json(new ReviewSaveResultViewModel
                {
                    Success = true,
                    IsClosed = review.IsClosed,
                    CanDelete = DetermineObjectAccess("0002", viewModel.CommunityId, AppCode),
                    CanEdiit = DetermineObjectAccess("0001", viewModel.CommunityId, AppCode),
                    CanEditDate = DetermineObjectAccess("0003", viewModel.CommunityId, AppCode),
                    CanCloseDietary = DetermineObjectAccess("0004", viewModel.CommunityId, AppCode),
                    CanCloseNursing = DetermineObjectAccess("0005", viewModel.CommunityId, AppCode),
                    ReviewDate = review.ReviewDate.Date.ToShortDateString(),
                    FormattedNursingClosedDate = review.ClosedByNurseDate.HasValue ? review.ClosedByNurseDate.Value.ToShortDateString() : string.Empty,
                    FormattedDietaryClosedDate = review.ClosedByDietitianDate.HasValue ? review.ClosedByDietitianDate.Value.ToShortDateString() : string.Empty
                });
            }
            return Edit(viewModel.ReviewId);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var review = Context.Reviews.SingleOrDefault(r => r.Id == id);
            if (review == null)
            {
                return NotFound("Could not find specified review.");
            }
            if (review.IsClosed && !DetermineObjectAccess("0002", review.CommunityId, AppCode))
            {
                return new UnauthorizedResult();
                //return new HttpUnauthorizedResult("Cannot delete closed review.");
            }
            Context.Reviews.Remove(review);
            Context.SaveChanges();
            if (EditingId.HasValue && EditingId.Value == id)
            {
                EditingId = null;
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        
        public ActionResult Sample(int id, ReviewSectionType type)
        {
            BaseReviewMeasure measure;
            IEnumerable<ReviewSampleOptionViewModel> data;
            IEnumerable<int> currentResidentIds;
            if (type == ReviewSectionType.StandardsOfCare)
            {
                var section = Context.StandardsOfCareSections.Include(e => e.Review).Include(e => e.ReviewMeasure).Include(e => e.ReviewSamples).SingleOrDefault(e => e.Id == id);
                if (section == null)
                {
                    return NotFound("Could not find review section with the specified Id.");
                }
                measure = section.ReviewMeasure;
                data = new SampleGenerator(Context).FindSet(section.Review.CommunityId, section.Review.BeginSampleDate);
                currentResidentIds = section.ReviewSamples.Select(d => d.ResidentId);
            }
            else
            {
                var section = Context.GeneralSections.Include(e => e.Review).Include(e => e.ReviewMeasure).Include(e => e.ReviewSamples).SingleOrDefault(e => e.Id == id);
                if (section == null)
                {
                    return NotFound("Could not find review section with the specified Id.");
                }
                if (!section.ReviewMeasure.RequiresPatientSample)
                {
                    throw new InvalidOperationException("Cannot select samples for general section that does not require patient samples.");
                }
                measure = section.ReviewMeasure;
                data = new SampleGenerator(Context).FindSet(section.Review.CommunityId, section.Review.BeginSampleDate);
                currentResidentIds = section.ReviewSamples.Select(d => d.ResidentId);
            }
            var viewModel = new ReviewSampleSelectionViewModel
            {
                SectionId = id,
                SectionType = type,
                Name = measure.Name,
                Code = measure.Code,
                Options = data
            };
            foreach (var option in viewModel.Options)
            {
                option.IsSelected = currentResidentIds.Contains(option.ResidentId);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Sample(ReviewSampleSelectionViewModel viewModel)
        {
            if (viewModel.SectionType == ReviewSectionType.StandardsOfCare)
            {
                var section = Context.StandardsOfCareSections.Include(e => e.ReviewMeasure.ReviewQuestions).Include(e => e.ReviewSamples).SingleOrDefault(e => e.Id == viewModel.SectionId);
                if (section == null)
                {
                    return NotFound("Could not find review section with the specified Id.");
                }
                new SampleGenerator(Context).Generate(viewModel.Options, section);
            }
            else
            {
                var section = Context.GeneralSections.Include(e => e.ReviewMeasure.ReviewQuestions).Include(e => e.ReviewSamples).SingleOrDefault(e => e.Id == viewModel.SectionId);
                if (section == null)
                {
                    return NotFound("Could not find review section with the specified Id.");
                }
                if (!section.ReviewMeasure.RequiresPatientSample)
                {
                    throw new InvalidOperationException("Cannot save samples for general section that does not require patient samples.");
                }
                new SampleGenerator(Context).Generate(viewModel.Options, section);
            }
            Context.SaveChanges();
            return RedirectToAction("Audit", new { id = viewModel.SectionId, type = viewModel.SectionType });
        }

        
        public ActionResult Audit(int id, ReviewSectionType type)
        {
            ReviewSectionAuditViewModel viewModel;
            if (type == ReviewSectionType.StandardsOfCare)
            {
                var section = Context.StandardsOfCareSections.Include(e => e.ReviewMeasure.ReviewQuestions)
                                                             .Include("ReviewSamples.ReviewAnswers")
                                                             .Include("ReviewSamples.Resident.Room.ResidentGroup")
                                                             .SingleOrDefault(e => e.Id == id);
                if (section == null)
                {
                    return NotFound("Could not find review section with the specified Id.");
                }
                viewModel = new ReviewSectionAuditViewModel
                {
                    SectionId = id,
                    SectionType = ReviewSectionType.StandardsOfCare,
                    RequiresPatientSample = true,
                    SampleAudits = section.ReviewSamples.Select(s => new ReviewSampleAuditViewModel
                    {
                        Id = s.Id,
                        Comments = s.Comments,
                        IsResidentSample = true,
                        FirstName = s.Resident.FirstName,
                        LastName = s.Resident.LastName,
                        Unit = s.Resident.Room.ResidentGroup.Name,
                        Room = s.Resident.Room.RoomName,
                        ResidentId = s.ResidentId,
                        Answers = s.ReviewAnswers.OrderBy(a => a.ReviewQuestion.SortOrder).ThenBy(a => a.ReviewQuestion.Text).Select(a => new ReviewAnswerViewModel
                        {
                            Id = a.Id,
                            Text = a.ReviewQuestion.Text,
                            IsCompliant = a.IsCompliant
                        }).ToList()
                    }).OrderBy(v => v.FullName)
                };
            }
            else
            {
                var section = Context.GeneralSections.Include(e => e.ReviewMeasure.ReviewQuestions)
                                                     .Include("ReviewSamples.ReviewAnswers")
                                                     .Include("ReviewSamples.Resident.Room.ResidentGroup")
                                                     .Include(e => e.ReviewAnswers)
                                                     .SingleOrDefault(e => e.Id == id);
                if (section == null)
                {
                    return NotFound("Could not find review section with the specified Id.");
                }
                if (section.ReviewMeasure.RequiresPatientSample)
                {
                    viewModel = new ReviewSectionAuditViewModel
                    {
                        SectionId = id,
                        SectionType = ReviewSectionType.General,
                        RequiresPatientSample = true,
                        SampleAudits = section.ReviewSamples.Select(s => new ReviewSampleAuditViewModel
                        {
                            Id = s.Id,
                            Comments = s.Comments,
                            IsResidentSample = true,
                            FirstName = s.Resident.FirstName,
                            LastName = s.Resident.LastName,
                            Unit = s.Resident.Room.ResidentGroup.Name,
                            Room = s.Resident.Room.RoomName,
                            ResidentId = s.ResidentId,
                            Answers = s.ReviewAnswers.OrderBy(a => a.ReviewQuestion.SortOrder).ThenBy(a => a.ReviewQuestion.Text).Select(a => new ReviewAnswerViewModel
                            {
                                Id = a.Id,
                                Text = a.ReviewQuestion.Text,
                                IsCompliant = a.IsCompliant
                            }).ToList()
                        }).OrderBy(v => v.FullName)
                    };
                }
                else
                {
                    viewModel = new ReviewSectionAuditViewModel
                    {
                        SectionId = id,
                        SectionType = ReviewSectionType.General,
                        RequiresPatientSample = false,
                        SampleAudits = new List<ReviewSampleAuditViewModel>
                        {
                            new ReviewSampleAuditViewModel
                            {
                                Id = section.Id,
                                Comments = section.Comments,
                                IsResidentSample = false,
                                SectionName = section.ReviewMeasure.Name,
                                Answers = section.ReviewAnswers.OrderBy(a => a.ReviewQuestion.SortOrder).ThenBy(a => a.ReviewQuestion.Text).Select(a => new ReviewAnswerViewModel
                                {
                                    Id = a.Id,
                                    Text = a.ReviewQuestion.Text,
                                    IsCompliant = a.IsCompliant
                                }).ToList()
                            }
                        }
                    };
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Audit(ReviewSectionAuditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SectionType == ReviewSectionType.StandardsOfCare)
                {
                    var section = Context.StandardsOfCareSections.Include("ReviewSamples.ReviewAnswers.ReviewQuestion")
                                                                 .SingleOrDefault(s => s.Id == viewModel.SectionId);
                    if (section == null)
                    {
                        return NotFound("Could not find specified section to update audit information for.");
                    }
                    foreach (var sampleViewModel in viewModel.SampleAudits)
                    {
                        var sample = section.ReviewSamples.Single(s => s.Id == sampleViewModel.Id);
                        sample.Comments = sampleViewModel.Comments;
                        foreach (var answerViewModel in sampleViewModel.Answers)
                        {
                            var answer = sample.ReviewAnswers.Single(a => a.Id == answerViewModel.Id);
                            answer.IsCompliant = answerViewModel.IsCompliant;
                            answer.EarnedScore = answerViewModel.IsCompliant ? answer.ReviewQuestion.MaxScore : 0;
                        }
                    }
                }
                else
                {
                    var section = Context.GeneralSections.Include("ReviewAnswers.ReviewQuestion")
                                                         .Include(s => s.ReviewMeasure)
                                                         .Include("ReviewSamples.ReviewAnswers.ReviewQuestion")
                                                         .SingleOrDefault(s => s.Id == viewModel.SectionId);
                    if (section == null)
                    {
                        return NotFound("Could not find specified section to update audit information for.");
                    }
                    if (section.ReviewMeasure.RequiresPatientSample)
                    {
                        foreach (var sampleViewModel in viewModel.SampleAudits)
                        {
                            var sample = section.ReviewSamples.Single(s => s.Id == sampleViewModel.Id);
                            sample.Comments = sampleViewModel.Comments;
                            foreach (var answerViewModel in sampleViewModel.Answers)
                            {
                                var answer = sample.ReviewAnswers.Single(a => a.Id == answerViewModel.Id);
                                answer.IsCompliant = answerViewModel.IsCompliant;
                                answer.EarnedScore = answerViewModel.IsCompliant ? answer.ReviewQuestion.MaxScore : 0;
                            }
                        }
                    }
                    else
                    {
                        var sampleViewModel = viewModel.SampleAudits.Single(); // NOTE: Only 1 "sample" if patient samples not required
                        section.Comments = sampleViewModel.Comments;
                        foreach (var answerViewModel in sampleViewModel.Answers)
                        {
                            var answer = section.ReviewAnswers.Single(a => a.Id == answerViewModel.Id);
                            answer.IsCompliant = answerViewModel.IsCompliant;
                            answer.EarnedScore = answerViewModel.IsCompliant ? answer.ReviewQuestion.MaxScore : 0;
                        }
                    }
                }
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return Audit(viewModel.SectionId, viewModel.SectionType);
        }

        
        public ActionResult AdditionalQuestions(int id)
        {
            var review = Context.Reviews.Include("AdditionalAnswers.ReviewQuestion").SingleOrDefault(r => r.Id == id);
            if (review == null)
            {
                return NotFound("Could not find reivew with the specified Id.");
            }
            var viewModel = new ReviewAdditionalAuditViewModel
            {
                ReviewId = id,
                Answers = review.AdditionalAnswers.OrderBy(a => a.ReviewQuestion.SortOrder).ThenBy(a => a.ReviewQuestion.Text).Select(a => new ReviewAnswerViewModel
                {
                    Id = a.Id,
                    Text = a.ReviewQuestion.Text,
                    IsCompliant = a.IsCompliant
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdditionalQuestions(ReviewAdditionalAuditViewModel viewModel)
        {
            var review = Context.Reviews.Include("AdditionalAnswers.ReviewQuestion")
                                        .SingleOrDefault(s => s.Id == viewModel.ReviewId);
            if (review == null)
            {
                return NotFound("Could not find specified review to update audit information for.");
            }
            foreach (var answerViewModel in viewModel.Answers)
            {
                var answer = review.AdditionalAnswers.Single(a => a.Id == answerViewModel.Id);
                answer.IsCompliant = answerViewModel.IsCompliant;
                answer.EarnedScore = answerViewModel.IsCompliant ? 0 : answer.ReviewQuestion.Score;
            }
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Lookback(string lookbackDate, string returnUrl, string CurrentCommunity)
        {
            return SaveLookbackDate(lookbackDate, returnUrl, CurrentCommunity, AppCode);
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

        private Review PersistNewReview(Community community)
        {
            var standardsOfCareMeasures = Context.StandardsOfCareMeasures.Include(e => e.ReviewQuestions).ToList();
            var generalMeasures = Context.GeneralMeasures.Include(e => e.ReviewQuestions).ToList();
            var additionalQuestions = Context.AdditionalQuestions.ToList();
            var startOfQuarter = DateTime.Today.CalculateCalendarQuarterStart();
            var newReview = new Review
            {
                CommunityId = community.CommunityId,
                Community = community,
                BeginSampleDate = startOfQuarter,
                EndSampleDate = startOfQuarter.AddDays(30),
                ReviewDate = DateTime.Today,
                AdditionalAnswers = new List<AdditionalReviewAnswer>()
            };
            newReview.StandardsOfCareSections = standardsOfCareMeasures.Select(e => new StandardsOfCareSection
            {
                ReviewMeasure = e,
                ReviewMeasureId = e.Id,
                Review = newReview,
                ReviewSamples = new List<StandardsOfCareSample>()
            }).ToList();
            newReview.GeneralSections = generalMeasures.Select(e => new GeneralSection
            {
                ReviewMeasure = e,
                ReviewMeasureId = e.Id,
                Review = newReview,
                ReviewSamples = new List<GeneralSample>()
            }).ToList();
            foreach (var section in newReview.GeneralSections)
            {
                if (!section.ReviewMeasure.RequiresPatientSample)
                {
                    section.ReviewAnswers = section.ReviewMeasure.ReviewQuestions.Select(q => new GeneralAnswer
                    {
                        ReviewQuestion = q,
                        ReviewQuestionId = q.Id,
                        ReviewSection = section,
                        ReviewSectionId = section.Id
                    }).ToList();
                }
            }
            foreach (var additionalQuestion in additionalQuestions)
            {
                newReview.AdditionalAnswers.Add(new AdditionalReviewAnswer
                {
                    Review = newReview,
                    ReviewId = newReview.Id,
                    ReviewQuestion = additionalQuestion,
                    ReviewQuestionId = additionalQuestion.Id,
                    IsCompliant = true
                });
            }
            Context.Reviews.Add(newReview);
            Context.SaveChanges();
            return newReview;
        }

        private static ReviewViewModel CreateViewModel(Review review)
        {
            var viewModel = new ReviewViewModel
            {
                ReviewId = review.Id,
                CommunityId = review.CommunityId,
                CommunityName = review.Community.CommunityShortName,
                BeginSampleDate = review.BeginSampleDate,
                EndSampleDate = review.EndSampleDate,
                ReviewDate = review.ReviewDate,
                CloseNursing = review.IsClosedByNurse,
                CloseNursingSignature = review.ClosedByNurseSignature,
                CloseDietary = review.IsClosedByDietitian,
                CloseDietarySignature = review.ClosedByDietitianSignature,
                Sections = review.StandardsOfCareSections.OrderBy(section => section.ReviewMeasure.SortOrder).ThenBy(section => section.ReviewMeasure.Name).Select(section => new ReviewSectionViewModel
                {
                    Id = section.Id,
                    Name = section.ReviewMeasure.Name,
                    SectionType = ReviewSectionType.StandardsOfCare,
                    Comments = section.Comments,
                    HasSample = section.ReviewSamples.Any(),
                    RequiresSample = true,
                    Questions = section.ReviewMeasure.ReviewQuestions.OrderBy(question => question.SortOrder).ThenBy(question => question.Text).Select(question => new ReviewScoredQuestionViewModel
                    {
                        Id = question.Id,
                        MaxPoints = question.MaxScore,
                        EarnedPoints = section.ReviewSamples.SelectMany(s => s.ReviewAnswers).Where(a => a.ReviewQuestionId == question.Id).Select(a => a.EarnedScore).DefaultIfEmpty().Min(),
                        Text = question.Text
                    }).ToList()
                }).Union(review.GeneralSections.OrderBy(section => section.ReviewMeasure.SortOrder).ThenBy(section => section.ReviewMeasure.Name).Select(section => new ReviewSectionViewModel
                {
                    Id = section.Id,
                    Name = section.ReviewMeasure.Name,
                    SectionType = ReviewSectionType.General,
                    Comments = section.Comments,
                    HasSample = section.ReviewSamples.Any(),
                    RequiresSample = section.ReviewMeasure.RequiresPatientSample,
                    Questions = section.ReviewMeasure.ReviewQuestions.OrderBy(question => question.SortOrder).ThenBy(question => question.Text).Select(question => new ReviewScoredQuestionViewModel
                    {
                        Id = question.Id,
                        MaxPoints = question.MaxScore,
                        EarnedPoints = section.ReviewMeasure.RequiresPatientSample
                                        ? section.ReviewSamples.SelectMany(s => s.ReviewAnswers).Where(a => a.ReviewQuestionId == question.Id).Select(a => a.EarnedScore).DefaultIfEmpty().Min()
                                        : section.ReviewAnswers.Where(a => a.ReviewQuestionId == question.Id).Select(a => a.EarnedScore).DefaultIfEmpty().Min(),
                        Text = question.Text
                    }).ToList()
                })).ToList(),
                PointAdjustment = review.AdditionalAnswers.Sum(a => a.EarnedScore)
            };
            return viewModel;
        }
    }
}