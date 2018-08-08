using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Survey.Models;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AtriumWebApp.Web.Survey.ViewComponents
{
    public class CreateDeficiencyGroupViewComponent : ViewComponent
    {
        SharedContext Context { get; set; }
        public CreateDeficiencyGroupViewComponent(MockSurveyContext context)
        {
            Context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new DeficiencyGroupViewModel());
        }
    }
}
