using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;
using System.Collections.Generic;
using System.Linq;

namespace LobbyServerTest;

class LobbyTest {
    LobbyServer server;

    [ClassInitialize]
    public void ClassInitialize() {
        this.server = new LobbyServer();
        this.server.Start(7000);
    }

    [TestMethod]
    public void lobby_model_sanity_test() {
        Assert.IsTrue(true);
    }
}