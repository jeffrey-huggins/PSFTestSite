using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Models.ViewModel
{
    public class ContractListViewModel
    {
        //public bool IsAdministrator { get; set; }
        public bool CanCreateNew { get; set; }

        public bool CanEdit { get; set; }

        public bool CanManageProviders { get; set; }

        public bool CanDelete { get; set; }

        public List<ContractViewModel> Contracts { get; set; }

        [DisplayName("Communities")]
        public List<int> SelectedCommunities { get; set; }

        public List<SelectListItem> Communities { get; set; }

        public ContractViewModel CurrentContract { get; set; }

        public void AndSetCurrentContract(ContractViewModel currentContract)
        {
            this.CurrentContract = currentContract;
        }
    }
}