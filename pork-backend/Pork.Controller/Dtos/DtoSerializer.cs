using System.Text.Json;
using Pork.Controller.Dtos.Messages.Requests;
using Pork.Controller.Dtos.Messages.Responses;
using Pork.Shared.Entities.Messages.Requests;

namespace Pork.Controller.Dtos;

public static class DtoSerializer {
    private static readonly Dictionary<string, Type> ResponseMap = new()
    {
        {"e", typeof(ExternalEvalResponse)},
        {"f", typeof(ExternalFailureResponse)},
        {"h", typeof(ExternalHookResponse)}
    };

    private static readonly Dictionary<Type, Type> RequestMap = new()
    {
        {typeof(ClientEvalRequest), typeof(ExternalEvalRequest)}
    };

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static bool TryMapResponse(string json, out ExternalResponse response) {
        response = null!;
        var baseResponse = JsonSerializer.Deserialize<ExternalResponse>(json, JsonSerializerOptions);

        if (baseResponse is null) {
            return false;
        }

        if (!ResponseMap.TryGetValue(baseResponse.Type, out var type)) {
            return false;
        }

        var typedResponse = JsonSerializer.Deserialize(json, type, JsonSerializerOptions);
        if (typedResponse is null) {
            return false;
        }

        response = (ExternalResponse) typedResponse;
        return true;
    }

    public static bool TrySerializeRequest(ClientRequest request, out string json) {
        json = null!;

        if (!RequestMap.TryGetValue(request.GetType(), out var externalType)) {
            return false;
        }

        json = JsonSerializer.Serialize(request, externalType, JsonSerializerOptions);
        return true;
    }
}