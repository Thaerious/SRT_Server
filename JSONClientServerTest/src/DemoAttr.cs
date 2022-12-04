using System.Net.Sockets;
using frar.JSONServer;
using Newtonsoft.Json.Linq;
namespace frar.JSONClientServerTest;

public class DemoAttr {
    static void Main(string[] args) {
        long isLong = 1;
        System.Console.WriteLine(isLong.GetType());
        int isInt32 = System.Convert.ToInt32(isLong);
        System.Console.WriteLine(isInt32.GetType());
        System.Console.WriteLine(isInt32);
        // System.Console.WriteLine("Demo Attribute Handler");
        // MyRouter myRouter = new MyRouter();
        // myRouter.Initialize();

        // System.Console.WriteLine("Routes\n------------");
        // foreach (RouteEntry route in myRouter.Routes){
        //     System.Console.WriteLine(route);
        // }
        // System.Console.WriteLine("\n");

        // var json = JObject.Parse(@"{
        //     'action' : 'login',
        //     'parameters' : {
        //         'username' : 'ima user'
        //     }
        // }");

        // myRouter.Process(json);
    }
}

public class MyRouter : AttributeRouter {

    [Route]
    public void Login(string username) {
        System.Console.WriteLine($"login: {username}");
    }

    [Route("apple", Index = 2)]
    public void Apple() {

    }

    [Route("zebra", Index = 1)]
    public void Who() {

    }


    [Route("aardvark", Index = 1)]
    public void What() {

    }

    [Route("llama")]
    public void Where() {

    }
}