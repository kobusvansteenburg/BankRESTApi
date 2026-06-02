using System.Collections.Generic;

namespace com.example.demo.models
{
    public record Customer
    {
        public int Id { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Email { get; init; }

        // Each customer can hold multiple accounts
        public List<Account> Accounts { get; init; } = new();
    }
}
