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
        [Route("api/Product")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromBody] Product model)
        {
            if (model is null) // Returns a bad request if the request body is empty
                return BadRequest("Please enter the product information.");

            if (!ModelState.IsValid) // Returns a bad request if the request body is missing required information
                return BadRequest(ModelState);

            _context.Products.Add(model); // Adds the product to the database

            if (await _context.SaveChangesAsync() > 0) // Returns OK if the changes were saved successfully
                return Ok("Product successfully created!");

            return InternalServerError(); // Generic error message for issues other than the above checks
        }

        // Get all products
        [Route("api/Product")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            List<Product> products = await _context.Products.ToListAsync(); // Gather all products from the database into a list object
            return Ok(products);
        }

        // Get product by ID
        [Route("api/Product/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetProductByID([FromUri] int id)
        {
            Product product = await _context.Products.FindAsync(id); // Find product in the database by ID

            if (product is null) // Returns 404 if the product does not exist in the database
                return NotFound();

            return Ok(product);
        }

        // Update existing product by ID
        [Route("api/Product/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateProduct([FromUri] int id, [FromBody] Product newProduct)
        {
            if (id != newProduct.ProductID) // Returns a bad request if the product ID does not match the URI
                return BadRequest("IDs do not match.");

            if (!ModelState.IsValid) // Returns a bad request if the request body is missing required information
                return BadRequest(ModelState);

            Product product = await _context.Products.FindAsync(id); // Find the product by ID

            if (product is null) // Return 404 if the product does not exist in the database
                return NotFound();

            // Update the product information
            product.ProductName = newProduct.ProductName;
            product.ProductCost = newProduct.ProductCost;
            product.ProductStockLevel = newProduct.ProductStockLevel;
            product.ProductSKU = newProduct.ProductSKU;

            if (await _context.SaveChangesAsync() > 0) // Returns OK if the changes were successfully saved
                return Ok("Product was successfully updated!");

            return InternalServerError(); // Generic error message for issues other than checks above
        }

        // Delete an existing product by ID
        [Route("api/Product/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProduct([FromUri] int id)
        {
            Product product = await _context.Products.FindAsync(id); // Find product in database by ID

            if (product is null) // Returns 404 if the product does not exist in the database
                return NotFound();

            _context.Products.Remove(product); // Remove the product from the database

            if (await _context.SaveChangesAsync() > 0) // Returns OK if the changes were successfully saved
                return Ok("Product was successfully deleted.");

            return InternalServerError(); // Generic error message for issues other than checks above
        }
    }
}
