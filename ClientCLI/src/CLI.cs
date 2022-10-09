using frar.LobbyServer;
using frar.JSONServer;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

class CLI {
    private bool running = true;
    Regex regex = new Regex("[ ]+");
    private Client client = new Client();
    private Thread? thread;

    public void Start() {
        this.MainLoop();
    }

    private void StartReadThread() {
        this.thread = new Thread(new ThreadStart(() => {
            do {
                JObject json = this.client.ReadJSON();
                Console.WriteLine($"client : {json.ToString()}");
                if (!this.client.Connected) running = false;
            } while (running);
        }));

        thread.Start();
    }

    private void MainLoop() {
        do {
            Console.Write("> ");
            string? line = Console.In.ReadLine();
            if (line == null) continue;
            string[] split = regex.Split(line);

            for (int i = 0; i < split.Length; i++) {
                if (split[i] == "''") split[i] = "";
            }

            switch (split[0].ToLower()) {
                case "x":
                case "q":
                case "quit":
                case "exit":
                    this.client.Close();
                    this.running = false;
                    break;
                case "c":
                case "connect":
                    Connect(split);
                    break;
                case "a":
                case "register":
                    Register(split);
                    break;
                case "l":
                case "login":
                    Login(split);
                    break;
                case "o":
                case "logout":
                    Logout(split);
                    break;
                case "j":
                case "join":
                    JoinGame(split);
                    break;
                case "n":
                case "new":
                    CreateGame(split);
                    break;       
                case "v":
                case "leave":
                    LeaveGame(split);
                    break;   
                case "r":
                case "remove":
                    LeaveGame(split);
                    break;   
                case "i":
                case "invite":
                    InvitePlayer(split);
                    break;    
                case "p":
                case "players":
                    RequestPlayers(split);
                    break;   
                case "g":
                case "games":
                    RequestGames(split);
                    break;                                                                                                                     

            }
        } while (this.running);
    }

    private void Connect(string[] split) {
        if (this.client.Connected) {
            Console.WriteLine("client already connected");
            return;
        }

        if (split.Length != 1) {
            Console.WriteLine("usage:\n  c, connect");
            return;
        }

        if (this.client.Connected) return;
        this.client.Connect(ip: "127.0.0.1", port: 7000);
        StartReadThread();
    }

    private void Register(string[] split) {
        if (!this.client.Connected) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 4) {
            Console.WriteLine("usage:\n  a, register [username] [password] [email]");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "register_player"),
            new JProperty("username", split[1] != null ? split[1] : ""),
            new JProperty("password", split[2] != null ? split[2] : ""),
            new JProperty("email", split[3] != null ? split[3] : "")
        ));
    }

    private void DeletePlayer(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "delete_player"),
            new JProperty("password", split[2] != null ? split[2] : "")
        ));
    }

    private void Login(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 3) {
            Console.WriteLine("usage:\n  l, login [username] [password]");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "login"),
            new JProperty("username", split[1] != null ? split[1] : ""),
            new JProperty("password", split[2] != null ? split[2] : "")
        ));
    }

    private void Logout(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 1) {
            Console.WriteLine("usage:\n  o, logout");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "logout")
        ));
    }

    private void JoinGame(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length < 2 || split.Length > 3) {
            Console.WriteLine("usage:\n  j, join [gamehash] [password?]");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "join_game"),
            new JProperty("gamehash", split[1] != null ? split[1] : ""),
            new JProperty("password", split[2] != null ? split[2] : "")
        ));
    }

    private void CreateGame(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length < 3 || split.Length > 4) {
            Console.WriteLine("usage:\n  n, new [gamename] [maxplayers] [password?]");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "create_game"),
            new JProperty("gamename", split[1] != null ? split[1] : ""),
            new JProperty("maxplayers", split[2] != null ? Int32.Parse(split[2]) : 0),
            new JProperty("password", split[3] != null ? split[3] : "")
        ));
    }

    private void LeaveGame(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 2) {
            Console.WriteLine("usage:\n  v, leave [gamehash]");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "leave_game"),
            new JProperty("gamehash", split[1] != null ? split[1] : "")
        ));
    }

    private void RemovePlayer(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 3) {
            Console.WriteLine("usage:\n  r, remove [gamehash] [playername]");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "remove_player"),
            new JProperty("gamehash", split[1] != null ? split[1] : ""),
            new JProperty("playername", split[2] != null ? split[2] : "")
        ));
    }

    private void StartGame(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 2) {
            Console.WriteLine("usage:\n  s, start [gamehash]");
            return;
        }


        this.client.WriteJSON(new JObject(
            new JProperty("action", "start_game"),
            new JProperty("gamehash", split[1] != null ? split[1] : "")
        ));
    }

    private void InvitePlayer(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 2) {
            Console.WriteLine("usage:\n  i, invite [gamehash] [playername]");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "invite_player"),
            new JProperty("gamehash", split[1] != null ? split[1] : ""),
            new JProperty("playername", split[1] != null ? split[2] : "")
        ));
    }

    private void RequestPlayers(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 1) {
            Console.WriteLine("usage:\n  p, players");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "request_players")
        ));
    }

    private void RequestGames(string[] split) {
        if (this.client == null) {
            Console.WriteLine("client not connected");
            return;
        }

        if (split.Length != 1) {
            Console.WriteLine("usage:\n  g, games");
            return;
        }

        this.client.WriteJSON(new JObject(
            new JProperty("action", "request_games")
        ));
    }
}







