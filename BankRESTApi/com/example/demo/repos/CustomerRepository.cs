using com.example.demo.data;
using com.example.demo.models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace com.example.demo.repos
{
    public class CustomerRepository : ICustomerRepository
    {
        private int _nextId = 0;
        private int _nextAccountId = 0;

        public CustomerRepository()
        {
            if (MockDataStore.Customers != null)
            {
                for (int i = 0; i < MockDataStore.Customers.Count; i++)
                {
                    var c = MockDataStore.Customers[i];
                    if (c == null) continue;

                    var id = Interlocked.Increment(ref _nextId).ToString();

                    var accounts = new List<Account>();
                    if (c.Accounts != null)
                    {
                        foreach (var a in c.Accounts)
                        {
                            var aid = Interlocked.Increment(ref _nextAccountId).ToString();
                            accounts.Add(a with { Id = aid });
                        }
                    }

                    MockDataStore.Customers[i] = c with { Id = id, Accounts = accounts };
                }
            }
        }

        public IEnumerable<Customer> GetAll() => MockDataStore.Customers;

        public Customer? Get(string id) =>
            MockDataStore.Customers.FirstOrDefault(c => c.Id == id);

        public Customer Create(Customer customer)
        {
            var id = Interlocked.Increment(ref _nextId).ToString();
            var accounts = new List<Account>();
            if (customer.Accounts != null)
            {
                foreach (var a in customer.Accounts)
                {
                    var aid = Interlocked.Increment(ref _nextAccountId).ToString();
                    accounts.Add(a with { Id = aid });
                }
            }

            var created = customer with { Id = id, Accounts = accounts };
            MockDataStore.Customers.Add(created);
            return created;
        }

        public IEnumerable<Account>? GetAccounts(string customerId) => Get(customerId)?.Accounts;

        public Account? GetAccount(string customerId, string accountId)
        {
            var accounts = GetAccounts(customerId);
            return accounts?.FirstOrDefault(a => a.Id == accountId);
        }

        public Account? CreateAccount(string customerId, Account account)
        {
            var customer = Get(customerId);
            if (customer == null) return null;
            var aid = Interlocked.Increment(ref _nextAccountId).ToString();
            var created = account with { Id = aid };
            customer.Accounts.Add(created);
            return created;
        }

        public Account? CreateAccountForCustomer(string customerId, Account account) =>
            CreateAccount(customerId, account);

        public Customer? UpdateCustomer(string id, Customer updated)
        {
            var idx = MockDataStore.Customers.FindIndex(c => c.Id == id);
            if (idx < 0) return null;
            var existing = MockDataStore.Customers[idx];
            var merged = existing with
            {
                FirstName = updated.FirstName ?? existing.FirstName,
                LastName = updated.LastName ?? existing.LastName,
                Email = updated.Email ?? existing.Email
            };
            MockDataStore.Customers[idx] = merged;
            return merged;
        }

        public bool DeleteCustomer(string id)
        {
            var c = Get(id);
            if (c == null) return false;
            return MockDataStore.Customers.Remove(c);
        }

        public IEnumerable<Account> GetAllAccounts() =>
            MockDataStore.Customers.SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());

        public IEnumerable<Account> GetAccountsByCustomerName(string name) =>
            MockDataStore.Customers
                .Where(c => string.Equals(c.FirstName, name, System.StringComparison.OrdinalIgnoreCase)
                         || string.Equals(c.LastName, name, System.StringComparison.OrdinalIgnoreCase))
                .SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());

        public Account? UpdateAccount(string accountId, Account updated)
        {
            foreach (var c in MockDataStore.Customers)
            {
                var idx = c.Accounts.FindIndex(a => a.Id == accountId);
                if (idx >= 0)
                {
                    var existing = c.Accounts[idx];
                    var merged = existing with
                    {
                        AccountNumber = updated.AccountNumber ?? existing.AccountNumber,
                        Type = updated.Type ?? existing.Type,
                        Balance = updated.Balance
                    };
                    c.Accounts[idx] = merged;
                    return merged;
                }
            }
            return null;
        }

        public bool DeleteAccount(string accountId)
        {
            foreach (var c in MockDataStore.Customers)
            {
                var a = c.Accounts.FirstOrDefault(x => x.Id == accountId);
                if (a != null) return c.Accounts.Remove(a);
            }
            return false;
        }

        public Account? GetAccountByNumber(string customerId, string accountNumber)
        {
            var c = Get(customerId);
            return c?.Accounts?.FirstOrDefault(a =>
                string.Equals(a.AccountNumber, accountNumber, System.StringComparison.OrdinalIgnoreCase));
        }

        public Account? GetAccountById(string accountId) =>
            MockDataStore.Customers
                .SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>())
                .FirstOrDefault(a => a.Id == accountId);

        public IEnumerable<Customer> GetPremiumCustomers(decimal threshold) =>
            MockDataStore.Customers.Where(c =>
                (c.Accounts ?? Enumerable.Empty<Account>()).Sum(a => a.Balance) > threshold);
    }
}
