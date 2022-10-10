using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;
using System.Diagnostics;

LobbyModel lobbyModel = new LobbyModel();
lobbyModel.CreateGame("Adam's Game", "Adam", "password", 4);

foreach (Game game in lobbyModel.Games) {
    Console.WriteLine(game.ToJSON());
}

Console.WriteLine(lobbyModel.PlayerHasGame("Adam"));

lobbyModel.CreateGame("Adam's Other Game", "Adam", "password", 4);