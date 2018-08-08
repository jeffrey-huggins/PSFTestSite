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
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "CON")]
    public class ContractProviderController : BaseController
    {
        public ContractProviderController(IOptions<AppSettingsConfig> config, ContractManagementContext context) : base(config, context)
        {
            ModelMapper = new ContractManagementMapper(context);
        }

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

        protected new ContractManagementContext Context
        {
            get { return (ContractManagementContext)base.Context; }
        }
        //null?
        private ContractManagementMapper ModelMapper { get; set; }

        #endregion

        #region Actions

        public ActionResult Providers(ProviderListViewModel viewModel)
        {
            LogSession(AppCode);

            ProviderListViewModel result;

            if (viewModel.Providers == null || viewModel.Providers.Count == 0)
                result = GetInitialListOfProviders();
            else
                result = viewModel;

            return View(result);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration =600)]
        public ActionResult CreateProvider()
        {
            var viewModel = new ProviderViewModel
            {
                Name = String.Empty,
                InsuranceCompany = String.Empty,
                InsurancePolicyId = String.Empty,
                LicenseId = String.Empty
            };

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult CreateProvider(ProviderViewModel fromViewModel)
        {
            if (ModelState.IsValid)
            {
                ContractProvider provider = Mapper.Map<ProviderViewModel, ContractProvider>(fromViewModel);

                ModelMapper.MapAddresses(fromViewModel, provider);
                ModelMapper.MapContacts(fromViewModel, provider);
                ModelMapper.MapDocuments(fromViewModel, provider);

                provider.IsActive = true;

                Context.Providers.Add(provider);
                Context.SaveChanges();

                var model = GetContractProviderById(provider.Id);

                var remappedViewModel = ModelMapper.MapProviderViewModelFrom(model);

                return Json(new { Success = true });
            }
            fromViewModel = ModelMapper.MapDropDownListsItemsFor(fromViewModel);
            return PartialView(fromViewModel);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult EditProvider(int id)
        {
            var model = GetContractProviderById(id);

            ProviderViewModel viewModel = ModelMapper.MapProviderViewModelFrom(model);

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult EditProvider(ProviderViewModel fromViewModel)
        {
            var existingModel = GetContractProviderById(fromViewModel.Id);

            if (ModelState.IsValid)
            {
                var mappedProvider = Mapper.Map<ProviderViewModel, ContractProvider>(fromViewModel, existingModel);

                ModelMapper.MapAddresses(fromViewModel, existingModel);
                ModelMapper.MapContacts(fromViewModel, existingModel);
                ModelMapper.MapDocuments(fromViewModel, existingModel);

                Context.Entry(mappedProvider).State = EntityState.Modified;
                Context.SaveChanges();

                return Json(new { Success = true });
            }

            fromViewModel = ModelMapper.MapDropDownListsItemsFor(fromViewModel);
            return PartialView(fromViewModel);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult ViewProvider(int id)
        {
            var model = GetContractProviderById(id);

            ProviderViewModel viewModel = ModelMapper.MapProviderViewModelFrom(model);

            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteProvider(int id)
        {
            var model = Context.Providers
                            .Include(pro => pro.Addresses)
                            .Include(pro => pro.Contacts)
                            .Include(pro => pro.Documents)
                            .Where(pro => pro.Id == id).SingleOrDefault();

            var providerHasExistingContracts = Context.Contracts.Include(x => x.Provider).Where(x => x.Provider.Id == id).ToList().Count > 0;
            var modelExists = model != null;

            if (!modelExists)
            {
                return Json(new ProviderActionResultViewModel
                {
                    Success = false,
                    ErrorMessage = "Could not find provider/vendor with the specified Id."
                });
            }

            if (providerHasExistingContracts)
            {
                return Json(new ProviderActionResultViewModel
                {
                    Success = false,
                    ErrorMessage = "Provider/Vendor has existing contracts and can not be deleted until they are removed"
                });
            }

            Context.Providers.Remove(model);
            Context.SaveChanges();

            return Json(new ProviderActionResultViewModel
            {
                Success = true
            });
        }

        public ActionResult StreamDocument(int id)
        {
            var document = Context.Documents.Where(x => x.Id == id).SingleOrDefault();

            byte[] theFile = document.Document;

            return File(theFile, document.ContentType, document.FileName);
        }

        [ResponseCache(Duration = 600)]
        public PartialViewResult CreateDocument(ProviderViewModel parentModel)
        {
            return PartialView(new ProviderDocumentViewModel() { SavedDate = DateTime.Now });
        }

        [ResponseCache(Duration = 600)]
        public PartialViewResult CreateAddress(ProviderViewModel parentModel)
        {
            return PartialView(new ProviderAddressViewModel()
            {
                States = ModelMapper.GetStates(),
                AddressTypes = ModelMapper.GetAddressTypes()
            });
        }

        [ResponseCache(Duration = 600)]
        public PartialViewResult CreateContact(ProviderViewModel parentModel)
        {
            return PartialView(new ProviderContactViewModel()
            {
                ProviderContactTypes = ModelMapper.GetContactTypes()
            });
        }

        #endregion

        #region Utilities

        private ProviderListViewModel GetInitialListOfProviders()
        {
            var activeProviders = ModelMapper.GetAllActiveProviders();

            var viewModel = new ProviderListViewModel()
            {
                Providers = Mapper.Map<List<ContractProvider>, List<ProviderViewModel>>(activeProviders),
                CanManage = DetermineObjectAccess("0003", null, AppCode)
            };

            return viewModel;
        }

        private ContractProvider GetContractProviderById(int id)
        {
            return ModelMapper.GetProviderById(id);
        }


        #endregion
    }
}