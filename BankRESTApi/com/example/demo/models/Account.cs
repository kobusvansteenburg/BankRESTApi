namespace com.example.demo.models
{
    public record Account
    {
        public int Id { get; init; }
        public string? AccountNumber { get; init; }
        public string? Type { get; init; }
        public decimal Balance { get; init; }
    }
}
