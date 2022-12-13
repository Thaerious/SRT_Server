using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.clientserver;
using System.Net;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace frar.clientserver.test;

[TestClass]
public class PacketTest {

    [TestMethod]
    public void No_Parameters() {
        Packet packet = new Packet("x");
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{}}";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Single_Int_Parameter() {
        Packet packet = new Packet("x").Set("a", 1);
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":1}}";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Int_Parameter_Recover() {
        Packet packet = new Packet("x").Set("i", 1);
        Packet rPacket = Packet.FromString(packet.ToString());

        int value = rPacket.Get<int>("i");
        Assert.AreEqual(typeof(int), value.GetType());
    }

    [TestMethod]
    public void Single_String_Parameter() {
        Packet packet = new Packet("x").Set("a", "b");
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":""b""}}";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Multiple_Parameters() {
        Packet packet = new Packet("x").Set("a", "b").Set("b", 1);
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":""b"",""b"":1}}";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Overwrite_Parameter() {
        Packet packet = new Packet("x").Set("a", "b").Set("a", 1);
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""a"":1}}";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Object_Parameter() {
        Packet packet = new Packet("x").Set("myobj", new SimpleObject());
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""myobj"":{""a"":0,""b"":0}}}";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Object_Parameter_Recover() {
        Packet packet = new Packet("x").Set("myobj", new SimpleObject());
        Packet rPacket = Packet.FromString(packet.ToString());

        SimpleObject value = rPacket.Get<SimpleObject>("myobj");
        Assert.AreEqual(typeof(SimpleObject), value.GetType());
    }

    [TestMethod]
    public void Array_Parameter() {
        Packet packet = new Packet("x").Set("ar", new int[] { 1, 2, 3 });
        string actual = packet.ToString(Formatting.None);
        string expected = @"{""Action"":""x"",""Parameters"":{""ar"":[1,2,3]}}";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Array_Parameter_Recover() {
        Packet packet = new Packet("x").Set("ar", new int[] { 1, 2, 3 });
        Packet rPacket = Packet.FromString(packet.ToString());

        Debug.WriteLine(rPacket);
        int[] value = rPacket.Get<int[]>("ar");
        Assert.AreEqual(1, value[0]);
    }    
}

public class SimpleObject {
    public int a;
    public int b;

    public SimpleObject() { }

    public SimpleObject(int a, int b) { this.a = a;  this.b = b; }
}