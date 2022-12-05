using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;
using System.Collections.Generic;
using System.Linq;
namespace LobbyServerTest;

[TestClass]
public class LobbyConnectorTest {
    static LobbyServer server;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext) {
        LobbyConnectorTest.server = new LobbyServer();
        LobbyConnectorTest.server.Start(7000);
    }

    [TestMethod]
    public void lobby_connector_sanity() {
        var client = new ClientConnector();
        client.Connect("127.0.0.1", 7000);
    }
}