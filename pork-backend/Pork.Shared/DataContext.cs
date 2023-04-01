using MongoDB.Driver;
using Pork.Shared.Entities;
using Pork.Shared.Entities.Messages;

namespace Pork.Shared;

public class DataContext {
    private MongoClient Client { get; }
    private IMongoDatabase Database { get; }

    public IMongoCollection<Client> Clients { get; }
    public IMongoCollection<ClientLog> ClientLogs { get; }
    public IMongoCollection<ClientMessage> ClientSocketMessages { get; }


    public DataContext() {
        Client = new MongoClient("mongodb://mongouser:secret@localhost:27017");
        Database = Client.GetDatabase("Pork");

        Clients = Database.GetCollection<Client>("Clients");
        ClientLogs = Database.GetCollection<ClientLog>("ClientLogs");
        ClientSocketMessages = Database.GetCollection<ClientMessage>("ClientSocketMessages");
    }
}