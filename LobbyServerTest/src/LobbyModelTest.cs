using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;

namespace LobbyServerTest;

[TestClass]
public class LobbyModelTest {
    [TestMethod]
    public void lobby_model_sanity_test() {
        new LobbyModel(); 
    }

    [TestMethod]
    public void added_player_has_corrent_name() {
        LobbyModel lobbyModel = new LobbyModel(); 
        Player player = lobbyModel.AddPlayer("Adam");
        Assert.AreEqual(player.name, "Adam");
    }

    [TestMethod]
    public void adding_same_name_twice_exception() {
        LobbyModel lobbyModel = new LobbyModel(); 
        Player player = lobbyModel.AddPlayer("Adam");
        Assert.ThrowsException<RejectedActionException>(()=>{
            Player player = lobbyModel.AddPlayer("Adam");
        });
    }            
}