using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using com.example.demo.repos;
using com.example.demo.services;
using com.example.demo.models;
using System.IO;
using System.Text.Json;

namespace BankRESTApi.Tests
{
    // Simple in-test fake repository to keep tests isolated from global MockDataStore
    internal class FakeCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new();
        private int _nextId = 0;
        private int _nextAccountId = 0;

        public FakeCustomerRepository(IEnumerable<Customer>? seed = null)
        {
            if (seed != null)
            {
                foreach (var c in seed)
                {
                    Create(c);
                }
            }
        }

        public IEnumerable<Customer> GetAll() => _customers;

        public Customer? Get(int id) => _customers.FirstOrDefault(x => x.Id == id);

        public Customer Create(Customer customer)
        {
            var id = ++_nextId;
            var accounts = new List<Account>();
            if (customer.Accounts != null)
            {
                foreach (var a in customer.Accounts)
                {
                    var aid = ++_nextAccountId;
                    accounts.Add(a with { Id = aid });
                }
            }
            var created = customer with { Id = id, Accounts = accounts };
            _customers.Add(created);
            return created;
        }

        public IEnumerable<Account>? GetAccounts(int customerId) => Get(customerId)?.Accounts;

        public Account? GetAccount(int customerId, int accountId) => GetAccounts(customerId)?.FirstOrDefault(a => a.Id == accountId);

        public Account? CreateAccount(int customerId, Account account)
        {
            var c = Get(customerId);
            if (c == null) return null;
            var aid = ++_nextAccountId;
            var created = account with { Id = aid };
            c.Accounts.Add(created);
            return created;
        }

        public Account? GetAccountByNumber(int customerId, string accountNumber) => GetAccounts(customerId)?.FirstOrDefault(a => string.Equals(a.AccountNumber, accountNumber, StringComparison.OrdinalIgnoreCase));

