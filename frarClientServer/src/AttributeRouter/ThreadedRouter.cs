namespace frar.clientserver;

public enum DISCONNECT_REASON {
    broken,
    gracefull
};

/// <summary>
/// Listens for incoming packets on it's own thread.
/// Extend this class and add annotations to dictate behaviour.
/// The accepted method annotations are: OnConnect, OnDisconnect, Route.
/// </summary>
public abstract class ThreadedRouter : Router {
    private Thread thread = null!;
    private bool isRunning = true;

    [OnConnect]
    protected override void InvokeConnect(IConnection connection) {
        base.InvokeConnect(connection);

        this.thread = new Thread(
            new ThreadStart(() => {
                while (isRunning) {
                    try {
                        Packet packet = connection.Read();
                        if (packet == null) {
                            this.isRunning = false;
                            this.InvokeDisconnect(DISCONNECT_REASON.broken);
                        }
                        else {
                            this.Process(packet);
                        }
                    }
                    catch (ConnectionException) {
                        if (this.isRunning) {
                            this.isRunning = false;
                            this.InvokeDisconnect(DISCONNECT_REASON.broken);
                        }
                    }
                }
            })
        );
        thread.Start();
    }

    /// <summary>
    /// Stops listening for new packets and shuts down the underlying connection.
    /// </summary>
    public void ShutDown() {
        try {
            this.isRunning = false;
            this.Connection.Shutdown();
            this.InvokeDisconnect(DISCONNECT_REASON.gracefull);
        }
        catch (ObjectDisposedException) { }
    }
}