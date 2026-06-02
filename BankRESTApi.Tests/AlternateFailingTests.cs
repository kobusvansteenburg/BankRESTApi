using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using com.example.demo.repos;
using com.example.demo.services;
using com.example.demo.models;

namespace BankRESTApi.Tests
{
    // Intentionally failing alternate tests — used to exercise test runner behavior.
    public class AlternateFailingTests
    {
        private (ICustomerRepository repo, CustomerService svc) CreateRepoAndService()
        {
            var seed = new List<Customer>
            {
                new Customer { FirstName = "Fail", LastName = "One", Email = "f1@example.com", Accounts = new List<Account>{ new Account{ AccountNumber = "A1", Type = "T", Balance = 1 } } }
            };
            var repo = new FakeCustomerRepository(seed);
            var svc = new InMemoryCustomerService(repo);
            return (repo, svc);
        }

        [Fact]
        public void Fail_GetAll_ShouldBeZero()
        {
            var (repo, svc) = CreateRepoAndService();
            var all = svc.GetAll().ToList();
            // intentionally wrong expectation
            Assert.Empty(all);
        }

        [Fact]
        public void Fail_GetById_ShouldBeNull()
        {
            var (repo, svc) = CreateRepoAndService();
            var c = svc.GetAll().First();
            var found = svc.Get(c.Id);
            Assert.Null(found);
        }

        [Fact]
        public void Fail_CreateCustomer_ShouldHaveIdZero()
        {
            var (repo, svc) = CreateRepoAndService();
            var created = svc.Create(new Customer { FirstName = "X" });
            Assert.Equal(0, created.Id);
        }

        [Fact]
        public void Fail_GetAccounts_ShouldBeNullEvenWhenPresent()
        {
            var (repo, svc) = CreateRepoAndService();
            var cust = svc.GetAll().First();
            var accs = svc.GetAccounts(cust.Id);
            Assert.Null(accs);
        }

        [Fact]
        public void Fail_GetAccountByNumber_ShouldReturnNullEvenIfExists()
        {
            var (repo, svc) = CreateRepoAndService();
            var cust = svc.GetAll().First();
            var acc = svc.GetAccounts(cust.Id).First();
            var found = svc.GetAccountByNumber(cust.Id, acc.AccountNumber!);
            Assert.Null(found);
        }

        [Fact]
        public void Fail_PremiumCustomers_ShouldReturnNoneForTinyThreshold()
        {
            var (repo, svc) = CreateRepoAndService();
            var premiums = svc.GetPremiumCustomers(0m);
            Assert.Empty(premiums);
        }

        [Fact]
        public void Fail_UpdateCustomer_ShouldBeNullAfterUpdate()
        {
            var (repo, svc) = CreateRepoAndService();
            var cust = svc.GetAll().First();
            var updated = svc.UpdateCustomer(cust.Id, new Customer { FirstName = "Z" });
            Assert.Null(updated);
        }

        [Fact]
        public void Fail_DeleteCustomer_ShouldBeFalseForExisting()
        {
            var (repo, svc) = CreateRepoAndService();
            var cust = svc.GetAll().First();
            var ok = svc.DeleteCustomer(cust.Id);
            Assert.False(ok);
        }

        [Fact]
        public void Fail_GetAllAccounts_ShouldBeEmpty()
        {
            var (repo, svc) = CreateRepoAndService();
            var all = svc.GetAllAccounts();
            Assert.Empty(all);
        }

        [Fact]
        public void Fail_UpdateAccount_ShouldReturnNull()
        {
            var (repo, svc) = CreateRepoAndService();
            var acc = svc.GetAllAccounts().First();
            var u = svc.UpdateAccount(acc.Id, acc with { Type = "No" });
            Assert.Null(u);
        }

        [Fact]
        public void Fail_DeleteAccount_ShouldReturnFalseEvenIfExists()
        {
            var (repo, svc) = CreateRepoAndService();
            var acc = svc.GetAllAccounts().First();
            var ok = svc.DeleteAccount(acc.Id);
            Assert.False(ok);
        }

        [Fact]
        public void Fail_CreateAccountForCustomer_ShouldReturnNull()
        {
            var (repo, svc) = CreateRepoAndService();
            var cust = svc.GetAll().First();
            var a = svc.CreateAccountForCustomer(cust.Id, new Account { AccountNumber = "X" });
            Assert.Null(a);
        }

        [Fact]
        public void Fail_GetAccountById_ShouldBeNull()
        {
            var (repo, svc) = CreateRepoAndService();
            var id = svc.GetAllAccounts().First().Id;
            var a = svc.GetAccountById(id);
            Assert.Null(a);
        }

        [Fact]
        public void Fail_SearchAccountsByName_ShouldReturnEmptyForExistingName()
        {
            var (repo, svc) = CreateRepoAndService();
            var res = svc.GetAccountsByCustomerName("Fail");
            Assert.Empty(res);
        }
    }
}
