using Newtonsoft.Json;
using System.Reflection;
using frar.clientserver;
using System;
namespace frar.clientserver.test;

public class Demo{
    static void Main(string[] args) {
        var mi = AttributeParser.SeekRoutes(new Demo());
        foreach (RouteEntry entry in mi) {
            Console.WriteLine(entry);
        }
    }

    [Route]
    public void imaRoute() { }

    [Route]
    private void uraRoute() { }

    [Route]
    public static void theyRoute() { }
}
