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

    public void Start(){        
        this.MainLoop();
    }

    private void StartReadThread(){
        this.thread = new Thread(new ThreadStart(() => {
            do{
                JObject json = this.client.ReadJSON();
                Console.WriteLine(json.ToString());
            }while(running);
        }));

        thread.Start();
    }

    private void MainLoop() {
        do {
            Console.Write("> ");
            string? line = Console.In.ReadLine();
            if (line == null) continue;
            string[] split = regex.Split(line);

            for (int i = 0; i < split.Length; i++){
                if (split[i] == "''") split[i] = "";
            }

            switch (split[0].ToLower()) {
                case "exit":
                    this.client.Close();
                    this.running = false;
                    break;
                case "connect":                    
                    Connect(split);                    
                    break;
                case "r":
                case "register":
                case "register_player":
                    Register(split);
                    break;

            }
        } while (this.running);
    }

    private void Connect(string[] split){
        if (this.client.Connected) return;
        this.client.Connect(ip: "127.0.0.1", port: 7000);
        StartReadThread();
    }

    private void Register(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "register_player"),
            new JProperty("username", split[1] != null ? split[1] : ""),
            new JProperty("password", split[2] != null ? split[2] : ""),
            new JProperty("email", split[3] != null ? split[3] : "")
        ));
    }    

    private void DeletePlayer(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "delete_player"),
            new JProperty("password", split[2] != null ? split[2] : "")
        ));
    }





    private void Login(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "login"),
            new JProperty("username", split[1] != null ? split[1] : ""),
            new JProperty("password", split[2] != null ? split[2] : "")
        ));
    }     

    private void Logout(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "logout")
        ));
    }          

    private void JoinGame(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "join_game"),
            new JProperty("gamehash", split[1] != null ? split[1] : ""),
            new JProperty("password", split[2] != null ? split[2] : "")            
        ));
    }   

    private void CreateGame(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "create_game"),
            new JProperty("gamename", split[1] != null ? split[1] : ""),
            new JProperty("maxplayers", split[2] != null ? Int32.Parse(split[2]) : 0),
            new JProperty("password", split[2] != null ? split[3] : "")  
        ));
    }   

    private void LeaveGame(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "leave_game"),
            new JProperty("gamehash", split[1] != null ? split[1] : "")
        ));
    }        

    private void RemovePlayer(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "remove_player"),
            new JProperty("gamehash", split[1] != null ? split[1] : ""),
            new JProperty("playername", split[1] != null ? split[2] : "")
        ));
    }              

    private void StartGame(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "start_game"),
            new JProperty("gamehash", split[1] != null ? split[1] : "")
        ));
    }   

    private void InvitePlayer(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "invite_player"),
            new JProperty("gamehash", split[1] != null ? split[1] : ""),
            new JProperty("playername", split[1] != null ? split[2] : "")
        ));
    }   

    private void RequestPlayers(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "request_players")
        ));
    }

    private void RequestGames(string[] split){
        this.client.WriteJSON(new JObject(
            new JProperty("action", "request_games")
        ));
    }                    
}







