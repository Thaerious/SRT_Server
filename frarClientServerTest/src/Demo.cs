using Newtonsoft.Json;
namespace frar.clientserver.test;


public class Demo{
    static void Main(string[] args) {
        string s = "[1, 2, 3]";
        var x = JsonConvert.DeserializeObject(s, typeof(int[]));
        System.Console.WriteLine(x.GetType());
    }
}
