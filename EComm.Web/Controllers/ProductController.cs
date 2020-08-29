using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EComm.DataAccess;
using EComm.Model;
using EComm.Web.Models;
using EComm.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy = "AdminsOnly")]
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
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            var product = _dataContext.Products.SingleOrDefault(p => p.Id == id);
            var totalCost = product.UnitPrice * quantity;
            string message = $"You added {product.ProductName} (x {quantity}) to cart at a total cost of {totalCost:C}.";

            var cart = ShoppingCart.GetFromSession(HttpContext.Session);    //get our cart from cache
            var lineItem = cart.LineItems.SingleOrDefault(item => item.Product.Id == id);  //see if the item we are adding to cart already exists
            if (lineItem != null)
            {
                lineItem.Quantity += quantity; //item exists, update quantity
            }
            else
            {
                //doesn't exist, add to the line itme list
                cart.LineItems.Add(new ShoppingCart.LineItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            //update cache
            ShoppingCart.StoreInSession(cart, HttpContext.Session);
            return PartialView("_AddedToCartPartial", message);
        }

        public IActionResult Cart()
        {
            //get our cart from cache and display in view using viewmodel
            var cart = ShoppingCart.GetFromSession(HttpContext.Session);
            var cvm = new CartViewModel() { Cart = cart };

            return View(cvm);
        }

        [HttpPost]
        public IActionResult Checkout(CartViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                //cart not valid, get from cache and show the cart view again
                cvm.Cart = ShoppingCart.GetFromSession(HttpContext.Session);
                return View("Cart", cvm);
            }
            //else clear from cache and show thank you
            HttpContext.Session.Clear();
            return View("ThankYou");
        }

        [HttpGet("api/products")]
        public IActionResult Get()
        {
            var products = _dataContext.Products.ToList();
            return new ObjectResult(products);
        }

        [HttpGet("api/products/{id:int}")]
        public IActionResult Get(int id)
        {
            var product = _dataContext.Products.SingleOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return new ObjectResult(product);
        }

        [HttpPut("api/products/{id:int}")]
        public IActionResult Put(int id, [FromBody]Product product)
        {
            //if product doesn't exist in request or the id in the request doesn't match the product return a badrequest
            if (product == null || product.Id != id)
            {
                return BadRequest();
            }
            var existing = _dataContext.Products.SingleOrDefault(p => p.Id == id);

            //if product doesn't exist return not found
            if (existing == null)
            {
                return NotFound();
            }

            //update our datacontext, with the request's info
            existing.ProductName = product.ProductName;
            existing.UnitPrice = product.UnitPrice;
            existing.Package = product.Package;
            existing.IsDiscontinued = product.IsDiscontinued;
            existing.SupplierId = product.SupplierId;
            _dataContext.Attach(existing);
            _dataContext.Entry(existing).State = EntityState.Modified;
            _dataContext.SaveChanges();
            return new NoContentResult();
        }
    }
}
