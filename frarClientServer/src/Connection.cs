using System.Net;
using System.Net.Sockets;
using System.Text;
namespace frar.clientserver;

/// <summary>
/// Manages reading & writing packets on a socket.
/// </summary>
public class Connection : IConnection {
    public static readonly int BUFFER_SIZE = 4096;
    public static readonly int INT_BUFFER_SIZE = 4;
    public readonly Socket Socket;

    public static Connection ConnectTo(string ip = "127.0.0.1", int port = 7000) {
        IPAddress ipAddress = IPAddress.Parse(ip);
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
        var socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        var connection = new Connection(socket);
        socket.Connect(ipEndPoint);
        return connection;
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="socket"></param>
    public Connection(Socket socket) {
        this.Socket = socket;
    }

    private int ReadSize() {
        byte[] bytes = new byte[INT_BUFFER_SIZE];
        int sz = Socket.Receive(bytes, INT_BUFFER_SIZE, SocketFlags.None);

        if (sz == INT_BUFFER_SIZE) {
            return BitConverter.ToInt32(bytes, 0);
        }

        throw new ConnectionException($"Expected {INT_BUFFER_SIZE} bytes read, found {sz}");
    }

    private string ReadString(int size) {
        byte[] bytes = new byte[BUFFER_SIZE];
        string data = "";
        int bytesRead = 0;

        while (bytesRead < size) {
            int nextReadSize = size < BUFFER_SIZE ? size : BUFFER_SIZE;
            int count = Socket.Receive(bytes, nextReadSize, SocketFlags.None);
            if (count == 0) break;
            data += Encoding.ASCII.GetString(bytes, 0, count);
            bytesRead += count;
        }

        return Encoding.ASCII.GetString(bytes, 0, size);
    }

    /// <summary>
    /// Read the next packet from the socket.<br>
    /// If a zero-length read occurs, then the socket is considered
    /// broken and this connection is shut down.<br>
    /// If this occurs the next json object returned will be:<br>
    /// {'system' : 'disconnect'}
    /// </summary>
    /// <returns>A JObject encoded packet</returns>
    /// <exception cref="ConnectionException"></exception>
    public Packet Read() {
        try {
            int size = ReadSize();
            String jsonString = ReadString(size);
            if (jsonString != "") return Packet.FromString(jsonString);

            this.Socket.Shutdown(SocketShutdown.Both);
            this.Socket.Close();
            return new EmptyPacket();
        }
        catch (Exception ex) {
            throw new ConnectionException("connection exception while reading", ex);
        }
    }

    /// <summary>
    /// Write a packet to the socket.
    /// </summary>
    /// <param name="packet">Non-null source packet.</param>
    public void Write(Packet packet) {
        this.Write(packet.ToString());
    }

    /// <summary>
    /// Write a string to the socket.
    /// </summary>
    /// <param name="packet">Non-null source string.</param>
    private void Write(string aString) {
        ArgumentNullException.ThrowIfNull(aString);
        byte[] message = Encoding.ASCII.GetBytes(aString);
        byte[] length = BitConverter.GetBytes(message.Length);
        this.Socket.Send(length);
        this.Socket.Send(message);
    }

    /// <summary>
    /// Shutdown & close the underlying socket.
    /// </summary>
    public virtual void Shutdown() {
        if (Socket != null) {
            try {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            catch (ObjectDisposedException) { }
        }
    }
}

// [1] https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.connect?view=net-7.0