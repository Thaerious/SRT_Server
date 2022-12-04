using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.JSONServer;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frar.JSONClientServerTest;

[TestClass]
public class UnitTest1 {

    // Run a server, terminate server on first connection.
    [TestMethod]
    public void Sanity() {
        Server<CloseTestHnd> server = new Server<CloseTestHnd>();
        server.Connect(7000);
        server.Listen();
        Client client = new Client();
        server.Close();
    }

    [TestMethod]
    public void ServerSendToClient() {
        Server<EchoTestHnd> server = new Server<EchoTestHnd>();
        server.Connect(7000);
        server.Listen();
        Client client = new Client(); 

        client.connection.WriteString("a1");
        client.ReadValue();

        Assert.AreEqual(client.lastValue, "a1");

        server.Close();
    }
}

class CloseTestHnd : ConnectionHnd{
    public void OnConnect(Connection connection){
        connection.Close();
    }
}

class EchoTestHnd : ConnectionHnd{
    public void OnConnect(Connection connection){
        string read = connection.ReadString();
        connection.WriteString(read);
        connection.Close();
    }
}