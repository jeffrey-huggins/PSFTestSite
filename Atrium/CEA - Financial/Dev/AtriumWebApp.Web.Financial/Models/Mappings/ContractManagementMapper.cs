using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AtriumWebApp.Models.ViewModel;
using AutoMapper;
using System.Data.Entity;

namespace AtriumWebApp.Models.Mappings
{
    public class ContractManagementMapper
    {
        private ContractManagementContext Context;

        public ContractManagementMapper(ContractManagementContext context)
        {
            Context = context;
        }

        public virtual ProviderViewModel MapDropDownListsItemsFor(ProviderViewModel viewModel)
        {
            var states = GetStates();
            var addressTypes = GetAddressTypes();
            var contactTypes = GetContactTypes();

            foreach (var address in viewModel.Addresses)
            {
                address.States = states;
                address.AddressTypes = addressTypes;
            }

            foreach (var contact in viewModel.Contacts)
            {
                contact.ProviderContactTypes = contactTypes;
            }

            return viewModel;
        }

        public virtual ContractViewModel MapDropDownListItemsFor(ContractViewModel viewModel)
        {
            viewModel.Providers = GetProviders();
            viewModel.Categories = GetCategories();
            viewModel.SubCategories = GetSubCategories(viewModel.ContractCategoryId);

            return viewModel;
        }

        public virtual ContractViewModel MapChildDropDownListItemsFor(ContractViewModel viewModel, List<Community> communities)
        {
            if (viewModel.ContractCommunities == null)
                viewModel.ContractCommunities = new List<ContractCommunityViewModel>();

            var paymentTerms = GetPaymentTerms();
            var renewals = GetRenewals();
            var notices = GetTerminationNotices();

            foreach (ContractCommunityViewModel contractCommunity in viewModel.ContractCommunities)
            {
                contractCommunity.Communities = communities;
                contractCommunity.PaymentTerms = paymentTerms;
                contractCommunity.Renewals = renewals;
                contractCommunity.TerminationNotices = notices;
            }

            viewModel.PaymentTerms = paymentTerms;
            viewModel.Renewals = renewals;
            viewModel.TerminationNotices = notices;
            if (viewModel.Providers != null && viewModel.Providers.Count > 0)
                viewModel.Providers = viewModel.Providers.OrderBy(p => p.Name).ToList();

            return viewModel;
        }

        public ICollection<ContractTerminationNotice> GetTerminationNotices()
        {
            var list = Context.ContractTerminationNotices.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();

            list.Insert(0, new ContractTerminationNotice() { Id = -1 });

            return list;
        }

        public ICollection<ContractRenewal> GetRenewals()
        {
            var list = Context.ContractRenewals.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();

            list.Insert(0, new ContractRenewal() { Id = -1 });

            return list;
        }

        public ICollection<ContractPaymentTerm> GetPaymentTerms()
        {
            var list = Context.ContractPaymentTerms.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();

            list.Insert(0, new ContractPaymentTerm() { Id = -1 });

            return list;
        }

        public virtual ProviderViewModel MapProviderViewModelFrom(ContractProvider model)
        {
            ProviderViewModel viewModel = Mapper.Map<ContractProvider, ProviderViewModel>(model);

            viewModel = MapDropDownListsItemsFor(viewModel);

            return viewModel;
        }

        public virtual ContractViewModel MapContractViewModelFrom(Contract model, ContractViewModel viewModel)
        {
            viewModel = Mapper.Map<Contract, ContractViewModel>(model, viewModel);

            viewModel = MapDropDownListItemsFor(viewModel);

            return viewModel;
        }

        public virtual void MapDocuments(ProviderViewModel fromViewModel, ContractProvider toModel)
        {
            if (toModel == null)
                toModel = Mapper.Map<ProviderViewModel, ContractProvider>(fromViewModel);

            MapDocumentDeletions(fromViewModel, toModel);

            MapDocumentCreationOrUpdate(fromViewModel);
        }

