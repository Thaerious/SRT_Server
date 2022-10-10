using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace frar.LobbyServer;

public class MaxPlayersException : Exception {
    public MaxPlayersException(int value) : base($"max players must be 2 <= x <= 5, {value} is out of bounds") { }
}

public class GameFullException : Exception {
    public GameFullException() : base("Game can not accept more players.") { }
}

public class RepeatedPlayerException : Exception {
    public RepeatedPlayerException(string name) : base($"Player {name} already added.") { }
}

public class RemoveOwnerException : Exception {
    public RemoveOwnerException(string name) : base($"Can not remove owner, {name}, from game.") { }
}

public class Game {
    public readonly string Name;
    public readonly string Owner;
    public readonly string Password;
    public readonly int MaxPlayers;

    private readonly List<string> _invited = new List<string>();
    public string[] Invited {
        get {
            return _invited.ToArray();
        }
    }

    private List<string> _players = new List<string>();
    public string[] Players {
        get {
            return _players.ToArray();
        }
    }

    public Game(string name, string owner, string password, int maxplayers) {
        if (maxplayers < 2 || maxplayers > 5) throw new MaxPlayersException(maxplayers);

        this.Name = name;
        this.Owner = owner;
        this.Password = password;
        this.MaxPlayers = maxplayers;
        this._players.Add(owner);
    }

    /// <summary>
    /// Add a new player to the the game.
    /// Can not add a player that has already been added.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="password">Plaintext PW must match PW game was made with</param>
    /// <returns>true if the player was added to the game, else false.</returns>
    /// <exception cref="RepeatedPlayerException">The player is already in the game.</exception>
    /// <exception cref="GameFullException">The game already has max players.</exception>
    /// 
    public bool AddPlayer(string name, string password = "") {
        if (!this._invited.Contains(name) && this.Password != password) return false;
        if (this._players.Contains(name)) throw new RepeatedPlayerException(name);
        if (_players.Count >= MaxPlayers) throw new GameFullException();
        this._players.Add(name);
        return true;
    }

    public void RemovePlayer(string name) {
        if (name == this.Owner) throw new RemoveOwnerException(name);
        if (!_players.Contains(name)) throw new UnknownPlayerException(name);
        _players.Remove(name);
    }

    public void AddInvite(string name) {
        if (this._invited.Contains(name)) throw new RepeatedPlayerException(name);
        this._invited.Add(name);
    }

    public void RemoveInvite(string name) {
        if (!_invited.Contains(name)) throw new UnknownPlayerException(name);
        _invited.Remove(name);
    }

    public JObject ToJSON() {
        return new JObject(
            new JProperty("name", Name),
            new JProperty("password", (Password != "")),
            new JProperty("max_players", MaxPlayers),
            new JProperty("players",
                new JArray(
                    from p in Players where p != null select p
                )
            )
        );
    }

    public bool HasPlayer(string pName) {
        return Players.Contains(pName);
    }
}