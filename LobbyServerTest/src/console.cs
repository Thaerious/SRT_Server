using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using frar.LobbyServer;
using System.Diagnostics;
using System.IO;

var x = System.AppDomain.CurrentDomain.BaseDirectory;
var y = Environment.CurrentDirectory;
Console.WriteLine(x);
Console.WriteLine(y);

LobbyModel lobbyModel = new LobbyModel();
lobbyModel.AddPlayer("Adam");
lobbyModel.CreateGame("Adam's Game", "Adam", "password", 4);
lobbyModel.GetGameByPlayer("Adam");