        public virtual void MapDocumentDeletions(ProviderViewModel fromViewModel, ContractProvider toModel)
        {
            if (toModel.Documents == null)
                return;

            foreach (var documentModel in toModel.Documents.ToList())
            {
                var existsInModelButNotInViewModel = fromViewModel.Documents.Where(x => x.Id == documentModel.Id).SingleOrDefault() == null;

                if (existsInModelButNotInViewModel)
                {
                    Context.Documents.Attach(documentModel);
                    Context.Entry(documentModel).State = EntityState.Deleted;
                }
            }
        }

        public virtual void MapDocumentCreationOrUpdate(ProviderViewModel fromViewModel)
        {
            foreach (var document in fromViewModel.Documents)
            {
                ProviderDocument existingDocumentModel = Context.Documents.Where(x => x.Id == document.Id).SingleOrDefault();

                bool modelDoesNotExist = document.Id <= 0 || existingDocumentModel == null;

                var result = modelDoesNotExist ?
                    MapDocumentCreate(fromViewModel, document) :
                    MapDocumentUpdate(existingDocumentModel, document);
            }
        }

        public virtual bool MapDocumentCreate(ProviderViewModel viewModel, ProviderDocumentViewModel document)
        {
            var documentModel = Mapper.Map<ProviderDocumentViewModel, ProviderDocument>(document);
            documentModel.ContractProviderId = viewModel.Id;

            if (document.Document == null || !(document.Document.Length > 0))
            {
                return false;
            }
            byte[] docData;
            using (var reader = new BinaryReader(document.Document.OpenReadStream()))
            {
                docData = reader.ReadBytes((int)document.Document.Length);
            }
            documentModel.Document = docData;
            documentModel.FileName = Path.GetFileName(document.Document.FileName);
            documentModel.ContentType = document.Document.ContentType;

            Context.Documents.Add(documentModel);

            return true;
        }

        public virtual bool MapDocumentUpdate(ProviderDocument existingDocumentModel, ProviderDocumentViewModel changedDocumentViewModel)
        {
            Context.Documents.Attach(existingDocumentModel);
            Mapper.Map<ProviderDocumentViewModel, ProviderDocument>(changedDocumentViewModel, existingDocumentModel);
            Context.Entry(existingDocumentModel).State = EntityState.Modified;

            return true;
        }

        public virtual void MapContacts(ProviderViewModel fromViewModel, ContractProvider toModel)
        {
            if (toModel == null)
                toModel = Mapper.Map<ProviderViewModel, ContractProvider>(fromViewModel);

            MapContactDeletions(fromViewModel, toModel);

            MapContactCreationOrUpdate(fromViewModel);
        }

        public virtual void MapContactDeletions(ProviderViewModel fromViewModel, ContractProvider toModel)
        {
            if (toModel.Contacts == null)
                return;

            foreach (var contactModel in toModel.Contacts.ToList())
            {
                var existsInModelButNotInViewModel = fromViewModel.Contacts.Where(x => x.Id == contactModel.Id).SingleOrDefault() == null;

                if (existsInModelButNotInViewModel)
                {
                    Context.Contacts.Attach(contactModel);
                    Context.Entry(contactModel).State = EntityState.Deleted;
                }
            }
        }

        public virtual void MapContactCreationOrUpdate(ProviderViewModel fromViewModel)
        {
            foreach (var contact in fromViewModel.Contacts)
            {
                ProviderContact existingContactModel = Context.Contacts.Where(x => x.Id == contact.Id).SingleOrDefault();

                bool modelDoesNotExist = contact.Id <= 0 || existingContactModel == null;

                var result = modelDoesNotExist ?
                    MapContactCreate(fromViewModel, contact) :
                    MapContactUpdate(existingContactModel, contact);
            }
        }

