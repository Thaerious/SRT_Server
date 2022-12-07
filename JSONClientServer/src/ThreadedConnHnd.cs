using Newtonsoft.Json.Linq;
namespace frar.JSONServer;

public enum DISCONNECT_REASON {
    broken,
    gracefull
};

/// <summary>
/// Implementation of ConnectionHnd that listens for incomming communication on a
/// dedicated thread.
/// </summary>
public abstract class ThreadedConnHnd : ConnectionHnd {
    private Thread thread = null!;
    private bool isRunning = true;

    private Connection? _connection;
    public Connection Connection{
        get{
            if (_connection == null) throw new NullReferenceException();
            return _connection;
        }
        private set{
            this._connection = value;
        }
    }

    virtual public void OnConnect(Connection connection) {
        this.Connection = connection;
        this.thread = new Thread(
            new ThreadStart(() => {
                while (isRunning) {
                    try {
                        Packet packet = connection.Read();                        
                        this.Process(packet);
                    } catch (ConnectionException) {
                        if (this.isRunning){
                            this.isRunning = false;
                            Console.WriteLine("broken");
                            this.OnDisconnect(DISCONNECT_REASON.broken);
                        } 
                    }
                }
            })
        );
        thread.Start();
    }

    /// <summary>
    /// Override this method to handle incoming communication packets.  Each packet
    /// is encoded as a json object.
    /// </summary>
    /// <param name="jObject"></param>
    abstract public void Process(Packet packet);

    /// <summary>
    /// Override this method to handle disconnect behaviour.    
    /// </summary>
    /// <param name="reason">
    /// Will be 'gracefull' when the server initiated the disconnect. 
    /// Otherwise will be 'broken' indicating an abropt disconnect.
    /// </param>
    virtual public void OnDisconnect(DISCONNECT_REASON reason){}

    /// <summary>
    /// Stops listening for new packets and shuts down the underlying connection.
    /// </summary>
    public void ShutDown() {
        try {
            this.isRunning = false;
            this.Connection.Shutdown();
            this.OnDisconnect(DISCONNECT_REASON.gracefull);
        } catch (ObjectDisposedException) {}
    }
}
