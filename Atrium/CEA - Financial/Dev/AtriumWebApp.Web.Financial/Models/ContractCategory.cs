using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractCategory
    {
        //[Key, ForeignKey("Contract")]
        public int Id { get; set; }
        //public virtual Contract Contract { get; set; }

        public string Name { get; set; }
        public bool IsProvider { get; set; }

        public List<ContractSubCategory> Children { get; set; }
    }
}