        public virtual bool MapContactCreate(ProviderViewModel viewModel, ProviderContactViewModel contact)
        {
            var contactModel = Mapper.Map<ProviderContactViewModel, ProviderContact>(contact);
            contactModel.ContractProviderId = viewModel.Id;
            Context.Contacts.Add(contactModel);

            return true;
        }

        public virtual bool MapContactUpdate(ProviderContact existingContactModel, ProviderContactViewModel changedContactViewModel)
        {
            Context.Contacts.Attach(existingContactModel);
            Mapper.Map<ProviderContactViewModel, ProviderContact>(changedContactViewModel, existingContactModel);
            Context.Entry(existingContactModel).State = EntityState.Modified;

            return true;
        }

        public virtual void MapAddresses(ProviderViewModel fromViewModel, ContractProvider toModel)
        {
            if (toModel == null)
                toModel = Mapper.Map<ProviderViewModel, ContractProvider>(fromViewModel);

            MapAddressDeletions(fromViewModel, toModel);

            MapAddressCreationOrUpdate(fromViewModel);
        }

        public virtual void MapAddressCreationOrUpdate(ProviderViewModel fromViewModel)
        {
            foreach (var address in fromViewModel.Addresses)
            {
                ProviderAddress existingAddressModel = Context.Addresses.Where(x => x.Id == address.Id).SingleOrDefault();
                bool modelDoesNotExist = address.Id <= 0 || existingAddressModel == null;

                var result = modelDoesNotExist ?
                    MapAddressCreate(fromViewModel, address) :
                    MapAddressUpdate(existingAddressModel, address);
            }
        }

        public virtual bool MapAddressCreate(ProviderViewModel viewModel, ProviderAddressViewModel address)
        {
            var addressModel = Mapper.Map<ProviderAddressViewModel, ProviderAddress>(address);
            addressModel.ContractProviderId = viewModel.Id;
            addressModel.StateId = address.StateId;
            addressModel.ProviderAddressTypeId = address.AddressTypeId;
            Context.Addresses.Add(addressModel);

            return true;
        }

        public virtual bool MapAddressUpdate(ProviderAddress existingAddressModel, ProviderAddressViewModel changedAddressViewModel)
        {
            Context.Addresses.Attach(existingAddressModel);
            Mapper.Map<ProviderAddressViewModel, ProviderAddress>(changedAddressViewModel, existingAddressModel);
            Context.Entry(existingAddressModel).State = EntityState.Modified;

            return true;
        }

        public virtual void MapAddressDeletions(ProviderViewModel viewModel, ContractProvider toModel)
        {
            if (toModel.Addresses == null)
                return;

            foreach (var addressModel in toModel.Addresses.ToList())
            {
                var existsInModelButNotInViewModel = viewModel.Addresses.Where(x => x.Id == addressModel.Id).SingleOrDefault() == null;

                if (existsInModelButNotInViewModel)
                {
                    Context.Addresses.Attach(addressModel);
                    Context.Entry(addressModel).State = EntityState.Deleted;
                }
            }
        }

        public virtual void MapContractCommunities(ContractViewModel fromViewModel, Contract toModel)
        {
            if (toModel == null)
                toModel = Mapper.Map<ContractViewModel, Contract>(fromViewModel);

            MapContractCommunityDeletions(fromViewModel, toModel);

            if (fromViewModel.ContractCommunities != null)
                MapContractCommunityCreationOrUpdate(fromViewModel, toModel);
        }

        private void MapContractCommunityDeletions(ContractViewModel fromViewModel, Contract toModel)
        {
            if (toModel.ContractCommunities == null)
                return;

            foreach (var contractCommunityModel in toModel.ContractCommunities.ToList())
            {
                Func<ContractCommunityViewModel, Boolean> hasSamePrimaryKey;
                hasSamePrimaryKey = (ccvm => ccvm.CommunityId == contractCommunityModel.CommunityId && ccvm.ContractInfoId == contractCommunityModel.ContractInfoId);

                var existsInModelButNotInViewModel = fromViewModel.ContractCommunities.Where(hasSamePrimaryKey).SingleOrDefault() == null;

                if (existsInModelButNotInViewModel)
                {
                    toModel.ContractCommunities.Remove(contractCommunityModel);
                }
            }
        }

