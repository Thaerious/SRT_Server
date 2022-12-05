using frar.JSONServer;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace frar.LobbyServer;

public class ClientConnectedException : Exception {
    public ClientConnectedException() : base("Client already connected.") { }
}

public class ClientConnector : ClientPublisher {
    private Thread? thread;

    public Client Client { get; } = new Client();
    public bool Running { get; private set; } = false;

    public void Connect(string ip, int port) {
        if (this.Client.Connected) throw new ClientConnectedException();
        this.Client.Connect(ip, port);
        StartReadThread();
    }

    private void StartReadThread() {
        this.Running = true;

        this.thread = new Thread(new ThreadStart(() => {
            do {
                JObject json = this.Client.ReadJSON();
                base.RaiseClientEvent(json);
                if (!this.Client.Connected) this.Running = false;
            } while (this.Running);
        }));

        thread.Start();
    }
}