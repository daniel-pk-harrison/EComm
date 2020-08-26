using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EComm.Web.ViewModels
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Package { get; set; }
        public bool IsDiscontinued { get; set; }
        public int SupplierId { get; set; }

        public List<SelectListItem> Suppliers { get; set; }
    }
}
