using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using frar.JSONServer;
using System.Threading;

namespace frar.LobbyServer;

public class LobbyServer{
    private Server server = new Server();
    private ConnectionManager connectionManager = new ConnectionManager();    
    private Thread thread = null!;
    private bool isRunning = true;

    public static void Main(string[] args){
        new LobbyServer().Start(7000);
    }

    public void Start(int port){
        Console.WriteLine($"starting server on port {port}");
        server.Connect(port);

        this.thread = new Thread(new ThreadStart(() => {
            foreach (Connection connection in server.Connections()) {
                if (!isRunning) break;
                Console.WriteLine($"new connection");
                connectionManager.AddConnection(connection);
            }
        }));

        thread.Start();
    }

    public void Stop(){
        this.server.Stop();
    }
}