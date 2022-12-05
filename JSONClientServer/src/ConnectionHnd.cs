namespace frar.JSONServer;

// Any class that the server ivokes upon connection must implement this interface.
// The server will call the OnConnect method whenever a new connection is made.
public interface ConnectionHnd{
    void OnConnect(Connection connection);
}