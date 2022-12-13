using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.clientserver;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace frar.clientserver.test;

[TestClass]
public class PacketTest {

    [TestMethod]
    public void No_Parameters() {
        Packet packet = new Packet("x");
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{}}";
        Debug.WriteLine(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Single_Int_Parameter() {
        Packet packet = new Packet("x").Set("a", 1);
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":1}}";
        Debug.WriteLine(actual);
        Assert.AreEqual(expected, actual);
    }    

    [TestMethod]
    public void Single_String_Parameter() {
        Packet packet = new Packet("x").Set("a", "b");
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":""b""}}";
        Debug.WriteLine(actual);
        Assert.AreEqual(expected, actual);
    }   

    [TestMethod]
    public void Multiple_Parameters() {
        Packet packet = new Packet("x").Set("a", "b").Set("b", 1);
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":""b"",""b"":1}}";
        Debug.WriteLine(actual);
        Assert.AreEqual(expected, actual);
    }
    
    [TestMethod]
    public void Overwrite_Parameter() {
        Packet packet = new Packet("x").Set("a", "b").Set("a", 1);
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":1}}";
        Debug.WriteLine(actual);
        Assert.AreEqual(expected, actual);
    }
}