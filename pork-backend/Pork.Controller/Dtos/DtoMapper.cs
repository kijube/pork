using Pork.Controller.Dtos.Messages.Responses;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Controller.Dtos;

public static class DtoMapper {
    public static ClientResponse MapExternalResponse(string clientId, ExternalResponse response) {
        ClientResponse result = response switch
        {
            ExternalEvalResponse evalResponse => new ClientEvalResponse
            {
                Data = evalResponse.Data
            },
            ExternalFailureResponse failureResponse => new ClientFailureResponse
            {
                Error = failureResponse.Error
            },
            ExternalHookResponse hookResponse => new ClientHookResponse
            {
                Method = hookResponse.Method,
                Args = hookResponse.Args,
                HookId = hookResponse.HookId
            },
            _ => throw new Exception($"Unknown response type {response.Type}")
        };

        result.FlowId = response.FlowId;
        result.ClientId = clientId;
        result.Timestamp = DateTimeOffset.Now; // maybe customize this

        return result;
    }
}