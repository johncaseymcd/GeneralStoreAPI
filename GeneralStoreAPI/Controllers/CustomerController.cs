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
    public class CustomerController : ApiController
    {
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext(); // Instantiate a DbContext object

        // Create a new customer
        [Route("api/Customer")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Customer model)
        {
            if (model is null) // Returns a bad request message if the body of the request is empty
                return BadRequest("Please enter customer information.");

            if (!ModelState.IsValid) // Returns a bad request message if the body of the request is missing required information
                return BadRequest(ModelState);

            _context.Customers.Add(model);
            
            if(await _context.SaveChangesAsync() > 0) // Returns OK if the changes were successfully saved
                return Ok("Customer successfully added!");

            return InternalServerError(); // Generic error message for issues other than checks above
        }

        // Get all customers
        [Route("api/Customer")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync(); // Project all customers into a list object
            return Ok(customers);
        }

        // Get customer by ID
        [Route("api/Customer/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerByID([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id); // Find the customer by ID

            if (customer is null) // Return 404 if customer ID is not in database
                return NotFound();

            return Ok(customer);
        }

        // Update an existing customer by ID
        [Route("api/Customer/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomer([FromUri] int id, [FromBody] Customer newCustomer)
        {
            if (id != newCustomer.CustomerID) // Return bad request message if the customer ID does not match the URI
                return BadRequest("Customer ID does not match.");

            if (!ModelState.IsValid) // Returns a bad request message if the body of the request is missing required information
                return BadRequest(ModelState);

            Customer customer = await _context.Customers.FindAsync(id); // Find the customer ID in the database

            if (customer is null) // Return 404 if the customer is not in the database
                return NotFound();

            customer.FirstName = newCustomer.FirstName;
            customer.LastName = newCustomer.LastName;

            if (await _context.SaveChangesAsync() > 0) // Returns OK if the changes were successfully saved
                return Ok("Customer information successfully updated!");

            return InternalServerError(); // Generic error message for issues other than checks above
        }

        // Delete a customer by ID
        [Route("api/Customer/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomer([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id); // Find customer by ID

            if (customer is null) // Return 404 if the customer does not exist in the database
                return NotFound();

            _context.Customers.Remove(customer); // Remove the customer from the database

            if (await _context.SaveChangesAsync() > 0) // Returns OK if the changes were successfully saved
                return Ok("Customer was successfully deleted.");

            return InternalServerError(); // Generic error message for errors other than checks above
        }
    }
}
