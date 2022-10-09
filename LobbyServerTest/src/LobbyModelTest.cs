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
    public void added_player_has_correct_name() {
        LobbyModel lobbyModel = new LobbyModel();
        Player player = lobbyModel.AddPlayer("Adam");
        Assert.AreEqual(player.name, "Adam");
    }

    [TestMethod]
    public void adding_same_name_twice_exception() {
        LobbyModel lobbyModel = new LobbyModel();
        Player player = lobbyModel.AddPlayer("Adam");
        Assert.ThrowsException<RejectedActionException>(() => {
            Player player = lobbyModel.AddPlayer("Adam");
        });
    }

    [TestMethod]
    public void create_game_sanity() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 4);
    }

    // The lobby can not have repeated game names.
    [TestMethod]
    [ExpectedException(typeof(GameNameInUseException))]
    public void create_game_same_name() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 4);
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 4);
    }
}