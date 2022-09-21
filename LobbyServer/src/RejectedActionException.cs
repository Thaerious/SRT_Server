using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frar.LobbyServer;

public class RejectedActionException : Exception {
    String action = "";

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
        JObject json = new JObject(
            new JProperty("event", "action_rejected"),            
            new JProperty("action", this.action),
            new JProperty("message", this.Message)
        );
        return json;
    }
}

public class UsernameInUseException : RejectedActionException{
    public UsernameInUseException(string action) : base(action, "username already in use") {}
}

public class MalformedActionException : RejectedActionException{
    public MalformedActionException(string action) : base(action, "malformed action") {}
}