using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace GeneralStoreAPI.Models
{
    public class CartController : ApiController
    {
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext();

        [Route("api/Cart")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateCart([FromBody] Cart model)
        {
            model.TransactionDate = DateTimeOffset.Now;
            model.Customer = _context.Customers.Find(model.CustomerID);

            if (model is null)
                return BadRequest("Your request body cannot be blank.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Carts.Add(model);

            if (await _context.SaveChangesAsync() > 0)
                return Ok("Cart successfully created!");

            return InternalServerError();
        }

        [Route("api/Cart")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCarts()
        {
            List<Cart> carts = await _context.Carts.ToListAsync();
            return Ok(carts);
        }

        [Route("api/Cart/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCartByID([FromUri] int id)
        {
            Cart cart = await _context.Carts.FindAsync(id);

            if (cart is null)
                return NotFound();

            return Ok(cart);
        }

        [Route("api/Cart/Customer/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCartsByCustomerID([FromUri] int id)
        {
            var query =
                _context
                .Carts
                .Where(e => id == e.CustomerID)
                .Select(
                    e => new CartQueryResult
                    {
                        CartID = e.CartID,
                        CustomerID = e.CustomerID,
                        CustomerName = e.Customer.FullName,
                        Products = e.Products,
                        TransactionDate = e.TransactionDate
                    }
                );

            var queryResults = await query.ToListAsync();
            return Ok(queryResults);
        }

        [Route("api/Cart/Product/{sku}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCartsByProductSKU([FromUri] string sku)
        {
            var product =
                _context
                .Products
                .Where(e => sku == e.ProductSKU)
                .Select(
                    e => new Product
                    {
                        ProductID = e.ProductID,
                        ProductName = e.ProductName,
                        ProductCost = e.ProductCost,
                        ProductSKU = e.ProductSKU,
                        ProductStockLevel = e.ProductStockLevel
                    }
                 )
                .FirstOrDefault<Product>();

            var query =
                _context
                .Carts
                .Where(e => e.Products.Contains(product))
                .Select(
                    e => new CartQueryResult
                    {
                        CartID = e.CartID,
                        CustomerID = e.CustomerID,
                        CustomerName = e.Customer.FullName,
                        Product = e.Product,
                        TransactionDate = e.TransactionDate
                    }
                );

            var queryResults = await query.ToListAsync();
            return Ok(queryResults);
        }
    }
}
