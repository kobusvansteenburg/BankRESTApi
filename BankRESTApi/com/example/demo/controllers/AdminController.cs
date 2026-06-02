using com.example.demo.models;
using com.example.demo.services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace com.example.demo.controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly CustomerService _service;

        public AdminController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet("premium")]
        public ActionResult<IEnumerable<Customer>> GetPremium([FromQuery] decimal threshold = 10000m)
        {
            var c = _service.GetPremiumCustomers(threshold);
            return Ok(c);
        }
    }
}
