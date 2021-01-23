using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

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
    }
}