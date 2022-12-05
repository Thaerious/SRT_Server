using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.JSONServer;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frar.JSONClientServerTest;


// Annotating a JObject parameter with [Req] will load
// that parameter with the json request object.
// If this object is modified, the modifications will survive to
// future routes.
[TestClass]
public class TestReq : AttributeRouter {
    public string hashString = "";   
    public string isString = "";   

    public TestReq() {
        this.Initialize();
    }

    [TestMethod]
    public void Rule_Order() {
        this.hashString = "";
        this.isString = "";

        this.Process(@"{
            'action' : 'setvalue',
            'parameters' : {
                'value' : 'middle'
            }
        }");

        // The appended field is preserved
        Assert.AreEqual("af59b2", this.hashString);

        // The new route is called, the old route is not.
        Assert.AreEqual("old", this.isString);        
    } 

    // Add a field to the req object.
    [Route(Rule = ".*", Index = 0)]
    public void Before(string value, [Req]JObject req){
        req["hash"] = "af59b2";
    }

    // The appended field is preserved
    [Route(Rule = ".*", Index = 1)]
    public void After([Req]JObject req){
        this.hashString += req["hash"];
    }

    // Change the action string
    [Route(Rule = ".*", Index = 2)]
    public void ChangeAction([Req]JObject req){
        req["action"] = "newaction";
    }

    // The changed action doesn't affect routing.
    [Route(Index = 3)]
    public void NewAction([Req]JObject req){
        this.isString += "new";
    }    

    // The changed action doesn't affect routing.
    [Route(Index = 4)]
    public void SetValue([Req]JObject req){
        Debug.WriteLine(req);
        this.isString += "old";
    }
}