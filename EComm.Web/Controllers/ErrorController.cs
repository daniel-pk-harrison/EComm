using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EComm.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EComm.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{statusCode:int}")]
        public IActionResult Index(int statusCode)
        {
            var evm = new ErrorViewModel();
            ViewBag.StatusCode = statusCode;
            return View("Error", evm);
        }
    }
}
