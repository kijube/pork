using MongoDB.Driver;
using Pork.Shared.Entities;
using Pork.Shared.Entities.Messages;

namespace Pork.Shared;

public class DataContext {
    private MongoClient Client { get; }
    private IMongoDatabase Database { get; }

    public IMongoCollection<Client> Clients { get; }
    public IMongoCollection<ClientLog> ClientLogs { get; }
    public IMongoCollection<ClientMessage> ClientMessages { get; }


    public DataContext() {
        Client = new MongoClient("mongodb://mongouser:secret@db:27017");
        Database = Client.GetDatabase("Pork");

        Clients = Database.GetCollection<Client>("Clients");
        ClientLogs = Database.GetCollection<ClientLog>("ClientLogs");
        ClientMessages = Database.GetCollection<ClientMessage>("ClientMessages");
    }
}