using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using frar.JSONServer;
using System.Threading;
namespace frar.LobbyServer;

/// <summary>
/// Starts the lobby server connection loop.  This is the main entry point for the
/// lobby server.  This class only handles new connetions to the lobby. All interaction 
/// logic is handled by the LobbyConnection class.
/// </summary>
public class LobbyServer {
    private Server server = new Server();
    private ConnectionManager connectionManager = new ConnectionManager();
    private Thread thread = null!;
    private bool isRunning = true;

    public static void Main(string[] args) {
        new LobbyServer().Start(7000);
    }

    /// <summary>
    /// Start the main loop and listen on 'port' for new connections.
    /// </summary>
    /// <param name="port">Listen on this port for new connetions. </param>
    public void Start(int port) {
        Console.WriteLine($"server> starting on port {port}");
        server.Connect(port);

        this.thread = new Thread(new ThreadStart(() => {
            foreach (Connection connection in server.Connections()) {
                if (!isRunning) break;
                Console.WriteLine($"server> new connection");
                connectionManager.AddConnection(connection);
            }
        }));

        thread.Start();
    }

    /// <summary>
    /// Stop listening for new connections and shut down the underlying server.
    /// </summary>
    public void Close() {
        this.isRunning = false;
        this.server.Close();
    }
}