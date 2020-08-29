using EComm.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComm.Web.Models
{
    public class ShoppingCart
    {
        //contains a list of items, each item is a product and a quantity (int)
        public ShoppingCart()
        {
            LineItems = new List<LineItem>();
        }
        public List<LineItem> LineItems { get; set; }
        public string FormattedGrandTotal
        {
            get
            {
                return $"{LineItems.Sum(i => i.TotalCost):C}";
            }
        }
        //specific itmes in the cart
        public class LineItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
            public decimal TotalCost
            {
                get
                {
                    return Product.UnitPrice.Value * Quantity;
                }
            }
        }
        //get our cart from cache
        public static ShoppingCart GetFromSession(ISession session)
        {
            byte[] data;
            ShoppingCart cart = null;
            //get our cart data from cache
            bool b = session.TryGetValue("ShoppingCart", out data);
            if (b)
            {
                //deserialize to an object
                string json = Encoding.UTF8.GetString(data);
                cart = JsonConvert.DeserializeObject<ShoppingCart>(json);
            }
            //return our deserialized object if not null, else just return a new cart object
            return cart ?? new ShoppingCart();
        }
        //set our cart to cache
        public static void StoreInSession(ShoppingCart cart, ISession session)
        {
            //serialize to JSON, encode to bytes then set in cache
            string json = JsonConvert.SerializeObject(cart);
            byte[] data = Encoding.UTF8.GetBytes(json);
            session.Set("ShoppingCart", data);
        }
    }
}
