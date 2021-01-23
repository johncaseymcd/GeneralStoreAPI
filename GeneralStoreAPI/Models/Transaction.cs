using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }
        
        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }
        public virtual Customer Customer
        {
            get
            {
                GeneralStoreDbContext ctx = new GeneralStoreDbContext();
                return ctx.Customers.Find(CustomerID);
            }
        }

        [ForeignKey(nameof(Product))]
        public string ProductSKU { get; set; }
        public virtual Product Product
        {
            get
            {
                GeneralStoreDbContext ctx = new GeneralStoreDbContext();
                return ctx.Products.Find(ProductSKU);
            }
        }

        [Required]
        public int ItemCount { get; set; }

        [Required]
        public DateTimeOffset DateOfTransaction { get; set; }
    }
}