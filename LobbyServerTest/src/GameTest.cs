using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;

namespace LobbyServerTest;

[TestClass]
public class GameTest {
    [TestMethod]
    public void player_sanity_test() {
        Player player = new Player("player_name", Guid.NewGuid());    
    }

    [TestMethod]
    public void game_sanity_test() {
        Game game = new Game("game_name", Guid.NewGuid(), Guid.NewGuid(), "password", 4);   
    }

    [TestMethod]
    public void owner_is_first_player() {
        Game game = new Game("game_name", Guid.NewGuid(), Guid.NewGuid(), "password", 4);
        Assert.AreEqual(game.ownerhash, game.playerhashes[0]);
    }
}