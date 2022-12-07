using frar.JSONServer;

namespace frar.JSONClientServerTest;

public class Demo{
    static void Main(string[] args) {
        Packet packet = new Packet("some_action");
        packet.Parameters["value"] =  "a";
        System.Console.WriteLine(packet.ToString());
        Packet p2 = Packet.FromString(@"{""Action"":""some_action"",""Parameters"":{}}");
        System.Console.WriteLine(p2.ToString());
    }
}

