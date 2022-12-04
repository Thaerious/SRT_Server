using frar.JSONServer;

namespace frar.JSONClientServerTest;

public class Demo{
    static void Main(string[] args) {
        System.Console.WriteLine("Begin");
        Server<ServerConnection> server = new Server<ServerConnection>();
        server.Connect(7000).Listen();

        Client client1 = new Client();     
        client1.connection.WriteString("Hello World 1");   
        client1.connection.Close();

        Client client2 = new Client();     
        client2.connection.WriteString("Hello World 2");   
        client2.connection.Close();

        server.Close();
        System.Console.WriteLine("Done");
    }
}

public class ServerConnection : ConnectionHnd{
    public void OnConnect(Connection connection){
        System.Console.WriteLine("Server OnConnect");
        var x = connection.ReadString();
        System.Console.WriteLine(x);
    }
    public void OnDisconnect(){
        System.Console.WriteLine("Server OnDisconnect");
    }
}