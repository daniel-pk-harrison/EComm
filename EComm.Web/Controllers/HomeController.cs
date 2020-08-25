using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EComm.Web.Models;
using EComm.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EComm.Web.Controllers
{
    public class HomeController : Controller
    {
        private ECommDataContext _dataContext;
        public HomeController(ECommDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            var products = _dataContext.Products.Include(p => p.Supplier).ToList();
            return View(products);

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
