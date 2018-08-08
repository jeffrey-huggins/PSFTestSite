using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using AtriumWebApp.Web.Survey.Models.ViewModel;

namespace AtriumWebApp.Web.Survey.Models.Mappings
{
    public class AutoMapperInitializer
    {
        public static void Initialize()
        {
            Mapper.CreateMap<CommunitySurvey, SurveyViewModel>()
                .ForMember(dest => dest.CurrentSurvey.SurveyCycleId, opt => opt.MapFrom(source => source.SurveyCycleId))
                .ForMember(dest => dest.CurrentSurvey.SurveyId, opt => opt.MapFrom(source => source.SurveyId))
                .ForMember(dest => dest.CurrentSurvey.SurveyDocuments, opt => opt.MapFrom(source => source.SurveyDocuments))
                ;

            Mapper.CreateMap<SurveyViewModel, CommunitySurvey>()
                .ForMember(dest => dest.SurveyCycleId, opt => opt.MapFrom(source => source.CurrentSurvey.SurveyCycleId))
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(source => source.CurrentSurvey.SurveyId))
                .ForMember(dest => dest.SurveyDocuments, opt => opt.MapFrom(source => source.CurrentSurvey.SurveyDocuments))
                ;

            Mapper.CreateMap<SurveyDocument, SurveyDocumentViewModel>()
                .ForMember(dest => dest.SurveyCycleId, opt => opt.MapFrom(source => source.SurveyCycleId))
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(source => source.SurveyId))
                .ForMember(dest => dest.Document, opt => opt.Ignore())
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(source => source.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(source => source.ContentType))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                ;

            Mapper.CreateMap<SurveyDocumentViewModel, SurveyDocument>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SurveyCycleId, opt => opt.MapFrom(source => source.SurveyCycleId))
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(source => source.SurveyId))
                .ForMember(dest => dest.Document, opt => opt.Ignore())
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(source => source.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(source => source.ContentType))
                ;

            Mapper.AssertConfigurationIsValid();
        }
    }
}