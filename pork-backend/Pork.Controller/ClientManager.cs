using System.Collections.Concurrent;

namespace Pork.Controller;

public static class ClientManager {
    public static readonly ConcurrentDictionary<Guid, ClientController> Controllers = new();

    public static bool TryGetController(Guid clientId, out ClientController controller) {
        return Controllers.TryGetValue(clientId, out controller);
    }
}