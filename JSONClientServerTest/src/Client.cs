using System.Net;
using System.Net.Sockets;
using frar.JSONServer;

namespace frar.JSONClientServerTest;

public class Client {

    public Socket socket;
    public Connection connection;

    public string lastValue = "";

    public Client(string ip = "127.0.0.1", int port = 7000) {
        IPAddress ipAddress = IPAddress.Parse(ip);
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
        this.socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Connect(ipEndPoint);
        this.connection = new Connection(this.socket);
    }

    public void ReadValue(){
        lastValue = this.connection.ReadString();
    }
}