        private void MapContractCommunityCreationOrUpdate(ContractViewModel fromViewModel, Contract toModel)
        {
            foreach (var contractCommunity in fromViewModel.ContractCommunities)
            {
                Func<ContractInfoCommunity, Boolean> hasSamePrimaryKey;
                hasSamePrimaryKey = (cic => cic.CommunityId == contractCommunity.CommunityId && cic.ContractInfoId == contractCommunity.ContractInfoId);

                ContractInfoCommunity existingContractCommunityModel = Context.ContractCommunities.Where(hasSamePrimaryKey).SingleOrDefault();

                bool modelDoesNotExist = (contractCommunity.ContractInfoId <= 0 || contractCommunity.CommunityId <= 0) || existingContractCommunityModel == null;

                var result = modelDoesNotExist ?
                    MapContractCommunityCreate(fromViewModel, contractCommunity, toModel) :
                    MapContractCommunityUpdate(existingContractCommunityModel, contractCommunity);
            }
        }

        private bool MapContractCommunityCreate(ContractViewModel viewModel, ContractCommunityViewModel contractCommunity, Contract toModel)
        {
            var contractCommunityModel = Mapper.Map<ContractCommunityViewModel, ContractInfoCommunity>(contractCommunity);

            if (toModel.ContractCommunities == null)
                toModel.ContractCommunities = new List<ContractInfoCommunity>();

            toModel.ContractCommunities.Add(contractCommunityModel);

            return true;
        }

        private bool MapContractCommunityUpdate(ContractInfoCommunity existingContractCommunityModel, ContractCommunityViewModel changedContractCommunityViewModel)
        {
            Context.ContractCommunities.Attach(existingContractCommunityModel);
            Mapper.Map<ContractCommunityViewModel, ContractInfoCommunity>(changedContractCommunityViewModel, existingContractCommunityModel);
            Context.Entry(existingContractCommunityModel).State = EntityState.Modified;

            return true;
        }

        public virtual void MapContractDocuments(ContractViewModel fromViewModel, Contract toModel)
        {
            if (toModel == null)
                toModel = Mapper.Map<ContractViewModel, Contract>(fromViewModel);

            MapContractDocumentDeletions(fromViewModel, toModel);

            if (fromViewModel.Documents != null)
                MapContractDocumentCreationOrUpdate(fromViewModel);
        }

        private void MapContractDocumentDeletions(ContractViewModel fromViewModel, Contract toModel)
        {
            var existsInModelButNotInViewModel = false;

            if (toModel.ContractDocuments == null)
                return;

            foreach (var documentModel in toModel.ContractDocuments.ToList())
            {
                existsInModelButNotInViewModel = (fromViewModel.Documents == null) ? true :
                        fromViewModel.Documents.Where(x => x.Id == documentModel.Id).SingleOrDefault() == null;

                if (existsInModelButNotInViewModel)
                {
                    Context.ContractDocuments.Attach(documentModel);
                    Context.Entry(documentModel).State = EntityState.Deleted;
                }
            }
        }

        private void MapContractDocumentCreationOrUpdate(ContractViewModel fromViewModel)
        {
            foreach (var document in fromViewModel.Documents)
            {
                bool isNewDocument = document.Id <= 0;
                ContractDocument existingDocumentModel = null;

                if (!isNewDocument)
                    existingDocumentModel = Context.ContractDocuments.Where(x => x.Id == document.Id).SingleOrDefault();

                bool modelDoesNotExist = isNewDocument || existingDocumentModel == null;

                var result = modelDoesNotExist ?
                    MapContractDocumentCreate(fromViewModel, document) :
                    MapContractDocumentUpdate(existingDocumentModel, document);
            }
        }

