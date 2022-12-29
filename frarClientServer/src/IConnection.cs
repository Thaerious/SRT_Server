namespace frar.clientserver;

public interface IConnection {
    Packet Read();
    void Shutdown();
    void Write(Packet packet);
}

// [1] https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.connect?view=net-7.0