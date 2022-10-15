namespace frar.LobbyServer;

// Data class for storing a player in the LobbyModel
public class Player {
    public readonly string name;
    public readonly Guid playerhash;
    public string gamehash = "";

    public Player(string name, Guid playerhash) {
        this.name = name;
        this.playerhash = playerhash;
    }
}

// Model for storing the current state of the Lobby.
// Any exceptions throws should be forwarded as ActionRejected events.
public class LobbyModel {
    private readonly Dictionary<Guid, Player> playerHashes = new Dictionary<Guid, Player>();
    private readonly Dictionary<String, Player> playerNames = new Dictionary<String, Player>();
    private readonly Dictionary<String, Game> games = new Dictionary<String, Game>();

    public ICollection<string> Players {
        get {
            return playerNames.Keys;
        }
        private set { }
    }

    public ICollection<Game> Games {
        get {
            return games.Values;
        }
        private set { }
    }

    // Add a new player to the players list
    // Returns: the player hash.
    public Player AddPlayer(string name) {
        if (playerNames.ContainsKey(name)) {
            throw new RejectedActionException("Player name already in use.");
        }

        Player player = new Player(name, Guid.NewGuid());
        playerHashes.Add(player.playerhash, player);
        playerNames.Add(player.name, player);

        return player;
    }

    public void RemovePlayer(string name) {
        if (!playerNames.ContainsKey(name)) {
            throw new RejectedActionException("", "Unknown player name.");
        }

        playerNames.Remove(name);
    }

    public Game CreateGame(string name, string owner, string password, int maxplayers) {
        if (this.PlayerHasGame(owner)) throw new PlayerInGameException();
        if (games.ContainsKey(name)) throw new GameNameInUseException();

        Game game = new Game(name, owner, password, maxplayers);
        this.games.Add(name, game);
        return game;
    }

    /// <summary>
    /// Retrieve a game by name.
    /// </summary>
    /// <param name="gName">The name of the game.</param>
    /// <exception cref="UnknownGameException">The game has not been created.</exception> 
    /// 
    public Game GetGame(string gName){
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