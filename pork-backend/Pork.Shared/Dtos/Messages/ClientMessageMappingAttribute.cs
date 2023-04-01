namespace Pork.Shared.Dtos.Messages;

[AttributeUsage(AttributeTargets.Class)]
public class ClientMessageMappingAttribute : Attribute {
    public Type Type { get; }
    public string Discriminator { get; }

    public ClientMessageMappingAttribute(Type type) {
        Type = type;
        Discriminator = type.Name.ToLowerInvariant().Replace("client", "");
    }
}