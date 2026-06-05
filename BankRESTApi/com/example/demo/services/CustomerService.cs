using com.example.demo.models;
using System.Collections.Generic;

namespace com.example.demo.services
{
    public interface CustomerService
    {
        IEnumerable<Customer> GetAll();
        Customer? Get(string id);
        Customer Create(Customer customer);
        IEnumerable<Account>? GetAccounts(string customerId);
        Account? GetAccount(string customerId, string accountId);
        Account? CreateAccount(string customerId, Account account);
        Account? GetAccountByNumber(string customerId, string accountNumber);
        Account? GetAccountById(string accountId);
        IEnumerable<Customer> GetPremiumCustomers(decimal threshold);
        Customer? UpdateCustomer(string id, Customer updated);
        bool DeleteCustomer(string id);

        IEnumerable<Account> GetAllAccounts();
        IEnumerable<Account> GetAccountsByCustomerName(string name);
        Account? UpdateAccount(string accountId, Account updated);
        bool DeleteAccount(string accountId);
        Account? CreateAccountForCustomer(string customerId, Account account);
    }
}
