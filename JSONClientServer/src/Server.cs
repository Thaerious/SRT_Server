using System.Net;
using System.Net.Sockets;

namespace frar.JSONServer;

public class Server<HND> where HND : ConnectionHnd, new() {
    public delegate void OnAccept(Connection connection);

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
        hnd.OnConnect(connection);
        return hnd;
    }

    // Stop accepting new connections and shut down the server.
    public void Close() {
        this.isRunning = false;
        try {
            this.socket.Shutdown(SocketShutdown.Both);
        }
        finally {
            this.socket.Close();
        }
    }
}

public class ServerNotStartedException : Exception {

    private static string MSG = "Server not connected.  Call #Connect before #Listen";

    public ServerNotStartedException() : base(MSG) { }
}