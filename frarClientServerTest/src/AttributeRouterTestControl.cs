using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.clientserver;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace frar.frarClientServerTest;

// The router control object can be used to terminate the current route chain.
[TestClass]
public class TestCtrl : ThreadedRouter {
    public string hashString = "";   

    [TestMethod]
    public void Rule_Order() {
        this.hashString = "";

        this.Process(Packet.FromString(@"{
            'Action' : 'setvalue',
            'Parameters' : {
                'value' : 'middle'
            }
        }"));

        Assert.AreEqual("before-middle", this.hashString);
    }

    // Route with a negative index get's called first.
    [Route(Rule = ".*", Index = -1)]
    public void Before(string value){
        this.hashString += "before-";
    }

    // This route terminates the chain.
    [Route(Rule = ".*", Index = 0)]
    public void SetValue(string value, [Ctrl] RouterController ctrl){
        this.hashString += value;
        ctrl.TerminateRoute = true;
    }

    // Route with a positive index does not get called.
    [Route(Rule = ".*", Index = 1)]
    public void After(string value){
        this.hashString += "-after";
    }
}