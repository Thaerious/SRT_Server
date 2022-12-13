using Microsoft.VisualStudio.TestTools.UnitTesting;
using frar.clientserver;
using System.Threading;
using System.Diagnostics;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace frar.clientserver.test;


/// <summary>
/// Test that the connection class can read and write to a socket.
/// A local socket is used between two classes.
/// 
/// To test just this class:
/// dotnet test --filter ClassName=frar.clientserver.test.ConnectionTest
/// </summary>
[TestClass]
public class ConnectionTest {
    private static Socket? server = null;
    private static IPHostEntry? ipHostEntry;
    private static IPAddress? ipAddress;
    private static IPEndPoint? ipEndPoint;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext){
        if (server != null) return;
        ipHostEntry = Dns.GetHostEntry("127.0.0.1");
        ipAddress = ipHostEntry.AddressList[0];
        ipEndPoint = new IPEndPoint(ipAddress, 12345);

        server = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(ipEndPoint);
        server.Listen(12345);
    }

    public Tuple<Connection, Connection> Connect() {
        Connection? c1 = null, c2 = null;

        Task t1 = Task.Run(() => {
            Socket socket = server.Accept();
            c1 = new Connection(socket);
        });

        System.Threading.Thread.Sleep(10);

        var socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ipEndPoint);
        c2 = new Connection(socket);

        t1.Wait();
        return new Tuple<Connection, Connection>(c1!, c2);
    }

    /// <summary>
    /// Write then read an empty packet.
    /// 1) The reading connection will get a packet with the same value.
    /// 2) The packets will not be the same (a new packet is returned by #Read).
    /// </summary>
    [TestMethod]
    public void Read_Write() {
        Tuple<Connection, Connection> connections = Connect();
        Packet pWrite = new Packet("empty");
        connections.Item1.Write(pWrite);
        Packet pRead = connections.Item2.Read();
        Assert.AreEqual("empty", pRead.Action);
        Assert.AreNotEqual(pWrite, pRead);

        connections.Item2.Shutdown();
        connections.Item1.Shutdown();
    }

    /// <summary>
    /// Write then read a packet multiple times.
    /// 1) The reading connection will get a packet with the same value (twice).
    /// </summary>
    [TestMethod]
    public void Read_Write_Multiple_1() {
        Tuple<Connection, Connection> connections = Connect();
        Packet pWrite = new Packet("empty");

        connections.Item1.Write(pWrite);
        Packet pRead1 = connections.Item2.Read();
        Assert.AreEqual("empty", pRead1.Action);
        Assert.AreNotEqual(pWrite, pRead1);

        connections.Item1.Write(pWrite);
        Packet pRead2 = connections.Item2.Read();
        Assert.AreEqual("empty", pRead2.Action);
        Assert.AreNotEqual(pWrite, pRead2);

        connections.Item2.Shutdown();
        connections.Item1.Shutdown();
    }

    /// <summary>
    /// Write twice then read twice.
    /// 1) The reading connection will get a packet with the same value (twice).
    /// </summary>
    [TestMethod]
    public void Read_Write_Multiple_2() {
        Tuple<Connection, Connection> connections = Connect();
        Packet pWrite = new Packet("empty");

        connections.Item1.Write(pWrite);
        connections.Item1.Write(pWrite);

        Packet pRead1 = connections.Item2.Read();
        Assert.AreEqual("empty", pRead1.Action);
        Assert.AreNotEqual(pWrite, pRead1);

        Packet pRead2 = connections.Item2.Read();
        Assert.AreEqual("empty", pRead2.Action);
        Assert.AreNotEqual(pWrite, pRead2);

        connections.Item2.Shutdown();
        connections.Item1.Shutdown();
    }        
}