        public Account? GetAccountById(int accountId) => _customers.SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>()).FirstOrDefault(a => a.Id == accountId);

        public IEnumerable<Customer> GetPremiumCustomers(decimal threshold) => _customers.Where(c => (c.Accounts ?? Enumerable.Empty<Account>()).Sum(a => a.Balance) > threshold);

        public Customer? UpdateCustomer(int id, Customer updated)
        {
            var idx = _customers.FindIndex(c => c.Id == id);
            if (idx < 0) return null;
            var existing = _customers[idx];
            var merged = existing with
            {
                FirstName = updated.FirstName ?? existing.FirstName,
                LastName = updated.LastName ?? existing.LastName,
                Email = updated.Email ?? existing.Email
            };
            _customers[idx] = merged;
            return merged;
        }

        public bool DeleteCustomer(int id)
        {
            var c = Get(id);
            if (c == null) return false;
            return _customers.Remove(c);
        }

        public IEnumerable<Account> GetAllAccounts() => _customers.SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());

        public IEnumerable<Account> GetAccountsByCustomerName(string name) => _customers.Where(c => string.Equals(c.FirstName, name, StringComparison.OrdinalIgnoreCase) || string.Equals(c.LastName, name, StringComparison.OrdinalIgnoreCase)).SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());

        public Account? UpdateAccount(int accountId, Account updated)
        {
            foreach (var c in _customers)
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
            foreach (var c in _customers)
            {
                var a = c.Accounts.FirstOrDefault(x => x.Id == accountId);
                if (a != null)
                {
                    return c.Accounts.Remove(a);
                }
            }
            return false;
        }

        public Account? CreateAccountForCustomer(int customerId, Account account) => CreateAccount(customerId, account);
    }

    public class ServiceLayerTests
    {
        private IEnumerable<Customer> SeedData()
        {
            // try to load the project's mock-customers.json from several likely locations
            string[] candidatePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "com", "example", "demo", "data", "mock-customers.json"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "BankRESTApi", "com", "example", "demo", "data", "mock-customers.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "com", "example", "demo", "data", "mock-customers.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "BankRESTApi", "com", "example", "demo", "data", "mock-customers.json")
            };

            foreach (var p in candidatePaths)
            {
                try
                {
                    if (File.Exists(p))
                    {
                        var json = File.ReadAllText(p);
                        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var list = JsonSerializer.Deserialize<List<Customer>>(json, opts);
                        if (list != null) return list;
                    }
                }
                catch
                {
                    // ignore and try next
                }
            }

            // fallback inline seed
            return new List<Customer>
            {
                new Customer
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Accounts = new List<Account>
                    {
                        new Account { AccountNumber = "GB29NWBK60161331926819", Type = "Checking", Balance = 1023.45m },
                        new Account { AccountNumber = "GB29NWBK60161331926820", Type = "Savings", Balance = 2048.00m }
                    }
                },
                new Customer
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Accounts = new List<Account>
                    {
                        new Account { AccountNumber = "GB29NWBK60161331926821", Type = "Checking", Balance = 560.50m }
                    }
                },
                new Customer
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Email = "alice.johnson@example.com",
                    Accounts = new List<Account>()
                },
                new Customer
                {
                    FirstName = "Bob",
                    LastName = "Brown",
                    Email = "bob.brown@example.com",
                    Accounts = new List<Account>
                    {
                        new Account { AccountNumber = "GB29NWBK60161331926822", Type = "Savings", Balance = 15000.00m }
                    }
                }
            };
        }

        private (ICustomerRepository repo, CustomerService svc) CreateRepoAndService()
        {
            var repo = new FakeCustomerRepository(SeedData());
            var svc = new InMemoryCustomerService(repo);
            return (repo, svc);
        }

        [Fact]
        public void GetAll_Customers_SuccessAndEmpty()
        {
            var (repo, svc) = CreateRepoAndService();

            var all = svc.GetAll().ToList();
            Assert.True(all.Count >= 4);

            // remove all and verify empty behavior
            foreach (var c in repo.GetAll().ToList()) repo.DeleteCustomer(c.Id);
            var empty = svc.GetAll().ToList();
            Assert.Empty(empty);
        }

        [Fact]
        public void Get_ById_Success_And_NotFound()
        {
            var (repo, svc) = CreateRepoAndService();

            var first = svc.GetAll().First();
            var found = svc.Get(first.Id);
            Assert.NotNull(found);

            var missing = svc.Get(int.MaxValue);
            Assert.Null(missing);
        }

        [Fact]
        public void CreateCustomer_Success_And_Null_Throws()
        {
            var (repo, svc) = CreateRepoAndService();

            // use an existing seed customer model from the repo as a template instead of constructing a new one
            var templateCust = repo.GetAll().First();
            var toCreateCust = templateCust with { Id = 0, FirstName = "New", LastName = "One", Email = "new@one.test" };
            var created = svc.Create(toCreateCust);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);

            // creating null should throw (service passes through to repo and will throw on null)
            Assert.ThrowsAny<Exception>(() => svc.Create(null!));
        }

        [Fact]
        public void GetAccounts_GetAccount_CreateAccount_Scenarios()
        {
            var (repo, svc) = CreateRepoAndService();

            var cust = svc.GetAll().First();
            var accounts = svc.GetAccounts(cust.Id);
            Assert.NotNull(accounts);

            var firstAcc = accounts.FirstOrDefault();
            if (firstAcc != null)
            {
                var a = svc.GetAccount(cust.Id, firstAcc.Id);
                Assert.NotNull(a);
            }

            // non-existent customer
            var missingAccounts = svc.GetAccounts(int.MaxValue);
            Assert.Null(missingAccounts);

            // create account for invalid customer -> returns null
            var createdNull = svc.CreateAccount(int.MaxValue, new Account { AccountNumber = "X1", Type = "Checking", Balance = 0 });
            Assert.Null(createdNull);

            // create valid account
            // use an existing account model from the repo as template
            var accTemplate = repo.GetAll().SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>()).FirstOrDefault();
            var accToCreate = (accTemplate ?? new Account()) with { Id = 0, AccountNumber = "TEST-ACC", Type = "Checking", Balance = 10 };
            var newAcc = svc.CreateAccount(cust.Id, accToCreate);
            Assert.NotNull(newAcc);
            Assert.True(newAcc.Id > 0);
        }

        [Fact]
        public void GetAccountByNumber_And_ById_Scenarios()
        {
            var (repo, svc) = CreateRepoAndService();

            var cust = svc.GetAll().First();
            var acc = svc.GetAccounts(cust.Id)?.FirstOrDefault();
            if (acc != null)
            {
                var byNum = svc.GetAccountByNumber(cust.Id, acc.AccountNumber!);
                Assert.NotNull(byNum);

                var byId = svc.GetAccountById(acc.Id);
                Assert.NotNull(byId);
            }

            // missing scenarios
            Assert.Null(svc.GetAccountByNumber(cust.Id, "NOPE"));
            Assert.Null(svc.GetAccountById(int.MaxValue));
        }

        [Fact]
        public void PremiumCustomers_Success_And_None()
        {
            var (repo, svc) = CreateRepoAndService();

            // Bob has 15000 -> should be premium over 10000
            var premiums = svc.GetPremiumCustomers(10000m).ToList();
            Assert.Contains(premiums, c => c.FirstName == "Bob");

            var none = svc.GetPremiumCustomers(1_000_000m).ToList();
            Assert.Empty(none);
        }

        [Fact]
        public void UpdateCustomer_Success_And_NotFound()
        {
            var (repo, svc) = CreateRepoAndService();

            var cust = svc.GetAll().First();
            var updated = svc.UpdateCustomer(cust.Id, new Customer { FirstName = "Updated" });
            Assert.NotNull(updated);
            Assert.Equal("Updated", updated.FirstName);

            var missing = svc.UpdateCustomer(int.MaxValue, new Customer { FirstName = "No" });
            Assert.Null(missing);
        }

        [Fact]
        public void DeleteCustomer_Success_And_NotFound()
        {
            var (repo, svc) = CreateRepoAndService();

            var cust = svc.GetAll().First();
            var ok = svc.DeleteCustomer(cust.Id);
            Assert.True(ok);

            var notOk = svc.DeleteCustomer(int.MaxValue);
            Assert.False(notOk);
        }

        [Fact]
        public void GlobalAccounts_And_Search_Update_Delete()
        {
            var (repo, svc) = CreateRepoAndService();

            var all = svc.GetAllAccounts().ToList();
            Assert.NotEmpty(all);

            var first = all.First();
            var byId = svc.GetAccountById(first.Id);
            Assert.NotNull(byId);

            var byName = svc.GetAccountsByCustomerName("John").ToList();
            Assert.NotEmpty(byName);

                // use the existing account model as template for update
                var accTemplateForUpdate = repo.GetAll().SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>()).First(a => a.Id == first.Id);
                var updated = svc.UpdateAccount(first.Id, accTemplateForUpdate with { Type = "Updated", Balance = first.Balance + 1 });
            Assert.NotNull(updated);
            Assert.Equal("Updated", updated.Type);

            var delOk = svc.DeleteAccount(first.Id);
            Assert.True(delOk);

            var delMissing = svc.DeleteAccount(int.MaxValue);
            Assert.False(delMissing);
        }

        [Fact]
        public void CreateAccountForCustomer_Success_And_Failure()
        {
            var (repo, svc) = CreateRepoAndService();

            var cust = svc.GetAll().First();
            var acc = svc.CreateAccountForCustomer(cust.Id, new Account { AccountNumber = "NEW-1", Type = "Checking", Balance = 5 });
            Assert.NotNull(acc);

            var accFail = svc.CreateAccountForCustomer(int.MaxValue, new Account { AccountNumber = "X", Type = "Checking", Balance = 0 });
            Assert.Null(accFail);
        }
    }
}
