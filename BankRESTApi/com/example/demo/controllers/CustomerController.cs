using com.example.demo.models;
using com.example.demo.services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace com.example.demo.controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _service;

        public CustomerController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAll()
        {
            try
            {
                return Ok(_service.GetAll());
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> Get(int id)
        {
            try
            {
                var c = _service.Get(id);
                if (c is null) return NotFound();
                return Ok(c);
            }
            catch
            {
                return NotFound();
            }
        }

        // GET /api/customers/search?name={name}
        [HttpGet("search")]
        public ActionResult<IEnumerable<Customer>> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var all = _service.GetAll();
            var matches = all.Where(c => string.Equals(c.FirstName, name, System.StringComparison.OrdinalIgnoreCase)
                                      || string.Equals(c.LastName, name, System.StringComparison.OrdinalIgnoreCase));
            return Ok(matches);
        }

        // GET /api/customers/premium?threshold=10000
        [HttpGet("premium")]
        public ActionResult<IEnumerable<Customer>> GetPremium([FromQuery] decimal threshold = 10000m)
        {
            var c = _service.GetPremiumCustomers(threshold);
            return Ok(c);
        }

        [HttpPost]
        public ActionResult<Customer> Create(Customer customer)
        {
            try
            {
                var created = _service.Create(customer);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Customer> Update(int id, Customer updated)
        {
            var c = _service.UpdateCustomer(id, updated);
            if (c is null) return NotFound();
            return Ok(c);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var ok = _service.DeleteCustomer(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
