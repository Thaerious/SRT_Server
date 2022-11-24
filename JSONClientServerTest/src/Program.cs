using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using frar.JSONServer;

Server<Connection> server = new Server<Connection>().Connect(7000);
Client client = new Client();

client.Connect(ip: "127.0.0.1", port: 7000);

Connection connection = server.AcceptNext();

JObject source = new JObject(
    new JProperty("pub", 4)
);

connection.WriteJSON(source);

JObject jObject = client.ReadJSON();
Console.WriteLine(jObject.ToString(Formatting.None));

server.Stop();
connection.Close();