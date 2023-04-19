namespace Pork.Manager.Dtos.Messages.Site;

public class InternalSiteBroadcastMessage : InternalSiteMessage {
    public required string Code { get; init; }
}