        private bool MapContractDocumentCreate(ContractViewModel viewModel, ContractDocumentViewModel document)
        {
            var documentModel = Mapper.Map<ContractDocumentViewModel, ContractDocument>(document);
            documentModel.ContractId = viewModel.Id;

            if (document.Document == null || !(document.Document.Length > 0))
            {
                return false;
            }

            byte[] docData;
            using (var reader = new BinaryReader(document.Document.OpenReadStream()))
            {
                docData = reader.ReadBytes((int)document.Document.Length);
            }
            documentModel.Document = docData;
            documentModel.FileName = Path.GetFileName(document.Document.FileName);
            documentModel.ContentType = document.Document.ContentType;

            Context.ContractDocuments.Add(documentModel);

            return true;
        }

        private bool MapContractDocumentUpdate(ContractDocument existingDocumentModel, ContractDocumentViewModel changedDocumentViewModel)
        {
            Context.ContractDocuments.Attach(existingDocumentModel);
            Mapper.Map<ContractDocumentViewModel, ContractDocument>(changedDocumentViewModel, existingDocumentModel);
            Context.Entry(existingDocumentModel).State = EntityState.Modified;

            return true;
        }

        private List<ContractCategory> GetCategories()
        {
            return Context.ContractCategories.ToList<ContractCategory>();
        }

        private List<ContractSubCategory> GetSubCategories(int selectedCategoryId)
        {
            if (selectedCategoryId == 0)
                selectedCategoryId = Context.ContractCategories.First().Id;

            List<ContractSubCategory> subCategories = Context.ContractSubCategories.Where(x => x.ContractCategoryId == selectedCategoryId).ToList<ContractSubCategory>();

            return subCategories;
        }

        public virtual List<ContractProvider> GetProviders()
        {
            return Context.Providers.Where(x => x.IsActive).ToList();
        }

        public virtual List<State> GetStates()
        {
            return Context.States.Where(x => x.StateName != String.Empty).ToList();
        }

        public virtual List<ProviderAddressType> GetAddressTypes()
        {
            return Context.AddressTypes.Where(x => x.Name != String.Empty).ToList();
        }

        public virtual List<ProviderContactType> GetContactTypes()
        {
            return Context.ContactTypes.ToList();
        }

        public virtual List<ContractProvider> GetAllActiveProviders()
        {
            return Context.Providers.Where(pro => pro.IsActive).OrderBy(p => p.Name).ToList();
        }

        public virtual ContractProvider GetProviderById(int id)
        {
            return Context.Providers
                                .Include(pro => pro.Addresses)
                                .Include(pro => pro.Addresses.Select(address => address.State))
                                .Include(pro => pro.Addresses.Select(address => address.AddressType))
                                .Include(pro => pro.Contacts)
                                .Include(pro => pro.Contacts.Select(contact => contact.ContactType))
                                .Include(pro => pro.Contracts)
                                .Include(pro => pro.Documents)
                                .Where(pro => pro.Id == id)
                                .SingleOrDefault();
        }

        public virtual Contract GetContractById(int id)
        {
            return Context.Contracts
                            .Include(con => con.ContractDocuments)
                            .Include(con => con.Provider)
                            .Include(con => con.ContractCommunities)
                            .Include(con => con.Category)
                            .Include(con => con.SubCategory)
                            .Where(con => con.Id == id)
                            .SingleOrDefault();
        }

        public virtual Contract GetFilteredContractById(int id)
        {
            var contract = Context.Contracts
                            .Include(con => con.Provider)
                            .Include(con => con.ContractCommunities)
                            .Include(con => con.Category)
                            .Include(con => con.SubCategory)
                            .Where(con => con.Id == id)
                            .SingleOrDefault();

            if (contract == null)
                return contract;

            Context.Entry(contract)
            .Collection(con => con.ContractDocuments)
            .Query()
            .Where(doc => !doc.ArchiveFlg)
            .Load();

            return contract;
        }

