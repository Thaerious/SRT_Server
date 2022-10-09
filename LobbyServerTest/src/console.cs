using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;
using System.Diagnostics;

Game game = new Game("ima name", "ima owner", "pw", 4);
game.AddPlayer("second player", "pw");
Console.WriteLine(game.ToJSON());
Assert.AreEqual(game.Players[1], "second player");
