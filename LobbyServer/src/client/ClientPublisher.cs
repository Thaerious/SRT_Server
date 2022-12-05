using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace frar.LobbyServer;

public class ClientEventArgs {
    public ClientEventArgs(JObject json) { JSON = json; }
    public JObject JSON { get; }
}

public class ClientPublisher {
    public delegate void ClientEventHandler(ClientEventArgs args);
    public event ClientEventHandler OnEvent;

    protected virtual void RaiseClientEvent(JObject json) {
        OnEvent?.Invoke(new ClientEventArgs(json));
    }
}