using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EComm.Web.ViewModels
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "Product Name is required")]
        public string ProductName { get; set; }
        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is required")]
        [Range(1.00, 500.00)]
        public decimal? UnitPrice { get; set; }
        public string Package { get; set; }
        [Display(Name = "Is Discotinued?")]
        public bool IsDiscontinued { get; set; }
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        public List<SelectListItem> Suppliers { get; set; }
    }
}
