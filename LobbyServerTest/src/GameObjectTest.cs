using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;
using System.Diagnostics;
namespace LobbyServerTest;

[TestClass]
public class GameObjectTest {
    [TestMethod]
    public void game_sanity_test() {
        Game game = new Game(
            name: "ima name",
            owner: "ima owner",
            password: "pw",
            maxplayers: 4
        );
    }

    [TestMethod]
    [ExpectedException(typeof(MaxPlayersException))]
    public void game_over_max() {
        Game game = new Game(
            name: "ima name",
            owner: "ima owner",
            password: "pw",
            maxplayers: 6
        );
    }

    [TestMethod]
    [ExpectedException(typeof(MaxPlayersException))]
    public void game_under_max() {
        Game game = new Game(
            name: "ima name",
            owner: "ima owner",
            password: "pw",
            maxplayers: 1
        );
    }

    [TestMethod]
    public void initial_player_is_owner() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        Assert.AreEqual(game.Players[0], "ima owner");
    }

    [TestMethod]
    public void players_getter_non_reflective() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.Players[0] = "x";
        Assert.AreEqual(game.Players[0], "ima owner");
    }

    [TestMethod]
    public void to_json_sanity() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.ToJSON();
    }

    [TestMethod]
    public void add_player() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.AddPlayer("second player", "pw");
        Assert.AreEqual(game.Players[1], "second player");
    }

    [TestMethod]
    public void add_player_returns_true_on_success() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        bool r = game.AddPlayer("second player", "pw");
        Assert.AreEqual(r, true);
    }

    [TestMethod]
    public void has_player() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.AddPlayer("second player", "pw");
        bool actual = game.HasPlayer("ima owner");
        bool expected = true;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void not_has_player() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.AddPlayer("second player", "pw");
        bool actual = game.HasPlayer("im not the owner");
        bool expected = false;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [ExpectedException(typeof(GameFullException))]
    public void add_player_full() {
        Game game = new Game("ima name", "ima owner", "pw", 2);
        game.AddPlayer("second player", "pw");
        game.AddPlayer("third player", "pw");
    }

    [TestMethod]
    [ExpectedException(typeof(RepeatedPlayerException))]
    public void add_player_repeat() {
        Game game = new Game("ima name", "ima owner", "pw", 3);
        game.AddPlayer("second player", "pw");
        game.AddPlayer("second player", "pw");
    }

    [TestMethod]
    public void remove_player() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.AddPlayer("second player", "pw");
        game.RemovePlayer("second player");
        Assert.AreEqual(game.Players.Length, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(RemoveOwnerException))]
    public void remove_owner() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.AddPlayer("second player");
        game.RemovePlayer("ima owner");
    }

    [TestMethod]
    [ExpectedException(typeof(UnknownPlayerException))]
    public void remove_unknown() {
        Game game = new Game("ima name", "ima owner", "pw", 4);
        game.RemovePlayer("second player");
    }
}