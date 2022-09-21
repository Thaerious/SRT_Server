using System.Net;
using System.Net.Sockets;

namespace frar.JSONServer;

public class Server {
    public delegate void OnAccept(Connection connection);

    public Socket socket {
        get; private set;
    } = null!;

    public bool isRunning {
        get; private set;
    } = true;

    public Server Connect(string ip, int port) {
        IPHostEntry ipHostEntry = Dns.GetHostEntry(ip);
        return this.Connect(ipHostEntry.AddressList[0], port);
    }

    public Server Connect(int port) {
        return this.Connect(IPAddress.Any, port);
    }

    private Server Connect(IPAddress ipAddress, int port) {
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
        this.socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Bind(ipEndPoint);
        this.socket.Listen(port);
        return this;
    }

    public IEnumerable<Connection> Connections() {
        while (this.isRunning) {
            Socket handler = this.socket.Accept();
            Connection connection = new Connection(handler);
            yield return connection;
        }
    }

    public Connection AcceptNext(OnAccept onAccept) {
        Socket handler = this.socket.Accept();
        Connection connection = new Connection(handler);
        onAccept(connection);
        return connection;
    }

    public Connection AcceptNext() {
        Socket handler = this.socket.Accept();
        Connection connection = new Connection(handler);
        return connection;
    }    

    public void Stop() {
        this.isRunning = false;
        this.socket.Shutdown(SocketShutdown.Both);
        this.socket.Close();
    }
}