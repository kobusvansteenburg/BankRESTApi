namespace com.example.demo.settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = "BankDb";
        public string CustomersCollectionName { get; set; } = "customers";
    }
}
