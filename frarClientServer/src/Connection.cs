using System.Net;
using System.Net.Sockets;
using System.Text;
namespace frar.clientserver;

/// <summary>
/// Manages reading from and writing to a socket.
/// </summary>
public class Connection {
    public static readonly int BUFFER_SIZE = 4096;
    public static readonly int INT_BUFFER_SIZE = 4;
    public readonly Socket socket;

    public bool Connected {
        get {
            if (this.socket == null) return false;
            return this.socket.Connected;
        }
    }

    public static Connection ConnectTo(string ip = "127.0.0.1", int port = 7000){
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
        this.socket = socket;
    }

    private int ReadSize() {
        byte[] bytes = new byte[INT_BUFFER_SIZE];
        int sz = socket.Receive(bytes, INT_BUFFER_SIZE, SocketFlags.None);

        if (sz == INT_BUFFER_SIZE) {
            return BitConverter.ToInt32(bytes, 0);
        }
        return 0;
    }

    private string ReadString(int size) {
        byte[] bytes = new byte[BUFFER_SIZE];
        string data = "";
        int bytesRead = 0;

        while (bytesRead < size) {
            int nextReadSize = size < BUFFER_SIZE ? size : BUFFER_SIZE;
            int count = socket.Receive(bytes, nextReadSize, SocketFlags.None);
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

            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();
            return null;
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
        this.socket.Send(length);
        this.socket.Send(message);
    }

    /// <summary>
    /// Shutdown & close the underlying socket.
    /// </summary>
    public virtual void Shutdown() {
        if (socket != null) {
            try {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (ObjectDisposedException) { }
        }
    }
}