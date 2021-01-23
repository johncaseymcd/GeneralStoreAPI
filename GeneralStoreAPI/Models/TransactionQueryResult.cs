using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class TransactionQueryResult
    {
        public int TransactionID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerFullName { get; set; }
        public string ProductSKU { get; set; }
        public string ProductName { get; set; }
        public double ProductCost { get; set; }
        public int ItemCount { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }
}