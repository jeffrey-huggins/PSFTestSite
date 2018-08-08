using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.Budget;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models.Budget.ViewModels;
using AtriumWebApp.Web.Budget.Library;

namespace AtriumWebApp.Web.Budget.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Intact", null);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
