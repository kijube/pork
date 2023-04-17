namespace Pork.Shared.Entities;

public class Site {
    private const string AllowedKeyChars = "abcdefghijklmnopqrstuvwxyz0123456789-";

    public int Id { get; init; }
    public required string Key { get; init; }
    public required List<LocalClient> LocalClients { get; set; }

    public static string NormalizeKey(string key) {
        return string.Join("", key.ToLowerInvariant()
            .Trim()
            .Replace(" ", "-")
            .Where(c => AllowedKeyChars.Contains(c))
            .Take(64));
    }
}