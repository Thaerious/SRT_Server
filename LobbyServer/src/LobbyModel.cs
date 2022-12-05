namespace frar.LobbyServer;

// Data class for storing a player in the LobbyModel
public class Player {
    public readonly string Name;

    public Player(string name) {
        this.Name = name;
    }
}

// Model for storing the current state of the Lobby.
// Any exceptions throws should be forwarded as ActionRejected events.
public class LobbyModel {
    private readonly Dictionary<String, Player> playerNames = new Dictionary<String, Player>();
    private readonly Dictionary<String, Game> games = new Dictionary<String, Game>();

    public ICollection<string> Players {
        get {
            return playerNames.Keys;
        }
    }

    public ICollection<Game> Games {
        get {
            return games.Values;
        }
    }

    // Add a new player to the players list
    // Returns: the player hash.
    public Player AddPlayer(string name) {
        if (playerNames.ContainsKey(name)) {
            throw new RejectedActionException("Player name already in use.");
        }

        Player player = new Player(name);
        playerNames.Add(player.Name, player);

        return player;
    }

    /// <summary>
    /// Remove a player from the lobby.
    /// </summary>
    /// <param name="name">The name of the player to remove</param>
    /// <exception cref="UnknownPlayerException">Thrown when the player does not exist.</exception>
    public void RemovePlayer(string name) {
        if (!playerNames.ContainsKey(name)) {
            throw new UnknownPlayerException(name);
        }

        playerNames.Remove(name);
    }

    public Game CreateGame(string gName, string pName, string password, int maxplayers) {
        if (!this.HasPlayer(pName)) throw new UnknownPlayerException(pName);
        if (this.PlayerHasGame(pName)) throw new PlayerInGameException();
        if (games.ContainsKey(gName)) throw new GameNameInUseException();

        Game game = new Game(gName, pName, password, maxplayers);
        this.games.Add(gName, game);
        return game;
    }

    /// <summary>
    /// Retrieve a game by name.
    /// </summary>
    /// <param name="gName">The name of the game.</param>
    /// <exception cref="UnknownGameException">The game has not been created.</exception> 
    /// 
    public Game GetGame(string gName) {
        if (!this.games.ContainsKey(gName)) throw new UnknownGameException();
        return this.games[gName];
    }

    /// <summary>
    /// Retrieve a game, if it exists.
    /// Throws UnknownGameException if the player is not in a game.
    /// </summary>
    /// <param name="pName"></param>
    /// <returns></returns>
    /// 
    public Game GetGameByPlayer(string pName) {
        if (!this.HasPlayer(pName)) throw new UnknownPlayerException(pName);
        foreach (Game game in this.Games) {
            if (game.HasPlayer(pName)) return game;
        }
        throw new UnknownGameException();
    }

    /// <summary>
    /// True if a player is in a game.
    /// </summary>
    /// <param name="pName"></param>
    /// <returns></returns>
    public bool PlayerHasGame(string pName) {
        foreach (Game game in this.Games) {
            if (game.HasPlayer(pName)) return true;
        }
        return false;
    }

    public bool HasPlayer(string pName) {
        return this.playerNames.ContainsKey(pName);
    }

    public bool HasGame(string name) {
        return this.games.ContainsKey(name);
    }
}