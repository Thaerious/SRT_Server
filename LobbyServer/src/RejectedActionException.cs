using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frar.LobbyServer;

public class RejectedActionException : Exception {
    public String action = "";

    public RejectedActionException() {}

    public RejectedActionException(string action) {
        this.action = action;
    }

    public RejectedActionException(string action, string message)
        : base(message) {
        this.action = action;
    }

    public RejectedActionException(string action, string message, Exception inner)
        : base(message, inner) {
        this.action = action;
    }

    public JObject ToJSON() {
        return Events.ActionRejected(this.action, this.Message);
    }
}

public class UsernameInUseException : RejectedActionException{
    public UsernameInUseException(string action) : base(action, "username already in use") {}
}

public class MalformedActionException : RejectedActionException{
    public MalformedActionException(string action) : base(action, "malformed action") {}
}

public class GameNameInUseException : RejectedActionException{
    public GameNameInUseException() : base(action: "", message: "game name already in use") {}
    public GameNameInUseException(string action) : base(action, "game name already in use") {}
}

public class PlayerInGameException : RejectedActionException{
    public PlayerInGameException() : base(action: "", message: "player is already in a game") {}
    public PlayerInGameException(string action) : base(action, "player is already in a game") {}
}

public class UnknownGameException : RejectedActionException{
    public UnknownGameException() : base(action: "", message: "game not found") {}
    public UnknownGameException(string action) : base(action, "game not found") {}
}

public class UnknownPlayerException : RejectedActionException {
    public UnknownPlayerException(string action, string pName) : base(action: "", message: $"player not found {pName}") {}
    public UnknownPlayerException(string pName) : base(action: "", message: $"player not found {pName}") {}
}