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
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext(); // Instantiate a DbContext object

        // Create a new transaction
        [Route("api/Transaction")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction([FromBody] Transaction model)
        {
            model.DateOfTransaction = DateTimeOffset.Now; // Set the date of the transaction when it is created
            model.Product = _context.Products.Find(model.ProductID); // Get the product name based on user-given product ID
            model.Customer = _context.Customers.Find(model.CustomerID); // Get the customer's name based on user-given customer ID

            if (model is null) // Returns a bad request message if the body of the request is blank
                return BadRequest("Please enter transaction data.");

            if (!ModelState.IsValid) // Returns a bad request message if the body of the request is missing required information
                return BadRequest(ModelState);

            if (model.Product is null) // Returns a bad request message if the product ID does not match an ID in the database
                return BadRequest("Product does not exist.");

            if (model.Customer is null) // Returns a bad request message if the customer ID does not match an ID in the database
                return BadRequest("Customer does not exist.");

            if (!model.Product.IsInStock) // Returns a bad request message if the product is not in stock (has 0 or negative ProductStockLevel)
                return BadRequest("Product is not currently in stock. Check back later.");

            if (model.ItemCount > model.Product.ProductStockLevel) // Returns a bad request message if the transaction contains more of an item than the store has in stock
                return BadRequest($"We don't have enough {model.Product.ProductName} to complete your order. Please reduce the quantity.");

            _context.Transactions.Add(model); // Add the transaction to the database after passing all checks above
            model.Product.ProductStockLevel -= model.ItemCount; // Reduce the stock level of the product by the number of items purchased

            if (await _context.SaveChangesAsync() > 0) // Checks that the changes were successfully saved
                return Ok("Transaction was successfully added!");

            return InternalServerError(); // Generic message if an error is caused by an issue other than the checks above
        }

        // Get all transactions
        // Creates and returns a list of all transactions in the database
        [Route("api/Transaction/all")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransactions()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync(); // Projects all transactions into a list object
            return Ok(transactions);
        } 

        // Get all transactions by Customer
        [Route("api/Transaction/Customer/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionsByCustomerName([FromUri] int id)
        {
            var query = _context // Within the Transaction database, select all rows where the Customer ID is equal to the user-given Customer ID
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

            List<TransactionQueryResult> queryResults = await query.ToListAsync(); // Project the query results into a list object
            return Ok(queryResults);
        }

        // Get transaction by transaction ID
        [Route("api/Transaction/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionByTransID([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id); // Find the Transaction ID in the database

            if (transaction is null) // Return 404 message if ID is not in the database
                return NotFound();

            return Ok(transaction);
        }

        // Update existing transaction by ID
        [Route("api/Transaction/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransaction([FromUri] int id, [FromBody] Transaction newTransaction)
        {
            if (id != newTransaction.TransactionID) // Returns a bad request message if the user-given ID does not match the URI variable
                return BadRequest("Transaction IDs do not match");

            if (!ModelState.IsValid) // Returns a bad request message if the body of the request is missing required information
                return BadRequest(ModelState);

            Transaction transaction = await _context.Transactions.FindAsync(id); // Find the Transaction ID in the database
            newTransaction.Product = _context.Products.Find(newTransaction.ProductID); // Get the Product by the user-given ID
            newTransaction.Customer = _context.Customers.Find(newTransaction.CustomerID); // Get the Customer by the user-given ID

            if (transaction is null) // Return 404 if the transaction does not exist
                return NotFound();

            if (newTransaction.Product is null) // Return a bad request message if the Product does not exist
                return BadRequest("Product does not exist.");

            if (newTransaction.Customer is null) // Return a bad request message if the Customer does not exist
                return BadRequest("Customer does not exist.");

            if (!newTransaction.Product.IsInStock) // Returns a bad request message if the product is not in stock
                return BadRequest($"{newTransaction.Product.ProductName} is currently out of stock. Please try again later.");

            if (newTransaction.ItemCount > newTransaction.Product.ProductStockLevel) // Returns a bad request message if the item count of the order is more than the product's stock level
                return BadRequest($"We do not have enough {newTransaction.Product.ProductName} in stock to complete your order. Please adjust the quantity.");

            // Update the transaction properties
            transaction.CustomerID = newTransaction.CustomerID;
            transaction.Customer = newTransaction.Customer;
            transaction.ProductID = newTransaction.ProductID;
            transaction.Product = newTransaction.Product;
            transaction.Product.ProductStockLevel -= newTransaction.ItemCount;
            transaction.ItemCount = newTransaction.ItemCount;

            if (await _context.SaveChangesAsync() > 0) // Returns OK if the changes were successfully saved
                return Ok("Transaction was successfully updated!");

            return InternalServerError(); // Generic error message for issues other than the above checks
        }

        // Delete a transaction by ID
        [Route("api/Transaction/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteTransaction([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id); // Find the transaction in the database

            if (transaction is null) // Return 404 if the transaction ID does not exist in the database
                return NotFound();

            transaction.Product.ProductStockLevel += transaction.ItemCount; // Restore the voided products to the store's inventory
            _context.Transactions.Remove(transaction); // Remove the transaction from the database

            if (await _context.SaveChangesAsync() > 0) // Returns OK if the changes were successfully saved
                return Ok("Transaction was successfully deleted.");

            return InternalServerError(); // Generic error message for issues other than the above checks
        }
    }
}
