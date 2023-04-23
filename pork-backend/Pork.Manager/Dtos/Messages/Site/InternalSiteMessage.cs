using System.Text.Json.Serialization;

namespace Pork.Manager.Dtos.Messages.Site;

[JsonDerivedType(typeof(InternalSiteBroadcastMessage))]
public class InternalSiteMessage {
    public string Type { get; set; }
    public Guid? FlowId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}