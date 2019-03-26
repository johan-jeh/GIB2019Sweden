using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OrganizerWebApp.Controllers
{
    public class ProductsFunction1Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}