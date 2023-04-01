using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pork.DtoGenerator.Attributes;

namespace Pork.Shared.Entities;

[GenerateDto]
public class Client {
    //public ObjectId Id { get; init; }
    [DtoInclude] public required string ClientId { get; init; }
    [DtoInclude] public string? RemoteIp { get; set; }
    [DtoInclude] public bool IsOnline { get; set; }
    [DtoInclude] public DateTimeOffset LastSeen { get; set; }
    [DtoInclude] public string? Nickname { get; set; }
}