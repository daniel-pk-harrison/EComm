using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EComm.DataAccess;
using EComm.Model;
using EComm.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EComm.Web.Controllers
{
    public class ProductController : Controller
    {
        private ECommDataContext _dataContext;

        public ProductController(ECommDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("product/{id:int}")]
        public IActionResult Detail(int id)
        {
            var product = _dataContext.Products.Include(p => p.Supplier).SingleOrDefault(p => p.Id == id);
            //linq doesnt find a product return 404
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            //find the product by id
            var product = _dataContext.Products.SingleOrDefault(p => p.Id == id);
            //linq doesnt find a product return 404
            if (product == null) return NotFound();
            //build list of suppliers
            var suppliers = _dataContext.Suppliers.ToList();
            //create a productviewmodel using our found product's properties
            var pvm = new ProductEditViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                Package = product.Package,
                IsDiscontinued = product.IsDiscontinued,
                SupplierId = product.SupplierId,
                //create our list of <select> options using the suppliers list
                Suppliers = suppliers.Select(s => new SelectListItem
                {
                    Text = s.CompanyName,
                    Value = s.Id.ToString()
                }).ToList()
            };
            return View(pvm);
        }

        [HttpPost]
        public IActionResult Edit(ProductEditViewModel pvm)
        {
            //model is invalid, reconstruct our list of suppliers and show edit view
            if (!ModelState.IsValid)
            {
                var suppliers = _dataContext.Suppliers.ToList();
                pvm.Suppliers = suppliers.Select(s => new SelectListItem
                {
                    Text = s.CompanyName,
                    Value = s.Id.ToString()
                }).ToList();
                return View(pvm);
            }
            //view model is valid, create a new product using the form data
            var product = new Product
            {
                Id = pvm.Id,
                ProductName = pvm.ProductName,
                UnitPrice = pvm.UnitPrice,
                Package = pvm.Package,
                IsDiscontinued = pvm.IsDiscontinued,
                SupplierId = pvm.SupplierId
            };
            //update our datacontext, go back to home/index
            _dataContext.Attach(product);
            _dataContext.Entry(product).State = EntityState.Modified;
            _dataContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
