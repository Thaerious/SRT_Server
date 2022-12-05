using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace frar.LobbyServer;

public class ClientListener {

    public ClientListener(ClientConnector clientConnector) {
        clientConnector.OnEvent += this.ClientEventHandler;
    }

    public void ClientEventHandler(ClientEventArgs args) {
        Console.WriteLine(args.JSON);
    }

}