        //public virtual List<Contract> GetAllContractsByCommunities(IList<int> communityIds)
        //{
        //    return GetAllContractsByCommunities(false, communityIds);
        //}

        //public virtual List<Contract> GetAllContractsByCommunitiesAndIsArchived(IList<int> communityIds)
        //{
        //    return GetAllContractsByCommunities(true, communityIds);
        //}

        private List<Contract> GetAllContractsByCommunities(bool queryIsArchived, IList<int> communityIds)
        {
            var test = Context.ContractCommunities.Where(a => communityIds.Contains(a.CommunityId) && (queryIsArchived ? !a.ArchiveFlg : true));
            List<Contract> contracts = test.Select(a => a.ContractInfo).ToList();
            //List<Contract> contracts = Context.Contracts
            //            .Include(x => x.ContractCommunities)
            //            .Include(con => con.ContractDocuments)
            //            .Where(x => x.ContractCommunities.Any(
            //                    y => communityIds.Contains(y.CommunityId)) &&
            //                    (queryIsArchived == true ? !x.ArchiveFlg : 1 == 1)
            //                )
            //            .ToList();
            return contracts;
        }

        public List<ContractViewModel> GetAllContractViewsByCommunities(bool queryIsArchived, IList<int> communityIds)
        {
            List<Contract> contracts = GetAllContractsByCommunities(queryIsArchived, communityIds);

            List<ContractCategory> categories = contracts.Select(a => a.Category).ToList();//(from cats in Context.ContractCategories select cats).ToList();
            List<ContractSubCategory> subCategories = contracts.Where(a => a.SubCategory != null).Select(a => a.SubCategory).ToList();//(from subCats in Context.ContractSubCategories select subCats).ToList();
            // Providers needed by Mapper ..
            List<ContractProvider> activeProviders = GetAllActiveProviders();
            List<ContractViewModel> view = Mapper.Map<List<Contract>, List<ContractViewModel>>(contracts);

            view.ForEach(vm =>
            {
                vm.DocumentCount = Context.ContractDocuments.Count(a => a.ContractId == vm.Id);
                vm.ContractCategoryName = (categories.Where(cat => cat.Id == vm.ContractCategoryId).FirstOrDefault()).Name ?? "";
                vm.ContractSubCategoryName = vm.ContractSubCategoryId != null ? (subCategories.Where(subCats => subCats.Id == vm.ContractSubCategoryId).FirstOrDefault()).Name ?? "" : "";
            });

            return view;
        }

        //public List<ContractViewModel> SetAllCategoryLevelNames(List<ContractViewModel> listVM)
        //{
        //    for (int n = 0; n < listVM.Count; n++)
        //    {
        //        string categoryName = GetContractCategoryName(listVM[n].ContractCategoryId);
        //        if (!String.IsNullOrWhiteSpace(categoryName)) listVM[n].ContractCategoryName = categoryName;
        //        string subCategoryName = GetContractSubCategoryName(listVM[n].ContractCategoryId);
        //        if (!String.IsNullOrWhiteSpace(categoryName)) listVM[n].ContractSubCategoryName = subCategoryName;
        //    }
        //    return listVM;
        //}

        //private string GetContractCategoryName(int categoryId)
        //{
        //    if (categoryId < 0) return null;
        //    return (from categories in Context.ContractCategories
        //            where categories.Id == categoryId
        //            select categories.Name).FirstOrDefault();
        //}
        //private string GetContractSubCategoryName(int categoryId)
        //{
        //    if (categoryId < 0) return null;
        //    return (from subCategories in Context.ContractSubCategories
        //            where subCategories.Id == categoryId
        //            select subCategories.Name).FirstOrDefault();
        //}

    }
}