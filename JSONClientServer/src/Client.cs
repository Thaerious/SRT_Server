using System.Net;
using System.Net.Sockets;

namespace frar.JSONServer;

public class Client : Connection{

    public Client Connect(string ip = "127.0.0.1", int port = 7000) {
        IPAddress ipAddress = IPAddress.Parse(ip);
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
        this.socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Connect(ipEndPoint);
        return this;
    }
}