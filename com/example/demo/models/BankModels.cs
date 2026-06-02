using System;

namespace com.example.demo.models
{
    public record Customer
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }

    public record Account
    {
        public int Id { get; init; }
        public int CustomerId { get; init; }
        public string Number { get; init; } = string.Empty;
        public decimal Balance { get; init; }
    }

    public record Transaction
    {
        public int Id { get; init; }
        public int AccountId { get; init; }
        public decimal Amount { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string Description { get; init; } = string.Empty;
    }
}
