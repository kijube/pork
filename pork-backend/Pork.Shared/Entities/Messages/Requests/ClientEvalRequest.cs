using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Shared.Entities.Messages.Requests;

public class ClientEvalRequest : ClientRequest {
    public required string Code { get; init; }

    public override bool ShowInConsole => true;

    public ClientEvalResponse? Response { get; set; }
}