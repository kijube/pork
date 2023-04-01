using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Shared;

using System.Text.Json;

public class SocketResponseParser {
    private static readonly Dictionary<string, Type> SocketEventTypeMap = new()
    {
        {"eval", typeof(ClientEvalResponse)},
        {"hook", typeof(ClientHookResponse)}
    };

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static bool TryParse(string json, out ClientResponse? socketEvent) {
        socketEvent = null;
        // json serializer options web

        var response = JsonSerializer.Deserialize<ClientResponse>(json, JsonSerializerOptions);
        if (response is null) {
            return false;
        }

        if (!response.Success) {
            socketEvent = JsonSerializer.Deserialize<FailureClientResponse>(json, JsonSerializerOptions);
            return true;
        }

        var eventType = response?.Type;
        if (eventType is null || !SocketEventTypeMap.TryGetValue(eventType, out var type)) {
            return false;
        }

        socketEvent = (ClientResponse?) JsonSerializer.Deserialize(json, type, JsonSerializerOptions);

        if (socketEvent is ClientHookResponse he) {
            // we need to parse the args/result property ourselves
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json, JsonSerializerOptions);
            if (jsonElement.TryGetProperty("args", out var argsElement)) {
                he.Args = JsonSerializer.Serialize(argsElement);
            }
            else if (jsonElement.TryGetProperty("result", out var resultElement)) {
                he.Result = JsonSerializer.Serialize(resultElement);
            }
        }

        return true;
    }
}