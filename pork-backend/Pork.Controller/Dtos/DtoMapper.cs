using System.Text.Json;
using Pork.Controller.Dtos.Messages.Responses;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Controller.Dtos;

public static class DtoMapper {
    public static ClientResponse MapExternalResponse(int clientId, ExternalResponse response) {
        ClientResponse result = response switch
        {
            ExternalEvalResponse evalResponse => new ClientEvalResponse
            {
                Data = JsonSerializer.Serialize(evalResponse.Data)
            },
            ExternalFailureResponse failureResponse => new ClientFailureResponse
            {
                Error = failureResponse.Error
            },
            ExternalHookResponse hookResponse => new ClientHookResponse
            {
                Method = hookResponse.Method,
                Args = JsonSerializer.Serialize(hookResponse.Args),
                Result = JsonSerializer.Serialize(hookResponse.Result),
                HookId = hookResponse.HookId
            },
            ExternalDumpResponse dumpResponse => new ClientDumpResponse
            {
                Key = dumpResponse.Key,
                Dump = dumpResponse.Dump
            },
            _ => throw new Exception($"Unknown response type {response.Type}")
        };

        result.FlowId = response.FlowId;
        result.LocalClientId = clientId;
        result.Timestamp = DateTimeOffset.UtcNow; // maybe customize this

        return result;
    }
}