using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using frar.JSONServer;
using System.Threading;
namespace frar.LobbyServer;

/// <summary>
/// Connects to a Lobby Server and emits events.
/// This is the client-side interface for a LobbyConnection instance.
/// This class does not handle server responses, it only emits action
/// requests to the server.
/// 
/// Methods will fail by emitting an exception if the server connection
/// is compromised.
/// </summary>
class ClientController {
    private LobbyModel lobbyModel = new LobbyModel();

    public ClientController() { }

    public void Connect(string ip, int port) { throw new NotImplementedException(); }

    public void Register(string username, string password, string email) { throw new NotImplementedException(); }

    public void DeletePlayer(string username, string password) { throw new NotImplementedException(); }

    public void Login(string username, string password) { throw new NotImplementedException(); }

    public void Logout(string username, string password) { throw new NotImplementedException(); }

    public void RecoverPassword(string username) { throw new NotImplementedException(); }

    public void CreateGame(string gamename, string password, int maxplayers) { throw new NotImplementedException(); }

    public void RemovePlayer(string playername) { throw new NotImplementedException(); }

    public void InvitePlayer(string playername) { throw new NotImplementedException(); }

    public void RemoveInvite(string playername) { throw new NotImplementedException(); }

    public void DeleteGame() { throw new NotImplementedException(); }

    public void JoinGame(string gamename, string password) { throw new NotImplementedException(); }

    public void LeaveGame() { throw new NotImplementedException(); }

    public void StartGame() { throw new NotImplementedException(); }

    public void Chat(string message) { throw new NotImplementedException(); }
}