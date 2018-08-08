using System;
using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Web.Survey.Models.ViewModel;

namespace AtriumWebApp.Web.Survey.Models
{
    public class SampleGenerator
    {
        private QuarterlySystemsReviewContext Context { get; set; }

        public SampleGenerator(QuarterlySystemsReviewContext context)
        {
            Context = context;
        }

        public IEnumerable<ReviewSampleOptionViewModel> FindSet(int communityId, DateTime minimumMostRecentCensusDate)
        {
            var query = Context.Residents.Where(r => r.CommunityId == communityId && r.LastCensusDate >= minimumMostRecentCensusDate).Select(r => new ReviewSampleOptionViewModel
            {
                ResidentId = r.PatientId,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Unit = r.Room.ResidentGroup.Name,
                Room = r.Room.RoomName
            });
            return query.ToList();
        }

        public void Generate(IEnumerable<ReviewSampleOptionViewModel> samples, StandardsOfCareSection section)
        {
            var currentSamplePatientIds = section.ReviewSamples.Select(s => s.ResidentId);
            foreach (var sample in samples)
            {
                if (sample.IsSelected)
                {
                    if (!currentSamplePatientIds.Contains(sample.ResidentId))
                    {
                        CreateSectionSample(section, sample);
                    }
                }
                else if (currentSamplePatientIds.Contains(sample.ResidentId))
                {
                    RemoveSectionSample(section, sample);
                }
            }
        }

        public void Generate(IEnumerable<ReviewSampleOptionViewModel> samples, GeneralSection section)
        {
            var currentSamplePatientIds = section.ReviewSamples.Select(s => s.ResidentId);
            foreach (var sample in samples)
            {
                if (sample.IsSelected)
                {
                    if (!currentSamplePatientIds.Contains(sample.ResidentId))
                    {
                        CreateSectionSample(section, sample);
                    }
                }
                else if (currentSamplePatientIds.Contains(sample.ResidentId))
                {
                    RemoveSectionSample(section, sample);
                }
            }
        }

        private void CreateSectionSample(StandardsOfCareSection section, ReviewSampleOptionViewModel sample)
        {
            var reviewSample = new StandardsOfCareSample
            {
                ReviewSection = section,
                ReviewSectionId = section.Id,
                ResidentId = sample.ResidentId
            };
            Context.StandardsOfCareSamples.Add(reviewSample);
            foreach (var question in section.ReviewMeasure.ReviewQuestions)
            {
                Context.StandardsOfCareAnswers.Add(new StandardsOfCareAnswer
                {
                    ReviewQuestion = question,
                    ReviewQuestionId = question.Id,
                    ReviewSample = reviewSample
                });
            }
        }

        private void CreateSectionSample(GeneralSection section, ReviewSampleOptionViewModel sample)
        {
            var reviewSample = new GeneralSample
            {
                ReviewSection = section,
                ReviewSectionId = section.Id,
                ResidentId = sample.ResidentId
            };
            Context.GeneralSamples.Add(reviewSample);
            foreach (var question in section.ReviewMeasure.ReviewQuestions)
            {
                Context.GeneralPatientAnswers.Add(new GeneralPatientAnswer
                {
                    ReviewQuestion = question,
                    ReviewQuestionId = question.Id,
                    ReviewSample = reviewSample
                });
            }
        }

        private void RemoveSectionSample(StandardsOfCareSection section, ReviewSampleOptionViewModel sample)
        {
            var toRemove = section.ReviewSamples.Single(s => s.ResidentId == sample.ResidentId);
            Context.StandardsOfCareSamples.Remove(toRemove);
        }

        private void RemoveSectionSample(GeneralSection section, ReviewSampleOptionViewModel sample)
        {
            var toRemove = section.ReviewSamples.Single(s => s.ResidentId == sample.ResidentId);
            Context.GeneralSamples.Remove(toRemove);
        }
    }
}