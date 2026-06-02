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
            // initialize id counters and assign ids to any seeded customers/accounts
            if (MockDataStore.Customers != null)
            {
                for (int i = 0; i < MockDataStore.Customers.Count; i++)
                {
                    var c = MockDataStore.Customers[i];
                    if (c == null) continue;

                    var id = Interlocked.Increment(ref _nextId);

                    var accounts = new List<Account>();
                    if (c.Accounts != null)
                    {
                        foreach (var a in c.Accounts)
                        {
                            var aid = Interlocked.Increment(ref _nextAccountId);
                            accounts.Add(a with { Id = aid });
                        }
                    }

                    MockDataStore.Customers[i] = c with { Id = id, Accounts = accounts };
                }
            }
        }

        public IEnumerable<Customer> GetAll()
        {
            return MockDataStore.Customers;
        }

        public Customer? Get(int id)
        {
            return MockDataStore.Customers.FirstOrDefault(c => c.Id == id);
        }

        public Customer Create(Customer customer)
        {
            var id = Interlocked.Increment(ref _nextId);
            var accounts = new List<Account>();
            if (customer.Accounts != null)
            {
                foreach (var a in customer.Accounts)
                {
                    var aid = Interlocked.Increment(ref _nextAccountId);
                    accounts.Add(a with { Id = aid });
                }
            }

            var created = customer with { Id = id, Accounts = accounts };
            MockDataStore.Customers.Add(created);
            return created;
        }

        public IEnumerable<Account>? GetAccounts(int customerId)
        {
            var c = Get(customerId);
            return c?.Accounts;
        }

        public Account? GetAccount(int customerId, int accountId)
        {
            var accounts = GetAccounts(customerId);
            return accounts?.FirstOrDefault(a => a.Id == accountId);
        }

        public Account? CreateAccount(int customerId, Account account)
        {
            var customer = Get(customerId);
            if (customer == null) return null;
            var aid = Interlocked.Increment(ref _nextAccountId);
            var created = account with { Id = aid };
            customer.Accounts.Add(created);
            return created;
        }

        public Account? CreateAccountForCustomer(int customerId, Account account) => CreateAccount(customerId, account);

        public Customer? UpdateCustomer(int id, Customer updated)
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

        public bool DeleteCustomer(int id)
        {
            var c = Get(id);
            if (c == null) return false;
            return MockDataStore.Customers.Remove(c);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return MockDataStore.Customers.SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());
        }

        public IEnumerable<Account> GetAccountsByCustomerName(string name)
        {
            return MockDataStore.Customers
                .Where(c => string.Equals(c.FirstName, name, System.StringComparison.OrdinalIgnoreCase) || string.Equals(c.LastName, name, System.StringComparison.OrdinalIgnoreCase))
                .SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());
        }

        public Account? UpdateAccount(int accountId, Account updated)
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

        public bool DeleteAccount(int accountId)
        {
            foreach (var c in MockDataStore.Customers)
            {
                var a = c.Accounts.FirstOrDefault(x => x.Id == accountId);
                if (a != null)
                {
                    return c.Accounts.Remove(a);
                }
            }
            return false;
        }

        public Account? GetAccountByNumber(int customerId, string accountNumber)
        {
            var c = Get(customerId);
            return c?.Accounts?.FirstOrDefault(a => string.Equals(a.AccountNumber, accountNumber, System.StringComparison.OrdinalIgnoreCase));
        }

        public Account? GetAccountById(int accountId)
        {
            return MockDataStore.Customers.SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>()).FirstOrDefault(a => a.Id == accountId);
        }

        public IEnumerable<Customer> GetPremiumCustomers(decimal threshold)
        {
            return MockDataStore.Customers.Where(c => (c.Accounts ?? Enumerable.Empty<Account>()).Sum(a => a.Balance) > threshold);
        }

    }
}
