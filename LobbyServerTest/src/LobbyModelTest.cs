using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;
using System.Collections.Generic;
using System.Linq;

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

    // Retrieving a game by it's name
    [TestMethod]
    public void get_game_by_name() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 2);
        Game game = lobbyModel.GetGame("Adam's Game");
        string actual = game.Name;
        string expected = "Adam's Game";
        Assert.AreEqual(expected, actual);
    }        

    // Retrieving a game that doesn't exists throws an exception.
    [TestMethod]
    [ExpectedException(typeof(UnknownGameException))]
    public void get_unknown_game_by_name() {
        LobbyModel lobbyModel = new LobbyModel();
        Game game = lobbyModel.GetGame("Adam's Game");
    }  

    [TestMethod]
    public void get_all_players() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.AddPlayer("Adam");
        lobbyModel.AddPlayer("Eve");
        lobbyModel.AddPlayer("Cain");
        lobbyModel.AddPlayer("Able");
        List<string> list = lobbyModel.Players.ToList();

        var actual = list.Count;
        var expected = 4;
        Assert.AreEqual(expected, actual);
    }         

    [TestMethod]
    public void get_all_games() {
        LobbyModel lobbyModel = new LobbyModel();
        lobbyModel.CreateGame("Adam's Game", "Adam", "password", 2);
        lobbyModel.CreateGame("Eve's Game", "Eve", "password", 2);
        lobbyModel.CreateGame("Cain's Game", "Cain", "password", 2);
        lobbyModel.CreateGame("Able's Game", "Able", "password", 2);
        List<Game> list = lobbyModel.Games.ToList();

        var actual = list.Count;
        var expected = 4;
        Assert.AreEqual(expected, actual);
    }        
}