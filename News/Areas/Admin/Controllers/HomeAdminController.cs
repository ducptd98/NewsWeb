using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronicStore.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace News.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}