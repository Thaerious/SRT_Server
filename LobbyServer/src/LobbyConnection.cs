using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using frar.JSONServer;

namespace frar.LobbyServer;

/// <summary>
/// Logic handling class for the lobby server.
/// All incoming communication from a connection get's handled by this class.
/// The shared connection with other parts of the server is done through he 
/// ConnectionManager class.
/// </summary>
public class LobbyConnection {
    private string username = "";
    private readonly ConnectionManager connectionManager;
    private readonly Connection connection;
    private string status = "disconnected";
    private Thread thread = null!;
    private bool isRunning = true;

    public LobbyConnection(ConnectionManager connectionManager, Connection connection) {
        this.connectionManager = connectionManager;
        this.connection = connection;
    }

    public void Start() {
#pragma warning disable CS0168
        this.status = "connected";
        this.thread = new Thread(new ThreadStart(() => {
            while (isRunning) {
                try {
                    JObject jObject = this.connection.ReadJSON();
                    Console.WriteLine("server : " + jObject.ToString());
                    this.ProcessPacket(jObject);
                } catch (ConnectionException ex) {
                    this.isRunning = false;
                }
            }
        }));
#pragma warning restore CS0168

        thread.Start();
    }

    public void Stop() {
#pragma warning disable CS0168
        try {
            this.isRunning = false;
            this.connection.Close();
        } catch (ObjectDisposedException ex) {
            Console.WriteLine(ex);
            // do nothing
        }
#pragma warning restore CS0168
    }

    public void ProcessPacket(JObject jObject) {
        string action = "";
        try {
            if (jObject["action"] != null) action = (string)jObject["action"]!;

            switch (action) {
                case "register_player":
                    this.RegisterPlayer(jObject);
                    break;
                case "delete_player":
                    this.DeletePlayer(jObject);
                    break;
                case "recover_password":
                    this.RecoverPassword(jObject);
                    break;
                case "login":
                    this.Login(jObject);
                    break;
                case "logout":
                    this.Logout(jObject);
                    break;
                case "join_game":
                    this.JoinGame(jObject);
                    break;
                case "create_game":
                    this.CreateGame(jObject);
                    break;
                case "leave_game":
                    this.LeaveGame(jObject);
                    break;
                case "delete_game":
                    this.DeleteGame(jObject);
                    break;
                case "remove_player":
                    this.RemovePlayer(jObject);
                    break;
                case "start_bame":
                    this.StartGame(jObject);
                    break;
                case "chat":
                    this.Chat(jObject);
                    break;
                case "request_players":
                    this.RequestPlayers(jObject);
                    break;
                case "request_games":
                    this.RequestGames(jObject);
                    break;
                default:
                    break;
            }
        } catch (RejectedActionException ex) {
            this.connection.WriteJSON(ex.ToJSON());
        } catch (NotImplementedException ex) {
            JObject json = new RejectedActionException(action, "action not implemented").ToJSON();
            this.connection.WriteJSON(json);
            Console.WriteLine(ex.Message);
        } catch (Exception ex) {
            Console.WriteLine(ex.ToString());
        }
    }

    private void RegisterPlayer(JObject jObject) {
        if (status == "disconnected") return;

        if (!jObject.EnsureKeys("username", "password", "email")) {
            throw new MalformedActionException("register_player");
        }

        bool r = this.connectionManager.userManager.AddUser(
            (string)jObject["username"]!,
            (string)jObject["password"]!,
            (string)jObject["email"]!
        );

        if (!r) {
            connection.WriteJSON(Events.ActionRejected("register_player"));
        } else {
            connection.WriteJSON(Events.ActionSuccess("register_player"));
        }
    }

    private void Login(JObject jObject) {
        if (status == "disconnected") return;

        if (!jObject.EnsureKeys("username", "password")) {
            throw new MalformedActionException("login");
        }

        bool r = this.connectionManager.userManager.VerifyUser(
            (string)jObject["username"]!,
            (string)jObject["password"]!
        );

        if (!r) {
            connection.WriteJSON(Events.ActionRejected("login", "incorrect credentials"));
        } else {
            connection.WriteJSON(Events.UJoinLobby((string)jObject["username"]!));
            this.username = (string)jObject["username"]!;
            this.connectionManager.lobbyModel.AddPlayer(this.username);
            this.connectionManager.Broadcast(Events.PJoinLobby((string)jObject["username"]!));
        }
    }

    private void Logout(JObject jObject) {
        if (status == "disconnected") return;

        connection.WriteJSON(Events.ULogout((string)jObject["username"]!));
        this.connectionManager.lobbyModel.RemovePlayer(this.username);
        this.connectionManager.Broadcast(Events.PLogout((string)jObject["username"]!));
    }

    private void JoinGame(JObject jObject) {
        if (status == "disconnected") return;

        if (!jObject.EnsureKeys("gamehash", "password")) {
            throw new MalformedActionException("join_game");
        }

        this.connectionManager.lobbyModel.RemovePlayer(this.username);
        this.connection.WriteJSON(Events.UJoinGame((string)jObject["gamehash"]!));
        this.connectionManager.Broadcast(Events.PJoinGame(this.username, (string)jObject["gamehash"]!));
    }

    private void CreateGame(JObject jObject) {
        if (status == "disconnected") return;

        if (!jObject.EnsureKeys("gamename", "maxplayers", "password")) {
            throw new MalformedActionException("create_game");
        }

        string gName = (string)jObject["gamename"]!;
        if (this.connectionManager.lobbyModel.ContainsGame(gName)) {
            throw new GameNameInUseException("create_game");
        }

        this.connectionManager.lobbyModel.RemovePlayer(this.username);
        this.connection.WriteJSON(Events.UJoinGame(gName));
        this.connectionManager.Broadcast(Events.PJoinGame(this.username, gName));
    }

    private void DeletePlayer(JObject jObject) { throw new NotImplementedException(); }
    private void RecoverPassword(JObject jObject) { throw new NotImplementedException(); }
    private void LeaveGame(JObject jObject) { throw new NotImplementedException(); }
    private void DeleteGame(JObject jObject) { throw new NotImplementedException(); }
    private void RemovePlayer(JObject jObject) { throw new NotImplementedException(); }
    private void StartGame(JObject jObject) { throw new NotImplementedException(); }
    private void Chat(JObject jObject) { throw new NotImplementedException(); }

    private void RequestPlayers(JObject jObject) {
        if (status == "disconnected") return;
        var players = this.connectionManager.lobbyModel.Players;
        connection.WriteJSON(Events.Players(players));
    }

    private void RequestGames(JObject jObject) {
        if (status == "disconnected") return;
        ICollection<Game> games = this.connectionManager.lobbyModel.Games;
        connection.WriteJSON(Events.Games(games));
    }
}