namespace Pork.Shared.Entities.Messages.Responses;

public class SuccessClientResponse : ClientResponse {
    public SuccessClientResponse() {
        Success = true;
    }
}