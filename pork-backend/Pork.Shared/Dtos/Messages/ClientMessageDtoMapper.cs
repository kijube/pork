using System.Reflection;
using Pork.Shared.Entities.Messages;

namespace Pork.Shared.Dtos.Messages;

public record MappingInfo(Type EntityType, Type DtoType, string Discriminator);

public static class ClientMessageDtoMapper {
    private static readonly Dictionary<Type, MappingInfo> TypeMap = new();
    private static readonly Dictionary<string, MappingInfo> DiscriminatorMap = new();

    static ClientMessageDtoMapper() {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types) {
            var attribute = type.GetCustomAttribute<ClientMessageMappingAttribute>();
            if (attribute == null) continue;
            TypeMap.Add(attribute.Type, new MappingInfo(attribute.Type, type, attribute.Discriminator));
            DiscriminatorMap.Add(attribute.Discriminator,
                new MappingInfo(attribute.Type, type, attribute.Discriminator));
        }
    }

    public static ClientMessageDto MapFrom(ClientMessage clientMessage) {
        var info = TypeMap[clientMessage.GetType()];
        var dto = (ClientMessageDto) Activator.CreateInstance(info.DtoType)!;
        dto.Type = info.Discriminator;
        dto.FlowId = clientMessage.FlowId;
        dto.Timestamp = clientMessage.Timestamp;

        return dto;
    }
}