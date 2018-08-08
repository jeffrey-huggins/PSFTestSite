using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtriumWebApp.Models.ViewModel;
using AutoMapper;

namespace AtriumWebApp.Models.Mappings
{
    public static class AutoMapperInitializer
    {
        public static void Initialize()
        {
            Mapper.CreateMap<ProviderContact, ProviderContactViewModel>()
               .ForMember(dest => dest.ProviderContactTypes, opt => opt.Ignore());

            Mapper.CreateMap<ProviderContactViewModel, ProviderContact>()
               .ForMember(dest => dest.ContactType, opt => opt.Ignore())
               .ForMember(dest => dest.Provider, opt => opt.Ignore());

            Mapper.CreateMap<ProviderAddress, ProviderAddressViewModel>()
                .ForMember(dest => dest.States, opt => opt.Ignore())
                .ForMember(dest => dest.AddressTypes, opt => opt.Ignore())
                .ForMember(dest => dest.StateId, opt => opt.ResolveUsing(x => x.State.StateCd))
                .ForMember(dest => dest.AddressTypeId, opt => opt.ResolveUsing( x => x.AddressType.Id));

            Mapper.CreateMap<ProviderAddressViewModel, ProviderAddress>()
               .ForMember(dest => dest.AddressType, opt => opt.Ignore())
               .ForMember(dest => dest.Provider, opt => opt.Ignore())
               .ForMember(dest => dest.State, opt => opt.Ignore())
               .ForMember(dest => dest.ProviderAddressTypeId, opt => opt.ResolveUsing(x => x.AddressTypeId));

            Mapper.CreateMap<ProviderDocument, ProviderDocumentViewModel>()
                .ForMember(dest => dest.Document, opt => opt.Ignore());

            Mapper.CreateMap<ProviderDocumentViewModel, ProviderDocument>()
                .ForMember(dest => dest.Document, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore());

            Mapper.CreateMap<ContractProvider, ProviderViewModel>()
                .ForMember(dest => dest.SelectedCommunityId, opt => opt.Ignore());
                
            Mapper.CreateMap<ProviderViewModel, ContractProvider>()
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Addresses, opt => opt.Ignore())
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.Documents, opt => opt.Ignore());

            Mapper.CreateMap<Contract, ContractViewModel>()
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(source => source.ContractDocuments))
                .ForMember(dest => dest.ContractCommunities, opt => opt.MapFrom(source => source.ContractCommunities))
                .ForMember(dest => dest.ContractProviderName, opt => opt.ResolveUsing(x => x.Provider.Name))
                .ForMember(dest => dest.ContractCategoryId, opt => opt.MapFrom(source => source.ContractCategoryId))
                .ForMember(dest => dest.ContractSubCategoryId, opt => opt.MapFrom(source => source.ContractSubCategoryId))
                .ForMember(dest => dest.ArchiveFlg, opt => opt.MapFrom(source => source.ArchiveFlg))
                .ForMember(dest => dest.Providers, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentTerms, opt => opt.Ignore())
                .ForMember(dest => dest.Renewals, opt => opt.Ignore())
                .ForMember(dest => dest.TerminationNotices, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
                .ForMember(dest => dest.ContractCategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.ContractSubCategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentCount, opt => opt.Ignore());
            

            Mapper.CreateMap<ContractViewModel, Contract>()
                .ForMember(dest => dest.ArchiveFlg, opt => opt.MapFrom(source => source.ArchiveFlg))
                .ForMember(dest => dest.ContractCategoryId, opt => opt.MapFrom(source => source.ContractCategoryId))
                .ForMember(dest => dest.ContractSubCategoryId, opt => opt.MapFrom(source => source.ContractSubCategoryId))
                .ForMember(dest => dest.ContractDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.ContractCommunities, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategory, opt => opt.Ignore());

            Mapper.CreateMap<ContractDocument, ContractDocumentViewModel>()
                .ForMember(dest => dest.ArchiveFlg, opt => opt.MapFrom(source => source.ArchiveFlg))
                .ForMember(dest => dest.Document, opt => opt.Ignore());

            Mapper.CreateMap<ContractDocumentViewModel, ContractDocument>()
                .ForMember(dest => dest.ArchiveFlg, opt => opt.MapFrom(source => source.ArchiveFlg))
                .ForMember(dest => dest.Document, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.Ignore());

            Mapper.CreateMap<ContractCommunityViewModel, ContractInfoCommunity>()
                .ForMember(dest => dest.ArchiveFlg, opt => opt.MapFrom(source => source.ArchiveFlg))
                .ForMember(dest => dest.ContractInfo, opt => opt.Ignore())                
                .ForMember(dest => dest.Community, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentTerm, opt => opt.Ignore())
                .ForMember(dest => dest.Renewal, opt => opt.Ignore())
                .ForMember(dest => dest.TerminationNotice, opt => opt.Ignore());

            Mapper.CreateMap<ContractInfoCommunity, ContractCommunityViewModel>()
                .ForMember(dest => dest.Communities, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentTerms, opt => opt.Ignore())
                .ForMember(dest => dest.Renewals, opt => opt.Ignore())
                .ForMember(dest => dest.TerminationNotices, opt => opt.Ignore())
                .ForMember(dest => dest.ArchiveFlg, opt => opt.MapFrom(source => source.ArchiveFlg))
                .ForMember(dest => dest.ArchiveDate, opt => opt.MapFrom(source => source.ArchiveDate));
            
            Mapper.AssertConfigurationIsValid();
        }
    }
}