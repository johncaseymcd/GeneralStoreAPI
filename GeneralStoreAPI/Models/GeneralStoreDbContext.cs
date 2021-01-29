using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GeneralStoreAPI.Models
{
    public class GeneralStoreDbContext : DbContext
    {
        public GeneralStoreDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Cart> Carts { get; set; }
    }
}