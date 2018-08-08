using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractSubCategory
    {
        //[Key, ForeignKey("Contract")]
        public int Id { get; set; }
        //public virtual Contract Contract { get; set; }

        public string Name { get; set; }
        public int ContractCategoryId { get; set; }
        public ContractCategory ContractCategory { get; set; }
    }
}