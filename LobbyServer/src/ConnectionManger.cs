using frar.JSONServer;
namespace frar.LobbyServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ConnectionManager{

    public readonly UserManager userManager = new UserManager();
    public readonly LobbyModel lobbyModel = new LobbyModel();

    public ConnectionManager(){
        userManager.Connect("config.json");
    }

    public void AddConnection(Connection connection){
        new LobbyConnection(this, connection).Start();
    }

    public void Broadcast(JObject jobject){

    }


}