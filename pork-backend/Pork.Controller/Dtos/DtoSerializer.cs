using System.Text.Json;
using Pork.Controller.Dtos.Messages.Requests;
using Pork.Controller.Dtos.Messages.Responses;
using Pork.Shared.Entities.Messages.Requests;
using Serilog;

namespace Pork.Controller.Dtos;

public static class DtoSerializer {
    private static readonly Dictionary<string, Type> ResponseMap = new() {
        {"e", typeof(ExternalEvalResponse)},
        {"f", typeof(ExternalFailureResponse)},
        {"h", typeof(ExternalHookResponse)}
    };

    private static readonly Dictionary<Type, Type> RequestMap = new() {
        {typeof(ClientEvalRequest), typeof(ExternalEvalRequest)}
    };

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {
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

    public static ExternalRequest MapRequest(ClientRequest request) {
        var result = request switch {
            ClientEvalRequest evalRequest => new ExternalEvalRequest {
                Type = "e",
                Code = evalRequest.Code
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
        };

        result.FlowId = request.FlowId;
        return result;
    }

    public static bool TrySerializeRequest(ClientRequest request, out string json) {
        json = null!;

        if (!RequestMap.TryGetValue(request.GetType(), out var externalType)) {
            return false;
        }

        if (!externalType.Name.StartsWith("External")) {
            // security feature: only allow External types
            Log.Warning("Blocked sending of non-external type to {ClientId}: {TypeName}", request.LocalClientId,
                externalType.Name);
            return false;
        }

        json = JsonSerializer.Serialize(MapRequest(request), externalType, JsonSerializerOptions);
        return true;
    }
}