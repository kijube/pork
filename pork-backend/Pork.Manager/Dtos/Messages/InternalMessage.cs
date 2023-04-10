using System.Text.Json.Serialization;
using Pork.Manager.Dtos.Messages.Requests;
using Pork.Manager.Dtos.Messages.Responses;

namespace Pork.Manager.Dtos.Messages;

[JsonDerivedType(typeof(InternalEvalRequest))]
[JsonDerivedType(typeof(InternalEvalResponse))]
[JsonDerivedType(typeof(InternalHookResponse))]
[JsonDerivedType(typeof(InternalFailureResponse))]
public class InternalMessage {
    public string Type { get; set; }
    public Guid? FlowId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}