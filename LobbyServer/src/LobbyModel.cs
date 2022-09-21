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

// Data class for storing a game in the LobbyModel
public class Game {
    public readonly string name;
    public readonly Guid gamehash;
    public readonly Guid ownerhash;
    public readonly string password;
    public readonly int maxplayers;
    public readonly Guid[] playerhashes;
    public readonly List<Guid> invited = new List<Guid>();

    public Game(string name, Guid gamehash, Guid ownerhash, string password, int maxplayers) {
        this.name = name;
        this.gamehash = gamehash;
        this.ownerhash = ownerhash;
        this.password = password;
        this.maxplayers = maxplayers;
        playerhashes = new Guid[maxplayers];
        playerhashes[0] = ownerhash;
    }
}

// Model for storing the current state of the Lobby.
// Any exceptions throws should be forwarded as ActionRejected events.
public class LobbyModel {
    private readonly Dictionary<Guid, Player> playerHashes = new Dictionary<Guid, Player>();
    private readonly Dictionary<Guid, Game> gameHashes = new Dictionary<Guid, Game>();
    private readonly Dictionary<String, Player> playerNames = new Dictionary<String, Player>();
    private readonly Dictionary<String, Game> gameNames = new Dictionary<String, Game>();

    // Add a new player to the players list
    // Returns: the player hash.
    public Player AddPlayer(string name){
        if (playerNames.ContainsKey(name)){
            throw new RejectedActionException("Player name already in use.");
        }

        Player player = new Player(name, Guid.NewGuid());
        playerHashes.Add(player.playerhash, player);
        playerNames.Add(player.name, player);

        return player;
    }

    public void RegisterPlayer(string name, string password){
        
    }
}