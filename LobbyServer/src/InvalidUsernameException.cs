namespace frar.LobbyServer;

public class InvalidUsernameException : Exception {
    public InvalidUsernameException(string username) : base($"invalid username {username}"){}
}