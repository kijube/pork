using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pork.Shared.Entities;

public class Client {
    [BsonId] public ObjectId Id { get; init; }
    public required string ClientId { get; init; }
    public string? RemoteIp { get; set; }
    public bool IsOnline { get; set; }
    public DateTimeOffset LastSeen { get; set; }
    public string? Nickname { get; set; }
}