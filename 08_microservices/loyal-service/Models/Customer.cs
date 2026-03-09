using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace loyal_service.Models;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int Points { get; set; }

    public string Tier { get; set; } = "Silver";
}
