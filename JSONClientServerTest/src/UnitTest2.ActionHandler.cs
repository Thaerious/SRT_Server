using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.JSONServer;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSONClientServerTest;

[TestClass]
public class UnitTest2 {
    [TestMethod]
    public void Single_Handler() {
        bool pass = false;
        var ahnd = new ActionHandler();        

        ahnd.Use("test", (json, next)=>{
            pass = true;
        });

        var json = JObject.Parse("{'action' : 'test'}");
        ahnd.Process(json);

        Assert.IsTrue(pass);
    }

    [TestMethod]
    public void Extraneous_Next() {
        bool pass = false;
        var ahnd = new ActionHandler();        

        ahnd.Use("test", (json, next)=>{
            pass = true;
            next();
        });

        var json = JObject.Parse("{'action' : 'test'}");
        ahnd.Process(json);

        Assert.IsTrue(pass);
    }

    [TestMethod]
    public void Chain_Two_Declarations() {
        int pass = 0;
        var ahnd = new ActionHandler();        

        ahnd.Use("test", (json, next)=>{
            pass = (pass * 10) + 1;
            next();
        });

        ahnd.Use("test", (json, next)=>{
            pass = (pass * 10) + 2;
        });        

        var json = JObject.Parse("{'action' : 'test'}");
        ahnd.Process(json);

        Assert.AreEqual(12, pass);
    }

    [TestMethod]    
    public void Chain_Single_Declaration() {
        int pass = 0;
        var ahnd = new ActionHandler();        

        ahnd.Use("test", 
            (json, next)=>{
                pass = (pass * 10) + 1;
                next();
            },
            (json, next)=>{
                pass = (pass * 10) + 2;
                next();
            },
            (json, next)=>{
                pass = (pass * 10) + 3;
            }        
        );

        var json = JObject.Parse("{'action' : 'test'}");
        ahnd.Process(json);

        Assert.AreEqual(123, pass);
    }    

    [TestMethod]    
    public void JSON_Is_Mutable() {
        int pass = 0;
        var ahnd = new ActionHandler();        

        ahnd.Use("test", 
            (json, next)=>{
                json.Add("field", "value");
                next();
            },
            (json, next)=>{
                var value = (string)json["field"];
                Assert.AreEqual("value", value);
                next();
            } 
        );

        var json = JObject.Parse("{'action' : 'test'}");
        ahnd.Process(json);
        var value = (string)json["field"];
        Assert.AreEqual("value", value);        
    }    

    [TestMethod]    
    public void Run_Chain_Twice() {
        int pass = 0;
        var ahnd = new ActionHandler();        

        ahnd.Use("test", 
            (json, next)=>{
                pass = (pass * 10) + 1;
                next();
            },
            (json, next)=>{
                pass = (pass * 10) + 2;
                next();
            },
            (json, next)=>{
                pass = (pass * 10) + 3;
            }        
        );

        var json = JObject.Parse("{'action' : 'test'}");
        ahnd.Process(json);
        ahnd.Process(json);

        Assert.AreEqual(123123, pass);
    }        
}