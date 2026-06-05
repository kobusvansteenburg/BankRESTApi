using com.example.demo.models;
using com.example.demo.repos;
using System.Collections.Generic;
using System.Linq;

namespace com.example.demo.services
{
    public class InMemoryCustomerService : CustomerService
    {
        private readonly ICustomerRepository _repo;

        public InMemoryCustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Customer> GetAll() => _repo.GetAll();
        public Customer? Get(string id) => _repo.Get(id);
        public Customer Create(Customer customer) => _repo.Create(customer);
        public IEnumerable<Account>? GetAccounts(string customerId) => _repo.GetAccounts(customerId);
        public Account? GetAccount(string customerId, string accountId) => _repo.GetAccount(customerId, accountId);
        public Account? CreateAccount(string customerId, Account account) => _repo.CreateAccount(customerId, account);
        public Account? GetAccountByNumber(string customerId, string accountNumber) => _repo.GetAccountByNumber(customerId, accountNumber);
        public Account? GetAccountById(string accountId) => _repo.GetAccountById(accountId);
        public IEnumerable<Customer> GetPremiumCustomers(decimal threshold) => _repo.GetPremiumCustomers(threshold);
        public Customer? UpdateCustomer(string id, Customer updated) => _repo.UpdateCustomer(id, updated);
        public bool DeleteCustomer(string id) => _repo.DeleteCustomer(id);
        public IEnumerable<Account> GetAllAccounts() => _repo.GetAllAccounts();
        public IEnumerable<Account> GetAccountsByCustomerName(string name) => _repo.GetAccountsByCustomerName(name);
        public Account? UpdateAccount(string accountId, Account updated) => _repo.UpdateAccount(accountId, updated);
        public bool DeleteAccount(string accountId) => _repo.DeleteAccount(accountId);
        public Account? CreateAccountForCustomer(string customerId, Account account) => _repo.CreateAccountForCustomer(customerId, account);
    }
}
