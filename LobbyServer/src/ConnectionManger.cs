using frar.JSONServer;
namespace frar.LobbyServer;

public class ConnectionManager{

    public readonly UserManager userManager = new UserManager();

    public ConnectionManager(){
        userManager.Connect("config.json");
    }

    public void AddConnection(Connection connection){
        new LobbyConnection(this, connection).Start();
    }


}