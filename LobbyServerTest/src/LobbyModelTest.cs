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
        lobbyModel.CreateGame("Game Name", "Adam", "password", 4);
        lobbyModel.CreateGame("Game Name", "Eve", "password", 4);
    }

    // One player can not start 2 games.
    [TestMethod]
    [ExpectedException(typeof(PlayerInGameException))]
    public void create_game_same_owner() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 4);
        lobbyModel.CreateGame("Adam's Other Game", "Adam", "password", 4);
    }

    // Game can not be less than 2 players
    [TestMethod]
    [ExpectedException(typeof(MaxPlayersException))]
    public void too_few_players() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 1);
    }

    // Game can not be more than 5 players
    [TestMethod]
    [ExpectedException(typeof(MaxPlayersException))]
    public void too_many_players() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 6);
    }

    // 2 Players is permitted
    [TestMethod]
    public void player_count_min() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 2);

        bool actual = lobbyModel.PlayerHasGame("Adam");
        bool expected = true;
        Assert.AreEqual(expected, actual);
    }

    // 5 Players is permitted
    [TestMethod]
    public void player_count_max() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 5);

        bool actual = lobbyModel.PlayerHasGame("Adam");
        bool expected = true;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void contains_game_true() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 5);

        bool actual = lobbyModel.HasGame("Adam's Game");
        bool expected = true;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void contains_game_false() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 5);

        bool actual = lobbyModel.HasGame("Eve's Game");
        bool expected = false;
        Assert.AreEqual(expected, actual);
    }

    // Retrieving a game with an unknown player throws an exception.
    [TestMethod]
    [ExpectedException(typeof(UnknownPlayerException))]
    public void get_game_by_unknown_player() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 2);
        lobbyModel.GetGameByPlayer("Eve");
    }    
}