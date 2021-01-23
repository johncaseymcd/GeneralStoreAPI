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
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext();

        // Create a new customer
        // api/Customer
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Customer model)
        {
            if (model is null)
                return BadRequest("Please enter customer information.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Customers.Add(model);
            
            if(await _context.SaveChangesAsync() > 0)
                return Ok("Customer successfully added!");

            return InternalServerError();
        }

        // Get all customers
        // api/Customer
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        // Get customer by ID
        // api/Customer/{id}
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerByID([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);

            if (customer is null)
                return NotFound();

            return Ok(customer);
        }

        // Update an existing customer by ID
        // api/Customer/{id}
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomer([FromUri] int id, [FromBody] Customer newCustomer)
        {
            if (id != newCustomer.CustomerID)
                return BadRequest("Customer ID does not match.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Customer customer = await _context.Customers.FindAsync(id);

            if (customer is null)
                return NotFound();

            customer.FirstName = newCustomer.FirstName;
            customer.LastName = newCustomer.LastName;

            if (await _context.SaveChangesAsync() <= 0)
                return Ok("Customer information successfully updated!");

            return InternalServerError();
        }

        // Delete a customer by ID
        // api/Customer/{id}
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomer([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);

            if (customer is null)
                return NotFound();

            _context.Customers.Remove(customer);

            if (await _context.SaveChangesAsync() > 0)
                return Ok("Customer was successfully deleted.");

            return InternalServerError();
        }
    }
}
