using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using System.IO;
using System.Data.Entity;
using AtriumWebApp.Web.Base.Library;

namespace AtriumWebApp.Web.Survey.Models.Mappings
{
    public class SurveyMapper
    {
        private SurveyContext Context;

        public SurveyMapper(SurveyContext context)
        {
            Context = context;
        }

        public virtual void MapSurveyDocuments(SurveyViewModel fromViewModel, CommunitySurvey toModel)
        {
            if (toModel == null)
                toModel = Mapper.Map<SurveyViewModel, CommunitySurvey>(fromViewModel);

            MapSurveyDocumentDeletions(fromViewModel, toModel);

            if (fromViewModel != null &&
                fromViewModel.CurrentSurvey != null &&
                fromViewModel.Documents != null)
                MapSurveyDocumentCreationOrUpdate(fromViewModel);
        }

        public virtual void MapSurveyDocuments(IList<SurveyDocumentViewModel> fromViewModel, IList<SurveyDocument> toModel)
        {
            SurveyViewModel vmContainer = new SurveyViewModel()
            {
                Documents = fromViewModel
            };
            CommunitySurvey modelContainer = new CommunitySurvey()
            {
                SurveyDocuments = toModel
            };
            MapSurveyDocuments(vmContainer, modelContainer);
        }

        private void MapSurveyDocumentDeletions(SurveyViewModel fromViewModel, CommunitySurvey toModel)
        {
            var existsInModelButNotInViewModel = false;

            if (toModel.SurveyDocuments == null || toModel.SurveyDocuments.Count == 0)
                return;

            foreach (var documentModel in toModel.SurveyDocuments.ToList())
            {
                existsInModelButNotInViewModel = (fromViewModel.Documents == null) ? true :
                        fromViewModel.Documents.Where(x => x.Id == documentModel.Id).SingleOrDefault() == null;

                if (existsInModelButNotInViewModel)
                {
                    Context.SurveyDocuments.Attach(documentModel);
                    Context.Entry(documentModel).State = EntityState.Deleted;
                }
            }
        }

        private void MapSurveyDocumentCreationOrUpdate(SurveyViewModel fromViewModel)
        {
            foreach (var document in fromViewModel.Documents)
            {
                bool isNewDocument = document.Id <= 0;
                SurveyDocument existingDocumentModel = null;

                if (!isNewDocument)
                    existingDocumentModel = Context.SurveyDocuments.Where(x => x.Id == document.Id).SingleOrDefault();

                bool modelDoesNotExist = isNewDocument || existingDocumentModel == null;

                var result = modelDoesNotExist ?
                    MapSurveyDocumentCreate(fromViewModel, document) :
                    MapSurveyDocumentUpdate(existingDocumentModel, document);
            }
        }

        private bool MapSurveyDocumentCreate(SurveyViewModel viewModel, SurveyDocumentViewModel document)
        {
            if (document.Document == null ||
                !(document.Document.Length > 0))
                return false;

            byte[] imgData;
            using (var reader = new BinaryReader(document.Document.OpenReadStream()))
                imgData = reader.ReadBytes((int)document.Document.Length);

            // NOTE: Compression as 7z file happens here.
            imgData = SevenZipHelper.CompressStreamLZMA(imgData).ToArray();
            //imgData = SevenZipHelper.CompressBytes(imgData).ToArray();

            SurveyDocument documentModel = MapSurveyDocumentViewModel(document, imgData,
                viewModel.CurrentSurvey.SurveyCycleId, viewModel.CurrentSurvey.SurveyId);

            // Mapping issue, b/c Document != same as byte[]
            //var documentModel = Mapper.Map<SurveyDocumentViewModel, SurveyDocument>(document, new SurveyDocument());

            Context.SurveyDocuments.Add(documentModel);

            return true;
        }

