using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GeneralStoreAPI.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace GeneralStoreAPI.Controllers
{
    public class TransactionController : ApiController
    {
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext();

        // Create a new transaction
        [Route("api/Transaction")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction([FromBody] Transaction model)
        {
            model.DateOfTransaction = DateTimeOffset.Now;
            model.Product = _context.Products.Find(model.ProductID);
            model.Customer = _context.Customers.Find(model.CustomerID);

            if (model is null)
                return BadRequest("Please enter transaction data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Product is null)
                return BadRequest("Product does not exist.");

            if (model.Customer is null)
                return BadRequest("Customer does not exist.");

            if (!model.Product.IsInStock)
                return BadRequest("Product is not currently in stock. Check back later.");

            if (model.ItemCount > model.Product.ProductStockLevel)
                return BadRequest($"We don't have enough {model.Product.ProductName} to complete your order. Please reduce the quantity.");

            _context.Transactions.Add(model);
            model.Product.ProductStockLevel -= model.ItemCount;

            if (await _context.SaveChangesAsync() > 0)
                return Ok("Transaction was successfully added!");

            return InternalServerError();
        }

        // Get all transactions
        [Route("api/Transaction/all")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransactions()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }

        // Get all transactions by Customer
        [Route("api/Transaction/Customer/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionsByCustomerName([FromUri] int id)
        {
            var query = _context
                .Transactions
                .Where(e => e.CustomerID == id)
                .Select(
                    e => new TransactionQueryResult
                    {
                        TransactionID = e.TransactionID,
                        CustomerID = e.CustomerID,
                        CustomerFirstName = e.Customer.FirstName,
                        CustomerLastName = e.Customer.LastName,
                        ProductSKU = e.Product.ProductSKU,
                        ProductName = e.Product.ProductName,
                        ProductCost = e.Product.ProductCost,
                        ItemCount = e.ItemCount,
                        TransactionDate = e.DateOfTransaction
                    }
                );

            List<TransactionQueryResult> queryResults = await query.ToListAsync();
            return Ok(queryResults);
        }

        // Get transaction by transaction ID
        [Route("api/Transaction/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionByTransID([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);

            if (transaction is null)
                return NotFound();

            return Ok(transaction);
        }

        // Update existing transaction by ID
        [Route("api/Transaction/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransaction([FromUri] int id, [FromBody] Transaction newTransaction)
        {
            if (id != newTransaction.TransactionID)
                return BadRequest("Transaction IDs do not match");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Transaction transaction = await _context.Transactions.FindAsync(id);
            newTransaction.Product = _context.Products.Find(newTransaction.ProductID);
            newTransaction.Customer = _context.Customers.Find(newTransaction.CustomerID);

            if (transaction is null)
                return NotFound();

            if (newTransaction.Product is null)
                return BadRequest("Product does not exist.");

            if (newTransaction.Customer is null)
                return BadRequest("Customer does not exist.");

            if (!newTransaction.Product.IsInStock)
                return BadRequest($"{newTransaction.Product.ProductName} is currently out of stock. Please try again later.");

            if (newTransaction.ItemCount > newTransaction.Product.ProductStockLevel)
                return BadRequest($"We do not have enough {newTransaction.Product.ProductName} in stock to complete your order. Please adjust the quantity.");

            transaction.CustomerID = newTransaction.CustomerID;
            transaction.Customer = newTransaction.Customer;
            transaction.ProductID = newTransaction.ProductID;
            transaction.Product = newTransaction.Product;
            transaction.Product.ProductStockLevel -= newTransaction.ItemCount;
            transaction.ItemCount = newTransaction.ItemCount;

            if (await _context.SaveChangesAsync() > 0)
                return Ok("Transaction was successfully updated!");

            return InternalServerError();
        }

        // Delete a transaction by ID
        [Route("api/Transaction/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteTransaction([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);

            if (transaction is null)
                return NotFound();

            transaction.Product.ProductStockLevel += transaction.ItemCount;
            _context.Transactions.Remove(transaction);

            if (await _context.SaveChangesAsync() > 0)
                return Ok("Transaction was successfully deleted.");

            return InternalServerError();
        }
    }
}
