using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models.ViewModel
{
    public class ContractViewModel 
    {
        public int Id { get; set; }
        public int ContractProviderId { get; set; }
        public string ContractProviderName { get; set; }

        [DisplayName("Archive?")]
        public bool ArchiveFlg { get; set; }

        [DisplayName("Archive Date")]
        public DateTime? ArchivedDate { get; set; }

        [Required]
        [DisplayName("Category")]
        public int ContractCategoryId { get; set; }

        [DisplayName("Category Name")]
        public string ContractCategoryName { get; set; }
        //{
        //    get
        //    {

        //        if(this.Categories == null)
        //            return null;
        //        var categoryVar = (this.Categories.Select(c => c.Id == this.ContractCategoryId));
        //        if (categoryVar != null)
        //        {
        //            ContractCategory category = ((IEnumerable<ContractCategory>)categoryVar).FirstOrDefault<ContractCategory>();
        //            return category.Name;
        //        }
        //        return null;
        //    }

        //}

        [DisplayName("SubCategory")]
        public int? ContractSubCategoryId { get; set; }

        [DisplayName("SubCategory Name")]
        public string ContractSubCategoryName { get; set; }
        //{
        //    get
        //    {
        //        if (this.SubCategories == null)
        //            return null;
        //        var subCategoryVar = (this.SubCategories.Select(c => c.Id == this.ContractSubCategoryId));
        //        if (subCategoryVar != null)
        //        {
        //            ContractSubCategory subCategory = ((IEnumerable<ContractSubCategory>)subCategoryVar).FirstOrDefault<ContractSubCategory>();
        //            return subCategory.Name;
        //        }
        //        return null;
        //    }
        //}
        
        public ICollection<ContractCommunityViewModel> ContractCommunities { get; set; }
        public int DocumentCount { get; set; }
        public ICollection<ContractDocumentViewModel> Documents { get; set; }

        [DisplayName("Provider/Vendor")]
        public ICollection<ContractProvider> Providers { get; set; }
        public ICollection<ContractCategory> Categories { get; set; }
        public ICollection<ContractSubCategory> SubCategories { get; set; }

        public ICollection<ContractPaymentTerm> PaymentTerms { get; set; }
        public ICollection<ContractRenewal> Renewals { get; set; }
        public ICollection<ContractTerminationNotice> TerminationNotices { get; set; } 
    }
}