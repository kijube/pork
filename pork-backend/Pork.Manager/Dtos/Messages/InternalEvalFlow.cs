using Pork.Manager.Dtos.Messages.Requests;
using Pork.Manager.Dtos.Messages.Responses;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Manager.Dtos.Messages;

public class InternalEvalFlow {
    public required InternalEvalRequest Request { get; init; }
    public InternalEvalResponse? Response { get; init; }

    public static InternalEvalFlow From(ClientEvalRequest request, ClientEvalResponse? response) => new()
    {
        Request = (InternalEvalRequest) DtoMapper.MapRequest(request),
        Response = response is not null ? (InternalEvalResponse) DtoMapper.MapResponse(response) : null
    };
}