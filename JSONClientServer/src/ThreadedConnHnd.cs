using Newtonsoft.Json.Linq;
namespace frar.JSONServer;

// Creates and starts a new thread when a new connection takes places.
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
                        JObject jObject = connection.ReadJSON();
                        Console.WriteLine("server : " + jObject.ToString());
                        this.Process(jObject);
                    } catch (ConnectionException) {
                        this.isRunning = false;
                        this.OnDisconnect();
                    }
                }

                connection.Close();
            })
        );
        thread.Start();
    }

    abstract public void Process(JObject jObject);

    virtual public void OnDisconnect(){
        this.isRunning = false;
    }

    public void Close() {
        try {
            this.isRunning = false;
            this.Connection.Close();
        } catch (ObjectDisposedException ex) {
            Console.WriteLine(ex);
            // do nothing
        }
    }
}
