using com.example.demo.models;
using com.example.demo.settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace com.example.demo.repos
{
    public class MongoDbCustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customers;

        public MongoDbCustomerRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.DatabaseName);
            _customers = db.GetCollection<Customer>(settings.Value.CustomersCollectionName);
        }

        public IEnumerable<Customer> GetAll() =>
            _customers.Find(_ => true).ToList();

        public Customer? Get(string id) =>
            _customers.Find(c => c.Id == id).FirstOrDefault();

        public Customer Create(Customer customer)
        {
            var accounts = (customer.Accounts ?? new List<Account>())
                .Select(a => a with { Id = ObjectId.GenerateNewId().ToString() })
                .ToList();
            var created = customer with { Id = ObjectId.GenerateNewId().ToString(), Accounts = accounts };
            _customers.InsertOne(created);
            return created;
        }

        public IEnumerable<Account>? GetAccounts(string customerId) =>
            Get(customerId)?.Accounts;

        public Account? GetAccount(string customerId, string accountId) =>
            GetAccounts(customerId)?.FirstOrDefault(a => a.Id == accountId);

        public Account? CreateAccount(string customerId, Account account)
        {
            if (Get(customerId) == null) return null;
            var created = account with { Id = ObjectId.GenerateNewId().ToString() };
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, customerId);
            var update = Builders<Customer>.Update.Push(c => c.Accounts, created);
            _customers.UpdateOne(filter, update);
            return created;
        }

        public Account? CreateAccountForCustomer(string customerId, Account account) =>
            CreateAccount(customerId, account);

        public Customer? UpdateCustomer(string id, Customer updated)
        {
            var existing = Get(id);
            if (existing == null) return null;
            var merged = existing with
            {
                FirstName = updated.FirstName ?? existing.FirstName,
                LastName = updated.LastName ?? existing.LastName,
                Email = updated.Email ?? existing.Email
            };
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
            _customers.ReplaceOne(filter, merged);
            return merged;
        }

        public bool DeleteCustomer(string id)
        {
            var result = _customers.DeleteOne(c => c.Id == id);
            return result.DeletedCount > 0;
        }

        public IEnumerable<Account> GetAllAccounts() =>
            _customers.Find(_ => true).ToList()
                .SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());

        public IEnumerable<Account> GetAccountsByCustomerName(string name)
        {
            var filter = Builders<Customer>.Filter.Or(
                Builders<Customer>.Filter.Regex(c => c.FirstName, new BsonRegularExpression(name, "i")),
                Builders<Customer>.Filter.Regex(c => c.LastName, new BsonRegularExpression(name, "i"))
            );
            return _customers.Find(filter).ToList()
                .SelectMany(c => c.Accounts ?? Enumerable.Empty<Account>());
        }

        public Account? UpdateAccount(string accountId, Account updated)
        {
            var customer = _customers
                .Find(c => c.Accounts.Any(a => a.Id == accountId))
                .FirstOrDefault();
            if (customer == null) return null;

            var idx = customer.Accounts.FindIndex(a => a.Id == accountId);
            if (idx < 0) return null;

            var existing = customer.Accounts[idx];
            var merged = existing with
            {
                AccountNumber = updated.AccountNumber ?? existing.AccountNumber,
                Type = updated.Type ?? existing.Type,
                Balance = updated.Balance
            };

            var newAccounts = customer.Accounts.ToList();
            newAccounts[idx] = merged;
            var updatedCustomer = customer with { Accounts = newAccounts };

            var filter = Builders<Customer>.Filter.Eq(c => c.Id, customer.Id);
            _customers.ReplaceOne(filter, updatedCustomer);
            return merged;
        }

        public bool DeleteAccount(string accountId)
        {
            var filter = Builders<Customer>.Filter.ElemMatch(c => c.Accounts, a => a.Id == accountId);
            var update = Builders<Customer>.Update.PullFilter(c => c.Accounts, a => a.Id == accountId);
            var result = _customers.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }

        public Account? GetAccountByNumber(string customerId, string accountNumber)
        {
            var c = Get(customerId);
            return c?.Accounts?.FirstOrDefault(a =>
                string.Equals(a.AccountNumber, accountNumber, System.StringComparison.OrdinalIgnoreCase));
        }

        public Account? GetAccountById(string accountId)
        {
            var customer = _customers
                .Find(c => c.Accounts.Any(a => a.Id == accountId))
                .FirstOrDefault();
            return customer?.Accounts?.FirstOrDefault(a => a.Id == accountId);
        }

        public IEnumerable<Customer> GetPremiumCustomers(decimal threshold) =>
            _customers.Find(_ => true).ToList()
                .Where(c => (c.Accounts ?? Enumerable.Empty<Account>()).Sum(a => a.Balance) > threshold);
    }
}
