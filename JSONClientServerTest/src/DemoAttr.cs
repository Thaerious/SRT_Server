using System.Net.Sockets;
using frar.JSONServer;
using Newtonsoft.Json.Linq;
namespace frar.JSONClientServerTest;

public class DemoAttr {
    static void Main(string[] args) {
        System.Console.WriteLine("Demo Attribute Handler");
        MyRouter myRouter = new MyRouter();
        myRouter.Initialize();

        System.Console.WriteLine("Routes\n------------");
        foreach (RouteEntry route in myRouter.Routes){
            System.Console.WriteLine(route);
        }
        System.Console.WriteLine("\n");

        var json = JObject.Parse(@"{
            'action' : 'login',
            'parameters' : {
                'username' : 'ima user'
            }
        }");

        myRouter.Process(json);
    }
}

public class MyRouter : AttributeRouter {

    [Route]
    public void Login(string username) {
        System.Console.WriteLine($"login: {username}");
    }

    [Route(".*", Index = -1)]
    public void Log([Req]JObject json) {
        System.Console.WriteLine($"log: {json["action"]}");
        json["hash"] = new JValue("ab90df");
    }

    [Route(".*", Index = 1)]
    public void ReLog([Req]JObject json) {
        System.Console.WriteLine(json);
    }
}