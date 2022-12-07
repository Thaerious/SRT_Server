using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.JSONServer;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace frar.JSONClientServerTest;


// Routes have a default index of zero.
// Routes are called in the acending order of their index.
// The lower the index, the earlier the route is called.
[TestClass]
public class TestRuleOrder : ThreadedAttributeRouter {
    public string hashString = "";

    public TestRuleOrder(){
        this.AddHandler(this);
    }

    [TestMethod]
    public void Rule_Order() {
        this.hashString = "";

        this.Process(Packet.FromString(@"{
            'Action' : 'setvalue',
            'Parameters' : {
                'value' : 'middle'
            }
        }"));

        Assert.AreEqual("before-middle-after", this.hashString);
    }

    // Route with a negative index get's called first.
    [Route(Rule = ".*", Index = -1)]
    public void Before(string value) {
        this.hashString += "before-";
    }

    // Route with a positive index get's called third.
    [Route(Rule = ".*", Index = 1)]
    public void After(string value) {
        this.hashString += "-after";
    }

    // Route with a default index get's called second.
    [Route]
    public void SetValue(string value) {
        this.hashString += value;
    }
}