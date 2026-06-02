using com.example.demo.models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Threading;
using com.example.demo.data;
using com.example.demo.repos;

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

        public Customer? Get(int id) => _repo.Get(id);

        public IEnumerable<Account>? GetAccounts(int customerId) => _repo.GetAccounts(customerId);

        public Account? GetAccountById(int accountId) => _repo.GetAccountById(accountId);

        public Account? GetAccountByNumber(int customerId, string accountNumber) => _repo.GetAccountByNumber(customerId, accountNumber);

        public Customer? UpdateCustomer(int id, Customer updated) => _repo.UpdateCustomer(id, updated);
        public bool DeleteCustomer(int id) => _repo.DeleteCustomer(id);

        public IEnumerable<Account> GetAllAccounts() => _repo.GetAllAccounts();
        public IEnumerable<Account> GetAccountsByCustomerName(string name) => _repo.GetAccountsByCustomerName(name);
        public Account? UpdateAccount(int accountId, Account updated) => _repo.UpdateAccount(accountId, updated);
        public bool DeleteAccount(int accountId) => _repo.DeleteAccount(accountId);
        public Account? CreateAccountForCustomer(int customerId, Account account) => _repo.CreateAccountForCustomer(customerId, account);
        public IEnumerable<Customer> GetPremiumCustomers(decimal threshold) => _repo.GetPremiumCustomers(threshold);
        public Account? GetAccount(int customerId, int accountId)
        {
            var accounts = GetAccounts(customerId);
            return accounts?.FirstOrDefault(a => a.Id == accountId);
        }

        public Customer Create(Customer customer) => _repo.Create(customer);

        public Account? CreateAccount(int customerId, Account account) => _repo.CreateAccount(customerId, account);
    }
}
