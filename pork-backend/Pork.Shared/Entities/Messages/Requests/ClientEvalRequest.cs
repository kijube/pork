using Pork.Shared.Entities.Messages.Responses;

namespace Pork.Shared.Entities.Messages.Requests;

public class ClientEvalRequest : ClientRequest {
    public required string Code { get; init; }

    public ClientEvalResponse? Response { get; set; }
    public int? ResponseId { get; set; }

    public ClientEvalRequest() {
        ShowInConsole = true;
    }
}