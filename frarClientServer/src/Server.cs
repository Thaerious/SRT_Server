using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace frar.clientserver;

/// <summary>
/// Listens for incoming connections on it's own thread.<br>
/// When a connection occurs, a new ConnectionHnd object is created.  A connection
/// object is then passed to the ConnectionHnd::OnConnect method.
/// 
/// </summary>
/// <typeparam name="HND">The object type emitted when a connection occurs.</typeparam>
public class Server<HND> where HND : Router, new() {
    public Socket socket {
        get; private set;
    } = null!;

    public bool isRunning {
        get; private set;
    } = true;

    public Server<HND> Connect(string ip, int port) {
        IPHostEntry ipHostEntry = Dns.GetHostEntry(ip);
        return this.Connect(ipHostEntry.AddressList[0], port);
    }

    // Start listening for new connections on the specified port.
    public Server<HND> Connect(int port) {
        return this.Connect(IPAddress.Any, port);
    }

    private Server<HND> Connect(IPAddress ipAddress, int port) {
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
        this.socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Bind(ipEndPoint);
        this.socket.Listen(port);
        return this;
    }

    // Listen for new connections.
    // When a connection occurs create a new ConnectionHnd object (specified by HDN type)
    // and call the OnConnect method for it, passing in the connection.
    public Thread Listen() {
        if (this.socket == null) {
            throw new ServerNotStartedException();
        }

        Thread thread = new Thread(new ThreadStart(() => {
            try {
                foreach (HND rListener in this.Connections()) ;
            }
            catch (Exception ex) {
                if (this.isRunning) throw new Exception("", ex);
            }
        }));

        thread.Start();
        return thread;
    }

    // Iterate over this method to recieve new connections.
    // foreach (Connection connection in server.Connections()) {
    //     ... do something with the connection ...
    // }
    public IEnumerable<HND> Connections() {
        while (this.isRunning) {
            yield return AcceptNext();
        }
    }

    // Wait for a single connection.
    // When a connection occurs create a new ConnectionHnd object (specified by HDN type)
    // and call the OnConnect method for it, passing in the connection.
    public HND AcceptNext() {
        Socket socket = this.socket.Accept();
        Connection connection = new Connection(socket);
        HND hnd = new HND();
        
        foreach (MethodInfo method in AttributeParser.SeekOnConnect(this)) {
            method.Invoke(this, new object[] { connection });
        }

        return hnd;
    }

    // Stop accepting new connections and shut down the server.
    public void Shutdown() {
        this.isRunning = false;
        try {
            this.socket.Shutdown(SocketShutdown.Both);
        }
        finally {
            this.socket.Close();
        }
    }
}


/// <summary>
/// Thrown if Listen is called before Connect on Server.
/// </summary>
public class ServerNotStartedException : Exception {
    private static string MSG = "Server not connected.  Call #Connect before #Listen";

    public ServerNotStartedException() : base(MSG) { }
}