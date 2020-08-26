using EComm.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EComm.Web.ViewComponents
{
    public class ProductList : ViewComponent
    {
        private ECommDataContext _dataContext;

        public ProductList(ECommDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var products = _dataContext.Products.Include(p => p.Supplier).ToList();
            return Task.FromResult<IViewComponentResult>(View(products));
        }
    }
}
