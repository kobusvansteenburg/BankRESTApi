using com.example.demo.models;
using System.Collections.Generic;

namespace com.example.demo.repos
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAll();
        Customer? Get(int id);
        Customer Create(Customer customer);

        IEnumerable<Account>? GetAccounts(int customerId);
        Account? GetAccount(int customerId, int accountId);
        Account? CreateAccount(int customerId, Account account);

        Account? GetAccountByNumber(int customerId, string accountNumber);
        Account? GetAccountById(int accountId);
        IEnumerable<Customer> GetPremiumCustomers(decimal threshold);

        Customer? UpdateCustomer(int id, Customer updated);
        bool DeleteCustomer(int id);

        IEnumerable<Account> GetAllAccounts();
        IEnumerable<Account> GetAccountsByCustomerName(string name);
        Account? UpdateAccount(int accountId, Account updated);
        bool DeleteAccount(int accountId);
        Account? CreateAccountForCustomer(int customerId, Account account);
    }
}
