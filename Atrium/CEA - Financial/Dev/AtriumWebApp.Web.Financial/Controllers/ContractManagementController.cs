using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.Mappings;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "CON")]
    public class ContractManagementController : BaseController
    {
        #region Properties and Constructors
        private const string AppCode = "CON";
        private IDictionary<string, bool> _adminAccess;
        private bool IsAdministrator
        {
            get
            {
                if (_adminAccess == null)
                {
                    _adminAccess = DetermineAdminAccess(PrincipalContext, UserPrincipal);
                }
                bool isAdministrator;
                if (_adminAccess.TryGetValue(AppCode, out isAdministrator))
                {

                    return isAdministrator;
                }
                return false;
            }
        }
        public List<int> SelectedCommunityIds
        {
            get
            {
                List<int> commIds;

                if (!Session.Contains(AppCode + "SelectedCommunityIds"))
                    Session.SetItem(AppCode + "SelectedCommunityIds", new List<int>());
                Session.TryGetObject(AppCode + "SelectedCommunityIds", out commIds);
                return commIds;
            }
            private set
            {
                if (value == null)
                {
                    Session.Remove(AppCode + "SelectedCommunityIds");
                }
                else
                {
                    Session.SetItem(AppCode + "SelectedCommunityIds", value);
                }
            }
        }

        public int CurrentCommunityId
        {
            get { return CurrentFacility[AppCode]; }
            set { CurrentFacility[AppCode] = value; }
        }

        public List<Community> Communities { get { return (List<Community>)FacilityList[AppCode]; } }

        protected new ContractManagementContext Context
        {
            get { return (ContractManagementContext)base.Context; }
        }

        private ContractManagementMapper ModelMapper { get; set; }

        #endregion
        public ContractManagementController(IOptions<AppSettingsConfig> config, ContractManagementContext context) : base(config, context)
        {
            ModelMapper = new ContractManagementMapper(context);
        }


        #region Actions

        public ActionResult Index()
        {
            return RedirectToAction("Contracts");
        }

        public ActionResult Contracts(ContractViewModel currentContract)
        {
            LogSession(AppCode);

            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);

            ContractListViewModel listOfContracts = GetContractsByCommunities();

            if (currentContract != null && currentContract.Id > 0)
                listOfContracts.AndSetCurrentContract(currentContract);

            return View(listOfContracts);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult CreateContract()
        {
            var viewModel = new ContractViewModel
            {
                ArchiveFlg = false,
                ContractProviderName = String.Empty
            };

            viewModel = ModelMapper.MapDropDownListItemsFor(viewModel);
            viewModel = ModelMapper.MapChildDropDownListItemsFor(viewModel, Communities);

            return PartialView("EditContract", viewModel);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult EditContract(int id)
        {
            var model = GetContractById(id);

            ContractViewModel viewModel = null;

            viewModel = ModelMapper.MapContractViewModelFrom(model, viewModel);

            viewModel = ModelMapper.MapDropDownListItemsFor(viewModel);
            viewModel = ModelMapper.MapChildDropDownListItemsFor(viewModel, Communities);

            return PartialView("EditContract", viewModel);
        }

        private void IsViewModelValid(ContractViewModel fromViewModel)
        {
            if (fromViewModel.ContractCommunities == null || fromViewModel.ContractCommunities.Count == 0)
            {
                ModelState.AddModelError("SelectedCommunity", "At least one community must be selected");
                return;
            }

            foreach (var contractCommunity in fromViewModel.ContractCommunities)
            {
                if (contractCommunity.ContractPaymentTermId < 1 || contractCommunity.ContractRenewalId < 1 || contractCommunity.ContractTerminationNoticeId < 0)
                {
                    ModelState.AddModelError("ContractCommunity Value error", "At least one value must be selected for Payment Term, Renewal and Termination Notice");
                    return;
                }
            }

        }

        [HttpPost]
        public ActionResult EditContract(ContractViewModel fromViewModel)
        {
            fromViewModel = ModelMapper.MapDropDownListItemsFor(fromViewModel);
            fromViewModel = ModelMapper.MapChildDropDownListItemsFor(fromViewModel, Communities);

            IsViewModelValid(fromViewModel);

            if (!ModelState.IsValid)
            {
                return PartialView("EditContract", fromViewModel);
            }

            Contract existingContract = GetContractById(fromViewModel.Id);

            bool contractExists = existingContract != null;

            SetArchiveDates(fromViewModel, existingContract);

            Contract contract = Mapper.Map<ContractViewModel, Contract>(fromViewModel, existingContract);

            ModelMapper.MapContractDocuments(fromViewModel, contract);
            ModelMapper.MapContractCommunities(fromViewModel, contract);

            AddContractToContext(contract, contractExists);

            Context.SaveChanges();

            return Content("{ \"Success\" : true }");
        }

        private void AddContractToContext(Contract contract, bool isExistingContract)
        {
            if (isExistingContract)
            {
                Context.Entry(contract).State = EntityState.Modified;
            }
            else
            {
                Context.Contracts.Add(contract);
            }
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult ViewContract(int id)
        {
            var model = GetContractById(id);

            ContractViewModel viewModel = null;
            viewModel = ModelMapper.MapContractViewModelFrom(model, viewModel);

            viewModel = ModelMapper.MapDropDownListItemsFor(viewModel);
            viewModel = ModelMapper.MapChildDropDownListItemsFor(viewModel, Communities);

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteContract(int id)
        {
            var model = GetContractById(id);

            Context.Contracts.Remove(model);
            Context.SaveChanges();

            return Json(new SaveResultViewModel
            {
                Success = true
            });
        }

        public ActionResult StreamContractDocument(int id)
        {
            var documentRequested = Context.ContractDocuments.Where(x => x.Id == id).SingleOrDefault();

            if (documentRequested == null)
                ModelState.AddModelError("DocumentMissing", "Document could not be found - THIS IS A TEST MESSAGE");

            byte[] theFile = documentRequested.Document;

            return File(theFile, documentRequested.ContentType, documentRequested.FileName);
        }

        [HttpPost]
        public ActionResult UpdateCurrentCommunity(List<int> communitySelections, string returnUrl)
        {
            SelectedCommunityIds = communitySelections;

            return Redirect(returnUrl);
        }

        public ActionResult Provider(int id)
        {
            ContractProvider provider = Context.Providers.SingleOrDefault(p => p.Id == id);

            return View(Mapper.Map<ProviderViewModel>(provider));
        }

        [HttpPost]
        public ActionResult Provider(ProviderViewModel viewModel)
        {
            ContractProvider provider = Context.Providers.SingleOrDefault(p => p.Id == viewModel.Id);
            Mapper.Map<ProviderViewModel, ContractProvider>(viewModel, provider);
            Context.SaveChanges();
            return null;
        }

        [HttpPost]
        public ActionResult GetSubCategories(int selectedCategoryId)
        {
            var subcategories = (from sub in Context.ContractSubCategories
                                 where sub.ContractCategoryId == selectedCategoryId
                                 select new
                                 {
                                     Id = sub.Id,
                                     Name = sub.Name
                                 }).ToArray();

            return Json(subcategories);
        }

        [HttpPost]
        public ActionResult IsProvider(int selectedCategoryId)
        {
            return Json(Context.ContractCategories.Find(selectedCategoryId).IsProvider);
        }

        [ResponseCache(Duration = 600)]
        public PartialViewResult CreateContractDocument()
        {
            return PartialView(new ContractDocumentViewModel());
        }

        [ResponseCache(Duration = 600)]
        public PartialViewResult CreateContractCommunity()
        {
            return PartialView(new ContractCommunityViewModel()
            {
                Communities = Communities,
                PaymentTerms = ModelMapper.GetPaymentTerms(),
                Renewals = ModelMapper.GetRenewals(),
                TerminationNotices = ModelMapper.GetTerminationNotices()
            });
        }

        #endregion

        private void SetArchiveDates(ContractViewModel viewModel, Contract contract)
        {
            SetContractArchiveDate(viewModel, contract);

            if (viewModel.Documents != null && viewModel.Documents.Count > 0)
                SetContractDocumentArchiveDates(viewModel);
        }

        private void SetContractDocumentArchiveDates(ContractViewModel viewModel)
        {
            foreach (var document in viewModel.Documents)
            {
                var existingDocument = Context.ContractDocuments.Where(x => x.Id == document.Id).SingleOrDefault();
                bool documentExists = existingDocument != null;

                if (!documentExists)
                {
                    if (document.ArchiveFlg)
                        document.ArchivedDate = DateTime.Now;
                    continue;
                }

                bool existingHasDate = existingDocument.ArchivedDate.HasValue;
                if (document.ArchiveFlg && !existingHasDate)
                {
                    document.ArchivedDate = DateTime.Now;
                }

                if (!document.ArchiveFlg && existingHasDate)
                    document.ArchivedDate = null;

            }


        }

        private void SetContractArchiveDate(ContractViewModel viewModel, Contract contract)
        {
            bool contractExists = contract != null;
            if (!contractExists)
            {
                if (viewModel.ArchiveFlg)
                    viewModel.ArchivedDate = DateTime.Now;

                return;
            }

            bool existingHasDate = contract.ArchivedDate.HasValue;
            if (viewModel.ArchiveFlg && !existingHasDate)
            {
                viewModel.ArchivedDate = DateTime.Now;
            }

            if (!viewModel.ArchiveFlg && existingHasDate)
                viewModel.ArchivedDate = null;
        }

        private Contract GetContractById(int id)
        {
            if (DetermineObjectAccess("0004", null, AppCode))
                return ModelMapper.GetContractById(id);

            return ModelMapper.GetFilteredContractById(id);
        }

        private List<SelectListItem> GetCommunityListItems()
        {
            return Communities.Select(x => new SelectListItem() { Value = x.CommunityId.ToString(), Text = x.CommunityShortName }).ToList<SelectListItem>();
        }

        private List<int> ToListOfIds(IEnumerable<Community> communities)
        {
            return communities.Select(x => x.CommunityId).ToList<int>();
        }

        private ContractListViewModel GetContractsByCommunities()
        {
            bool objectAccess = DetermineObjectAccess("0004", null, AppCode);
            List<ContractViewModel> contracts =
                ModelMapper.GetAllContractViewsByCommunities(!objectAccess, SelectedCommunityIds);

            List<SelectListItem> communityItems = GetCommunityListItems();

            // Used by .. Mapper.CreateMap<Contract, ContractViewModel>()
            //var activeProviders = ModelMapper.GetAllActiveProviders();

            var viewModel = new ContractListViewModel()
            {
                Communities = communityItems,
                SelectedCommunities = SelectedCommunityIds,
                Contracts = contracts,
                CurrentContract = null,
                CanCreateNew = DetermineObjectAccess("0001", null, AppCode),
                CanEdit = DetermineObjectAccess("0002", null, AppCode),
                CanManageProviders = DetermineObjectAccess("0003", null, AppCode),
                CanDelete = DetermineObjectAccess("0005", null, AppCode)
            };
            return viewModel;
        }
    }
}