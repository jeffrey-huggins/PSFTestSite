using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class Contract 
    {
        
        public int Id { get; set; }
        public int ContractProviderId { get; set; }
        public int ContractCategoryId { get; set; }
        //public string ContractCategoryName { get; set; }
        public int? ContractSubCategoryId { get; set; }
        //public string ContractSubCategoryName { get; set; }
        [ForeignKey("ContractProviderId")]
        public virtual ContractProvider Provider { get; set; }
        [ForeignKey("ContractCategoryId")]
        public virtual ContractCategory Category { get; set; }
        [ForeignKey("ContractSubCategoryId")]
        public virtual ContractSubCategory SubCategory { get; set; }


        public bool ArchiveFlg { get; set; }
        public DateTime? ArchivedDate { get; set; }


        public ICollection<ContractInfoCommunity> ContractCommunities { get; set; }
        public ICollection<ContractDocument> ContractDocuments { get; set; }
       
    }
}