using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using frar.JSONServer;

namespace frar.LobbyServer;

public class LobbyConnection {
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
        this.status = "connected";
        this.thread = new Thread(new ThreadStart(() => {
            while (isRunning) {
                try{
                    JObject jObject = this.connection.ReadJSON();
                    Console.WriteLine(jObject.ToString());
                    this.ProcessPacket(jObject);
                } catch (ConnectionException ex){
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null){
                        Console.WriteLine(" - " + ex.InnerException!.Message);
                    }
                    this.Stop();
                }
            }
        }));

        thread.Start();
    }

    public void Stop() {
        this.isRunning = false;
        this.connection.Close();
    }

    public void ProcessPacket(JObject jObject) {
        string action = "";
        try {
            if (jObject["action"] != null) action = (string)jObject["action"]!;
            Console.WriteLine($"> {action}");

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
                    Console.WriteLine("unknown action");
                    var raEx = new RejectedActionException(action, "unknown action");
                    this.connection.WriteJSON(raEx.ToJSON());
                    break;
            }
        } catch (RejectedActionException ex) {
            this.connection.WriteJSON(ex.ToJSON());
        } catch (NotImplementedException ex) {
            JObject json = new RejectedActionException(action, "action not implemented").ToJSON();
            this.connection.WriteJSON(json);
            Console.WriteLine(ex.Message);
        } catch (Exception ex){
            JObject json = new RejectedActionException(action, ex.Message).ToJSON();
            this.connection.WriteJSON(json);
            Console.WriteLine(ex.Message);
        }
    }

    private void RegisterPlayer(JObject jObject) {
        if (status == "disconnected") return;

        if (!jObject.EnsureKeys("username", "password", "email"))
            throw new MalformedActionException("register_player");

        bool r = this.connectionManager.userManager.AddUser(
            (string)jObject["username"]!,
            (string)jObject["password"]!,
            (string)jObject["email"]!
        );

        if (!r) throw new UsernameInUseException("register_player");

        connection.WriteJSON(new JObject(
            new JProperty("event", "action_success"),
            new JProperty("action", "register_player")
        ));
    }

    private void DeletePlayer(JObject jObject) { throw new NotImplementedException(); }
    private void RecoverPassword(JObject jObject) { throw new NotImplementedException(); }
    private void Login(JObject jObject) { throw new NotImplementedException(); }
    private void Logout(JObject jObject) { throw new NotImplementedException(); }
    private void JoinGame(JObject jObject) { throw new NotImplementedException(); }
    private void CreateGame(JObject jObject) { throw new NotImplementedException(); }
    private void LeaveGame(JObject jObject) { throw new NotImplementedException(); }
    private void DeleteGame(JObject jObject) { throw new NotImplementedException(); }
    private void RemovePlayer(JObject jObject) { throw new NotImplementedException(); }
    private void StartGame(JObject jObject) { throw new NotImplementedException(); }
    private void Chat(JObject jObject) { throw new NotImplementedException(); }
    private void RequestPlayers(JObject jObject) { throw new NotImplementedException(); }
    private void RequestGames(JObject jObject) { throw new NotImplementedException(); }
}