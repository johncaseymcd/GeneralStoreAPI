using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class CartQueryResult
    {
        public int CartID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }
}