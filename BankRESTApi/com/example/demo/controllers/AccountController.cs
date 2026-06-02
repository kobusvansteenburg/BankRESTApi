using com.example.demo.models;
using com.example.demo.services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace com.example.demo.controllers
{
    [ApiController]
    [Route("api/customers/{customerId}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly CustomerService _service;

        public AccountController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAll(int customerId)
        {
            var accounts = _service.GetAccounts(customerId);
            if (accounts is null) return NotFound();
            return Ok(accounts);
        }

        // GET /api/customers/{customerId}/Account/{accountId}
        [HttpGet("{id:int}")]
        public ActionResult<Account> Get(int customerId, int id)
        {
            var a = _service.GetAccount(customerId, id);
            if (a is null) return NotFound();
            return Ok(a);
        }

        // GET /api/customers/{customerId}/Account/{accountNumber}
        // accountNumber route placed after id route; id route constrained to int so this will match non-numeric account numbers
        [HttpGet("{accountNumber}")]
        public ActionResult<Account> GetByNumber(int customerId, string accountNumber)
        {
            var a = _service.GetAccountByNumber(customerId, accountNumber);
            if (a is null) return NotFound();
            return Ok(a);
        }

        [HttpPost]
        public ActionResult<Account> Create(int customerId, Account account)
        {
            var created = _service.CreateAccount(customerId, account);
            if (created is null) return NotFound();
            return CreatedAtAction(nameof(Get), new { customerId = customerId, id = created.Id }, created);
        }

        // Global accounts
        [HttpGet]
        [Route("/api/accounts")]
        public ActionResult<IEnumerable<Account>> GetAllGlobal()
        {
            var a = _service.GetAllAccounts();
            return Ok(a);
        }

        [HttpGet]
        [Route("/api/accounts/{id:int}")]
        public ActionResult<Account> GetGlobalById(int id)
        {
            var a = _service.GetAccountById(id);
            if (a is null) return NotFound();
            return Ok(a);
        }

        [HttpGet]
        [Route("/api/accounts/search")]
        public ActionResult<IEnumerable<Account>> SearchAccountsByName([FromQuery] string name)
        {
            var a = _service.GetAccountsByCustomerName(name);
            return Ok(a);
        }

        [HttpPost]
        [Route("/api/accounts")]
        public ActionResult<Account> CreateGlobal([FromQuery] int customerId, [FromBody] Account account)
        {
            var created = _service.CreateAccountForCustomer(customerId, account);
            if (created is null) return NotFound();
            return CreatedAtAction(nameof(GetGlobalById), new { id = created.Id }, created);
        }

        [HttpPut]
        [Route("/api/accounts/{id:int}")]
        public ActionResult<Account> UpdateGlobal(int id, Account updated)
        {
            var a = _service.UpdateAccount(id, updated);
            if (a is null) return NotFound();
            return Ok(a);
        }

        [HttpDelete]
        [Route("/api/accounts/{id:int}")]
        public ActionResult DeleteGlobal(int id)
        {
            var ok = _service.DeleteAccount(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
