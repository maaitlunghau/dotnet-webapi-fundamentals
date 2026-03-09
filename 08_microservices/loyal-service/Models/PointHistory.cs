using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace loyal_service.Models;

public class PointHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int PointChanged { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
