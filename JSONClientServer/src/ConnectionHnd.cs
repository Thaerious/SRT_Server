namespace frar.JSONServer;

/// <summary>
/// Any class that the server invokes upon connection must implement this interface.
/// The server will call the OnConnect method whenever a new connection is made.
/// </summary>
public interface ConnectionHnd{

    /// <summary>
    /// Method called by Server when a new connection occurs.
    /// </summary>
    /// <param name="connection"></param>
    void OnConnect(Connection connection);
}