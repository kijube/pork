using Pork.Manager.Dtos.Messages;
using Pork.Manager.Dtos.Messages.Requests;
using Pork.Manager.Dtos.Messages.Responses;
using Pork.Manager.Dtos.Messages.Site;
using Pork.Shared.Entities.Messages;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;
using Pork.Shared.Entities.Messages.Site;

namespace Pork.Manager.Dtos;

public static class DtoMapper {
    private static readonly Dictionary<Type, string> TypeMap = new() {
        {typeof(ClientHookResponse), "hook"},
        {typeof(ClientFailureResponse), "error"},
        {typeof(ClientEvalResponse), "eval"},
        {typeof(ClientEvalRequest), "evalreq"},

        {typeof(SiteBroadcastMessage), "broadcast"}
    };

    public static InternalMessage MapMessage(ClientMessage message) {
        return message switch {
            ClientRequest request => MapRequest(request),
            ClientResponse response => MapResponse(response),
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, "Unknown message type")
        };
    }

    public static InternalResponse MapResponse(ClientResponse response) {
        InternalResponse result = response switch {
            ClientHookResponse hookResponse => new InternalHookResponse {
                Method = hookResponse.Method,
                HookId = hookResponse.HookId,
                Args = hookResponse.Args,
                Result = hookResponse.Result
            },
            ClientFailureResponse failureResponse => new InternalFailureResponse {
                Error = failureResponse.Error
            },
            ClientEvalResponse evalResponse => new InternalEvalResponse {
                Data = evalResponse.Data
            },
            _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
        };

        result.Timestamp = response.Timestamp;
        result.Type = TypeMap[response.GetType()];
        result.FlowId = response.FlowId;

        return result;
    }

    public static InternalRequest MapRequest(ClientRequest request) {
        InternalRequest result = request switch {
            ClientEvalRequest evalRequest => new InternalEvalRequest {
                Code = evalRequest.Code,
                Sent = evalRequest.Sent,
                SentAt = evalRequest.SentAt,
                Response = evalRequest.Response is not null
                    ? MapResponse(evalRequest.Response) as InternalEvalResponse
                    : null
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
        };

        result.Timestamp = request.Timestamp;
        result.Type = TypeMap[request.GetType()];
        result.FlowId = request.FlowId;

        return result;
    }

    public static ClientRequest MapRequest(InternalRequest request) {
        ClientRequest result = request switch {
            InternalEvalRequest evalRequest => new ClientEvalRequest {
                Code = evalRequest.Code,
                Sent = evalRequest.Sent,
                SentAt = evalRequest.SentAt
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
        };

        result.FlowId = request.FlowId;
        request.Timestamp = result.Timestamp;

        return result;
    }

    public static InternalSiteMessage MapMessage(SiteMessage message) {
        InternalSiteMessage ism = message switch {
            SiteBroadcastMessage broadcastMessage => new InternalSiteBroadcastMessage {
                Code = broadcastMessage.Code,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, null)
        };

        ism.FlowId = message.FlowId;
        ism.Timestamp = message.Timestamp;
        ism.Type = TypeMap[message.GetType()];

        return ism;
    }
}