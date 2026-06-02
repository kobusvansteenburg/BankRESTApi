using com.example.demo.models;
using System.Collections.Generic;

namespace com.example.demo.data
{
    public static class MockDataStore
    {
        // Static global lists for seed data used during development/testing
        public static List<Customer> Customers { get; } = new()
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
}
