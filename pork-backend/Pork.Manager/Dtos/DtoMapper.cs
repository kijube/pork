using Pork.Manager.Dtos.Messages.Requests;
using Pork.Manager.Dtos.Messages.Responses;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Manager.Dtos;

public static class DtoMapper {
    private static readonly Dictionary<Type, string> TypeMap = new()
    {
        {typeof(ClientHookResponse), "hook"},
        {typeof(ClientFailureResponse), "error"},
        {typeof(ClientEvalResponse), "eval"}
    };

    public static InternalResponse MapResponse(ClientResponse response) {
        InternalResponse result = response switch
        {
            ClientHookResponse hookResponse => new InternalHookResponse
            {
                Method = hookResponse.Method,
                HookId = hookResponse.HookId,
                Args = hookResponse.Args,
                Result = hookResponse.Result
            },
            ClientFailureResponse failureResponse => new InternalFailureResponse
            {
                Error = failureResponse.Error
            },
            ClientEvalResponse evalResponse => new InternalEvalResponse
            {
                Data = evalResponse.Data
            },
            _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
        };

        result.Type = TypeMap[result.GetType()];
        result.FlowId = response.FlowId;

        return result;
    }

    public static InternalRequest MapRequest(ClientRequest request) {
        InternalRequest result = request switch
        {
            ClientEvalRequest evalRequest => new InternalEvalRequest
            {
                Code = evalRequest.Code,
                Sent = evalRequest.Sent,
                SentAt = evalRequest.SentAt
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
        };

        result.Type = TypeMap[result.GetType()];
        result.FlowId = request.FlowId;

        return result;
    }

    public static ClientRequest MapRequest(InternalRequest request) {
        ClientRequest result = request switch
        {
            InternalEvalRequest evalRequest => new ClientEvalRequest
            {
                Code = evalRequest.Code,
                Sent = evalRequest.Sent,
                SentAt = evalRequest.SentAt
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
        };

        result.FlowId = request.FlowId;

        return result;
    }
}