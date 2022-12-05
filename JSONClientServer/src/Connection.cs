using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace frar.JSONServer;

// Read and write json objects from/to a socket.
// The connection class is generated from the server Connections() iterator.
public class Connection {
    public static readonly int BUFFER_SIZE = 4096;
    public static readonly int INT_BUFFER_SIZE = 4;

    public readonly Socket socket;

    public bool Connected{
        get {
            if (this.socket == null) return false;
            return this.socket.Connected;
        }
    }

    public Connection(Socket socket){
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

    // Read the next string from the socket.
    public String ReadString(){
        int size = ReadSize();
        return ReadString(size);        
    }

    // Read the next JSON object from the socket.
    public JObject ReadJSON() {
        try{
            int size = ReadSize();
            String jsonString = ReadString(size);
            if (jsonString != "") return JObject.Parse(jsonString);
            this.socket.Close();
            return JObject.Parse("{}");
        } catch (Exception ex){
            throw new ConnectionException("connection exception while reading", ex);
        }
    }

    // Write a string on to the socket.
    public void WriteString(string aString) {
        byte[] msg = Encoding.ASCII.GetBytes(aString);
        byte[] len = BitConverter.GetBytes(msg.Length);
        this.socket.Send(len);
        this.socket.Send(msg);
    }

    // Write a JSON object on to the socket.
    public void WriteJSON(JObject jObject){
        String jString = jObject.ToString();
        this.WriteString(jString);
    }

    // Write a JSON object on to the socket.
    public void WriteJSON(Object anObject){
        String jString = JsonConvert.SerializeObject(anObject);
        this.WriteString(jString);
    }

    // Terminate this connection.
    public virtual void Close() {
        if (socket != null){
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}