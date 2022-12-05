using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.JSONServer;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frar.JSONClientServerTest;


// Routes have a default index of zero.
// Routes are called in the acending order of their index.
// The lower the index, the earlier the route is called.
[TestClass]
public class TestRuleOrder : AttributeRouter {
    public string hashString = "";   

    public TestRuleOrder() {
        this.Initialize();
    }

    [TestMethod]
    public void Rule_Order() {
        this.hashString = "";

        this.Process(@"{
            'action' : 'setvalue',
            'parameters' : {
                'value' : 'middle'
            }
        }");

        Assert.AreEqual("before-middle-after", this.hashString);
    }

    // Route with a negative index get's called first.
    [Route(Rule = ".*", Index = -1)]
    public void Before(string value){
        this.hashString += "before-";
    }

    // Route with a positive index get's called third.
    [Route(Rule = ".*", Index = 1)]
    public void After(string value){
        this.hashString += "-after";
    }

    // Route with a default index get's called second.
    [Route]
    public void SetValue(string value){
        this.hashString += value;
    }
}