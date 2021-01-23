using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class ProductController : ApiController
    {
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext();

        // Create a new product
        // api/Product
        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromBody] Product model)
        {
            if (model is null)
                return BadRequest("Please enter the product information.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Products.Add(model);

            if (await _context.SaveChangesAsync() > 0)
                return Ok("Product successfully created!");

            return InternalServerError();
        }

        // Get all products
        // api/Product
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // Get product by SKU
        // api/Product/{sku}
        [HttpGet]
        public async Task<IHttpActionResult> GetProductByID([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        // Update existing product by SKU
        // api/Product/{sku}
        [HttpPut]
        public async Task<IHttpActionResult> UpdateProduct([FromUri] string sku, [FromBody] Product newProduct)
        {
            if (sku != newProduct.ProductSKU)
                return BadRequest("SKUs do not match.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
                return NotFound();

            product.ProductName = newProduct.ProductName;
            product.ProductCost = newProduct.ProductCost;
            product.ProductStockLevel = newProduct.ProductStockLevel;

            if (await _context.SaveChangesAsync() > 0)
                return Ok("Product was successfully updated!");

            return InternalServerError();
        }

        // Delete an existing product by SKU
        // api/Product/{sku}
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProduct([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
                return NotFound();

            _context.Products.Remove(product);
            if (await _context.SaveChangesAsync() > 0)
                return Ok("Product was successfully deleted.");

            return InternalServerError();
        }
    }
}
