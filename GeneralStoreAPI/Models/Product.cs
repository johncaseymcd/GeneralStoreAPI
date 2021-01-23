using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductSKU { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public double ProductCost { get; set; }
        [Required]
        public int ProductStockLevel { get; set; }
        public bool IsInStock
        {
            get
            {
                if (ProductStockLevel <= 0)
                    return false;

                return true;
            }
        }
    }
}