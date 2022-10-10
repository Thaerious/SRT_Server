using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace frar.LobbyServer;

public class Events {
    public static JObject ActionSuccess(string action) {
        return new JObject(
            new JProperty("event", "action_sucess"),
            new JProperty("action", action)
        );
    }

    public static JObject ActionRejected(string action, string message = "") {
        return new JObject(
            new JProperty("event", "action_rejected"),
            new JProperty("action", action),
            new JProperty("message", message)
        );
    }

    public static JObject UJoinLobby(string name) {
        return new JObject(
            new JProperty("event", "uJoinLobby"),
            new JProperty("name", name)
        );
    }

    public static JObject PJoinLobby(string name) {
        return new JObject(
            new JProperty("event", "pJoinLobby"),
            new JProperty("name", name)
        );
    }

    public static JObject ULogout(string name) {
        return new JObject(
            new JProperty("event", "uLogout"),
            new JProperty("name", name)
        );
    }   

    public static JObject PLogout(string name) {
        return new JObject(
            new JProperty("event", "pLogout"),
            new JProperty("name", name)
        );
    } 

    public static JObject ULeaveLobby(string name) {
        return new JObject(
            new JProperty("event", "uLogout")
        );
    }   

    public static JObject PLeaveLobby(string name) {
        return new JObject(
            new JProperty("event", "pLogout"),
            new JProperty("name", name)
        );
    } 

    // see https://www.newtonsoft.com/json/help/html/CreatingLINQtoJSON.htm
    public static JObject Players(ICollection<string> names) {
        return new JObject(
            new JProperty("event", "players"),
            new JProperty("names",
                new JArray(
                    from name in names select new JObject(
                        new JProperty("playerName", name)
                    )
                )
            )
        );
    }

    // see https://www.newtonsoft.com/json/help/html/CreatingLINQtoJSON.htm
    public static JObject Games(ICollection<Game> games) {
        return new JObject(
            new JProperty("event", "players"),
            new JProperty("names",
                new JArray(
                    from game in games select game.ToJSON()
                )
            )
        );
    } 

    public static JObject UJoinGame(string gamehash) {
        return new JObject(
            new JProperty("event", "uJoinGame"),
            new JProperty("gamehash", gamehash)
        );
    }                          

    public static JObject ULeaveGame(string gamehash) {
        return new JObject(
            new JProperty("event", "uLeaveGame"),
            new JProperty("gamehash", gamehash)
        );
    }               

    public static JObject UKick(string gamehash) {
        return new JObject(
            new JProperty("event", "uKick"),
            new JProperty("gamehash", gamehash)
        );
    }   

    public static JObject PLeaveGame(string name, string gamehash) {
        return new JObject(
            new JProperty("event", "uLeaveGame"),
            new JProperty("playername", name),
            new JProperty("gamehash", gamehash)
        );
    }       

    public static JObject Chat(string name, string message) {
        return new JObject(
            new JProperty("event", "chat"),
            new JProperty("playername", name),
            new JProperty("message", message)
        );
    }      

    public static JObject GameInvite(string from, string gamehash) {
        return new JObject(
            new JProperty("event", "gameInvite"),
            new JProperty("playername", from),
            new JProperty("gamehash", gamehash)
        );
    }       

    public static JObject PLeaveGame(string gamehash) {
        return new JObject(
            new JProperty("event", "uLeaveGame")
        );
    }        

    public static JObject PJoinGame(string playerName, string gamehash) {
        return new JObject(
            new JProperty("event", "pJoinGame"),
            new JProperty("name", playerName),
            new JProperty("gamehash", gamehash)
        );
    }        

    public static JObject StartGame(string ip, int port, string gamehash) {
        return new JObject(
            new JProperty("event", "startGame"),
            new JProperty("ip", ip),
            new JProperty("port", port),
            new JProperty("gamehash", gamehash)
        );
    }        

    public static JObject DeleteGame(string gamehash) {
        return new JObject(
            new JProperty("event", "deleteGame"),
            new JProperty("gamehash", gamehash)
        );
    }      

    public static JObject NewGame(Game game) {
        JObject json = game.ToJSON();
        json.Add("event", "newGame");
        return json;
    }          
}