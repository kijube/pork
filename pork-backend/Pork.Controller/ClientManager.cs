using System.Collections.Concurrent;

namespace Pork.Controller;

public static class ClientManager {
    public static readonly ConcurrentDictionary<string, ClientController> Controllers = new();

    public static bool TryGetController(string clientId, out ClientController controller) {
        return Controllers.TryGetValue(clientId, out controller);
    }
}