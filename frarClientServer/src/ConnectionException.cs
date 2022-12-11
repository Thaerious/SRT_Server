namespace frar.clientserver;

/// <summary>
/// Thrown by the Connection class.
/// </summary>
public class ConnectionException : Exception {

    public ConnectionException() {}

    public ConnectionException(string message)
        : base(message) {}

    public ConnectionException(string message, Exception inner)
        : base(message, inner) {}
}
