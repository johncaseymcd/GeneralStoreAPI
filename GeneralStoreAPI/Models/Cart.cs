﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }
        
        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual List<Product> Products { get; set; } = new List<Product>();
        public virtual Product Product { get; set; }

        public DateTimeOffset TransactionDate { get; set; }
    }
}