using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.JSONServer;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSONClientServerTest;

[TestClass]
public class UnitTest1 {
    [TestMethod]
    public void SanityTest() {
        Server server = new Server().Connect(7000);

        Thread thread = new Thread(new ThreadStart(() => {
            Debug.WriteLine("Waiting for Connections");
            foreach (Connection connection in server.Connections()) {
                Debug.WriteLine("Connection Received");
                connection.Close();
                server.Stop();
            }
        }));

        thread.Start();

        Debug.WriteLine("new Client");
        Client client = new Client().Connect();
    }

    [TestMethod]
    public void ServerSendToClient() {
        Server server = new Server().Connect(7000);

        Thread thread = new Thread(new ThreadStart(() => {
            foreach (Connection connection in server.Connections()) {
                connection.WriteJSON(new Foo(3));
                connection.Close();
            }
        }));

        thread.Start();
        Client client = new Client();

        client.Connect();
        server.Stop();
    }

    [TestMethod]
    public void ServerSendToClient2() {
        Server server = new Server().Connect(7000);
        Client client = new Client().Connect(ip: "127.0.0.1", port: 7000);

        Connection connection = server.AcceptNext();
        connection.WriteJSON(new Foo(3));
        JObject jObject = client.ReadJSON();

        Foo foo = jObject.ToObject<Foo>();

        server.Stop();
        connection.Close();

        Assert.AreEqual(foo.ToString(), "I am foo 3, 0");
    }

    [TestMethod]
    public void send_jobject_to_client() {
        Server server = new Server().Connect(7000);
        Client client = new Client().Connect(ip: "127.0.0.1", port: 7000);

        Connection connection = server.AcceptNext();
        JObject source = new JObject(
            new JProperty("pub", 4)
        );

        connection.WriteJSON(source);
        JObject dest = client.ReadJSON();

        server.Stop();
        connection.Close();

        Assert.AreEqual(dest.ToString(Formatting.None), "{\"pub\":4}");
    }      
}