        private bool MapSurveyDocumentUpdate(SurveyDocument existingDocumentModel, SurveyDocumentViewModel changedDocumentVM)
        {
            //Mapper.Map<SurveyDocumentViewModel, SurveyDocument>(changedDocumentViewModel, existingDocumentModel);

            if (!existingDocumentModel.ContentType.Equals(changedDocumentVM.ContentType) ||
                !existingDocumentModel.FileName.Equals(changedDocumentVM.FileName) ||
                existingDocumentModel.SurveyId != changedDocumentVM.SurveyId ||
                !existingDocumentModel.SurveyCycleId.Equals(changedDocumentVM.SurveyCycleId)
                )
            {
                if(changedDocumentVM.Document != null)
                    using (var reader = new BinaryReader(changedDocumentVM.Document.OpenReadStream())) {
                        existingDocumentModel.Document =
                            reader.ReadBytes((int)changedDocumentVM.Document.Length);
                    }
                existingDocumentModel.Id = changedDocumentVM.Id;
                existingDocumentModel.ContentType = changedDocumentVM.ContentType;
                existingDocumentModel.FileName = Path.GetFileName(changedDocumentVM.FileName);
                existingDocumentModel.SurveyCycleId = changedDocumentVM.SurveyCycleId;
                existingDocumentModel.SurveyId = changedDocumentVM.SurveyId;
                //SurveyDocument sd = MapSurveyDocumentViewModel(changedDocumentVM, imgData, null, null); 

                Context.Entry(existingDocumentModel).State = EntityState.Modified;
            }
            return true;
        }


        public SurveyDocumentViewModel MapSurveyDocument(SurveyDocument sd)
        {
            return new SurveyDocumentViewModel()
            {
                ContentType = sd.ContentType,
                //Document = NOT mapped
                FileName = sd.FileName,
                Id = sd.Id,
                SurveyCycleId = sd.SurveyCycleId,
                SurveyId = sd.SurveyId
            };
        }

        public SurveyDocument MapSurveyDocumentViewModel(SurveyDocumentViewModel sdvm, byte[] imgData, string surveyCycleId, int? surveyId)
        {
            return new SurveyDocument()
            {
                Document = imgData,
                ContentType = sdvm.Document.ContentType,
                FileName = Path.GetFileName(sdvm.Document.FileName),
                //Id = document.Id,
                SurveyCycleId = surveyCycleId ?? sdvm.SurveyCycleId,
                SurveyId = surveyId ?? sdvm.SurveyId
            };
        }

        public virtual CommunitySurvey GetSurveyDocumentsByIds(int surveyId, string surveyCycleId)
        {
            var survey = Context.CommunitySurveys
                //.Include(con => con.SurveyDocuments)
                //.Include(con => con.Provider)
                //.Include(con => con.ContractCommunities)
                //.Include(con => con.Category)
                //.Include(con => con.SubCategory)
                            .Where(con => con.SurveyId == surveyId &&
                                con.SurveyCycleId.Equals(surveyCycleId, StringComparison.OrdinalIgnoreCase))
                            .SingleOrDefault();
            if (survey == null)
                return survey;
            Context.Entry(survey).Collection(con => con.SurveyDocuments).Load();
            return survey;
        }

        public virtual CommunitySurvey GetFilteredDocumentsByIds(int surveyId, string surveyCycleId)
        {
            var survey = Context.CommunitySurveys
                //.Include(con => con.Provider)
                //.Include(con => con.ContractCommunities)
                //.Include(con => con.Category)
                //.Include(con => con.SubCategory)
                            .Where(con => con.SurveyId == surveyId &&
                                con.SurveyCycleId.Equals(surveyCycleId, StringComparison.OrdinalIgnoreCase))
                            .SingleOrDefault();

            if (survey == null)
                return survey;

            Context.Entry(survey)
            .Collection(con => con.SurveyDocuments).Load();
            //.Query();
            //.Where(doc => !doc.ArchiveFlg)
            //.Load();

            return survey;
        }
    }
}