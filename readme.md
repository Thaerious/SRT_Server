Creating & Staring a Server
===========================

Create a server which instantiates a new MyRouter object when a client connects.
The MyRouter class is responsible for managing Annotated classes.

```
public static void Main(string[] args){
    System.Console.WriteLine("Starting Server");
    Server<MyRouter> server = new Server<MyRouter>();
    server.Connect(port: 7000);
    server.Listen();
}
```

Create the Router Class
=======================

This class uses it's self for Annotated Endpoints.
The Router Class is unique for every connection.

```
using System.Net;
using System.Net.Sockets;
using frar.clientserver;

class MyRouter : ThreadedRouter{
    public MyRouter(){
        System.Console.WriteLine($"");
        this.AddHandler(this);
    }

    [OnConnect]
    public void Connect(Connection connection){
        System.Console.WriteLine($"srv> client connected");
    }

    [Route]
    public void Message(string msg){
        System.Console.WriteLine($"msg> {msg}");
    }
}
```

Creating & a Client
===========================

A Client is connected by creating a Socket and passing it to a Connection.
The Connection class will manage communication between the client and server. 
The Connection.ConnectTo generator method encapsulates this process.

```
using System.Net;
using System.Net.Sockets;
using frar.clientserver;

public class Client {

    static void Main(string[] args) {
        Client client = new Client();
        client.Connect();
    }

    private Connection? connection;

    public void Connect(string ip = "127.0.0.1", int port = 7000) {
        this.connection = Connection.ConnectTo(ip, port);
    }
}
```

Sending a Packet
================
    
Messages are sent with the class packet.
The 'action' is declared with the constructor and must match the name of the router.
Since we used a default 'Route' annotation, it's the method name, case insensative.

Declare the packet inline:

    connection.Write(new Packet("message").Set("msg", "hello world"));

Build the packet up on multiple lines:

    var packet = new Packet("message");
    packet["msg"] = "hello world";
    connection.Write(packet);

Explicitly Reading a Packet
===========================

Packets can be read explicitly.  This method blocks until a packet is read.
If a packet is read explicitly it will not trigger the router.

    Packet packet = connection.Read();

Declaring a Route
=================

Routes are declared with the 'Route' annotation.
Every route has a 'Rule' value, this is used to match with a packets 'action'.
The Rule is a Regular-Expression, that by default matches with the case-insenstative
name of the method is is annotating.  To make a rule case-insensative prefix the regex with '(?i)'.

**Methods actiing as routes can only have primitive arguments (value types).  No arrays, no objects. **

See https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types.

To override the default, specify a 'Rule' value:

```
// Matches all packet actions starting with 'api-'.
[Route(Rule = "api-.*")]
public void AllAPICalls(string value){
    System.Console.WriteLine($"api> {value}");
}
```

Multiple routes can match the same packet.  To control the order they trigger in, declare an 'Index' value on the route.  Negative values are permitted, the default value is 0.

```
// Runs first on all all packet actions starting with 'api-'.
[Route(Rule = "api-.*", Index = 1)]
public void AllAPICalls1(string value){
    System.Console.WriteLine($"api 1st> {value}");
}

// Runs second on all all packet actions starting with 'api-'.
[Route(Rule = "api-.*", Index = 2)]
public void AllAPICalls2(string value){
    System.Console.WriteLine($"api 2nd> {value}");
}    
```

## Manipulating the Packet

The packet can be passed to a route as a method argument.  To do so annotate it with '[Req]' (stands for REQuest packet).  Changes made to the packet will be available in subsequent routes handling the same packet.

```
[Route]
public void PrintPacket([Req] Packet packet) {
    System.Console.WriteLine(packet);
}
```

Development Notes
=================

Generate api docs
/JSONClientServer$ doxygen

Run Tests
/JSONClientServerTest$ dotnet test