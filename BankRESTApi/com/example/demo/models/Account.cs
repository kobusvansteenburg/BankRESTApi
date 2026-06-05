using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace com.example.demo.models
{
    public record Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; init; }
        public string? AccountNumber { get; init; }
        public string? Type { get; init; }
        public decimal Balance { get; init; }
    }
}
